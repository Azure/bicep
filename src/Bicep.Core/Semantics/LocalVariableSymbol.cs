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
            this.DeclaredTypeSyntax = null;
        }

        public LocalVariableSymbol(ISymbolContext context, string name, TypedLocalVariableSyntax declaringSyntax, LocalKind localKind)
            : base(context, name, declaringSyntax, declaringSyntax.Name)
        {
            this.LocalKind = localKind;
            this.DeclaredTypeSyntax = declaringSyntax.Type;
        }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitLocalVariableSymbol(this);

        public override SymbolKind Kind => SymbolKind.Local;

        public SyntaxBase? DeclaredTypeSyntax;

        public LocalKind LocalKind { get; }
    }
}
