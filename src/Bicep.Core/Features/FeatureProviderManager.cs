// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Configuration;

namespace Bicep.Core.Features;

public class FeatureProviderManager : IFeatureProviderManager
{
    private readonly IConfigurationManager configurationManager;
    private readonly IEnumerable<IFeatureProviderSource> sources;

    public FeatureProviderManager(IConfigurationManager configurationManager, IEnumerable<IFeatureProviderSource> additionalSources)
    {
        this.configurationManager = configurationManager;
        sources = additionalSources.Append(new EnvironmentVariablesFeatureProviderSource()).ToList();
    }

    public IFeatureProvider GetFeatureProvider(Uri templateUri) => new FeatureProvider(IFeatureProviderManager.FeatureDefaults,
        sources.Append(new BicepConfigFeatureProviderSource(configurationManager.GetConfiguration(templateUri))));
}
