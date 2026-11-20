// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Buffers;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Bicep.Core.Json;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration;

/// <param name="Root">The merged configuration.</param>
/// <param name="Provenance">Which file declared each value of <paramref name="Root"/>.</param>
public sealed record MergedConfiguration(JsonElement Root, ConfigurationProvenance Provenance);

/// <summary>
/// Merges the built-in configuration and a chain of configuration files into one configuration.
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item>Files are applied from the built-in configuration to the leaf, so a more specific file wins.</item>
/// <item>Objects merge deeply, property by property.</item>
/// <item>Arrays and other values are replaced as a whole.</item>
/// <item>A <c>null</c> value is ignored, so the inherited value stays. A value can be changed, but not removed.</item>
/// <item>The top-level "extends" and "$schema" properties are not merged.</item>
/// </list>
/// </remarks>
public static class ConfigurationMerger
{
    private const string SchemaKey = "$schema";

    /// <param name="builtIn">The built-in configuration.</param>
    /// <param name="files">The configuration files, leaf first.</param>
    public static MergedConfiguration Merge(JsonElement builtIn, IEnumerable<ConfigurationFile> files)
    {
        var root = CreateObject(builtIn);
        var declaringFiles = new Dictionary<ConfigurationPropertyPath, IOUri>();

        foreach (var file in files.Reverse())
        {
            MergeObject(root, file.Root, ConfigurationPropertyPath.Root, file.Uri, declaringFiles);
        }

        return new(ToElement(root), new(declaringFiles.ToImmutableDictionary()));
    }

    private static void MergeObject(JsonObject target, JsonElement source, ConfigurationPropertyPath path, IOUri file, Dictionary<ConfigurationPropertyPath, IOUri> declaringFiles)
    {
        foreach (var property in source.EnumerateObject())
        {
            if (property.Value.ValueKind == JsonValueKind.Null ||
                (path.Count == 0 && property.Name is ConfigurationFile.ExtendsKey or SchemaKey))
            {
                continue;
            }

            var propertyPath = path.Append(property.Name);

            if (property.Value.ValueKind == JsonValueKind.Object && target[property.Name] is JsonObject targetObject)
            {
                MergeObject(targetObject, property.Value, propertyPath, file, declaringFiles);
                continue;
            }

            // The whole value now comes from this file, including anything an earlier file set under it.
            foreach (var key in declaringFiles.Keys.Where(key => key.IsDescendantOf(propertyPath)).ToList())
            {
                declaringFiles.Remove(key);
            }

            declaringFiles[propertyPath] = file;
            target[property.Name] = CreateNode(property.Value);
        }
    }


    private static JsonNode CreateNode(JsonElement value) => value.ValueKind switch
    {
        JsonValueKind.Object => CreateObject(value),
        // Arrays are replaced as a whole, so they're kept exactly as written.
        JsonValueKind.Array => JsonArray.Create(value) ?? throw new UnreachableException(),
        _ => JsonValue.Create(value) ?? throw new UnreachableException(),
    };

    private static JsonObject CreateObject(JsonElement value)
    {
        var result = new JsonObject();

        foreach (var property in value.EnumerateObject())
        {
            if (property.Value.ValueKind != JsonValueKind.Null)
            {
                result[property.Name] = CreateNode(property.Value);
            }
        }

        return result;
    }

    private static JsonElement ToElement(JsonNode node)
    {
        var bufferWriter = new ArrayBufferWriter<byte>();
        using (var writer = new Utf8JsonWriter(bufferWriter))
        {
            node.WriteTo(writer);
        }

        return JsonElementFactory.CreateElement(bufferWriter.WrittenMemory);
    }
}
