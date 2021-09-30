// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.Features;

namespace Bicep.Core.IntegrationTests.Extensibility
{
    public class ExtensibilityNamespaceProvider : INamespaceProvider
    {
        private readonly INamespaceProvider defaultNamespaceProvider;

        public ExtensibilityNamespaceProvider(IAzResourceTypeLoader azResourceTypeLoader, IFeatureProvider featureProvider)
        {
            defaultNamespaceProvider = new DefaultNamespaceProvider(azResourceTypeLoader, featureProvider);
        }

        public bool AllowImportStatements => defaultNamespaceProvider.AllowImportStatements;

        public NamespaceType? TryGetNamespace(string providerName, string aliasName, ResourceScope resourceScope)
        {
            if (defaultNamespaceProvider.TryGetNamespace(providerName, aliasName, resourceScope) is { } namespaceType)
            {
                return namespaceType;
            }

            switch (providerName)
            {
                case StorageNamespaceType.BuiltInName:
                    return StorageNamespaceType.Create(aliasName);
            }

            return default;
        }
    }
}