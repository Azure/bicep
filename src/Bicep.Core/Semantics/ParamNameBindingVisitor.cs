// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public sealed class ParamNameBindingVisitor : SyntaxVisitor
    {
        private readonly IDictionary<SyntaxBase, Symbol> bindings;

        private readonly IReadOnlyDictionary<string, ParameterAssignmentSymbol> symbols;

        private ParamNameBindingVisitor(
            IReadOnlyDictionary<string, ParameterAssignmentSymbol> symbols,
            IDictionary<SyntaxBase, Symbol> bindings)
        {
            this.symbols = symbols;
            this.bindings = bindings;
        }

        public static ImmutableDictionary<SyntaxBase, Symbol> GetBindings(
            ProgramSyntax programSyntax,
            IReadOnlyDictionary<string, ParameterAssignmentSymbol> uniqueSymbols)
        {
            // bind identifiers to declarations
            var bindings = new Dictionary<SyntaxBase, Symbol>();
            var binder = new ParamNameBindingVisitor(uniqueSymbols, bindings);
            binder.Visit(programSyntax);

            return bindings.ToImmutableDictionary();
        }
        
        public override void VisitProgramSyntax(ProgramSyntax syntax)
        {
            base.VisitProgramSyntax(syntax);

            // create bindings for all of the declarations to their corresponding symbol
            // this is needed to make find all references work correctly
            // (doing this here to avoid side-effects in the constructor)
            foreach (ParameterAssignmentSymbol symbol in this.symbols.Values)
            {
                this.bindings.Add(symbol.AssigningSyntax, symbol);
            }
        }
    }
}
