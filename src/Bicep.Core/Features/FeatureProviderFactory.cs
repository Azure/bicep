// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Configuration;
using System;

namespace Bicep.Core.Features;

public class FeatureProviderFactory : IFeatureProviderFactory
{
    private readonly IConfigurationManager configurationManager;

    public FeatureProviderFactory(IConfigurationManager configurationManager)
    {
        this.configurationManager = configurationManager;
    }

    public IFeatureProvider GetFeatureProvider(Uri templateUri) => new FeatureProvider(configurationManager.GetConfiguration(templateUri));
}
