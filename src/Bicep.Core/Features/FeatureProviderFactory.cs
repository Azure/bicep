// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Configuration;

namespace Bicep.Core.Features;

public class FeatureProviderFactory : IFeatureProviderFactory
{
    private readonly IConfigurationManager configurationManager;
    private readonly IEnumerable<IFeatureProviderSource> sources = ImmutableArray.Create(new EnvironmentVariablesFeatureProviderSource());

    public FeatureProviderFactory(IConfigurationManager configurationManager)
    {
        this.configurationManager = configurationManager;
    }

    public IFeatureProvider GetFeatureProvider(Uri templateUri) => new FeatureProvider(IFeatureProviderFactory.FeatureDefaults,
        sources.Append(new BicepConfigFeatureProviderSource(configurationManager.GetConfiguration(templateUri))));
}
