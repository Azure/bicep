// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Runtime.CompilerServices;
using Bicep.Core.Features;
using Bicep.Core.Semantics.Namespaces;

namespace Bicep.Core.Analyzers.Linter.ApiVersions;

public class ApiVersionProviderFactory : IApiVersionProviderFactory
{
    private readonly ConditionalWeakTable<IFeatureProvider, IApiVersionProvider> apiVersionProviderCache = new();
    private readonly IFeatureProviderFactory featureProviderFactory;
    private readonly INamespaceProvider namespaceProvider;

    public ApiVersionProviderFactory(IFeatureProviderFactory featureProviderFactory, INamespaceProvider namespaceProvider)
    {
        this.featureProviderFactory = featureProviderFactory;
        this.namespaceProvider = namespaceProvider;
    }

    public IApiVersionProvider GetApiVersionProvider(Uri templateUri) 
        => apiVersionProviderCache.GetValue(featureProviderFactory.GetFeatureProvider(templateUri),
            features => new ApiVersionProvider(features, namespaceProvider));
}
