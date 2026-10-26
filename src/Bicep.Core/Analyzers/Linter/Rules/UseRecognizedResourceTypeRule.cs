// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Bicep.Types.Concrete;
using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem.Providers.Az;
using Microsoft.Extensions.DependencyInjection;

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

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, IServiceProvider serviceProvider, DiagnosticLevel diagnosticLevel)
    {
        var azResourceTypeProvider = serviceProvider.GetRequiredService<AzResourceTypeProvider>();

        var functionCalls = LinterExpressionHelper.FindFunctionCallsByName(
            model,
            model.SourceFile.ProgramSyntax,
            AzNamespaceType.BuiltInName,
            "reference|(list.*)");

        foreach (var functionCall in functionCalls)
        {
            if (TryGetUnrecognizedResourceType(azResourceTypeProvider, model, functionCall) is string unrecognizedType)
            {
                var suggestion = SpellChecker.GetSpellingSuggestion(
                    unrecognizedType,
                    model.Binder.NamespaceResolver.GetAvailableAzureResourceTypes().Select(x => x.FormatType()));

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

    private static string? TryGetUnrecognizedResourceType(AzResourceTypeProvider azResourceTypeProvider, SemanticModel model, FunctionCallSyntaxBase functionCall)
    {
        if (functionCall.Arguments.Length < 1)
        {
            return null;
        }

        var firstArg = functionCall.Arguments[0].Expression;
        if (TryExtractResourceType(model, firstArg) is { } resourceType &&
            !azResourceTypeProvider.TypeReferencesByType.ContainsKey(resourceType))
        {
            return resourceType;
        }

        return null;
    }

    private static string? TryExtractResourceType(SemanticModel model, SyntaxBase expression)
    {
        // Handle resourceId(<resourcetype>, ...)
        if (expression is FunctionCallSyntaxBase functionCall)
        {
            return TryGetResourceTypeFromResourceIdCall(model, functionCall);
        }

        // Handle string literal resource type like 'Microsoft.Storage/storageAccounts'
        if (model.GetTypeInfo(expression).Type is TypeSystem.Types.StringLiteralType stringLiteralType &&
            LinterResourceTypePatterns.ResourceTypeRegex.IsMatch(stringLiteralType.RawStringValue))
        {
            return stringLiteralType.RawStringValue;
        }

        return null;
    }

    private static string? TryGetResourceTypeFromResourceIdCall(SemanticModel model, FunctionCallSyntaxBase functionCall)
    {
        if (!functionCall.NameEquals("resourceId"))
        {
            return null;
        }

        // resourceId() can have optional subscription/resource group args at the beginning,
        // so look for the first argument that looks like a resource type
        foreach (var arg in functionCall.Arguments)
        {
            if (LinterExpressionHelper.TryGetEvaluatedStringLiteral(model, arg.Expression) is { } argLiteral &&
                LinterResourceTypePatterns.ResourceTypeRegex.IsMatch(argLiteral))
            {
                return argLiteral;
            }
        }

        return null;
    }
}
