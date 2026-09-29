// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Features.Custom.Visualization.Models;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    public static class VisualGraphTopology
    {
        /// <summary>
        /// Returns whether a measured graph has the same topology as the live graph: the same nodes (by id, kind,
        /// and parent) and the same edges (by id). Metadata such as error state is not topology, so a
        /// metadata-only edit never invalidates a layout request.
        /// </summary>
        public static bool Matches(MeasuredGraph measured, CanonicalGraph live)
        {
            if (measured.Nodes.Count != live.Nodes.Count || measured.Edges.Count != live.Edges.Count)
            {
                return false;
            }

            var measuredNodes = measured.Nodes.ToDictionary(node => node.Id);
            var measuredEdgeIds = measured.Edges.Select(edge => edge.Id).ToHashSet();

            return live.Nodes.All(node =>
                    measuredNodes.TryGetValue(node.Id, out var measuredNode) &&
                    measuredNode.Kind == node.Kind &&
                    measuredNode.ParentId == node.ParentId) &&
                live.Edges.All(edge => measuredEdgeIds.Contains(edge.Id));
        }
    }
}
