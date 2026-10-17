// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Features.Custom.Visualization.Models;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    public static class VisualGraphTopology
    {
        /// <summary>
        /// Projects the canonical graph to what a client with the specified modules collapsed renders.
        /// Dependencies crossing a collapsed boundary terminate at the module; internal dependencies disappear.
        /// </summary>
        public static CanonicalGraph Project(CanonicalGraph graph, IReadOnlySet<string> collapsedNodeIds)
        {
            var nodesById = graph.Nodes.ToDictionary(node => node.Id, StringComparer.Ordinal);
            var validCollapsedNodeIds = collapsedNodeIds
                .Where(nodeId => nodesById.TryGetValue(nodeId, out var node) &&
                    node.Kind == GraphNodeKind.Module &&
                    node.HasChildren)
                .ToHashSet(StringComparer.Ordinal);

            if (validCollapsedNodeIds.Count == 0)
            {
                return graph;
            }

            var visibleAncestorByNodeId = new Dictionary<string, string>(StringComparer.Ordinal);

            string GetVisibleAncestor(string nodeId)
            {
                if (visibleAncestorByNodeId.TryGetValue(nodeId, out var cached))
                {
                    return cached;
                }

                string? currentId = nodeId;
                var visibleId = nodeId;

                while (currentId is not null)
                {
                    if (validCollapsedNodeIds.Contains(currentId))
                    {
                        visibleId = currentId;
                    }

                    currentId = nodesById.GetValueOrDefault(currentId)?.ParentId;
                }

                visibleAncestorByNodeId[nodeId] = visibleId;
                return visibleId;
            }

            var nodes = graph.Nodes
                .Where(node => GetVisibleAncestor(node.Id) == node.Id)
                .Select(node => validCollapsedNodeIds.Contains(node.Id) ? node with { HasChildren = false } : node)
                .ToArray();
            var endpointPairs = new HashSet<(string SourceId, string TargetId)>();
            var edges = new List<GraphEdge>();

            foreach (var edge in graph.Edges)
            {
                var sourceId = GetVisibleAncestor(edge.SourceId);
                var targetId = GetVisibleAncestor(edge.TargetId);

                if (sourceId == targetId || !endpointPairs.Add((sourceId, targetId)))
                {
                    continue;
                }

                var id = sourceId == edge.SourceId && targetId == edge.TargetId
                    ? edge.Id
                    : $"{sourceId}->{targetId}";
                edges.Add(new GraphEdge(id, sourceId, targetId));
            }

            return new CanonicalGraph(nodes, edges);
        }

        /// <summary>
        /// Returns an expanded module and its descendants, with the dependencies wholly among them, or null when
        /// <paramref name="rootNodeId"/> is not a module with children in <paramref name="graph"/>. Node parents are
        /// unchanged, so the result can be matched against a measured subtree.
        /// </summary>
        public static CanonicalGraph? Subtree(CanonicalGraph graph, string rootNodeId)
        {
            var root = graph.Nodes.FirstOrDefault(node => node.Id == rootNodeId);

            if (root is not { Kind: GraphNodeKind.Module, HasChildren: true })
            {
                return null;
            }

            var descendantPrefix = $"{rootNodeId}::";
            var nodes = graph.Nodes
                .Where(node => node.Id == rootNodeId || node.Id.StartsWith(descendantPrefix, StringComparison.Ordinal))
                .ToArray();
            var nodeIds = nodes.Select(node => node.Id).ToHashSet(StringComparer.Ordinal);
            var edges = graph.Edges
                .Where(edge => nodeIds.Contains(edge.SourceId) && nodeIds.Contains(edge.TargetId))
                .ToArray();

            return new CanonicalGraph(nodes, edges);
        }

        /// <summary>
        /// Returns whether a measured graph has the same topology as the live graph: the same nodes (by id, kind,
        /// and parent) and the same edges (by id and endpoints). Metadata such as error state is not topology, so a
        /// metadata-only edit never invalidates a layout request.
        /// </summary>
        public static bool Matches(MeasuredGraph measured, CanonicalGraph live)
        {
            if (measured.Nodes.Count != live.Nodes.Count || measured.Edges.Count != live.Edges.Count)
            {
                return false;
            }

            var measuredNodes = measured.Nodes.ToDictionary(node => node.Id);
            var measuredEdges = measured.Edges.ToDictionary(edge => edge.Id);

            return live.Nodes.All(node =>
                    measuredNodes.TryGetValue(node.Id, out var measuredNode) &&
                    measuredNode.Kind == node.Kind &&
                    measuredNode.ParentId == node.ParentId) &&
                live.Edges.All(edge =>
                    measuredEdges.TryGetValue(edge.Id, out var measuredEdge) &&
                    measuredEdge.SourceId == edge.SourceId &&
                    measuredEdge.TargetId == edge.TargetId);
        }
    }
}
