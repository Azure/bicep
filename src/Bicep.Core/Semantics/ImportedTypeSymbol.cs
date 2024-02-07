// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics;

public class ImportedTypeSymbol(ISymbolContext context, ImportedSymbolsListItemSyntax declaringSyntax, CompileTimeImportDeclarationSyntax enclosingDeclartion, ISemanticModel sourceModel, ExportedTypeMetadata exportMetadata) : ImportedSymbol<ExportedTypeMetadata>(context, declaringSyntax, enclosingDeclartion, sourceModel, exportMetadata)
{
    public override SymbolKind Kind => SymbolKind.TypeAlias;

    public override void Accept(SymbolVisitor visitor) => visitor.VisitImportedTypeSymbol(this);
}
