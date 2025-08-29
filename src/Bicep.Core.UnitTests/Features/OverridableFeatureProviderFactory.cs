// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
using Bicep.IO.Abstraction;

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

    public IFeatureProvider GetFeatureProvider(IOUri sourceFileUri)
        => new OverriddenFeatureProvider(factory.GetFeatureProvider(sourceFileUri), overrides);
}
