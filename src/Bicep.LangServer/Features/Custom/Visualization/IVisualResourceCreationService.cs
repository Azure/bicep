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
        /// Returns every resource type deployable at the document's target scope, unless the client already holds
        /// that catalog (<paramref name="knownCatalogId"/>).
        /// </summary>
        VisualResourceTypesResult GetResourceTypes(SemanticModel model, string? knownCatalogId);

        VisualResourceTypeVersionsResult GetResourceTypeVersions(
            SemanticModel model,
            string fullyQualifiedType);

        /// <summary>
        /// Generates a top-level resource declaration for the requested resource type and returns a
        /// proposed versioned <see cref="WorkspaceEdit"/> that the client can apply to the active document.
        /// </summary>
        PrepareVisualResourceCreationResult PrepareResourceCreation(
            BicepCompiler compiler,
            CompilationContext context,
            PrepareVisualResourceCreationParams request);

        /// <summary>
        /// Returns, for each designer resource creation, the edit that undoes or redoes it exactly against the current
        /// document, or null when it can no longer be replayed exactly.
        /// </summary>
        PrepareVisualResourceReplayResult PrepareResourceReplays(
            CompilationContext context,
            PrepareVisualResourceReplayParams request);
    }
}
