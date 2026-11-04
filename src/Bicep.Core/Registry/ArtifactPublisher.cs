// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Core.Semantics;

namespace Bicep.Core.Registry;

public class ArtifactPublisher(IArtifactRegistryProvider registryProvider) : IArtifactPublisher
{
    public bool CanPublish(ArtifactReference target) =>
        this.GetRegistry(target).GetCapabilities(target.Type, target).HasFlag(RegistryCapabilities.Publish);

    public Task<bool> ExistsAsync(ArtifactReference target) =>
        this.GetRegistry(target).CheckArtifactExists(target.Type, target);

    public Task PublishAsync(ArtifactReference target, ArtifactPackage package)
    {
        if (target.Type != package.Type)
        {
            throw new ArgumentException($"Cannot publish a package of type '{package.Type}' to a target of type '{target.Type}'.", nameof(package));
        }

        var registry = this.GetRegistry(target);

        return package switch
        {
            ModulePackage module => registry.PublishModule(target, module.Template, module.Sources, module.DocumentationUri, DescriptionHelper.TryGetFromArmTemplate(module.Template)),
            ExtensionPackage extension => registry.PublishExtension(target, extension),
            _ => throw new UnreachableException($"Unexpected package type '{package.GetType().Name}'."),
        };
    }

    private IArtifactRegistry GetRegistry(ArtifactReference reference) => registryProvider.GetRegistry(reference.Scheme);
}
