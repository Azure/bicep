// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Syntax;
using System.Diagnostics.CodeAnalysis;

namespace Bicep.Core.Semantics;

public class ImportedTypeSymbol : DeclaredSymbol
{
    public ImportedTypeSymbol(ISymbolContext context, ImportedSymbolsListItemSyntax declaringSyntax, CompileTimeImportDeclarationSyntax enclosingDeclartion)
        : base(context, declaringSyntax.Name.IdentifierName, declaringSyntax, declaringSyntax.Name)
    {
        EnclosingDeclaration = enclosingDeclartion;
    }

    public CompileTimeImportDeclarationSyntax EnclosingDeclaration { get; }

    public ImportedSymbolsListItemSyntax DeclaringImportedSymbolsListItem => (ImportedSymbolsListItemSyntax)DeclaringSyntax;

    public string OriginalSymbolName => DeclaringImportedSymbolsListItem.OriginalSymbolName.IdentifierName;

    public override void Accept(SymbolVisitor visitor)
    {
        visitor.VisitImportedTypeSymbol(this);
    }

    public override SymbolKind Kind => SymbolKind.TypeAlias;

    public bool TryGetSemanticModel([NotNullWhen(true)] out ISemanticModel? semanticModel, [NotNullWhen(false)] out ErrorDiagnostic? failureDiagnostic)
        => SemanticModelHelper.TryGetSemanticModelForForeignTemplateReference(Context.Compilation.SourceFileGrouping,
            EnclosingDeclaration,
            b => b.CompileTimeImportDeclarationMustReferenceTemplate(),
            Context.Compilation,
            out semanticModel,
            out failureDiagnostic);

    public bool TryGetModuleReference([NotNullWhen(true)] out ArtifactReference? moduleReference, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        => Context.Compilation.ModuleReferenceFactory.TryGetModuleReference(EnclosingDeclaration, Context.SourceFile.FileUri, out moduleReference, out failureBuilder);
}
