// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.CodeAction;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    internal class BCPL1060 : LinterRule
    {
        internal BCPL1060() : base(
            code: "BCPL1060",
            ruleName: "Dynamic variable used concat",
            description: "Dynamic variable should not use concat - string interpolation should be used.",
            docUri: "https://bicep/linter/rules/BCPL1060")
        { }


        private CodeReplacement GetCodeReplacement(TextSpan span)
            => new CodeReplacement(span, "BCPL1060 - this is the new code");

        override public IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel model)
        {
            var funcSymbols = model.Root.Declarations.OfType<VariableSymbol>()
                                    .Select(sym => sym.Value)
                                    .OfType<FunctionCallSyntax>();

            foreach (var func in funcSymbols.Where(f => f.Name.IdentifierName.Equals("concat", System.StringComparison.OrdinalIgnoreCase)))
            {
                var fix = CreateFix(func);
                yield return CreateFixableDiagnosticForSpan(func.Span, fix);
            }
        }

        private CodeFix CreateFix(FunctionCallSyntax func) =>
            new CodeFix("Use string interpolation", true, GetCodeReplacement(func));

        private CodeReplacement GetCodeReplacement(FunctionCallSyntax func)
        {
            var arg1 = func.Arguments[0];
            var arg2 = func.Arguments[1];
            return new CodeReplacement(func.Span, $"${arg1}{arg2}");
        }

        //private SyntaxBase? TryParseStringExpression(LanguageExpression expression)
        //{
        //    var flattenedExpression = ExpressionHelpers.FlattenStringOperations(expression);
        //    if (flattenedExpression is JTokenExpression flattenedJTokenExpression)
        //    {
        //        var stringVal = flattenedJTokenExpression.Value.Value<string>();
        //        if (stringVal == null)
        //        {
        //            return null;
        //        }

        //        return SyntaxFactory.CreateStringLiteral(stringVal);
        //    }

        //    if (flattenedExpression is not FunctionExpression functionExpression)
        //    {
        //        throw new InvalidOperationException($"Expected {nameof(FunctionExpression)}");
        //    }
        //    expression = functionExpression;

        //    var values = new List<string>();
        //    var stringTokens = new List<Token>();
        //    var expressions = new List<SyntaxBase>();
        //    for (var i = 0; i < functionExpression.Parameters.Length; i++)
        //    {
        //        // FlattenStringOperations will have already simplified the concat statement to the point where we know there won't be two string literals side-by-side.
        //        // We can use that knowledge to simplify this logic.

        //        var isStart = (i == 0);
        //        var isEnd = (i == functionExpression.Parameters.Length - 1);

        //        if (functionExpression.Parameters[i] is JTokenExpression jTokenExpression)
        //        {
        //            stringTokens.Add(SyntaxFactory.CreateStringInterpolationToken(isStart, isEnd, jTokenExpression.Value.ToString()));

        //            // if done, exit early
        //            if (isEnd)
        //            {
        //                break;
        //            }
        //            // otherwise, process the expression in this iteration
        //            i++;
        //        }
        //        else
        //        {
        //            //  we always need a token between expressions, even if it's empty
        //            stringTokens.Add(SyntaxFactory.CreateStringInterpolationToken(isStart, false, ""));
        //        }

        //        expressions.Add(ParseLanguageExpression(functionExpression.Parameters[i]));
        //        isStart = (i == 0);
        //        isEnd = (i == functionExpression.Parameters.Length - 1);

        //        if (isEnd)
        //        {
        //            // always make sure we end with a string token
        //            stringTokens.Add(SyntaxFactory.CreateStringInterpolationToken(isStart, isEnd, ""));
        //        }
        //    }

        //    var rawSegments = Lexer.TryGetRawStringSegments(stringTokens);
        //    if (rawSegments == null)
        //    {
        //        return null;
        //    }

        //    return new StringSyntax(stringTokens, expressions, rawSegments);
        //}
    }
}
