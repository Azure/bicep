// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;

namespace Bicep.Core.Parsing
{
    public class Parser : BaseParser
    {
        public Parser(string text) : base(text)
        {
        }

        public override ProgramSyntax Program()
        {
            var declarationsOrTokens = new List<SyntaxBase>();

            while (!this.IsAtEnd())
            {
                // this produces either a declaration node, skipped tokens node or just a token
                var declarationOrToken = Declaration();
                declarationsOrTokens.Add(declarationOrToken);

                // if skipped node is returned above, the newline is not consumed
                // if newline token is returned, we must not expect another (could be a beginning of a declaration)
                if (declarationOrToken is ITopLevelDeclarationSyntax)
                {
                    // declarations must be followed by a newline or the file must end
                    var newLine = this.WithRecoveryNullable(this.NewLineOrEof, RecoveryFlags.ConsumeTerminator, TokenType.NewLine);
                    if (newLine != null)
                    {
                        declarationsOrTokens.Add(newLine);
                    }
                }
            }

            var endOfFile = reader.Read();

            var programSyntax = new ProgramSyntax(declarationsOrTokens, endOfFile);

            var parsingErrorVisitor = new ParseDiagnosticsVisitor(this.ParsingErrorTree);
            parsingErrorVisitor.Visit(programSyntax);

            return programSyntax;
        }

        protected override SyntaxBase Declaration() =>
            this.WithRecovery(
                () =>
                {
                    var leadingNodes = DecorableSyntaxLeadingNodes().ToImmutableArray();

                    Token current = reader.Peek();

                    return current.Type switch
                    {
                        TokenType.Identifier => current.Text switch
                        {
                            LanguageConstants.TargetScopeKeyword => this.TargetScope(leadingNodes),
                            LanguageConstants.MetadataKeyword => this.MetadataDeclaration(leadingNodes),
                            LanguageConstants.TypeKeyword => this.TypeDeclaration(leadingNodes),
                            LanguageConstants.ParameterKeyword => this.ParameterDeclaration(leadingNodes),
                            LanguageConstants.VariableKeyword => this.VariableDeclaration(leadingNodes),
                            LanguageConstants.FunctionKeyword => this.FunctionDeclaration(leadingNodes),
                            LanguageConstants.ResourceKeyword => this.ResourceDeclaration(leadingNodes),
                            LanguageConstants.OutputKeyword => this.OutputDeclaration(leadingNodes),
                            LanguageConstants.ModuleKeyword => this.ModuleDeclaration(leadingNodes),
                            LanguageConstants.TestKeyword => this.TestDeclaration(leadingNodes),
                            LanguageConstants.ImportKeyword => this.ImportDeclaration(leadingNodes),
                            LanguageConstants.AssertKeyword => this.AssertDeclaration(leadingNodes),
                            _ => leadingNodes.Length > 0
                                ? new MissingDeclarationSyntax(leadingNodes)
                                : throw new ExpectedTokenException(current, b => b.UnrecognizedDeclaration()),
                        },
                        TokenType.NewLine => this.NewLine(),

                        _ => leadingNodes.Length > 0
                            ? new MissingDeclarationSyntax(leadingNodes)
                            : throw new ExpectedTokenException(current, b => b.UnrecognizedDeclaration()),
                    };
                },
                RecoveryFlags.None,
                TokenType.NewLine);

