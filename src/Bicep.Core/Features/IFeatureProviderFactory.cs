// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.Features;

public interface IFeatureProviderFactory
{
    IFeatureProvider GetFeatureProvider(Uri templateUri);

    static IFeatureProviderFactory WithStaticFeatureProvider(IFeatureProvider featureProvider)
        => new ConstantFeatureProviderFactory(featureProvider);

    private class ConstantFeatureProviderFactory : IFeatureProviderFactory
    {
        private readonly IFeatureProvider features;

        internal ConstantFeatureProviderFactory(IFeatureProvider features)
        {
            this.features = features;
        }

        public IFeatureProvider GetFeatureProvider(Uri templateUri) => features;
    }
}
