// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.Helpers.WhatIf;

internal static class JTokenExtensions
{
    /// <summary>
    /// Checks if a <see cref="JToken"/> is a leaf node.
    /// </summary>
    /// <param name="value">The <see cref="JToken"/> value to check.</param>
    internal static bool IsLeaf(this JToken value)
    {
        return value == null ||
               value is JValue ||
               value is JArray arrayValue && arrayValue.Count == 0 ||
               value is JObject objectValue && objectValue.Count == 0;
    }

    /// <summary>
    /// Checks if a <see cref="JToken"/> is a non empty <see cref="JArray"/>.
    /// </summary>
    /// <param name="value">The <see cref="JToken"/> value to check.</param>
    internal static bool IsNonEmptyArray(this JToken value)
    {
        return value is JArray arrayValue && arrayValue.Count > 0;
    }

    /// <summary>
    /// Checks if a <see cref="JToken"/> is a non empty <see cref="JObject"/>.
    /// </summary>
    /// <param name="value">The <see cref="JToken"/> value to check.</param>
    internal static bool IsNonEmptyObject(this JToken value)
    {
        return value is JObject objectValue && objectValue.Count > 0;
    }
}
