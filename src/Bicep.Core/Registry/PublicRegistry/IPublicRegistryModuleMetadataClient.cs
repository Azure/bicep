// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Registry.Oci;

namespace Bicep.Core.Registry.PublicRegistry;

public record BicepModuleTagPropertiesEntry(string Description, string DocumentationUri);

public record BicepModuleMetadata(
    string ModuleName, // e.g. "avm/app/dapr-containerapp"
    List<string> Tags, // e.g. "1.0.0" (not guaranteed to be in that format, although it currently is for public modules)
    ImmutableDictionary<string, BicepModuleTagPropertiesEntry> Properties // Module properties per tag
);

public interface IPublicRegistryModuleMetadataClient
{
    Task<ImmutableArray<BicepModuleMetadata>> GetModuleMetadata();
}
