// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Features.Custom.Visualization.Models;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// The client-measured size of a node, used purely as layout input. The server's canonical graph does
    /// not carry sizes (the webview owns node measurement), so the engine receives them separately and falls
    /// back to a deterministic default for any node the client has not measured yet (for example on initial
    /// load, where the client submits no graph at all).
    /// </summary>
    public record NodeSize(double Width, double Height);

    /// <summary>
    /// Padding reserved inside a module box around its laid-out children.
    /// </summary>
    public sealed record ModulePadding
    {
        public ModulePadding(double top, double right, double bottom, double left)
        {
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
            this.Left = left;
        }

        public double Top { get; init; }

        public double Right { get; init; }

        public double Bottom { get; init; }

        public double Left { get; init; }
    }

    /// <summary>
    /// Shared layout defaults and optional renderer-provided overrides. These defaults travel with the layout
    /// engine API rather than a particular client, so future callers such as the CLI can use the engine without
    /// duplicating webview-specific constants.
    /// </summary>
    public sealed record VisualGraphLayoutOptions
    {
        public static VisualGraphLayoutOptions Default { get; } = new(
            defaultNodeSize: new NodeSize(200, 80),
            nodeSeparation: 60,
            layerSeparation: 50,
            modulePadding: new ModulePadding(top: 50, right: 40, bottom: 40, left: 40));

        public VisualGraphLayoutOptions(
            NodeSize defaultNodeSize,
            double nodeSeparation,
            double layerSeparation,
            ModulePadding modulePadding)
        {
            this.DefaultNodeSize = defaultNodeSize;
            this.NodeSeparation = nodeSeparation;
            this.LayerSeparation = layerSeparation;
            this.ModulePadding = modulePadding;
        }

        public NodeSize DefaultNodeSize { get; init; }

        public double NodeSeparation { get; init; }

        public double LayerSeparation { get; init; }

        public ModulePadding ModulePadding { get; init; }
    }

    /// <summary>
    /// Maps the canonical <see cref="CanonicalGraph"/> onto a layout engine's geometry model and returns a
    /// position for every node, in graph coordinates. The engine computes positions only; the webview owns
    /// all rendering, pan/zoom, and fit-view, so no edge routes or viewport-relative offsets are produced.
    /// </summary>
    public interface IVisualGraphLayoutEngine
    {
        /// <summary>
        /// Computes a position for every node in <paramref name="graph"/>.
        /// </summary>
        /// <param name="graph">The canonical graph to lay out.</param>
        /// <param name="nodeSizes">
        /// Client-measured sizes keyed by node id. Missing or non-positive sizes fall back to a deterministic
        /// default; container nodes (modules with children) are sized by the engine from their contents.
        /// </param>
        /// <param name="options">Shared defaults and renderer-provided layout overrides.</param>
        /// <param name="cancellationToken">Cancels the layout computation.</param>
        /// <returns>
        /// A position per node id and the overall graph bounds, in graph coordinates with a top-left origin and
        /// y increasing downward. An empty result (no positions, null bounds) means no layout was produced (the
        /// graph was empty or a recoverable failure occurred); callers should keep any previously known
        /// positions in that case.
        /// </returns>
        VisualGraphLayout Layout(
            CanonicalGraph graph,
            IReadOnlyDictionary<string, NodeSize> nodeSizes,
            VisualGraphLayoutOptions options,
            CancellationToken cancellationToken);
    }

    /// <summary>
    /// The result of a layout pass: a position per node id, plus the bounding box the whole graph occupies
    /// (null when no layout was produced). The engine already computes module box extents (children plus
    /// padding) to size containers, so it returns the overall bounds for free, sparing the client from
    /// re-deriving them.
    /// </summary>
    public record VisualGraphLayout(IReadOnlyDictionary<string, NodeLayout> Positions, GraphBounds? Bounds);
}
