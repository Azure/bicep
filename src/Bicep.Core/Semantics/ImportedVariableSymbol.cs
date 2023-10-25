// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics;

public class ImportedVariableSymbol : ImportedSymbol<ExportedVariableMetadata>
{
    public ImportedVariableSymbol(ISymbolContext context, ImportedSymbolsListItemSyntax declaringSyntax, CompileTimeImportDeclarationSyntax enclosingDeclartion, ISemanticModel sourceModel, ExportedVariableMetadata exportMetadata)
        : base(context, declaringSyntax, enclosingDeclartion, sourceModel, exportMetadata) { }

    public override SymbolKind Kind => SymbolKind.Variable;

    public override void Accept(SymbolVisitor visitor) => visitor.VisitImportedVariableSymbol(this);
}
