// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics;

public class TypeAliasSymbol : DeclaredSymbol
{
    public TypeAliasSymbol(ISymbolContext context, string name, TypeDeclarationSyntax declaringSyntax, SyntaxBase value)
        : base(context, name, declaringSyntax, declaringSyntax.Name)
    {
        this.Value = value;
    }

    public TypeDeclarationSyntax DeclaringType => (TypeDeclarationSyntax)this.DeclaringSyntax;

    public SyntaxBase Value { get; }

    public TypeSymbol UnwrapType() => Type is TypeType typeRef ? typeRef.Unwrapped : Type;

    public override void Accept(SymbolVisitor visitor) => visitor.VisitTypeAliasSymbol(this);

    public override SymbolKind Kind => SymbolKind.TypeAlias;
}
