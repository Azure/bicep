// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics;

public class DeclaredTypeSymbol : DeclaredSymbol
{
    public DeclaredTypeSymbol(ISymbolContext context, string name, TypeDeclarationSyntax declaringSyntax, SyntaxBase value)
        : base(context, name, declaringSyntax, declaringSyntax.Name)
    {
        this.Value = value;
    }

    public TypeDeclarationSyntax DeclaringType => (TypeDeclarationSyntax)this.DeclaringSyntax;

    public SyntaxBase Value { get; }

    public override void Accept(SymbolVisitor visitor) => visitor.VisitDeclaredTypeSymbol(this);

    public override SymbolKind Kind => SymbolKind.Type;

    public override IEnumerable<Symbol> Descendants
    {
        get
        {
            yield return this.Type;
        }
    }
}
