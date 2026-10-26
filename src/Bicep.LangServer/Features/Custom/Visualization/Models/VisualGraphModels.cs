// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Security.Cryptography;
using System.Text;
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
        IReadOnlyList<GraphEdge> Edges)
    {
        /// <summary>
        /// Identifies the graph's topology: its nodes (by id, kind, and parent) and its edges (by id and endpoints).
        /// Metadata such as error state is not topology, so a metadata-only edit keeps the revision, and node and
        /// edge order are irrelevant.
        /// </summary>
        public string GetRevision()
        {
            var builder = new StringBuilder();

            foreach (var node in this.Nodes.OrderBy(node => node.Id, StringComparer.Ordinal))
            {
                builder.Append("n\0").Append(node.Id).Append('\0').Append(node.Kind).Append('\0').Append(node.ParentId).Append('\n');
            }

            foreach (var edge in this.Edges.OrderBy(edge => edge.Id, StringComparer.Ordinal))
            {
                builder.Append("e\0").Append(edge.Id).Append('\0').Append(edge.SourceId).Append('\0').Append(edge.TargetId).Append('\n');
            }

            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString())));
        }
    }

    /// <summary>
    /// The graph the webview renders, with the size it measured for each node: the input to layout. It may omit
    /// nodes the webview hides, such as the contents of a collapsed module.
    /// </summary>
    public record MeasuredGraph(
        IReadOnlyList<MeasuredGraphNode> Nodes,
        IReadOnlyList<GraphEdge> Edges)
    {
        /// <summary>
        /// Returns the graph to lay out, or null when this graph is not well formed: duplicate ids, a parent or edge
        /// endpoint that is not in the graph, or a containment cycle.
        /// </summary>
        /// <remarks>
        /// Layout reads only containment and edges; other node metadata is not part of the measured graph and is
        /// left empty.
        /// </remarks>
        public CanonicalGraph? ToLayoutGraph()
        {
            var nodesById = new Dictionary<string, MeasuredGraphNode>(StringComparer.Ordinal);

            foreach (var node in this.Nodes)
            {
                if (!nodesById.TryAdd(node.Id, node))
                {
                    return null;
                }
            }

            var parentIds = new HashSet<string>(StringComparer.Ordinal);

            foreach (var node in this.Nodes)
            {
                if (node.ParentId is null)
                {
                    continue;
                }

                if (!nodesById.ContainsKey(node.ParentId) || HasContainmentCycle(node, nodesById))
                {
                    return null;
                }

                parentIds.Add(node.ParentId);
            }

            var edgeIds = new HashSet<string>(StringComparer.Ordinal);

            foreach (var edge in this.Edges)
            {
                if (!edgeIds.Add(edge.Id) || !nodesById.ContainsKey(edge.SourceId) || !nodesById.ContainsKey(edge.TargetId))
                {
                    return null;
                }
            }

            var nodes = this.Nodes
                .Select(node => new GraphNode(
                    Id: node.Id,
                    Kind: node.Kind,
                    ParentId: node.ParentId,
                    Type: string.Empty,
                    SymbolName: string.Empty,
                    IsCollection: false,
                    HasChildren: parentIds.Contains(node.Id),
                    HasError: false))
                .ToArray();

            return new CanonicalGraph(nodes, this.Edges);
        }

        private static bool HasContainmentCycle(MeasuredGraphNode node, IReadOnlyDictionary<string, MeasuredGraphNode> nodesById)
        {
            var visited = new HashSet<string>(StringComparer.Ordinal) { node.Id };

            for (var parentId = node.ParentId; parentId is not null; parentId = nodesById.GetValueOrDefault(parentId)?.ParentId)
            {
                if (!visited.Add(parentId))
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// A node's identity and containment, and its client-measured size.
    /// </summary>
    public record MeasuredGraphNode(
        string Id,
        string Kind,
        string? ParentId,
        double Width,
        double Height);
}
