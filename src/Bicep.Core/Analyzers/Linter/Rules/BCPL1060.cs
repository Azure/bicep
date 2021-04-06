// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.CodeAction;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrint;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

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
                if (CreateFix(func) is CodeFix fix)
                {
                    yield return CreateFixableDiagnosticForSpan(func.Span, fix);
                }
            }
        }

        private CodeFix? CreateFix(FunctionCallSyntax func)
        {
            if (GetCodeReplacement(func) is CodeReplacement cr)
            {
                return new CodeFix($"Use string interpolation: {cr.Text}", true, cr);
            }
            return null;
        }

        private CodeReplacement? GetCodeReplacement(FunctionCallSyntax func)
        {
            if (RewriteConcatToInterpolate(func) is StringSyntax newSyntax)
            {
                return CodeReplacement.FromSyntax(func.Span, newSyntax);
            }
            return null;
        }

        private StringSyntax? RewriteConcatToInterpolate(FunctionCallSyntax func)
        {
            var rewrite = CallbackConvertorRewriter<FunctionCallSyntax, StringSyntax>.Rewrite(func, RewriteConcatCallback);
            return rewrite;
        }

        string SyntaxPrint(SyntaxBase syntax)
        {
            var sb = new StringBuilder();
            var documentBuildVisitor = new DocumentBuildVisitor();
            var document = documentBuildVisitor.BuildDocument(syntax);
            document.Layout(sb, "", System.Environment.NewLine);
            return sb.ToString();
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

            var index = 0;
            var openToken = false;
            var lastToken = false;

            foreach(var syntax in argExpressions) {
                // increment the index first
                lastToken = ++index == argExpressions.Length;

                // if a string literal append
                if (syntax is StringSyntax stringSyntax && !stringSyntax.Expressions.Any())
                {
                    var stringText = string.Join("", stringSyntax.SegmentValues);
                    appendStringLiteral(getTokenType(), stringText);
                }
                // otherwise:  function, variable, other embedded
                else
                {
                    appendInterpolatedSyntax(syntax);
                }
            }

            return new StringSyntax(tokens, expressions, new string[0]);

            TokenType getTokenType()
            {

                if(tokens.Any())
                {
                    return lastToken ? TokenType.StringRightPiece : TokenType.StringMiddlePiece;
                }
                else
                {
                    return TokenType.StringLeftPiece;
                }
            }

            void appendStringLiteral(TokenType tokenType, string literal)
            {
                var newToken = tokenType switch
                {
                    TokenType.StringLeftPiece => SyntaxFactory.CreateStringInterpolationToken(true, false, literal),
                    TokenType.StringRightPiece => SyntaxFactory.CreateStringInterpolationToken(false, true, literal),
                    TokenType.StringMiddlePiece => SyntaxFactory.CreateStringInterpolationToken(false, false, literal),
                    _ => throw new NotSupportedException($"Only string interpolation tokens are supported. Found {tokenType.ToString()}")
                };
                tokens.Add(newToken);
                segments.Add(literal);
                openToken = newToken.Type != TokenType.StringRightPiece;
            }

            void appendInterpolatedSyntax(SyntaxBase syntaxBase)
            {
                // start or middle token must be added before
                // an expression can be added
                if (!openToken)
                {
                    var tokenType = tokens.Any() ? TokenType.StringMiddlePiece : TokenType.StringLeftPiece;
                    appendStringLiteral(tokenType, "");
                }
                // when appending an expression note that
                // no token is current open
                expressions.Add(syntaxBase);
                openToken = false;

                if(lastToken)
                {
                    appendStringLiteral(TokenType.StringRightPiece,"");
                }
            }
        }

        /// <summary>
        /// Rewriter that allows use of a callback to rewrite any type of node.
        /// It can also replace the node type based on callback conversion
        /// </summary>
        protected class CallbackConvertorRewriter<TSyntax, TReturn> : SyntaxRewriteVisitor
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
