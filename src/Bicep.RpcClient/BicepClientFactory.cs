// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Bicep.RpcClient.Helpers;

namespace Bicep.RpcClient;

public class BicepClientFactory(HttpClient httpClient) : IBicepClientFactory
{
    private static readonly string DefaultInstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".bicep", "bin");
    private static readonly OSPlatform CurrentOsPlatform = BicepInstaller.DetectCurrentOSPlatform();
    private static readonly Architecture CurrentArchitecture = RuntimeInformation.OSArchitecture;

    private readonly HttpClient httpClient = httpClient;

    public async Task<IBicepClient> DownloadAndInitialize(BicepClientConfiguration configuration, string? bicepVersion, CancellationToken cancellationToken)
    {
        var osPlatform = configuration.OsPlatform ?? CurrentOsPlatform;
        var architecture = configuration.Architecture ?? CurrentArchitecture;
        var baseDownloadPath = configuration.InstallPath ?? DefaultInstallPath;

        var versionTag = bicepVersion is { } ? $"v{bicepVersion}" : await BicepInstaller.ResolveBicepVersionTagAsync(httpClient, bicepVersion, cancellationToken).ConfigureAwait(false);

        var versionedPath = Path.Combine(baseDownloadPath, versionTag);
        var bicepCliName = osPlatform.Equals(OSPlatform.Windows) ? "bicep.exe" : "bicep";
        var bicepCliPath = Path.Combine(versionedPath, bicepCliName);

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

        return await InitializeFromPath(bicepCliPath, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IBicepClient> InitializeFromPath(string bicepCliPath, CancellationToken cancellationToken)
    {
        return await BicepClient.Initialize(bicepCliPath, cancellationToken).ConfigureAwait(false);
    }
}
