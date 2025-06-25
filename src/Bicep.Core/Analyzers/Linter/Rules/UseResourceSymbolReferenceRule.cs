// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Comparers;
using Bicep.Core.Syntax.Visitors;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class UseResourceSymbolReferenceRule : LinterRuleBase
{
    public new const string Code = "use-resource-symbol-reference";

    public UseResourceSymbolReferenceRule() : base(
        code: Code,
        description: CoreResources.UseResourceSymbolReferenceRule_Description,
        LinterRuleCategory.BestPractice)
    { }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.UseResourceSymbolReferenceRule_MessageFormat, values);

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        var functionCalls = SyntaxAggregator.Aggregate(model.Root.Syntax, syntax =>
            SemanticModelHelper.TryGetFunctionInNamespace(model, AzNamespaceType.BuiltInName, syntax) is { } functionCall && (
                functionCall.Name.IdentifierName.StartsWith("list", LanguageConstants.IdentifierComparison) ||
                functionCall.Name.IdentifierName.Equals("reference", LanguageConstants.IdentifierComparison)))
            .OfType<FunctionCallSyntaxBase>();

        foreach (var functionCall in functionCalls)
        {
            if (functionCall.Name.IdentifierName.Equals("reference", LanguageConstants.IdentifierComparison) &&
                AnalyzeReferenceCall(model, diagnosticLevel, functionCall) is { } referenceDiagnostic)
            {
                yield return referenceDiagnostic;
            }

            if (functionCall.Name.IdentifierName.StartsWith("list", LanguageConstants.IdentifierComparison) &&
                AnalyzeListCall(model, diagnosticLevel, functionCall) is { } listDiagnostic)
            {
                yield return listDiagnostic;
            }
        }
    }

    private IEnumerable<DeclaredResourceMetadata> AnalyzeResourceId(SemanticModel model, SyntaxBase syntax)
    {
        if (syntax is PropertyAccessSyntax idProp &&
            (idProp.PropertyName.NameEquals("id") || idProp.PropertyName.NameEquals("name")) &&
            model.ResourceMetadata.TryLookup(idProp.BaseExpression) is DeclaredResourceMetadata idResource)
        {
            yield return idResource;
            yield break;
        }

        if (SemanticModelHelper.TryGetFunctionInNamespace(model, AzNamespaceType.BuiltInName, syntax) is not { } functionCall)
        {
            yield break;
        }

        if (functionCall.Name.NameEquals("resourceId") && functionCall.Arguments.Length > 1)
        {
            // Only support the scope-less format for now...
            if (functionCall.Arguments[0].Expression is not StringSyntax s ||
                s.TryGetLiteralValue() is not { } resourceType ||
                !resourceType.Contains('/'))
            {
                yield break;
            }

            var matchingResources = model.AllResources
                .Where(x => x.IsAzResource)
                .Where(x => x.TypeReference.FormatType().EqualsOrdinalInsensitively(resourceType))
                .OfType<DeclaredResourceMetadata>();

            foreach (var resource in matchingResources)
            {
                var remainingArgs = functionCall.Arguments.Skip(1).Select(x => x.Expression);
                var ancestry = EnumerableExtensions.EnumerateRecursively(resource, x => x.Parent?.Metadata as DeclaredResourceMetadata)
                    .Reverse().Select(x => x.TryGetNameSyntax());

                if (Enumerable.SequenceEqual(ancestry, remainingArgs, SyntaxIgnoringTriviaComparer.Instance))
                {
                    yield return resource;
                }
            }
        }
    }

    private DeclaredResourceMetadata? TryMatchApiVersion(SemanticModel model, ImmutableArray<DeclaredResourceMetadata> resourceCandidates, SyntaxBase? syntax)
    {
        if (syntax is PropertyAccessSyntax prop &&
            prop.PropertyName.NameEquals("apiVersion") &&
            model.ResourceMetadata.TryLookup(prop.BaseExpression) is DeclaredResourceMetadata apiVersionResource &&
            resourceCandidates.IndexOf(apiVersionResource) > -1)
        {
            return apiVersionResource;
        }

        if (syntax is StringSyntax s &&
            s.TryGetLiteralValue() is { } version &&
            resourceCandidates.FirstOrDefault(x => x.TypeReference.ApiVersion.EqualsOrdinalInsensitively(version)) is { } resource)
        {
            return resource;
        }

        if (syntax is null && resourceCandidates.Length == 1)
        {
            return resourceCandidates.First();
        }

        return null;
    }

    private IDiagnostic? AnalyzeReferenceCall(SemanticModel model, DiagnosticLevel diagnosticLevel, FunctionCallSyntaxBase functionCall)
    {
        if (functionCall.Arguments.Length < 1 ||
            functionCall.Arguments.Length > 3)
        {
            return null;
        }

        var idArg = functionCall.Arguments[0].Expression;
        var apiVersionArg = functionCall.Arguments.Length > 1 ? functionCall.Arguments[1].Expression : null;

        var resourceCandidates = AnalyzeResourceId(model, idArg).ToImmutableArray();
        var resource = TryMatchApiVersion(model, resourceCandidates, apiVersionArg);
        if (resource is null)
        {
            return null;
        }

        var isFull = functionCall.Arguments.Length > 2 &&
            functionCall.Arguments[2].Expression is StringSyntax fullString &&
            fullString.TryGetLiteralValue() is { } fullValue &&
            fullValue.EqualsOrdinalInsensitively("full");

        SyntaxBase newSyntax = SyntaxFactory.CreateIdentifier(resource.Symbol.Name);
        if (!isFull)
        {
            newSyntax = SyntaxFactory.CreateAccessSyntax(newSyntax, "properties");
        }

        var codeReplacement = new CodeReplacement(functionCall.Span, newSyntax.ToString());

        return CreateFixableDiagnosticForSpan(
            diagnosticLevel,
            functionCall.Span,
            new CodeFix(CoreResources.UseResourceSymbolReferenceRule_CodeFix, isPreferred: true, CodeFixKind.QuickFix, codeReplacement),
            functionCall.Name.IdentifierName);
    }

    private IDiagnostic? AnalyzeListCall(SemanticModel model, DiagnosticLevel diagnosticLevel, FunctionCallSyntaxBase functionCall)
    {
        if (functionCall.Arguments.Length < 2 ||
            functionCall.Arguments.Length > 3)
        {
            return null;
        }

        var idArg = functionCall.Arguments[0].Expression;
        var apiVersionArg = functionCall.Arguments.Length > 1 ? functionCall.Arguments[1].Expression : null;

        var resourceCandidates = AnalyzeResourceId(model, idArg).ToImmutableArray();
        var resource = TryMatchApiVersion(model, resourceCandidates, apiVersionArg);
        if (resource is null)
        {
            return null;
        }

        var newArgs = functionCall.Arguments.Length == 2 ?
            new SyntaxBase[] { } :
            [functionCall.Arguments[1].Expression, functionCall.Arguments[2].Expression];

        var newFunctionCall = SyntaxFactory.CreateInstanceFunctionCall(
            SyntaxFactory.CreateIdentifier(resource.Symbol.Name),
            functionCall.Name.IdentifierName,
            newArgs);

        var codeReplacement = new CodeReplacement(functionCall.Span, newFunctionCall.ToString());

        return CreateFixableDiagnosticForSpan(
            diagnosticLevel,
            functionCall.Span,
            new CodeFix(CoreResources.UseResourceSymbolReferenceRule_CodeFix, isPreferred: true, CodeFixKind.QuickFix, codeReplacement),
            functionCall.Name.IdentifierName);
    }
}
