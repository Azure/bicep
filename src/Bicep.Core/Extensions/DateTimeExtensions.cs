// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Extensions;

public static class DateTimeExtensions
{
    public static string ToRfc3339Format(this DateTime date)
    {
        return date.ToUniversalTime().ToString("o");
    }
}
