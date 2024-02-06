// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Utils;
using Bicep.Core.Workspaces;

namespace Bicep.Core.TypeSystem.Providers;

public class ResourceTypesProviderDescriptor
{
    public ResourceTypesProviderDescriptor(
        string NamespaceIdentifier,
        Uri ParentModuleUri,
        string? Version = null,
        string? Alias = null,
        ArtifactReference? ArtifactReference = null,
        Uri? TypesDataUri = null)
    {
        this.Name = NamespaceIdentifier;
        this.ParentModuleUri = ParentModuleUri;
        if (Version is not null)
        {
            this.IsBuiltIn = false;
            this.Version = Version;
        }
        else
        {
            this.IsBuiltIn = true;
            this.Version = NamespaceIdentifier switch
            {
                AzNamespaceType.BuiltInName => AzNamespaceType.Settings.ArmTemplateProviderVersion,
                K8sNamespaceType.BuiltInName => K8sNamespaceType.Settings.ArmTemplateProviderVersion,
                MicrosoftGraphNamespaceType.BuiltInName => MicrosoftGraphNamespaceType.Settings.ArmTemplateProviderVersion,
                SystemNamespaceType.BuiltInName => SystemNamespaceType.Settings.ArmTemplateProviderVersion,
                _ => throw new NotImplementedException($"Built-in provider {NamespaceIdentifier} is not supported.")
            };
        }
        this.Alias = Alias ?? NamespaceIdentifier;
        this.ArtifactReference = ArtifactReference;
        this.TypesDataFileUri = TypesDataUri;
    }

    public ArtifactReference? ArtifactReference { get; }
    public bool IsBuiltIn { get; }
    public string Alias { get; }
    public string Name { get; }
    public Uri ParentModuleUri { get; }
    public string Version { get; }
    public Uri? TypesDataFileUri { get; }
};
