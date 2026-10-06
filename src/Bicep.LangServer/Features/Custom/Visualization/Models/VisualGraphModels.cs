// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
    /// A node in the server's canonical visual graph. Sizes are intentionally absent: the webview owns node
    /// measurement, so the layout engine receives sizes from the client (<see cref="MeasuredGraphNode"/>).
    /// <para>
    /// The node carries no source location (<c>filePath</c>/<c>range</c>). Those change on nearly every edit,
    /// so they are resolved on demand when the user reveals a node (see <c>textDocument/visualGraphNodeSource</c>),
    /// which keeps an unchanged graph identical from one edit to the next.
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
    /// <c>filePath</c>/<c>range</c> data never travels with the graph. See <see cref="GraphNode"/>.
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
    /// A node's server-computed position, as sent to the client. A list of these rather than a dictionary keeps
    /// node ids out of JSON property names, which the LSP serializer would camel-case.
    /// </summary>
    public record NodePosition(string NodeId, double X, double Y);

    /// <summary>
    /// The size of the bounding box enclosing the whole laid-out graph, in graph coordinates. The layout
    /// engine normalizes the graph so its top-left corner sits at the origin, so the bounds are
    /// <c>{ min: (0, 0), max: (Width, Height) }</c>. The webview fits the viewport to this so Reset Layout and
    /// Fit View settle on an identical frame without the client re-deriving module box extents.
    /// </summary>
    public record GraphBounds(double Width, double Height);

    /// <summary>
    /// The server's canonical visual graph, rebuilt from the live compilation on each request.
    /// </summary>
    public record CanonicalGraph(
        IReadOnlyList<GraphNode> Nodes,
        IReadOnlyList<GraphEdge> Edges);

    /// <summary>
    /// The graph the webview has rendered, with the size it measured for each node: the input to layout. The
    /// server checks its topology against the live compilation before laying it out.
    /// </summary>
    public record MeasuredGraph(
        IReadOnlyList<MeasuredGraphNode> Nodes,
        IReadOnlyList<GraphEdge> Edges);

    /// <summary>
    /// A node's identity and containment, which must match the live compilation, and its client-measured size.
    /// </summary>
    public record MeasuredGraphNode(
        string Id,
        string Kind,
        string? ParentId,
        double Width,
        double Height);
}
