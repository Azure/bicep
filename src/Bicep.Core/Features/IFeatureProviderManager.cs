// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.Features;

public interface IFeatureProviderManager
{
    IFeatureProvider GetFeatureProvider(Uri templateUri);

    static IFeatureProviderManager ForFeatureProvider(IFeatureProvider featureProvider) 
        => new ConstantFeatureProviderManager(featureProvider);

    private class ConstantFeatureProviderManager : IFeatureProviderManager
    {
        private readonly IFeatureProvider features;

        internal ConstantFeatureProviderManager(IFeatureProvider features)
        {
            this.features = features;
        }

        public IFeatureProvider GetFeatureProvider(Uri templateUri) => features;
    }
}
