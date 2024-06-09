// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace Bicep.Core.Registry.Oci;

[JsonSerializable(typeof(OciProvidersV1Config))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class OciProvidersV1ConfigSerializationContext : JsonSerializerContext { }

public class OciProvidersV1Config
{
    // Avoid writing null properties for backwards compatibility
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? LocalDeployEnabled { get; }

    // Avoid writing null properties for backwards compatibility
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ImmutableArray<string>? SupportedArchitectures { get; }

    [JsonConstructor]
    public OciProvidersV1Config(bool? localDeployEnabled, ImmutableArray<string>? supportedArchitectures)
    {
        LocalDeployEnabled = localDeployEnabled;
        SupportedArchitectures = supportedArchitectures;
    }
}

public record SupportedArchitecture(
    string Name,
    Architecture Architecture,
    OSPlatform OsPlatform);

public static class SupportedArchitectures
{
    private static SupportedArchitecture LinuxX64 { get; } = new("linux-x64", Architecture.X64, OSPlatform.Linux);

    private static SupportedArchitecture LinuxArm64 { get; } = new("linux-arm64", Architecture.Arm64, OSPlatform.Linux);

    private static SupportedArchitecture OsxX64 { get; } = new("osx-x64", Architecture.X64, OSPlatform.OSX);

    private static SupportedArchitecture OsxArm64 { get; } = new("osx-arm64", Architecture.Arm64, OSPlatform.OSX);

    private static SupportedArchitecture WindowsX64 { get; } = new("win-x64", Architecture.X64, OSPlatform.Windows);

    private static SupportedArchitecture WindowsArm64 { get; } = new("win-arm64", Architecture.Arm64, OSPlatform.Windows);

    public static ImmutableArray<SupportedArchitecture> All { get; } = [
        LinuxX64,
        LinuxArm64,
        OsxX64,
        OsxArm64,
        WindowsX64,
        WindowsArm64,
    ];

    public static SupportedArchitecture? TryGetCurrent()
    {
        return RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => LinuxX64,
            Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => LinuxArm64,
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => OsxX64,
            Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => OsxArm64,
            Architecture.X64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => WindowsX64,
            Architecture.Arm64 when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => WindowsArm64,
            _ => null,
        };
    }
}
