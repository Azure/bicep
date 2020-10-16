// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.TypeSystem;
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

            var vertexes = graph.SelectMany(x => new [] { x.from, x.to }).Distinct().ToDictionary(i => i, i => new Vertex(i));
            var vertexLookup = graph.ToLookup(x => vertexes[x.from], x => vertexes[x.to]);

            var cycles = CycleDetector<Vertex>.FindCycles(vertexLookup);

            cycles.Keys.Should().BeEquivalentTo(new [] { vertexes[1], vertexes[2], vertexes[3], vertexes[6], vertexes[7], vertexes[8], vertexes[9], vertexes[10] });

            cycles[vertexes[1]].Should().ContainInOrder(new [] { vertexes[1] });

            cycles[vertexes[2]].Should().ContainInOrder(new [] { vertexes[2], vertexes[3] });
            cycles[vertexes[3]].Should().ContainInOrder(new [] { vertexes[3], vertexes[2] });

            // note that the cycle for each key should be returned in-order, starting with the key
            cycles[vertexes[6]].Should().ContainInOrder(new [] { vertexes[6], vertexes[7], vertexes[8], vertexes[9], vertexes[10] });
            cycles[vertexes[7]].Should().ContainInOrder(new [] { vertexes[7], vertexes[8], vertexes[9], vertexes[10], vertexes[6] });
            cycles[vertexes[8]].Should().ContainInOrder(new [] { vertexes[8], vertexes[9], vertexes[10], vertexes[6], vertexes[7] });
            cycles[vertexes[9]].Should().ContainInOrder(new [] { vertexes[9], vertexes[10], vertexes[6], vertexes[7], vertexes[8] });
            cycles[vertexes[10]].Should().ContainInOrder(new [] { vertexes[10], vertexes[6], vertexes[7], vertexes[8], vertexes[9] });
        }
    }
}