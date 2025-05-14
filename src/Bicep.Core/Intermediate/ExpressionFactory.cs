// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;

namespace Bicep.Core.Intermediate;

public static class ExpressionFactory
{
    private static readonly int MaxCopyIndexStringLength = LanguageConstants.MaxResourceCopyIndexValue.ToString().Length;

    public static ObjectPropertyExpression CreateObjectProperty(string name, Expression value, SyntaxBase? sourceSyntax = null)
        => new(
            sourceSyntax,
            new StringLiteralExpression(sourceSyntax, name),
            value);

    public static ObjectExpression CreateObject(IEnumerable<ObjectPropertyExpression> properties, SyntaxBase? sourceSyntax = null)
        => new(sourceSyntax, [.. properties]);

    public static ArrayExpression CreateArray(IEnumerable<Expression> items, SyntaxBase? sourceSyntax = null)
        => new(sourceSyntax, [.. items]);

    public static StringLiteralExpression CreateStringLiteral(string value, SyntaxBase? sourceSyntax = null)
        => new(sourceSyntax, value);

    public static BooleanLiteralExpression CreateBooleanLiteral(bool value, SyntaxBase? sourceSyntax = null)
        => new(sourceSyntax, value);

    public static IntegerLiteralExpression CreateIntegerLiteral(long value, SyntaxBase? sourceSyntax = null)
        => new(sourceSyntax, value);

    public static PropertyAccessExpression CreatePropertyAccess(Expression baseExpression, string propertyName, SyntaxBase? sourceSyntax = null, AccessExpressionFlags flags = AccessExpressionFlags.None)
        => new(sourceSyntax ?? baseExpression.SourceSyntax, baseExpression, propertyName, flags);

    public static PropertyAccessExpression CreateResourcePropertyAccess(ResourceMetadata metadata, IndexReplacementContext? indexContext, string propertyName, PropertyAccessSyntax? sourceSyntax = null, AccessExpressionFlags flags = AccessExpressionFlags.None)
        => CreatePropertyAccess(
            new ResourceReferenceExpression(sourceSyntax?.BaseExpression, metadata, indexContext),
            propertyName,
            sourceSyntax,
            flags);

    public static FunctionCallExpression CreateFunctionCall(string functionName, SyntaxBase? sourceSyntax = null, params Expression[] parameters)
        => CreateFunctionCall(functionName, parameters, sourceSyntax);

    public static FunctionCallExpression CreateFunctionCall(string functionName, IEnumerable<Expression> parameters, SyntaxBase? sourceSyntax = null)
        => new(sourceSyntax, functionName, [.. parameters]);

    public static FunctionCallExpression CreateGeneratedModuleName(ModuleSymbol moduleSymbol, Expression? indexExpression = null)
    {
        var formatParameters = new List<Expression>();
        var formatParametersLength = 0;

        if (moduleSymbol.IsCollection)
        {
            formatParameters.Add(indexExpression ?? new CopyIndexExpression(SourceSyntax: null, Name: null));
            formatParametersLength += MaxCopyIndexStringLength;
        }

        formatParameters.Add(
            CreateFunctionCall(
                "uniqueString",
                null,
                CreateStringLiteral(moduleSymbol.Name, sourceSyntax: null),
                CreatePropertyAccess(
                    CreateFunctionCall("deployment", sourceSyntax: null),
                    "name",
                    null,
                    AccessExpressionFlags.None)));

        formatParametersLength += (int)SystemNamespaceType.UniqueStringHashLength;

        // the format string will use a single dash delimiter per parameter
        formatParametersLength += formatParameters.Count;

        var maxSymbolicNamePrefixLength = LanguageConstants.MaxDeploymentNameLength - formatParametersLength;
        var actualSymbolicNamePrefixLength = Math.Min(maxSymbolicNamePrefixLength, moduleSymbol.Name.Length);
        var symbolicNamePrefix = moduleSymbol.Name[..actualSymbolicNamePrefixLength];

        var formatStringExpression = ExpressionFactory.CreateStringLiteral(moduleSymbol.IsCollection
            ? $"{symbolicNamePrefix}-{{0}}-{{1}}"
            : $"{symbolicNamePrefix}-{{0}}");

        Debug.Assert(actualSymbolicNamePrefixLength <= maxSymbolicNamePrefixLength, "The symbolic name prefix should not exceed the maximum length.");
        Debug.Assert(formatParametersLength + maxSymbolicNamePrefixLength == LanguageConstants.MaxDeploymentNameLength, "The sum of the format parameters length and the symbolic name prefix length should equal the maximum deployment name length.");

        // in loops, the generated name expression should be:
        //   '<symbolicNamePrefix>-${copyIndex()}-${uniqueString('<symbolicName>', deployment().name)}'
        // outside of loops, the name expression should be:
        //   '<symbolicNamePrefix>-uniqueString('<symbolicName>', deployment().name)'

        return CreateFunctionCall("format", formatParameters.Prepend(formatStringExpression));
    }
}
