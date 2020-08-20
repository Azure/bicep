using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Syntax;

namespace Bicep.Core.SemanticModel
{
    public class SymbolGraphVisitor : SyntaxVisitor
    {
        private readonly IReadOnlyDictionary<string, DeclaredSymbol> declarations;
        private readonly IReadOnlyDictionary<SyntaxBase, Symbol> bindings;
        private readonly Dictionary<Symbol, IList<Symbol>> symbolGraph;
        private Symbol currentDeclaration;

        public static SymbolGraph Build(FileSymbol fileSymbol, IReadOnlyDictionary<string, DeclaredSymbol> declarations, IReadOnlyDictionary<SyntaxBase, Symbol> bindings)
        {
            var visitor = new SymbolGraphVisitor(fileSymbol, declarations, bindings);
            visitor.VisitProgramSyntax(fileSymbol.Syntax);

            return new SymbolGraph(visitor.symbolGraph.ToDictionary(kvp => kvp.Key, kvp => new HashSet<Symbol>(kvp.Value)));
        }

        private SymbolGraphVisitor(FileSymbol fileSymbol, IReadOnlyDictionary<string, DeclaredSymbol> declarations, IReadOnlyDictionary<SyntaxBase, Symbol> bindings)
        {
            this.currentDeclaration = fileSymbol;
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
            var accessedSymbol = bindings[syntax];
            symbolGraph[currentDeclaration].Add(accessedSymbol);
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            var accessedSymbol = bindings[syntax];
            symbolGraph[currentDeclaration].Add(accessedSymbol);
        }
    }
}
