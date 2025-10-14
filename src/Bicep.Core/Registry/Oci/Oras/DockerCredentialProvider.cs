// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Utils;
using OrasProject.Oras.Registry.Remote.Auth;

namespace Bicep.Core.Registry.Oci.Oras;

public interface IDockerCredentialHelperInvoker
{
    Task<Credential?> InvokeAsync(string helperName, string registry, CancellationToken cancellationToken);
}

internal sealed class ProcessDockerCredentialHelperInvoker : IDockerCredentialHelperInvoker
{
    public async Task<Credential?> InvokeAsync(string helperName, string registry, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(helperName))
        {
            return null;
        }

        var executableName = GetExecutableName(helperName);

        try
        {
            using var process = Process.Start(new ProcessStartInfo(executableName, "get")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            });

            if (process is null)
            {
                return null;
            }

            var stdoutTask = process.StandardOutput.ReadToEndAsync();
            var stderrTask = process.StandardError.ReadToEndAsync();

            using (var writer = process.StandardInput)
            {
                await writer.WriteAsync(registry).ConfigureAwait(false);
                await writer.WriteAsync(System.Environment.NewLine).ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);
            }

            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

            var stdout = await stdoutTask.ConfigureAwait(false);
            _ = await stderrTask.ConfigureAwait(false);

            if (process.ExitCode != 0 || string.IsNullOrWhiteSpace(stdout))
            {
                return null;
            }

            using var document = JsonDocument.Parse(stdout);
            var root = document.RootElement;

            if (root.TryGetProperty("IdentityToken", out var identityTokenElement) && identityTokenElement.ValueKind == JsonValueKind.String)
            {
                var identityToken = identityTokenElement.GetString();
                if (!string.IsNullOrEmpty(identityToken))
                {
                    return new Credential(AccessToken: identityToken);
                }
            }

            string? username = null;
            string? password = null;

            if (root.TryGetProperty("Username", out var usernameElement) && usernameElement.ValueKind == JsonValueKind.String)
            {
                username = usernameElement.GetString();
            }

            if (root.TryGetProperty("Secret", out var secretElement) && secretElement.ValueKind == JsonValueKind.String)
            {
                password = secretElement.GetString();
            }

            if (!string.IsNullOrEmpty(username) && password is not null)
            {
                return new Credential(username, password);
            }

            return null;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            return null;
        }
    }

    private static string GetExecutableName(string helperName)
    {
        var executable = $"docker-credential-{helperName}";
        return OperatingSystem.IsWindows() ? $"{executable}.exe" : executable;
    }
}

/// <summary>
/// Provides Docker-compatible credentials for the ORAS client by reading the user's Docker config file
/// and common environment variables.
/// </summary>
public class DockerCredentialProvider : ICredentialProvider
{
    private readonly IEnvironment environment;
    private readonly IFileSystem fileSystem;
    private readonly IDockerCredentialHelperInvoker helperInvoker;

    public DockerCredentialProvider(IEnvironment environment, IFileSystem fileSystem, IDockerCredentialHelperInvoker? helperInvoker = null)
    {
        this.environment = environment;
        this.fileSystem = fileSystem;
        this.helperInvoker = helperInvoker ?? new ProcessDockerCredentialHelperInvoker();
    }

    public async Task<Credential> ResolveCredentialAsync(string hostname, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var credential = await ResolveFromDockerConfigAsync(hostname, cancellationToken).ConfigureAwait(false)
            ?? ResolveFromEnvironment();

        return credential ?? default;
    }

    private Credential? ResolveFromEnvironment()
    {
        var username = environment.GetVariable("DOCKER_USERNAME");
        var password = environment.GetVariable("DOCKER_PASSWORD");

        if (!string.IsNullOrEmpty(username) && password is not null)
        {
            return new Credential(username, password);
        }

        return null;
    }

    private async Task<Credential?> ResolveFromDockerConfigAsync(string hostname, CancellationToken cancellationToken)
    {
        var configPath = GetDockerConfigPath();
        if (configPath is null || !fileSystem.File.Exists(configPath))
        {
            return null;
        }

        try
        {
            using var stream = fileSystem.File.OpenRead(configPath);
            using var document = JsonDocument.Parse(stream);

            if (document.RootElement.TryGetProperty("auths", out var auths) && auths.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in auths.EnumerateObject())
                {
                    if (!MatchesRegistry(property.Name, hostname))
                    {
                        continue;
                    }

                    var credential = TryReadCredential(property.Value);
                    if (credential is not null)
                    {
                        return credential;
                    }
                }
            }

            if (document.RootElement.TryGetProperty("credHelpers", out var credHelpers) && credHelpers.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in credHelpers.EnumerateObject())
                {
                    if (property.Value.ValueKind != JsonValueKind.String || !MatchesRegistry(property.Name, hostname))
                    {
                        continue;
                    }

                    var helperName = property.Value.GetString();
                    if (string.IsNullOrWhiteSpace(helperName))
                    {
                        continue;
                    }

                    var helperCredential = await helperInvoker.InvokeAsync(helperName!, property.Name, cancellationToken).ConfigureAwait(false);
                    if (helperCredential.HasValue && !helperCredential.Value.IsEmpty())
                    {
                        return helperCredential.Value;
                    }
                }
            }

