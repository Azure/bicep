// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.InteropServices;

namespace Bicep.RpcClient;

public record BicepClientConfiguration
{
    public string? InstallPath { get; init; }

    public OSPlatform? OsPlatform { get; init; }

    public Architecture? Architecture { get; init; }

    public string? BicepVersion { get; init; }

    public static BicepClientConfiguration Default
        => new();
}
