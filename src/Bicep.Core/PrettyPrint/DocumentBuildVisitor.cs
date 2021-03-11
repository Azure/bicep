// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrint.Documents;
using Bicep.Core.Syntax;

namespace Bicep.Core.PrettyPrint
{
    public class DocumentBuildVisitor : SyntaxVisitor
    {
        private static readonly ILinkedDocument Nil = new NilDocument();

        private static readonly ILinkedDocument Space = new TextDocument(" ");

        private static readonly ILinkedDocument Line = new NestDocument(0);

        private static readonly ILinkedDocument NoLine = new NilDocument();

        private static readonly ILinkedDocument SingleLine = new NestDocument(0);

        private static readonly ILinkedDocument DoubleLine = new NestDocument(0).Concat(Line);

        private static readonly ImmutableDictionary<string, TextDocument> CommonTextCache =
            LanguageConstants.ContextualKeywords
            .Concat(LanguageConstants.Keywords.Keys)
            .Concat(new[] { "(", ")", "[", "]", "{", "}", "=", ":", "+", "-", "*", "/", "!" })
            .Concat(new[] { "name", "properties", "string", "bool", "int", "array", "object" })
            .ToImmutableDictionary(value => value, value => new TextDocument(value));

        private readonly Stack<ILinkedDocument> documentStack = new Stack<ILinkedDocument>();

        private bool visitingBlockOpenSyntax;

        private bool visitingBlockCloseSyntax;

        private bool visitingSkippedTriviaSyntax;

        private bool visitingBrokenStatement;

        private bool visitingComment;

        public ILinkedDocument BuildDocument(SyntaxBase syntax)
        {
            this.Visit(syntax);

            Debug.Assert(this.documentStack.Count == 1);

            return this.documentStack.Pop();
        }

        public override void VisitProgramSyntax(ProgramSyntax syntax) =>
            this.BuildWithConcat(() =>
            {
                this.PushDocument(NoLine);
                this.VisitNodes(syntax.Children);
                this.PushDocument(NoLine);
                this.Visit(syntax.EndOfFile);
            });

        public override void VisitDecoratorSyntax(DecoratorSyntax syntax) =>
            this.BuildWithConcat(() => base.VisitDecoratorSyntax(syntax));

