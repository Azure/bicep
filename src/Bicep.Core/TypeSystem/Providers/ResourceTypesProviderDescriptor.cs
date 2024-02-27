// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;
using Bicep.Core.Semantics.Namespaces;

namespace Bicep.Core.TypeSystem.Providers;

public record ResourceTypesProviderDescriptor
{
    public const string LegacyVersionPlaceholder = "1.0.0";

    private ResourceTypesProviderDescriptor(
        string NamespaceIdentifier,
        string Version,
        bool IsBuiltIn,
        Uri ParentModuleUri,
        string? Alias = null,
        ArtifactReference? ArtifactReference = null,
        Uri? TypesTgzUri = null)
    {
        this.Name = NamespaceIdentifier;
        this.ParentModuleUri = ParentModuleUri;
        this.Version = Version;
        this.IsBuiltIn = IsBuiltIn;
        this.Alias = Alias ?? NamespaceIdentifier;
        this.ArtifactReference = ArtifactReference;
        this.TypesTgzUri = TypesTgzUri;
    }

    public ArtifactReference? ArtifactReference { get; }
    public bool IsBuiltIn { get; }
    public string Alias { get; }
    public string Name { get; }
    public Uri ParentModuleUri { get; }
    public string Version { get; }
    public Uri? TypesTgzUri { get; }

    public static ResourceTypesProviderDescriptor CreateBuiltInProviderDescriptor(string namespaceIdentifier, string version, Uri parentModuleUri, string? alias = null)
    {
        var isBuiltIn = (version == LegacyVersionPlaceholder) || (namespaceIdentifier == AzNamespaceType.BuiltInName
                                                                  && version == AzNamespaceType.Settings.ArmTemplateProviderVersion);
        return new(namespaceIdentifier, version, isBuiltIn, parentModuleUri, alias);
    }

    public static ResourceTypesProviderDescriptor CreateDynamicallyLoadedProviderDescriptor(string namespaceIdentifier, string version, Uri parentModuleUri, ArtifactReference? artifactReference, Uri? typesTgzUri, string? alias = null)
        => new(namespaceIdentifier, version, IsBuiltIn: false, parentModuleUri, alias, artifactReference, typesTgzUri);


};
