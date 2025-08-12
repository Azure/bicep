// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.TextFixtures.Assertions;
using FluentAssertions;

namespace Bicep.TextFixtures.Utils
{
    public class TestSourcePrinter
    {
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

        public static string AssertValidAndPrint(ProgramSyntax programSyntax)
        {
            programSyntax.Should2().NotHaveAnySyntaxErrors();

            return PrettyPrinterV2.PrintValid(programSyntax, PrettyPrinterV2Options.Default);
        }

        public static string Print(BicepSourceFile sourceFile) => sourceFile.ProgramSyntax.ToString(NewlineKind.LF);

        public static string PrintWithAnnotations(BicepSourceFile bicepFile, IEnumerable<Annotation> annotations, int context, bool includeLineNumbers) =>
            PrintWithAnnotations(bicepFile.ProgramSyntax.ToString(), bicepFile.LineStarts, annotations, context, includeLineNumbers);

        public static string PrintWithAnnotations(string fileText, ImmutableArray<int> lineStarts, IEnumerable<Annotation> annotations, int context, bool includeLineNumbers)
        {
            var annotationPositions = annotations.ToDictionary(
                x => x,
                x => TextCoordinateConverter.GetPosition(lineStarts, x.Span.Position));

            if (annotationPositions.Count == 0)
            {
                return "";
            }

            var output = new StringBuilder();
            var programLines = StringUtils.SplitOnNewLine(fileText).ToArray();

            var annotationsByLine = annotationPositions.ToLookup(x => x.Value.line, x => x.Key);

            var minLine = annotationPositions.Values.Aggregate(int.MaxValue, (min, curr) => Math.Min(curr.line, min));
            var maxLine = annotationPositions.Values.Aggregate(0, (max, curr) => Math.Max(curr.line, max)) + 1;

            minLine = Math.Max(0, minLine - context);
            maxLine = Math.Min(lineStarts.Length, maxLine + context);
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
