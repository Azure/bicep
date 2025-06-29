// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics;

public class WildcardImportSymbol : DeclaredSymbol, INamespaceSymbol
{
    public WildcardImportSymbol(ISymbolContext context, ISemanticModel sourceModel, WildcardImportSyntax declaringSyntax, CompileTimeImportDeclarationSyntax enclosingDeclaration)
        : base(context, declaringSyntax.Name.IdentifierName, declaringSyntax, declaringSyntax.AliasAsClause.Alias)
    {
        SourceModel = sourceModel;
        EnclosingDeclaration = enclosingDeclaration;
    }

    public ISemanticModel SourceModel { get; }

    public CompileTimeImportDeclarationSyntax EnclosingDeclaration { get; }

    public override void Accept(SymbolVisitor visitor)
    {
        visitor.VisitWildcardImportSymbol(this);
    }

    public override SymbolKind Kind => SymbolKind.ImportedNamespace;

    public NamespaceType? TryGetNamespaceType() => this.Type as NamespaceType;

    public ResultWithDiagnosticBuilder<ArtifactReference> TryGetArtifactReference()
        => Context.ArtifactReferenceFactory.TryGetArtifactReference(this.Context.SourceFile, EnclosingDeclaration);

    public override IEnumerable<Diagnostic> GetDiagnostics()
    {
        if (SourceModel.Exports.Values.OfType<DuplicatedExportMetadata>().Any())
        {
            yield return DiagnosticBuilder.ForPosition(DeclaringSyntax).ImportedModelContainsAmbiguousExports(
                SourceModel.Exports.Values.OfType<DuplicatedExportMetadata>().Select(md => md.Name));
        }
    }
}
