// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.Text;

namespace Bicep.Core.Diagnostics;

public class DisabledDiagnosticsCache
{
    private readonly Lazy<(
        ImmutableDictionary<int, DisableNextLineDirectiveEndPositionAndCodes> disableNextLineDiagnosticDirectivesCache,
        FrozenDictionary<string, ImmutableArray<TextSpan>> disabledDiagnosticsByCodeCache
    )> cachesLazy;

    public DisabledDiagnosticsCache(ProgramSyntax programSyntax, ImmutableArray<int> lineStarts)
    {
        cachesLazy = new(() => SyntaxTriviaVisitor.ComputeDisabledDiagnosticsCaches(lineStarts, programSyntax));
    }

    public DisableNextLineDirectiveEndPositionAndCodes? TryGetDisabledNextLineDirective(int lineNumber)
        => cachesLazy.Value.disableNextLineDiagnosticDirectivesCache.TryGetValue(lineNumber);

    public record DisableNextLineDirectiveEndPositionAndCodes(int endPosition, ImmutableArray<string> diagnosticCodes);

    public ImmutableDictionary<int, DisableNextLineDirectiveEndPositionAndCodes> GetDisableNextLineDiagnosticDirectivesCache()
        => cachesLazy.Value.disableNextLineDiagnosticDirectivesCache;

    public bool IsDisabledAtPosition(string diagnosticCode, int position)
    {
        if (cachesLazy.Value.disabledDiagnosticsByCodeCache.TryGetValue(diagnosticCode, out var disabledForSpans))
        {
            int startIndex = 0;
            int endIndex = disabledForSpans.Length - 1;

            while (startIndex <= endIndex)
            {
                int midIndex = startIndex + (endIndex - startIndex) / 2;
                var midSpan = disabledForSpans[midIndex];

                if (position < midSpan.Position)
                {
                    // The provided position precedes the start of the mid span, so it must be in an earlier span (if any)
                    endIndex = midIndex - 1;
                }
                else if (position > midSpan.GetEndPosition())
                {
                    // The provided position follows the end of the mid span, so it must be in an later span (if any)
                    startIndex = midIndex + 1;
                }
                else
                {
                    // The provided position is within the mid span, so the diagnostic is disabled at that position
                    return true;
                }
            }
        }

        return false;
    }

    private class SyntaxTriviaVisitor : CstVisitor
    {
        private readonly ImmutableArray<int> lineStarts;
        private readonly int eof;
        private readonly ImmutableDictionary<int, DisableNextLineDirectiveEndPositionAndCodes>.Builder disableNextLineDiagnosticDirectivesCacheBuilder = ImmutableDictionary.CreateBuilder<int, DisableNextLineDirectiveEndPositionAndCodes>();
        private readonly Dictionary<string, List<TextSpan>> disabledDiagnosticSpansByCode = new();
        private readonly Dictionary<string, int> disabledDiagnosticSpanStarts = new();

        public SyntaxTriviaVisitor(ImmutableArray<int> lineStarts, int eof)
        {
            this.lineStarts = lineStarts;
            this.eof = eof;
        }

        public static (
            ImmutableDictionary<int, DisableNextLineDirectiveEndPositionAndCodes>,
            FrozenDictionary<string, ImmutableArray<TextSpan>>
        ) ComputeDisabledDiagnosticsCaches(ImmutableArray<int> lineStarts, ProgramSyntax programSyntax)
        {
            var visitor = new SyntaxTriviaVisitor(lineStarts, programSyntax.Span.GetEndPosition());
            visitor.Visit(programSyntax);

            Dictionary<string, ImmutableArray<TextSpan>> disabledDiagnosticSpansByCodeBuilder = new();
            foreach (var kvp in visitor.disabledDiagnosticSpanStarts)
            {
                visitor.disabledDiagnosticSpansByCode.GetOrAdd(kvp.Key, _ => new()).Add(new(kvp.Value, visitor.eof - kvp.Value));
            }

            foreach (var kvp in visitor.disabledDiagnosticSpansByCode.Where(kvp => kvp.Value.Count > 0))
            {
                kvp.Value.Sort((a, b) => a.Position.CompareTo(b.Position));

                var processedSpans = ImmutableArray.CreateBuilder<TextSpan>();
                var processing = kvp.Value[0];

                for (int i = 1; i < kvp.Value.Count; i++)
                {
                    if (kvp.Value[i].Position <= processing.GetEndPosition())
                    {
                        // The span at position i overlaps with or is contained within the span being processed. Combine them into a single span
                        processing = new(
                            processing.Position,
                            Math.Max(processing.Length, kvp.Value[i].GetEndPosition() - processing.Position));
                        continue;
                    }

                    processedSpans.Add(processing);
                    processing = kvp.Value[i];
                }

                processedSpans.Add(processing);

                disabledDiagnosticSpansByCodeBuilder[kvp.Key] = processedSpans.ToImmutable();
            }

            return (visitor.disableNextLineDiagnosticDirectivesCacheBuilder.ToImmutable(), disabledDiagnosticSpansByCodeBuilder.ToFrozenDictionary());
        }

        public override void VisitSyntaxTrivia(SyntaxTrivia syntaxTrivia)
        {
            if (syntaxTrivia is DiagnosticsPragmaSyntaxTrivia diagnosticsPragma)
            {
                switch (diagnosticsPragma.PragmaType)
                {
                    case DiagnosticsPragmaType.DisableNextLine:
                        var codes = diagnosticsPragma.DiagnosticCodes.Select(x => x.Text).ToImmutableArray();
                        (int line, _) = TextCoordinateConverter.GetPosition(lineStarts, syntaxTrivia.Span.Position);
                        DisableNextLineDirectiveEndPositionAndCodes disableNextLineDirectiveEndPosAndCodes = new(syntaxTrivia.Span.GetEndPosition(), codes);
                        disableNextLineDiagnosticDirectivesCacheBuilder.Add(line, disableNextLineDirectiveEndPosAndCodes);

                        int spanEnd = line + 2 < lineStarts.Length ? lineStarts[line + 2] - 1 : eof;
                        foreach (var code in codes)
                        {
                            disabledDiagnosticSpansByCode.GetOrAdd(code, _ => new()).Add(new(syntaxTrivia.Span.Position, spanEnd - syntaxTrivia.Span.Position));
                        }
                        break;
                    case DiagnosticsPragmaType.Disable:
                        foreach (var code in diagnosticsPragma.DiagnosticCodes)
                        {
                            disabledDiagnosticSpanStarts.TryAdd(code.Text, syntaxTrivia.Span.Position);
                        }
                        break;
                    case DiagnosticsPragmaType.Restore:
                        foreach (var code in diagnosticsPragma.DiagnosticCodes)
                        {
                            if (disabledDiagnosticSpanStarts.TryGetValue(code.Text, out int spanStart))
                            {
                                disabledDiagnosticSpansByCode.GetOrAdd(code.Text, _ => new()).Add(new(spanStart, syntaxTrivia.Span.GetEndPosition() - spanStart));
                                disabledDiagnosticSpanStarts.Remove(code.Text);
                            }
                        }
                        break;
                }
            }
        }
    }
}
