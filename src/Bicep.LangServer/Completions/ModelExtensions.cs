// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Completions
{
    /// <summary>
    /// Class that implements extension methods for OmniSharp classes.
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// Minimum position value.
        /// </summary>
        public static readonly Position MinPosition = new Position(0, 0);

        /// <summary>
        /// Value indicating no position.
        /// </summary>
        public static readonly Position EmptyPosition = new Position(-1, -1);

        /// <summary>
        /// predicate indicating the position doesn't have a value.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>False if the position has a value, true otherwise.</returns>
        public static bool IsEmpty(this Position position) => position == null || (position.Character <= -1 && position.Line <= -1);

        /// <summary>
        /// predicate indicating the range doesn't have a value.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns>False if the range has a value, true otherwise.</returns>
        public static bool IsEmpty(this Range range) => range == null || range.Start.IsEmpty() || range.End.IsEmpty() || range.Start.GreaterOrEqual(range.End);

        /// <summary>
        /// Compare two positions.
        /// </summary>
        /// <param name="leftPosition">First position.</param>
        /// <param name="rightPosition">Second position.</param>
        /// <returns>True if the first position is greater then or equal to the second.</returns>
        public static bool GreaterOrEqual(this Position leftPosition, Position rightPosition)
        {
            if (leftPosition == null || leftPosition.IsEmpty() || rightPosition == null || rightPosition.IsEmpty())
            {
                return false;
            }

            return leftPosition.Line > rightPosition.Line || (leftPosition.Line == rightPosition.Line && leftPosition.Character >= rightPosition.Character);
        }

        /// <summary>
        /// Predicate indicating the given position lies within the range.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="position">The position.</param>
        /// <returns>True if the position lies within the range.</returns>
        public static bool Contains(this Range range, Position position)
        {
            return range != null && !range.IsEmpty() && position.GreaterOrEqual(range.Start) && range.End.GreaterOrEqual(position);
        }
    }
}
