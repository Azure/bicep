// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
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

        public static JsonElement CreateElement(Stream utf8Json, JsonDocumentOptions? options = null)
        {
            using var document = JsonDocument.Parse(utf8Json, options ?? DefaultJsonDocumentOptions);

            return document.RootElement.Clone();
        }

        public static JsonElement CreateElement(object @object, JsonSerializerOptions? options = null)
        {
            var objectBytes = JsonSerializer.SerializeToUtf8Bytes(@object, options ?? DefaultSerializeOptions);

            return CreateElement(objectBytes);
        }

        public static JsonElement? CreateNullableElement(object? @object, JsonSerializerOptions? options = null) =>
            @object is not null ?  CreateElement(@object, options) : null;
    }
}
