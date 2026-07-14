// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Features.Custom.Visualization.Models;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// Computes the ordered <see cref="GraphPatch"/> delta that transforms the graph the client currently
    /// displays (<see cref="RenderedGraph"/>) into the server's freshly built <see cref="CanonicalGraph"/>.
    /// <para>
    /// The submitted client graph carries each node's identity (id, kind, parentId), measured size, and the
    /// layout-irrelevant metadata (type, isCollection, hasChildren, hasError). Because the metadata travels
    /// with the request, the differ can emit an <see cref="GraphPatch.UpdateNode"/> only for nodes whose
    /// metadata actually changed, while staying stateless per request. A whitespace-only edit therefore
    /// produces no metadata patches at all (source ranges are resolved on demand and never diffed here).
    /// </para>
    /// </summary>
    public static class VisualGraphDiffer
    {
        public static IReadOnlyList<GraphPatch> Diff(
            RenderedGraph? current,
            CanonicalGraph target)
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

            // Refresh metadata for surviving nodes, but only for those whose metadata actually changed.
            // The submitted graph carries each node's metadata, so the server can diff precisely and stay
            // stateless: a whitespace-only edit (which changes nothing but source ranges, no longer part of
            // the node) produces no UpdateNode patches at all.
            foreach (var targetNode in target.Nodes.OrderBy(node => node.Id, StringComparer.Ordinal))
            {
                if (currentNodes.TryGetValue(targetNode.Id, out var renderedNode) &&
                    Survives(renderedNode, targetNode) &&
                    TryGetMetadataChanges(renderedNode, targetNode, out var changes))
                {
                    patches.Add(new GraphPatch.UpdateNode(targetNode.Id, changes));
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

        /// <summary>
        /// Returns whether the submitted graph and the freshly built target differ in topology — the set of
        /// nodes (by id, kind, and parent) or the set of edges (by id). Metadata-only differences (such as
        /// error state) are not topology changes. The handler uses this to decide whether to run layout, so
        /// metadata-only edits never trigger a reflow.
        /// </summary>
        public static bool HasTopologyChange(RenderedGraph? current, CanonicalGraph target)
        {
            var currentNodes = (current?.Nodes ?? []).ToDictionary(node => node.Id);

            // Counts equal plus every target node matching a surviving current node implies identical id sets.
            if (currentNodes.Count != target.Nodes.Count)
            {
                return true;
            }

            foreach (var targetNode in target.Nodes)
            {
                if (!currentNodes.TryGetValue(targetNode.Id, out var renderedNode) || !Survives(renderedNode, targetNode))
                {
                    return true;
                }
            }

            var currentEdgeIds = (current?.Edges ?? []).Select(edge => edge.Id).ToHashSet();
            var targetEdgeIds = target.Edges.Select(edge => edge.Id).ToHashSet();

            return !currentEdgeIds.SetEquals(targetEdgeIds);
        }

        // A node "survives" only if its id, kind, and parent are all unchanged. A change to kind or parent is
        // structural (re-parenting / kind flip), so the node is removed and re-added rather than updated.
        private static bool Survives(RenderedGraphNode renderedNode, GraphNode targetNode) =>
            renderedNode.Kind == targetNode.Kind && renderedNode.ParentId == targetNode.ParentId;

        // Produces the changed-only metadata delta between the submitted node and the freshly built node.
        // Returns false (and a null changeset) when no metadata field differs, so unchanged nodes emit no patch.
        private static bool TryGetMetadataChanges(RenderedGraphNode renderedNode, GraphNode targetNode, out GraphNodeChanges changes)
        {
            var typeChanged = renderedNode.Type != targetNode.Type;
            var isCollectionChanged = renderedNode.IsCollection != targetNode.IsCollection;
            var hasChildrenChanged = renderedNode.HasChildren != targetNode.HasChildren;
            var hasErrorChanged = renderedNode.HasError != targetNode.HasError;

            if (!typeChanged && !isCollectionChanged && !hasChildrenChanged && !hasErrorChanged)
            {
                changes = new GraphNodeChanges();
                return false;
            }

            changes = new GraphNodeChanges(
                Type: typeChanged ? targetNode.Type : null,
                IsCollection: isCollectionChanged ? targetNode.IsCollection : null,
                HasChildren: hasChildrenChanged ? targetNode.HasChildren : null,
                HasError: hasErrorChanged ? targetNode.HasError : null);

            return true;
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
