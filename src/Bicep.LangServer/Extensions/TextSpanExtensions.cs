using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Parser;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Extensions
{
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
}
