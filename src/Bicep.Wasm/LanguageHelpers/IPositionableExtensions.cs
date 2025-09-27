// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Text;

namespace Bicep.Wasm.LanguageHelpers
{
    public static class IPositionableExtensions
    {
        public static Range ToRange(this IPositionable positionable, ImmutableArray<int> lineStarts) =>
            new()
            {
                Start = PositionHelper.GetPosition(lineStarts, positionable.Span.Position),
                End = PositionHelper.GetPosition(lineStarts, positionable.GetEndPosition())
            };

        public static IEnumerable<Range> ToRangeSpanningLines(this IPositionable positionable, ImmutableArray<int> lineStarts)
        {
            var start = PositionHelper.GetPosition(lineStarts, positionable.Span.Position);
            var end = PositionHelper.GetPosition(lineStarts, positionable.GetEndPosition());

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

