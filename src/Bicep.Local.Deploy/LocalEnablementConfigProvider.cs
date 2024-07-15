// -----------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// -----------------------------------------------------------

using Azure.Deployments.Core.FeatureEnablement;

namespace Bicep.Local.Deploy;

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