// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Features.Custom.Visualization.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// Handles <c>textDocument/visualGraphUpdate</c>: builds the canonical graph from the live compilation,
    /// diffs it against the graph the client submitted, and returns the patch delta. No layout is computed yet.
    /// </summary>
    public class VisualGraphUpdateHandler : IJsonRpcRequestHandler<VisualGraphUpdateParams, VisualGraphUpdateResult>
    {
        private readonly ILogger<VisualGraphUpdateHandler> logger;

        private readonly ICompilationManager compilationManager;

        public VisualGraphUpdateHandler(ILogger<VisualGraphUpdateHandler> logger, ICompilationManager compilationManager)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
        }

        public Task<VisualGraphUpdateResult> Handle(VisualGraphUpdateParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context is null)
            {
                this.logger.LogError("Visual graph update request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                // The client keeps what it has; the next document change reconciles once compilation is ready.
                return Task.FromResult(new VisualGraphUpdateResult([]));
            }

            var target = VisualGraphBuilder.Build(context, request.TextDocument.Uri.ToIOUri());
            var patches = VisualGraphDiffer.Diff(request.Current, target);

            return Task.FromResult(new VisualGraphUpdateResult(patches));
        }
    }
}
