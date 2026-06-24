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
    /// diffs it against the graph topology the client submitted, and returns the topology/metadata patch delta.
    /// Layout is computed later by <see cref="VisualGraphLayoutHandler"/> after the client renders and measures nodes.
    /// </summary>
    public class VisualGraphUpdateHandler : IJsonRpcRequestHandler<VisualGraphUpdateParams, VisualGraphUpdateResult>
    {
        private readonly ILogger<VisualGraphUpdateHandler> logger;

        private readonly ICompilationManager compilationManager;

        public VisualGraphUpdateHandler(
            ILogger<VisualGraphUpdateHandler> logger,
            ICompilationManager compilationManager)
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

    /// <summary>
    /// Handles <c>textDocument/visualGraphLayout</c>: validates the measured graph against the live compilation,
    /// runs MSAGL with actual client-measured sizes, and returns layout patches.
    /// </summary>
    public class VisualGraphLayoutHandler : IJsonRpcRequestHandler<VisualGraphLayoutParams, VisualGraphLayoutResult>
    {
        private readonly ILogger<VisualGraphLayoutHandler> logger;

        private readonly ICompilationManager compilationManager;

        private readonly IVisualGraphLayoutEngine layoutEngine;

        public VisualGraphLayoutHandler(
            ILogger<VisualGraphLayoutHandler> logger,
            ICompilationManager compilationManager,
            IVisualGraphLayoutEngine layoutEngine)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.layoutEngine = layoutEngine;
        }

        public Task<VisualGraphLayoutResult> Handle(VisualGraphLayoutParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context is null)
            {
                this.logger.LogError("Visual graph layout request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                return Task.FromResult(new VisualGraphLayoutResult(VisualGraphLayoutStatus.GraphChanged, []));
            }

            var target = VisualGraphBuilder.Build(context, request.TextDocument.Uri.ToIOUri());

            if (VisualGraphDiffer.HasTopologyChange(request.Current, target))
            {
                return Task.FromResult(new VisualGraphLayoutResult(VisualGraphLayoutStatus.GraphChanged, []));
            }

            var nodeSizes = BuildNodeSizes(request.Current);
            var layout = this.layoutEngine.Layout(target, nodeSizes, request.Options ?? VisualGraphLayoutOptions.Default, cancellationToken);

            if (target.Nodes.Count > 0 && layout.Positions.Count == 0)
            {
                return Task.FromResult(new VisualGraphLayoutResult(VisualGraphLayoutStatus.LayoutFailed, []));
            }

            var layoutPatches = target.Nodes
                .OrderBy(node => node.Id, StringComparer.Ordinal)
                .Where(node => layout.Positions.ContainsKey(node.Id))
                .Select(node => (GraphPatch)new GraphPatch.SetNodeLayout(node.Id, layout.Positions[node.Id]));

            // The engine returns the overall graph bounds for free (it already sizes module boxes), so the
            // webview fits the viewport to these instead of re-deriving them client-side.
            var patches = layout.Bounds is { } bounds
                ? [.. layoutPatches, new GraphPatch.SetGraphBounds(bounds)]
                : layoutPatches.ToArray();

            return Task.FromResult(new VisualGraphLayoutResult(VisualGraphLayoutStatus.Ok, patches));
        }

        private static IReadOnlyDictionary<string, NodeSize> BuildNodeSizes(RenderedGraph current)
        {
            var sizes = new Dictionary<string, NodeSize>(current.Nodes.Count, StringComparer.Ordinal);

            foreach (var node in current.Nodes)
            {
                sizes[node.Id] = new NodeSize(node.Width, node.Height);
            }

            return sizes;
        }
    }

    /// <summary>
    /// Handles <c>textDocument/visualGraphNodeSource</c>: maps a node id to its source location from the live
    /// compilation, so the webview can reveal a node on demand without the graph carrying volatile range/file
    /// data. Returns <see cref="VisualGraphNodeSourceResult.Found"/> = false when the node no longer exists.
    /// </summary>
    public class VisualGraphNodeSourceHandler : IJsonRpcRequestHandler<VisualGraphNodeSourceParams, VisualGraphNodeSourceResult>
    {
        private readonly ILogger<VisualGraphNodeSourceHandler> logger;

        private readonly ICompilationManager compilationManager;

        public VisualGraphNodeSourceHandler(
            ILogger<VisualGraphNodeSourceHandler> logger,
            ICompilationManager compilationManager)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
        }

        public Task<VisualGraphNodeSourceResult> Handle(VisualGraphNodeSourceParams request, CancellationToken cancellationToken)
        {
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);

            if (context is null)
            {
                this.logger.LogError("Visual graph node source request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                return Task.FromResult(new VisualGraphNodeSourceResult(Found: false, FilePath: null, Range: null));
            }

            var (_, sources) = VisualGraphBuilder.BuildWithSources(context, request.TextDocument.Uri.ToIOUri());

            if (!sources.TryGetValue(request.NodeId, out var source))
            {
                return Task.FromResult(new VisualGraphNodeSourceResult(Found: false, FilePath: null, Range: null));
            }

            return Task.FromResult(new VisualGraphNodeSourceResult(Found: true, source.FilePath, source.Range));
        }
    }
}
