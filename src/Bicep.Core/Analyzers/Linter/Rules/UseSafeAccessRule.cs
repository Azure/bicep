// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Comparers;
using Bicep.Core.Syntax.Visitors;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class UseSafeAccessRule : LinterRuleBase
{
    public new const string Code = "use-safe-access";

    public UseSafeAccessRule() : base(
        code: Code,
        description: CoreResources.UseSafeAccessRule_Description,
        LinterRuleCategory.BestPractice,
        docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"))
    { }

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        foreach (var ternary in SyntaxAggregator.AggregateByType<TernaryOperationSyntax>(model.Root.Syntax))
        {
            if (SemanticModelHelper.TryGetNamedFunction(model, SystemNamespaceType.BuiltInName, "contains", ternary.ConditionExpression) is not { } functionCall ||
                functionCall.Arguments.Length != 2)
            {
                continue;
            }

            if (ternary.TrueExpression is not AccessExpressionSyntax truePropertyAccess ||
                !SyntaxIgnoringTriviaComparer.Instance.Equals(functionCall.Arguments[0].Expression, truePropertyAccess.BaseExpression))
            {
                continue;
            }

            if (!truePropertyAccess.AccessExpressionMatches(functionCall.Arguments[1].Expression))
            {
                continue;
            }

            // See https://github.com/Azure/bicep/issues/14705 for context on why this is necessary
            var rhsNeedsParentheses = ternary.FalseExpression switch
            {
                TernaryOperationSyntax => true,
                BinaryOperationSyntax binaryOperation =>
                    TokenTypeHelper.GetOperatorPrecedence(binaryOperation.OperatorToken.Type) < TokenTypeHelper.GetOperatorPrecedence(TokenType.DoubleQuestion),
                _ => false,
            };

            var replacement = SyntaxFactory.CreateBinaryOperationSyntax(
                SyntaxFactory.CreateSafeAccess(truePropertyAccess.BaseExpression, functionCall.Arguments[1].Expression),
                TokenType.DoubleQuestion,
                rhsNeedsParentheses ? SyntaxFactory.CreateParenthesized(ternary.FalseExpression) : ternary.FalseExpression);

            yield return CreateFixableDiagnosticForSpan(
                diagnosticLevel,
                ternary.Span,
                new CodeFix(
                    CoreResources.UseSafeAccessRule_CodeFix,
                    isPreferred: true,
                    CodeFixKind.QuickFix,
                    new CodeReplacement(ternary.Span, replacement.ToString())),
                CoreResources.UseSafeAccessRule_MessageFormat);
        }
    }
}
