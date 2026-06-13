// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Bicep.RpcClient.Helpers;

namespace Bicep.RpcClient;

public class BicepClientFactory(HttpClient? httpClient = null) : IBicepClientFactory
{
    private static readonly string DefaultInstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".bicep", "bin");
    private static readonly OSPlatform CurrentOsPlatform = BicepInstaller.DetectCurrentOSPlatform();
    private static readonly Architecture CurrentArchitecture = RuntimeInformation.OSArchitecture;
    private readonly HttpClient httpClient = httpClient ?? new();

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
        var versionTag = configuration.BicepVersion is { } ?
            $"v{configuration.BicepVersion}" :
            await BicepInstaller.GetLatestBicepVersion(httpClient, cancellationToken).ConfigureAwait(false);

        var bicepCliName = osPlatform.Equals(OSPlatform.Windows) ? "bicep.exe" : "bicep";
        // E.g. ~/.bicep/bin/v0.37.4/bicep
        var bicepCliPath = Path.Combine(baseDownloadPath, versionTag, bicepCliName);

        if (!File.Exists(bicepCliPath))
        {
            await BicepInstaller.DownloadAndInstallBicepCliAsync(
                httpClient: httpClient,
                bicepCliPath: bicepCliPath,
                targetOSPlatform: osPlatform,
                targetOSArchitecture: architecture,
                versionTag: versionTag,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        return bicepCliPath;
    }

    [Obsolete($"Use {nameof(Initialize)} with a {nameof(BicepClientConfiguration)} that has {nameof(BicepClientConfiguration.ExistingCliPath)} set instead.")]
    public Task<IBicepClient> InitializeFromPath(string bicepCliPath, CancellationToken cancellationToken)
        => Initialize(new() { ExistingCliPath = bicepCliPath }, cancellationToken);

    [Obsolete($"Use {nameof(Initialize)} instead.")]
    public Task<IBicepClient> DownloadAndInitialize(BicepClientConfiguration configuration, CancellationToken cancellationToken)
        => Initialize(configuration, cancellationToken);
}
