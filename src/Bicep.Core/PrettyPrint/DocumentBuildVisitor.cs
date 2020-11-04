// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Parser;
using Bicep.Core.PrettyPrint.Documents;
using Bicep.Core.PrettyPrint.DocumentCombinators;
using Bicep.Core.Syntax;

namespace Bicep.Core.PrettyPrint
{
    public class DocumentBuildVisitor : SyntaxVisitor
    {
        private static readonly ILinkedDocument Nil = new NilDocument();

        private static readonly ILinkedDocument Space = new TextDocument(" ", Nil);

        private static readonly ILinkedDocument Line = new NestDocument(0, Nil);

        private static readonly IReadOnlyDictionary<string, TextDocument> CommonTextCache;

        private readonly Stack<ILinkedDocument> documents = new Stack<ILinkedDocument>();

        private readonly Stack<DocumentBlockContext> documentBlockContexts = new Stack<DocumentBlockContext>();

        private readonly List<ILinkedDocument> precedingTrailingComments = new List<ILinkedDocument>();

        static DocumentBuildVisitor()
        {
            var commonTextCache = new Dictionary<string, TextDocument>();

            foreach (var symbol in new[] { "(", ")", "[", "]", "{", "}", "=", ":", "+", "-", "*", "/", "!" })
            {
                commonTextCache.Add(symbol, new TextDocument(symbol, Nil));
            }

            foreach (var name in new[] { "name", "properties", "string", "bool", "int", "array", "object" })
            {
                commonTextCache.Add(name, new TextDocument(name, Nil));
            }

            foreach (var keyword in LanguageConstants.DeclarationKeywords.Concat(LanguageConstants.Keywords.Keys))
            {
                commonTextCache.Add(keyword, new TextDocument(keyword, Nil));
            }

            CommonTextCache = commonTextCache;
        }

        public ILinkedDocument BuildDocument(SyntaxBase syntax)
        {
            this.Visit(syntax);

            Debug.Assert(this.documents.Count == 1);

            return this.documents.Pop();
        }

