// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.Emit.CompileTimeImports;

internal class ImportedTypeDeclarationMigrator : ExpressionRewriteVisitor
{
    private readonly SemanticModel sourceModel;
    private readonly ImmutableDictionary<TypeAliasSymbol, string> declaredTypeNames;
    private readonly ImmutableDictionary<ImportedTypeSymbol, string> importedTypeNames;
    private readonly ImmutableDictionary<WildcardImportPropertyReference, string> wildcardImportPropertyNames;
    private readonly SyntaxBase? sourceSyntax;

    public ImportedTypeDeclarationMigrator(SemanticModel sourceModel,
        ImmutableDictionary<TypeAliasSymbol, string> declaredTypeNames,
        ImmutableDictionary<ImportedTypeSymbol, string> importedTypeNames,
        ImmutableDictionary<WildcardImportPropertyReference, string> wildcardImportPropertyNames,
        SyntaxBase? sourceSyntax)
    {
        this.sourceModel = sourceModel;
        this.declaredTypeNames = declaredTypeNames;
        this.importedTypeNames = importedTypeNames;
        this.wildcardImportPropertyNames = wildcardImportPropertyNames;
        this.sourceSyntax = sourceSyntax;
    }

    public TExpression RewriteForMigration<TExpression>(TExpression expression) where TExpression : Expression
    {
        var processed = Replace(expression);
        if (processed is not TExpression newExpressionTyped)
        {
            throw new InvalidOperationException($"Expected {nameof(processed)} to be of type {typeof(TExpression)}");
        }

        return newExpressionTyped;
    }

    protected override Expression Replace(Expression current)
        => base.Replace(current) with { SourceSyntax = sourceSyntax };

    public override Expression ReplaceDeclaredTypeExpression(DeclaredTypeExpression expression)
        => new DeclaredTypeExpression(sourceSyntax,
            declaredTypeNames[LookupTypeAliasByName(expression.Name)],
            RewriteForMigration(expression.Value),
            expression.Description,
            expression.Metadata,
            expression.Secure,
            expression.MinLength,
            expression.MaxLength,
            expression.MinValue,
            expression.MaxValue,
            expression.Sealed,
            // An imported type is never automatically re-exported
            Exported: null);

    public override Expression ReplaceTypeAliasReferenceExpression(TypeAliasReferenceExpression expression)
        => new TypeAliasReferenceExpression(sourceSyntax, declaredTypeNames[LookupTypeAliasByName(expression.Name)], expression.ExpressedType);

    public override Expression ReplaceImportedTypeReferenceExpression(ImportedTypeReferenceExpression expression)
        => new TypeAliasReferenceExpression(sourceSyntax, importedTypeNames[expression.Symbol], expression.ExpressedType);

    public override Expression ReplaceWildcardImportPropertyReferenceExpression(WildcardImportPropertyReferenceExpression expression)
        => new TypeAliasReferenceExpression(sourceSyntax, wildcardImportPropertyNames[new(expression.ImportSymbol, expression.PropertyName)], expression.ExpressedType);

    private TypeAliasSymbol LookupTypeAliasByName(string name) => sourceModel.Root.TypeDeclarations
        .Where(t => LanguageConstants.IdentifierComparer.Equals(t.Name, name))
        .Single();
}
