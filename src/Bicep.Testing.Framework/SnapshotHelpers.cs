// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Globalization;
using System.Text.Json;

namespace Bicep.Testing.Framework;

public static class SnapshotHelpers
{
    /// <summary>
    /// Selects resources by type, ignoring case.
    /// </summary>
    public static SnapshotResource[] ResourcesOfType(this SnapshotResult snapshot, string type)
        => [.. snapshot.PredictedResources.Where(resource => string.Equals(resource.Type, type, StringComparison.OrdinalIgnoreCase))];

    /// <summary>
    /// Selects exactly one resource by ID, ignoring case. Throws if no resource or multiple resources match.
    /// </summary>
    public static SnapshotResource RequireResource(this SnapshotResult snapshot, string id)
        => RequireSingle(snapshot.PredictedResources.Where(resource => string.Equals(resource.Id, id, StringComparison.OrdinalIgnoreCase)), id);

    /// <summary>
    /// Selects exactly one resource by case-insensitive type and case-sensitive name.
    /// Throws if no resource or multiple resources match.
    /// </summary>
    public static SnapshotResource RequireResource(this SnapshotResult snapshot, string type, string name)
        => RequireSingle(snapshot.ResourcesOfType(type).Where(resource => resource.Name == name), $"{type}/{name}");

    private static SnapshotResource RequireSingle(IEnumerable<SnapshotResource> resources, string selector)
    {
        var matches = resources.ToArray();
        if (matches.Length != 1)
        {
            throw new InvalidOperationException($"Expected exactly one resource matching '{selector}'; found {matches.Length}.");
        }

        return matches[0];
    }

    /// <summary>
    /// Reads a JSON value using an RFC 6901 JSON Pointer. Throws for invalid pointers or missing values.
    /// </summary>
    public static JsonElement GetSnapshotValue(JsonElement value, string pointer)
    {
        if (pointer == "")
        {
            return value;
        }

        if (!pointer.StartsWith('/'))
        {
            throw new ArgumentException("A JSON Pointer must be empty or start with '/'.", nameof(pointer));
        }

        foreach (var encoded in pointer[1..].Split('/'))
        {
            for (var index = 0; index < encoded.Length; index++)
            {
                if (encoded[index] == '~' && (++index >= encoded.Length || encoded[index] is not ('0' or '1')))
                {
                    throw new ArgumentException($"Invalid JSON Pointer escape: {pointer}", nameof(pointer));
                }
            }

            var key = encoded.Replace("~1", "/").Replace("~0", "~");
            if (value.ValueKind == JsonValueKind.Object && value.TryGetProperty(key, out var property))
            {
                value = property;
            }
            else if (value.ValueKind == JsonValueKind.Array
                && (key == "0" || (key.Length > 0 && key[0] is >= '1' and <= '9'))
                && int.TryParse(key, NumberStyles.None, CultureInfo.InvariantCulture, out var index)
                && index < value.GetArrayLength())
            {
                value = value[index];
            }
            else
            {
                throw new KeyNotFoundException($"JSON Pointer not found: {pointer}");
            }
        }

        return value;
    }
}
