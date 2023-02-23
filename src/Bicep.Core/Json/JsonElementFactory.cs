// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;

namespace Bicep.Core.Json
{
    public static class JsonElementFactory
    {
        private static readonly JsonDocumentOptions DefaultJsonDocumentOptions = new()
        {
            CommentHandling = JsonCommentHandling.Skip,
        };

        private static readonly JsonSerializerOptions DefaultSerializeOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public static JsonElement CreateElement(ReadOnlyMemory<byte> utf8Json, JsonDocumentOptions? options = null)
        {
            using var document = JsonDocument.Parse(utf8Json, options ?? DefaultJsonDocumentOptions);

            // JsonDocument is IDisposable, so we need to clone RootElement.
            // See: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-migrate-from-newtonsoft-how-to?pivots=dotnet-5-0#jsondocument-is-idisposable.
            return document.RootElement.Clone();
        }

        public static JsonElement CreateElementFromStream(Stream utf8Json, JsonDocumentOptions? options = null)
        {
            using var document = JsonDocument.Parse(utf8Json, options ?? DefaultJsonDocumentOptions);

            return document.RootElement.Clone();
        }

        public static JsonElement CreateElement(string utf8Json, JsonDocumentOptions? options = null)
        {
            using var document = JsonDocument.Parse(utf8Json, options ?? DefaultJsonDocumentOptions);

            return document.RootElement.Clone();
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Relying on references to required properties of the generic type elsewhere in the codebase.")]
        public static JsonElement CreateElement<T>(T value, JsonSerializerOptions? options = null)
        {
            if (value is JsonElement element)
            {
                return element;
            }

            var bytes = JsonSerializer.SerializeToUtf8Bytes(value, options ?? DefaultSerializeOptions);

            return CreateElement((ReadOnlyMemory<byte>)bytes);
        }

        public static JsonElement? CreateNullableElement<T>(T? value, JsonSerializerOptions? options = null) =>
            value is not null ? CreateElement(value, options) : null;
    }
}
