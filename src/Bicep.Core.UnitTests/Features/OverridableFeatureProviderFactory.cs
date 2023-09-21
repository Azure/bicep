// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Features;

namespace Bicep.Core.UnitTests.Features;

public class OverriddenFeatureProviderFactory : IFeatureProviderFactory
{
    private readonly IFeatureProviderFactory factory;
    private readonly FeatureProviderOverrides overrides;

    public OverriddenFeatureProviderFactory(FeatureProviderFactory factory, FeatureProviderOverrides overrides)
    {
        this.factory = factory;
        this.overrides = overrides;
    }

    public IFeatureProvider GetFeatureProvider(Uri templateUri)
        => new OverriddenFeatureProvider(factory.GetFeatureProvider(templateUri), overrides);
}
