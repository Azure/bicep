using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.Core.SemanticModel
{
    public sealed class NameBindingVisitor : SyntaxVisitor
    {
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
            
            switch (symbol.Kind)
            {
                case SymbolKind.Error:
                case SymbolKind.Variable:
                case SymbolKind.Parameter:
                    this.bindings.Add(syntax, symbol);
                    break;

                default:
                    this.bindings.Add(syntax, new ErrorSymbol(DiagnosticBuilder.ForPosition(syntax.Span).SymbolicNameIsNotAParameterOrVariable(syntax.Name.IdentifierName)));
                    break;
            }
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            base.VisitFunctionCallSyntax(syntax);

            var symbol = this.LookupSymbolByName(syntax.FunctionName.IdentifierName, syntax.FunctionName.Span);

            switch (symbol.Kind)
            {
                case SymbolKind.Error:
                case SymbolKind.Function:
                    this.bindings.Add(syntax, symbol);
                    break;

                default:
                    this.bindings.Add(syntax, new ErrorSymbol(DiagnosticBuilder.ForPosition(syntax.FunctionName.Span).SymbolicNameIsNotAFunction(syntax.FunctionName.IdentifierName)));
                    break;
            }
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
                return localSymbol;
            }

            // symbol does not exist in the local namespace
            // try it in the imported namespaces
            if (this.namespaces.Count(ns => ns.Symbols.ContainsKey(name)) > 1)
            {
                // ambiguous symbol
                return new ErrorSymbol(DiagnosticBuilder.ForPosition(span).AmbiguousSymbolReference(name, this.namespaces.Select(ns => ns.Name)));
            }

            // match in one of the namespaces
            return this.namespaces
                .Where(ns => ns.Symbols.ContainsKey(name))
                .Select(ns => ns.Symbols[name])
                .FirstOrDefault() ?? new ErrorSymbol(DiagnosticBuilder.ForPosition(span).SymbolicNameDoesNotExist(name));
        }
    }
}
