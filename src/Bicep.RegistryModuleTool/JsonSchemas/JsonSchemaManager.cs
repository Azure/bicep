// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Reflection;
using System.Text.Json;
using Json.Schema;

namespace Bicep.RegistryModuleTool.Schemas
{
    internal class JsonSchemaManager
    {
        private const string SchemaResourcePrefix = "Bicep.RegistryModuleTool.JsonSchemas";

        private static readonly Lazy<JsonSchema> LazyMetadataFileSchema = new(() => LoadJsonSchema($"{SchemaResourcePrefix}.schema.metadata.json"));

        private static readonly Lazy<JsonSchema> LazyVersionFileSchema = new(() => LoadJsonSchema($"{SchemaResourcePrefix}.schema.version.json"));

        public static JsonSchema MetadataFileSchema => LazyMetadataFileSchema.Value;

        public static JsonSchema VersionFileSchema => LazyVersionFileSchema.Value;

        private static JsonSchema LoadJsonSchema(string resourceName)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            if (stream is null)
            {
                throw new InvalidOperationException($"Could not get resource stream for {resourceName}.");
            }

            var schema = JsonSerializer.Deserialize<JsonSchema>(stream, new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
            });

            if (schema is null)
            {
                throw new InvalidOperationException($"The json schema resource \"{resourceName}\" is invalid.");
            }

            return schema;
        }
    }
}
