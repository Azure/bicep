// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Compilation;
using Bicep.LanguageServer.Features.Custom.Visualization.Models;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// Handles <c>textDocument/visualGraphLayout</c>: checks that the client's graph revision is still current,
    /// runs MSAGL on the graph the client renders with its measured sizes, and returns node positions and the
    /// graph's bounds.
    /// </summary>
    public class VisualGraphLayoutHandler : IJsonRpcRequestHandler<VisualGraphLayoutParams, VisualGraphLayoutResult>
    {
        private readonly ILogger<VisualGraphLayoutHandler> logger;

        private readonly ICompilationManager compilationManager;

        private readonly IVisualGraphLayoutEngine layoutEngine;

        private readonly VisualGraphBuilder graphBuilder;

        public VisualGraphLayoutHandler(
            ILogger<VisualGraphLayoutHandler> logger,
            ICompilationManager compilationManager,
            IVisualGraphLayoutEngine layoutEngine,
            VisualGraphBuilder graphBuilder)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.layoutEngine = layoutEngine;
            this.graphBuilder = graphBuilder;
        }

        public async Task<VisualGraphLayoutResult> Handle(VisualGraphLayoutParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context is null)
            {
                this.logger.LogError("Visual graph layout request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                return new VisualGraphLayoutResult(VisualGraphLayoutStatus.GraphChanged, [], null);
            }

            var liveGraph = this.graphBuilder.Build(context).Graph;

            if (request.Revision != liveGraph.GetRevision())
            {
                return new VisualGraphLayoutResult(VisualGraphLayoutStatus.GraphChanged, [], null);
            }

            if (request.Graph.ToLayoutGraph() is not { } target)
            {
                this.logger.LogWarning("Visual graph layout request for {Uri} contained a malformed graph.", request.TextDocument.Uri);

                return new VisualGraphLayoutResult(VisualGraphLayoutStatus.LayoutFailed, [], null);
            }

            var nodeSizes = request.Graph.Nodes.ToDictionary(node => node.Id, node => new NodeSize(node.Width, node.Height), StringComparer.Ordinal);
            var options = request.Options ?? VisualGraphLayoutOptions.Default;

            // Offload the CPU-bound MSAGL layout so a pathological graph cannot block the request dispatch
            // thread; cancellation flows through to abandon a layout the client no longer cares about.
            var layout = await Task.Run(() => this.layoutEngine.Layout(target, nodeSizes, options, cancellationToken), cancellationToken);

            if (target.Nodes.Count > 0 && layout.Positions.Count == 0)
            {
                return new VisualGraphLayoutResult(VisualGraphLayoutStatus.LayoutFailed, [], null);
            }

            var positions = target.Nodes
                .OrderBy(node => node.Id, StringComparer.Ordinal)
                .Where(node => layout.Positions.ContainsKey(node.Id))
                .Select(node => new NodePosition(node.Id, layout.Positions[node.Id].X, layout.Positions[node.Id].Y))
                .ToArray();

            return new VisualGraphLayoutResult(VisualGraphLayoutStatus.Ok, positions, layout.Bounds);
        }
    }
}
