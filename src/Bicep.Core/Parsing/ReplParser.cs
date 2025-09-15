// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core; 

namespace Bicep.Core.Parsing;

/// <summary>
/// Lightweight REPL parser: parses either a single variable declaration starting with 'var'
/// or a single expression (allowing complex literals).
/// </summary>
public class ReplParser : BaseParser
{
    public ReplParser(string text) : base(text) { }

    /// <summary>
    /// Parses a single REPL line (variable declaration or expression) and returns syntax + diagnostics.
    /// </summary>
    public SyntaxBase ParseExpression(out IEnumerable<IDiagnostic> diagnostics)
    {
        SyntaxBase syntax;
        try
        {
            var first = reader.Peek();
            syntax = first.Type == TokenType.Identifier && first.Text == LanguageConstants.VariableKeyword
                ? VariableDeclaration([])
                : Expression(ExpressionFlags.AllowComplexLiterals);
        }
        catch (ExpectedTokenException ex)
        {
            var span = new TextSpan(0, 0);
            syntax = new SkippedTriviaSyntax(span, Array.Empty<SyntaxBase>(), new[] { ex.Error });
        }

        diagnostics = LexingErrorLookup.Concat(ParsingErrorLookup);
        return syntax;
    }

    public override ProgramSyntax Program()
    {
        var firstToken = reader.Peek();
        SyntaxBase first = firstToken.Type == TokenType.Identifier && firstToken.Text == LanguageConstants.VariableKeyword
            ? VariableDeclaration([])
            : Expression(ExpressionFlags.AllowComplexLiterals);

        var eof = reader.Read();
        var program = new ProgramSyntax(new[] { first }, eof);
        var parsingErrorVisitor = new ParseDiagnosticsVisitor(this.ParsingErrorTree);
        parsingErrorVisitor.Visit(program);
        return program;
    }

    protected override SyntaxBase Declaration(params string[] expectedKeywords) => SkipEmpty(b => b.UnrecognizedDeclaration());
}
