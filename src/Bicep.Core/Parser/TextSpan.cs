using System;
using System.Text.RegularExpressions;

namespace Bicep.Core.Parser
{
    public class TextSpan
    {
        private static readonly Regex TextSpanPattern = new Regex(@"^\[(?<startInclusive>\d+)\:(?<endExclusive>\d+)\]$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public TextSpan(int position, int length) 
        {
            if (position < 0)
            {
                throw new ArgumentException("Position must not be negative.", nameof(position));
            }

            if (length < 0)
            {
                throw new ArgumentException("Length must not be negative.", nameof(length));
            }

            Position = position;
            Length = length;
        }

        public int Position { get; }

        public int Length { get; }

        public override string ToString() => $"[{Position}:{Position + Length}]";

        /// <summary>
        /// Calculates the span from the beginning of the first span to the end of the second span.
        /// </summary>
        /// <param name="a">The first span</param>
        /// <param name="b">The second span</param>
        /// <returns>the span from the beginning of the first span to the end of the second span</returns>
        public static TextSpan Between(TextSpan a, TextSpan b) => new TextSpan(a.Position, b.Position + b.Length - a.Position);

        /// <summary>
        /// Calculates the span from the beginning of the first object to the end of the 2nd one.
        /// </summary>
        /// <param name="a">The first object</param>
        /// <param name="b">The second object</param>
        /// <returns>the span from the beginning of the first object to the end of the 2nd one</returns>
        public static TextSpan Between(IPositionable a, IPositionable b) => TextSpan.Between(a.Span,b.Span);

        /// <summary>
        /// Calculates the span from the end of the first span to the beginning of the second span.
        /// </summary>
        /// <param name="a">The first span</param>
        /// <param name="b">The second span</param>
        /// <returns>the span from the end of the first span to the beginning of the second span</returns>
        public static TextSpan BetweenNonInclusive(TextSpan a, TextSpan b) => new TextSpan(a.Position + a.Length, b.Position - (a.Position + a.Length));

        /// <summary>
        /// Calculates the span from the end of the first object to the beginning of the second one.
        /// </summary>
        /// <param name="a">The first span</param>
        /// <param name="b">The second span</param>
        /// <returns>the span from the end of the first object to the beginning of the second one</returns>
        public static TextSpan BetweenNonInclusive(IPositionable a, IPositionable b) => TextSpan.BetweenNonInclusive(a.Span, b.Span);

        public static bool TryParse(string? text, out TextSpan? span)
        {
            span = null;

            if (text == null)
            {
                return false;
            }

            var match = TextSpanPattern.Match(text);
            if (match.Success == false)
            {
                return false;
            }

            if (int.TryParse(match.Groups["startInclusive"].Value, out int startInclusive) == false)
            {
                return false;
            }

            if (int.TryParse(match.Groups["endExclusive"].Value, out int endExclusive) == false)
            {
                return false;
            }

            int length = endExclusive - startInclusive;
            if (length < 0)
            {
                return false;
            }

            span = new TextSpan(startInclusive, length);
            return true;
        }
    }
}