// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Features;
using Bicep.Core.Semantics.Namespaces;

namespace Bicep.Core.Analyzers.Linter.ApiVersions;

public class ApiVersionProviderFactory : IApiVersionProviderFactory
{
    private readonly IFeatureProviderFactory featureProviderFactory;
    private readonly INamespaceProvider namespaceProvider;

    public ApiVersionProviderFactory(IFeatureProviderFactory featureProviderFactory, INamespaceProvider namespaceProvider)
    {
        this.featureProviderFactory = featureProviderFactory;
        this.namespaceProvider = namespaceProvider;
    }

    public IApiVersionProvider GetApiVersionProvider(Uri templateUri)
        => new ApiVersionProvider(featureProviderFactory.GetFeatureProvider(templateUri), namespaceProvider);
}
