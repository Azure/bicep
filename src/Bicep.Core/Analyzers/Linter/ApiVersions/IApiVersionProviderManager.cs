// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.Analyzers.Linter.ApiVersions;

public interface IApiVersionProviderManager
{
    IApiVersionProvider GetApiVersionProvider(Uri templateUri);

    static IApiVersionProviderManager ForApiVersionProvider(IApiVersionProvider apiVersionProvider)
        => new ConstantApiVersionProviderManager(apiVersionProvider);

    private class ConstantApiVersionProviderManager : IApiVersionProviderManager
    {
        private readonly IApiVersionProvider apiVersionProvider;

        internal ConstantApiVersionProviderManager(IApiVersionProvider apiVersionProvider)
        {
            this.apiVersionProvider = apiVersionProvider;
        }

        public IApiVersionProvider GetApiVersionProvider(Uri templateUri) => apiVersionProvider;
    }
}
