// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Emit.CompileTimeImports;

internal class ImportedSymbolDeclarationMigrator : ExpressionRewriteVisitor
{
    private readonly SemanticModel sourceModel;
    private readonly ImmutableDictionary<DeclaredSymbol, string> declaredSymbolNames;
    private readonly ImmutableDictionary<string, string> synthesizedVariableNames;
    private readonly SyntaxBase? sourceSyntax;
    private string? currentCopyLoopName;

    public ImportedSymbolDeclarationMigrator(SemanticModel sourceModel,
        ImmutableDictionary<DeclaredSymbol, string> declaredSymbolNames,
        ImmutableDictionary<string, string> synthesizedVariableNames,
        SyntaxBase sourceSyntax)
    {
        this.sourceModel = sourceModel;
        this.declaredSymbolNames = declaredSymbolNames;
        this.synthesizedVariableNames = synthesizedVariableNames;
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

    public override Expression ReplaceDeclaredTypeExpression(DeclaredTypeExpression expression) => expression with
    {
        SourceSyntax = sourceSyntax,
        Name = declaredSymbolNames[LookupTypeAliasByName(expression.Name)],
        Value = RewriteForMigration(expression.Value),
        // An imported type is never automatically re-exported
        Exported = null,
    };

    public override Expression ReplaceDeclaredVariableExpression(DeclaredVariableExpression expression)
    {
        var newVariableName = declaredSymbolNames[LookupVariableByName(expression.Name)];
        var previousCopyLoopName = currentCopyLoopName;
        currentCopyLoopName = newVariableName;

        try
        {
            return expression with
            {
                SourceSyntax = sourceSyntax,
                Name = newVariableName,
                Value = RewriteForMigration(expression.Value),
                // An imported variable is never automatically re-exported
                Exported = null,
            };
        }
        finally
        {
            currentCopyLoopName = previousCopyLoopName;
        }
    }

    public override Expression ReplaceCopyIndexExpression(CopyIndexExpression expression)
        => expression with { Name = currentCopyLoopName };

    public override Expression ReplaceDeclaredFunctionExpression(DeclaredFunctionExpression expression)
    {
        var (namespaceName, name) = GetFunctionName(declaredSymbolNames[LookupDeclaredFunctionByName(expression.Name)]);

        return expression with
        {
            SourceSyntax = sourceSyntax,
            Namespace = namespaceName,
            Name = name,
            Lambda = RewriteForMigration(expression.Lambda),
            // An imported function is never automatically re-exported
            Exported = null,
        };
    }

    public override Expression ReplaceTypeAliasReferenceExpression(TypeAliasReferenceExpression expression)
        => new SynthesizedTypeAliasReferenceExpression(sourceSyntax, declaredSymbolNames[expression.Symbol], expression.ExpressedType);

    public override Expression ReplaceVariableReferenceExpression(VariableReferenceExpression expression)
        => new SynthesizedVariableReferenceExpression(sourceSyntax, declaredSymbolNames[expression.Variable]);

    public override Expression ReplaceUserDefinedFunctionCallExpression(UserDefinedFunctionCallExpression expression)
    {
        var (namespaceName, name) = GetFunctionName(declaredSymbolNames[expression.Symbol]);
        return new SynthesizedUserDefinedFunctionCallExpression(sourceSyntax, namespaceName, name,
            [.. expression.Parameters.Select(Replace)]);
    }

    public override Expression ReplaceSynthesizedVariableReferenceExpression(SynthesizedVariableReferenceExpression expression)
        => new SynthesizedVariableReferenceExpression(sourceSyntax, synthesizedVariableNames[expression.Name]);

    private TypeAliasSymbol LookupTypeAliasByName(string name) => sourceModel.Root.TypeDeclarations
        .Where(NameEquals<TypeAliasSymbol>(name))
        .Single();

    private VariableSymbol LookupVariableByName(string name) => sourceModel.Root.VariableDeclarations
        .Where(NameEquals<VariableSymbol>(name))
        .Single();

    private DeclaredFunctionSymbol LookupDeclaredFunctionByName(string name) => sourceModel.Root.FunctionDeclarations
        .Where(NameEquals<DeclaredFunctionSymbol>(name))
        .Single();

    private static Func<S, bool> NameEquals<S>(string name) where S : DeclaredSymbol
        => s => LanguageConstants.IdentifierComparer.Equals(s.Name, name);

    private static (string namespaceName, string functionName) GetFunctionName(string potentiallyQualifiedName) => potentiallyQualifiedName.IndexOf('.') switch
    {
        int separatorLocation when separatorLocation > -1 => (potentiallyQualifiedName[..separatorLocation], potentiallyQualifiedName[(separatorLocation + 1)..]),
        _ => (EmitConstants.UserDefinedFunctionsNamespace, potentiallyQualifiedName),
    };
}
