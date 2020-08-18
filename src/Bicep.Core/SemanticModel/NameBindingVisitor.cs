using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public sealed class NameBindingVisitor : SyntaxVisitor
    {
        private FunctionPlacementConstraints currentConstraints;

        private readonly ILookup<string, Symbol> declarations;

        private readonly IDictionary<SyntaxBase, Symbol> bindings;

        private readonly ImmutableArray<NamespaceSymbol> namespaces;

        public NameBindingVisitor(IList<Symbol> declarations, IDictionary<SyntaxBase, Symbol> bindings, IEnumerable<NamespaceSymbol> namespaces)
        {
            this.declarations = declarations.ToLookup(declaration => declaration.Name, LanguageConstants.IdentifierComparer);
            this.bindings = bindings;
            this.namespaces = namespaces.ToImmutableArray();
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            base.VisitVariableAccessSyntax(syntax);

            var symbol = this.LookupSymbolByName(syntax.Name.IdentifierName, syntax.Name.Span);

            // bind what we got - the type checker will validate if it fits
            this.bindings.Add(syntax, symbol);
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            currentConstraints = FunctionPlacementConstraints.Resources;
            // TODO: is this precise enough or should we only set when visiting the specific sub-node?
            base.VisitResourceDeclarationSyntax(syntax);
            currentConstraints = FunctionPlacementConstraints.None;
        }

        public override void VisitParameterDefaultValueSyntax(ParameterDefaultValueSyntax syntax)
        {
            currentConstraints = FunctionPlacementConstraints.ParameterDefaults;
            // TODO: is this precise enough or should we only set when visiting the specific sub-node?
            base.VisitParameterDefaultValueSyntax(syntax);
            currentConstraints = FunctionPlacementConstraints.None;
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            currentConstraints = FunctionPlacementConstraints.Variables;
            // TODO: is this precise enough or should we only set when visiting the specific sub-node?
            base.VisitVariableDeclarationSyntax(syntax);
            currentConstraints = FunctionPlacementConstraints.None;
        }

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            currentConstraints = FunctionPlacementConstraints.Outputs;
            // TODO: is this precise enough or should we only set when visiting the specific sub-node?
            base.VisitOutputDeclarationSyntax(syntax);
            currentConstraints = FunctionPlacementConstraints.None;
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            base.VisitFunctionCallSyntax(syntax);

            var symbol = this.LookupSymbolByName(syntax.FunctionName.IdentifierName, syntax.FunctionName.Span);

            // bind what we got - the type checker will validate if it fits
            this.bindings.Add(syntax, symbol);
        }

        private Symbol ValidateFunctionConstraints(Symbol symbol, TextSpan span)
        {
            if (!(symbol is FunctionSymbol functionSymbol))
            {
                return symbol;
            }

            var validConstraints = functionSymbol.Overloads.Select(overload => overload.PlacementConstraints).Aggregate((x, y) => x | y);
            if ((validConstraints & currentConstraints) == FunctionPlacementConstraints.None)
            {
                return new ErrorSymbol(DiagnosticBuilder.ForPosition(span).FunctionDisallowedForLocation(
                    functionSymbol.Name,
                    DiagnosticBuilder.GetFunctionPlacementConstraintsList(validConstraints)));
            }

            return symbol;
        }

        private Symbol LookupSymbolByName(string name, TextSpan span)
        {
            // in cases of duplicate declarations we will see multiple declaration symbols in the result list
            // for simplicitly we will bind to the first one
            // it may cause follow-on type errors, but there will also be errors about duplicate identifiers as well
            Symbol? localSymbol = this.declarations[name].FirstOrDefault();

            if (localSymbol != null)
            {
                // we found the symbol in the local namespace
                return ValidateFunctionConstraints(localSymbol, span);
            }

            // symbol does not exist in the local namespace
            // try it in the imported namespaces

            // match in one of the namespaces
            var foundSymbols = this.namespaces
                .Select(ns => ns.TryGetFunctionSymbol(name))
                .Where(symbol => symbol != null)
                .ToList();

            if (foundSymbols.Count() > 1)
            {
                // ambiguous symbol
                return new ErrorSymbol(DiagnosticBuilder.ForPosition(span).AmbiguousSymbolReference(name, this.namespaces.Select(ns => ns.Name)));
            }

            var foundSymbol = foundSymbols.FirstOrDefault();
            if (foundSymbol == null)
            {
                return new ErrorSymbol(DiagnosticBuilder.ForPosition(span).SymbolicNameDoesNotExist(name));
            }

            return ValidateFunctionConstraints(foundSymbol, span);
        }
    }
}
