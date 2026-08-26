// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Configuration
{
    public static class ExperimentalFeaturesExtensions
    {
        public static IBicepConfiguration WithExperimentalFeaturesConfiguration(this IBicepConfiguration current, ExperimentalFeaturesEnabled featuresEnabled) =>
            current.With(experimentalFeaturesEnabled: featuresEnabled);

        public static IBicepConfiguration WithExperimentalFeaturesEnabled(this IBicepConfiguration current, ExperimentalFeaturesEnabled configuration) =>
            current.WithExperimentalFeaturesConfiguration(configuration);
    }
}
