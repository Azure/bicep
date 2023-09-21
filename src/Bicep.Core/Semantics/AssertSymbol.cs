// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public class AssertSymbol : DeclaredSymbol
    {
        public AssertSymbol(ISymbolContext context, string name, AssertDeclarationSyntax declaringSyntax)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
        }

        public AssertDeclarationSyntax DeclaringAssert => (AssertDeclarationSyntax)this.DeclaringSyntax;

        public override void Accept(SymbolVisitor visitor) => visitor.VisitAssertSymbol(this);

        public override SymbolKind Kind => SymbolKind.Assert;

        public override IEnumerable<Symbol> Descendants
        {
            get
            {
                yield return this.Type;
            }
        }
    }
}
