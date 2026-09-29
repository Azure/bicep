// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Bicep.LanguageServer.Compilation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    public class VisualResourceEditingService : IVisualResourceEditingService
    {
        public PrepareVisualResourceCreationResult PrepareResourceCreation(BicepCompiler compiler, CompilationContext context, PrepareVisualResourceCreationParams request)
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

            return new(request.OperationId, resource.SymbolicName, resource.UnresolvedRequiredProperties, edit);
        }

        public PrepareVisualResourceReplayResult PrepareResourceReplays(CompilationContext context, PrepareVisualResourceReplayParams request) =>
            ResourceCreationReplay.Prepare(context, request);

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
            if (ResourceTypeCatalog.GetDeploymentScope(model) is { } scope && !(resourceType.ValidParentScopes & ~resourceType.ReadOnlyScopes).HasFlag(scope))
            {
                throw new VisualResourceCreationException(
                    $"Resource type \"{typeReference.FormatName()}\" cannot be deployed at the \"{LanguageConstants.GetResourceScopeDescriptions(scope).First()}\" scope.");
            }

            return resourceType;
        }
    }
}
