// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Features.Custom.Visualization.Models
{
    /// <summary>
    /// Well-known values for <see cref="GraphNode.Kind"/>. Modeled as strings (rather than an enum)
    /// to mirror the existing deployment graph contract and to keep wire serialization trivial.
    /// </summary>
    public static class GraphNodeKind
    {
        public const string Resource = "resource";

        public const string Module = "module";
    }

    /// <summary>
    /// A node in the server's canonical visual graph. Sizes are intentionally absent:
    /// the webview owns node measurement, so the layout engine receives sizes from the client
    /// (<see cref="RenderedGraphNode"/>) and the server only emits positions via <see cref="GraphPatch.SetNodeLayout"/>.
    /// <para>
    /// The node intentionally carries no source location (<c>filePath</c>/<c>range</c>). Those change on
    /// nearly every edit (a blank-line insertion shifts every node's range), so pushing them would force an
    /// <see cref="GraphPatch.UpdateNode"/> for every node on every edit. Instead the source location is
    /// resolved on demand when the user reveals a node (see <c>textDocument/visualGraphNodeSource</c>), which
    /// keeps the common whitespace-only edit free of metadata churn.
    /// </para>
    /// </summary>
    public record GraphNode(
        string Id,
        string Kind,
        string? ParentId,
        string Type,
        string SymbolName,
        bool IsCollection,
        bool HasChildren,
        bool HasError);

    /// <summary>
    /// The source location of a node, resolved on demand from the live compilation so that volatile
    /// <c>filePath</c>/<c>range</c> data never travels through the graph diff. See <see cref="GraphNode"/>.
    /// </summary>
    public record NodeSource(string? FilePath, Range Range);

    /// <summary>
    /// A directed dependency edge in the server's canonical visual graph. Containment (parent/child)
    /// is expressed via <see cref="GraphNode.ParentId"/>, not edges, so edges carry no kind today.
    /// </summary>
    public record GraphEdge(
        string Id,
        string SourceId,
        string TargetId);

    /// <summary>
    /// A server-computed position for a node, in graph coordinates. Pan/zoom and fit-view remain client concerns.
    /// </summary>
    public record NodeLayout(double X, double Y);

    /// <summary>
    /// The size of the bounding box enclosing the whole laid-out graph, in graph coordinates. The layout
    /// engine normalizes the graph so its top-left corner sits at the origin, so the bounds are
    /// <c>{ min: (0, 0), max: (Width, Height) }</c>. The webview fits the viewport to this so Reset Layout and
    /// Fit View settle on an identical frame without the client re-deriving module box extents.
    /// </summary>
    public record GraphBounds(double Width, double Height);

    /// <summary>
    /// The mutable subset of a <see cref="GraphNode"/> that can change without altering topology.
    /// Used by <see cref="GraphPatch.UpdateNode"/> for metadata-only updates (for example error state)
    /// so the client can refresh a node without triggering a re-layout. Null fields are left unchanged.
    /// </summary>
    public record GraphNodeChanges(
        [property: JsonProperty(NullValueHandling = NullValueHandling.Ignore)] string? Type = null,
        [property: JsonProperty(NullValueHandling = NullValueHandling.Ignore)] bool? IsCollection = null,
        [property: JsonProperty(NullValueHandling = NullValueHandling.Ignore)] bool? HasChildren = null,
        [property: JsonProperty(NullValueHandling = NullValueHandling.Ignore)] bool? HasError = null);

    /// <summary>
    /// The server's canonical visual graph, rebuilt from the live compilation on each request.
    /// </summary>
    public record CanonicalGraph(
        IReadOnlyList<GraphNode> Nodes,
        IReadOnlyList<GraphEdge> Edges,
        int ErrorCount);

    /// <summary>
    /// The graph as currently rendered by the webview, submitted with each update request so the server can diff
    /// against it. Null/empty on first load. It carries only what the server needs to reconcile: topology plus
    /// the client-measured node sizes that feed layout.
    /// </summary>
    public record RenderedGraph(
        IReadOnlyList<RenderedGraphNode> Nodes,
        IReadOnlyList<RenderedGraphEdge> Edges);

    /// <summary>
    /// The node identity, metadata, and measured size the webview submits with each update request.
    /// <para>
    /// Identity (<see cref="Id"/>/<see cref="Kind"/>/<see cref="ParentId"/>) lets the server diff topology;
    /// the metadata (<see cref="Type"/>/<see cref="IsCollection"/>/<see cref="HasChildren"/>/<see cref="HasError"/>)
    /// lets it emit an <see cref="GraphPatch.UpdateNode"/> only for nodes whose metadata actually changed,
    /// instead of a blanket refresh; and <see cref="Width"/>/<see cref="Height"/> are the client-measured
    /// sizes used as layout input.
    /// </para>
    /// </summary>
    public record RenderedGraphNode(
        string Id,
        string Kind,
        string? ParentId,
        string Type,
        bool IsCollection,
        bool HasChildren,
        bool HasError,
        double Width,
        double Height);

    /// <summary>
    /// The minimal edge identity the server needs to diff.
    /// </summary>
    public record RenderedGraphEdge(
        string Id,
        string SourceId,
        string TargetId);
}
