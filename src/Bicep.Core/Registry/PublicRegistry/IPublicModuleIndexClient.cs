// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Bicep.Core.Extensions;
using Bicep.Core.Registry.Oci;
using Semver;
using Semver.Comparers;

namespace Bicep.Core.Registry.PublicRegistry;

public readonly record struct PublicModuleProperties(string Description, string DocumentationUri);

public record PublicModuleIndexEntry(
    [property: JsonPropertyName("moduleName")] string ModulePath, // e.g. "avm/app/dapr-containerapp"
    ImmutableArray<string> Tags, // e.g. "1.0.0" (not guaranteed to be in that format, although it currently is for public modules)
    [property: JsonPropertyName("properties")] ImmutableDictionary<string, PublicModuleProperties> PropertiesByTag // Module properties per tag
)
{
    private static readonly SemVersion DefaultVersion = new(0);

    // Sort tags by version numbers in descending order.
    public ImmutableArray<string> Versions
    {
        get
        {
            {
                var parsedVersions = Tags.Select(x =>
                    (@string: x, version: SemVersion.TryParse(x, SemVersionStyles.AllowV, out var version) ? version : DefaultVersion))
                    .ToArray();
                return parsedVersions.OrderByDescending(x => x.version, SemVersion.SortOrderComparer)
                    .Select(x => x.@string)
                    .ToImmutableArray();
            }
        }
    }

    public string? GetDescription(string? version = null) => this.GetProperty(version, x => x.Description);

    public string? GetDocumentationUri(string? version = null) => this.GetProperty(version, x => x.DocumentationUri);

    private string? GetProperty(string? version, Func<PublicModuleProperties, string> propertySelector)
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

public interface IPublicModuleIndexClient
{
    Task<ImmutableArray<PublicModuleIndexEntry>> GetModuleIndexAsync();
}
