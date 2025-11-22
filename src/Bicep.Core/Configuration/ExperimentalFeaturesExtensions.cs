// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Configuration
{
    public static class ExperimentalFeaturesExtensions
    {
        public static RootConfiguration WithExperimentalFeaturesConfiguration(this RootConfiguration current, ExperimentalFeaturesEnabled featuresEnabled) =>
            new(
                current.Cloud,
                current.ModuleAliases,
                current.Extensions,
                current.ImplicitExtensions,
                current.Analyzers,
                current.CacheRootDirectory,
                current.ExperimentalFeaturesWarning,
                featuresEnabled,
                current.Formatting,
                current.ExternalInputResolverConfiguration,
                current.ConfigFileUri,
                current.Diagnostics);

        public static RootConfiguration WithExperimentalFeaturesEnabled(this RootConfiguration current, ExperimentalFeaturesEnabled configuration) =>
            current.WithExperimentalFeaturesConfiguration(configuration);
    }
}
