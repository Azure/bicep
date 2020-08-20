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
        private FunctionFlags allowedFlags;

        private readonly IReadOnlyDictionary<string, DeclaredSymbol> declarations;

        private readonly IDictionary<SyntaxBase, Symbol> bindings;

        private readonly ImmutableArray<NamespaceSymbol> namespaces;

        public NameBindingVisitor(IReadOnlyDictionary<string, DeclaredSymbol> declarations, IDictionary<SyntaxBase, Symbol> bindings, IEnumerable<NamespaceSymbol> namespaces)
        {
            this.declarations = declarations;
            this.bindings = bindings;
            this.namespaces = namespaces.ToImmutableArray();
        }

        public override void VisitProgramSyntax(ProgramSyntax syntax)
        {
            base.VisitProgramSyntax(syntax);

            // create bindings for all of the declarations to their corresponding symbol
            // this is needed to make find all references work correctly
            // (doing this here to avoid side-effects in the constructor)
            foreach (DeclaredSymbol declaredSymbol in this.declarations.Values)
            {
                this.bindings.Add(declaredSymbol.DeclaringSyntax, declaredSymbol);
            }
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
            allowedFlags = FunctionFlags.RequiresInlining;
            base.VisitResourceDeclarationSyntax(syntax);
            allowedFlags = FunctionFlags.Default;
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            allowedFlags = FunctionFlags.RequiresInlining;
            base.VisitVariableDeclarationSyntax(syntax);
            allowedFlags = FunctionFlags.Default;
        }

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            allowedFlags = FunctionFlags.RequiresInlining;
            base.VisitOutputDeclarationSyntax(syntax);
            allowedFlags = FunctionFlags.Default;
        }

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            allowedFlags = FunctionFlags.ParamDefaultsOnly;
            base.VisitParameterDeclarationSyntax(syntax);
            allowedFlags = FunctionFlags.Default;
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            base.VisitFunctionCallSyntax(syntax);

            var symbol = this.LookupSymbolByName(syntax.Name.IdentifierName, syntax.Name.Span);

            // bind what we got - the type checker will validate if it fits
            this.bindings.Add(syntax, symbol);
        }

        private Symbol ValidateFunctionFlags(Symbol symbol, TextSpan span)
        {
            if (!(symbol is FunctionSymbol functionSymbol))
            {
                return symbol;
            }

            var functionFlags = functionSymbol.Overloads.Select(overload => overload.Flags).Aggregate((x, y) => x | y);
            
            if (functionFlags.HasFlag(FunctionFlags.ParamDefaultsOnly) && !allowedFlags.HasFlag(FunctionFlags.ParamDefaultsOnly))
            {
                return new ErrorSymbol(DiagnosticBuilder.ForPosition(span).FunctionOnlyValidInParameterDefaults(functionSymbol.Name));
            }
            
            if (functionFlags.HasFlag(FunctionFlags.RequiresInlining) && !allowedFlags.HasFlag(FunctionFlags.RequiresInlining))
            {
                return new ErrorSymbol(DiagnosticBuilder.ForPosition(span).FunctionOnlyValidInResourceBody(functionSymbol.Name));
            }

            return symbol;
        }

        private Symbol LookupSymbolByName(string name, TextSpan span)
        {
            if (this.declarations.TryGetValue(name, out var localSymbol))
            {
                // we found the symbol in the local namespace
                return ValidateFunctionFlags(localSymbol, span);
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

            return ValidateFunctionFlags(foundSymbol, span);
        }
    }
}
