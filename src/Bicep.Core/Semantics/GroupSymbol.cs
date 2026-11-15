// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics;

public class GroupSymbol : DeclaredSymbol
{
    public GroupSymbol(ISymbolContext context, string name, GroupDeclarationSyntax declaringSyntax)
        : base(context, name, declaringSyntax, declaringSyntax.Name)
    {
    }

    public GroupDeclarationSyntax DeclaringGroup => (GroupDeclarationSyntax)DeclaringSyntax;

    public override SymbolKind Kind => SymbolKind.Group;

    public override void Accept(SymbolVisitor visitor) => visitor.VisitGroupSymbol(this);

    public override IEnumerable<Symbol> Descendants => [Type];
}
