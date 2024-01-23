// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public class ParameterAssignmentSymbol : DeclaredSymbol
    {
        public ParameterAssignmentSymbol(ISymbolContext context, string name, ParameterAssignmentSyntax declaringSyntax)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
            // TODO: Shouldn't we store the value syntax in the symbol?
        }
        /// <summary>
        /// Gets the syntax node that declared this symbol.
        /// </summary>
        public ParameterAssignmentSyntax DeclaringParameterAssignment => (ParameterAssignmentSyntax)this.DeclaringSyntax;

        public override SymbolKind Kind => SymbolKind.ParameterAssignment;

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitParamAssignmentSymbol(this);
        }
    }
}
