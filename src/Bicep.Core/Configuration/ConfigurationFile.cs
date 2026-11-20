// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration;

/// <summary>
/// A parsed Bicep configuration file. <see cref="Root"/> is always a JSON object.
/// </summary>
public sealed record ConfigurationFile(IOUri Uri, JsonElement Root)
{
    public const string ExtendsKey = "extends";

    /// <summary>
    /// The raw "extends" value, or null if the file doesn't set one (or sets it to null).
    /// </summary>
    public JsonElement? Extends =>
        Root.TryGetProperty(ExtendsKey, out var extends) && extends.ValueKind != JsonValueKind.Null ? extends : null;
}
