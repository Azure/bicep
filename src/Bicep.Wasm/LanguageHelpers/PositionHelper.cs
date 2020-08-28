// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Text;

namespace Bicep.Wasm.LanguageHelpers
{
    public static class PositionHelper
    {
        public static Position GetPosition(ImmutableArray<int> lineStarts, in int spanPosition)
        {
            (int line, int character) = TextCoordinateConverter.GetPosition(lineStarts, spanPosition);
            return new Position(line, character);
        }
    }
}

