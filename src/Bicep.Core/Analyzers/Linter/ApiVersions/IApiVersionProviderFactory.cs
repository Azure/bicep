// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Workspaces;

namespace Bicep.Core.Analyzers.Linter.ApiVersions;

public interface IApiVersionProviderFactory
{
    IApiVersionProvider GetApiVersionProvider(BicepSourceFile bicepFile);

    static IApiVersionProviderFactory WithStaticApiVersionProvider(IApiVersionProvider apiVersionProvider)
        => new ConstantApiVersionProviderFactory(apiVersionProvider);

    private class ConstantApiVersionProviderFactory : IApiVersionProviderFactory
    {
        private readonly IApiVersionProvider apiVersionProvider;

        internal ConstantApiVersionProviderFactory(IApiVersionProvider apiVersionProvider)
        {
            this.apiVersionProvider = apiVersionProvider;
        }
        public IApiVersionProvider GetApiVersionProvider(BicepSourceFile bicepFile) => apiVersionProvider;
    }
}
