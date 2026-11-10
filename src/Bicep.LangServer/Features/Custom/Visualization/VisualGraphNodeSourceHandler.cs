// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Compilation;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// Handles <c>textDocument/visualGraphNodeSource</c>: maps a node id to its source location from the live
    /// compilation, so the webview can reveal a node on demand without the graph carrying volatile range/file
    /// data. Returns a null location when the node no longer exists.
    /// </summary>
    public class VisualGraphNodeSourceHandler : IJsonRpcRequestHandler<VisualGraphNodeSourceParams, VisualGraphNodeSourceResult>
    {
        private readonly ILogger<VisualGraphNodeSourceHandler> logger;

        private readonly ICompilationManager compilationManager;

        private readonly VisualGraphBuilder graphBuilder;

        public VisualGraphNodeSourceHandler(
            ILogger<VisualGraphNodeSourceHandler> logger,
            ICompilationManager compilationManager,
            VisualGraphBuilder graphBuilder)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.graphBuilder = graphBuilder;
        }

        public Task<VisualGraphNodeSourceResult> Handle(VisualGraphNodeSourceParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context is null)
            {
                this.logger.LogError("Visual graph node source request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                return Task.FromResult(new VisualGraphNodeSourceResult(Uri: null, Range: null));
            }

            var sources = this.graphBuilder.Build(context).Sources;

            if (!sources.TryGetValue(request.NodeId, out var source))
            {
                return Task.FromResult(new VisualGraphNodeSourceResult(Uri: null, Range: null));
            }

            return Task.FromResult(new VisualGraphNodeSourceResult(source.Uri, source.Range));
        }
    }
}