        private SyntaxBase TargetScope(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.TargetScopeKeyword);
            var assignment = this.WithRecovery(this.Assignment, RecoveryFlags.None, TokenType.NewLine);
            var value = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), RecoveryFlags.None, TokenType.NewLine);

            return new TargetScopeSyntax(leadingNodes, keyword, assignment, value);
        }

        private SyntaxBase MetadataDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.MetadataKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedMetadataIdentifier(), RecoveryFlags.None, TokenType.Assignment, TokenType.NewLine);
            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(name), TokenType.NewLine);
            var value = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), GetSuppressionFlag(assignment), TokenType.NewLine);

            return new MetadataDeclarationSyntax(leadingNodes, keyword, name, assignment, value);
        }

        private SyntaxBase TypeDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.TypeKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedTypeIdentifier(), RecoveryFlags.None, TokenType.Assignment, TokenType.NewLine);
            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(name), TokenType.NewLine);
            var value = this.WithRecovery(() => Type(allowOptionalResourceType: false), GetSuppressionFlag(name), TokenType.Assignment, TokenType.LeftBrace, TokenType.NewLine);

            return new TypeDeclarationSyntax(leadingNodes, keyword, name, assignment, value);
        }

        private SyntaxBase ParameterDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.ParameterKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedParameterIdentifier(), RecoveryFlags.None, TokenType.Identifier, TokenType.NewLine);
            var type = this.WithRecovery(() => Type(allowOptionalResourceType: false), GetSuppressionFlag(name), TokenType.Assignment, TokenType.LeftBrace, TokenType.NewLine);

            // TODO: Need a better way to choose the terminating token
            SyntaxBase? modifier = this.WithRecoveryNullable(
                () =>
                {
                    var current = reader.Peek();
                    return current.Type switch
                    {
                        // the parameter does not have a modifier
                        TokenType.NewLine => null,
                        TokenType.EndOfFile => null,

                        // default value is specified
                        TokenType.Assignment => this.ParameterDefaultValue(),

                        _ => throw new ExpectedTokenException(current, b => b.ExpectedParameterContinuation())
                    };
                },
                GetSuppressionFlag(type),
                TokenType.NewLine);

            return new ParameterDeclarationSyntax(leadingNodes, keyword, name, type, modifier);
        }

        private SyntaxBase ParameterDefaultValue()
        {
            var assignmentToken = this.Expect(TokenType.Assignment, b => b.ExpectedCharacter("="));
            SyntaxBase defaultValue = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), RecoveryFlags.None, TokenType.NewLine);

            return new ParameterDefaultValueSyntax(assignmentToken, defaultValue);
        }

        private SyntaxBase VariableDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.VariableKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedVariableIdentifier(), RecoveryFlags.None, TokenType.Assignment, TokenType.NewLine);
            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(name), TokenType.NewLine);
            var value = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), GetSuppressionFlag(assignment), TokenType.NewLine);

            return new VariableDeclarationSyntax(leadingNodes, keyword, name, assignment, value);
        }

        private SyntaxBase FunctionDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.FunctionKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedVariableIdentifier(), RecoveryFlags.None, TokenType.Assignment, TokenType.NewLine);
            var lambda = this.WithRecovery(() => this.TypedLambda(), GetSuppressionFlag(name), TokenType.NewLine);

            return new FunctionDeclarationSyntax(leadingNodes, keyword, name, lambda);
        }

        private SyntaxBase OutputDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.OutputKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedOutputIdentifier(), RecoveryFlags.None, TokenType.Identifier, TokenType.NewLine);
            var type = this.WithRecovery(() => Type(allowOptionalResourceType: true), GetSuppressionFlag(name), TokenType.Assignment, TokenType.NewLine);
            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(type), TokenType.NewLine);
            var value = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), GetSuppressionFlag(assignment), TokenType.NewLine);

            return new OutputDeclarationSyntax(leadingNodes, keyword, name, type, assignment, value);
        }

        private SyntaxBase ResourceDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.ResourceKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedResourceIdentifier(), RecoveryFlags.None, TokenType.StringComplete, TokenType.StringLeftPiece, TokenType.NewLine);

            // TODO: Unify StringSyntax with TypeSyntax
            var type = this.WithRecovery(
                () => ThrowIfSkipped(this.InterpolableString, b => b.ExpectedResourceTypeString()),
                GetSuppressionFlag(name),
                TokenType.Assignment, TokenType.NewLine);

            var existingKeyword = GetOptionalKeyword(LanguageConstants.ExistingKeyword);
            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(type), TokenType.LeftBrace, TokenType.NewLine);

            var newlines = !assignment.IsSkipped && reader.Peek(skipNewlines: true).IsKeyword(LanguageConstants.IfKeyword)
                ? this.NewLines().ToImmutableArray()
                : ImmutableArray<Token>.Empty;

            var value = this.WithRecovery(() =>
                {
                    var current = reader.Peek();
                    return current.Type switch
                    {
                        TokenType.Identifier when current.Text == LanguageConstants.IfKeyword => this.IfCondition(ExpressionFlags.AllowResourceDeclarations | ExpressionFlags.AllowComplexLiterals, insideForExpression: false),
                        TokenType.LeftBrace => this.Object(ExpressionFlags.AllowResourceDeclarations | ExpressionFlags.AllowComplexLiterals),
                        TokenType.LeftSquare => this.ForExpression(ExpressionFlags.AllowResourceDeclarations | ExpressionFlags.AllowComplexLiterals, isResourceOrModuleContext: true),
                        _ => throw new ExpectedTokenException(current, b => b.ExpectBodyStartOrIfOrLoopStart())
                    };
                },
                GetSuppressionFlag(assignment),
                TokenType.NewLine);

            return new ResourceDeclarationSyntax(leadingNodes, keyword, name, type, existingKeyword, assignment, newlines, value);
        }

        private SyntaxBase ModuleDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.ModuleKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedModuleIdentifier(), RecoveryFlags.None, TokenType.StringComplete, TokenType.StringLeftPiece, TokenType.NewLine);

            // TODO: Unify StringSyntax with TypeSyntax
            var path = this.WithRecovery(
                () => ThrowIfSkipped(this.InterpolableString, b => b.ExpectedModulePathString()),
                GetSuppressionFlag(name),
                TokenType.Assignment, TokenType.NewLine);

            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(path), TokenType.LeftBrace, TokenType.NewLine);
            var newlines = reader.Peek(skipNewlines: true).IsKeyword(LanguageConstants.IfKeyword)
                ? this.NewLines().ToImmutableArray()
                : ImmutableArray<Token>.Empty;

            var value = this.WithRecovery(() =>
                {
                    var current = reader.Peek();
                    return current.Type switch
                    {
                        TokenType.Identifier when current.Text == LanguageConstants.IfKeyword => this.IfCondition(ExpressionFlags.AllowComplexLiterals, insideForExpression: false),
                        TokenType.LeftBrace => this.Object(ExpressionFlags.AllowComplexLiterals),
                        TokenType.LeftSquare => this.ForExpression(ExpressionFlags.AllowComplexLiterals, isResourceOrModuleContext: true),
                        _ => throw new ExpectedTokenException(current, b => b.ExpectBodyStartOrIfOrLoopStart())
                    };
                },
                GetSuppressionFlag(assignment),
                TokenType.NewLine);

            return new ModuleDeclarationSyntax(leadingNodes, keyword, name, path, assignment, newlines, value);
        }
        private SyntaxBase TestDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.TestKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedTestIdentifier(), RecoveryFlags.None, TokenType.StringComplete, TokenType.StringLeftPiece, TokenType.NewLine);

            // TODO: Unify StringSyntax with TypeSyntax
            var path = this.WithRecovery(
                () => ThrowIfSkipped(this.InterpolableString, b => b.ExpectedTestPathString()),
                GetSuppressionFlag(name),
                TokenType.Assignment, TokenType.NewLine);

            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(path), TokenType.LeftBrace, TokenType.NewLine);

            var value = this.WithRecovery(() =>
                {
                    var current = reader.Peek();
                    return current.Type switch
                    {
                        TokenType.LeftBrace => this.Object(ExpressionFlags.AllowComplexLiterals),
                        _ => throw new ExpectedTokenException(current, b => b.ExpectedCharacter("{"))
                    };
                },
                GetSuppressionFlag(assignment),
                TokenType.NewLine);

            return new TestDeclarationSyntax(leadingNodes, keyword, name, path, assignment, value);
        }

        private SyntaxBase ImportDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.ImportKeyword);

            // Provider namespace imports will use the `provider` keyword soon, but in the meantime, the keyword is
            // shared between provider imports and type imports. If the next character is a '{' or '*', assume the
            // statement is a type import
            return reader.Peek().Type switch
            {
                TokenType.LeftBrace or TokenType.Asterisk => CompileTimeImportDeclaration(keyword, leadingNodes),
                _ => ProviderImportDeclaration(keyword, leadingNodes),
            };
        }

        private CompileTimeImportDeclarationSyntax CompileTimeImportDeclaration(Token keyword, IEnumerable<SyntaxBase> leadingNodes)
        {
            SyntaxBase importExpression = reader.Peek().Type switch
            {
                TokenType.EndOfFile or
                TokenType.NewLine or
                TokenType.Identifier => SkipEmpty(b => b.ExpectedSymbolListOrWildcard()),
                TokenType.LeftBrace => ImportedSymbolsList(),
                TokenType.Asterisk => WildcardImport(),
                _ => Skip(reader.Read(), b => b.ExpectedSymbolListOrWildcard()),
            };

            return new(leadingNodes,
                keyword,
                importExpression,
                WithRecovery(CompileTimeImportFromClause, GetSuppressionFlag(keyword), TokenType.NewLine));
        }

        private ImportedSymbolsListSyntax ImportedSymbolsList()
        {
            var openBrace = Expect(TokenType.LeftBrace, b => b.ExpectedCharacter("{"));

            var itemsOrTokens = HandleArrayOrObjectElements(
                closingTokenType: TokenType.RightBrace,
                parseChildElement: ImportedSymbolsListItem);

            var closeBrace = Expect(TokenType.RightBrace, b => b.ExpectedCharacter("}"));

            return new(openBrace, itemsOrTokens, closeBrace);
        }

        private ImportedSymbolsListItemSyntax ImportedSymbolsListItem()
            => new(Identifier(b => b.ExpectedExportedSymbolName()), ImportedSymbolsListItemAsClause());

        private AliasAsClauseSyntax? ImportedSymbolsListItemAsClause() => Check(reader.Peek(), TokenType.AsKeyword)
            ? new(Expect(TokenType.AsKeyword, b => b.ExpectedKeyword(LanguageConstants.AsKeyword)),
                IdentifierWithRecovery(b => b.ExpectedTypeIdentifier(), RecoveryFlags.None, TokenType.Comma, TokenType.NewLine))
            : null;

        private WildcardImportSyntax WildcardImport() => new(Expect(TokenType.Asterisk, b => b.ExpectedCharacter("*")),
            new AliasAsClauseSyntax(Expect(TokenType.AsKeyword, b => b.ExpectedKeyword(LanguageConstants.AsKeyword)),
                Identifier(b => b.ExpectedNamespaceIdentifier())));

        private CompileTimeImportFromClauseSyntax CompileTimeImportFromClause()
        {
            var keyword = ExpectKeyword(LanguageConstants.FromKeyword);
            var path = WithRecovery(
                () => ThrowIfSkipped(InterpolableString, b => b.ExpectedModulePathString()),
                GetSuppressionFlag(keyword),
                TokenType.NewLine);

            return new(keyword, path);
        }

        private ProviderDeclarationSyntax ProviderImportDeclaration(Token keyword, IEnumerable<SyntaxBase> leadingNodes)
        {
            var providerSpecification = this.WithRecovery(
                () => ThrowIfSkipped(this.InterpolableString, b => b.ExpectedProviderSpecificationOrCompileTimeImportExpression()),
                RecoveryFlags.None,
                TokenType.Assignment,
                TokenType.NewLine);

            var withClause = this.reader.Peek().Type switch
            {
                TokenType.EndOfFile or
                TokenType.NewLine or
                TokenType.AsKeyword => this.SkipEmpty(),

                _ => this.WithRecovery(() => this.ImportWithClause(), GetSuppressionFlag(providerSpecification), TokenType.NewLine),
            };

            var asClause = this.reader.Peek().Type switch
            {
                TokenType.EndOfFile or
                TokenType.NewLine => this.SkipEmpty(),

                _ => this.WithRecovery(() => this.ImportAsClause(), GetSuppressionFlag(withClause), TokenType.NewLine),
            };

            return new(leadingNodes, keyword, providerSpecification, withClause, asClause);
        }

        private ProviderWithClauseSyntax ImportWithClause()
        {
            var keyword = this.Expect(TokenType.WithKeyword, b => b.ExpectedWithOrAsKeywordOrNewLine());
            var config = this.WithRecovery(() => this.Object(ExpressionFlags.AllowComplexLiterals), RecoveryFlags.None, TokenType.AsKeyword, TokenType.NewLine);

            return new(keyword, config);
        }

        private AliasAsClauseSyntax ImportAsClause()
        {
            var keyword = this.Expect(TokenType.AsKeyword, b => b.ExpectedKeyword(LanguageConstants.AsKeyword));
            var modifier = this.IdentifierWithRecovery(b => b.ExpectedImportAliasName(), RecoveryFlags.None, TokenType.NewLine);

            return new(keyword, modifier);
        }

        private SyntaxBase AssertDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.AssertKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedAssertIdentifier(), RecoveryFlags.None, TokenType.Assignment, TokenType.NewLine);
            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(name), TokenType.NewLine);
            var value = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), GetSuppressionFlag(assignment), TokenType.NewLine);

            return new AssertDeclarationSyntax(leadingNodes, keyword, name, assignment, value);
        }
    }
}
