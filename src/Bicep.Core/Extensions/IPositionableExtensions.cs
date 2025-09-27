// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Text;

namespace Bicep.Core.Extensions
{
    public static class IPositionableExtensions
    {
        public static TextSpan ToZeroLengthSpan(this IPositionable positionable)
            => new(positionable.Span.Position, 0);

        public static int GetPosition(this IPositionable positionable)
            => positionable.Span.Position;

        public static int GetEndPosition(this IPositionable positionable)
            => positionable.Span.Position + positionable.Span.Length;

        public static bool IsOverlapping(this IPositionable positionable, int position)
            => positionable.GetPosition() <= position && position <= positionable.GetEndPosition();

        public static bool IsEnclosing(this IPositionable positionable, int position)
            => positionable.GetPosition() < position && position < positionable.GetEndPosition();

        public static bool IsBefore(this IPositionable positionable, int offset)
            => positionable.GetEndPosition() < offset;

        public static bool IsAfter(this IPositionable positionable, int offset)
            => positionable.GetPosition() > offset;

        public static bool IsOnOrAfter(this IPositionable positionable, int offset)
            => positionable.GetPosition() >= offset;

        public static TextSpan GetSpanSlice(this IPositionable positionable, int start, int length)
            => new(positionable.Span.Position + start, length);

        public static TextSpan TryGetSpanSlice(this IPositionable positionable, int start, int length)
            => GetSpanSlice(
                positionable,
                Math.Min(start, positionable.Span.Length),
                Math.Min(length, positionable.Span.Length - start));
    }
}
