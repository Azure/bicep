// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core; 
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.Parsing;

/// <summary>
/// Lightweight REPL parser: parses either a single variable declaration starting with 'var'
/// or a single expression (allowing complex literals).
/// </summary>
public class ReplParser(string text) : BaseParser(text)
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
                    TokenType.Identifier => current.Text switch
                    {
                        LanguageConstants.VariableKeyword => this.VariableDeclaration(leadingNodes),
                        LanguageConstants.TypeKeyword => this.TypeDeclaration(leadingNodes),
                        LanguageConstants.FunctionKeyword => this.FunctionDeclaration(leadingNodes),
                        _ => WithRecovery(() => Expression(ExpressionFlags.AllowComplexLiterals), RecoveryFlags.ConsumeTerminator),
                    },
                    TokenType.NewLine => this.NewLine(),
                    _ => WithRecovery(() => Expression(ExpressionFlags.AllowComplexLiterals), RecoveryFlags.ConsumeTerminator),
                };
            },
            RecoveryFlags.None,
            TokenType.NewLine);
}
