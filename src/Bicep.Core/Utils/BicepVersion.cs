// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.Utils;

public record BicepVersion(
    string Value,
    string? CommitHash)
{
    public static readonly BicepVersion Instance = GetVersion();

    private static BicepVersion GetVersion()
    {
        var split = ThisAssembly.AssemblyInformationalVersion.Split('+');
        
        return new(
            split[0],
            split.Length > 1 ? split[1] : null);
    }
}
