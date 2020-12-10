// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;

namespace Bicep.Core.Extensions
{
    public static class IPositionableExtensions
    {
        public static TextSpan ToZeroLengthSpan(this IPositionable positionable)
            => new TextSpan(positionable.Span.Position, 0);

        public static int GetEndPosition(this IPositionable positionable)
            => positionable.Span.Position + positionable.Span.Length;
    }
}