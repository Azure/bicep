// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Bicep.Local.Rpc;

/// <summary>
/// Owns the ephemeral identities for one local extension launch.
/// </summary>
public sealed class LocalExtensionAuthentication : IDisposable
{
    private const int BootstrapVersion = 1;
    private const int MaximumCertificateBytes = 16 * 1024;
    private const int MaximumKeyNameBytes = 256;

    // TLS authentication key purposes: https://www.rfc-editor.org/rfc/rfc5280.html#section-4.2.1.12
    private const string ServerAuthenticationOid = "1.3.6.1.5.5.7.3.1";
    private const string ClientAuthenticationOid = "1.3.6.1.5.5.7.3.2";

    private readonly CngKey? ownedServerKey;
    private readonly CngKey? ownedClientKey;
    private int disposed;

    private LocalExtensionAuthentication(X509Certificate2 serverCertificate, X509Certificate2 clientCertificate,
        CngKey? ownedServerKey = null, CngKey? ownedClientKey = null)
    {
        ServerCertificate = serverCertificate;
        ClientCertificate = clientCertificate;
        this.ownedServerKey = ownedServerKey;
        this.ownedClientKey = ownedClientKey;
    }

    public X509Certificate2 ServerCertificate { get; }

    public X509Certificate2 ClientCertificate { get; }

