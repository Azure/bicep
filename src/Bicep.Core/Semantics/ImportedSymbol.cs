// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics;

public abstract class ImportedSymbol : DeclaredSymbol
{
    public ImportedSymbol(ISymbolContext context, ImportedSymbolsListItemSyntax declaringSyntax, CompileTimeImportDeclarationSyntax enclosingDeclartion)
        : base(context, declaringSyntax.Name.IdentifierName, declaringSyntax, declaringSyntax.Name)
    {
        EnclosingDeclaration = enclosingDeclartion;
    }

    public CompileTimeImportDeclarationSyntax EnclosingDeclaration { get; }

    public ImportedSymbolsListItemSyntax DeclaringImportedSymbolsListItem => (ImportedSymbolsListItemSyntax)DeclaringSyntax;

    public string? OriginalSymbolName => DeclaringImportedSymbolsListItem.TryGetOriginalSymbolNameText();

    public abstract ISemanticModel? TryGetSourceModel();

    public string? TryGetDescription() => TryGetExportMetadata()?.Description;

    public ResultWithDiagnostic<ArtifactReference> TryGetModuleReference()
        => Context.Compilation.ModuleReferenceFactory.TryGetModuleReference(EnclosingDeclaration, Context.SourceFile.FileUri);

    public override IEnumerable<ErrorDiagnostic> GetDiagnostics()
    {
        if (TryGetExportMetadata() is ExportMetadata exportMetadata && !IsSupportedImportKind(exportMetadata.Kind))
        {
            yield return DiagnosticBuilder.ForPosition(DeclaringImportedSymbolsListItem.OriginalSymbolName)
                .ImportedSymbolKindNotSupportedInSourceFileKind(exportMetadata.Name, exportMetadata.Kind, Context.SourceFile.FileKind);
        }
    }

    protected abstract ExportMetadata? TryGetExportMetadata();

    private bool IsSupportedImportKind(ExportMetadataKind exportKind) => Context.SourceFile switch
    {
        BicepFile => exportKind switch
        {
            ExportMetadataKind.Type or
            ExportMetadataKind.Variable => true,
            ExportMetadataKind.Function => true,
            _ => false,
        },
        BicepParamFile => exportKind switch
        {
            ExportMetadataKind.Variable => true,
            _ => false,
        },
        _ => false,
    };
}
