// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics;

public class WildcardImportSymbol : DeclaredSymbol, INamespaceSymbol
{
    private readonly Lazy<ISemanticModel?> sourceModelLazy;

    public WildcardImportSymbol(ISymbolContext context, WildcardImportSyntax declaringSyntax, CompileTimeImportDeclarationSyntax enclosingDeclaration)
        : base(context, declaringSyntax.Name.IdentifierName, declaringSyntax, declaringSyntax.AliasAsClause.Alias)
    {
        EnclosingDeclaration = enclosingDeclaration;

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

    public override void Accept(SymbolVisitor visitor)
    {
        visitor.VisitWildcardImportSymbol(this);
    }

    public override SymbolKind Kind => SymbolKind.ImportedNamespace;

    public NamespaceType? TryGetNamespaceType() => this.Type as NamespaceType;

    public ISemanticModel? TryGetSemanticModel() => sourceModelLazy.Value;

    public bool TryGetModuleReference([NotNullWhen(true)] out ArtifactReference? moduleReference, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        => Context.Compilation.ModuleReferenceFactory.TryGetModuleReference(EnclosingDeclaration, Context.SourceFile.FileUri, out moduleReference, out failureBuilder);
}
