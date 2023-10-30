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

internal class ImportReferenceExpressionRewriter : ExpressionRewriteVisitor
{
    private readonly ImmutableDictionary<ImportedSymbol, string> importedSymbolNames;
    private readonly ImmutableDictionary<WildcardImportPropertyReference, string> wildcardImportPropertyNames;
    private readonly SyntaxBase? sourceSyntax;

    public ImportReferenceExpressionRewriter(ImmutableDictionary<ImportedSymbol, string> importedSymbolNames,
        ImmutableDictionary<WildcardImportPropertyReference, string> wildcardImportPropertyNames,
        SyntaxBase? sourceSyntax)
    {
        this.importedSymbolNames = importedSymbolNames;
        this.wildcardImportPropertyNames = wildcardImportPropertyNames;
        this.sourceSyntax = sourceSyntax;
    }

    public Expression ReplaceImportReferences(Expression expression) => Replace(expression);

    public override Expression ReplaceImportedTypeReferenceExpression(ImportedTypeReferenceExpression expression)
        => new SynthesizedTypeAliasReferenceExpression(sourceSyntax ?? expression.SourceSyntax, importedSymbolNames[expression.Symbol], expression.ExpressedType);

    public override Expression ReplaceWildcardImportPropertyReferenceExpression(WildcardImportTypePropertyReferenceExpression expression)
        => new SynthesizedTypeAliasReferenceExpression(sourceSyntax ?? expression.SourceSyntax, wildcardImportPropertyNames[new(expression.ImportSymbol, expression.PropertyName)], expression.ExpressedType);

    public override Expression ReplaceImportedVariableReferenceExpression(ImportedVariableReferenceExpression expression)
        => new SynthesizedVariableReferenceExpression(sourceSyntax ?? expression.SourceSyntax, importedSymbolNames[expression.Variable]);

    public override Expression ReplaceWildcardImportVariablePropertyReferenceExpression(WildcardImportVariablePropertyReferenceExpression expression)
        => new SynthesizedVariableReferenceExpression(sourceSyntax ?? expression.SourceSyntax, wildcardImportPropertyNames[new(expression.ImportSymbol, expression.PropertyName)]);

    public override Expression ReplaceImportedUserDefinedFunctionCallExpression(ImportedUserDefinedFunctionCallExpression expression)
    {
        var (namespaceName, functionName) = GetFunctionName(importedSymbolNames[expression.Symbol]);
        return new SynthesizedUserDefinedFunctionCallExpression(sourceSyntax ?? expression.SourceSyntax, namespaceName, functionName, expression.Parameters);
    }

    public override Expression ReplaceWildcardImportInstanceFunctionCallExpression(WildcardImportInstanceFunctionCallExpression expression)
    {
        var (namespaceName, functionName) = GetFunctionName(wildcardImportPropertyNames[new(expression.ImportSymbol, expression.MethodName)]);
        return new SynthesizedUserDefinedFunctionCallExpression(sourceSyntax ?? expression.SourceSyntax, namespaceName, functionName, expression.Parameters);
    }

    protected static (string namespaceName, string functionName) GetFunctionName(string potentiallyQualifiedName) => potentiallyQualifiedName.IndexOf('.') switch
    {
        int separatorLocation when separatorLocation > -1 => (potentiallyQualifiedName[..separatorLocation], potentiallyQualifiedName[(separatorLocation + 1)..]),
        _ => (EmitConstants.UserDefinedFunctionsNamespace, potentiallyQualifiedName),
    };
}
