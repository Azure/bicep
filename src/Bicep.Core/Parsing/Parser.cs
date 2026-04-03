// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;

using SyntaxResult = Bicep.Core.Utils.Result<Bicep.Core.Syntax.SyntaxBase, Bicep.Core.Diagnostics.Diagnostic>;

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
                            LanguageConstants.ExtensionKeyword => this.ExtensionDeclaration(reader.Read(), leadingNodes),
                            LanguageConstants.AssertKeyword => this.AssertDeclaration(leadingNodes),
                            _ => leadingNodes.Length > 0
                                ? SyntaxResult.Success(new MissingDeclarationSyntax(leadingNodes))
                                : Fail<SyntaxBase>(current, b => b.UnrecognizedDeclaration()),
                        },
                        TokenType.NewLine => this.NewLine().Transform(x => (SyntaxBase)x),

                        _ => leadingNodes.Length > 0
                            ? SyntaxResult.Success(new MissingDeclarationSyntax(leadingNodes))
                            : Fail<SyntaxBase>(current, b => b.UnrecognizedDeclaration()),
                    };

                    string? ValidateKeyword(string keyword) =>
                        expectedKeywords.Length == 0 || expectedKeywords.Contains(keyword) ? keyword : null;
                },
                RecoveryFlags.None,
                TokenType.NewLine);

        private SyntaxResult TargetScope(IEnumerable<SyntaxBase> leadingNodes)
        {
            if (!ExpectKeyword(LanguageConstants.TargetScopeKeyword).IsSuccess(out var keyword, out var err))
            {
                return SyntaxResult.Failure(err);
            }

            var assignment = this.WithRecovery(this.Assignment, RecoveryFlags.None, TokenType.NewLine);
            var value = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), RecoveryFlags.None, TokenType.NewLine);

            return SyntaxResult.Success(new TargetScopeSyntax(leadingNodes, keyword, assignment, value));
        }

        private SyntaxResult MetadataDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            if (!ExpectKeyword(LanguageConstants.MetadataKeyword).IsSuccess(out var keyword, out var err))
            {
                return SyntaxResult.Failure(err);
            }

            var name = this.IdentifierWithRecovery(b => b.ExpectedMetadataIdentifier(), RecoveryFlags.None, TokenType.Assignment, TokenType.NewLine);
            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(name), TokenType.NewLine);
            var value = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), GetSuppressionFlag(assignment), TokenType.NewLine);

            return SyntaxResult.Success(new MetadataDeclarationSyntax(leadingNodes, keyword, name, assignment, value));
        }

        private SyntaxResult ParameterDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            if (!ExpectKeyword(LanguageConstants.ParameterKeyword).IsSuccess(out var keyword, out var err))
            {
                return SyntaxResult.Failure(err);
            }

            var name = this.IdentifierWithRecovery(b => b.ExpectedParameterIdentifier(), RecoveryFlags.None, TokenType.Identifier, TokenType.NewLine);
            var type = this.WithRecovery(() => Type(allowOptionalResourceType: false), GetSuppressionFlag(name), TokenType.Assignment, TokenType.LeftBrace, TokenType.NewLine);

            // TODO: Need a better way to choose the terminating token
            SyntaxBase? modifier = this.WithRecoveryNullable<SyntaxBase>(
                () =>
                {
                    var current = reader.Peek();
                    return current.Type switch
                    {
                        // the parameter does not have a modifier
                        TokenType.NewLine => (SyntaxResult?)null,
                        TokenType.EndOfFile => (SyntaxResult?)null,

                        // default value is specified
                        TokenType.Assignment => this.ParameterDefaultValue(),

                        _ => Fail<SyntaxBase>(current, b => b.ExpectedParameterContinuation())
                    };
                },
                GetSuppressionFlag(type),
                TokenType.NewLine);

            return SyntaxResult.Success(new ParameterDeclarationSyntax(leadingNodes, keyword, name, type, modifier));
        }

        private SyntaxResult ParameterDefaultValue()
        {
            // Caller pre-checks for Assignment token, so read directly.
            var assignmentToken = reader.Read();
            SyntaxBase defaultValue = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), RecoveryFlags.None, TokenType.NewLine);

            return SyntaxResult.Success(new ParameterDefaultValueSyntax(assignmentToken, defaultValue));
        }

        private SyntaxResult OutputDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            if (!ExpectKeyword(LanguageConstants.OutputKeyword).IsSuccess(out var keyword, out var err))
            {
                return SyntaxResult.Failure(err);
            }

            var name = this.IdentifierWithRecovery(b => b.ExpectedOutputIdentifier(), RecoveryFlags.None, TokenType.Identifier, TokenType.NewLine);
            var type = this.WithRecovery(() => Type(allowOptionalResourceType: true), GetSuppressionFlag(name), TokenType.Assignment, TokenType.NewLine);
            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(type), TokenType.NewLine);
            var value = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), GetSuppressionFlag(assignment), TokenType.NewLine);

            return SyntaxResult.Success(new OutputDeclarationSyntax(leadingNodes, keyword, name, type, assignment, value));
        }

        private SyntaxResult ResourceDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            if (!ExpectKeyword(LanguageConstants.ResourceKeyword).IsSuccess(out var keyword, out var err))
            {
                return SyntaxResult.Failure(err);
            }

            var name = this.IdentifierWithRecovery(b => b.ExpectedResourceIdentifier(), RecoveryFlags.None, TokenType.StringComplete, TokenType.StringLeftPiece, TokenType.NewLine);

            // TODO: Unify StringSyntax with TypeSyntax
            var type = this.WithRecovery(
                () => FailIfSkipped(this.InterpolableString, b => b.ExpectedResourceTypeString()),
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
                        _ => Fail<SyntaxBase>(current, b => b.ExpectBodyStartOrIfOrLoopStart())
                    };
                },
                GetSuppressionFlag(assignment),
                TokenType.NewLine);

            return SyntaxResult.Success(new ResourceDeclarationSyntax(leadingNodes, keyword, name, type, existingKeyword, assignment, newlines, value));
        }

        private SyntaxResult ModuleDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            if (!ExpectKeyword(LanguageConstants.ModuleKeyword).IsSuccess(out var keyword, out var err))
            {
                return SyntaxResult.Failure(err);
            }

            var name = this.IdentifierWithRecovery(b => b.ExpectedModuleIdentifier(), RecoveryFlags.None, TokenType.StringComplete, TokenType.StringLeftPiece, TokenType.NewLine);

            // TODO: Unify StringSyntax with TypeSyntax
            var path = this.WithRecovery(
                () => FailIfSkipped(this.InterpolableString, b => b.ExpectedModulePathString()),
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
                        _ => Fail<SyntaxBase>(current, b => b.ExpectBodyStartOrIfOrLoopStart())
                    };
                },
                GetSuppressionFlag(assignment),
                TokenType.NewLine);

            return SyntaxResult.Success(new ModuleDeclarationSyntax(leadingNodes, keyword, name, path, assignment, newlines, value));
        }
        private SyntaxResult TestDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            if (!ExpectKeyword(LanguageConstants.TestKeyword).IsSuccess(out var keyword, out var err))
            {
                return SyntaxResult.Failure(err);
            }

            var name = this.IdentifierWithRecovery(b => b.ExpectedTestIdentifier(), RecoveryFlags.None, TokenType.StringComplete, TokenType.StringLeftPiece, TokenType.NewLine);

            // TODO: Unify StringSyntax with TypeSyntax
            var path = this.WithRecovery(
                () => FailIfSkipped(this.InterpolableString, b => b.ExpectedTestPathString()),
                GetSuppressionFlag(name),
                TokenType.Assignment, TokenType.NewLine);

            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(path), TokenType.LeftBrace, TokenType.NewLine);

            var value = this.WithRecovery(() =>
                {
                    var current = reader.Peek();
                    return current.Type switch
                    {
                        TokenType.LeftBrace => this.Object(ExpressionFlags.AllowComplexLiterals),
                        _ => Fail<SyntaxBase>(current, b => b.ExpectedCharacter("{"))
                    };
                },
                GetSuppressionFlag(assignment),
                TokenType.NewLine);

            return SyntaxResult.Success(new TestDeclarationSyntax(leadingNodes, keyword, name, path, assignment, value));
        }

        private SyntaxResult ImportDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            if (!ExpectKeyword(LanguageConstants.ImportKeyword).IsSuccess(out var keyword, out var err))
            {
                return SyntaxResult.Failure(err);
            }

            // Provider namespace declarations use the `provider` keyword as of Bicep 0.23, but to avoid breaking
            // extensibility users without warning, the `import` keyword is shared between provider declarations and
            // compile-time imports. If the token following the keyword is a string, assume the statement is a provider
            // declaration.
            // TODO(extensibility): Consider removing this
            return reader.Peek().Type switch
            {
                TokenType.StringLeftPiece or
                TokenType.StringComplete => ExtensionDeclaration(keyword, leadingNodes),
                _ => CompileTimeImportDeclaration(keyword, leadingNodes).Transform(x => (SyntaxBase)x),
            };
        }

        private SyntaxResult AssertDeclaration(IEnumerable<SyntaxBase> leadingNodes)
        {
            if (!ExpectKeyword(LanguageConstants.AssertKeyword).IsSuccess(out var keyword, out var err))
            {
                return SyntaxResult.Failure(err);
            }

            var name = this.IdentifierWithRecovery(b => b.ExpectedAssertIdentifier(), RecoveryFlags.None, TokenType.Assignment, TokenType.NewLine);
            var assignment = this.WithRecovery(this.Assignment, GetSuppressionFlag(name), TokenType.NewLine);
            var value = this.WithRecovery(() => this.Expression(ExpressionFlags.AllowComplexLiterals), GetSuppressionFlag(assignment), TokenType.NewLine);

            return SyntaxResult.Success(new AssertDeclarationSyntax(leadingNodes, keyword, name, assignment, value));
        }
    }
}
