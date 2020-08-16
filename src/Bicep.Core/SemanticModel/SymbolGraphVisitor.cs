using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Syntax;

namespace Bicep.Core.SemanticModel
{
    public class SymbolGraphVisitor : SyntaxVisitor
    {
        private readonly IReadOnlyDictionary<string, Symbol> declarations;
        private readonly IReadOnlyDictionary<SyntaxBase, Symbol> bindings;
        private readonly IDictionary<Symbol, IList<Symbol>> symbolGraph;
        private Symbol? currentDeclaration;

        public static SymbolGraph Build(ProgramSyntax programSyntax, IReadOnlyDictionary<string, Symbol> declarations, IReadOnlyDictionary<SyntaxBase, Symbol> bindings)
        {
            var visitor = new SymbolGraphVisitor(declarations, bindings);
            visitor.VisitProgramSyntax(programSyntax);

            var resourceGraph = visitor.GetResourceDependencyGraph();

            return new SymbolGraph(resourceGraph);
        }

        private ImmutableDictionary<ResourceSymbol, ImmutableArray<ResourceSymbol>> GetResourceDependencyGraph()
        {
            var output = new Dictionary<ResourceSymbol, ImmutableArray<ResourceSymbol>>();
            foreach (var declaration in declarations.Values)
            {
                if (!(declaration is ResourceSymbol resourceSymbol))
                {
                    continue;
                }
                
                output[resourceSymbol] = GetResourceDependencies(resourceSymbol);
            }

            return output.ToImmutableDictionary(x => x.Key, x => x.Value);
        }

        private ImmutableArray<ResourceSymbol> GetResourceDependencies(ResourceSymbol startNode)
        {
            var output = new List<ResourceSymbol>();
            var visited = new HashSet<Symbol>();
            var nodeQueue = new Queue<Symbol>();

            visited.Add(startNode);            
            nodeQueue.Enqueue(startNode);

            // non-recursive DFS
            while (nodeQueue.Any())
            {
                var node = nodeQueue.Dequeue();
                if (node is ResourceSymbol dependency && dependency != startNode)
                {
                    // stop searching here, we only need direct dependencies
                    output.Add(dependency);
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

            return output.ToImmutableArray();
        }

        private SymbolGraphVisitor(IReadOnlyDictionary<string, Symbol> declarations, IReadOnlyDictionary<SyntaxBase, Symbol> bindings)
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
