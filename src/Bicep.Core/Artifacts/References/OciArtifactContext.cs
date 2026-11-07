// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts.References;

/// <summary>
/// The settings used to reach an OCI artifact.
/// </summary>
/// <param name="UseOrasSdk">Whether registries are reached with the ORAS SDK instead of the Azure SDK (the <c>ociEnabled</c> experimental feature).</param>
public sealed record OciArtifactContext(IBicepCloudConfiguration Cloud, IDirectoryHandle CacheRootDirectory, bool UseOrasSdk)
    : RemoteArtifactContext(Cloud, CacheRootDirectory)
{
    public static OciArtifactContext Create(IBicepConfiguration configuration, IFeatureProvider features) =>
        new(configuration.Cloud, features.CacheRootDirectory, features.OciEnabled);
}
