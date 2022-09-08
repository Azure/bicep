// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Features;
using Bicep.Core.TypeSystem.Az;

namespace Bicep.Core.Semantics.Namespaces;

public class DefaultNamespaceProviderManager : INamespaceProviderManager
{
    private readonly IAzResourceTypeLoader azResourceTypeLoader;
    private readonly IFeatureProviderManager featureProviderManager;

    public DefaultNamespaceProviderManager(IAzResourceTypeLoader azResourceTypeLoader, IFeatureProviderManager featureProviderManager)
    {
        this.azResourceTypeLoader = azResourceTypeLoader;
        this.featureProviderManager = featureProviderManager;
    }

    public INamespaceProvider GetNamespaceProvider(Uri templateUri)
        => new DefaultNamespaceProvider(azResourceTypeLoader, featureProviderManager.GetFeatureProvider(templateUri));
}
