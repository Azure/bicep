// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Syntax;

namespace Bicep.Core.Parsing
{
    public class ParamsParser : BaseParser
    {
        public ParamsParser(string text) : base(text)
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
                            LanguageConstants.UsingKeyword => this.UsingDeclaration(),
                            LanguageConstants.ExtendsKeyword => this.ExtendsDeclaration(),
                            LanguageConstants.ParameterKeyword => this.ParameterAssignment(),
                            LanguageConstants.VariableKeyword => this.VariableDeclaration(leadingNodes),
                            LanguageConstants.ImportKeyword => this.CompileTimeImportDeclaration(ExpectKeyword(LanguageConstants.ImportKeyword), leadingNodes),
                            LanguageConstants.ExtensionKeyword => this.ExtensionConfigAssignment(leadingNodes),
                            LanguageConstants.TypeKeyword => this.TypeDeclaration(leadingNodes),
                            _ => throw new ExpectedTokenException(current, b => b.UnrecognizedParamsFileDeclaration(supportsTypeDeclarations: true)),
                        },
                        TokenType.NewLine => this.NewLine(),
                        _ => throw new ExpectedTokenException(current, b => b.UnrecognizedParamsFileDeclaration(supportsTypeDeclarations: true)),
                    };

                    string? ValidateKeyword(string keyword) =>
                        expectedKeywords.Length == 0 || expectedKeywords.Contains(keyword) ? keyword : null;
                },
                RecoveryFlags.None,
                TokenType.NewLine);

        private UsingDeclarationSyntax UsingDeclaration()
        {
            var keyword = ExpectKeyword(LanguageConstants.UsingKeyword);

            SyntaxBase expression = reader.Peek().Type switch
            {
                TokenType.Identifier => new NoneLiteralSyntax(ExpectKeyword(LanguageConstants.NoneKeyword)),
                TokenType.StringComplete => ThrowIfSkipped(this.InterpolableString, b => b.ExpectedFilePathString()),
                _ => SkipEmpty(b => b.ExpectedSymbolListOrWildcard()),
            };

            return new(keyword, expression);
        }

        private ExtendsDeclarationSyntax ExtendsDeclaration()
        {
            var keyword = ExpectKeyword(LanguageConstants.ExtendsKeyword);
            var path = this.WithRecovery(
                () => ThrowIfSkipped(this.InterpolableString, b => b.ExtendsPathHasNotBeenSpecified()),
                GetSuppressionFlag(keyword),
                TokenType.NewLine);

            return new ExtendsDeclarationSyntax(keyword, path);
        }

        private SyntaxBase ParameterAssignment()
        {
            var keyword = ExpectKeyword(LanguageConstants.ParameterKeyword);
            var name = this.IdentifierWithRecovery(b => b.ExpectedParameterIdentifier(), RecoveryFlags.None, TokenType.Identifier, TokenType.NewLine);
            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(name), TokenType.NewLine);
            var value = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), GetSuppressionFlag(assignment), TokenType.NewLine);

            return new ParameterAssignmentSyntax(keyword, name, assignment, value);
        }

        private ExtensionConfigAssignmentSyntax ExtensionConfigAssignment(IEnumerable<SyntaxBase> leadingNodes)
        {
            var extKeyword = ExpectKeyword(LanguageConstants.ExtensionKeyword);

            var specificationSyntax = reader.Peek().Type switch
            {
                TokenType.Identifier => new IdentifierSyntax(reader.Read()),
                _ => this.WithRecovery(
                    () => ThrowIfSkipped(this.InterpolableString, b => b.ExpectedExtensionSpecification()),
                    RecoveryFlags.None,
                    TokenType.NewLine)
            };

            var withClause = this.WithRecovery(this.ExtensionWithClause, GetSuppressionFlag(specificationSyntax), TokenType.NewLine);

            return new(leadingNodes, extKeyword, specificationSyntax, withClause);
        }

        private ExtensionWithClauseSyntax ExtensionWithClause()
        {
            var keyword = this.ExpectKeyword(LanguageConstants.WithKeyword);
            var config = this.WithRecovery(() => this.Object(ExpressionFlags.AllowComplexLiterals), RecoveryFlags.None, TokenType.NewLine);

            return new(keyword, config);
        }
    }
}
