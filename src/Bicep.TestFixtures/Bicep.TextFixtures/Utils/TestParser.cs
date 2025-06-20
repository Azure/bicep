// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.TextFixtures.Utils
{
    public static class TestParser
    {
        public static ProgramSyntax Parse(string text) => new Parser(text).Program();

        public static ProgramSyntax Parse(string text, out IEnumerable<IDiagnostic> syntaxErrors)
        {
            var program = Parse(text, out var lexingErrorLookup, out var parsingErrorLookup);

            syntaxErrors = lexingErrorLookup.Concat(parsingErrorLookup);

            return program;
        }

        public static ProgramSyntax Parse(string text, out IDiagnosticLookup lexingErrorLookup, out IDiagnosticLookup parsingErrorLookup)
        {
            var parser = new Parser(text);
            var program = parser.Program();

            lexingErrorLookup = parser.LexingErrorLookup;
            parsingErrorLookup = parser.ParsingErrorLookup;

            return program;
        }

        public static ProgramSyntax ParamsParse(string text)
        {
            var parser = new ParamsParser(text);

            return parser.Program();
        }

        public static ProgramSyntax ParamsParse(string text, out IEnumerable<IDiagnostic> syntaxErrors)
        {
            var program = ParamsParse(text, out var lexingErrorLookup, out var parsingErrorLookup);

            syntaxErrors = lexingErrorLookup.Concat(parsingErrorLookup);

            return program;
        }

        public static ProgramSyntax ParamsParse(string text, out IDiagnosticLookup lexingErrorLookup, out IDiagnosticLookup parsingErrorLookup)
        {
            var parser = new ParamsParser(text);
            var program = parser.Program();

            lexingErrorLookup = parser.LexingErrorLookup;
            parsingErrorLookup = parser.ParsingErrorLookup;

            return program;
        }

        public static SyntaxBase ParseExpression(string text, ExpressionFlags expressionFlags = ExpressionFlags.AllowComplexLiterals) => new Parser(text).Expression(expressionFlags);
    }
}
