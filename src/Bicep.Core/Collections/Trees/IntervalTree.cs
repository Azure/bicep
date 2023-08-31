// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Collections.Trees
{
    /// <summary>
    /// An augumented red-black tree that holds intervals [start, end].
    /// For reference, see chapter 14.3 of <see href="https://en.wikipedia.org/wiki/Introduction_to_Algorithms">Introduction to Algorithms (3rd Edition)</see>.
    /// </summary>
    /// <typeparam name="TData">The tree node payload data type.</typeparam>
    public class IntervalTree<TData>
    {
        private readonly HashSet<IntervalTreeNode<TData>> redNodes = new();

        public IntervalTreeNode<TData> Root { get; private set; } = IntervalTreeNode<TData>.Nil;

        /// <summary>
        /// Performs an in-order traversal for the tree and returns the nodes traversed.
        /// </summary>
        public IEnumerable<IntervalTreeNode<TData>> Traverse() => this.Traverse(this.Root);

        /// <summary>
        /// Performs an in-order traversal for the sub-tree rooted at node and returns the nodes traversed.
        /// </summary>
        public IEnumerable<IntervalTreeNode<TData>> Traverse(IntervalTreeNode<TData> node)
        {
            // Do an in-order traversal so the nodes are sorted by interval starts.
            var stack = new Stack<IntervalTreeNode<TData>>();

            while (stack.Any() || node.IsNotNil)
            {
                if (node.IsNotNil)
                {
                    stack.Push(node);
                    node = node.Left;
                }
                else
                {
                    node = stack.Pop();
                    yield return node;
                    node = node.Right;
                }
            }
        }

        /// <summary>
        /// Insert a node to the tree in O(lg n).
        /// </summary>
        /// <param name="node">The node to insert.</param>
        public void Insert(IntervalTreeNode<TData> node)
        {
            var parent = IntervalTreeNode<TData>.Nil;
            var current = Root;

            while (current.IsNotNil)
            {
                parent = current;

                if (node.Start < current.Start)
                {
                    current = current.Left;
                    parent.MaxEnd = Math.Max(parent.MaxEnd, node.MaxEnd);
                }
                else
                {
                    current = current.Right;
                    parent.MaxEnd = Math.Max(parent.MaxEnd, node.MaxEnd);
                }
            }

            if (parent.Start == node.Start && parent.End == node.End)
            {
                parent.Data.AddRange(node.Data);
                return;
            }

            node.Parent = parent;

            if (parent.IsNil)
            {
                Root = node;
            }
            else if (node.Start < parent.Start)
            {
                parent.Left = node;
            }
            else
            {
                parent.Right = node;
            }

            node.Left = IntervalTreeNode<TData>.Nil;
            node.Right = IntervalTreeNode<TData>.Nil;

            PaintRed(node);
            FixAdjancentRed(node);
        }

        /// <summary>
        /// Delete a node from the tree in O(lg n).
        /// </summary>
        /// <param name="node">The node to delete.</param>
        public void Delete(IntervalTreeNode<TData> node)
        {
            if (this.Root.IsNil)
            {
                return;
            }

            if (node.Left.IsNil || node.Right.IsNil)
            {
                var subsitution = node.Left.IsNil ? node.Right : node.Left;

                this.Transplant(node, subsitution);
                subsitution.Parent.RefreshMaxEndUpwards();

                if (IsBlack(node))
                {
                    this.FixExtraBlack(subsitution);
                }
            }
            else
            {
                var successor = node.Right.GetSuccessor();
                var successorSubsitution = successor.Right;

                if (successor.Parent == node)
                {
                    successorSubsitution.Parent = successor; // Normalize the special case where successorSubsitution is Nil.
                }
                else
                {
                    this.Transplant(successor, successorSubsitution);
                    successor.Right = node.Right;
                    successor.Right.Parent = successor;
                }

                Transplant(node, successor);
                successor.Left = node.Left;
                successor.Left.Parent = successor;

                successorSubsitution.Parent.RefreshMaxEndUpwards();

                var successorWasBlack = this.IsBlack(successor);

                this.CopyColor(node, successor);

                if (successorWasBlack)
                {
                    this.FixExtraBlack(successorSubsitution);
                }
            }

            this.PaintBlack(node);
        }

        /// <summary>
        /// Perform a <see href="https://en.wikipedia.org/wiki/Tree_rotation">tree rotation</see>.
        /// If indexer is <see cref="BinaryTreeIndexer.Default">BinaryTreeIndexer.Default</see>, the rotation is a left-rotation.
        /// If indexer is <see cref="BinaryTreeIndexer.Inverted">BinaryTreeIndexer.Inverted</see>, the rotation is a right-rotation.
        /// </summary>
        /// <param name="node">The root node of the tree to rotate.</param>
        /// <param name="indexer">The indexer that controls the rotation direction.</param>
        private void Rotate(IntervalTreeNode<TData> node, BinaryTreeIndexer indexer)
        {
            var (leftIndex, rightIndex) = indexer;
            var right = node[rightIndex];

            // Turn child's left subtree into node's right subtree.
            node[rightIndex] = right[leftIndex];

            if (right[leftIndex].IsNotNil)
            {
                right[leftIndex].Parent = node;
            }

            // Link node's parent to child.
            right.Parent = node.Parent;

            if (node.IsRoot)
            {
                Root = right;
            }
            else if (node.IsLeft)
            {
                node.Parent.Left = right;
            }
            else
            {
                node.Parent.Right = right;
            }

            // Put node on child's left.
            right[leftIndex] = node;
            node.Parent = right;

            // Update max end.
            right.MaxEnd = node.MaxEnd;
            node.RefreshMaxEnd();

        }

        /// <summary>
        /// Within the current tree, replace a sub-tree whose root is node by a sub-tree whose root is subsitution.
        /// </summary>
        /// <param name="node">The root of the sub-tree to be replaced.</param>
        /// <param name="subsitution">The root of the sub-tree to transplant.</param>
        private void Transplant(IntervalTreeNode<TData> node, IntervalTreeNode<TData> subsitution)
        {
            if (node.IsRoot)
            {
                this.Root = subsitution;
            }
            else if (node.IsLeft)
            {
                node.Parent.Left = subsitution;
            }
            else
            {
                node.Parent.Right = subsitution;
            }

            subsitution.Parent = node.Parent;
        }

        /// <summary>
        /// The method is called after an insertion to fix the violation to the red-black tree property that
        /// if a node is red, then both its children are black (no adjancent red nodes).
        /// </summary>
        /// <param name="node">The node to fix.</param>
        private void FixAdjancentRed(IntervalTreeNode<TData> node)
        {
            while (IsRed(node.Parent))
            {
                var indexer = node.Parent.IsLeft ? BinaryTreeIndexer.Default : BinaryTreeIndexer.Inverted;
                var uncle = node.Parent.Parent[indexer.RightIndex];

                // Case 1.
                if (IsRed(uncle))
                {
                    PaintBlack(node.Parent);
                    PaintBlack(uncle);
                    PaintRed(node.Parent.Parent);
                    node = node.Parent.Parent;
                }
                else
                {
                    // Case 2.
                    if (node == node.Parent[indexer.RightIndex])
                    {
                        node = node.Parent;
                        Rotate(node, indexer);
                    }

                    // Case 3.
                    PaintBlack(node.Parent);
                    PaintRed(node.Parent.Parent);
                    Rotate(node.Parent.Parent, indexer.Invert());
                }
            }

            this.PaintBlack(Root);
        }

        /// <summary>
        /// The method is called after a deletion to fix a doubly-black or red-or-black node.
        /// </summary>
        /// <param name="node">The node to fix.</param>
        private void FixExtraBlack(IntervalTreeNode<TData> node)
        {
            while (node.IsNotRoot && this.IsBlack(node))
            {
                var indexer = node.IsLeft ? BinaryTreeIndexer.Default : BinaryTreeIndexer.Inverted;
                var (leftIndex, rightIndex) = indexer;

                var sibling = node.Parent[rightIndex];

                // Case 1.
                if (this.IsRed(sibling))
                {
                    this.PaintBlack(sibling);
                    this.PaintRed(node.Parent);
                    this.Rotate(node.Parent, indexer);
                    sibling = node.Parent[rightIndex];
                }

                // Case 2.
                if (this.IsBlack(sibling.Left) && this.IsBlack(sibling.Right))
                {
                    this.PaintRed(sibling);
                    node = node.Parent;
                }
                else
                {
                    // Case 3.
                    if (this.IsBlack(sibling[rightIndex]))
                    {
                        this.PaintBlack(sibling[leftIndex]);
                        this.PaintRed(sibling);
                        this.Rotate(sibling, indexer.Invert());
                        sibling = node.Parent[rightIndex];
                    }

                    // Case 4.
                    this.CopyColor(node.Parent, sibling);
                    this.PaintBlack(node.Parent);
                    this.PaintBlack(sibling[rightIndex]);
                    this.Rotate(node.Parent, indexer);
                    node = this.Root;
                }
            }

            this.PaintBlack(node);
        }

        private bool IsRed(IntervalTreeNode<TData> node) => redNodes.Contains(node);

        private bool IsBlack(IntervalTreeNode<TData> node) => !redNodes.Contains(node);

        private void PaintRed(IntervalTreeNode<TData> node) => redNodes.Add(node);

        private void PaintBlack(IntervalTreeNode<TData> node) => redNodes.Remove(node);

        private void CopyColor(IntervalTreeNode<TData> source, IntervalTreeNode<TData> target)
        {
            if (this.IsRed(source))
            {
                this.PaintRed(target);
            }
            else
            {
                this.PaintBlack(target);
            }
        }
    }
}
