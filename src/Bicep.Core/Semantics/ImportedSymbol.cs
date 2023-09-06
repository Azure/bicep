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
    private readonly Lazy<ISemanticModel?> sourceModelLazy;

    public ImportedSymbol(ISymbolContext context, ImportedSymbolsListItemSyntax declaringSyntax, CompileTimeImportDeclarationSyntax enclosingDeclartion)
        : base(context, declaringSyntax.Name.IdentifierName, declaringSyntax, declaringSyntax.Name)
    {
        EnclosingDeclaration = enclosingDeclartion;

        sourceModelLazy = new(() =>
        {
            SemanticModelHelper.TryGetSemanticModelForForeignTemplateReference(Context.Compilation.SourceFileGrouping,
                EnclosingDeclaration,
                b => b.CompileTimeImportDeclarationMustReferenceTemplate(),
                Context.Compilation,
                out var semanticModel,
                out _);

            return semanticModel;
        });
    }

    public CompileTimeImportDeclarationSyntax EnclosingDeclaration { get; }

    public ImportedSymbolsListItemSyntax DeclaringImportedSymbolsListItem => (ImportedSymbolsListItemSyntax)DeclaringSyntax;

    public string? OriginalSymbolName => DeclaringImportedSymbolsListItem.OriginalSymbolName switch
    {
        IdentifierSyntax identifier => identifier.IdentifierName,
        StringSyntax @string when Context.TypeManager.GetTypeInfo(@string) is StringLiteralType stringLiteral => stringLiteral.RawStringValue,
        _ => null,
    };

    public override SymbolKind Kind => TryGetExportMetadata() switch
    {
        ExportedTypeMetadata => SymbolKind.TypeAlias,
        ExportedVariableMetadata => SymbolKind.Variable,
        _ => SymbolKind.Error,
    };

    public ISemanticModel? TryGetSemanticModel() => sourceModelLazy.Value;

    public string? TryGetDescription() => TryGetExportMetadata()?.Description;

    public bool TryGetModuleReference([NotNullWhen(true)] out ArtifactReference? moduleReference, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        => Context.Compilation.ModuleReferenceFactory.TryGetModuleReference(EnclosingDeclaration, Context.SourceFile.FileUri, out moduleReference, out failureBuilder);

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
