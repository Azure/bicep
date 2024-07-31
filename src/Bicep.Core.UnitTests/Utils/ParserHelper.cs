// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
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
            var parser = new Parser(text);

            return parser.Program();
        }

        public static ProgramSyntax Parse(string text, out IEnumerable<IDiagnostic> syntaxErrors)
        {
            var parser = new Parser(text);
            var program = parser.Program();

            syntaxErrors = parser.LexingErrorLookup.Concat(parser.ParsingErrorLookup);

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

        public static ProgramSyntax ParamsParse(string text, out IDiagnosticLookup lexingErrorLookup, out IDiagnosticLookup parsingErrorLookup)
        {
            var parser = new ParamsParser(text);
            var program = parser.Program();

            lexingErrorLookup = parser.LexingErrorLookup;
            parsingErrorLookup = parser.ParsingErrorLookup;

            return program;
        }

        public static SyntaxBase ParseExpression(string text, ExpressionFlags expressionFlags = ExpressionFlags.AllowComplexLiterals) => new Parser(text).Expression(expressionFlags);

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

        public static (string file, TextSpan selection) GetFileWithSingleSelection(string fileWithSelections, char emptySelectionCursor = '|', string selectionStartCursor = "<<", string selectionEndCursor = ">>")
        {
            var (file, selections) = GetFileWithSelections(fileWithSelections, emptySelectionCursor, selectionStartCursor, selectionEndCursor);
            selections.Should().HaveCount(1);

            return (file, selections.Single());
        }

        public static (string file, IReadOnlyList<TextSpan> selections) GetFileWithSelections(string fileWithSelections, char emptySelectionCursor = '|', string selectionStartCursor = "<<", string selectionEndCursor = ">>")
        {
            fileWithSelections = fileWithSelections.Replace(emptySelectionCursor.ToString(), selectionStartCursor + selectionEndCursor);

            var fileWithoutSelections = fileWithSelections;

            var selections = new List<TextSpan>();
            int startPosition, endPosition;
            int nextPosition = 0;

            while ((startPosition = fileWithoutSelections.IndexOf(selectionStartCursor, nextPosition)) != -1
                && (endPosition = fileWithoutSelections.IndexOf(selectionEndCursor, startPosition)) != -1)
            {
                var span = new TextSpan(startPosition, endPosition - startPosition - selectionStartCursor.Length);

                fileWithoutSelections = fileWithoutSelections.Substring(0, startPosition)
                    + fileWithoutSelections.Substring(startPosition + selectionStartCursor.Length, span.Length)
                    + fileWithoutSelections.Substring(endPosition + selectionEndCursor.Length);

                selections.Add(span);
                nextPosition = span.Position + span.Length;
            }

            if (fileWithoutSelections.IndexOf(selectionStartCursor) != -1 || fileWithoutSelections.IndexOf(selectionEndCursor) != -1)
            {
                throw new ArgumentException($"{nameof(GetFileWithSelections)}: Mismatched selection cursors in input string");
            }

            return (fileWithoutSelections, selections);
        }

    }
}

