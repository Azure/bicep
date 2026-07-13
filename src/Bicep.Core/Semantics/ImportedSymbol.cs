// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics;

public abstract class ImportedSymbol : DeclaredSymbol
{
    public ImportedSymbol(ISymbolContext context, ImportedSymbolsListItemSyntax declaringSyntax, CompileTimeImportDeclarationSyntax enclosingDeclaration, ISemanticModel sourceModel)
        : base(context, declaringSyntax.Name.IdentifierName, declaringSyntax, declaringSyntax.Name)
    {
        EnclosingDeclaration = enclosingDeclaration;
        SourceModel = sourceModel;
    }

    public CompileTimeImportDeclarationSyntax EnclosingDeclaration { get; }

    public ImportedSymbolsListItemSyntax DeclaringImportedSymbolsListItem => (ImportedSymbolsListItemSyntax)DeclaringSyntax;

    public string? OriginalSymbolName => DeclaringImportedSymbolsListItem.TryGetOriginalSymbolNameText();

    public ISemanticModel SourceModel { get; }

    public abstract string? Description { get; }

    /// <summary>
    /// The sorted, distinct names of deployment-context ("non-pure") functions referenced by the imported symbol,
    /// either directly or transitively. Such functions cannot be evaluated in a Bicep parameters file.
    /// </summary>
    public abstract ImmutableArray<string> NonPureFunctionsInClosure { get; }

    public ResultWithDiagnosticBuilder<ArtifactReference> TryGetArtifactReference()
        => Context.ArtifactReferenceFactory.TryGetArtifactReference(this.Context.SourceFile, this.EnclosingDeclaration);
}

public abstract class ImportedSymbol<T> : ImportedSymbol where T : ExportMetadata
{
    public ImportedSymbol(ISymbolContext context, ImportedSymbolsListItemSyntax declaringSyntax, CompileTimeImportDeclarationSyntax enclosingDeclaration, ISemanticModel sourceModel, T exportMetadata)
        : base(context, declaringSyntax, enclosingDeclaration, sourceModel)
    {
        ExportMetadata = exportMetadata;
    }

    public T ExportMetadata { get; }

    public override string? Description => ExportMetadata.Description;

    public override IEnumerable<Diagnostic> GetDiagnostics()
    {
        if (!IsSupportedImportKind())
        {
            yield return DiagnosticBuilder.ForPosition(DeclaringImportedSymbolsListItem.OriginalSymbolName)
                .ImportedSymbolKindNotSupportedInSourceFileKind(ExportMetadata.Name, ExportMetadata.Kind, Context.SourceFile.FileKind);
        }

        if (Context.SourceFile is BicepParamFile &&
            NonPureFunctionsInClosure is { IsDefaultOrEmpty: false } nonPureFunctions)
        {
            yield return DiagnosticBuilder.ForPosition(DeclaringImportedSymbolsListItem.OriginalSymbolName)
                .ImportedSymbolReferencesNonPureFunctions(ExportMetadata.Name, nonPureFunctions);
        }
    }

    public override ImmutableArray<string> NonPureFunctionsInClosure => ExportMetadata switch
    {
        ExportedVariableMetadata variable => variable.NonPureFunctionsInClosure,
        ExportedFunctionMetadata function => function.NonPureFunctionsInClosure,
        _ => [],
    };

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
            ExportMetadataKind.Variable or
            ExportMetadataKind.Function => true,
            ExportMetadataKind.Type => true,
            _ => false,
        },
        _ => false,
    };
}
