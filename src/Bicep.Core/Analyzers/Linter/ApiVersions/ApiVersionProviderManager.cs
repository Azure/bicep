// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Semantics.Namespaces;

namespace Bicep.Core.Analyzers.Linter.ApiVersions;

public class ApiVersionProviderManager : IApiVersionProviderManager
{
    private readonly INamespaceProviderManager namespaceProviderManager;

    public ApiVersionProviderManager(INamespaceProviderManager namespaceProviderManager)
    {
        this.namespaceProviderManager = namespaceProviderManager;
    }

    public IApiVersionProvider GetApiVersionProvider(Uri templateUri)
        => new ApiVersionProvider(namespaceProviderManager.GetNamespaceProvider(templateUri));
}
