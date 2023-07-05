// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics;

public class ImportedTypeSymbol : DeclaredSymbol
{
    public ImportedTypeSymbol(ISymbolContext context, ImportedSymbolsListItemSyntax declaringSyntax)
        : base(context, declaringSyntax.Name.IdentifierName, declaringSyntax, declaringSyntax.Name)
    {
    }

    public ImportedSymbolsListItemSyntax DeclaringImportedSymbolsListItem => (ImportedSymbolsListItemSyntax)DeclaringSyntax;

    public override void Accept(SymbolVisitor visitor)
    {
        visitor.VisitImportedTypeSymbol(this);
    }

    public override SymbolKind Kind => SymbolKind.TypeAlias;
}
