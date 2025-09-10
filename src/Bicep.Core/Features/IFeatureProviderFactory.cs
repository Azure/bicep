// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.IO.Abstraction;

namespace Bicep.Core.Features;

public interface IFeatureProviderFactory
{
    IFeatureProvider GetFeatureProvider(IOUri sourceFileUri);

    static IFeatureProviderFactory WithStaticFeatureProvider(IFeatureProvider featureProvider)
        => new ConstantFeatureProviderFactory(featureProvider);

    private class ConstantFeatureProviderFactory : IFeatureProviderFactory
    {
        private readonly IFeatureProvider features;

        internal ConstantFeatureProviderFactory(IFeatureProvider features)
        {
            this.features = features;
        }

        public IFeatureProvider GetFeatureProvider(IOUri sourceFileUri) => features;
    }
}