        public override void VisitTargetScopeSyntax(TargetScopeSyntax syntax) =>
            this.BuildStatement(syntax, () =>
            {
                this.VisitNodes(syntax.LeadingNodes);
                this.Visit(syntax.Keyword);
                this.documentStack.Push(Nil);
                this.Visit(syntax.Assignment);
                this.Visit(syntax.Value);
            });

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax) =>
            this.BuildStatement(syntax, () =>
            {
                this.VisitNodes(syntax.LeadingNodes);
                this.Visit(syntax.Keyword);
                this.documentStack.Push(Nil);
                this.Visit(syntax.Name);
                this.Visit(syntax.Type);
                this.Visit(syntax.Modifier);
            });

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax) =>
            this.BuildStatement(syntax, () =>
            {
                this.VisitNodes(syntax.LeadingNodes);
                this.Visit(syntax.Keyword);
                this.documentStack.Push(Nil);
                this.Visit(syntax.Name);
                this.Visit(syntax.Assignment);
                this.Visit(syntax.Value);
            });

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax) =>
            this.BuildStatement(syntax, () =>
            {
                this.VisitNodes(syntax.LeadingNodes);
                this.Visit(syntax.Keyword);
                this.documentStack.Push(Nil);
                this.Visit(syntax.Name);
                this.Visit(syntax.Type);
                this.Visit(syntax.ExistingKeyword);
                this.Visit(syntax.Assignment);
                this.Visit(syntax.Value);
            });

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax) =>
            this.BuildStatement(syntax, () =>
            {
                this.VisitNodes(syntax.LeadingNodes);
                this.Visit(syntax.Keyword);
                this.documentStack.Push(Nil);
                this.Visit(syntax.Name);
                this.Visit(syntax.Path);
                this.Visit(syntax.Assignment);
                this.Visit(syntax.Value);
            });

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax) =>
            this.BuildStatement(syntax, () =>
            {
                this.VisitNodes(syntax.LeadingNodes);
                this.Visit(syntax.Keyword);
                this.documentStack.Push(Nil);
                this.Visit(syntax.Name);
                this.Visit(syntax.Type);
                this.Visit(syntax.Assignment);
                this.Visit(syntax.Value);
            });

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

        public override void VisitResourceAccessSyntax(ResourceAccessSyntax syntax) =>
            this.BuildWithConcat(() => base.VisitResourceAccessSyntax(syntax));

        public override void VisitParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax) =>
            this.BuildWithConcat(() => base.VisitParenthesizedExpressionSyntax(syntax));

        public override void VisitForSyntax(ForSyntax syntax) =>
            this.Build(() => base.VisitForSyntax(syntax), children =>
            {
                Debug.Assert(children.Length == 8);

                ILinkedDocument openBracket = children[0];
                ILinkedDocument forKeyword = children[1];
                ILinkedDocument variableBlock = children[2];
                ILinkedDocument inKeyword = children[3];
                ILinkedDocument arrayExpression = children[4];
                ILinkedDocument colon = children[5];
                ILinkedDocument loopBody = children[6];
                ILinkedDocument closeBracket = children[7];

                return Concat(openBracket, Spread(forKeyword, variableBlock, inKeyword, arrayExpression), Spread(colon, loopBody), closeBracket);
            });

        public override void VisitForVariableBlockSyntax(ForVariableBlockSyntax syntax) =>
            this.Build(() => base.VisitForVariableBlockSyntax(syntax), children =>
             {
                 Debug.Assert(children.Length == 5);

                 ILinkedDocument openParen = children[0];
                 ILinkedDocument itemVariable = children[1];
                 ILinkedDocument comma = children[2];
                 ILinkedDocument indexVariable = children[3];
                 ILinkedDocument closeParen = children[4];

                 return Spread(Concat(openParen, itemVariable, comma), Concat(indexVariable, closeParen));
             });

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

        public override void VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax)
        {
            ILinkedDocument top = this.documentStack.Peek();

            if (!this.visitingBrokenStatement &&
                syntax.Elements.Length > 0 &&
                top != NoLine &&
                top != Line &&
                top != SingleLine &&
                top != DoubleLine)
            {
                /*
                 * The skipped trivia is after some valid declaration which won't emit trailing whitespaces,
                 * so insert a Space as a separator.
                 */
                this.documentStack.Push(Space);
            }

            this.visitingSkippedTriviaSyntax = true;
            base.VisitSkippedTriviaSyntax(syntax);
            this.visitingSkippedTriviaSyntax = false;
        }

        public override void VisitStringSyntax(StringSyntax syntax) =>
            this.BuildWithConcat(() => base.VisitStringSyntax(syntax));

        public override void VisitToken(Token token)
        {
            foreach (var trivia in token.LeadingTrivia)
            {
                this.VisitSyntaxTrivia(trivia);
            }

            var pushDocument = this.visitingBrokenStatement
                ? (Action<ILinkedDocument>)this.documentStack.Push
                : (Action<ILinkedDocument>)this.PushDocument;

            if (token.Type == TokenType.NewLine)
            {
                int newlineCount = StringUtils.CountNewlines(token.Text);

                for (int i = 0; i < newlineCount; i++)
                {
                    pushDocument(Line);
                }
            }
            else
            {
                pushDocument(Text(token.Text));
            }

            foreach (var trivia in token.TrailingTrivia)
            {
                this.VisitSyntaxTrivia(trivia);
            }
        }

        public override void VisitSyntaxTrivia(SyntaxTrivia syntaxTrivia)
        {
            if (this.visitingBrokenStatement || this.visitingSkippedTriviaSyntax)
            {
                if (syntaxTrivia.Type == SyntaxTriviaType.Whitespace &&
                    this.visitingSkippedTriviaSyntax &&
                    this.documentStack.Peek() == Space)
                {
                    this.documentStack.Pop();
                }

                this.documentStack.Push(Text(syntaxTrivia.Text));
                return;
            }

            if (syntaxTrivia.Type == SyntaxTriviaType.SingleLineComment ||
                syntaxTrivia.Type == SyntaxTriviaType.MultiLineComment)
            {
                this.visitingComment = true;
                this.PushDocument(Text(syntaxTrivia.Text));
                this.visitingComment = false;
            }
        }

        public override void VisitObjectSyntax(ObjectSyntax syntax) =>
            this.BuildBlock(() =>
            {
                this.visitingBlockOpenSyntax = true;
                this.Visit(syntax.OpenBrace);
                this.visitingBlockOpenSyntax = false;

                this.VisitNodes(syntax.Children);

                this.visitingBlockCloseSyntax = true;
                this.Visit(syntax.CloseBrace);
                this.visitingBlockCloseSyntax = false;
            });

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
            this.BuildBlock(() =>
            {
                this.visitingBlockOpenSyntax = true;
                this.Visit(syntax.OpenBracket);
                this.visitingBlockOpenSyntax = false;

                this.VisitNodes(syntax.Children);

                this.visitingBlockCloseSyntax = true;
                this.Visit(syntax.CloseBracket);
                this.visitingBlockCloseSyntax = false;
            });

        private static ILinkedDocument Text(string text) =>
            CommonTextCache.TryGetValue(text, out var cached) ? cached : new TextDocument(text);

        private static ILinkedDocument Concat(params ILinkedDocument[] combinators) =>
            Concat(combinators as IEnumerable<ILinkedDocument>);

        private static ILinkedDocument Concat(IEnumerable<ILinkedDocument> combinators) =>
            combinators.Aggregate(Nil, (a, b) => a.Concat(b));

        private static ILinkedDocument Spread(params ILinkedDocument[] combinators) =>
            Spread(combinators as IEnumerable<ILinkedDocument>);

        private static ILinkedDocument Spread(IEnumerable<ILinkedDocument> combinators) =>
            combinators.Any() ? combinators.Aggregate((a, b) => a.Concat(Space).Concat(b)) : Nil;

        private void BuildWithConcat(Action visitAciton) => this.Build(visitAciton, Concat);

        private void BuildWithSpread(Action visitAciton) => this.Build(visitAciton, Spread);

        private void BuildBlock(Action visitAction) =>
            this.Build(visitAction, children =>
            {
                Debug.Assert(children.Length >= 2);

                ILinkedDocument openSymbol = children[0];
                ILinkedDocument body = Concat(children.Skip(1).SkipLast(2)).Nest();
                ILinkedDocument lastLine = children.Length > 2 ? children[^2] : Nil;
                ILinkedDocument closeSymbol = children[^1];

                return Concat(openSymbol, body, lastLine, closeSymbol);
            });

        private void BuildStatement(SyntaxBase syntax, Action visitAction)
        {
            if (syntax.GetParseDiagnostics().Count > 0)
            {
                this.visitingBrokenStatement = true;
                visitAction();
                this.visitingBrokenStatement = false;

                // Everyting left on the stack will be concatenated by the top level Concat rule defined in VisitProgram.
                return;
            }

            this.Build(visitAction, children =>
            {
                var splitIndex = Array.IndexOf(children, Nil);

                // Need to concat leading decorators and the statment keyword.
                var head = Concat(children.Take(splitIndex));
                var tail = children.Skip(splitIndex + 1);

                return Spread(head.AsEnumerable().Concat(tail));
            });
        }

        private void Build(Action visitAction, Func<ILinkedDocument[], ILinkedDocument> buildFunc)
        {
            int beforeCount = this.documentStack.Count;

            visitAction();

            if (this.visitingBrokenStatement)
            {
                return;
            }

            int childrenCount = this.documentStack.Count - beforeCount;

            var children = new ILinkedDocument[childrenCount];

            for (int i = childrenCount - 1; i >= 0; i--)
            {
                children[i] = this.documentStack.Pop();
            }

            this.PushDocument(buildFunc(children));
        }

        private void PushDocument(ILinkedDocument document)
        {
            if (this.documentStack.Count == 0)
            {
                this.documentStack.Push(document);

                return;
            }

            ILinkedDocument top = this.documentStack.Peek();

            if (document == Line)
            {
                if (top == NoLine || top == SingleLine || top == DoubleLine)
                {
                    // No newlines are allowed after a NoLine / SingleLine / DoubleLine.
                    return;
                }

                if (top == Line)
                {
                    // Two Lines form a DoubleLine that prevents more newlines from being added.
                    this.documentStack.Pop();
                    this.documentStack.Push(DoubleLine);
                }
                else
                {
                    this.documentStack.Push(Line);
                }
            }
            else if (document == NoLine)
            {
                if (top == Line || top == DoubleLine)
                {
                    // No newlines are allowed before a NoLine.
                    this.documentStack.Pop();
                }

                this.documentStack.Push(document);
            }
            else if (document == SingleLine)
            {
                if (top == SingleLine)
                {
                    // When two SingleLines meet, the current block is empty. Remove all newlines inside the block.
                    this.documentStack.Pop();
                    return;
                }

                if (top == Line || top == DoubleLine)
                {
                    // No newlines are allowed before a SingleLine.
                    this.documentStack.Pop();
                }

                this.documentStack.Push(document);
            }
            else if (visitingComment)
            {
                // Add a space before the comment if it's not at the begining of the file or after a newline.
                ILinkedDocument gap = top != NoLine && top != Line && top != SingleLine && top != DoubleLine ? Space : Nil;

                // Combine the comment and the document at the top of the stack. This is the key to simplify VisitToken.
                this.documentStack.Push(Concat(this.documentStack.Pop(), gap, document));
            }
            else
            {
                if (this.visitingBlockCloseSyntax)
                {
                    // Insert a SingleLine before "}" and "]", which will remove extra newlines before it (by calling PushDocument).
                    this.PushDocument(SingleLine);
                }

                this.documentStack.Push(document);

                if (this.visitingBlockOpenSyntax)
                {
                    // Add a SingleLine after "{" and "[", which will prevent more newlines from being added after it.
                    this.documentStack.Push(SingleLine);
                }
            }
        }
    }
}
