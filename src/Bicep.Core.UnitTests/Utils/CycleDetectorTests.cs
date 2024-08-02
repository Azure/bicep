// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Utils
{
    [TestClass]
    public class CycleDetectorTests
    {
        private class Vertex
        {
            public Vertex(int id)
            {
                Id = id;
            }

            public int Id { get; }
        }

        [TestMethod]
        public void CycleDetector_should_detect_cycles_correctly()
        {
            var graph = new (int from, int to)[] {
                (1, 1), // self-cycle
                (2, 3), // bi-cycle
                (3, 2), // bi-cycle :)
                (4, 5),
                (4, 6),
                (5, 6),
                (6, 7), // big cycle (6 -> 7 -> 8 -> 9 -> 10 -> 6)
                (7, 8),
                (8, 9),
                (9, 10),
                (10, 6),
            };

            var vertices = graph.SelectMany(x => new[] { x.from, x.to }).Distinct().ToDictionary(i => i, i => new Vertex(i));
            var vertexLookup = graph.ToLookup(x => vertices[x.from], x => vertices[x.to]);

            var cycles = CycleDetector<Vertex>.FindCycles(vertexLookup);

            cycles.Keys.Should().BeEquivalentTo(new[] { vertices[1], vertices[2], vertices[3], vertices[6], vertices[7], vertices[8], vertices[9], vertices[10] });

            cycles[vertices[1]].Should().ContainInOrder([vertices[1]]);

            cycles[vertices[2]].Should().ContainInOrder([vertices[2], vertices[3]]);
            cycles[vertices[3]].Should().ContainInOrder([vertices[3], vertices[2]]);

            // note that the cycle for each key should be returned in-order, starting with the key
            cycles[vertices[6]].Should().ContainInOrder([vertices[6], vertices[7], vertices[8], vertices[9], vertices[10]]);
            cycles[vertices[7]].Should().ContainInOrder([vertices[7], vertices[8], vertices[9], vertices[10], vertices[6]]);
            cycles[vertices[8]].Should().ContainInOrder([vertices[8], vertices[9], vertices[10], vertices[6], vertices[7]]);
            cycles[vertices[9]].Should().ContainInOrder([vertices[9], vertices[10], vertices[6], vertices[7], vertices[8]]);
            cycles[vertices[10]].Should().ContainInOrder([vertices[10], vertices[6], vertices[7], vertices[8], vertices[9]]);
        }
    }
}
