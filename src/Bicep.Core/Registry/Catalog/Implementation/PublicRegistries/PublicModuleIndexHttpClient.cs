// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Bicep.Core.Extensions;
using Bicep.Core.Registry.Oci;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Semver;
using Semver.Comparers;

namespace Bicep.Core.Registry.Catalog.Implementation.PublicRegistries;

/// <summary>
/// This is the DTO for modules listed in the public bicep registry
/// </summary>
public record PublicModuleIndexEntry(
    [property: JsonPropertyName("moduleName")] string ModulePath, // e.g. "avm/app/dapr-containerapp" - the "bicep/" prefix is assumed and not included in the index we download
    ImmutableArray<string> Tags, // e.g. "1.0.0" (not guaranteed to be in semver format, although it currently is for all our public modules)
    [property: JsonPropertyName("properties")] ImmutableDictionary<string, PublicModuleIndexProperties> PropertiesByTag // Module properties per tag
)
{
    private static readonly SemVersion DefaultVersion = new(0);

    public ImmutableArray<string> Versions
    {
        get
        {
            {
                var parsedVersions = Tags.Select(x =>
                    (@string: x, version: SemVersion.TryParse(x, SemVersionStyles.AllowV, out var version) ? version : DefaultVersion))
                    .ToArray();
                // Sort by ascending version number here, the completion provider will reverse it to show the most recent version first
                return [.. parsedVersions.OrderByAscending(x => x.version, SemVersion.SortOrderComparer).Select(x => x.@string)];
            }
        }
    }

    public string? GetDescription(string? version = null) => GetProperty(version, x => x.Description);

    public string? GetDocumentationUri(string? version = null) => GetProperty(version, x => x.DocumentationUri);

    private string? GetProperty(string? version, Func<PublicModuleIndexProperties, string> propertySelector)
    {
        if (version is null)
        {
            // Get description for most recent version with a description
            foreach (var tag in Versions.Reverse())
            {
                if (PropertiesByTag.TryGetValue(tag, out var propertiesEntry))
                {
                    return propertySelector(propertiesEntry);
                }
            }

            return null;
        }
        else
        {
            return PropertiesByTag.TryGetValue(version, out var propertiesEntry) ? propertySelector(propertiesEntry) : null;
        }
    }
}
