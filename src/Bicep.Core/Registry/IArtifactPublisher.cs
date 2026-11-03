// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry;

/// <summary>
/// Publishes artifacts to registries.
/// </summary>
public interface IArtifactPublisher
{
    bool CanPublish(ArtifactType artifactType, ArtifactReference reference);

    Task<bool> ModuleExists(ArtifactReference reference);

    Task<bool> ExtensionExists(ArtifactReference reference);

    Task PublishModule(ArtifactReference reference, BinaryData compiledArmTemplate, BinaryData? bicepSources, string? documentationUri);

    Task PublishExtension(ArtifactReference reference, ExtensionPackage package);
}
