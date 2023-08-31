// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Text;
using System.Collections.Immutable;

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

