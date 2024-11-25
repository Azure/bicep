// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Applies JSON Pointer segment replacements and url-decodes the input string per <see href="https://www.rfc-editor.org/rfc/rfc6901#section-6">RFC 6901, section 6</see>.
    /// </summary>
    /// <param name="toDecode">The unmodified path segment string</param>
    /// <returns>The decoded path segment</returns>
    public static string Rfc6901Decode(this string toDecode)
        => Uri.UnescapeDataString(toDecode.Replace("~1", "/").Replace("~0", "~"));

    /// <summary>
    /// Applies JSON Pointer segment replacements and url-encodes the input string per <see href="https://www.rfc-editor.org/rfc/rfc6901#section-6">RFC 6901, section 6</see>.
    /// </summary>
    /// <param name="toEncode">The unmodified path segment string</param>
    /// <returns>The encoded path segment</returns>
    public static string Rfc6901Encode(this string toEncode)
        => Uri.EscapeDataString(toEncode.Replace("~", "~0").Replace("/", "~1"));

    public static string TruncateWithEllipses(this string input, int maxLength)
    {
        if (maxLength < 4)
        {
            throw new ArgumentException("maxLength must be at least 4");
        }

        if (input.Length > maxLength)
        {
            return input.Substring(0, maxLength - 3) + "...";
        }

        return input;
    }

    public static string UppercaseFirstLetter(this string input)
    {
        if (input.Length > 0)
        {
            return char.ToUpperInvariant(input[0]) + input[1..];
        }
        else
        {
            return input;
        }
    }

}
