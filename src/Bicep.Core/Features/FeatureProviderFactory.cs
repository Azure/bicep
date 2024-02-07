// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Configuration;

namespace Bicep.Core.Features;

public class FeatureProviderFactory(IConfigurationManager configurationManager) : IFeatureProviderFactory
{
    private readonly IConfigurationManager configurationManager = configurationManager;

    public IFeatureProvider GetFeatureProvider(Uri templateUri) => new FeatureProvider(configurationManager.GetConfiguration(templateUri));
}
