// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Collections.Trees;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.UnitTests.Collections.Trees
{
    [TestClass]
    public class IntervalTreeTests
    {
        public class TestIntervalTree : IntervalTree<string>
        {
            public string Visualize()
            {
                var treeBuilder = new StringBuilder();

                Visualize(treeBuilder, Root, "", true);

                return treeBuilder.ToString();
            }

            private void Visualize(StringBuilder treeBuilder, IntervalTreeNode<string> node, string indent, bool isLast)
            {
                if (node.IsNil)
                {
                    return;
                }
                
                treeBuilder.AppendLine($"{indent}+- [{node.Start}, {node.End} | {node.MaxEnd}]: {string.Join(", ", node.Data)}");
                indent += isLast ? "   " : "|  ";

                if (node.Right.IsNotNil)
                {
                    Visualize(treeBuilder, node.Right, indent, node.Left.IsNil);
                }

                if (node.Left.IsNotNil)
                {
                    Visualize(treeBuilder, node.Left, indent, isLast: true);
                }

            }
        }

        [TestMethod]
        public void Insert_NodeSequence_ProducesExpectedTrees()
        {
            var nodeSequence = new IntervalTreeNode<string>[]
            {
                new(123, 456, "aaaa"),
                new(89, 192, "bbbb"),
                new(500, 1000, "cccc"),
                new(236, 250, "dddd"),
                new(122, 122, "eeee"),
                new(0, 40, "ffff"),
                new(409, 460, "gggg"),
                new(41, 42, "hhhh"),
                new(602, 701, "iiii"),
                new(800, 888, "jjjj"),
            };

            var expectedTrees = new[]
            {
                """
                +- [123, 456 | 456]: aaaa

                """,
                """
                +- [123, 456 | 456]: aaaa
                   +- [89, 192 | 192]: bbbb

                """,
                """
                +- [123, 456 | 1000]: aaaa
                   +- [500, 1000 | 1000]: cccc
                   +- [89, 192 | 192]: bbbb

                """,
                """
                +- [123, 456 | 1000]: aaaa
                   +- [500, 1000 | 1000]: cccc
                   |  +- [236, 250 | 250]: dddd
                   +- [89, 192 | 192]: bbbb

                """,
                """
                +- [123, 456 | 1000]: aaaa
                   +- [500, 1000 | 1000]: cccc
                   |  +- [236, 250 | 250]: dddd
                   +- [89, 192 | 192]: bbbb
                      +- [122, 122 | 122]: eeee

                """,
                """
                +- [123, 456 | 1000]: aaaa
                   +- [500, 1000 | 1000]: cccc
                   |  +- [236, 250 | 250]: dddd
                   +- [89, 192 | 192]: bbbb
                      +- [122, 122 | 122]: eeee
                      +- [0, 40 | 40]: ffff

                """,
                """
                +- [123, 456 | 1000]: aaaa
                   +- [409, 460 | 1000]: gggg
                   |  +- [500, 1000 | 1000]: cccc
                   |  +- [236, 250 | 250]: dddd
                   +- [89, 192 | 192]: bbbb
                      +- [122, 122 | 122]: eeee
                      +- [0, 40 | 40]: ffff

                """,
                """
                +- [123, 456 | 1000]: aaaa
                   +- [409, 460 | 1000]: gggg
                   |  +- [500, 1000 | 1000]: cccc
                   |  +- [236, 250 | 250]: dddd
                   +- [89, 192 | 192]: bbbb
                      +- [122, 122 | 122]: eeee
                      +- [0, 40 | 42]: ffff
                         +- [41, 42 | 42]: hhhh

                """,
                """
                +- [123, 456 | 1000]: aaaa
                   +- [409, 460 | 1000]: gggg
                   |  +- [500, 1000 | 1000]: cccc
                   |  |  +- [602, 701 | 701]: iiii
                   |  +- [236, 250 | 250]: dddd
                   +- [89, 192 | 192]: bbbb
                      +- [122, 122 | 122]: eeee
                      +- [0, 40 | 42]: ffff
                         +- [41, 42 | 42]: hhhh

                """,
                """
                +- [123, 456 | 1000]: aaaa
                   +- [409, 460 | 1000]: gggg
                   |  +- [602, 701 | 1000]: iiii
                   |  |  +- [800, 888 | 888]: jjjj
                   |  |  +- [500, 1000 | 1000]: cccc
                   |  +- [236, 250 | 250]: dddd
                   +- [89, 192 | 192]: bbbb
                      +- [122, 122 | 122]: eeee
                      +- [0, 40 | 42]: ffff
                         +- [41, 42 | 42]: hhhh

                """,
            };

            var sut = new TestIntervalTree();

            for (int i = 0; i < nodeSequence.Length; i++)
            {
                sut.Insert(nodeSequence[i]);

                sut.Visualize().Should().Be(expectedTrees[i].ReplaceLineEndings());

            }
        }

        [TestMethod]
        public void Delete_NodeSequence_ProducesExpectedTrees()
        {
            var nodeSequence = new IntervalTreeNode<string>[]
            {
                new(123, 456, "aaaa"),
                new(89, 192, "bbbb"),
                new(500, 1000, "cccc"),
                new(236, 250, "dddd"),
                new(122, 122, "eeee"),
                new(0, 40, "ffff"),
                new(409, 460, "gggg"),
                new(41, 42, "hhhh"),
                new(602, 701, "iiii"),
                new(800, 888, "jjjj"),
            };

            var expectedTrees = new[]
            {
                """
                +- [236, 250 | 1000]: dddd
                   +- [602, 701 | 1000]: iiii
                   |  +- [800, 888 | 888]: jjjj
                   |  +- [409, 460 | 1000]: gggg
                   |     +- [500, 1000 | 1000]: cccc
                   +- [89, 192 | 192]: bbbb
                      +- [122, 122 | 122]: eeee
                      +- [0, 40 | 42]: ffff
                         +- [41, 42 | 42]: hhhh

                """,
                """
                +- [236, 250 | 1000]: dddd
                   +- [602, 701 | 1000]: iiii
                   |  +- [800, 888 | 888]: jjjj
                   |  +- [409, 460 | 1000]: gggg
                   |     +- [500, 1000 | 1000]: cccc
                   +- [41, 42 | 122]: hhhh
                      +- [122, 122 | 122]: eeee
                      +- [0, 40 | 40]: ffff

                """,
                """
                +- [236, 250 | 888]: dddd
                   +- [602, 701 | 888]: iiii
                   |  +- [800, 888 | 888]: jjjj
                   |  +- [409, 460 | 460]: gggg
                   +- [41, 42 | 122]: hhhh
                      +- [122, 122 | 122]: eeee
                      +- [0, 40 | 40]: ffff

                """,
                """
                +- [409, 460 | 888]: gggg
                   +- [602, 701 | 888]: iiii
                   |  +- [800, 888 | 888]: jjjj
                   +- [41, 42 | 122]: hhhh
                      +- [122, 122 | 122]: eeee
                      +- [0, 40 | 40]: ffff

                """,
                """
                +- [409, 460 | 888]: gggg
                   +- [602, 701 | 888]: iiii
                   |  +- [800, 888 | 888]: jjjj
                   +- [41, 42 | 42]: hhhh
                      +- [0, 40 | 40]: ffff

                """,
                """
                +- [409, 460 | 888]: gggg
                   +- [602, 701 | 888]: iiii
                   |  +- [800, 888 | 888]: jjjj
                   +- [41, 42 | 42]: hhhh

                """,
                """
                +- [602, 701 | 888]: iiii
                   +- [800, 888 | 888]: jjjj
                   +- [41, 42 | 42]: hhhh

                """,
                """
                +- [602, 701 | 888]: iiii
                   +- [800, 888 | 888]: jjjj

                """,
                """
                +- [800, 888 | 888]: jjjj

                """,
                """

                """,
            };


            var sut = new TestIntervalTree();

            foreach (var node in nodeSequence)
            {
                sut.Insert(node);
            }

            for (int i = 0; i < nodeSequence.Length; i++)
            {
                sut.Delete(nodeSequence[i]);

                sut.Visualize().Should().Be(expectedTrees[i].ReplaceLineEndings());
            }
        }

        //[DataTestMethod]
        //[DataRow(-10, -1, false)]
        //[DataRow(11, 11, false)]
        //[DataRow(21, 22, false)]
        //[DataRow(22, 29, false)]
        //[DataRow(61, 99, false)]
        //[DataRow(70, 80, false)]
        //[DataRow(101, 102, false)]
        //[DataRow(0, 0, true)]
        //[DataRow(0, 10, true)]
        //[DataRow(0, 100, true)]
        //[DataRow(10, 10, true)]
        //[DataRow(2, 2, true)]
        //[DataRow(3, 6, true)]
        //[DataRow(21, 31, true)]
        //[DataRow(31, 31, true)]
        //[DataRow(50, 70, true)]
        //public void ContainsOverlappingNodes_AnyInverval_ReturnsTrueIfFoundOverlappingNodes(int start, int end, bool expectedResult)
        //{
        //    var sut = new TestIntervalTree();

        //    sut.Insert(0, 10, "aaaa");
        //    sut.Insert(12, 20, "bbbb");
        //    sut.Insert(30, 60, "cccc");
        //    sut.Insert(100, 100, "dddd");

        //    var result = sut.Contains(start, end);

        //    result.Should().Be(expectedResult);
        //}

        //[DataTestMethod]
        //[DataRow(0, 9, new string[0])]
        //[DataRow(1, 8, new string[0])]
        //[DataRow(200, 300, new string[0])]
        //[DataRow(9, 10, new[] { "bbbb" })]
        //[DataRow(60, 60, new[] { "ffff" })]
        //[DataRow(300, 400, new[] { "eeee" })]
        //[DataRow(5, 80, new[] { "aaaa", "bbbb", "ffff" })]
        //[DataRow(0, 777, new[] { "aaaa", "bbbb", "cccc", "dddd", "eeee", "ffff" })]
        //[DataRow(700, 800, new[] { "cccc", "dddd" })]
        //public void FindOverlappingNodes_AnyInverval_ReturnsOverlappingNodes(int start, int end, string[] expectedOverlappingNodeData)
        //{
        //    var sut = new TestIntervalTree();

        //    sut.Insert(20, 22, "aaaa");
        //    sut.Insert(10, 14, "bbbb");
        //    sut.Insert(666, 777, "cccc");
        //    sut.Insert(666, 777, "dddd");
        //    sut.Insert(400, 500, "eeee");
        //    sut.Insert(50, 60, "ffff");

        //    var overlappingNodeData = sut.Find(start, end).SelectMany(x => x.Data);

        //    overlappingNodeData.Should().BeEquivalentTo(expectedOverlappingNodeData);
        //}

        [TestMethod]
        public void TraverseInOrder_NonEmptyTree_ReturnsInOrderSequence()
        {
            var sut = new TestIntervalTree();

            sut.Insert(new(20, 22, "aaaa"));
            sut.Insert(new(10, 14, "bbbb"));
            sut.Insert(new(666, 777, "cccc"));
            sut.Insert(new(400, 500, "eeee"));
            sut.Insert(new(50, 60, "ffff"));

            var traversedNodesData = sut.Traverse().SelectMany(x => x.Data);

            traversedNodesData.Should().Equal("bbbb", "aaaa", "ffff", "eeee", "cccc");
        }
    }
}
