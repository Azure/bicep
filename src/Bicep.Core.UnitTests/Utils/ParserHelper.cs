// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using FluentAssertions;

namespace Bicep.Core.UnitTests.Utils
{
    public static class ParserHelper
    {
        public static ProgramSyntax Parse(string text)
        {
            var parser = new Parser(text, BicepTestConstants.Features);

            return parser.Program();
        }

        public static ProgramSyntax Parse(string text, out IEnumerable<IDiagnostic> syntaxErrors)
        {
            var parser = new Parser(text,BicepTestConstants.Features);
            var program = parser.Program();

            syntaxErrors = parser.LexingErrorLookup.Concat(parser.ParsingErrorLookup);

            return program;
        }

        public static ProgramSyntax Parse(string text, out IDiagnosticLookup lexingErrorLookup, out IDiagnosticLookup parsingErrorLookup)
        {
            var parser = new Parser(text,BicepTestConstants.Features);
            var program = parser.Program();

            lexingErrorLookup = parser.LexingErrorLookup;
            parsingErrorLookup = parser.ParsingErrorLookup;

            return program;
        }

        public static ProgramSyntax ParamsParse(string text)
        {
            var parser = new ParamsParser(text,BicepTestConstants.Features);

            return parser.Program();
        }

        public static ProgramSyntax ParamsParse(string text, out IDiagnosticLookup lexingErrorLookup, out IDiagnosticLookup parsingErrorLookup)
        {
            var parser = new ParamsParser(text,BicepTestConstants.Features);
            var program = parser.Program();

            lexingErrorLookup = parser.LexingErrorLookup;
            parsingErrorLookup = parser.ParsingErrorLookup;

            return program;
        }

        public static SyntaxBase ParseExpression(string text, ExpressionFlags expressionFlags = ExpressionFlags.AllowComplexLiterals) => new Parser(text).Expression(expressionFlags,BicepTestConstants.Features);

        public static (string file, IReadOnlyList<int> cursors) GetFileWithCursors(string fileWithCursors, char cursor = '|')
            => GetFileWithCursors(fileWithCursors, cursor.ToString());

        public static (string file, IReadOnlyList<int> cursors) GetFileWithCursors(string fileWithCursors, string cursor)
        {
            var fileWithoutCursors = fileWithCursors.Replace(cursor, "");
            var cursors = new List<int>();
            var position = 0;

            while ((position = fileWithCursors.IndexOf(cursor, position)) != -1)
            {
                cursors.Add(position - (cursors.Count * cursor.Length));
                position += cursor.Length;
            }

            return (fileWithoutCursors, cursors);
        }

        public static (string file, int cursor) GetFileWithSingleCursor(string fileWithCursors, char cursor = '|')
            => GetFileWithSingleCursor(fileWithCursors, cursor.ToString());

        public static (string file, int cursor) GetFileWithSingleCursor(string fileWithCursors, string cursor)
        {
            var (file, cursors) = GetFileWithCursors(fileWithCursors, cursor);
            cursors.Should().HaveCount(1);

            return (file, cursors.Single());
        }
    }
}

