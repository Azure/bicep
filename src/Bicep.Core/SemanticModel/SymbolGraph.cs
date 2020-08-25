// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.SemanticModel
{
    public class SymbolGraph
    {
        public SymbolGraph(IReadOnlyDictionary<Symbol, HashSet<Symbol>> symbolGraph)
        {
            // parent = the symbol doing the accessing, child = the symbol being accessed
            this.symbolDependenciesGraph = symbolGraph;
            // parent = the symbol being accessed, child = the symbol doing the accessing
            this.symbolDependentsGraph = Invert(symbolGraph);

            // cache of resource symbols that a particular symbol depends on
            this.resourceDependencies = new SymbolResultCache<ImmutableHashSet<ResourceSymbol>>(
                symbol => SearchGraph(symbolDependenciesGraph, symbol, node => node as ResourceSymbol));

            // cache of function symbols that a particular symbol depends on
            this.functionDependencies = new SymbolResultCache<ImmutableHashSet<FunctionSymbol>>(
                symbol => SearchGraph(symbolDependenciesGraph, symbol, node => node as FunctionSymbol));
        }

        private readonly IReadOnlyDictionary<Symbol, HashSet<Symbol>> symbolDependenciesGraph;

        private readonly IReadOnlyDictionary<Symbol, HashSet<Symbol>> symbolDependentsGraph;

        private readonly SymbolResultCache<ImmutableHashSet<ResourceSymbol>> resourceDependencies;

        private readonly SymbolResultCache<ImmutableHashSet<FunctionSymbol>> functionDependencies;

        public ImmutableHashSet<ResourceSymbol> GetResourceDependencies(Symbol symbol)
            => resourceDependencies.Lookup(symbol);

        public ImmutableHashSet<FunctionSymbol> GetFunctionDependencies(Symbol symbol)
            => functionDependencies.Lookup(symbol);

        private static IReadOnlyDictionary<Symbol, HashSet<Symbol>> Invert(IReadOnlyDictionary<Symbol, HashSet<Symbol>> graph)
        {
            var inverted = new Dictionary<Symbol, HashSet<Symbol>>();

            foreach (var symbol in graph.Keys)
            {
                if (!graph.TryGetValue(symbol, out var childSymbols))
                {
                    continue;
                }

                foreach (var childSymbol in childSymbols)
                {
                    if (!inverted.TryGetValue(childSymbol, out var childSet))
                    {
                        childSet = new HashSet<Symbol>();
                        inverted[childSymbol] = childSet;
                    }

                    childSet.Add(symbol);
                }
            }

            return inverted;
        }

        private static ImmutableHashSet<TSymbol> SearchGraph<TSymbol>(IReadOnlyDictionary<Symbol, HashSet<Symbol>> graph, Symbol startNode, Func<Symbol, TSymbol?> getDependencyFunc)
            where TSymbol : Symbol
        {
            // TODO: this could be smarter by caching results from already-visited nodes.
            var dependencies = new HashSet<TSymbol>();

            var visited = new HashSet<Symbol>();
            var nodeQueue = new Queue<Symbol>();
            nodeQueue.Enqueue(startNode);

            // non-recursive DFS
            while (nodeQueue.Any())
            {
                var node = nodeQueue.Dequeue();
                if (visited.Contains(node))
                {
                    // no infinite loop pls
                    continue;
                }

                var dependency = getDependencyFunc(node);
                if (dependency != null && node != startNode)
                {
                    // stop searching here
                    dependencies.Add(dependency);
                }
                else if (graph.TryGetValue(node, out var symbols))
                {
                    // not all nodes will be in the symbolGraph - for example ErrorSymbols. Hence the TryGetValue here.
                    foreach (var symbol in symbols)
                    {
                        nodeQueue.Enqueue(symbol);
                    }
                }

                visited.Add(node);
            }

            return dependencies.ToImmutableHashSet();
        }
    }
}
