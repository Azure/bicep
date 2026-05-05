// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrint.Documents;
using Bicep.Core.Syntax;

namespace Bicep.Core.PrettyPrint
{
    public class DocumentBuildVisitor : CstVisitor
    {
        private static readonly ILinkedDocument Nil = new NilDocument();

        private static readonly ILinkedDocument Space = new TextDocument(" ");

        private static readonly ILinkedDocument Line = new NestDocument(0);

        private static readonly ILinkedDocument NoLine = new NilDocument();

        private static readonly ILinkedDocument SingleLine = new NestDocument(0);

        private static readonly ILinkedDocument DoubleLine = new NestDocument(0).Concat(Line);

        private static readonly ImmutableDictionary<string, TextDocument> CommonTextCache =
            LanguageConstants.ContextualKeywords
            .Concat(LanguageConstants.NonContextualKeywords.Keys)
            .Concat(["(", ")", "[", "]", "{", "}", "=", ":", "+", "-", "*", "/", "!"])
            .Concat(["name", "properties", "string", "bool", "int", "array", "object"])
            .ToImmutableDictionary(value => value, value => new TextDocument(value));

        private readonly Stack<ILinkedDocument> documentStack = new();

        private readonly IDiagnosticLookup lexingErrorLookup;

        private readonly IDiagnosticLookup parsingErrorLookup;

        private bool visitingSkippedTriviaSyntax;

        private bool visitingBrokenStatement;

        private bool visitingComment;

        private bool visitingLeadingTrivia;

        private ILinkedDocument? LeadingDirectiveOrComments = null;

        public DocumentBuildVisitor()
            : this(EmptyDiagnosticLookup.Instance, EmptyDiagnosticLookup.Instance)
        {
        }

        public DocumentBuildVisitor(IDiagnosticLookup lexingErrorLookup, IDiagnosticLookup parsingErrorLookup)
        {
            this.lexingErrorLookup = lexingErrorLookup;
            this.parsingErrorLookup = parsingErrorLookup;
        }

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

                if (!syntax.EndOfFile.LeadingTrivia.Any(x => x.Type == SyntaxTriviaType.DiagnosticsPragma))
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

