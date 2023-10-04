// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Registry;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics;

public class ImportedSymbol : DeclaredSymbol
{
    public ImportedSymbol(ISymbolContext context, ImportedSymbolsListItemSyntax declaringSyntax, CompileTimeImportDeclarationSyntax enclosingDeclartion)
        : base(context, declaringSyntax.Name.IdentifierName, declaringSyntax, declaringSyntax.Name)
    {
        EnclosingDeclaration = enclosingDeclartion;
    }

    public CompileTimeImportDeclarationSyntax EnclosingDeclaration { get; }

    public ImportedSymbolsListItemSyntax DeclaringImportedSymbolsListItem => (ImportedSymbolsListItemSyntax)DeclaringSyntax;

    public string? OriginalSymbolName => DeclaringImportedSymbolsListItem.OriginalSymbolName switch
    {
        IdentifierSyntax identifier => identifier.IdentifierName,
        StringSyntax @string when @string.TryGetLiteralValue() is string literalValue => literalValue,
        _ => null,
    };

    public override SymbolKind Kind => TryGetExportMetadata() switch
    {
        ExportedTypeMetadata => SymbolKind.TypeAlias,
        ExportedVariableMetadata => SymbolKind.Variable,
        _ => SymbolKind.Error,
    };

    public ISemanticModel? TryGetSemanticModel()
        => SemanticModelHelper.TryGetModelForArtifactReference(Context.Compilation.SourceFileGrouping,
            EnclosingDeclaration,
            Context.Compilation)
            .TryUnwrap();

    public string? TryGetDescription() => TryGetExportMetadata()?.Description;

    public ResultWithDiagnostic<ArtifactReference> TryGetModuleReference()
        => Context.Compilation.ModuleReferenceFactory.TryGetModuleReference(EnclosingDeclaration, Context.SourceFile.FileUri);

    public override void Accept(SymbolVisitor visitor)
    {
        visitor.VisitImportedSymbol(this);
    }

    public override IEnumerable<ErrorDiagnostic> GetDiagnostics()
    {
        if (TryGetExportMetadata() is ExportMetadata exportMetadata)
        {
            if (exportMetadata.Kind == ExportMetadataKind.Error)
            {
                yield return DiagnosticBuilder.ForPosition(DeclaringImportedSymbolsListItem.OriginalSymbolName)
                    .ImportedSymbolHasErrors(exportMetadata.Name, exportMetadata.Description ?? "unknown error");
                // if we're already reporting an error from the source model, no need to check for import-context specific error conditions
                yield break;
            }

            if (!IsSupportedImportKind(exportMetadata.Kind))
            {
                yield return DiagnosticBuilder.ForPosition(DeclaringImportedSymbolsListItem.OriginalSymbolName)
                    .ImportedSymbolKindNotSupportedInSourceFileKind(exportMetadata.Name, exportMetadata.Kind, Context.SourceFile.FileKind);
            }
        }
    }

    public override IEnumerable<Symbol> Descendants
    {
        get
        {
            yield return this.Type;
        }
    }

    private ExportMetadata? TryGetExportMetadata() => OriginalSymbolName is string nonNullName && TryGetSemanticModel()?.Exports.TryGetValue(nonNullName, out var exportMetadata) is true
        ? exportMetadata
        : null;

    private bool IsSupportedImportKind(ExportMetadataKind exportKind) => Context.SourceFile switch
    {
        BicepFile => exportKind switch
        {
            ExportMetadataKind.Type or
            ExportMetadataKind.Variable => true,
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
