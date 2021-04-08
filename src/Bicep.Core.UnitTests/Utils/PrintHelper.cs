// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Assertions;

namespace Bicep.Core.UnitTests.Utils
{
    public static class PrintHelper
    {
        private static PrettyPrintOptions DefaultOptions { get; } = new PrettyPrintOptions(
            NewlineOption.Auto,
            IndentKindOption.Space,
            2,
            false);

        public static string PrintAndCheckForParseErrors(ProgramSyntax programSyntax)
        {
            var asString = PrettyPrinter.PrintProgram(programSyntax, DefaultOptions);

            var parsed = ParserHelper.Parse(asString);
            parsed.GetParseDiagnostics().Should().BeEmpty();

            return asString;
        }

        public class Annotation : IPositionable
        {
            public Annotation(TextSpan span, string message)
            {
                Span = span;
                Message = message;
            }

            public TextSpan Span { get; }

            public string Message { get; }
        }

        private static string GetProgramText(SyntaxTree syntaxTree)
        {
            var buffer = new StringBuilder();
            var visitor = new PrintVisitor(buffer);

            visitor.Visit(syntaxTree.ProgramSyntax);

            return buffer.ToString();
        }

        private static string[] GetProgramTextLines(SyntaxTree syntaxTree)
        {
            var programText = GetProgramText(syntaxTree);

            return StringUtils.ReplaceNewlines(programText, "\n").Split("\n");
        }

        public static string PrintWithAnnotations(SyntaxTree syntaxTree, IEnumerable<Annotation> annotations, int context, bool includeLineNumbers)
        {
            var output = new StringBuilder();
            var programLines = GetProgramTextLines(syntaxTree);

            var annotationPositions = annotations.ToDictionary(
                x => x,
                x => TextCoordinateConverter.GetPosition(syntaxTree.LineStarts, x.Span.Position));

            var annotationsByLine = annotationPositions.ToLookup(x => x.Value.line, x => x.Key);

            var minLine = annotationPositions.Values.Aggregate(int.MaxValue, (min, curr) => Math.Min(curr.line, min));
            var maxLine = annotationPositions.Values.Aggregate(0, (max, curr) => Math.Max(curr.line, max)) + 1;

            minLine = Math.Max(0, minLine - context);
            maxLine = Math.Min(syntaxTree.LineStarts.Length, maxLine + context);
            var digits = maxLine.ToString().Length;

            for (var i = minLine; i < maxLine; i++)
            {
                var gutterOffset = 0;
                if (includeLineNumbers)
                {
                    var lineNumber = i + 1; // to match VSCode's line numbering (starting at 1)
                    output.Append(lineNumber.ToString().PadLeft(digits, '0'));
                    output.Append("| ");

                    gutterOffset = digits + 2;
                }
                output.Append(programLines[i]);
                output.Append('\n');

                var annotationsToDisplay = annotationsByLine[i].OrderBy(x => annotationPositions[x].character);
                foreach (var annotation in annotationsToDisplay)
                {
                    var position = annotationPositions[annotation];
                    output.Append(new String(' ', gutterOffset + position.character));

                    switch (annotation.Span.Length)
                    {
                        case 0:
                            output.Append("^");
                            break;
                        case int x:
                            // TODO handle annotation spanning multiple lines
                            output.Append(new String('~', x));
                            break;
                    }
                    
                    output.Append(" ");
                    output.Append(annotation.Message);
                    output.Append('\n');
                }
            }

            return output.ToString();
        }
    }
}