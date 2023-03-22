// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.Collections.Trees
{
    public class IntervalTreeNode<TData>
    {
        public readonly static IntervalTreeNode<TData> Nil = new(-1, -1, new List<TData>());

        private readonly IntervalTreeNode<TData>[] children = new[] { Nil, Nil };

        public IntervalTreeNode(int start, int end, TData data)
            : this(start, end, new List<TData>() { data })
        {
        }

        private IntervalTreeNode(int start, int end, IList<TData> data)
        {
            if (end < start)
            {
                throw new ArgumentException($"The argument {nameof(end)} ({end}) cannot be smaller than the argument {nameof(start)} ({start}).");
            }

            Start = start;
            End = end;
            MaxEnd = end;
            Data = data;
        }

        public IntervalTreeNode<TData> this[BinaryTreeIndex index]
        {
            get => children[(int)index];
            set => children[(int)index] = value;
        }

        public IntervalTreeNode<TData> Parent { get; set; } = Nil;

        public IntervalTreeNode<TData> Left
        {
            get => this[BinaryTreeIndex.Left];
            set => this[BinaryTreeIndex.Left] = value;
        }

        public IntervalTreeNode<TData> Right
        {
            get => this[BinaryTreeIndex.Right];
            set => this[BinaryTreeIndex.Right] = value;
        }

        public int Start { get; }

        public int End { get; }

        public IList<TData> Data { get; }

        public int MaxEnd { get; set; }

        public bool IsNil => this == Nil;

        public bool IsNotNil => this != Nil;

        public bool IsRoot => this.Parent == Nil;

        public bool IsNotRoot => this.Parent != Nil;

        public bool IsLeft => this == Parent.Left;

        public bool IsRight => this == Parent.Right;

        public IntervalTreeNode<TData> GetSuccessor()
        {
            var node = this;

            while (node.Left.IsNotNil)
            {
                node = node.Left;
            }

            return node;
        }

        public void RefreshMaxEnd() => this.MaxEnd = Math.Max(this.End, Math.Max(this.Left.MaxEnd, this.Right.MaxEnd));

        public void RefreshMaxEndUpwards()
        {
            var current = this;

            while (current.IsNotNil)
            {
                current.RefreshMaxEnd();
                current = current.Parent;
            }
        }
    }
}
