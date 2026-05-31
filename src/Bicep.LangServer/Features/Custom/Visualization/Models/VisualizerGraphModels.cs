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
    /// A node in the server's canonical visualizer graph. Sizes are intentionally absent:
    /// the webview owns node measurement, so the layout engine receives sizes from the client
    /// (<see cref="ClientGraphNode"/>) and the server only emits positions via <see cref="GraphPatch.SetNodeLayout"/>.
    /// </summary>
    public record GraphNode(
        string Id,
        string Kind,
        string? ParentId,
        string Type,
        string SymbolName,
        bool IsCollection,
        bool HasChildren,
        bool HasError,
        string? FilePath,
        Range Range);

    /// <summary>
    /// A directed dependency edge in the server's canonical visualizer graph. Containment (parent/child)
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
    /// The mutable subset of a <see cref="GraphNode"/> that can change without altering topology.
    /// Used by <see cref="GraphPatch.UpdateNode"/> for metadata-only updates (for example error state or source ranges)
    /// so the client can refresh a node without triggering a re-layout. Null fields are left unchanged.
    /// </summary>
    public record GraphNodeChanges(
        string? Type = null,
        bool? IsCollection = null,
        bool? HasChildren = null,
        bool? HasError = null,
        string? FilePath = null,
        Range? Range = null);

    /// <summary>
    /// The graph the webview currently displays, submitted with each update request so the server can diff against it.
    /// Null/empty on first load.
    /// </summary>
    public record ClientGraph(
        IReadOnlyList<ClientGraphNode> Nodes,
        IReadOnlyList<ClientGraphEdge> Edges);

    /// <summary>
    /// The minimal node identity the server needs to diff and to decide whether layout must run.
    /// <see cref="Width"/>/<see cref="Height"/> are the client-measured sizes used as layout input.
    /// </summary>
    public record ClientGraphNode(
        string Id,
        string Kind,
        string? ParentId,
        double Width,
        double Height);

    /// <summary>
    /// The minimal edge identity the server needs to diff.
    /// </summary>
    public record ClientGraphEdge(
        string Id,
        string SourceId,
        string TargetId);
}
