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
        if (TryGetExportMetadata() is DuplicatedExportMetadata duplicatedExportMetadata)
        {
            yield return DiagnosticBuilder.ForPosition(DeclaringImportedSymbolsListItem.OriginalSymbolName).AmbiguousExportFromArmTemplate(duplicatedExportMetadata.Name, duplicatedExportMetadata.ExportKindsWithSameName);
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
}
