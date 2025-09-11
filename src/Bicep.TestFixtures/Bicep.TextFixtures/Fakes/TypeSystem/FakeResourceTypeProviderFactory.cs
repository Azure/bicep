// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Bicep.IO.Abstraction;

namespace Bicep.TextFixtures.Fakes.TypeSystem
{
    public class FakeResourceTypeProviderFactory : IResourceTypeProviderFactory
    {
        private readonly IResourceTypeProvider provider;

        public FakeResourceTypeProviderFactory(IResourceTypeProvider provider)
        {
            this.provider = provider;
        }

        public static FakeResourceTypeProviderFactory ForAzureResourceTypes(IEnumerable<ResourceTypeComponents> resourceTypes) =>
            new(new AzResourceTypeProvider(new FakeResourceTypeLoader(resourceTypes)));

        public IResourceTypeProvider GetBuiltInAzResourceTypesProvider() => this.provider;

        public ResultWithDiagnosticBuilder<IResourceTypeProvider> GetResourceTypeProvider(IFileHandle typesTgzFileHandle) => new(this.provider);
    }
}
