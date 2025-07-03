// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
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
            var parser = new ParamsParser(text, RecordBasedFeatureProvider.AllDisabled);

            return parser.Program();
        }

        public static ProgramSyntax ParamsParse(string text, out IDiagnosticLookup lexingErrorLookup, out IDiagnosticLookup parsingErrorLookup)
        {
            var parser = new ParamsParser(text, RecordBasedFeatureProvider.AllDisabled);
            var program = parser.Program();

            lexingErrorLookup = parser.LexingErrorLookup;
            parsingErrorLookup = parser.ParsingErrorLookup;

            return program;
        }

        public static SyntaxBase ParseExpression(string text, ExpressionFlags expressionFlags = ExpressionFlags.AllowComplexLiterals) => new Parser(text).Expression(expressionFlags);

        public static (string file, IReadOnlyList<int> cursors) GetFileWithCursors(string fileWithCursors, char cursor = '|', string escapedCursor = "||")
            => GetFileWithCursors(fileWithCursors, cursor.ToString(), escapedCursor);

        public static (string file, IReadOnlyList<int> cursors) GetFileWithCursors(string fileWithCursors, string cursor, string escapedCursor)
        {
            var fileWithoutCursors = fileWithCursors
                .Replace(escapedCursor, "<<ESCAPEDCURSOR>>")
                .Replace(cursor, "")
                .Replace("<<ESCAPEDCURSOR>>", "");
            var cursors = new List<int>();
            var position = 0;

            while ((position = fileWithCursors.IndexOf(cursor, position)) != -1)
            {
                cursors.Add(position - (cursors.Count * cursor.Length));
                position += cursor.Length;
            }

            return (fileWithoutCursors, cursors);
        }

        public static (string file, int cursor) GetFileWithSingleCursor(string fileWithCursors, char cursor = '|', string escapedCursor = "||")
            => GetFileWithSingleCursor(fileWithCursors, cursor.ToString(), escapedCursor);

        public static (string file, int cursor) GetFileWithSingleCursor(string fileWithCursors, string cursor, string escapedCursor = "||")
        {
            var (file, cursors) = GetFileWithCursors(fileWithCursors, cursor, escapedCursor);
            cursors.Should().HaveCount(1);

            return (file, cursors.Single());
        }

        public static (string file, TextSpan selection) GetFileWithSingleSelection(string fileWithSelections, string emptySelectionCursor = "|", string escapedCursor = "||", string selectionStartCursor = "<<", string selectionEndCursor = ">>")
        {
            var (file, selections) = GetFileWithSelections(fileWithSelections, emptySelectionCursor, escapedCursor, selectionStartCursor, selectionEndCursor);
            selections.Should().HaveCount(1);

            return (file, selections.Single());
        }

        public static (string file, IReadOnlyList<TextSpan> selections) GetFileWithSelections(string fileWithSelections, string emptySelectionCursor = "|", string escapedCursor = "||", string selectionStartCursor = "<<", string selectionEndCursor = ">>")
        {
            const string SELECTIONSTART = "SELECTIONSTART";
            const string SELECTIONEND = "SELECTIONEND";
            const string ESCAPEDCURSOR = "ESCAPEDCURSOR";

            if (emptySelectionCursor.Length > 0)
            {
                fileWithSelections = fileWithSelections
                    .Replace(escapedCursor, ESCAPEDCURSOR)
                    .Replace(emptySelectionCursor, SELECTIONSTART + SELECTIONEND)
                    .Replace(ESCAPEDCURSOR, emptySelectionCursor);
            }

            if (selectionStartCursor.Length > 0 && selectionEndCursor.Length > 0)
            {
                fileWithSelections = fileWithSelections.Replace(selectionStartCursor, SELECTIONSTART)
                    .Replace(selectionEndCursor, SELECTIONEND);
            }

            var fileWithoutSelections = fileWithSelections;

            var selections = new List<TextSpan>();
            int startPosition, endPosition;
            int nextPosition = 0;

            while ((startPosition = fileWithoutSelections.IndexOf(SELECTIONSTART, nextPosition)) != -1
                && (endPosition = fileWithoutSelections.IndexOf(SELECTIONEND, startPosition)) != -1)
            {
                var span = new TextSpan(startPosition, endPosition - startPosition - SELECTIONSTART.Length);

                fileWithoutSelections = fileWithoutSelections.Substring(0, startPosition)
                    + fileWithoutSelections.Substring(startPosition + SELECTIONSTART.Length, span.Length)
                    + fileWithoutSelections.Substring(endPosition + SELECTIONEND.Length);

                selections.Add(span);
                nextPosition = span.Position + span.Length;
            }

            if (fileWithoutSelections.IndexOf(SELECTIONSTART) != -1 || fileWithoutSelections.IndexOf(SELECTIONEND) != -1)
            {
                throw new ArgumentException($"{nameof(GetFileWithSelections)}: Mismatched selection cursors in input string");
            }

            return (fileWithoutSelections, selections);
        }

    }
}

