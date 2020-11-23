// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Parsing;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Extensions
{
    public static class IPositionableExtensions
    {
        public static TextSpan GetSpanSlice(this IPositionable positionable, int start, int length)
            => new TextSpan(positionable.Span.Position + start, length);

        public static TextSpan SafeGetSpanSlice(this IPositionable positionable, int start, int length)
            => GetSpanSlice(
                positionable,
                Math.Min(start, positionable.Span.Length),
                Math.Min(length, positionable.Span.Length - start));

        public static Range ToRange(this IPositionable positionable, ImmutableArray<int> lineStarts) =>
            new Range
            {
                Start = PositionHelper.GetPosition(lineStarts, positionable.Span.Position),
                End = PositionHelper.GetPosition(lineStarts, positionable.Span.Position + positionable.Span.Length)
            };

        public static IEnumerable<Range> ToRangeSpanningLines(this IPositionable positionable, ImmutableArray<int> lineStarts)
        {
            var start = PositionHelper.GetPosition(lineStarts, positionable.Span.Position);
            var end = PositionHelper.GetPosition(lineStarts, positionable.Span.Position + positionable.Span.Length);

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

