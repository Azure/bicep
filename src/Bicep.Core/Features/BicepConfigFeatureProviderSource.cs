// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Configuration;

namespace Bicep.Core.Features;

internal class BicepConfigFeatureProviderSource : IFeatureProviderSource
{
    private readonly RootConfiguration configuration;

    public BicepConfigFeatureProviderSource(RootConfiguration configuration)
    {
        this.configuration = configuration;
    }

    sbyte IFeatureProviderSource.Priority => 1;
    public string? CacheRootDirectory => configuration.CacheRootDirectory;
    public bool? SymbolicNameCodegenEnabled => configuration.ExperimentalFeaturesEnabled.IsEnabled("symbolicNameCodegen");
    public bool? ImportsEnabled => configuration.ExperimentalFeaturesEnabled.IsEnabled("imports");
    public bool? ResourceTypedParamsAndOutputsEnabled => configuration.ExperimentalFeaturesEnabled.IsEnabled("resourceTypedParamsAndOutputs");
    public bool? SourceMappingEnabled => configuration.ExperimentalFeaturesEnabled.IsEnabled("sourceMapping");
    public bool? ParamsFilesEnabled => configuration.ExperimentalFeaturesEnabled.IsEnabled("paramsFiles");
}
