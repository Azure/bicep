// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public class ParameterSymbol : DeclaredSymbol
    {
        public ParameterSymbol(ISymbolContext context, string name, ParameterDeclarationSyntax declaringSyntax)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
        }

        public ParameterDeclarationSyntax DeclaringParameter => (ParameterDeclarationSyntax)this.DeclaringSyntax;

        public override SymbolKind Kind => SymbolKind.Parameter;

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitParameterSymbol(this);
        }

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }
    }
}
