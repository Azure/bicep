// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.Collections.Trees
{
    public readonly struct BinaryTreeIndexer
    {
        public static readonly BinaryTreeIndexer Default = new(inverted: false);

        public static readonly BinaryTreeIndexer Inverted = new(inverted: true);

        private readonly bool inverted;

        private BinaryTreeIndexer(bool inverted)
        {
            this.inverted = inverted;
        }

        public BinaryTreeIndex LeftIndex => inverted ? BinaryTreeIndex.Right : BinaryTreeIndex.Left;

        public BinaryTreeIndex RightIndex => inverted ? BinaryTreeIndex.Left : BinaryTreeIndex.Right;

        public void Deconstruct(out BinaryTreeIndex leftIndex, out BinaryTreeIndex rightIndex)
        {
            leftIndex = LeftIndex;
            rightIndex = RightIndex;
        }

        public BinaryTreeIndexer Invert() => inverted ? Default : Inverted;
    }
}
