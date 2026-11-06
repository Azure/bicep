// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry;

/// <summary>
/// The settings used to reach a Template Spec.
/// </summary>
public sealed record TemplateSpecArtifactContext(IBicepCloudConfiguration Cloud, IDirectoryHandle CacheRootDirectory)
    : ExternalArtifactContext(Cloud, CacheRootDirectory)
{
    public static TemplateSpecArtifactContext Create(IBicepConfiguration configuration, IFeatureProvider features) =>
        new(configuration.Cloud, features.CacheRootDirectory);
}
