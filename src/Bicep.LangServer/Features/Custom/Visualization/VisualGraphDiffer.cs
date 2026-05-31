// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Features.Custom.Visualization.Models;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// Computes the ordered <see cref="GraphPatch"/> delta that transforms the graph the client currently
    /// displays (<see cref="RenderedGraph"/>) into the server's freshly built <see cref="CanonicalGraph"/>.
    /// <para>
    /// The submitted client graph intentionally carries only topology (id, kind, parentId) and measured size,
    /// not node metadata, so the differ can detect structural changes precisely but cannot tell whether a
    /// surviving node's metadata (error state, source range, type) changed. It therefore emits an idempotent
    /// <see cref="GraphPatch.UpdateNode"/> refresh for every surviving node. This keeps the server stateless
    /// per request at the cost of slightly chattier responses; applying an unchanged update is a no-op on the
    /// client and never triggers a re-layout.
    /// </para>
    /// <para>
    /// No layout (<see cref="GraphPatch.SetNodeLayout"/>) patches are produced here: positioning is still the
    /// client's responsibility until the server-side layout engine is introduced.
    /// </para>
    /// </summary>
    public static class VisualGraphDiffer
    {
        public static IReadOnlyList<GraphPatch> Diff(RenderedGraph? current, CanonicalGraph target)
        {
            var currentNodes = (current?.Nodes ?? []).ToDictionary(node => node.Id);
            var currentEdgeIds = (current?.Edges ?? []).Select(edge => edge.Id).ToHashSet();
            var targetNodes = target.Nodes.ToDictionary(node => node.Id);
            var targetEdgeIds = target.Edges.Select(edge => edge.Id).ToHashSet();

            var patches = new List<GraphPatch>();

            // Remove edges the target no longer has.
            foreach (var edge in current?.Edges ?? [])
            {
                if (!targetEdgeIds.Contains(edge.Id))
                {
                    patches.Add(new GraphPatch.RemoveEdge(edge.Id));
                }
            }

            // A node "survives" only if its id, kind, and parent are all unchanged. A change to kind or parent
            // is structural (re-parenting / kind flip), so the node is removed and re-added rather than updated.
            static bool Survives(RenderedGraphNode renderedNode, GraphNode targetNode) =>
                renderedNode.Kind == targetNode.Kind && renderedNode.ParentId == targetNode.ParentId;

            // Remove nodes that are gone or structurally changed, deepest containment first so children are
            // removed before their parents.
            var removed = currentNodes.Values
                .Where(renderedNode => !targetNodes.TryGetValue(renderedNode.Id, out var targetNode) || !Survives(renderedNode, targetNode))
                .OrderByDescending(renderedNode => ContainmentDepth(renderedNode.Id))
                .ThenBy(renderedNode => renderedNode.Id, StringComparer.Ordinal);

            foreach (var renderedNode in removed)
            {
                patches.Add(new GraphPatch.RemoveNode(renderedNode.Id));
            }

            // Add nodes that are new or structurally changed, shallowest containment first so parents exist
            // before their children are added.
            var added = targetNodes.Values
                .Where(targetNode => !currentNodes.TryGetValue(targetNode.Id, out var renderedNode) || !Survives(renderedNode, targetNode))
                .OrderBy(targetNode => ContainmentDepth(targetNode.Id))
                .ThenBy(targetNode => targetNode.Id, StringComparer.Ordinal);

            foreach (var targetNode in added)
            {
                patches.Add(new GraphPatch.AddNode(targetNode));
            }

            // Refresh metadata for surviving nodes (see remarks on idempotent updates above).
            foreach (var targetNode in target.Nodes.OrderBy(node => node.Id, StringComparer.Ordinal))
            {
                if (currentNodes.TryGetValue(targetNode.Id, out var renderedNode) && Survives(renderedNode, targetNode))
                {
                    patches.Add(new GraphPatch.UpdateNode(targetNode.Id, new GraphNodeChanges(
                        Type: targetNode.Type,
                        IsCollection: targetNode.IsCollection,
                        HasChildren: targetNode.HasChildren,
                        HasError: targetNode.HasError,
                        FilePath: targetNode.FilePath,
                        Range: targetNode.Range)));
                }
            }

            // Add edges the client does not yet have.
            foreach (var edge in target.Edges)
            {
                if (!currentEdgeIds.Contains(edge.Id))
                {
                    patches.Add(new GraphPatch.AddEdge(edge));
                }
            }

            patches.Add(new GraphPatch.SetErrorCount(target.ErrorCount));

            return patches;
        }

        private static int ContainmentDepth(string nodeId)
        {
            var depth = 0;
            var index = 0;

            while ((index = nodeId.IndexOf("::", index, StringComparison.Ordinal)) >= 0)
            {
                depth++;
                index += 2;
            }

            return depth;
        }
    }
}
