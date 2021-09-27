// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bicep.Core.Parsing
{
    public class TextSpan : IPositionable, IEquatable<TextSpan>
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

        TextSpan IPositionable.Span => this;

        public override string ToString() => $"[{Position}:{Position + Length}]";

        public bool Contains(int offset) => offset >= this.Position && offset < this.Position + this.Length;

        public bool ContainsInclusive(int offset) => offset >= this.Position && offset <= this.Position + this.Length;

        /// <summary>
        /// Calculates the span from the beginning of the first span to the end of the second span.
        /// </summary>
        /// <param name="a">The first span</param>
        /// <param name="b">The second span</param>
        /// <returns>the span from the beginning of the first span to the end of the second span</returns>
        public static TextSpan Between(TextSpan a, TextSpan b)
        {
            if (IsPairInOrder(a, b))
            {
                return new TextSpan(a.Position, b.Position + b.Length - a.Position);
            }

            // the spans are in reverse order - flip them
            return TextSpan.Between(b, a);
        }

        /// <summary>
        /// Calculates the span for a sequence of positionables, returning a 0-length span at a fallback position if the sequence is empty.
        /// </summary>
        public static TextSpan SafeBetween(IEnumerable<IPositionable> positionables, int fallbackPosition)
        {
            return positionables.Any() ?
                TextSpan.Between(positionables.First(), positionables.Last()) :
                new TextSpan(fallbackPosition, 0);
        }

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
        public static TextSpan BetweenExclusive(TextSpan a, TextSpan b)
        {
            if (IsPairInOrder(a, b))
            {
                return new TextSpan(a.Position + a.Length, b.Position - (a.Position + a.Length));
            }

            return TextSpan.BetweenExclusive(b, a);
        }

        /// <summary>
        /// Calculates the span from the end of the first object to the beginning of the second one.
        /// </summary>
        /// <param name="a">The first span</param>
        /// <param name="b">The second span</param>
        /// <returns>the span from the end of the first object to the beginning of the second one</returns>
        public static TextSpan BetweenExclusive(IPositionable a, IPositionable b) => TextSpan.BetweenExclusive(a.Span, b.Span);

        public static TextSpan BetweenInclusiveAndExclusive(IPositionable inclusive, IPositionable exclusive) => TextSpan.BetweenInclusiveAndExclusive(inclusive.Span, exclusive.Span);

        public static TextSpan BetweenInclusiveAndExclusive(TextSpan inclusive, TextSpan exclusive)
        {
            if (IsPairInOrder(inclusive, exclusive))
            {
                return new TextSpan(inclusive.Position, exclusive.Position - inclusive.Position);
            }

            // this operation is not commutative, so we can't just call ourselves with flipped order
            int startPosition = exclusive.Position + exclusive.Length;
            return new TextSpan(startPosition, inclusive.Position + inclusive.Length - startPosition);
        }


        /// <summary>
        /// Checks if the two spans are overlapping.
        /// </summary>
        /// <param name="a">The first span</param>
        /// <param name="b">The second span</param>
        public static bool AreOverlapping(IPositionable a, IPositionable b) => TextSpan.AreOverlapping(a.Span, b.Span);

        /// <summary>
        /// Checks if the two spans are overlapping.
        /// </summary>
        /// <param name="a">The first span</param>
        /// <param name="b">The second span</param>
        public static bool AreOverlapping(TextSpan a, TextSpan b)
        {
            if (a.Length == 0 || b.Length == 0)
            {
                // 0-length spans do not overlap with anything regardless of order
                // in other words, you can have an infinite number of 0-length spans at any position
                return false;
            }

            if (IsPairInOrder(a, b))
            {
                return b.Position >= a.Position && b.Position < a.Position + a.Length;
            }

            return AreOverlapping(b, a);
        }

        /// <summary>
        /// Checks if span of b begins where span of a ends.
        /// </summary>
        /// <param name="a">The first span</param>
        /// <param name="b">The second span</param>
        public static bool AreNeighbors(IPositionable a, IPositionable b) => AreNeighbors(a.Span, b.Span);

        /// <summary>
        /// Checks if span b begins where span a ends.
        /// </summary>
        /// <param name="a">The first span</param>
        /// <param name="b">The second span</param>
        public static bool AreNeighbors(TextSpan a, TextSpan b)
        {
            if (IsPairInOrder(a, b))
            {
                return a.Position + a.Length == b.Position;
            }

            // this method is not pair order agnostic
            return false;
        }

        public static TextSpan Parse(string text)
        {
            if (TryParse(text, out TextSpan? span))
            {
                return span!;
            }

            throw new FormatException($"The specified text span string '{text}' is not valid.");
        }

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

        /// <summary>
        /// Checks if a comes before b.
        /// </summary>
        /// <param name="a">The first span</param>
        /// <param name="b">The second span</param>
        private static bool IsPairInOrder(TextSpan a, TextSpan b) => a.Position <= b.Position;

        /// <summary>
        /// Returns the last non-null <see cref="IPositionable"/> in a sequence.
        /// </summary>
        /// <param name="first">The first non-null positionable</param>
        /// <param name="after">The sequence of nullable positionables</param>
        public static IPositionable LastNonNull(IPositionable first, params IPositionable?[] after)
            => after.LastOrDefault(x => x is not null) ?? first;

        public bool Equals(TextSpan? other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Position == other.Position && Length == other.Length;
        }

        public override bool Equals(object? obj) => Equals(obj as TextSpan);

        public override int GetHashCode()
        {
            // was generated by R# for improved distribution of hashcodes
            // see https://stackoverflow.com/questions/102742/why-is-397-used-for-resharper-gethashcode-override for explanation
            unchecked
            {
                return (Position * 397) ^ Length;
            }
        }
    }
}
