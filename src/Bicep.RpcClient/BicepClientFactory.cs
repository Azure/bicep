// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Bicep.RpcClient.Helpers;
using Bicep.RpcClient.SemanticVersioning;

namespace Bicep.RpcClient;

/// <summary>
/// A factory that creates and manages Bicep clients, handling the download and installation of the Bicep CLI if necessary.
/// </summary>
public class BicepClientFactory(HttpClient? httpClient = null) : IBicepClientFactory
{
    private static readonly string DefaultInstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".bicep", "bin");
    private static readonly OSPlatform CurrentOsPlatform = BicepInstaller.DetectCurrentOSPlatform();
    private static readonly Architecture CurrentArchitecture = RuntimeInformation.OSArchitecture;
    private readonly BicepInstaller installer = new(httpClient ?? new());

    /// <inheritdoc/>
    public async Task<IBicepClient> Initialize(BicepClientConfiguration configuration, CancellationToken cancellationToken)
    {
        BicepClientConfiguration.Validate(configuration);

        if (configuration.ExistingCliPath is not { } bicepCliPath)
        {
            bicepCliPath = await Download(configuration, cancellationToken).ConfigureAwait(false);
        }

        return configuration.ConnectionMode switch
        {
            BicepConnectionMode.NamedPipe => await BicepClient.InitializeWithNamedPipe(bicepCliPath, configuration.ConnectionTimeout, cancellationToken).ConfigureAwait(false),
            BicepConnectionMode.Stdio => await BicepClient.InitializeWithStdio(bicepCliPath, cancellationToken).ConfigureAwait(false),
            _ => throw new NotSupportedException($"Unsupported connection mode: {configuration.ConnectionMode}")
        };
    }

    internal async Task<string> Download(BicepClientConfiguration configuration, CancellationToken cancellationToken)
    {
        var osPlatform = configuration.OsPlatform ?? CurrentOsPlatform;
        var architecture = configuration.Architecture ?? CurrentArchitecture;
#pragma warning disable CS0618 // Type or member is obsolete
        var baseDownloadPath = configuration.InstallBasePath ?? configuration.InstallPath ?? DefaultInstallPath;
#pragma warning restore CS0618 // Type or member is obsolete
        var versionTag = await ResolveVersionTag(configuration, cancellationToken).ConfigureAwait(false);

        var bicepCliName = osPlatform.Equals(OSPlatform.Windows) ? "bicep.exe" : "bicep";
        // E.g. ~/.bicep/bin/v0.37.4/bicep
        var bicepCliPath = Path.Combine(baseDownloadPath, versionTag, bicepCliName);

        if (!File.Exists(bicepCliPath))
        {
            await installer.DownloadAndInstallBicepCliAsync(
                bicepCliPath: bicepCliPath,
                targetOSPlatform: osPlatform,
                targetOSArchitecture: architecture,
                versionTag: versionTag,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        return bicepCliPath;
    }

    /// <summary>
    /// Lists published release versions without the leading "v". The releases endpoint returns
    /// only recent releases, not the complete release history. Successful results are cached
    /// for the lifetime of this factory.
    /// </summary>
    internal Task<ImmutableArray<string>> ListReleases(CancellationToken cancellationToken = default)
        => installer.ListBicepVersions(cancellationToken);

    internal async Task<string> ResolveVersionTag(BicepClientConfiguration configuration, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var resolved = configuration.SourceFilePath is { } sourceFilePath
            ? await BicepConfigResolver.ResolveConfigChain(sourceFilePath, cancellationToken).ConfigureAwait(false)
            : new ResolvedConfigChain(null, null);

        if (configuration.BicepVersion is { } explicitVersion)
        {
            if (resolved.BicepVersionRange is { } constraint &&
                (!SemanticVersion.TryParse(explicitVersion, out var parsedVersion) ||
                 parsedVersion is null ||
                 !constraint.IsSatisfiedBy(parsedVersion)))
            {
                throw new InvalidOperationException($"Bicep version '{explicitVersion}' does not satisfy constraint '{constraint}' in '{resolved.DeclaringConfigPath}'.");
            }

            return $"v{explicitVersion}";
        }

        if (resolved.BicepVersionRange is not { } range)
        {
            return await installer.GetLatestBicepVersion(cancellationToken).ConfigureAwait(false);
        }

        if (range.IsExactVersion && range.LowerBound is { } exact)
        {
            return $"v{exact.Version}";
        }

        SemanticVersion? highestVersion = null;
        foreach (var release in await ListReleases(cancellationToken).ConfigureAwait(false))
        {
            if (SemanticVersion.TryParse(release, out var version) &&
                version is not null &&
                range.IsSatisfiedBy(version) &&
                (highestVersion is null || version.CompareTo(highestVersion) > 0))
            {
                highestVersion = version;
            }
        }

        return highestVersion is not null
            ? $"v{highestVersion}"
            : throw new InvalidOperationException($"No published Bicep CLI release satisfies constraint '{range}' in '{resolved.DeclaringConfigPath}'.");
    }

    [Obsolete($"Use {nameof(Initialize)} with a {nameof(BicepClientConfiguration)} that has {nameof(BicepClientConfiguration.ExistingCliPath)} set instead.")]
    public Task<IBicepClient> InitializeFromPath(string bicepCliPath, CancellationToken cancellationToken)
        => Initialize(new() { ExistingCliPath = bicepCliPath }, cancellationToken);

    [Obsolete($"Use {nameof(Initialize)} instead.")]
    public Task<IBicepClient> DownloadAndInitialize(BicepClientConfiguration configuration, CancellationToken cancellationToken)
        => Initialize(configuration, cancellationToken);
}
