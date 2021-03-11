// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public class LocalVariableSymbol : DeclaredSymbol
    {
        public LocalVariableSymbol(ISymbolContext context, string name, LocalVariableSyntax declaringSyntax, LocalKind localKind)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
            this.LocalKind = localKind;
        }

        public LocalVariableSyntax DeclaringLocalVariable => (LocalVariableSyntax) this.DeclaringSyntax;

        public override void Accept(SymbolVisitor visitor) => visitor.VisitLocalVariableSymbol(this);

        public override SymbolKind Kind => SymbolKind.Local;

        public LocalKind LocalKind { get; }
    }
}
