// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Utils;

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

    public ISemanticModel? TryGetSemanticModel()
        => SemanticModelHelper.TryGetSemanticModelForForeignTemplateReference(Context.Compilation.SourceFileGrouping,
            EnclosingDeclaration,
            b => b.CompileTimeImportDeclarationMustReferenceTemplate(),
            Context.Compilation)
            .TryUnwrap();

    public ResultWithDiagnostic<ArtifactReference> TryGetModuleReference()
        => Context.Compilation.ModuleReferenceFactory.TryGetModuleReference(EnclosingDeclaration, Context.SourceFile.FileUri);

    public override IEnumerable<ErrorDiagnostic> GetDiagnostics()
    {
        if (TryGetSemanticModel()?.Exports.Values.OfType<DuplicatedExportMetadata>() is { } duplicatedExports && duplicatedExports.Any())
        {
            yield return DiagnosticBuilder.ForPosition(DeclaringSyntax).ImportedModelContainsAmbiguousExports(duplicatedExports.Select(md => md.Name));
        }
    }
}
