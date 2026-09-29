// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Compilation;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// Handles <c>textDocument/prepareVisualResourceReplay</c>: for each resource creation in the visual designer's
    /// undo history, returns the edit that undoes or redoes it exactly, or null when that is no longer possible. The
    /// designer enables Undo and Redo only for creations with an edit, and the client applies the edit itself.
    /// </summary>
    public class PrepareVisualResourceReplayHandler : IJsonRpcRequestHandler<PrepareVisualResourceReplayParams, PrepareVisualResourceReplayResult>
    {
        private readonly ILogger<PrepareVisualResourceReplayHandler> logger;

        private readonly ICompilationManager compilationManager;

        private readonly IVisualResourceEditingService editingService;

        public PrepareVisualResourceReplayHandler(
            ILogger<PrepareVisualResourceReplayHandler> logger,
            ICompilationManager compilationManager,
            IVisualResourceEditingService editingService)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.editingService = editingService;
        }

        public Task<PrepareVisualResourceReplayResult> Handle(PrepareVisualResourceReplayParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context is null)
            {
                this.logger.LogError("Prepare visual resource replay request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                throw new RpcErrorException(ErrorCodes.RequestFailed, string.Empty, $"The document \"{request.TextDocument.Uri}\" is not currently compiled.");
            }

            return Task.FromResult(this.editingService.PrepareResourceReplays(context, request));
        }
    }
}
