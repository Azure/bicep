// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;
using Bicep.Core.Semantics.Namespaces;

namespace Bicep.Core.TypeSystem.Providers;

public class ResourceTypesProviderDescriptor
{
    public const string LegacyVersionPlaceholder = "1.0.0";

    public ResourceTypesProviderDescriptor(
        string NamespaceIdentifier,
        Uri ParentModuleUri,
        string Version = LegacyVersionPlaceholder,
        string? Alias = null,
        ArtifactReference? ArtifactReference = null,
        Uri? TypesDataUri = null)
    {
        this.Name = NamespaceIdentifier;
        this.ParentModuleUri = ParentModuleUri;
        this.Version = Version;
        this.IsBuiltIn = IsBuiltInNamespace(NamespaceIdentifier, Version);
        this.Alias = Alias ?? NamespaceIdentifier;
        this.ArtifactReference = ArtifactReference;
        this.TypesDataFileUri = TypesDataUri;
    }

    private static bool IsBuiltInNamespace(string namespaceIdentifier, string version)
    {
        var usesLegacyPlaceholderVersion = version == LegacyVersionPlaceholder;
        if (namespaceIdentifier == AzNamespaceType.BuiltInName)
        {
            return usesLegacyPlaceholderVersion || version == AzNamespaceType.Settings.ArmTemplateProviderVersion;
        }
        return usesLegacyPlaceholderVersion;
    }

    public ArtifactReference? ArtifactReference { get; }
    public bool IsBuiltIn { get; }
    public string Alias { get; }
    public string Name { get; }
    public Uri ParentModuleUri { get; }
    public string Version { get; }
    public Uri? TypesDataFileUri { get; }
};
