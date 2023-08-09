// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Xml;

public static class DateTimeExtensions
{
    public static string ToRfc3339Format(this DateTime date)
    {
        XmlConvert.ToString(date, XmlDateTimeSerializationMode.Utc);
        return date.ToUniversalTime().ToString("o");
    }
}