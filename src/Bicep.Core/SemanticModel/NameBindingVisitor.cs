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
        private readonly IReadOnlyDictionary<string, Symbol> declarations;

        private readonly IDictionary<SyntaxBase, Symbol> bindings;

        private readonly ImmutableArray<NamespaceSymbol> namespaces;

        public NameBindingVisitor(IReadOnlyDictionary<string, Symbol> declarations, IDictionary<SyntaxBase, Symbol> bindings, IEnumerable<NamespaceSymbol> namespaces)
        {
            this.declarations = declarations;
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

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            base.VisitFunctionCallSyntax(syntax);

            var symbol = this.LookupSymbolByName(syntax.FunctionName.IdentifierName, syntax.FunctionName.Span);

            // bind what we got - the type checker will validate if it fits
            this.bindings.Add(syntax, symbol);
        }

        private Symbol LookupSymbolByName(string name, TextSpan span)
        {
            if (this.declarations.TryGetValue(name, out var localSymbol))
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
