// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;

namespace Bicep.Core.UnitTests.Features;

public class OverriddenFeatureProviderFactory(FeatureProviderFactory factory, FeatureProviderOverrides overrides) : IFeatureProviderFactory
{
    private readonly IFeatureProviderFactory factory = factory;
    private readonly FeatureProviderOverrides overrides = overrides;

    public IFeatureProvider GetFeatureProvider(Uri templateUri)
        => new OverriddenFeatureProvider(factory.GetFeatureProvider(templateUri), overrides);
}
