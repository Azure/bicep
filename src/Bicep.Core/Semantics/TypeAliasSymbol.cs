// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics;

public class TypeAliasSymbol(ISymbolContext context, string name, TypeDeclarationSyntax declaringSyntax, SyntaxBase value) : DeclaredSymbol(context, name, declaringSyntax, declaringSyntax.Name)
{
    public TypeDeclarationSyntax DeclaringType => (TypeDeclarationSyntax)this.DeclaringSyntax;

    public SyntaxBase Value { get; } = value;

    public override void Accept(SymbolVisitor visitor) => visitor.VisitTypeAliasSymbol(this);

    public override SymbolKind Kind => SymbolKind.TypeAlias;
}
