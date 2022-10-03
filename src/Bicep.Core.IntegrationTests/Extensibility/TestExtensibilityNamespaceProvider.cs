// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.Features;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.IntegrationTests.Extensibility
{
    public class TestExtensibilityNamespaceProvider : INamespaceProvider
    {
        private readonly INamespaceProvider defaultNamespaceProvider;

        public TestExtensibilityNamespaceProvider(IAzResourceTypeLoader azResourceTypeLoader)
        {
            defaultNamespaceProvider = new DefaultNamespaceProvider(azResourceTypeLoader);
        }

        public IEnumerable<string> AvailableNamespaces => defaultNamespaceProvider.AvailableNamespaces.Concat(new [] {
            StorageNamespaceType.BuiltInName,
            AadNamespaceType.BuiltInName,
        });

        public NamespaceType? TryGetNamespace(string providerName, string aliasName, ResourceScope resourceScope, IFeatureProvider featureProvider)
        {
            if (defaultNamespaceProvider.TryGetNamespace(providerName, aliasName, resourceScope, featureProvider) is { } namespaceType)
            {
                return namespaceType;
            }

            switch (providerName)
            {
                case StorageNamespaceType.BuiltInName:
                    return StorageNamespaceType.Create(aliasName);
                case AadNamespaceType.BuiltInName:
                    return AadNamespaceType.Create(aliasName);
            }

            return default;
        }
    }
}
