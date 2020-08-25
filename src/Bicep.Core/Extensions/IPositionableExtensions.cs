// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parser;

namespace Bicep.Core.Extensions
{
    public static class IPositionableExtensions
    {
        public static TextSpan ToZeroLengthSpan(this IPositionable positionable)
            => new TextSpan(positionable.Span.Position, 0);
    }
}