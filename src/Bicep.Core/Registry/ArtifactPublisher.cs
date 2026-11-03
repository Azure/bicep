// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;

namespace Bicep.Core.Registry;

public class ArtifactPublisher(IArtifactRegistryProvider registryProvider) : IArtifactPublisher
{
    public bool CanPublish(ArtifactType artifactType, ArtifactReference reference) =>
        this.GetRegistry(reference).GetCapabilities(artifactType, reference).HasFlag(RegistryCapabilities.Publish);

    public Task<bool> ModuleExists(ArtifactReference reference) =>
        this.GetRegistry(reference).CheckArtifactExists(ArtifactType.Module, reference);

    public Task<bool> ExtensionExists(ArtifactReference reference) =>
        this.GetRegistry(reference).CheckArtifactExists(ArtifactType.Extension, reference);

    public Task PublishModule(ArtifactReference reference, BinaryData compiledArmTemplate, BinaryData? bicepSources, string? documentationUri)
    {
        var description = DescriptionHelper.TryGetFromArmTemplate(compiledArmTemplate);
        return this.GetRegistry(reference).PublishModule(reference, compiledArmTemplate, bicepSources, documentationUri, description);
    }

    public Task PublishExtension(ArtifactReference reference, ExtensionPackage package) =>
        this.GetRegistry(reference).PublishExtension(reference, package);

    private IArtifactRegistry GetRegistry(ArtifactReference reference) => registryProvider.GetRegistry(reference.Scheme);
}
