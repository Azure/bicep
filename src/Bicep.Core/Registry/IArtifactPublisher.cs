// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry;

/// <summary>
/// Publishes artifacts to registries.
/// </summary>
public interface IArtifactPublisher
{
    bool CanPublish(ArtifactReference target);

    Task<bool> ExistsAsync(ArtifactReference target);

    /// <exception cref="ArgumentException">The package type does not match the target type.</exception>
    Task PublishAsync(ArtifactReference target, ArtifactPackage package);
}
