// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics;

public abstract class ImportedSymbol(ISymbolContext context, ImportedSymbolsListItemSyntax declaringSyntax, CompileTimeImportDeclarationSyntax enclosingDeclartion, ISemanticModel sourceModel) : DeclaredSymbol(context, declaringSyntax.Name.IdentifierName, declaringSyntax, declaringSyntax.Name)
{
    public CompileTimeImportDeclarationSyntax EnclosingDeclaration { get; } = enclosingDeclartion;

    public ImportedSymbolsListItemSyntax DeclaringImportedSymbolsListItem => (ImportedSymbolsListItemSyntax)DeclaringSyntax;

    public string? OriginalSymbolName => DeclaringImportedSymbolsListItem.TryGetOriginalSymbolNameText();

    public ISemanticModel SourceModel { get; } = sourceModel;

    public abstract string? Description { get; }

    public ResultWithDiagnostic<ArtifactReference> TryGetArtifactReference()
        => Context.Compilation.ArtifactReferenceFactory.TryGetArtifactReference(EnclosingDeclaration, Context.SourceFile.FileUri);
}

public abstract class ImportedSymbol<T>(ISymbolContext context, ImportedSymbolsListItemSyntax declaringSyntax, CompileTimeImportDeclarationSyntax enclosingDeclartion, ISemanticModel sourceModel, T exportMetadata) : ImportedSymbol(context, declaringSyntax, enclosingDeclartion, sourceModel) where T : ExportMetadata
{
    public T ExportMetadata { get; } = exportMetadata;

    public override string? Description => ExportMetadata.Description;

    public override IEnumerable<ErrorDiagnostic> GetDiagnostics()
    {
        if (!IsSupportedImportKind())
        {
            yield return DiagnosticBuilder.ForPosition(DeclaringImportedSymbolsListItem.OriginalSymbolName)
                .ImportedSymbolKindNotSupportedInSourceFileKind(ExportMetadata.Name, ExportMetadata.Kind, Context.SourceFile.FileKind);
        }
    }

    private bool IsSupportedImportKind() => Context.SourceFile switch
    {
        BicepFile => ExportMetadata.Kind switch
        {
            ExportMetadataKind.Type or
            ExportMetadataKind.Variable or
            ExportMetadataKind.Function => true,
            _ => false,
        },
        BicepParamFile => ExportMetadata.Kind switch
        {
            ExportMetadataKind.Variable => true,
            _ => false,
        },
        _ => false,
    };
}
