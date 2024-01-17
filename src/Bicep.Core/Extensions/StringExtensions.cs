// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.Extensions;

public static class StringExtensions
{
    public static string Rfc6901Encode(this string toEncode)
        => Uri.EscapeDataString(toEncode.Replace("~", "~0").Replace("/", "~1"));
}
