// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class LocalVariableSymbol : DeclaredSymbol
    {
        public LocalVariableSymbol(ISymbolContext context, string name, LocalVariableSyntax declaringSyntax)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
        }

        public LocalVariableSyntax DeclaringLocalVariable => (LocalVariableSyntax) this.DeclaringSyntax;

        public override void Accept(SymbolVisitor visitor) => visitor.VisitLocalVariableSymbol(this);

        public override SymbolKind Kind => SymbolKind.Local;
    }
}
