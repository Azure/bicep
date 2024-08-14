// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics;

public class ImportedTypeSymbol : ImportedSymbol<ExportedTypeMetadata>
{
    public ImportedTypeSymbol(ISymbolContext context, ImportedSymbolsListItemSyntax declaringSyntax, CompileTimeImportDeclarationSyntax enclosingDeclaration, ISemanticModel sourceModel, ExportedTypeMetadata exportMetadata)
        : base(context, declaringSyntax, enclosingDeclaration, sourceModel, exportMetadata) { }

    public override SymbolKind Kind => SymbolKind.TypeAlias;

    public override void Accept(SymbolVisitor visitor) => visitor.VisitImportedTypeSymbol(this);
}
