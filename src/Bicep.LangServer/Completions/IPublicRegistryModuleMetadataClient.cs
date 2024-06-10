// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Registry.Oci;

namespace Bicep.LanguageServer.Providers;

public record ModuleTagPropertiesEntry(string Description, string DocumentationUri);

public record ModuleMetadata(
    string ModuleName, // e.g. "avm/app/dapr-containerapp"
    List<string> Tags, // e.g. "1.0.0.0" (not guaranteed in that format)
    ImmutableDictionary<string, ModuleTagPropertiesEntry> Properties // Module properties per tag
);

public interface IPublicRegistryModuleMetadataClient
{
    Task<ImmutableArray<ModuleMetadata>> GetModuleMetadata();
}
