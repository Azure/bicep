// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Collections.Trees;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FluentAssertions.FluentActions;

namespace Bicep.Core.UnitTests.Collections.Trees
{
    [TestClass]
    public class IntervalTreeNodeTests
    {
        [DataTestMethod]
        [DataRow(0, -1)]
        [DataRow(100, 20)]
        [DataRow(10, 0)]
        public void IntervalTree_EndSmallerThanStart_Throws(int start, int end)
        {
            Invoking(() => new IntervalTreeNode<int>(start, end, 0))
                .Should()
                .Throw<ArgumentException>()
                .WithMessage($"The argument {nameof(end)} ({end}) cannot be smaller than the argument {nameof(start)} ({start}).");
        }

        [DataTestMethod]
        [DataRow(0, 0)]
        [DataRow(100, 200)]
        [DataRow(20, 21)]
        public void IntervalTree_StartSmallerThanOrEqualToEnd_DoesNotThrow(int start, int end)
        {
            Invoking(() => new IntervalTreeNode<int>(start, end, 0))
                .Should()
                .NotThrow();
        }
    }
}
