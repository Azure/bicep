// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Xml;

public static class DateTimeExtensions
{
    public static string ToRFC3339(this DateTime date)
    {
        XmlConvert.ToString(date, XmlDateTimeSerializationMode.Utc);
        return date.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffK");
    }
}