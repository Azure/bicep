// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.CompilerServices;
using Bicep.Core;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Bicep.LanguageServer.Compilation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    public class VisualResourceCreationService : IVisualResourceCreationService
    {
        // Client-supplied page sizes are clamped rather than echoed verbatim.
        public const int DefaultPageSize = 50;
        public const int MaxPageSize = 200;

        private readonly ConditionalWeakTable<IResourceTypeProvider, Lazy<ResourceTypeCatalog>> catalogs = new();

        public VisualResourceTypeNamespacesResult GetResourceTypeNamespaces(SemanticModel model)
        {
            var (catalog, scope) = this.GetCatalog(model);

            return new(catalog.GetId(scope), catalog.GetNamespaces(scope));
        }

        public VisualResourceTypesResult GetResourceTypes(
            SemanticModel model,
            string? providerNamespace,
            string? query,
            int pageSize,
            string? continuationToken)
        {
            var (catalog, scope) = this.GetCatalog(model);
            var resourceTypes = providerNamespace is not null
                ? catalog.GetResourceTypes(providerNamespace, scope)
                : string.IsNullOrWhiteSpace(query)
                    ? catalog.GetResourceTypes(scope)
                    : catalog.Search(query, scope);

            var offset = ParseContinuationToken(continuationToken);
            var page = resourceTypes.Skip(offset).Take(pageSize <= 0 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize)).ToImmutableArray();
            var nextOffset = offset + page.Length;

            return new(
                catalog.GetId(scope),
                page,
                nextOffset < resourceTypes.Length ? nextOffset.ToString(CultureInfo.InvariantCulture) : null);
        }

        public VisualResourceTypeVersionsResult GetResourceTypeVersions(SemanticModel model, string fullyQualifiedType)
        {
            var (catalog, scope) = this.GetCatalog(model);

            return new(catalog.GetId(scope), catalog.GetApiVersions(fullyQualifiedType));
        }

        public ResourceDeclarationInsertion CreateResourceDeclarationInsertion(BicepCompiler compiler, CompilationContext context, CreateResourceDeclarationInsertionParams request)
        {
            if (context.SourceFileKind != BicepSourceFileKind.BicepFile)
            {
                throw new VisualResourceCreationException("Visual resource creation is only supported for Bicep files.");
            }

            var model = context.Compilation.GetEntrypointSemanticModel();
            var typeReference = new ResourceTypeReference(request.ResourceType.FullyQualifiedType, request.ResourceType.ApiVersion);
            var resource = GeneratedResourceDeclaration.Create(
                typeReference,
                ResolveDeployableResourceType(model, typeReference),
                model.Root.Declarations.Select(declaration => declaration.Name),
                model);
            var insertion = resource.CreateInsertionEdit(compiler, context);

            var edit = new WorkspaceEdit
            {
                DocumentChanges = new Container<WorkspaceEditDocumentChange>(new TextDocumentEdit
                {
                    TextDocument = new OptionalVersionedTextDocumentIdentifier
                    {
                        Uri = request.TextDocument.Uri,
                        Version = request.TextDocument.Version,
                    },
                    Edits = new TextEditContainer(insertion),
                }),
            };

            return new(request.OperationId, resource.SymbolicName, resource.SymbolicName, resource.UnresolvedRequiredProperties, edit);
        }

        /// <summary>
        /// Resources are inserted as top-level declarations without a <c>scope</c> property, so the catalog offers only
        /// types that can be deployed at the document's own target scope. Anything other than a single deployment scope
        /// (not expected for Bicep files) disables filtering rather than hiding every type.
        /// </summary>
        private static ResourceScope? GetDeploymentScope(SemanticModel model) => model.TargetScope switch
        {
            ResourceScope.ResourceGroup or ResourceScope.Subscription or ResourceScope.ManagementGroup or ResourceScope.Tenant => model.TargetScope,
            _ => null,
        };

        private (ResourceTypeCatalog Catalog, ResourceScope? Scope) GetCatalog(SemanticModel model)
        {
            var azNamespace = model.Binder.NamespaceResolver.TryGetNamespace(AzNamespaceType.BuiltInName) ??
                throw new VisualResourceCreationException("The Azure type namespace is not available.");

            var catalog = this.catalogs.GetValue(
                azNamespace.ResourceTypeProvider,
                typeProvider => new(() => CreateCatalog(typeProvider, azNamespace), LazyThreadSafetyMode.ExecutionAndPublication)).Value;

            return (catalog, GetDeploymentScope(model));
        }

        private static ResourceTypeCatalog CreateCatalog(IResourceTypeProvider typeProvider, NamespaceType azNamespace)
        {
            // Imported Azure type packages are rare and are read type by type.
            IResourceWritableScopeResolver scopeResolver = ReferenceEquals(typeProvider, AzResourceTypeProvider.Instance)
                ? BuiltInAzureResourceWritableScopeResolver.Instance.Value
                : new ImportedExtensionResourceWritableScopeResolver(typeProvider, azNamespace);

            return new(typeProvider.TypeReferencesByType, scopeResolver);
        }

        private static ResourceType ResolveDeployableResourceType(SemanticModel model, ResourceTypeReference typeReference)
        {
            var resolver = model.Binder.NamespaceResolver;
            if (resolver.TryGetNamespace(AzNamespaceType.BuiltInName)?.ResourceTypeProvider.HasDefinedType(typeReference) != true)
            {
                throw new VisualResourceCreationException($"Resource type \"{typeReference.FormatName()}\" was not found.");
            }

            var resourceType = resolver.GetMatchingResourceTypes(typeReference, ResourceTypeGenerationFlags.None).FirstOrDefault() ??
                throw new VisualResourceCreationException($"Unable to resolve a type definition for resource type \"{typeReference.FormatName()}\".");

            // The catalog filters on each type's default version; an explicitly chosen version may differ.
            if (GetDeploymentScope(model) is { } scope && !(resourceType.ValidParentScopes & ~resourceType.ReadOnlyScopes).HasFlag(scope))
            {
                throw new VisualResourceCreationException(
                    $"Resource type \"{typeReference.FormatName()}\" cannot be deployed at the \"{LanguageConstants.GetResourceScopeDescriptions(scope).First()}\" scope.");
            }

            return resourceType;
        }

        private static int ParseContinuationToken(string? continuationToken) =>
            int.TryParse(continuationToken, NumberStyles.None, CultureInfo.InvariantCulture, out var offset) ? offset : 0;
    }
}
