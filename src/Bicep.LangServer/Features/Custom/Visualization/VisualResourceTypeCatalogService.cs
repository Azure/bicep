// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.CompilerServices;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    public class VisualResourceTypeCatalogService : IVisualResourceTypeCatalogService
    {
        private readonly ConditionalWeakTable<IResourceTypeProvider, Lazy<ResourceTypeCatalog>> catalogs = new();

        public VisualResourceTypesResult GetResourceTypes(SemanticModel model, string? knownCatalogId)
        {
            var (catalog, scope) = this.GetCatalog(model);
            var catalogId = catalog.GetId(scope);

            return new(catalogId, catalogId == knownCatalogId ? null : catalog.GetResourceTypes(scope));
        }

        public VisualResourceTypeVersionsResult GetResourceTypeVersions(SemanticModel model, string fullyQualifiedType)
        {
            var (catalog, scope) = this.GetCatalog(model);

            return new(catalog.GetId(scope), catalog.GetApiVersions(fullyQualifiedType));
        }

        private (ResourceTypeCatalog Catalog, ResourceScope? Scope) GetCatalog(SemanticModel model)
        {
            var azNamespace = model.Binder.NamespaceResolver.TryGetNamespace(AzNamespaceType.BuiltInName) ??
                throw new VisualResourceCreationException("The Azure type namespace is not available.");

            var catalog = this.catalogs.GetValue(
                azNamespace.ResourceTypeProvider,
                typeProvider => new(() => CreateCatalog(typeProvider, azNamespace), LazyThreadSafetyMode.ExecutionAndPublication)).Value;

            return (catalog, ResourceTypeCatalog.GetDeploymentScope(model));
        }

        private static ResourceTypeCatalog CreateCatalog(IResourceTypeProvider typeProvider, NamespaceType azNamespace)
        {
            // Imported Azure type packages are rare and are read type by type.
            IResourceWritableScopeResolver scopeResolver = ReferenceEquals(typeProvider, AzResourceTypeProvider.Instance)
                ? BuiltInAzureResourceWritableScopeResolver.Instance.Value
                : new ImportedExtensionResourceWritableScopeResolver(typeProvider, azNamespace);

            return new(typeProvider.TypeReferencesByType, scopeResolver);
        }
    }
}
