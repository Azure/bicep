// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Features;
using Bicep.Core.TypeSystem.Az;

namespace Bicep.Core.Analyzers.Linter.ApiVersions;

public class ApiVersionProviderFactory : IApiVersionProviderFactory
{
    private readonly IFeatureProviderFactory featureProviderFactory;
    private readonly IAzResourceTypeLoader resourceTypeLoader;

    public ApiVersionProviderFactory(IFeatureProviderFactory featureProviderFactory, IAzResourceTypeLoader resourceTypeLoader)
    {
        this.featureProviderFactory = featureProviderFactory;
        this.resourceTypeLoader = resourceTypeLoader;
    }

    public IApiVersionProvider GetApiVersionProvider(Uri templateUri)
        => new ApiVersionProvider(featureProviderFactory.GetFeatureProvider(templateUri), resourceTypeLoader);
}
