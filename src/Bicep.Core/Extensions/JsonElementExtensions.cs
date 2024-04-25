// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Buffers;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bicep.Core.Json;
using Json.Patch;

namespace Bicep.Core.Extensions
{
    public static class JsonElementExtensions
    {
        private static readonly JsonSerializerOptions DefaultDeserializeOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Skip,
            Converters = { new JsonStringEnumConverter() },
        };

        public static bool IsNotNullValue(this JsonElement element) => element.ValueKind is not JsonValueKind.Null;

        public static string ToNonNullString(this JsonElement element) =>
            element.GetString() ?? throw new JsonException($"Expected \"{element}\" to be non-null.");

        [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Relying on references to required properties of the generic type elsewhere in the codebase.")]
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

        [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute'")]
        public static JsonElement Patch(this JsonElement element, params PatchOperation[] operations)
        {
            var patch = new JsonPatch(operations);
            var patched = patch.Apply(element);

            return patched;
        }

        public static string ToIndentedString(this JsonElement element)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using var writer = new Utf8JsonWriter(bufferWriter, new JsonWriterOptions { Indented = true });

            element.WriteTo(writer);

            writer.Flush();

            return Encoding.UTF8.GetString(bufferWriter.WrittenSpan);
        }

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

        public static ImmutableArray<string>? TryGetStringArray(this JsonElement element)
        {
            if (element.ValueKind != JsonValueKind.Array)
            {
                return null;
            }
            return element.EnumerateArray().Select(x => x.GetString()).WhereNotNull().ToImmutableArray();
        }
    }
}
