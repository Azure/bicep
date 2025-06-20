// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Rewriters;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class PreferInterpolationRule : LinterRuleBase
    {
        public new const string Code = "prefer-interpolation";

        public PreferInterpolationRule() : base(
            code: Code,
            description: CoreResources.InterpolateNotConcatRuleDescription,
            LinterRuleCategory.Style)
        { }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            var concatFunctions = SemanticModelHelper.GetFunctionsByName(model, SystemNamespaceType.BuiltInName, "concat", model.Root.Syntax);

            foreach (var concatFunction in concatFunctions)
            {
                // must have more than 1 argument to use interpolation
                if (concatFunction.Arguments.Length > 1 && !model.HasParsingError(concatFunction))
                {
                    // We should only suggest rewriting concat() calls that result in a string (concat can also operate on and
                    // return arrays)
                    var resultType = model.GetTypeInfo(concatFunction);
                    if (resultType is not AnyType && TypeValidator.AreTypesAssignable(resultType, LanguageConstants.String))
                    {
                        var fix = CreateFix(concatFunction);
                        yield return CreateFixableDiagnosticForSpan(diagnosticLevel, concatFunction.Span, fix);
                    }
                }
            }
        }

        private CodeFix CreateFix(FunctionCallSyntaxBase functionCallSyntax)
        {
            var newSyntax = CreateStringInterpolation(functionCallSyntax);

            var cr = new CodeReplacement(functionCallSyntax.Span, newSyntax.ToString());

            return new CodeFix(CoreResources.InterpolateNotConcatFixTitle, true, CodeFixKind.QuickFix, cr);
        }

        private StringSyntax CreateStringInterpolation(FunctionCallSyntaxBase functionCallSyntax)
        {
            List<string> segments = [""];
            List<SyntaxBase> expressions = [];

            foreach (var arg in functionCallSyntax.Arguments)
            {
                var expression = arg.Expression;

                // if a string literal append
                if (expression is StringSyntax stringSyntax)
                {
                    var prevSegment = segments.Last();
                    segments.RemoveAt(segments.Count - 1);

                    var firstSegment = prevSegment + stringSyntax.SegmentValues.First();

                    segments.Add(firstSegment);
                    segments.AddRange(stringSyntax.SegmentValues.Skip(1));
                    expressions.AddRange(stringSyntax.Expressions);
                }
                // otherwise: some other function, variable, other embedded
                else
                {
                    segments.Add("");
                    expressions.Add(expression);
                }
            }

            return SyntaxFactory.CreateString(segments, expressions);
        }
    }
}
