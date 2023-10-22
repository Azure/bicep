// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics;

public class ImportedTypeSymbol : ImportedSymbol
{
    public ImportedTypeSymbol(ISymbolContext context, ImportedSymbolsListItemSyntax declaringSyntax, CompileTimeImportDeclarationSyntax enclosingDeclartion, ISemanticModel sourceModel, ExportedTypeMetadata exportMetadata)
        : base(context, declaringSyntax, enclosingDeclartion)
    {
        SourceModel = sourceModel;
        ExportMetadata = exportMetadata;
    }

    public ISemanticModel SourceModel { get; }

    public ExportedTypeMetadata ExportMetadata { get; }

    public override SymbolKind Kind => SymbolKind.TypeAlias;

    public override void Accept(SymbolVisitor visitor) => visitor.VisitImportedTypeSymbol(this);

    public override ISemanticModel? TryGetSourceModel() => SourceModel;

    protected override ExportMetadata? TryGetExportMetadata() => ExportMetadata;
}
