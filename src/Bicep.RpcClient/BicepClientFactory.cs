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

public class BicepClientFactory(HttpClient httpClient) : IBicepClientFactory
{
    private static readonly Regex VersionRegex = new(@"^\d+\.\d+\.\d+$");
    private static readonly string DefaultInstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".bicep", "bin");
    private static readonly OSPlatform CurrentOsPlatform = BicepInstaller.DetectCurrentOSPlatform();
    private static readonly Architecture CurrentArchitecture = RuntimeInformation.OSArchitecture;

    public async Task<IBicepClient> DownloadAndInitialize(BicepClientConfiguration configuration, CancellationToken cancellationToken)
    {
        var bicepCliPath = await Download(configuration, cancellationToken).ConfigureAwait(false);

        return await InitializeFromPath(bicepCliPath, cancellationToken).ConfigureAwait(false);
    }

    internal async Task<string> Download(BicepClientConfiguration configuration, CancellationToken cancellationToken)
    {
        if (configuration.BicepVersion is { } version && !VersionRegex.IsMatch(version))
        {
            throw new ArgumentException($"Invalid Bicep version format '{version}'. Expected format: 'x.y.z' where x, y, and z are integers.");
        }

        var osPlatform = configuration.OsPlatform ?? CurrentOsPlatform;
        var architecture = configuration.Architecture ?? CurrentArchitecture;
        var baseDownloadPath = configuration.InstallPath ?? DefaultInstallPath;
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

    public async Task<IBicepClient> InitializeFromPath(string bicepCliPath, CancellationToken cancellationToken)
    {
        return await BicepClient.Initialize(bicepCliPath, cancellationToken).ConfigureAwait(false);
    }
}
