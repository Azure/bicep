// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Bicep.Core.Extensions;
using Bicep.Core.Registry.Oci;
using Semver;
using Semver.Comparers;

namespace Bicep.Core.Registry.PublicRegistry; //asdfg rename to Registry.Acr?  Oci?  Modules?

public readonly record struct PublicRegistryModuleIndexProperties(string Description, string DocumentationUri);

public interface IPublicRegistryModuleIndexHttpClient
{
    Task<ImmutableArray<PublicRegistryModuleIndexEntry>> GetModuleIndexAsync();
}

/// <summary>
/// This is the DTO for modules listed in the public bicep registry
/// </summary>
public record PublicRegistryModuleIndexEntry(
    [property: JsonPropertyName("moduleName")] string ModulePath, // e.g. "avm/app/dapr-containerapp" - the "bicep/" prefix is assumed and not included in the index
    ImmutableArray<string> Tags, // e.g. "1.0.0" (not guaranteed to be in semver format, although it currently is for all our public modules)
    [property: JsonPropertyName("properties")] ImmutableDictionary<string, PublicRegistryModuleIndexProperties> PropertiesByTag // Module properties per tag
)
{
    private static readonly SemVersion DefaultVersion = new(0);

    // Sort tags by version numbers in descending order.
    public ImmutableArray<string> Versions
    {
        get
        {
            {
                var parsedVersions = Tags.Select(x => //asdfg this to indexer so private and public use it
                    (@string: x, version: SemVersion.TryParse(x, SemVersionStyles.AllowV, out var version) ? version : DefaultVersion))
                    .ToArray();
                return [.. parsedVersions.OrderByDescending(x => x.version, SemVersion.SortOrderComparer).Select(x => x.@string)];
            }
        }
    }

    public string? GetDescription(string? version = null) => this.GetProperty(version, x => x.Description);

    public string? GetDocumentationUri(string? version = null) => this.GetProperty(version, x => x.DocumentationUri);

    private string? GetProperty(string? version, Func<PublicRegistryModuleIndexProperties, string> propertySelector)
    {
        if (version is null)
        {
            // Get description for most recent version with a description
            foreach (var tag in this.Versions)
            {
                if (this.PropertiesByTag.TryGetValue(tag, out var propertiesEntry))
                {
                    return propertySelector(propertiesEntry);
                }
            }

            return null;
        }
        else
        {
            return this.PropertiesByTag.TryGetValue(version, out var propertiesEntry) ? propertySelector(propertiesEntry) : null;
        }
    }
}
