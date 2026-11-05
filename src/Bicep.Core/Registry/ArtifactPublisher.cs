// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Registry.Extensions;
using Bicep.Core.Semantics;

namespace Bicep.Core.Registry;

public class ArtifactPublisher(IArtifactRegistryProvider registryProvider) : IArtifactPublisher
{
    public bool CanPublish(ArtifactReference target) => target switch
    {
        // An extension package can be written to a local file; a module cannot.
        LocalArtifactReference local => local.Type == ArtifactType.Extension,
        ExternalArtifactReference external => this.GetRegistry(external).GetCapabilities(external.Type, external).HasFlag(RegistryCapabilities.Publish),
        _ => throw new UnreachableException($"Unexpected reference type '{target.GetType().Name}'."),
    };

    public Task<bool> ExistsAsync(ArtifactReference target) => target switch
    {
        LocalArtifactReference { Type: ArtifactType.Module } => throw new NotSupportedException("Local modules cannot be published."),
        // Publishing an extension to a local file overwrites it.
        LocalArtifactReference => Task.FromResult(false),
        ExternalArtifactReference external => this.GetRegistry(external).CheckArtifactExists(external.Type, external),
        _ => throw new UnreachableException($"Unexpected reference type '{target.GetType().Name}'."),
    };

    public async Task PublishAsync(ArtifactReference target, ArtifactPackage package)
    {
        if (target.Type != package.Type)
        {
            throw new ArgumentException($"Cannot publish a package of type '{package.Type}' to a target of type '{target.Type}'.", nameof(package));
        }

        switch (target, package)
        {
            case (LocalArtifactReference, ModulePackage):
                throw new NotSupportedException("Local modules cannot be published.");
            case (LocalArtifactReference local, ExtensionPackage extension):
                local.File.Write(await ExtensionV1Archive.Build(extension));
                return;
            case (ExternalArtifactReference external, ModulePackage module):
                await this.GetRegistry(external).PublishModule(external, module.Template, module.Sources, module.DocumentationUri, DescriptionHelper.TryGetFromArmTemplate(module.Template));
                return;
            case (ExternalArtifactReference external, ExtensionPackage extension):
                await this.GetRegistry(external).PublishExtension(external, extension);
                return;
            default:
                throw new UnreachableException($"Unexpected reference type '{target.GetType().Name}' or package type '{package.GetType().Name}'.");
        }
    }

    private IArtifactRegistry GetRegistry(ExternalArtifactReference reference) => registryProvider.GetRegistry(reference);
}
