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

                if (!syntax.EndOfFile.LeadingTrivia.Any(x => x.Type == SyntaxTriviaType.DisableNextLineDiagnosticsDirective))
                {
                    this.PushDocument(NoLine);
                }

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

        public override void VisitImportDeclarationSyntax(ImportDeclarationSyntax syntax) =>
            this.BuildStatement(syntax, () =>
            {
                this.VisitNodes(syntax.LeadingNodes);
                this.Visit(syntax.Keyword);
                this.documentStack.Push(Nil);
                this.Visit(syntax.ProviderName);
                this.Visit(syntax.AsKeyword);
                this.Visit(syntax.AliasName);
                this.Visit(syntax.Config);
            });

        public override void VisitMetadataDeclarationSyntax(MetadataDeclarationSyntax syntax) =>
            this.BuildStatement(syntax, () =>
            {
                this.VisitNodes(syntax.LeadingNodes);
                this.Visit(syntax.Keyword);
                this.documentStack.Push(Nil);
                this.Visit(syntax.Name);
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

        public override void VisitParameterAssignmentSyntax(ParameterAssignmentSyntax syntax) =>
            this.BuildStatement(syntax, () =>
            {
                this.VisitNodes(syntax.LeadingNodes);
                this.Visit(syntax.Keyword);
                this.documentStack.Push(Nil);
                this.Visit(syntax.Name);
                this.Visit(syntax.Assignment);
                this.Visit(syntax.Value);
            });

        public override void VisitUsingDeclarationSyntax(UsingDeclarationSyntax syntax) =>
            this.BuildStatement(syntax, () =>
            {
                this.VisitNodes(syntax.LeadingNodes);
                this.Visit(syntax.Keyword);
                this.documentStack.Push(Nil);
                this.Visit(syntax.Path);
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

        public override void VisitIfConditionSyntax(IfConditionSyntax syntax) =>
            this.BuildWithSpread(() => base.VisitIfConditionSyntax(syntax));

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

        public override void VisitVariableBlockSyntax(VariableBlockSyntax syntax) =>
            this.BuildWithConcat(() => {
                this.Visit(syntax.OpenParen);
                this.VisitCommaAndNewLineSeparated(syntax.Children, leadingAndTrailingSpace: false);
                this.Visit(syntax.CloseParen);
            });

        public override void VisitLambdaSyntax(LambdaSyntax syntax) =>
            this.Build(() => base.VisitLambdaSyntax(syntax), children =>
             {
                 Debug.Assert(children.Length == 3);

                 ILinkedDocument token = children[0];
                 ILinkedDocument arrow = children[1];
                 ILinkedDocument body = children[2];

                 return Spread(token, arrow, body);
             });

        private void VisitCommaAndNewLineSeparated(ImmutableArray<SyntaxBase> nodes, bool leadingAndTrailingSpace)
        {
            SyntaxBase? leadingNewLine = null;
            if (nodes.Length > 0 && nodes[0] is Token { Type: TokenType.NewLine })
            {
                leadingNewLine = nodes[0];
                nodes = nodes.RemoveAt(0);
            }

            SyntaxBase? trailingNewLine = null;
            if (nodes.Length > 0 && nodes[^1] is Token { Type: TokenType.NewLine })
            {
                trailingNewLine = nodes[^1];
                nodes = nodes.RemoveAt(nodes.Length - 1);
            }

            this.Build(() => {
                this.Visit(leadingNewLine);
                if (leadingAndTrailingSpace && nodes.Any() && leadingNewLine is null)
                {
                    this.PushDocument(Space);
                }

                for (var i = 0; i < nodes.Length; i++)
                {
                    this.Visit(nodes[i]);

                    if (i < nodes.Length - 1 &&
                        nodes[i] is Token { Type: TokenType.Comma } &&
                        nodes[i + 1] is not Token { Type: TokenType.NewLine })
                    {
                        this.PushDocument(Space);
                    }
                }

                if (leadingAndTrailingSpace && nodes.Any() && trailingNewLine is null)
                {
                    this.PushDocument(Space);
                }
                this.Visit(trailingNewLine);
            }, children => {
                // This logic ensures that syntax with only a single child is not doubly-nested,
                // and that the final newline does not cause the next piece of text to be indented
                // e.g. 'bar' in the following is only indented once, and '})' is not indented:
                //   foo({
                //     bar: 123
                //   })
                var hasTrailingNewline = trailingNewLine is not null;
                var nestedChildren = Concat(hasTrailingNewline ? children[..^1] : children);
                var newLine = hasTrailingNewline ? children[^1] : Nil;

                if (children.Length > 1)
                {
                    nestedChildren = nestedChildren.Nest();
                }

                return Concat(nestedChildren, newLine);
            });
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax) =>
            this.BuildWithConcat(() => {
                this.Visit(syntax.Name);
                this.Visit(syntax.OpenParen);
                this.VisitCommaAndNewLineSeparated(syntax.Children, leadingAndTrailingSpace: false);
                this.Visit(syntax.CloseParen);
            });

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax) =>
            this.BuildWithConcat(() => {
                this.Visit(syntax.BaseExpression);
                this.Visit(syntax.Dot);
                this.Visit(syntax.Name);
                this.Visit(syntax.OpenParen);
                this.VisitCommaAndNewLineSeparated(syntax.Children, leadingAndTrailingSpace: false);
                this.Visit(syntax.CloseParen);
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

            if (syntaxTrivia.Type == SyntaxTriviaType.DisableNextLineDiagnosticsDirective)
            {
                this.PushDocument(Text(syntaxTrivia.Text));
            }
        }

        public override void VisitObjectSyntax(ObjectSyntax syntax) =>
            this.BuildWithConcat(() => {
                this.Visit(syntax.OpenBrace);
                this.VisitCommaAndNewLineSeparated(syntax.Children, leadingAndTrailingSpace: true);
                this.Visit(syntax.CloseBrace);
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
            this.BuildWithConcat(() => {
                this.Visit(syntax.OpenBracket);
                this.VisitCommaAndNewLineSeparated(syntax.Children, leadingAndTrailingSpace: true);
                this.Visit(syntax.CloseBracket);
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
                this.documentStack.Push(document);
            }
        }
    }
}
