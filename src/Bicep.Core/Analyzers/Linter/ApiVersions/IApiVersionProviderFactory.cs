// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.Analyzers.Linter.ApiVersions;

public interface IApiVersionProviderFactory
{
    IApiVersionProvider GetApiVersionProvider(Uri templateUri);

    static IApiVersionProviderFactory WithStaticApiVersionProvider(IApiVersionProvider apiVersionProvider)
        => new ConstantApiVersionProviderFactory(apiVersionProvider);

    private class ConstantApiVersionProviderFactory : IApiVersionProviderFactory
    {
        private readonly IApiVersionProvider apiVersionProvider;

        internal ConstantApiVersionProviderFactory(IApiVersionProvider apiVersionProvider)
        {
            this.apiVersionProvider = apiVersionProvider;
        }

        public IApiVersionProvider GetApiVersionProvider(Uri templateUri) => apiVersionProvider;
    }
}
