// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics;

public class ImportedVariableSymbol : ImportedSymbol
{
    public ImportedVariableSymbol(ISymbolContext context, ImportedSymbolsListItemSyntax declaringSyntax, CompileTimeImportDeclarationSyntax enclosingDeclartion, ISemanticModel sourceModel, ExportedVariableMetadata exportMetadata)
        : base(context, declaringSyntax, enclosingDeclartion)
    {
        SourceModel = sourceModel;
        ExportMetadata = exportMetadata;
    }

    public ISemanticModel SourceModel { get; }

    public ExportedVariableMetadata ExportMetadata { get; }

    public override SymbolKind Kind => SymbolKind.Variable;

    public override void Accept(SymbolVisitor visitor) => visitor.VisitImportedVariableSymbol(this);

    public override ISemanticModel? TryGetSourceModel() => SourceModel;

    protected override ExportMetadata? TryGetExportMetadata() => ExportMetadata;
}
