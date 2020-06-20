using System.Collections.Immutable;
using Bicep.Core.Position;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Utils
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
