// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Emit.CompileTimeImports;

internal class ImportedSymbolDeclarationMigrator : ImportReferenceExpressionRewriter
{
    private readonly SemanticModel sourceModel;
    private readonly ImmutableDictionary<DeclaredSymbol, string> declaredSymbolNames;
    private readonly SyntaxBase? sourceSyntax;

    public ImportedSymbolDeclarationMigrator(SemanticModel sourceModel,
        ImmutableDictionary<DeclaredSymbol, string> declaredSymbolNames,
        ImmutableDictionary<ImportedSymbol, string> importedSymbolNames,
        ImmutableDictionary<WildcardImportPropertyReference, string> wildcardImportPropertyNames,
        SyntaxBase sourceSyntax) : base(importedSymbolNames, wildcardImportPropertyNames, sourceSyntax)
    {
        this.sourceModel = sourceModel;
        this.declaredSymbolNames = declaredSymbolNames;
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
            declaredSymbolNames[LookupTypeAliasByName(expression.Name)],
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

    public override Expression ReplaceDeclaredVariableExpression(DeclaredVariableExpression expression)
        => new DeclaredVariableExpression(sourceSyntax,
            declaredSymbolNames[LookupVariableByName(expression.Name)],
            RewriteForMigration(expression.Value),
            expression.Description,
            // An imported variable is never automatically re-exported
            Exported: null);

    public override Expression ReplaceTypeAliasReferenceExpression(TypeAliasReferenceExpression expression)
        => new SynthesizedTypeAliasReferenceExpression(sourceSyntax, declaredSymbolNames[expression.Symbol], expression.ExpressedType);

    public override Expression ReplaceVariableReferenceExpression(VariableReferenceExpression expression)
        => new SynthesizedVariableReferenceExpression(sourceSyntax, declaredSymbolNames[expression.Variable]);

    private TypeAliasSymbol LookupTypeAliasByName(string name) => sourceModel.Root.TypeDeclarations
        .Where(NameEquals<TypeAliasSymbol>(name))
        .Single();

    private VariableSymbol LookupVariableByName(string name) => sourceModel.Root.VariableDeclarations
        .Where(NameEquals<VariableSymbol>(name))
        .Single();

    private static Func<S, bool> NameEquals<S>(string name) where S : DeclaredSymbol
        => s => LanguageConstants.IdentifierComparer.Equals(s.Name, name);
}
