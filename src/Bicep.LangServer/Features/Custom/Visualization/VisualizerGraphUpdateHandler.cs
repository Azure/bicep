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
    /// Handles <c>textDocument/visualizerGraphUpdate</c>: builds the canonical graph from the live compilation,
    /// diffs it against the graph the client submitted, and returns the patch delta. No layout is computed yet.
    /// </summary>
    public class VisualizerGraphUpdateHandler : IJsonRpcRequestHandler<VisualizerGraphUpdateParams, VisualizerGraphUpdateResult>
    {
        private readonly ILogger<VisualizerGraphUpdateHandler> logger;

        private readonly ICompilationManager compilationManager;

        public VisualizerGraphUpdateHandler(ILogger<VisualizerGraphUpdateHandler> logger, ICompilationManager compilationManager)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
        }

        public Task<VisualizerGraphUpdateResult> Handle(VisualizerGraphUpdateParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context is null)
            {
                this.logger.LogError("Visualizer graph update request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                // The client keeps what it has; the next document change reconciles once compilation is ready.
                return Task.FromResult(new VisualizerGraphUpdateResult([]));
            }

            var target = VisualizerGraphBuilder.Build(context, request.TextDocument.Uri.ToIOUri());
            var patches = VisualizerGraphDiffer.Diff(request.Current, target);

            return Task.FromResult(new VisualizerGraphUpdateResult(patches));
        }
    }
}
