// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;

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
    }
}
