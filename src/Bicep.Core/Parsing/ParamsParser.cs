// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Features;
using Bicep.Core.Syntax;

namespace Bicep.Core.Parsing
{
    public class ParamsParser : BaseParser
    {
        private readonly IFeatureProvider featureProvider;

        public ParamsParser(string text, IFeatureProvider featureProvider) : base(text)
        {
            this.featureProvider = featureProvider;
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
                if (declarationOrToken is StatementSyntax)
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
                            LanguageConstants.UsingKeyword => this.UsingDeclaration(leadingNodes),
                            LanguageConstants.ExtendsKeyword => this.ExtendsDeclaration(leadingNodes),
                            LanguageConstants.ParameterKeyword => this.ParameterAssignment(leadingNodes),
                            LanguageConstants.VariableKeyword => this.VariableDeclaration(leadingNodes),
                            LanguageConstants.ImportKeyword => this.CompileTimeImportDeclaration(ExpectKeyword(LanguageConstants.ImportKeyword), leadingNodes),
                            LanguageConstants.ExtensionConfigKeyword => this.ExtensionConfigAssignment(leadingNodes),
                            LanguageConstants.TypeKeyword => this.TypeDeclaration(leadingNodes),
                            _ => throw new ExpectedTokenException(current, b => b.UnrecognizedParamsFileDeclaration(featureProvider.ModuleExtensionConfigsEnabled)),
                        },
                        TokenType.NewLine => this.NewLine(),
                        _ => throw new ExpectedTokenException(current, b => b.UnrecognizedParamsFileDeclaration(featureProvider.ModuleExtensionConfigsEnabled)),
                    };

                    string? ValidateKeyword(string keyword) =>
                        expectedKeywords.Length == 0 || expectedKeywords.Contains(keyword) ? keyword : null;
                },
                RecoveryFlags.None,
                TokenType.NewLine);

        private UsingDeclarationSyntax UsingDeclaration(ImmutableArray<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.UsingKeyword);

            SyntaxBase expression = reader.Peek().Type switch
            {
                TokenType.Identifier => new NoneLiteralSyntax(ExpectKeyword(LanguageConstants.NoneKeyword)),
                TokenType.StringComplete => ThrowIfSkipped(this.InterpolableString, b => b.ExpectedFilePathString()),
                _ => SkipEmpty(b => b.ExpectedSymbolListOrWildcard()),
            };

            var current = this.reader.Peek();
            var withClause = current.Type switch
            {
                TokenType.EndOfFile or
                TokenType.NewLine => this.SkipEmpty(),
                _ => this.WithRecovery(this.UsingWithClause, GetSuppressionFlag(expression), TokenType.NewLine),
            };

            return new(leadingNodes, keyword, expression, withClause);
        }

        private ExtendsDeclarationSyntax ExtendsDeclaration(ImmutableArray<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.ExtendsKeyword);
            var path = this.WithRecovery(
                () => ThrowIfSkipped(this.InterpolableString, b => b.ExtendsPathHasNotBeenSpecified()),
                GetSuppressionFlag(keyword),
                TokenType.NewLine);

            return new ExtendsDeclarationSyntax(leadingNodes, keyword, path);
        }

        private SyntaxBase ParameterAssignment(ImmutableArray<SyntaxBase> leadingNodes)
        {
            var keyword = ExpectKeyword(LanguageConstants.ParameterKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedParameterIdentifier(), RecoveryFlags.None, TokenType.Identifier, TokenType.NewLine);
            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(name), TokenType.NewLine);
            var value = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), GetSuppressionFlag(assignment), TokenType.NewLine);

            return new ParameterAssignmentSyntax(leadingNodes, keyword, name, assignment, value);
        }

        private ExtensionConfigAssignmentSyntax ExtensionConfigAssignment(IEnumerable<SyntaxBase> leadingNodes)
        {
            var extKeyword = ExpectKeyword(LanguageConstants.ExtensionConfigKeyword);
            var aliasSyntax = this.IdentifierWithRecovery(b => b.ExpectedExtensionAliasName(), RecoveryFlags.None, TokenType.Identifier, TokenType.NewLine);
            var withClause = this.WithRecovery(this.ExtensionWithClause, GetSuppressionFlag(aliasSyntax), TokenType.NewLine);

            return new(leadingNodes, extKeyword, aliasSyntax, withClause);
        }

        private ExtensionWithClauseSyntax ExtensionWithClause()
        {
            var keyword = this.ExpectKeyword(LanguageConstants.WithKeyword);
            var config = this.WithRecovery(() => this.Object(ExpressionFlags.AllowComplexLiterals), RecoveryFlags.None, TokenType.NewLine);

            return new(keyword, config);
        }

        private UsingWithClauseSyntax UsingWithClause()
        {
            var keyword = this.ExpectKeyword(LanguageConstants.WithKeyword, b => b.ExpectedWithKeywordOrNewLine());
            var config = this.WithRecovery(() => this.Object(ExpressionFlags.AllowComplexLiterals), RecoveryFlags.None, TokenType.NewLine);

            return new(keyword, config);
        }
    }
}
