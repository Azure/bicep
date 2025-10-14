// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text;
using Bicep.Cli.Helpers.WhatIf; // TODO: Move the usages (namely colorization helpers) to a common namespace
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Highlighting;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.Helpers.Repl;

public class PrintHelper
{
    public const string MoveCursorToLineStart = "\r";
    public const string ClearToEndOfScreen = "\u001b[0J";
    public const string HideCursor = "\u001b[?25l";
    public const string ShowCursor = "\u001b[?25h";
    public static string MoveCursorRight(int count) => count > 0 ? $"\u001b[{count}C" : string.Empty;
    public static string MoveCursorUp(int count) => count > 0 ? $"\u001b[{count}A" : string.Empty;
    
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

    public static string PrintWithAnnotations(string text, IEnumerable<AnnotatedDiagnostic> annotations, string? fileTextToPrint = null)
        => PrintWithAnnotations(text, TextCoordinateConverter.GetLineStarts(text), annotations, fileTextToPrint);

    public static string PrintWithAnnotations(string fileText, ImmutableArray<int> lineStarts, IEnumerable<AnnotatedDiagnostic> annotations, string? fileTextToPrint = null)
    {
        var annotationPositions = annotations.ToDictionary(
            x => x,
            x => TextCoordinateConverter.GetPosition(lineStarts, x.Span.Position));

        if (annotationPositions.Count == 0)
        {
            return string.Empty;
        }

        var outputBuilder = new ColoredStringBuilder();

        var linesToPrint = StringUtils.SplitOnNewLine(fileTextToPrint ?? fileText).ToArray();

        var annotationsByLine = annotationPositions.ToLookup(x => x.Value.line, x => x.Key);

        var minLine = annotationPositions.Values.Aggregate(int.MaxValue, (min, curr) => Math.Min(curr.line, min));
        var maxLine = annotationPositions.Values.Aggregate(0, (max, curr) => Math.Max(curr.line, max)) + 1;

        minLine = Math.Max(0, minLine);
        maxLine = Math.Min(lineStarts.Length, maxLine);

        for (var i = minLine; i < maxLine; i++)
        {
            var gutterOffset = 0;
            outputBuilder.Append(linesToPrint[i]);
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

                var position = annotationPositions[annotation];
                outputBuilder.Append(new string(' ', gutterOffset + position.character));

                using (outputBuilder.NewColorScope(color))
                {
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
                }
                outputBuilder.Append('\n');
            }
        }

        return outputBuilder.ToString();
    }

    public static string PrintWithSyntaxHighlighting(SemanticModel model, string fullContent, int startPosition = 0)
    {
        var tokens = SemanticTokenVisitor.Build(model);
        var overlapping = tokens
            .Where(x => x.Positionable.IsOverlapping(startPosition) || x.Positionable.IsOnOrAfter(startPosition))
            .OrderBy(x => x.Positionable.GetPosition());

        var outputSb = new StringBuilder();
        var cursor = startPosition;
        foreach (var token in overlapping)
        {
            for (var i = cursor; i < token.Positionable.GetPosition(); i++)
            {
                outputSb.Append(fullContent[i]);
            }

            var tokenStart = Math.Max(cursor, token.Positionable.GetPosition());
            var tokenEnd = token.Positionable.GetEndPosition();

            outputSb.Append(GetSyntaxColor(token).ToString());
            for (var i = tokenStart; i < tokenEnd; i++)
            {
                outputSb.Append(fullContent[i]);
            }
            outputSb.Append(Color.Reset.ToString());
            cursor = tokenEnd;
        }

        for (var i = cursor; i < fullContent.Length; i++)
        {
            outputSb.Append(fullContent[i]);
        }

        return outputSb.ToString();
    }

    public static string PrintInputLine(string prefix, string content, int cursorOffset)
    {
        var output = new StringBuilder();

        output.Append(HideCursor);

        output.Append(MoveCursorToLineStart);
        output.Append(ClearToEndOfScreen);

        output.Append(ColoredStringBuilder.Colorize(prefix, Color.Gray));
        output.Append(content);

        output.Append(MoveCursorToLineStart);
        if (!content.Contains('\n'))
        {
            output.Append(MoveCursorRight(prefix.Length));
        }
        output.Append(MoveCursorRight(cursorOffset));

        output.Append(ShowCursor);

        return output.ToString();
    }

    private static Color GetSyntaxColor(SemanticToken token) => token.TokenType switch
    {
        SemanticTokenType.Operator => Color.Reset,
        SemanticTokenType.Comment => Color.Green,
        SemanticTokenType.Keyword => Color.Gray,
        SemanticTokenType.Variable => Color.Blue,
        SemanticTokenType.Function => Color.Blue,
        SemanticTokenType.Type => Color.Purple,
        SemanticTokenType.Property => Color.DarkYellow,
        SemanticTokenType.TypeParameter => Color.DarkYellow,
        SemanticTokenType.String => Color.Orange,
        SemanticTokenType.Number => Color.Orange,
        _ => Color.Reset,
    };
}
