// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;

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

    public Result<ISemanticModel, ErrorDiagnostic> TryGetSemanticModel()
        => SemanticModelHelper.TryGetSemanticModelForForeignTemplateReference(Context.Compilation.SourceFileGrouping,
            EnclosingDeclaration,
            b => b.CompileTimeImportDeclarationMustReferenceTemplate(),
            Context.Compilation);

    public ResultWithDiagnostic<ArtifactReference> TryGetModuleReference()
        => Context.Compilation.ModuleReferenceFactory.TryGetModuleReference(EnclosingDeclaration, Context.SourceFile.FileUri);
}
