// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text;
using Bicep.Cli.Helpers.WhatIf; // TODO: Move the usages (namely colorization helpers) to a common namespace
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.SourceGraph;
using Bicep.Core.Text;

namespace Bicep.Cli.Helpers.Repl;

public class PrintHelper
{
    public class AnnotatedDiagnostic : IPositionable
    {
        // TODO: Rethink this
        public AnnotatedDiagnostic(IDiagnostic diagnostic, Func<TextSpan>? narrowSpan = null)
        {
            Span = narrowSpan?.Invoke() ?? diagnostic.Span;
            Diagnostic = diagnostic;
        }

        public TextSpan Span { get; }
        public IDiagnostic Diagnostic { get; }
    }

    public static string PrintWithAnnotations(string text, IEnumerable<AnnotatedDiagnostic> annotations)
        => PrintWithAnnotations(text, TextCoordinateConverter.GetLineStarts(text), annotations);

    public static string PrintWithAnnotations(string fileText, ImmutableArray<int> lineStarts, IEnumerable<AnnotatedDiagnostic> annotations)
    {
        var annotationPositions = annotations.ToDictionary(
            x => x,
            x => TextCoordinateConverter.GetPosition(lineStarts, x.Span.Position));

        if (annotationPositions.Count == 0)
        {
            return string.Empty;
        }

        var outputBuilder = new ColoredStringBuilder();

        var programLines = StringUtils.SplitOnNewLine(fileText).ToArray();

        var annotationsByLine = annotationPositions.ToLookup(x => x.Value.line, x => x.Key);

        var minLine = annotationPositions.Values.Aggregate(int.MaxValue, (min, curr) => Math.Min(curr.line, min));
        var maxLine = annotationPositions.Values.Aggregate(0, (max, curr) => Math.Max(curr.line, max)) + 1;

        minLine = Math.Max(0, minLine);
        maxLine = Math.Min(lineStarts.Length, maxLine);

        for (var i = minLine; i < maxLine; i++)
        {
            var gutterOffset = 0;
            outputBuilder.Append(programLines[i]);
            outputBuilder.Append('\n');

            var annotationsToDisplay = annotationsByLine[i].OrderBy(x => annotationPositions[x].character);
            foreach (var annotation in annotationsToDisplay)
            {
                var color = annotation.Diagnostic.Level switch
                {
                    DiagnosticLevel.Error => Color.Red,
                    DiagnosticLevel.Warning => Color.Orange,
                    _ => Color.Reset,
                };

                using (outputBuilder.NewColorScope(color))
                {
                    var position = annotationPositions[annotation];
                    outputBuilder.Append(new string(' ', gutterOffset + position.character));

                    switch (annotation.Span.Length)
                    {
                        case 0:
                            outputBuilder.Append('^');
                            break;
                        case int x:
                            outputBuilder.Append(new string('~', x));
                            break;
                    }

                    outputBuilder.Append(' ');
                    outputBuilder.Append(annotation.Diagnostic.Message);
                    outputBuilder.Append('\n');
                }
                
            }
        }
        
        return outputBuilder.ToString();
    }
}
