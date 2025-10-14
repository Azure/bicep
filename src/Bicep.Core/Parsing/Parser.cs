// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;

namespace Bicep.Core.Parsing
{
    public class Parser(string text) : BaseParser(text)
    {
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

        protected override SyntaxBase Declaration(params string[] expectedKeywords) =>
            this.WithRecovery(
                () =>
                {
                    var leadingNodes = DecorableSyntaxLeadingNodes().ToImmutableArray();

                    var current = reader.Peek();

                    return current.Type switch
                    {
                        TokenType.Identifier => ValidateKeyword(current.Text) switch
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
                            LanguageConstants.ExtensionKeyword => this.ExtensionDeclaration(ExpectKeyword(current.Text), leadingNodes),
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

                    string? ValidateKeyword(string keyword) =>
                        expectedKeywords.Length == 0 || expectedKeywords.Contains(keyword) ? keyword : null;
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
                : [];

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
                : [];

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

            // Provider namespace declarations use the `provider` keyword as of Bicep 0.23, but to avoid breaking
            // extensibility users without warning, the `import` keyword is shared between provider declarations and
            // compile-time imports. If the token following the keyword is a string, assume the statement is a provider
            // declaration.
            // TODO(extensibility): Consider removing this
            return reader.Peek().Type switch
            {
                TokenType.StringLeftPiece or
                TokenType.StringComplete => ExtensionDeclaration(keyword, leadingNodes),
                _ => CompileTimeImportDeclaration(keyword, leadingNodes),
            };
        }

        private ExtensionDeclarationSyntax ExtensionDeclaration(Token keyword, IEnumerable<SyntaxBase> leadingNodes)
        {
            var specificationSyntax = reader.Peek().Type switch
            {
                TokenType.Identifier => new IdentifierSyntax(reader.Read()),

                _ => this.WithRecovery(
                    () => ThrowIfSkipped(this.InterpolableString, b => b.ExpectedExtensionSpecification()),
                    RecoveryFlags.None,
                    TokenType.NewLine)
            };

            var current = this.reader.Peek();
            var withClause = current.Type switch
            {
                TokenType.EndOfFile or
                TokenType.NewLine => this.SkipEmpty(),
                TokenType.Identifier when current.Text == LanguageConstants.AsKeyword => this.SkipEmpty(),

                _ => this.WithRecovery(() => this.ExtensionWithClause(), GetSuppressionFlag(specificationSyntax), TokenType.NewLine),
            };

            current = this.reader.Peek();
            var asClause = current.Type switch
            {
                TokenType.EndOfFile or
                TokenType.NewLine => this.SkipEmpty(),

                _ => this.WithRecovery(() => this.ExtensionAsClause(), GetSuppressionFlag(withClause), TokenType.NewLine),
            };

            return new(leadingNodes, keyword, specificationSyntax, withClause, asClause);
        }

        private ExtensionWithClauseSyntax ExtensionWithClause()
        {
            var keyword = this.ExpectKeyword(LanguageConstants.WithKeyword, b => b.ExpectedWithOrAsKeywordOrNewLine());
            var config = this.WithRecovery(() => this.Object(ExpressionFlags.AllowComplexLiterals), RecoveryFlags.None, TokenType.NewLine);

            return new(keyword, config);
        }

        private AliasAsClauseSyntax ExtensionAsClause()
        {
            var keyword = this.ExpectKeyword(LanguageConstants.AsKeyword, b => b.ExpectedWithOrAsKeywordOrNewLine());
            var modifier = this.IdentifierWithRecovery(b => b.ExpectedExtensionAliasName(), RecoveryFlags.None, TokenType.NewLine);

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
