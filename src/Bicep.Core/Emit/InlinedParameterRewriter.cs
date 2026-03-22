// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit;

// public class InlinedParameterRewriter(
//     SemanticModel model,
//     ParameterAssignmentEvaluator evaluator,
//     ExpressionConverter converter) : ExpressionRewriteVisitor
// {
//     public Expression Rewrite(Expression expression) => Replace(expression);
//
//     public override Expression ReplaceVariableReferenceExpression(VariableReferenceExpression expression)
//     {
//         var result = evaluator.EvaluateVariable(expression.Variable);
//         if (result.Value is { } value)
//         {
//             return JTokenToExpression(value, expression.SourceSyntax);
//         }
//
//         throw new InvalidOperationException($"Variable {expression.Variable.Name} could not be evaluated");
//     }
//     public override Expression ReplaceParametersAssignmentReferenceExpression(ParametersAssignmentReferenceExpression expression)
//     {
//         var result = evaluator.EvaluateParameter(expression.Parameter);
//         if (result.Value is { } value)
//         {
//             return JTokenToExpression(value, expression.SourceSyntax);
//         }
//
//         throw new InvalidOperationException($"Parameter {expression.Parameter.Name} could not be evaluated");
//     }
//
//
//     public override Expression ReplaceUserDefinedFunctionCallExpression(UserDefinedFunctionCallExpression expression)
//         => ConvertAndEvaluate(base.ReplaceUserDefinedFunctionCallExpression(expression));
//
//     public override Expression ReplaceImportedUserDefinedFunctionCallExpression(ImportedUserDefinedFunctionCallExpression expression)
//         => ConvertAndEvaluate(base.ReplaceImportedUserDefinedFunctionCallExpression(expression));
//
//     public override Expression ReplaceWildcardImportInstanceFunctionCallExpression(WildcardImportInstanceFunctionCallExpression expression)
//         => ConvertAndEvaluate(base.ReplaceWildcardImportInstanceFunctionCallExpression(expression));
//     private Expression ConvertAndEvaluate(Expression expression)
//     {
//         var context = evaluator.GetExpressionEvaluationContextForModel(model);
//         return JTokenToExpression(converter.ConvertExpression(expression).EvaluateExpression(context), expression.SourceSyntax);
//     }
//
//     private static Expression JTokenToExpression(JToken token, SyntaxBase? syntax) => token switch
//     {
//         JObject obj => ExpressionFactory.CreateObject(obj.Properties().Select(p => ExpressionFactory.CreateObjectProperty(p.Name, JTokenToExpression(p.Value, syntax), syntax)), syntax),
//         JArray arr => ExpressionFactory.CreateArray(arr.Select(item => JTokenToExpression(item, syntax)), syntax),
//         JValue jValue => jValue.Value switch
//         {
//             string @string => ExpressionFactory.CreateStringLiteral(@string, syntax),
//             long @long => ExpressionFactory.CreateIntegerLiteral(@long, syntax),
//             bool @bool => ExpressionFactory.CreateBooleanLiteral(@bool, syntax),
//             null => new NullLiteralExpression(syntax),
//             _ => throw new InvalidOperationException($"Unrecognized JValue value of type {jValue.Value?.GetType().Name}"),
//         },
//         _ => throw new InvalidOperationException($"Unsupported JToken type: {token.Type}"),
//     };
// }
