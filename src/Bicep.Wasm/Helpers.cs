using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Parser;
using Bicep.Core.Text;

namespace Bicep.Wasm
{
    public class Position
    {
        public Position(int line, int character)
        {
            Line = line;
            Character = character;
        }

        public int Line { get; }

        public int Character { get; }
    }

    public class Range
    {
        public Range(Position start, Position end)
        {
            Start = start;
            End = end;
        }

        public Range()
            : this(new Position(0, 0), new Position(0, 0))
        {
        }

        public Position Start { get; set; }

        public Position End { get; set; }
    }

    public static class PositionHelper
    {
        public static Position GetPosition(ImmutableArray<int> lineStarts, in int spanPosition)
        {
            (int line, int character) = TextCoordinateConverter.GetPosition(lineStarts, spanPosition);
            return new Position(line, character);
        }
    }

    public static class TextSpanExtensions
    {
        public static Range ToRange(this IPositionable positionable, ImmutableArray<int> lineStarts) => positionable.Span.ToRange(lineStarts);

        public static Range ToRange(this TextSpan span, ImmutableArray<int> lineStarts) =>
            new Range
            {
                Start = PositionHelper.GetPosition(lineStarts, span.Position),
                End = PositionHelper.GetPosition(lineStarts, span.Position + span.Length)
            };

        public static IEnumerable<Range> ToRangeSpanningLines(this IPositionable positionable, ImmutableArray<int> lineStarts) => positionable.Span.ToRangeSpanningLines(lineStarts);

        public static IEnumerable<Range> ToRangeSpanningLines(this TextSpan span, ImmutableArray<int> lineStarts)
        {
            var start = PositionHelper.GetPosition(lineStarts, span.Position);
            var end = PositionHelper.GetPosition(lineStarts, span.Position + span.Length);

            while (start.Line < end.Line)
            {
                var lineEnd = PositionHelper.GetPosition(lineStarts, lineStarts[start.Line + 1] - 1);
                yield return new Range(start, lineEnd);
                
                start = new Position(start.Line + 1, 0);
            }

            yield return new Range(start, end);
        }
    }

    public enum SemanticTokenType
    {
        Comment,
        EnumMember,
        Event,
        Modifier,
        Label,
        Parameter,
        Variable,
        Property,
        Member,
        Function,
        TypeParameter,
        Macro,
        Interface,
        Enum,
        String,
        Number,
        Regexp,
        Operator,
        Keyword,
        Type,
        Struct,
        Class,
        Namespace,
    }

    public class SemanticToken
    {
        public SemanticToken(int line, int character, int length, SemanticTokenType tokenType)
        {
            Line = line;
            Character = character;
            Length = length;
            TokenType = tokenType;
        }

        public int Line { get; set; }

        public int Character { get; set; }

        public int Length { get; set; }

        public SemanticTokenType TokenType { get; set; }
    }
}
