// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Semantics;
using Bicep.LanguageServer.Compilation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    public interface IVisualResourceCreationService
    {
        /// <summary>
        /// Lists the Azure Resource Provider namespaces of resource types deployable at the document's target scope.
        /// </summary>
        VisualResourceTypeNamespacesResult GetResourceTypeNamespaces(SemanticModel model);

        /// <summary>
        /// Returns a page of resource types, either of one Azure Resource Provider namespace or matching a search query.
        /// </summary>
        VisualResourceTypesResult GetResourceTypes(
            SemanticModel model,
            string? providerNamespace,
            string? query,
            int pageSize,
            string? continuationToken);

        VisualResourceTypeVersionsResult GetResourceTypeVersions(
            SemanticModel model,
            string fullyQualifiedType);

        /// <summary>
        /// Generates a top-level resource declaration for the requested resource type and returns a
        /// proposed versioned <see cref="WorkspaceEdit"/> that the client can apply to the active document.
        /// </summary>
        ResourceDeclarationInsertion CreateResourceDeclarationInsertion(
            BicepCompiler compiler,
            CompilationContext context,
            CreateResourceDeclarationInsertionParams request);
    }
}
