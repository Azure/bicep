// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public class VariableSymbol(ISymbolContext context, string name, VariableDeclarationSyntax declaringSyntax) : DeclaredSymbol(context, name, declaringSyntax, declaringSyntax.Name)
    {
        public VariableDeclarationSyntax DeclaringVariable => (VariableDeclarationSyntax)this.DeclaringSyntax;

        public override void Accept(SymbolVisitor visitor) => visitor.VisitVariableSymbol(this);

        public override SymbolKind Kind => SymbolKind.Variable;

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }
    }
}

