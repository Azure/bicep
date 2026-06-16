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
    /// diffs it against the graph the client submitted, and returns the patch delta. When topology changes it
    /// also runs the MSAGL layout engine and includes the resulting positions; metadata-only edits carry none.
    /// </summary>
    public class VisualGraphUpdateHandler : IJsonRpcRequestHandler<VisualGraphUpdateParams, VisualGraphUpdateResult>
    {
        private static readonly IReadOnlyDictionary<string, NodeSize> EmptySizes =
            new Dictionary<string, NodeSize>(StringComparer.Ordinal);

        private readonly ILogger<VisualGraphUpdateHandler> logger;

        private readonly ICompilationManager compilationManager;

        private readonly IVisualGraphLayoutEngine layoutEngine;

        public VisualGraphUpdateHandler(
            ILogger<VisualGraphUpdateHandler> logger,
            ICompilationManager compilationManager,
            IVisualGraphLayoutEngine layoutEngine)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.layoutEngine = layoutEngine;
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

            // Lay out only when topology changed. Metadata-only edits (error state, source ranges) reuse the
            // client's current positions, so no layout runs and nothing reflows.
            IReadOnlyDictionary<string, NodeLayout>? layout = null;

            if (VisualGraphDiffer.HasTopologyChange(request.Current, target))
            {
                var nodeSizes = BuildNodeSizes(request.Current);
                layout = this.layoutEngine.Layout(target, nodeSizes, VisualGraphLayoutOptions.Default, cancellationToken);
            }

            var patches = VisualGraphDiffer.Diff(request.Current, target, layout);

            return Task.FromResult(new VisualGraphUpdateResult(patches));
        }

        private static IReadOnlyDictionary<string, NodeSize> BuildNodeSizes(RenderedGraph? current)
        {
            if (current is null)
            {
                return EmptySizes;
            }

            var sizes = new Dictionary<string, NodeSize>(current.Nodes.Count, StringComparer.Ordinal);

            foreach (var node in current.Nodes)
            {
                sizes[node.Id] = new NodeSize(node.Width, node.Height);
            }

            return sizes;
        }
    }
}
