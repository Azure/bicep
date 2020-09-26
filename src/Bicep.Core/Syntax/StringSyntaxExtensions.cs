// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Linq;

namespace Bicep.Core.Syntax
{
    public static class StringSyntaxExtensions
    {
        /// <summary>
        /// Checks if the syntax node contains an interpolated string or a literal string.
        /// </summary>
        /// <param name="syntax">The string syntax node</param>
        public static bool IsInterpolated(this StringSyntax syntax)
            => syntax.SegmentValues.Length > 1;

        /// <summary>
        /// Try to get the string literal value for a syntax node. Returns null if the string is interpolated.
        /// </summary>
        /// <param name="syntax">The string syntax node</param>
        public static string? TryGetLiteralValue(this StringSyntax syntax)
            => syntax.IsInterpolated() ? null : syntax.SegmentValues[0];
    }
}