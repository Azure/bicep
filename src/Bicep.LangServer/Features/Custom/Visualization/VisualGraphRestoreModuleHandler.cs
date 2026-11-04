// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.LanguageServer.Compilation;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// Handles <c>textDocument/visualGraphRestoreModule</c>: restores the registry module behind a node into the local
    /// module cache when the user expands it, as resolving a link to a module in published source does in the editor.
    /// The document is not recompiled: its compilation does not depend on the module, and the graph reads the module
    /// cache when it is built.
    /// </summary>
    public class VisualGraphRestoreModuleHandler : IJsonRpcRequestHandler<VisualGraphRestoreModuleParams, VisualGraphRestoreModuleResult>
    {
        private readonly ILogger<VisualGraphRestoreModuleHandler> logger;

        private readonly ICompilationManager compilationManager;

        private readonly VisualGraphBuilder graphBuilder;

        private readonly IArtifactRestorer restorer;

        public VisualGraphRestoreModuleHandler(
            ILogger<VisualGraphRestoreModuleHandler> logger,
            ICompilationManager compilationManager,
            VisualGraphBuilder graphBuilder,
            IArtifactRestorer restorer)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.graphBuilder = graphBuilder;
            this.restorer = restorer;
        }

        public async Task<VisualGraphRestoreModuleResult> Handle(VisualGraphRestoreModuleParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context is null || this.graphBuilder.TryGetRestorableReference(context, request.NodeId) is not { } registryReference)
            {
                return new(Restored: false);
            }

            // A module that is not restored yet is restored as the editor would. Anything else is the user retrying after an
            // error, which forces the restore: it retries a recent failure, and replaces a cached archive that cannot be read.
            var isRetry = this.restorer.GetRestoreState(registryReference) is not ArtifactRestoreState.NotRestored;
            await this.restorer.RestoreAsync([registryReference], isRetry ? RestoreMode.Force : RestoreMode.IfMissing);

            var state = this.restorer.GetRestoreState(registryReference);

            if (state is not ArtifactRestoreState.Restored)
            {
                this.logger.LogWarning(
                    "Could not restore module {Module} for the visualizer: {Message}",
                    registryReference.FullyQualifiedReference,
                    GetErrorMessage(registryReference, state));
            }

            return new(Restored: state is ArtifactRestoreState.Restored);
        }

        private static string GetErrorMessage(ArtifactReference reference, ArtifactRestoreState state)
        {
            DiagnosticBuilder.DiagnosticBuilderDelegate error = state is ArtifactRestoreState.RestoreFailed failed
                ? failed.Error
                : x => x.ArtifactRequiresRestore(reference.FullyQualifiedReference);

            return error(DiagnosticBuilder.ForDocumentStart()).Message;
        }
    }
}