        public override void VisitProgramSyntax(ProgramSyntax syntax) =>
            this.BuildWithConcat(() =>
            {
                this.documentBlockContexts.Push(new DocumentBlockContext(
                    null,
                    syntax.EndOfFile,
                    syntax.Children.FirstOrDefault(),
                    syntax.Children.LastOrDefault()));

                base.VisitProgramSyntax(syntax);

                this.documentBlockContexts.Pop();
            });

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax) =>
            this.BuildWithSpread(() => base.VisitParameterDeclarationSyntax(syntax));

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax) =>
            this.BuildWithSpread(() => base.VisitVariableDeclarationSyntax(syntax));

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax) =>
            this.BuildWithSpread(() => base.VisitResourceDeclarationSyntax(syntax));

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax) =>
            this.BuildWithSpread(() => base.VisitModuleDeclarationSyntax(syntax));

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax) =>
            this.BuildWithSpread(() => base.VisitOutputDeclarationSyntax(syntax));

        public override void VisitTernaryOperationSyntax(TernaryOperationSyntax syntax) =>
            this.BuildWithSpread(() => base.VisitTernaryOperationSyntax(syntax));

        public override void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax) =>
            this.BuildWithSpread(() => base.VisitBinaryOperationSyntax(syntax));

        public override void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax) =>
            this.BuildWithConcat(() => base.VisitUnaryOperationSyntax(syntax));

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax) =>
            this.BuildWithConcat(() => base.VisitArrayAccessSyntax(syntax));

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax) =>
            this.BuildWithConcat(() => base.VisitPropertyAccessSyntax(syntax));

        public override void VisitParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax) =>
            this.BuildWithConcat(() => base.VisitParenthesizedExpressionSyntax(syntax));

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax) =>
            this.Build(() => base.VisitFunctionCallSyntax(syntax), children =>
            {
                Debug.Assert(children.Length >= 3);

                ILinkedDocument name = children[0];
                ILinkedDocument openParen = children[1];
                ILinkedDocument arguments = Spread(children.Skip(2).SkipLast(1));
                ILinkedDocument closeParen = children[^1];

                return Concat(name, openParen, arguments, closeParen);
            });

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax) =>
            this.Build(() => base.VisitInstanceFunctionCallSyntax(syntax), children =>
            {
                Debug.Assert(children.Length >= 5);

                ILinkedDocument baseExpression = children[0];
                ILinkedDocument dot = children[1];
                ILinkedDocument name = children[2];
                ILinkedDocument openParen = children[3];
                ILinkedDocument arguments = Spread(children.Skip(4).SkipLast(1));
                ILinkedDocument closeParen = children[^1];

                return Concat(baseExpression, dot, name, openParen, arguments, closeParen);
            });

        public override void VisitFunctionArgumentSyntax(FunctionArgumentSyntax syntax) =>
            this.BuildWithConcat(() => base.VisitFunctionArgumentSyntax(syntax));

        public override void VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax) =>
            this.BuildWithConcat(() => base.VisitSkippedTriviaSyntax(syntax));

        public override void VisitStringSyntax(StringSyntax syntax) =>
            this.BuildWithConcat(() => base.VisitStringSyntax(syntax));

        public override void VisitToken(Token token) =>
            this.Build(() =>
            {
                // Leading comments.
                foreach (var commentTrivia in token.LeadingTrivia)
                {
                    this.VisitSyntaxTrivia(commentTrivia);
                }

                // Use Nil as the boundary of the leading comments and the token text.
                this.documents.Push(Nil);

                // Token text.
                this.documents.Push(token.Type switch
                {
                    TokenType.NewLine =>
                        // Normalize newlines.
                        !this.IsBlockFirstNewLine(token) &&
                        !this.IsBlockLastNewLine(token) &&
                        StringUtils.IsMultilineString(token.Text)
                            ? Concat(Line, Line)
                            : Line,
                    _ => Text(token.Text)
                });

                // Trailing comments.
                foreach (var syntaxTrivia in token.TrailingTrivia)
                {
                    this.VisitSyntaxTrivia(syntaxTrivia);
                }
            },
            children =>
            {
                /*
                 * Head:
                 * - If there are leading comments, join them with spaces.
                 * - If the comments are after "[" and "{", add a Line before the comments.
                 * - If the comments are before "]" and "}", add a Line after the comments.
                 */
                var leadingComments = precedingTrailingComments;
                int position = 0;

                while (position < children.Length && children[position] != Nil)
                {
                    leadingComments.Add(children[position]);
                    position++;
                }

                ILinkedDocument head = Spread(leadingComments);

                if (leadingComments.Count > 0)
                {
                    if (this.IsBlockFirstNewLine(token))
                    {
                        this.documents.Push(Line);
                        this.documents.Push(head);
                        head = Nil;
                    }

                    if (this.IsBlockCloseSyntax(token))
                    {
                        this.documents.Push(head);
                        this.documents.Push(Line);
                        head = Nil;
                    }
                }

                // Token text.
                position++;
                Debug.Assert(position < children.Length);
                ILinkedDocument tokenText = children[position];

                /*
                 * Tail:
                 * - Space if there are comments after a token that is not "{", "[" and NewLine.
                 * - Nil otherwise.
                 */
                precedingTrailingComments.Clear();
                var trailingComments = precedingTrailingComments;
                position++;

                while (position < children.Length)
                {
                    trailingComments.Add(children[position]);
                    position++;
                }

                ILinkedDocument tail =
                    trailingComments.Count > 0 &&
                    token.Type != TokenType.NewLine &&
                    !IsBlockOpenSyntax(token)
                        ? Space
                        : Nil;

                return Concat(head, tokenText, tail);
            });

        public override void VisitSyntaxTrivia(SyntaxTrivia syntaxTrivia)
        {
            if (syntaxTrivia.Type == SyntaxTriviaType.SingleLineComment ||
                syntaxTrivia.Type == SyntaxTriviaType.MultiLineComment)
            {
                this.documents.Push(Text(syntaxTrivia.Text));
            }
        }

        public override void VisitObjectSyntax(ObjectSyntax syntax) =>
            this.BuildBlock(
                syntax.OpenBrace,
                syntax.CloseBrace,
                syntax.Children[0],
                syntax.Children[^1],
                () => base.VisitObjectSyntax(syntax));

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax) =>
            this.Build(() => base.VisitObjectPropertySyntax(syntax), children =>
            {
                Debug.Assert(children.Length == 3);

                ILinkedDocument key = children[0];
                ILinkedDocument colon = children[1];
                ILinkedDocument value = children[2];

                return Concat(key, colon, Space, value);
            });

        public override void VisitArraySyntax(ArraySyntax syntax) =>
            this.BuildBlock(
                syntax.OpenBracket,
                syntax.CloseBracket,
                syntax.Children[0],
                syntax.Children[^1],
                () => base.VisitArraySyntax(syntax));

        private static ILinkedDocument Text(string text)
        {
            if (CommonTextCache.TryGetValue(text, out TextDocument cached))
            {
                return cached;
            }

            return new TextDocument(text, Nil);
        }

        private static ILinkedDocument Concat(params ILinkedDocument[] combinators)
            => Concat(combinators as IEnumerable<ILinkedDocument>);

        private static ILinkedDocument Concat(IEnumerable<ILinkedDocument> combinators)
            => combinators.Aggregate(Nil, (a, b) => a.Concat(b));

        private static ILinkedDocument Spread(params ILinkedDocument[] combinators)
            => Spread(combinators as IEnumerable<ILinkedDocument>);

        private static ILinkedDocument Spread(IEnumerable<ILinkedDocument> combinators)
            => Delimited(combinators, Space);

        private static ILinkedDocument Delimited(IEnumerable<ILinkedDocument> combinators, ILinkedDocument delimiter)
        {
            if (!combinators.Any())
            {
                return Nil;
            }

            return combinators.Aggregate((a, b) => a.Concat(delimiter).Concat(b));
        }

        private void BuildWithSpread(Action visitAciton) => this.Build(visitAciton, Spread);

        private void BuildWithConcat(Action visitAciton) => this.Build(visitAciton, Concat);

        private void BuildBlock(SyntaxBase openSyntax, SyntaxBase closeSyntax, SyntaxBase firstNewLine, SyntaxBase lastNewLine, Action visitAction) =>
            this.Build(() =>
            {
                this.documentBlockContexts.Push(new DocumentBlockContext(
                    openSyntax,
                    closeSyntax,
                    firstNewLine,
                    lastNewLine));

                visitAction();

                this.documentBlockContexts.Pop();
            }, children =>
            {
                Debug.Assert(children.Length >= 2);

                ILinkedDocument openSymbol = children[0];
                ILinkedDocument body = Concat(children.Skip(1).SkipLast(2)).Nest(1);
                ILinkedDocument lastLine = children[^2];
                ILinkedDocument closeSymbol = children[^1];

                return Concat(openSymbol, body, lastLine, closeSymbol);
            });

        private void Build(Action visitAction, Func<ILinkedDocument[], ILinkedDocument> buildFunc)
        {
            int beforeCount = this.documents.Count;

            visitAction();

            int childrenCount = this.documents.Count - beforeCount;

            var children = new ILinkedDocument[childrenCount];

            for (int i = childrenCount - 1; i >= 0; i--)
            {
                children[i] = this.documents.Pop();
            }

            this.documents.Push(buildFunc(children));
        }

        private bool IsBlockOpenSyntax(SyntaxBase syntax) => syntax == this.documentBlockContexts.Peek().OpenSyntax;

        private bool IsBlockCloseSyntax(SyntaxBase syntax) => syntax == this.documentBlockContexts.Peek().CloseSyntax;

        private bool IsBlockFirstNewLine(SyntaxBase syntax) => syntax == this.documentBlockContexts.Peek().FirstNewLine;

        private bool IsBlockLastNewLine(SyntaxBase syntax) => syntax == this.documentBlockContexts.Peek().LastNewLine;
    }
}
