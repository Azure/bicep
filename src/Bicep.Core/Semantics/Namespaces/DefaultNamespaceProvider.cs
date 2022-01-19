// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;

namespace Bicep.Core.Semantics.Namespaces
{
    public class DefaultNamespaceProvider : INamespaceProvider
    {
        private readonly AzResourceTypeProvider azResourceTypeProvider;
        private readonly IFeatureProvider featureProvider;

        public DefaultNamespaceProvider(IAzResourceTypeLoader azResourceTypeLoader, IFeatureProvider featureProvider)
        {
            this.azResourceTypeProvider = new AzResourceTypeProvider(azResourceTypeLoader);
            this.featureProvider = featureProvider;
        }

        public NamespaceType? TryGetNamespace(string providerName, string aliasName, ResourceScope resourceScope)
        {
            switch (providerName)
            {
                case SystemNamespaceType.BuiltInName:
                    return SystemNamespaceType.Create(aliasName);
                case AzNamespaceType.BuiltInName:
                    return AzNamespaceType.Create(aliasName, resourceScope, azResourceTypeProvider);
                case K8sNamespaceType.BuiltInName:
                    return K8sNamespaceType.Create(aliasName);
            }

            return null;
        }

        public bool AllowImportStatements
            => featureProvider.ImportsEnabled;
    }
}