            if (document.RootElement.TryGetProperty("credsStore", out var credsStoreElement) && credsStoreElement.ValueKind == JsonValueKind.String)
            {
                var helperName = credsStoreElement.GetString();
                if (!string.IsNullOrWhiteSpace(helperName))
                {
                    var helperCredential = await helperInvoker.InvokeAsync(helperName!, hostname, cancellationToken).ConfigureAwait(false);
                    if (helperCredential.HasValue && !helperCredential.Value.IsEmpty())
                    {
                        return helperCredential.Value;
                    }
                }
            }
        }
        catch (IOException)
        {
            // Ignore IO issues and fall back to other providers.
        }
        catch (JsonException)
        {
            // Ignore invalid docker config formats.
        }
        catch (UnauthorizedAccessException)
        {
            // Ignore permission issues.
        }
        catch (FormatException)
        {
            // Ignore invalid base64 entries.
        }

        return null;
    }

    private Credential? TryReadCredential(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        if (element.TryGetProperty("identitytoken", out var identityTokenElement)
            && identityTokenElement.ValueKind == JsonValueKind.String
            && !string.IsNullOrEmpty(identityTokenElement.GetString()))
        {
            return new Credential(AccessToken: identityTokenElement.GetString());
        }

        if (element.TryGetProperty("username", out var usernameElement) &&
            element.TryGetProperty("password", out var passwordElement) &&
            usernameElement.ValueKind == JsonValueKind.String &&
            passwordElement.ValueKind == JsonValueKind.String)
        {
            var username = usernameElement.GetString();
            var password = passwordElement.GetString();
            if (!string.IsNullOrEmpty(username) && password is not null)
            {
                return new Credential(username, password);
            }
        }

        if (element.TryGetProperty("auth", out var authElement) && authElement.ValueKind == JsonValueKind.String)
        {
            var authValue = authElement.GetString();
            if (!string.IsNullOrEmpty(authValue))
            {
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(authValue));
                var parts = decoded.Split(':', 2);
                if (parts.Length == 2)
                {
                    return new Credential(parts[0], parts[1]);
                }
            }
        }

        return null;
    }

    private string? GetDockerConfigPath()
    {
        var dockerConfigDir = environment.GetVariable("DOCKER_CONFIG");
        if (string.IsNullOrWhiteSpace(dockerConfigDir))
        {
            var home = environment.GetVariable("HOME") ?? environment.GetVariable("USERPROFILE");
            if (string.IsNullOrWhiteSpace(home))
            {
                return null;
            }

            dockerConfigDir = fileSystem.Path.Combine(home, ".docker");
        }

        return fileSystem.Path.Combine(dockerConfigDir, "config.json");
    }

    private static bool MatchesRegistry(string entry, string hostname)
    {
        if (string.Equals(entry, hostname, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (TryNormalize(entry, out var entryHost, out var entryPort) &&
            TryNormalize(hostname, out var requestedHost, out var requestedPort))
        {
            if (!string.Equals(entryHost, requestedHost, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (entryPort is null || requestedPort is null)
            {
                return true;
            }

            return entryPort == requestedPort;
        }

        return false;
    }

    private static bool TryNormalize(string value, out string host, out int? port)
    {
        host = value;
        port = null;

        if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            host = uri.Host;
            port = ResolvePort(uri);
            return true;
        }

        if (Uri.TryCreate($"https://{value.TrimEnd('/')}", UriKind.Absolute, out uri))
        {
            host = uri.Host;
            port = ResolvePort(uri);
            return true;
        }

        var trimmed = value.TrimEnd('/');
        var colonIndex = trimmed.LastIndexOf(':');
        if (colonIndex > 0 && colonIndex < trimmed.Length - 1 && int.TryParse(trimmed[(colonIndex + 1)..], out var parsedPort))
        {
            host = trimmed[..colonIndex];
            port = parsedPort;
            return true;
        }

        host = trimmed;
        return true;
    }

    private static int? ResolvePort(Uri uri)
    {
        if (!uri.IsAbsoluteUri)
        {
            return null;
        }

        if (!uri.IsDefaultPort)
        {
            return uri.Port;
        }

        return uri.Scheme.ToLowerInvariant() switch
        {
            "http" => 80,
            "https" => 443,
            _ => null,
        };
    }
}
