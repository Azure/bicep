// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics;

public class WildcardImportSymbol : DeclaredSymbol, INamespaceSymbol
{
    public WildcardImportSymbol(ISymbolContext context, WildcardImportSyntax declaringSyntax, CompileTimeImportDeclarationSyntax enclosingDeclaration)
        : base(context, declaringSyntax.Name.IdentifierName, declaringSyntax, declaringSyntax.AliasAsClause.Alias)
    {
        EnclosingDeclaration = enclosingDeclaration;
    }

    public CompileTimeImportDeclarationSyntax EnclosingDeclaration { get; }

    public override void Accept(SymbolVisitor visitor)
    {
        visitor.VisitWildcardImportSymbol(this);
    }

    public override SymbolKind Kind => SymbolKind.ImportedNamespace;

    public NamespaceType? TryGetNamespaceType() => this.Type as NamespaceType;

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
