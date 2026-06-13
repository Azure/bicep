// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Text;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class UseRecognizedResourceTypeRule : LinterRuleBase
{
    public new const string Code = "use-recognized-resource-type";

    public UseRecognizedResourceTypeRule() : base(
        code: Code,
        description: CoreResources.UseRecognizedResourceTypeRule_Description,
        LinterRuleCategory.PotentialCodeIssues)
    { }

    public override string FormatMessage(params object[] values)
        => (string)values[0];

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        var functionCalls = LinterExpressionHelper.FindFunctionCallsByName(
            model,
            model.SourceFile.ProgramSyntax,
            AzNamespaceType.BuiltInName,
            "reference|(list.*)");

        foreach (var functionCall in functionCalls)
        {
            if (TryGetUnrecognizedResourceType(model, functionCall) is string unrecognizedType)
            {
                var suggestion = SpellChecker.GetSpellingSuggestion(
                    unrecognizedType,
                    model.ApiVersionProvider.GetResourceTypeNames(model.TargetScope));

                string message;
                if (suggestion is not null)
                {
                    message = string.Format(CoreResources.UseRecognizedResourceTypeRule_MessageFormatWithSuggestion, unrecognizedType, functionCall.Name.IdentifierName, suggestion);
                }
                else
                {
                    message = string.Format(CoreResources.UseRecognizedResourceTypeRule_MessageFormat, unrecognizedType, functionCall.Name.IdentifierName);
                }

                yield return CreateDiagnosticForSpan(diagnosticLevel, functionCall.Span, message);
            }
        }
    }

    private static string? TryGetUnrecognizedResourceType(SemanticModel model, FunctionCallSyntaxBase functionCall)
    {
        if (functionCall.Arguments.Length < 1)
        {
            return null;
        }

        var firstArg = functionCall.Arguments[0].Expression;
        var resourceType = TryExtractResourceType(model, firstArg);

        if (resourceType is null)
        {
            return null;
        }

        // Check if the resource type is recognized
        if (model.ApiVersionProvider.GetApiVersions(model.TargetScope, resourceType).Any())
        {
            return null;
        }

        return resourceType;
    }

    private static string? TryExtractResourceType(SemanticModel model, SyntaxBase expression)
    {
        // Handle resourceId(<resourcetype>, ...)
        if (expression is FunctionCallSyntaxBase functionCall)
        {
            return TryGetResourceTypeFromResourceIdCall(model, functionCall);
        }

        // Handle string literal resource type like 'Microsoft.Storage/storageAccounts'
        if (LinterExpressionHelper.TryGetEvaluatedStringLiteral(model, expression)
            is (string literalValue, _, _) && LinterResourceTypePatterns.ResourceTypeRegex.IsMatch(literalValue))
        {
            return literalValue;
        }

        return null;
    }

    private static string? TryGetResourceTypeFromResourceIdCall(SemanticModel model, FunctionCallSyntaxBase functionCall)
    {
        if (!functionCall.Name.IdentifierName.Equals("resourceId", LanguageConstants.IdentifierComparison))
        {
            return null;
        }

        // resourceId() can have optional subscription/resource group args at the beginning,
        // so look for the first argument that looks like a resource type
        foreach (var arg in functionCall.Arguments)
        {
            if (LinterExpressionHelper.TryGetEvaluatedStringLiteral(model, arg.Expression) is (string argLiteral, _, _))
            {
                argLiteral = argLiteral.TrimEnd('/');
                if (LinterResourceTypePatterns.ResourceTypeRegex.IsMatch(argLiteral))
                {
                    return argLiteral;
                }
            }
        }

        return null;
    }
}
