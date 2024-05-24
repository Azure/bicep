// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Semver;

namespace Bicep.Core.Registry.PublicRegistry;

// Module names look like e.g. "avm/app/dapr-containerapp"
public record RegistryModule(string Name, string? Description, string? DocumentationUri);

public record RegistryModuleVersion(string Version, string? Description, string? DocumentationUri);

public interface IPublicRegistryModuleMetadataProvider
{
    bool IsModulesCacheAvailable { get; }

    IEnumerable<RegistryModule> GetCachedModules();

    IEnumerable<RegistryModuleVersion> GetCachedModuleVersions(string modulePath);
}

//public class RegistryModuleVersionComparer : IComparer<RegistryModuleVersion>
//{
//    public int Compare(RegistryModuleVersion x, RegistryModuleVersion y)
//    {
//        var xSemVer = SemVersion.SortOrderComparer.Parse(x.Version);
//        var ySemVer = SemanticVersion.Parse(y.Version);

//        // Handle nulls
//        if (x == null)
//        {
//            return (y == null) ? 0 : -1; //asdfg?
//        }
//        else if (y == null)
//        {
//            return 1;
//        }

//        // Parse the version strings to SemanticVersion objects

//        // Use the CompareTo method of SemanticVersion
//        return xSemVer.CompareTo(ySemVer);
//    }
//}
