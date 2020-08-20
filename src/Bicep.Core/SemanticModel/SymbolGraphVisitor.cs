using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Syntax;

namespace Bicep.Core.SemanticModel
{
    public class SymbolGraphVisitor : SyntaxVisitor
    {
        private readonly IReadOnlyDictionary<string, DeclaredSymbol> declarations;
        private readonly IReadOnlyDictionary<SyntaxBase, Symbol> bindings;
        private readonly IDictionary<Symbol, IList<Symbol>> symbolGraph;
        private Symbol? currentDeclaration;

        public static SymbolDependencyGraph Build(ProgramSyntax programSyntax, IReadOnlyDictionary<string, DeclaredSymbol> declarations, IReadOnlyDictionary<SyntaxBase, Symbol> bindings)
        {
            var visitor = new SymbolGraphVisitor(declarations, bindings);
            visitor.VisitProgramSyntax(programSyntax);

            var dependencyGraph = visitor.GetSymbolDependencyGraph();

            return new SymbolDependencyGraph(dependencyGraph);
        }

        private ImmutableDictionary<Symbol, SymbolDependencies> GetSymbolDependencyGraph()
        {
            var output = new Dictionary<Symbol, SymbolDependencies>();
            foreach (var declaration in declarations.Values)
            {
                output[declaration] = GetSymbolDependencies(declaration);
            }

            return output.ToImmutableDictionary(x => x.Key, x => x.Value);
        }

        private SymbolDependencies GetSymbolDependencies(Symbol startNode)
        {
            // TODO: this could be smarter by caching results from already-visited nodes.
            var resources = new HashSet<ResourceSymbol>();

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

                if (node is ResourceSymbol dependency && node != startNode)
                {
                    // stop searching here, we only need direct resource dependencies
                    resources.Add(dependency);
                }
                else if (symbolGraph.TryGetValue(node, out var symbols))
                {
                    // not all nodes will be in the symbolGraph - for example ErrorSymbols. Hence the TryGetValue here.
                    foreach (var symbol in symbols)
                    {
                        nodeQueue.Enqueue(symbol);
                    }
                }

                visited.Add(node);
            }

            return new SymbolDependencies(resources);
        }

        private SymbolGraphVisitor(IReadOnlyDictionary<string, DeclaredSymbol> declarations, IReadOnlyDictionary<SyntaxBase, Symbol> bindings)
        {
            this.declarations = declarations;
            this.bindings = bindings;
            this.symbolGraph = new Dictionary<Symbol, IList<Symbol>>();
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            currentDeclaration = declarations[syntax.Name.IdentifierName];
            symbolGraph[currentDeclaration] = new List<Symbol>();

            base.VisitResourceDeclarationSyntax(syntax);
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            currentDeclaration = declarations[syntax.Name.IdentifierName];
            symbolGraph[currentDeclaration] = new List<Symbol>();

            base.VisitVariableDeclarationSyntax(syntax);
        }

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            currentDeclaration = declarations[syntax.Name.IdentifierName];
            symbolGraph[currentDeclaration] = new List<Symbol>();

            base.VisitParameterDeclarationSyntax(syntax);
        }

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            currentDeclaration = declarations[syntax.Name.IdentifierName];
            symbolGraph[currentDeclaration] = new List<Symbol>();

            base.VisitOutputDeclarationSyntax(syntax);
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            if (currentDeclaration == null || !symbolGraph.ContainsKey(currentDeclaration))
            {
                // this should never be possible - you can't reference a symbol outside of a declaration in Bicep
                throw new InvalidOperationException($"Variable access outside declaration");
            }

            var accessedSymbol = bindings[syntax];
            symbolGraph[currentDeclaration].Add(accessedSymbol);
        }
    }
}