        public override void VisitExtensionDeclarationSyntax(ExtensionDeclarationSyntax syntax) =>
            this.BuildStatement(syntax, () =>
            {
                this.VisitNodes(syntax.LeadingNodes);
                this.Visit(syntax.Keyword);
                this.documentStack.Push(Nil);
                this.Visit(syntax.SpecificationString);
                this.Visit(syntax.WithClause);
                this.Visit(syntax.AsClause);
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

        public override void VisitFunctionDeclarationSyntax(FunctionDeclarationSyntax syntax) =>
            this.BuildStatement(syntax, () =>
            {
                this.VisitNodes(syntax.LeadingNodes);
                this.Visit(syntax.Keyword);
                this.Visit(syntax.Name);
                this.Visit(syntax.Lambda);
            }, children =>
            {
                var leadingNodes = children[0..^3];
                var keyword = children[^3];
                var name = children[^2];
                var lambda = children[^1];

                return Spread(
                    Concat(leadingNodes.Concat(keyword)),
                    Concat(name, lambda));
            });

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax) =>
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

        public override void VisitTestDeclarationSyntax(TestDeclarationSyntax syntax) =>
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

        public override void VisitAssertDeclarationSyntax(AssertDeclarationSyntax syntax) =>
            this.BuildStatement(syntax, () =>
            {
                this.VisitNodes(syntax.LeadingNodes);
                this.Visit(syntax.Keyword);
                this.documentStack.Push(Nil);
                this.Visit(syntax.Name);
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
                this.Visit(syntax.WithClause);
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

        public override void VisitForSyntax(ForSyntax syntax)
        {
            this.Build(
            () =>
            {
                this.Visit(syntax.OpenSquare);
                this.VisitNodes(syntax.OpenNewlines);
                this.documentStack.Push(Nil);
                this.Visit(syntax.ForKeyword);
                this.Visit(syntax.VariableSection);
                this.Visit(syntax.InKeyword);
                this.Visit(syntax.Expression);
                this.Visit(syntax.Colon);
                this.Visit(syntax.Body);
                this.documentStack.Push(Nil);
                this.VisitNodes(syntax.CloseNewlines);
                this.Visit(syntax.CloseSquare);
            },
            children =>
            {
                var firstNilIndex = Array.IndexOf(children, Nil);
                var lastNilIndex = Array.LastIndexOf(children, Nil);

                ILinkedDocument openBracket = children[0];
                var openNewlines = children[1..firstNilIndex];

                var closeNewlines = children[(lastNilIndex + 1)..^1];
                ILinkedDocument closeBracket = children[^1];

                var bracketEnclosed = children[(firstNilIndex + 1)..lastNilIndex];
                ILinkedDocument forKeyword = bracketEnclosed[0];
                ILinkedDocument variableBlock = bracketEnclosed[1];
                ILinkedDocument inKeyword = bracketEnclosed[2];
                ILinkedDocument arrayExpression = bracketEnclosed[3];
                ILinkedDocument colon = bracketEnclosed[4];
                ILinkedDocument loopBody = bracketEnclosed[5];

                var documentsToConcat = new List<ILinkedDocument> { openBracket };
                documentsToConcat.AddRange(openNewlines);
                documentsToConcat.Add(Spread(forKeyword, variableBlock, inKeyword, arrayExpression));
                documentsToConcat.Add(Spread(colon, loopBody));
                documentsToConcat.AddRange(closeNewlines);
                documentsToConcat.Add(closeBracket);

                return Concat(documentsToConcat);
            });
        }

        public override void VisitVariableBlockSyntax(VariableBlockSyntax syntax) =>
            this.BuildWithConcat(() =>
            {
                this.Visit(syntax.OpenParen);
                this.VisitCommaAndNewLineSeparated(syntax.Children, leadingAndTrailingSpace: false);
                this.Visit(syntax.CloseParen);
            });

        public override void VisitTypedVariableBlockSyntax(TypedVariableBlockSyntax syntax) =>
            this.BuildWithConcat(() =>
            {
                this.Visit(syntax.OpenParen);
                this.VisitCommaAndNewLineSeparated(syntax.Children, leadingAndTrailingSpace: false);
                this.Visit(syntax.CloseParen);
            });

        public override void VisitTypedLocalVariableSyntax(TypedLocalVariableSyntax syntax) =>
            this.BuildWithSpread(() => base.VisitTypedLocalVariableSyntax(syntax));

        public override void VisitLambdaSyntax(LambdaSyntax syntax) =>
            this.BuildWithSpread(() => base.VisitLambdaSyntax(syntax));

        public override void VisitTypedLambdaSyntax(TypedLambdaSyntax syntax) =>
            this.BuildWithSpread(() => base.VisitTypedLambdaSyntax(syntax));

        private void VisitCommaAndNewLineSeparated(ImmutableArray<SyntaxBase> nodes, bool leadingAndTrailingSpace)
        {
            if (nodes.Length == 1 && nodes[0] is Token { Type: TokenType.NewLine })
            {
                this.Build(() => this.Visit(nodes[0]), children =>
                {
                    if (children.Length == 1)
                    {
                        if (children[0] == Line || children[0] == SingleLine || children[0] == DoubleLine)
                        {
                            return Nil;
                        }

                        // Trailing comment.
                        return children[0];
                    }

                    return new NestDocument(1, [.. children]);
                });
                return;
            }

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

            this.Build(() =>
            {
                this.Visit(leadingNewLine);
                if (leadingAndTrailingSpace && nodes.Any() && leadingNewLine is null)
                {
                    this.PushDocument(Space);
                }

                for (var i = 0; i < nodes.Length; i++)
                {
                    var visitingBrokenStatement = this.visitingBrokenStatement;

                    if (this.HasSyntaxError(nodes[i]))
                    {
                        this.visitingBrokenStatement = true;
                    }

                    this.Visit(nodes[i]);

                    if (!this.visitingBrokenStatement)
                    {
                        if (i < nodes.Length - 1 &&
                            nodes[i] is Token { Type: TokenType.Comma } &&
                            nodes[i + 1] is not Token { Type: TokenType.NewLine })
                        {
                            this.PushDocument(Space);
                        }
                    }

                    this.visitingBrokenStatement = visitingBrokenStatement;
                }

                if (leadingAndTrailingSpace && nodes.Any() && trailingNewLine is null)
                {
                    this.PushDocument(Space);
                }
                this.Visit(trailingNewLine);
            }, children =>
            {
                // This logic ensures that syntax with only a single child is not doubly-nested,
                // and that the final newline does not cause the next piece of text to be indented
                // e.g. 'bar' in the following is only indented once, and '})' is not indented:
                //   foo({
                //     bar: 123
                //   })
                var hasTrailingNewline = trailingNewLine is not null;
                var nestedChildren = Concat(hasTrailingNewline ? children[..^1] : children);
                var newLine = hasTrailingNewline ? children[^1] : Nil;

                // Do not call Nest() if we are inside a broken statement, otherwise it will keep on indenting further when the user formats document.
                if (!this.visitingBrokenStatement && children.Length > 1)
                {
                    nestedChildren = nestedChildren.Nest();
                }

                return Concat(nestedChildren, newLine);
            });
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax) =>
            this.BuildWithConcat(() =>
            {
                this.Visit(syntax.Name);
                this.Visit(syntax.OpenParen);
                this.VisitCommaAndNewLineSeparated(syntax.Children, leadingAndTrailingSpace: false);
                this.Visit(syntax.CloseParen);
            });

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax) =>
            this.BuildWithConcat(() =>
            {
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
                this.visitingLeadingTrivia = true;
                this.VisitSyntaxTrivia(trivia);
                this.visitingLeadingTrivia = false;
            }

            var pushDocument = this.visitingBrokenStatement
                ? (Action<ILinkedDocument>)this.documentStack.Push
                : (Action<ILinkedDocument>)this.PushDocument;

            if (token.Type == TokenType.NewLine)
            {
                if (this.LeadingDirectiveOrComments is not null)
                {
                    pushDocument(this.LeadingDirectiveOrComments);
                }

                int newlineCount = StringUtils.CountNewlines(token.Text);

                for (int i = 0; i < newlineCount; i++)
                {
                    pushDocument(Line);
                }
            }
            else if (token.Type == TokenType.EndOfFile)
            {
                if (this.LeadingDirectiveOrComments is not null)
                {
                    pushDocument(SingleLine);
                    pushDocument(this.LeadingDirectiveOrComments);
                }
            }
            else
            {
                if (this.LeadingDirectiveOrComments is not null)
                {
                    pushDocument(Concat(this.LeadingDirectiveOrComments, Space, Text(token.Text)));
                }
                else
                {
                    pushDocument(Text(token.Text));
                }
            }

            this.LeadingDirectiveOrComments = null;

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

            if (syntaxTrivia.Type == SyntaxTriviaType.DiagnosticsPragma)
            {
                if (this.LeadingDirectiveOrComments is null)
                {
                    this.LeadingDirectiveOrComments = Text(syntaxTrivia.Text);
                }
                else
                {
                    this.LeadingDirectiveOrComments = Concat(this.LeadingDirectiveOrComments, Space, Text(syntaxTrivia.Text));
                }
            }
        }

        public override void VisitObjectSyntax(ObjectSyntax syntax) =>
            this.BuildWithConcat(() =>
            {
                this.Visit(syntax.OpenBrace);
                this.VisitCommaAndNewLineSeparated(syntax.Children, leadingAndTrailingSpace: true);
                this.Visit(syntax.CloseBrace);
            });

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax) =>
            this.Build(() => base.VisitObjectPropertySyntax(syntax), children =>
            {
                // When a property value is an unterminated string, there can be more than
                // 3 children.
                Debug.Assert(children.Length >= 3);

                ILinkedDocument key = children[0];
                ILinkedDocument colon = children[1];
                ILinkedDocument value = children[2];

                return Concat(key, colon, Space, value);
            });

        public override void VisitArraySyntax(ArraySyntax syntax) =>
            this.BuildWithConcat(() =>
            {
                this.Visit(syntax.OpenBracket);
                this.VisitCommaAndNewLineSeparated(syntax.Children, leadingAndTrailingSpace: true);
                this.Visit(syntax.CloseBracket);
            });

        public override void VisitTypeDeclarationSyntax(TypeDeclarationSyntax syntax) =>
            this.BuildStatement(syntax, () =>
            {
                this.VisitNodes(syntax.LeadingNodes);
                this.Visit(syntax.Keyword);
                this.documentStack.Push(Nil);
                this.Visit(syntax.Name);
                this.Visit(syntax.Assignment);
                this.Visit(syntax.Value);
            });

        public override void VisitArrayTypeSyntax(ArrayTypeSyntax syntax) =>
            this.BuildWithConcat(() =>
            {
                this.Visit(syntax.Item);
                this.Visit(syntax.OpenBracket);
                this.Visit(syntax.CloseBracket);
            });

        public override void VisitObjectTypeSyntax(ObjectTypeSyntax syntax) =>
            this.BuildWithConcat(() =>
            {
                this.Visit(syntax.OpenBrace);
                this.VisitCommaAndNewLineSeparated(syntax.Children, leadingAndTrailingSpace: true);
                this.Visit(syntax.CloseBrace);
            });

        public override void VisitObjectTypePropertySyntax(ObjectTypePropertySyntax syntax) =>
            this.BuildWithConcat(() =>
            {
                this.VisitNodes(syntax.LeadingNodes);
                this.Visit(syntax.Key);
                this.Visit(syntax.Colon);
                this.documentStack.Push(Space);
                this.Visit(syntax.Value);
            });

        public override void VisitObjectTypeAdditionalPropertiesSyntax(ObjectTypeAdditionalPropertiesSyntax syntax) =>
            this.BuildWithConcat(() =>
            {
                this.VisitNodes(syntax.LeadingNodes);
                this.Visit(syntax.Asterisk);
                this.Visit(syntax.Colon);
                this.documentStack.Push(Space);
                this.Visit(syntax.Value);
            });

        public override void VisitTupleTypeSyntax(TupleTypeSyntax syntax) =>
            this.BuildWithConcat(() =>
            {
                this.Visit(syntax.OpenBracket);
                this.VisitCommaAndNewLineSeparated(syntax.Children, leadingAndTrailingSpace: true);
                this.Visit(syntax.CloseBracket);
            });

        public override void VisitTupleTypeItemSyntax(TupleTypeItemSyntax syntax) =>
            this.BuildWithConcat(() =>
            {
                this.VisitNodes(syntax.LeadingNodes);
                this.Visit(syntax.Value);
            });

        public override void VisitUnionTypeSyntax(UnionTypeSyntax syntax) =>
            this.BuildWithConcat(() =>
            {
                int stackTare = documentStack.Count;
                var firstLineWritten = false;

                void AggregateCurrentLine()
                {
                    LinkedList<ILinkedDocument> currentLineDocs = new();
                    while (documentStack.Count > stackTare)
                    {
                        currentLineDocs.AddFirst(documentStack.Pop());
                    }

                    var line = Spread(currentLineDocs);
                    this.PushDocument(firstLineWritten ? new NestDocument(1, [line]) : line);
                    firstLineWritten = true;
                    stackTare++;
                }

                for (int i = 0; i < syntax.Children.Length; i++)
                {
                    if (syntax.Children[i] is Token { Type: TokenType.NewLine })
                    {
                        AggregateCurrentLine();
                    }
                    else
                    {
                        this.Visit(syntax.Children[i]);
                    }
                }

                AggregateCurrentLine();
            });

        public override void VisitNonNullAssertionSyntax(NonNullAssertionSyntax syntax) =>
            this.BuildWithConcat(() =>
            {
                this.Visit(syntax.BaseExpression);
                this.Visit(syntax.AssertionOperator);
            });

        public override void VisitNullableTypeSyntax(NullableTypeSyntax syntax) =>
            this.BuildWithConcat(() =>
            {
                this.Visit(syntax.Base);
                this.Visit(syntax.NullabilityMarker);
            });

        public override void VisitCompileTimeImportDeclarationSyntax(CompileTimeImportDeclarationSyntax syntax) =>
            this.BuildStatement(syntax, () =>
            {
                this.VisitNodes(syntax.LeadingNodes);
                this.Visit(syntax.Keyword);
                this.documentStack.Push(Nil);
                this.Visit(syntax.ImportExpression);
                this.Visit(syntax.FromClause);
            });

        public override void VisitImportedSymbolsListSyntax(ImportedSymbolsListSyntax syntax) =>
            this.BuildWithConcat(() =>
            {
                this.Visit(syntax.OpenBrace);
                this.VisitCommaAndNewLineSeparated(syntax.Children, leadingAndTrailingSpace: true);
                this.Visit(syntax.CloseBrace);
            });

        public override void VisitImportedSymbolsListItemSyntax(ImportedSymbolsListItemSyntax syntax) =>
            this.BuildWithSpread(() =>
            {
                this.Visit(syntax.OriginalSymbolName);
                this.Visit(syntax.AsClause);
            });

        public override void VisitAliasAsClauseSyntax(AliasAsClauseSyntax syntax) =>
            this.BuildWithSpread(() =>
            {
                this.Visit(syntax.Keyword);
                this.Visit(syntax.Alias);
            });

        public override void VisitWildcardImportSyntax(WildcardImportSyntax syntax) =>
            this.BuildWithSpread(() =>
            {
                this.Visit(syntax.Wildcard);
                this.Visit(syntax.AliasAsClause);
            });

        public override void VisitCompileTimeImportFromClauseSyntax(CompileTimeImportFromClauseSyntax syntax) =>
            this.BuildWithSpread(() =>
            {
                this.Visit(syntax.Keyword);
                this.Visit(syntax.Path);
            });

        public override void VisitParameterizedTypeInstantiationSyntax(ParameterizedTypeInstantiationSyntax syntax) =>
            this.BuildWithConcat(() =>
            {
                this.Visit(syntax.Name);
                this.Visit(syntax.OpenChevron);
                this.VisitCommaAndNewLineSeparated(syntax.Children, leadingAndTrailingSpace: false);
                this.Visit(syntax.CloseChevron);
            });

        public override void VisitInstanceParameterizedTypeInstantiationSyntax(InstanceParameterizedTypeInstantiationSyntax syntax) =>
            this.BuildWithConcat(() =>
            {
                this.Visit(syntax.BaseExpression);
                this.Visit(syntax.Dot);
                this.Visit(syntax.Name);
                this.Visit(syntax.OpenChevron);
                this.VisitCommaAndNewLineSeparated(syntax.Children, leadingAndTrailingSpace: false);
                this.Visit(syntax.CloseChevron);
            });

        public override void VisitTypePropertyAccessSyntax(TypePropertyAccessSyntax syntax) =>
            this.BuildWithConcat(() => base.VisitTypePropertyAccessSyntax(syntax));

        public override void VisitTypeAdditionalPropertiesAccessSyntax(TypeAdditionalPropertiesAccessSyntax syntax) =>
            this.BuildWithConcat(() => base.VisitTypeAdditionalPropertiesAccessSyntax(syntax));

        public override void VisitTypeArrayAccessSyntax(TypeArrayAccessSyntax syntax)
            => this.BuildWithConcat(() => base.VisitTypeArrayAccessSyntax(syntax));

        public override void VisitTypeItemsAccessSyntax(TypeItemsAccessSyntax syntax)
            => this.BuildWithConcat(() => base.VisitTypeItemsAccessSyntax(syntax));

        private static ILinkedDocument Text(string text) =>
            CommonTextCache.TryGetValue(text, out var cached) ? cached : new TextDocument(text);

        private static ILinkedDocument Concat(params ILinkedDocument[] combinators) =>
            Concat(combinators as IEnumerable<ILinkedDocument>);

        private static ILinkedDocument Concat(IEnumerable<ILinkedDocument> combinators) =>
            combinators.Aggregate(Nil, (a, b) => a.Concat(b));

        private static ILinkedDocument Spread(params ILinkedDocument[] combinators) =>
            Spread(combinators as IEnumerable<ILinkedDocument>);

        private static ILinkedDocument Spread(IEnumerable<ILinkedDocument> combinators) =>
            combinators.DefaultIfEmpty(Nil).Aggregate((a, b) => a.Concat(Space).Concat(b));

        private void BuildWithConcat(Action visitAciton) => this.Build(visitAciton, Concat);

        private void BuildWithSpread(Action visitAciton) => this.Build(visitAciton, Spread);

        private void BuildStatement(SyntaxBase syntax, Action visitAction, Func<ILinkedDocument[], ILinkedDocument> buildFunc)
        {
            if (this.HasSyntaxError(syntax))
            {
                this.visitingBrokenStatement = true;
                visitAction();
                this.visitingBrokenStatement = false;

                // Everything left on the stack will be concatenated by the top level Concat rule defined in VisitProgram.
                return;
            }

            this.Build(visitAction, buildFunc);
        }

        private void BuildStatement(SyntaxBase syntax, Action visitAction)
            => BuildStatement(syntax, visitAction, children =>
            {
                var splitIndex = Array.IndexOf(children, Nil);

                // Need to concat leading decorators and the statement keyword.
                var head = Concat(children.Take(splitIndex));
                var tail = children.Skip(splitIndex + 1);

                return Spread(head.AsEnumerable().Concat(tail));
            });

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
            else if (this.visitingComment && this.visitingLeadingTrivia)
            {
                if (this.LeadingDirectiveOrComments is null)
                {
                    this.LeadingDirectiveOrComments = document;
                }
                else
                {
                    this.LeadingDirectiveOrComments = Concat(this.LeadingDirectiveOrComments, Space, document);
                }
            }
            else if (visitingComment)
            {
                // Add a space before the comment if it's not at the beginning of the file or after a newline.
                ILinkedDocument gap = top != NoLine && top != Line && top != SingleLine && top != DoubleLine ? Space : Nil;

                // Combine the comment and the document at the top of the stack. This is the key to simplify VisitToken.

                this.documentStack.Push(Concat(this.documentStack.Pop(), gap, document));
            }
            else
            {
                this.documentStack.Push(document);
            }
        }

        private bool HasSyntaxError(SyntaxBase syntax) =>
            this.lexingErrorLookup.Contains(syntax) ||
            this.parsingErrorLookup.Contains(syntax);
    }
}
