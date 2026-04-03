// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;

using SyntaxResult = Bicep.Core.Utils.Result<Bicep.Core.Syntax.SyntaxBase, Bicep.Core.Diagnostics.Diagnostic>;

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
                            LanguageConstants.ImportKeyword => this.CompileTimeImportDeclaration(reader.Read(), leadingNodes).Transform(x => (SyntaxBase)x),
                            LanguageConstants.ExtensionConfigKeyword => this.ExtensionConfigAssignment(leadingNodes),
                            LanguageConstants.ExtensionKeyword => this.ExtensionDeclaration(reader.Read(), leadingNodes),
                            LanguageConstants.TypeKeyword => this.TypeDeclaration(leadingNodes),
                            _ => Fail<SyntaxBase>(current, b => b.UnrecognizedParamsFileDeclaration(featureProvider.ModuleExtensionConfigsEnabled)),
                        },
                        TokenType.NewLine => this.NewLine().Transform(x => (SyntaxBase)x),
                        _ => Fail<SyntaxBase>(current, b => b.UnrecognizedParamsFileDeclaration(featureProvider.ModuleExtensionConfigsEnabled)),
                    };

                    string? ValidateKeyword(string keyword) =>
                        expectedKeywords.Length == 0 || expectedKeywords.Contains(keyword) ? keyword : null;
                },
                RecoveryFlags.None,
                TokenType.NewLine);

        private SyntaxResult UsingDeclaration(ImmutableArray<SyntaxBase> leadingNodes)
        {
            if (!ExpectKeyword(LanguageConstants.UsingKeyword).IsSuccess(out var keyword, out var err))
            {
                return SyntaxResult.Failure(err);
            }

            SyntaxBase expression = reader.Peek().Type switch
            {
                TokenType.Identifier => new NoneLiteralSyntax(reader.Read()), // 'none' keyword - pre-checked
                TokenType.StringComplete => this.WithRecovery(
                    () => FailIfSkipped(this.InterpolableString, b => b.ExpectedFilePathString()),
                    RecoveryFlags.None,
                    TokenType.NewLine),
                _ => SkipEmpty(b => b.ExpectedSymbolListOrWildcard()),
            };

            var current = this.reader.Peek();
            var withClause = current.Type switch
            {
                TokenType.EndOfFile or
                TokenType.NewLine => this.SkipEmpty(),
                _ => this.WithRecovery(this.UsingWithClause, GetSuppressionFlag(expression), TokenType.NewLine),
            };

            return SyntaxResult.Success(new UsingDeclarationSyntax(leadingNodes, keyword, expression, withClause));
        }

        private SyntaxResult ExtendsDeclaration(ImmutableArray<SyntaxBase> leadingNodes)
        {
            if (!ExpectKeyword(LanguageConstants.ExtendsKeyword).IsSuccess(out var keyword, out var err))
            {
                return SyntaxResult.Failure(err);
            }

            var path = this.WithRecovery(
                () => FailIfSkipped(this.InterpolableString, b => b.ExtendsPathHasNotBeenSpecified()),
                GetSuppressionFlag(keyword),
                TokenType.NewLine);

            return SyntaxResult.Success(new ExtendsDeclarationSyntax(leadingNodes, keyword, path));
        }

        private SyntaxResult ParameterAssignment(ImmutableArray<SyntaxBase> leadingNodes)
        {
            if (!ExpectKeyword(LanguageConstants.ParameterKeyword).IsSuccess(out var keyword, out var err))
            {
                return SyntaxResult.Failure(err);
            }

            var name = this.IdentifierWithRecovery(b => b.ExpectedParameterIdentifier(), RecoveryFlags.None, TokenType.Identifier, TokenType.NewLine);
            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(name), TokenType.NewLine);
            var value = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), GetSuppressionFlag(assignment), TokenType.NewLine);

            return SyntaxResult.Success(new ParameterAssignmentSyntax(leadingNodes, keyword, name, assignment, value));
        }

        private SyntaxResult ExtensionConfigAssignment(IEnumerable<SyntaxBase> leadingNodes)
        {
            if (!ExpectKeyword(LanguageConstants.ExtensionConfigKeyword).IsSuccess(out var extKeyword, out var err))
            {
                return SyntaxResult.Failure(err);
            }

            var aliasSyntax = this.IdentifierWithRecovery(b => b.ExpectedExtensionAliasName(), RecoveryFlags.None, TokenType.Identifier, TokenType.NewLine);
            var withClause = this.WithRecovery(this.ExtensionWithClause, GetSuppressionFlag(aliasSyntax), TokenType.NewLine);

            return SyntaxResult.Success(new ExtensionConfigAssignmentSyntax(leadingNodes, extKeyword, aliasSyntax, withClause));
        }

        private Result<ExtensionWithClauseSyntax, Diagnostic> ExtensionWithClause()
        {
            if (!ExpectKeyword(LanguageConstants.WithKeyword).IsSuccess(out var keyword, out var err))
            {
                return Result<ExtensionWithClauseSyntax, Diagnostic>.Failure(err);
            }

            var config = this.WithRecovery(() => this.Object(ExpressionFlags.AllowComplexLiterals), RecoveryFlags.None, TokenType.NewLine);

            return Result<ExtensionWithClauseSyntax, Diagnostic>.Success(new ExtensionWithClauseSyntax(keyword, config));
        }

        private Result<UsingWithClauseSyntax, Diagnostic> UsingWithClause()
        {
            if (!ExpectKeyword(LanguageConstants.WithKeyword, b => b.ExpectedWithKeywordOrNewLine()).IsSuccess(out var keyword, out var err))
            {
                return Result<UsingWithClauseSyntax, Diagnostic>.Failure(err);
            }

            var config = this.WithRecovery(() => this.Object(ExpressionFlags.AllowComplexLiterals), RecoveryFlags.None, TokenType.NewLine);

            return Result<UsingWithClauseSyntax, Diagnostic>.Success(new UsingWithClauseSyntax(keyword, config));
        }
    }
}
