// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.InteropServices;

namespace Bicep.RpcClient;

public class BicepClientConfiguration
{
    public string? InstallPath { get; set; }

    public OSPlatform? OsPlatform { get; set; }

    public Architecture? Architecture { get; set; }
}