    public static LocalExtensionAuthentication Create()
    {
        if (OperatingSystem.IsWindows())
        {
            return CreateWindows();
        }

        var server = CreateCertificate("Bicep local extension", ServerAuthenticationOid);
        try
        {
            return new(server, CreateCertificate("Bicep local client", ClientAuthenticationOid));
        }
        catch
        {
            server.Dispose();
            throw;
        }
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static LocalExtensionAuthentication CreateWindows()
    {
        // Schannel needs named CNG keys. The parent owns both keys and removes them during launch cleanup.
        var serverKey = CngKey.Create(CngAlgorithm.ECDsaP256, $"Bicep-{Guid.NewGuid():N}");
        CngKey? clientKey = null;
        X509Certificate2? server = null;
        try
        {
            clientKey = CngKey.Create(CngAlgorithm.ECDsaP256, $"Bicep-{Guid.NewGuid():N}");
            using var serverAlgorithm = new ECDsaCng(serverKey);
            using var clientAlgorithm = new ECDsaCng(clientKey);
            server = CreateCertificate("Bicep local extension", ServerAuthenticationOid, serverAlgorithm);
            var client = CreateCertificate("Bicep local client", ClientAuthenticationOid, clientAlgorithm);
            return new(server, client, serverKey, clientKey);
        }
        catch
        {
            server?.Dispose();
            DeleteWindowsKeys(serverKey, clientKey);
            throw;
        }
    }

    /// <summary>
    /// Sends the server identity and public client identity, never the client private key.
    /// Windows uses a parent-owned CNG key reference; other platforms use an in-memory PKCS#12 key.
    /// The caller must close the child's redirected stdin after writing.
    /// </summary>
    public async Task WriteBootstrapAsync(Stream output, CancellationToken cancellationToken)
    {
        var keyName = OperatingSystem.IsWindows() && ownedServerKey is { } key
            ? Encoding.UTF8.GetBytes(key.KeyName ?? throw new InvalidOperationException("The server key must have a name."))
            : [];
        var server = keyName.Length > 0 ? ServerCertificate.RawData : ServerCertificate.Export(X509ContentType.Pfx);
        try
        {
            var client = ClientCertificate.RawData;
            var header = new byte[16];
            BinaryPrimitives.WriteInt32BigEndian(header, BootstrapVersion);
            BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(4), server.Length);
            BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(8), client.Length);
            BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(12), keyName.Length);
            await output.WriteAsync(header, cancellationToken);
            await output.WriteAsync(server, cancellationToken);
            await output.WriteAsync(client, cancellationToken);
            await output.WriteAsync(keyName, cancellationToken);
            await output.FlushAsync(cancellationToken);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(server);
        }
    }

    /// <summary>
    /// Reads one bounded bootstrap frame and requires EOF within the startup deadline.
    /// </summary>
    public static async Task<LocalExtensionAuthentication> ReadBootstrapAsync(Stream input, CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(30));
        var header = new byte[16];
        await input.ReadExactlyAsync(header, timeout.Token).AsTask().WaitAsync(timeout.Token);
        var version = BinaryPrimitives.ReadInt32BigEndian(header);
        var serverLength = BinaryPrimitives.ReadInt32BigEndian(header.AsSpan(4));
        var clientLength = BinaryPrimitives.ReadInt32BigEndian(header.AsSpan(8));
        var keyNameLength = BinaryPrimitives.ReadInt32BigEndian(header.AsSpan(12));
        if (version != BootstrapVersion ||
            serverLength is <= 0 or > MaximumCertificateBytes ||
            clientLength is <= 0 or > MaximumCertificateBytes ||
            keyNameLength is < 0 or > MaximumKeyNameBytes)
        {
            throw new InvalidOperationException("Invalid local extension authentication bootstrap.");
        }

        var serverBytes = new byte[serverLength];
        Task? serverRead = null;
        try
        {
            var clientBytes = new byte[clientLength];
            var keyNameBytes = new byte[keyNameLength];
            serverRead = input.ReadExactlyAsync(serverBytes, timeout.Token).AsTask();
            await serverRead.WaitAsync(timeout.Token);
            await input.ReadExactlyAsync(clientBytes, timeout.Token).AsTask().WaitAsync(timeout.Token);
            await input.ReadExactlyAsync(keyNameBytes, timeout.Token).AsTask().WaitAsync(timeout.Token);
            var trailingBytes = await input.ReadAsync(new byte[1], timeout.Token).AsTask().WaitAsync(timeout.Token);
            if (trailingBytes != 0)
            {
                throw new InvalidOperationException("Unexpected data after local extension authentication bootstrap.");
            }

            var server = LoadServerCertificate(serverBytes, keyNameBytes);
            try
            {
                if (!server.HasPrivateKey)
                {
                    throw new InvalidOperationException("The local extension server identity requires a private key.");
                }

                return new(server, X509CertificateLoader.LoadCertificate(clientBytes));
            }
            catch
            {
                server.Dispose();
                throw;
            }
        }
        finally
        {
            if (serverRead is { IsCompleted: false })
            {
                // WaitAsync can return before the stream stops writing into the buffer.
                _ = serverRead.ContinueWith(completed =>
                {
                    _ = completed.Exception;
                    CryptographicOperations.ZeroMemory(serverBytes);
                }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            }
            else
            {
                _ = serverRead?.Exception;
                CryptographicOperations.ZeroMemory(serverBytes);
            }
        }
    }

    public static bool MatchesCertificate(X509Certificate? actual, X509Certificate2 expected)
        => actual is not null &&
            CryptographicOperations.FixedTimeEquals(actual.GetRawCertData(), expected.RawData);

    private static X509Certificate2 LoadServerCertificate(byte[] serverBytes, byte[] keyNameBytes)
    {
        if (OperatingSystem.IsWindows() && keyNameBytes.Length > 0)
        {
            using var key = CngKey.Open(Encoding.UTF8.GetString(keyNameBytes));
            using var algorithm = new ECDsaCng(key);
            using var certificate = X509CertificateLoader.LoadCertificate(serverBytes);
            return certificate.CopyWithPrivateKey(algorithm);
        }

        if (OperatingSystem.IsWindows() || keyNameBytes.Length > 0)
        {
            throw new InvalidOperationException("The local extension bootstrap does not match the current platform.");
        }

        // macOS requires a runtime-managed temporary keychain rather than an ephemeral PKCS#12 import.
        var storageFlags = OperatingSystem.IsMacOS()
            ? X509KeyStorageFlags.DefaultKeySet
            : X509KeyStorageFlags.EphemeralKeySet;
        return X509CertificateLoader.LoadPkcs12(serverBytes, null, storageFlags);
    }

    private static X509Certificate2 CreateCertificate(string name, string usage)
    {
        using var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        return CreateCertificate(name, usage, key);
    }

    private static X509Certificate2 CreateCertificate(string name, string usage, ECDsa key)
    {
        var request = new CertificateRequest($"CN={name}", key, HashAlgorithmName.SHA256);
        request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, true));
        request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true));
        request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension([new Oid(usage)], true));
        var names = new SubjectAlternativeNameBuilder();
        names.AddDnsName("localhost");
        request.CertificateExtensions.Add(names.Build());
        var now = DateTimeOffset.UtcNow;
        return request.CreateSelfSigned(now.AddMinutes(-5), now.AddDays(1));
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed, 1) != 0)
        {
            return;
        }

        ServerCertificate.Dispose();
        ClientCertificate.Dispose();
        if (OperatingSystem.IsWindows())
        {
            DeleteWindowsKeys(ownedServerKey, ownedClientKey);
        }
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static void DeleteWindowsKeys(CngKey? server, CngKey? client)
    {
        try
        {
            try
            {
                server?.Delete();
            }
            finally
            {
                server?.Dispose();
            }
        }
        finally
        {
            try
            {
                client?.Delete();
            }
            finally
            {
                client?.Dispose();
            }
        }
    }
}
