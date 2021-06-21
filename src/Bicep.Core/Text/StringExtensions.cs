// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Bicep.Core.Text
{
    public static class StringExtensions
    {
        public static string RegexEscaped(this string text)
            => text.Replace("]", @"\]")
                .Replace("[", @"\[")
                .Replace(@"\", @"\\")
                .Replace("^", @"\^")
                .Replace(".", @"\.")
                .Replace("|", @"\|")
                .Replace("?", @"\?")
                .Replace("*", @"\*");
    }
}
