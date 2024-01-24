// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

public static class DateTimeExtensions
{
    public static string ToRfc3339Format(this DateTime date)
    {
        return date.ToUniversalTime().ToString("o");
    }
}
