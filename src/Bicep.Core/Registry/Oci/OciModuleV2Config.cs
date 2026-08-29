// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Bicep.Core.Registry.Oci;

[JsonSerializable(typeof(OciModuleV2Config))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class OciModuleV2ConfigSerializationContext : JsonSerializerContext { }

/// <summary>
/// The config blob of a layered module artifact (<see cref="BicepMediaTypes.BicepModuleConfigV2"/>).
/// </summary>
public class OciModuleV2Config
{
    [JsonConstructor]
    public OciModuleV2Config(string entryPointDigest)
    {
        EntryPointDigest = entryPointDigest;
    }

    public string EntryPointDigest { get; }
}
