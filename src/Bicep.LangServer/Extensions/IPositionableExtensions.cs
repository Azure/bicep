// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Text;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharpPosition = OmniSharp.Extensions.LanguageServer.Protocol.Models.Position;
using OmniSharpRange = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Extensions
{
    public static class IPositionableExtensions
    {
        public static TextSpan GetSpanSlice(this IPositionable positionable, int start, int length)
            => new(positionable.Span.Position + start, length);

        public static TextSpan TryGetSpanSlice(this IPositionable positionable, int start, int length)
            => GetSpanSlice(
                positionable,
                Math.Min(start, positionable.Span.Length),
                Math.Min(length, positionable.Span.Length - start));

        public static OmniSharpRange ToRange(this IPositionable positionable, ImmutableArray<int> lineStarts) =>
            new()
            {
                Start = PositionHelper.GetPosition(lineStarts, positionable.Span.Position),
                End = PositionHelper.GetPosition(lineStarts, positionable.GetEndPosition())
            };

        public static IEnumerable<OmniSharpRange> ToRangeSpanningLines(this IPositionable positionable, ImmutableArray<int> lineStarts)
        {
            var start = PositionHelper.GetPosition(lineStarts, positionable.Span.Position);
            var end = PositionHelper.GetPosition(lineStarts, positionable.GetEndPosition());

            while (start.Line < end.Line)
            {
                var lineEnd = PositionHelper.GetPosition(lineStarts, lineStarts[start.Line + 1] - 1);
                yield return new OmniSharpRange(start, lineEnd);

                start = new Position(start.Line + 1, 0);
            }

            yield return new OmniSharpRange(start, end);
        }

        public static int ToTextOffset(this OmniSharpPosition position, ImmutableArray<int> lineStarts)
        {
            return TextCoordinateConverter.GetOffset(lineStarts, position.Line, position.Character);
        }

        public static TextSpan ToTextSpan(this OmniSharpRange range, ImmutableArray<int> lineStarts)
        {
            var start = range.Start.ToTextOffset(lineStarts);
            return new(start, range.End.ToTextOffset(lineStarts) - start);
        }
    }
}
