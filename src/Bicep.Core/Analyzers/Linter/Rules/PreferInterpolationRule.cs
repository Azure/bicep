// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class PreferInterpolationRule : LinterRuleBase
    {
        public new const string Code = "prefer-interpolation";

        public PreferInterpolationRule() : base(
            code: Code,
            description: CoreResources.InterpolateNotConcatRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"))
        { }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var visitor = new Visitor(this, model);
            visitor.Visit(model.SourceFile.ProgramSyntax);
            return visitor.diagnostics;
        }

        private class Visitor : SyntaxVisitor
        {
            public List<IDiagnostic> diagnostics = new List<IDiagnostic>();

            private const string concatFunction = "concat";
            private readonly PreferInterpolationRule parent;
            private readonly SemanticModel model;

            public Visitor(PreferInterpolationRule parent, SemanticModel model)
            {
                this.parent = parent;
                this.model = model;
            }

            public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
            {
                // must have more than 1 argument to use interpolation
                if (syntax.NameEquals(concatFunction)
                   && syntax.Arguments.Length > 1
                   && !syntax.GetParseDiagnostics().Any())
                {
                    // We should only suggest rewriting concat() calls that result in a string (concat can also operate on and
                    // return arrays)
                    var resultType = this.model.GetTypeInfo(syntax);
                    if (resultType is not AnyType && TypeValidator.AreTypesAssignable(resultType, LanguageConstants.String))
                    {
                        if (CreateFix(syntax) is CodeFix fix)
                        {
                            this.diagnostics.Add(parent.CreateFixableDiagnosticForSpan(syntax.Span, fix));

                            // Only report on the top-most string-valued concat call
                            return;
                        }
                    }
                }

                base.VisitFunctionCallSyntax(syntax);
            }

            private CodeFix? CreateFix(FunctionCallSyntax functionCallSyntax)
            {
                if (GetCodeReplacement(functionCallSyntax) is CodeReplacement cr)
                {
                    string title = string.Format(CoreResources.InterpolateNotConcatFixTitle, cr.Text);
                    return new CodeFix(title, true, cr);
                }
                return null;
            }

            private CodeReplacement? GetCodeReplacement(FunctionCallSyntax functionCallSyntax)
            {
                if (RewriteConcatToInterpolate(functionCallSyntax) is StringSyntax newSyntax)
                {
                    return CodeReplacement.FromSyntax(functionCallSyntax.Span, newSyntax);
                }
                return null;
            }

            private StringSyntax? RewriteConcatToInterpolate(FunctionCallSyntax func)
            {
                var rewrite = CallbackConvertorRewriter<FunctionCallSyntax, StringSyntax>.Rewrite(func, RewriteConcatCallback);
                return rewrite;
            }

            private StringSyntax RewriteConcatCallback(FunctionCallSyntax syntax)
            {
                var tokens = new List<Token>();
                var expressions = new List<SyntaxBase>();
                var segments = new List<string>();

                var flattened = SyntaxFactory.FlattenStringOperations(syntax);
                if (flattened is FunctionCallSyntax concatSyntax)
                {
                    return CreateStringInterpolation(concatSyntax.Arguments.Select(a => a.Expression).ToImmutableArray());
                }
                else if (flattened is StringSyntax stringSyntax)
                {
                    return stringSyntax;
                }

                // TODO:  What is the correct way to handle a failed codefix?
                throw new NotSupportedException("Rewrite to string interpolation not successful");
            }

            /// <summary>
            /// TODO: Move to SyntaxFactory
            /// </summary>
            /// <param name="argExpressions"></param>
            /// <returns></returns>
            private StringSyntax CreateStringInterpolation(ImmutableArray<SyntaxBase> argExpressions)
            {
                var tokens = new List<Token>();
                var expressions = new List<SyntaxBase>();
                var segments = new List<string>();

                SyntaxBase? prevArg = default;
                var argList = argExpressions.Select((arg, i) => new { arg = arg, argindex = i });

                void addStringSyntax(StringSyntax stringSyntax)
                {
                    expressions.AddRange(stringSyntax.Expressions);
                    segments.AddRange(stringSyntax.SegmentValues);
                }

                foreach (var argSet in argList)
                {
                    // if a string literal append
                    if (argSet.arg is StringSyntax stringSyntax)
                    {
                        addStringSyntax(stringSyntax);
                        prevArg = stringSyntax;
                    }
                    else if (argSet.arg is FunctionCallSyntax funcSyntax && funcSyntax.NameEquals(concatFunction))
                    {
                        stringSyntax = RewriteConcatCallback(funcSyntax);
                        addStringSyntax(stringSyntax);
                        prevArg = stringSyntax;
                    }
                    // otherwise: some other function, variable, other embedded
                    else
                    {
                        // not preceded by a string segment
                        if (prevArg is not StringSyntax)
                        {
                            segments.Add("");
                        }

                        expressions.Add(argSet.arg);
                        prevArg = argSet.arg;
                    }
                }

                // close out interpolation if needed
                if (prevArg is not StringSyntax)
                {
                    segments.Add("");
                }

                // build tokens from segment list
                var last = segments.Count() - 1;
                var index = 0;
                segments.ForEach(segment =>
                {
                    tokens.Add(SyntaxFactory.CreateStringInterpolationToken(index == 0, index == last, segment));
                    index++;
                });

                return new StringSyntax(tokens, expressions, segments);
            }
        }

        /// <summary>
        /// Rewriter that allows use of a callback to rewrite any type of node.
        /// It can also replace the node type based on callback conversion
        /// </summary>
        private class CallbackConvertorRewriter<TSyntax, TReturn> : SyntaxRewriteVisitor
            where TSyntax : SyntaxBase
            where TReturn : SyntaxBase
        {
            private readonly Func<TSyntax, TReturn> callback;

            public static TSyntaxOut? Rewrite<TSyntaxIn, TSyntaxOut>(TSyntaxIn syntax, Func<TSyntaxIn, TSyntaxOut> callback)
                where TSyntaxIn : TSyntax
                where TSyntaxOut : TReturn
            {
                var rewriter = new CallbackConvertorRewriter<TSyntaxIn, TSyntaxOut>(callback);
                if (rewriter != null)
                {
                    return rewriter.Rewrite<TSyntaxIn, TSyntaxOut>(syntax);
                }
                return null;
            }

            private CallbackConvertorRewriter(Func<TSyntax, TReturn> callback)
            {
                this.callback = callback;
            }

            protected override SyntaxBase RewriteInternal(SyntaxBase syntax)
                => this.callback((TSyntax)syntax);
        }
    }
}
