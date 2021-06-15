// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.Utils
{
    public static class CycleDetector<TNode>
        where TNode : class
    {
        private enum VisitorState
        {
            Partial,
            Complete,
        }

        public static ImmutableDictionary<TNode, ImmutableArray<TNode>> FindCycles(ILookup<TNode, TNode> graph)
        {
            var shortestCycleByNode = new Dictionary<TNode, ImmutableArray<TNode>>();
            var visitState = new Dictionary<TNode, VisitorState>();
            foreach (var grouping in graph)
            {
                var parent = grouping.Key;
                if (visitState.ContainsKey(parent))
                {
                    continue;
                }

                var visitStack = new Stack<TNode>();
                visitStack.Push(parent);
                visitState[parent] = VisitorState.Partial;

                FindCyclesDfs(graph, visitStack, visitState, shortestCycleByNode);
            }

			return shortestCycleByNode.ToImmutableDictionary(
				x => x.Key,
				x => {
                    // cycles are returned by FindCycleDfs in reverse order
                    var cycle = x.Value.Reverse();

					// return the cycle as originating from the current key (e.g. a cycle 5 -> 6 -> 7 for key 6 should be displayed as 6 -> 7 -> 5)
					var cycleSuffix = cycle.TakeWhile(y => y != x.Key);
					var cyclePrefix = cycle.Skip(cycleSuffix.Count());

					return cyclePrefix.Concat(cycleSuffix).ToImmutableArray();
				});
        }

        private static void FindCyclesDfs(ILookup<TNode, TNode> graph, Stack<TNode> visitStack, Dictionary<TNode, VisitorState> visitState, IDictionary<TNode, ImmutableArray<TNode>> shortestCycleByNode)
        {
            var currentNode = visitStack.Peek();

            foreach (var childNode in graph[currentNode])
            {
                if (!visitState.TryGetValue(childNode, out var referencedState))
                {
                    visitStack.Push(childNode);
                    visitState[childNode] = VisitorState.Partial;
                    FindCyclesDfs(graph, visitStack, visitState, shortestCycleByNode);
                    continue;
                }

                if (referencedState == VisitorState.Partial)
                {
                    AddCycleInformation(visitStack, childNode, shortestCycleByNode);
                }
            }

            visitState[currentNode] = VisitorState.Complete;
            visitStack.Pop();
        }

        private static void AddCycleInformation(Stack<TNode> visitStack, TNode currentNode, IDictionary<TNode, ImmutableArray<TNode>> shortestCycleByNode)
        {
            var cycle = visitStack
                .TakeWhile(x => x != currentNode)
                .Concat(new [] { currentNode })
                .ToImmutableArray();

            foreach (var element in cycle)
            {
                if (shortestCycleByNode.TryGetValue(element, out var otherCycle) && otherCycle.Length <= cycle.Length)
                {
                    // we've already found a shorter cycle
                    continue;
                }

                shortestCycleByNode[element] = cycle;
            }
        }
    }
}
