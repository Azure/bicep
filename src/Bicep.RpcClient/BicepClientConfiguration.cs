// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Bicep.RpcClient;

public record BicepClientConfiguration
{
    private static readonly Regex VersionRegex = new(@"^\d+\.\d+\.\d+$");

    [Obsolete($"Use the {nameof(InstallBasePath)} property instead. This property will be removed in a future release.")]
    public string? InstallPath { get; init; }

    /// <summary>
    /// The base directory where Bicep CLI binaries are installed. Defaults to <c>~/.bicep/bin</c>.
    /// </summary>
    public string? InstallBasePath { get; init; }

    /// <summary>
    /// The OS platform to use when downloading the Bicep CLI. Defaults to the current OS platform.
    /// </summary>
    public OSPlatform? OsPlatform { get; init; }

    /// <summary>
    /// The processor architecture to use when downloading the Bicep CLI. Defaults to the current OS architecture.
    /// </summary>
    public Architecture? Architecture { get; init; }

    /// <summary>
    /// The version of the Bicep CLI to download, in <c>x.y.z</c> format. Defaults to the latest available version.
    /// </summary>
    public string? BicepVersion { get; init; }

    /// <summary>
    /// The path to a pre-installed existing Bicep CLI executable. If specified, the factory will skip the download and installation steps and directly initialize the client with the provided CLI path. This is useful for scenarios where the caller wants to manage the Bicep CLI installation separately or use a custom-built version of the CLI.
    /// </summary>
    public string? ExistingCliPath { get; init; }

    /// <summary>
    /// The maximum time to wait for the Bicep CLI process to connect via the named pipe. Defaults to 30 seconds.
    /// </summary>
    public TimeSpan ConnectionTimeout { get; init; } = TimeSpan.FromSeconds(30);

    public static BicepClientConfiguration Default
        => new();

    internal static void Validate(BicepClientConfiguration config)
    {
        if (config.BicepVersion is { } version && !VersionRegex.IsMatch(version))
        {
            throw new ArgumentException($"Invalid Bicep version format '{version}'. Expected format: 'x.y.z' where x, y, and z are integers.");
        }

        if (config.ExistingCliPath is { })
        {
            if (config.InstallBasePath is not null)
            {
                throw new ArgumentException($"The {nameof(ExistingCliPath)} property cannot be used in conjunction with {nameof(InstallBasePath)}.");
            }

            if (config.BicepVersion is not null)
            {
                throw new ArgumentException($"The {nameof(ExistingCliPath)} property cannot be used in conjunction with {nameof(BicepVersion)}.");
            }

            if (config.OsPlatform is not null)
            {
                throw new ArgumentException($"The {nameof(ExistingCliPath)} property cannot be used in conjunction with {nameof(OsPlatform)}.");
            }

            if (config.Architecture is not null)
            {
                throw new ArgumentException($"The {nameof(ExistingCliPath)} property cannot be used in conjunction with {nameof(Architecture)}.");
            }
        }
    }
}
