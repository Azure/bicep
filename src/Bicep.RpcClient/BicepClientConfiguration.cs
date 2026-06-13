// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Bicep.RpcClient;

public record BicepClientConfiguration
{
    private static readonly Regex VersionRegex = new(@"^\d+\.\d+\.\d+$");
    private static readonly TimeSpan DefaultConnectionTimeout = TimeSpan.FromSeconds(30);

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
    /// Not applicable when <see cref="ConnectionMode"/> is <see cref="BicepConnectionMode.Stdio"/>.
    /// </summary>
    public TimeSpan ConnectionTimeout { get; init; } = DefaultConnectionTimeout;

    /// <summary>
    /// The transport used to communicate with the Bicep CLI process. Defaults to <see cref="BicepConnectionMode.NamedPipe"/>.
    /// </summary>
    public BicepConnectionMode ConnectionMode { get; init; } = BicepConnectionMode.NamedPipe;

    public static BicepClientConfiguration Default
        => new();

    internal static void Validate(BicepClientConfiguration config)
    {
        if (config.BicepVersion is { } version && !VersionRegex.IsMatch(version))
        {
            throw new ArgumentException($"Invalid Bicep version format '{version}'. Expected format: 'x.y.z' where x, y, and z are integers.");
        }

        if (config.ConnectionMode == BicepConnectionMode.Stdio && config.ConnectionTimeout != DefaultConnectionTimeout)
        {
            throw new ArgumentException($"The {nameof(ConnectionTimeout)} property cannot be used when {nameof(ConnectionMode)} is {nameof(BicepConnectionMode.Stdio)}.");
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

            if (!File.Exists(config.ExistingCliPath))
            {
                throw new FileNotFoundException($"The specified Bicep CLI path does not exist: '{config.ExistingCliPath}'.");
            }
        }
    }
}
