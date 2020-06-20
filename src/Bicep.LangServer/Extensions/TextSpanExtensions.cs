using System.Collections.Immutable;
using Bicep.Core.Parser;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Extensions
{
    public static class TextSpanExtensions
    {
        public static Range ToRange(this IPositionable positionable, ImmutableArray<int> lineStarts) => positionable.Span.ToRange(lineStarts);

        public static Range ToRange(this TextSpan span, ImmutableArray<int> lineStarts) =>
            new Range
            {
                Start = PositionHelper.GetPosition(lineStarts, span.Position),
                End = PositionHelper.GetPosition(lineStarts, span.Position + span.Length)
            };
    }
}
