// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Json;
using Json.Patch;
using Json.Path;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Bicep.Core.Extensions
{
    public static class JsonElementExtensions
    {
        private static readonly JsonSerializerOptions DefaultDeserializeOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

        public static bool IsNotNullValue(this JsonElement element) => element.ValueKind is not JsonValueKind.Null;

        public static string ToNonNullString(this JsonElement element) =>
            element.GetString() ?? throw new JsonException($"Expected \"{element}\" to be non-null.");

        public static T ToNonNullObject<T>(this JsonElement element, JsonSerializerOptions? options = null)
        {
            options ??= DefaultDeserializeOptions;

            return JsonSerializer.Deserialize<T>(element, options) ??
                throw new JsonException($"Expected deserialized value of \"{element}\" to be non-null.");
        }

        public static JsonElement GetPropertyByPath(this JsonElement element, string path) =>
            element.TryGetPropertyByPath(path) ?? throw new InvalidOperationException($"The property \"{path}\" does not exist.");

        public static JsonElement? TryGetPropertyByPath(this JsonElement element, string path)
        {
            var current = element;

            foreach (var propertyName in GetPropertyNames(path))
            {
                if (!current.TryGetProperty(propertyName, out var childElement))
                {
                    return null;
                }

                current = childElement;
            }

            return current;
        }

        public static JsonElement SetPropertyByPath(this JsonElement element, string path, object value)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(bufferWriter))
            {
                foreach (var propertyName in GetPropertyNames(path))
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName(propertyName);
                }

                JsonElementFactory.CreateElement(value).WriteTo(writer);

                foreach (var _ in GetPropertyNames(path))
                {
                    writer.WriteEndObject();
                }
            }

            var valueElement = JsonElementFactory.CreateElement(bufferWriter.WrittenMemory);

            return element.Merge(valueElement);
        }

        public static JsonElement Patch(this JsonElement element, params PatchOperation[] operations)
        {
            var patch = new JsonPatch(operations);
            var patched = patch.Apply(element);

            return patched;
        }

        public static IEnumerable<JsonElement> Select(this JsonElement element, string jsonPathQuery)
        {
            var jsonPath = JsonPath.Parse(jsonPathQuery);
            var result = jsonPath.Evaluate(element);

            if (result.Error is string error)
            {
                throw new InvalidOperationException(error);
            }

            return result.Matches?.Select(match => match.Value) ?? Enumerable.Empty<JsonElement>();
        }

        public static string ToFormattedString(this JsonElement element) => JsonSerializer.Serialize(element, new JsonSerializerOptions
        {
            WriteIndented = true,
        });

        public static JsonElement Merge(this JsonElement element, JsonElement other)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(bufferWriter))
            {
                Merge(element, other, writer);
            }

            return JsonElementFactory.CreateElement(bufferWriter.WrittenMemory);
        }

        private static void Merge(JsonElement first, JsonElement second, Utf8JsonWriter writer)
        {
            switch (first.ValueKind, second.ValueKind)
            {
                case (JsonValueKind.Null, _):
                    second.WriteTo(writer);
                    return;

                case (_, JsonValueKind.Null):
                    first.WriteTo(writer);
                    return;

                case (JsonValueKind.Object, JsonValueKind.Object):
                    writer.WriteStartObject();

                    foreach (var propertyInFirst in first.EnumerateObject())
                    {
                        if (second.TryGetProperty(propertyInFirst.Name, out var properyInSecondValue))
                        {
                            writer.WritePropertyName(propertyInFirst.Name);
                            Merge(propertyInFirst.Value, properyInSecondValue, writer);
                        }
                        else
                        {
                            propertyInFirst.WriteTo(writer);
                        }
                    }

                    foreach (var propertyInSecond in second.EnumerateObject())
                    {
                        if (!first.TryGetProperty(propertyInSecond.Name, out var _))
                        {
                            propertyInSecond.WriteTo(writer);
                        }
                    }

                    writer.WriteEndObject();
                    return;

                default:
                    second.WriteTo(writer);
                    return;
            }
        }

        private static string[] GetPropertyNames(string path) => path.Split('.');
    }
}
