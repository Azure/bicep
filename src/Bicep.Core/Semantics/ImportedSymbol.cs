// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;

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

    public string OriginalSymbolName => DeclaringImportedSymbolsListItem.OriginalSymbolName.IdentifierName;

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

    private ExportMetadata? TryGetExportMetadata() => TryGetSemanticModel()?.Exports.TryGetValue(OriginalSymbolName, out var exportMetadata) is true
        ? exportMetadata
        : null;
}
