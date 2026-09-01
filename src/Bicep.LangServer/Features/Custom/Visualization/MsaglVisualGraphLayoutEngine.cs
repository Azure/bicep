// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Features.Custom.Visualization.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Msagl.Core;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Miscellaneous;
using MsaglNode = Microsoft.Msagl.Core.Layout.Node;
using MsaglPoint = Microsoft.Msagl.Core.Geometry.Point;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// Lays out the canonical graph using the core MSAGL engine (the <c>Msagl</c> package). It runs a layered
    /// (Sugiyama) top-to-bottom layout to match the visualizer's established layout direction.
    /// <para>
    /// Containment is handled by laying each scope out independently and composing the results: a module's
    /// children are laid out on their own, the module is then sized to that result (plus padding for its
    /// label), and the parent scope lays the module out as a single box before its children are lifted back
    /// into place. This sidesteps MSAGL's compound-graph (cluster) layout, which is fragile here, and is exact
    /// because the canonical graph only ever connects siblings (every dependency edge stays within one scope).
    /// </para>
    /// <para>
    /// Only node positions are read back. Edges are added to each scope's geometry graph so they influence
    /// layering, but MSAGL uses straight-line routing (the client draws its own straight edges, so spline
    /// routes would be discarded). Positions are returned in graph coordinates with a top-left origin and y
    /// increasing downward, normalized so the graph's top-left corner sits at the origin; pan/zoom and
    /// fit-view stay on the client. MSAGL's own coordinate system is y-up, so the y axis is flipped on
    /// read-out. Layout is best-effort: a recoverable MSAGL failure is logged and yields an empty result, so
    /// the handler keeps prior positions and the view stays usable. Cancellation is observed and propagated.
    /// </para>
    /// </summary>
    public class MsaglVisualGraphLayoutEngine : IVisualGraphLayoutEngine
    {
        // Sentinel scope key for top-level nodes (those with no parent). Real node ids are never empty.
        private const string RootScope = "";

        private readonly ILogger<MsaglVisualGraphLayoutEngine> logger;

        public MsaglVisualGraphLayoutEngine(ILogger<MsaglVisualGraphLayoutEngine> logger)
        {
            this.logger = logger;
        }

        public VisualGraphLayout Layout(
            CanonicalGraph graph,
            IReadOnlyDictionary<string, NodeSize> nodeSizes,
            VisualGraphLayoutOptions options,
            CancellationToken cancellationToken)
        {
            if (graph.Nodes.Count == 0)
            {
                return EmptyLayout;
            }

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var childrenByScope = GroupChildrenByScope(graph.Nodes);
                var edgesByScope = GroupEdgesByScope(graph);

                using var cancelRegistration = LinkCancellation(cancellationToken, out var cancelToken);

                var (positions, width, height) = this.LayoutScope(RootScope, childrenByScope, edgesByScope, nodeSizes, options, cancelToken);

                // The engine normalizes the content's top-left corner to the origin, so the bounds the graph
                // occupies are exactly { (0, 0), (width, height) }.
                return new VisualGraphLayout(positions, new GraphBounds(width, height));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                this.logger.LogWarning(
                    exception,
                    "MSAGL layout failed for a graph with {NodeCount} node(s) and {EdgeCount} edge(s); keeping previous positions.",
                    graph.Nodes.Count,
                    graph.Edges.Count);

                return EmptyLayout;
            }
        }

        /// <summary>
        /// Lays out a single scope (a set of sibling nodes), recursively laying out any module among them
        /// first. Returns absolute positions for the scope's whole subtree in coordinates local to the scope
        /// (origin at its top-left), plus the size of the laid-out content.
        /// </summary>
        private (IReadOnlyDictionary<string, NodeLayout> Positions, double Width, double Height) LayoutScope(
            string scope,
            IReadOnlyDictionary<string, IReadOnlyList<GraphNode>> childrenByScope,
            IReadOnlyDictionary<string, IReadOnlyList<GraphEdge>> edgesByScope,
            IReadOnlyDictionary<string, NodeSize> nodeSizes,
            VisualGraphLayoutOptions options,
            CancelToken cancelToken)
        {
            var children = childrenByScope.TryGetValue(scope, out var scopedChildren) ? scopedChildren : [];
            var sizeByChild = new Dictionary<string, NodeSize>(children.Count, StringComparer.Ordinal);
            var subtreeByModule = new Dictionary<string, IReadOnlyDictionary<string, NodeLayout>>(StringComparer.Ordinal);

            // Lay out module contents first so each module can be sized as a single box in this scope's layout.
            foreach (var child in children)
            {
                if (child.HasChildren)
                {
                    var subtree = this.LayoutScope(child.Id, childrenByScope, edgesByScope, nodeSizes, options, cancelToken);

                    subtreeByModule[child.Id] = subtree.Positions;
                    sizeByChild[child.Id] = new NodeSize(
                        subtree.Width + options.ModulePadding.Left + options.ModulePadding.Right,
                        subtree.Height + options.ModulePadding.Top + options.ModulePadding.Bottom);
                }
                else
                {
                    sizeByChild[child.Id] = ResolveSize(child.Id, nodeSizes, options);
                }
            }

            var edges = edgesByScope.TryGetValue(scope, out var scopedEdges) ? scopedEdges : [];
            var (localPositions, width, height) = FlatLayout(children, sizeByChild, edges, options, cancelToken);

            // Lift each module's subtree into place relative to the module's slot in this scope.
            var positions = new Dictionary<string, NodeLayout>(StringComparer.Ordinal);

            foreach (var child in children)
            {
                var childPosition = localPositions[child.Id];
                positions[child.Id] = childPosition;

                if (subtreeByModule.TryGetValue(child.Id, out var subtree))
                {
                    var offsetX = childPosition.X + options.ModulePadding.Left;
                    var offsetY = childPosition.Y + options.ModulePadding.Top;

                    foreach (var (innerId, innerPosition) in subtree)
                    {
                        positions[innerId] = new NodeLayout(innerPosition.X + offsetX, innerPosition.Y + offsetY);
                    }
                }
            }

            return (positions, width, height);
        }

        /// <summary>
        /// Runs the layered MSAGL layout over a flat set of sized sibling nodes and their edges, returning each
        /// node's top-left position (normalized so the content's top-left is the origin) and the content size.
        /// </summary>
        private static (IReadOnlyDictionary<string, NodeLayout> Positions, double Width, double Height) FlatLayout(
            IReadOnlyList<GraphNode> nodes,
            IReadOnlyDictionary<string, NodeSize> sizeByNode,
            IReadOnlyList<GraphEdge> edges,
            VisualGraphLayoutOptions options,
            CancelToken cancelToken)
        {
            cancelToken.ThrowIfCanceled();

            // A single isolated node needs no layout pass; place it at the origin to avoid layout edge cases.
            if (nodes.Count == 1 && edges.Count == 0)
            {
                var only = nodes[0];
                var onlySize = sizeByNode[only.Id];

                return (Single(only.Id, new NodeLayout(0, 0)), onlySize.Width, onlySize.Height);
            }

            var geometryGraph = new GeometryGraph();
            var elementsById = new Dictionary<string, MsaglNode>(nodes.Count, StringComparer.Ordinal);

            foreach (var node in nodes)
            {
                var size = sizeByNode[node.Id];
                var element = new MsaglNode(CurveFactory.CreateRectangle(size.Width, size.Height, new MsaglPoint()), node.Id);

                elementsById[node.Id] = element;
                geometryGraph.Nodes.Add(element);
            }

            foreach (var edge in edges)
            {
                if (elementsById.TryGetValue(edge.SourceId, out var source) &&
                    elementsById.TryGetValue(edge.TargetId, out var target))
                {
                    geometryGraph.Edges.Add(new Edge(source, target));
                }
            }

            var settings = new SugiyamaLayoutSettings
            {
                LayerSeparation = options.LayerSeparation,
                NodeSeparation = options.NodeSeparation,
                EdgeRoutingSettings = { EdgeRoutingMode = EdgeRoutingMode.StraightLine },
            };

            LayoutHelpers.CalculateLayout(geometryGraph, settings, cancelToken);

            return ReadFlatLayout(nodes, elementsById);
        }

        private static (IReadOnlyDictionary<string, NodeLayout> Positions, double Width, double Height) ReadFlatLayout(
            IReadOnlyList<GraphNode> nodes,
            Dictionary<string, MsaglNode> elementsById)
        {
            // Normalize so the content's top-left corner is the origin. MSAGL is y-up (a rectangle's Top is its
            // larger y), so the topmost node maps to y = 0 and y grows downward, as the client expects.
            var minLeft = double.PositiveInfinity;
            var minBottom = double.PositiveInfinity;
            var maxRight = double.NegativeInfinity;
            var maxTop = double.NegativeInfinity;

            foreach (var element in elementsById.Values)
            {
                var box = element.BoundingBox;
                minLeft = Math.Min(minLeft, box.Left);
                minBottom = Math.Min(minBottom, box.Bottom);
                maxRight = Math.Max(maxRight, box.Right);
                maxTop = Math.Max(maxTop, box.Top);
            }

            var positions = new Dictionary<string, NodeLayout>(nodes.Count, StringComparer.Ordinal);

            foreach (var node in nodes)
            {
                var box = elementsById[node.Id].BoundingBox;
                positions[node.Id] = new NodeLayout(box.Left - minLeft, maxTop - box.Top);
            }

            return (positions, maxRight - minLeft, maxTop - minBottom);
        }

        private static IReadOnlyDictionary<string, IReadOnlyList<GraphNode>> GroupChildrenByScope(IReadOnlyList<GraphNode> nodes)
        {
            var childrenByScope = new Dictionary<string, List<GraphNode>>(StringComparer.Ordinal);

            foreach (var node in nodes)
            {
                var scope = node.ParentId ?? RootScope;

                if (!childrenByScope.TryGetValue(scope, out var children))
                {
                    children = [];
                    childrenByScope[scope] = children;
                }

                children.Add(node);
            }

            return childrenByScope.ToDictionary(pair => pair.Key, pair => (IReadOnlyList<GraphNode>)pair.Value, StringComparer.Ordinal);
        }

        private static IReadOnlyDictionary<string, IReadOnlyList<GraphEdge>> GroupEdgesByScope(CanonicalGraph graph)
        {
            // Every edge connects two siblings, so the scope of an edge is the parent of its endpoints.
            var scopeByNodeId = graph.Nodes.ToDictionary(node => node.Id, node => node.ParentId ?? RootScope, StringComparer.Ordinal);
            var edgesByScope = new Dictionary<string, List<GraphEdge>>(StringComparer.Ordinal);

            foreach (var edge in graph.Edges)
            {
                if (!scopeByNodeId.TryGetValue(edge.SourceId, out var scope))
                {
                    continue;
                }

                if (!edgesByScope.TryGetValue(scope, out var edges))
                {
                    edges = [];
                    edgesByScope[scope] = edges;
                }

                edges.Add(edge);
            }

            return edgesByScope.ToDictionary(pair => pair.Key, pair => (IReadOnlyList<GraphEdge>)pair.Value, StringComparer.Ordinal);
        }

        private static NodeSize ResolveSize(
            string id,
            IReadOnlyDictionary<string, NodeSize> nodeSizes,
            VisualGraphLayoutOptions options) =>
            nodeSizes.TryGetValue(id, out var size) && size.Width > 0 && size.Height > 0
                ? size
                : options.DefaultNodeSize;

        private static IReadOnlyDictionary<string, NodeLayout> Single(string id, NodeLayout position) =>
            new Dictionary<string, NodeLayout>(StringComparer.Ordinal) { [id] = position };

        private static CancellationTokenRegistration LinkCancellation(CancellationToken cancellationToken, out CancelToken cancelToken)
        {
            // Bridge the framework token onto MSAGL's own cancellation flag, which its layout loops poll.
            var msaglToken = new CancelToken();
            cancelToken = msaglToken;

            return cancellationToken.Register(() => msaglToken.Canceled = true);
        }

        private static readonly IReadOnlyDictionary<string, NodeLayout> EmptyPositions =
            new Dictionary<string, NodeLayout>(StringComparer.Ordinal);

        // Returned when no layout can be produced (empty graph or recoverable failure): no positions and no
        // bounds, so callers keep any previously known positions and skip fitting the view.
        private static readonly VisualGraphLayout EmptyLayout = new(EmptyPositions, null);
    }
}
