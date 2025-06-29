// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.FeatureEnablement;

namespace Bicep.Local.Deploy.Engine;

public class LocalEnablementConfigProvider : IEnablementConfigProvider
{
    public EnablementConfig GetEnablementConfig(PreviewDeploymentFunction feature)
    {
        return new EnablementConfig
        {
            FeatureName = feature.ToString(),
            DisabledForAll = false,
            EnabledForAll = true
        };
    }
}
