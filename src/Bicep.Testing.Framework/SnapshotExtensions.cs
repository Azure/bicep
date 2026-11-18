// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Globalization;
using System.Text.Json;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Testing.Framework;

/// <summary>
/// Resource selection and strict JSON Pointer accessors for snapshot assertions.
/// </summary>
public static class SnapshotExtensions
{
    /// <summary>
    /// Selects resources with resolved, parseable ARM resource IDs.
    /// </summary>
    public static IEnumerable<ArmSnapshotResource> GetArmResources(this SnapshotResult snapshot)
        => snapshot.PredictedResources
            .OfType<ArmSnapshotResource>();

    /// <summary>
    /// Selects ARM resources by fully qualified resource type, ignoring case.
    /// </summary>
    public static IEnumerable<ArmSnapshotResource> GetArmResourceOfType(this SnapshotResult snapshot, string type)
        => GetArmResources(snapshot).Where(x => x.Type.EqualsOrdinalInsensitively(type));

    /// <summary>
    /// Reads a JSON value using a case-sensitive RFC 6901 JSON Pointer.
    /// An empty pointer returns the resource body.
    /// </summary>
    /// <exception cref="ArgumentException">The pointer is malformed.</exception>
    /// <exception cref="KeyNotFoundException">The path does not exist.</exception>
    public static JsonElement GetProperty(this SnapshotResource resource, string pointer)
    {
        var result = resource.Body;
        if (pointer == "")
        {
            return result;
        }

        if (!pointer.StartsWith('/'))
        {
            throw new ArgumentException("A JSON Pointer must be empty or start with '/'.", nameof(pointer));
        }

        var segments = pointer[1..].Split('/');
        foreach (var encoded in segments)
        {
            for (var index = 0; index < encoded.Length; index++)
            {
                if (encoded[index] == '~' && (++index >= encoded.Length || encoded[index] is not ('0' or '1')))
                {
                    throw new ArgumentException($"Invalid JSON Pointer escape: {pointer}", nameof(pointer));
                }
            }
        }

        foreach (var encoded in segments)
        {
            var key = encoded.Replace("~1", "/").Replace("~0", "~");
            if (result.ValueKind == JsonValueKind.Object && result.TryGetProperty(key, out var property))
            {
                result = property;
            }
            else if (result.ValueKind == JsonValueKind.Array
                && (key == "0" || (key.Length > 0 && key[0] is >= '1' and <= '9'))
                && int.TryParse(key, NumberStyles.None, CultureInfo.InvariantCulture, out var index)
                && index < result.GetArrayLength())
            {
                result = result[index];
            }
            else
            {
                throw new KeyNotFoundException($"JSON Pointer not found: {pointer}");
            }
        }

        return result;
    }

    /// <summary>
    /// Reads a string, returning null for JSON null. Missing paths, invalid pointers, and other JSON types throw.
    /// </summary>
    public static string? GetStringProperty(this SnapshotResource resource, string pointer)
        => resource.GetProperty(pointer).GetString();

    /// <summary>
    /// Reads a non-null string. Throws for JSON null, other JSON types, missing paths, or invalid pointers.
    /// </summary>
    public static string GetRequiredStringProperty(this SnapshotResource resource, string pointer)
        => resource.GetStringProperty(pointer) ?? throw new JsonException($"Expected the value at JSON Pointer '{pointer}' to be a non-null string.");

    /// <summary>
    /// Reads a 32-bit integer, returning null for JSON null.
    /// Missing paths, invalid pointers, other JSON types, fractional values, and out-of-range numbers throw.
    /// </summary>
    public static int? GetIntProperty(this SnapshotResource resource, string pointer)
        => resource.GetProperty(pointer) is { ValueKind: not JsonValueKind.Null } element
            ? element.GetInt32()
            : null;

    /// <summary>
    /// Reads a non-null 32-bit integer. Throws for JSON null or any value rejected by <see cref="GetIntProperty"/>.
    /// </summary>
    public static int GetRequiredIntProperty(this SnapshotResource resource, string pointer)
        => resource.GetIntProperty(pointer) ?? throw new JsonException($"Expected the value at JSON Pointer '{pointer}' to be a non-null integer.");

    /// <summary>
    /// Reads a boolean, returning null for JSON null. Missing paths, invalid pointers, and other JSON types throw.
    /// </summary>
    public static bool? GetBoolProperty(this SnapshotResource resource, string pointer)
        => resource.GetProperty(pointer) is { ValueKind: not JsonValueKind.Null } element
            ? element.GetBoolean()
            : null;

    /// <summary>
    /// Reads a non-null boolean. Throws for JSON null, other JSON types, missing paths, or invalid pointers.
    /// </summary>
    public static bool GetRequiredBoolProperty(this SnapshotResource resource, string pointer)
        => resource.GetBoolProperty(pointer) ?? throw new JsonException($"Expected the value at JSON Pointer '{pointer}' to be a non-null boolean.");
}
