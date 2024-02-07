// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics;

public class WildcardImportSymbol(ISymbolContext context, ISemanticModel sourceModel, WildcardImportSyntax declaringSyntax, CompileTimeImportDeclarationSyntax enclosingDeclaration) : DeclaredSymbol(context, declaringSyntax.Name.IdentifierName, declaringSyntax, declaringSyntax.AliasAsClause.Alias), INamespaceSymbol
{
    public ISemanticModel SourceModel { get; } = sourceModel;

    public CompileTimeImportDeclarationSyntax EnclosingDeclaration { get; } = enclosingDeclaration;

    public override void Accept(SymbolVisitor visitor)
    {
        visitor.VisitWildcardImportSymbol(this);
    }

    public override SymbolKind Kind => SymbolKind.ImportedNamespace;

    public NamespaceType? TryGetNamespaceType() => this.Type as NamespaceType;

    public ResultWithDiagnostic<ArtifactReference> TryGetArtifactReference()
        => Context.Compilation.ArtifactReferenceFactory.TryGetArtifactReference(EnclosingDeclaration, Context.SourceFile.FileUri);

    public override IEnumerable<ErrorDiagnostic> GetDiagnostics()
    {
        if (SourceModel.Exports.Values.OfType<DuplicatedExportMetadata>().Any())
        {
            yield return DiagnosticBuilder.ForPosition(DeclaringSyntax).ImportedModelContainsAmbiguousExports(
                SourceModel.Exports.Values.OfType<DuplicatedExportMetadata>().Select(md => md.Name));
        }
    }
}
