// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Bicep.Core.Collections.Trees;
using Bicep.Core.Extensions;
using Bicep.Core.Text;

namespace Bicep.Core.Diagnostics
{
    public sealed class DiagnosticTree : IntervalTree<IDiagnostic>, IDiagnosticWriter, IDiagnosticLookup
    {
        public IEnumerable<IDiagnostic> this[IPositionable positionable] =>
            FindEnclosedNode(positionable).SelectMany(x => x.Data);

        public bool Contains(IPositionable positionable) =>
            this.ContainsEnclosedNode(positionable.GetPosition(), positionable.GetEndPosition());

        public void Write(IDiagnostic diagnostic) => this.Insert(new(diagnostic.GetPosition(), diagnostic.GetEndPosition(), diagnostic));

        public void Delete(IPositionable positionable)
        {
            foreach (var node in FindEnclosedNode(positionable))
            {
                this.Delete(node);
            }
        }

        public IEnumerator<IDiagnostic> GetEnumerator() => this.Traverse().SelectMany(x => x.Data).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Check if there is at least one node enclosed by [start, end] in O(lg n),
        /// where n is the number of diagnostics in the tree.
        /// </summary>
        /// <param name="start">The interval start position.</param>
        /// <param name="end">The interval end position.</param>
        private bool ContainsEnclosedNode(int start, int end)
        {
            if (this.Root.IsNil)
            {
                return false;
            }

            var current = this.Root;

            while (true)
            {
                if (start <= current.Start && current.End <= end)
                {
                    return true;
                }

                if (start < current.Start)
                {
                    // ----|start...
                    // ------|current.Start...
                    // When start < current.Start, technically there can exist intervals enclosed by [start, end]
                    // in both the left sub-tree and the right sub-tree. However, since the invervals in the tree
                    // are spans of diagnostics attached to syntax nodes, and one syntax node can overlap with
                    // another only when one of them is enclosed by the other, the following situation is impossible:
                    //
                    // ---|diagnostic1.start...diagnostic1.end|
                    // --------|diagnostic1.start...diagnostic2.end|
                    //
                    // If start < current.Start, and current is not enclosed by [start, end], end must be smaller
                    // than current.Start as well:
                    //
                    // ---|start...end|
                    // ----------------|current.start...current.end|
                    //
                    // Thus, we only need to search the left-subtree.
                    current = current.Left;
                }
                else
                {
                    // --------|start...
                    // ------|current.Start...
                    // Search current.Right since only the left sub-tree cannot have nodes enclosed by [start, end].
                    current = current.Right;
                }

                if (current.IsNil)
                {
                    return false;
                }

                // ----------------------------------|start...
                // --|current.Start...current.MaxEnd|...
                // There are has no overlapping nodes.
                if (start > current.MaxEnd)
                {
                    return false;
                }
            }
        }

        private IEnumerable<IntervalTreeNode<IDiagnostic>> FindEnclosedNode(IPositionable positionable) =>
            FindEnclosedNodesRecursively(this.Root, positionable.GetPosition(), positionable.GetEndPosition());

        /// <summary>
        /// Find nodes enclosed by [start, end] in O(min(n, k * lg n)), where n is the number of diagnostics
        /// in the tree, and k is the number of nodes enclosed by [start, end].
        /// </summary>
        /// <param name="root">The current root node to search.</param>
        /// <param name="start">The interval start position.</param>
        /// <param name="end">The interval end position.</param>
        private static IEnumerable<IntervalTreeNode<IDiagnostic>> FindEnclosedNodesRecursively(IntervalTreeNode<IDiagnostic> root, int start, int end)
        {
            if (root.IsNil)
            {
                yield break;
            }

            if (root.Left.IsNotNil && start <= root.Left.MaxEnd)
            {
                foreach (var node in FindEnclosedNodesRecursively(root.Left, start, end))
                {
                    yield return node;
                }
            }

            if (start <= root.Start && root.End <= end)
            {
                yield return root;
            }

            if (root.Right.IsNotNil && start <= root.Right.MaxEnd)
            {
                foreach (var node in FindEnclosedNodesRecursively(root.Right, start, end))
                {
                    yield return node;
                }
            }
        }
    }
}
