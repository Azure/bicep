// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class LocalSymbol : DeclaredSymbol
    {
        public LocalSymbol(ISymbolContext context, string name, LocalVariableSyntax declaringSyntax)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
        }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitLocalSymbol(this);

        public override SymbolKind Kind => SymbolKind.Local;
    }
}
