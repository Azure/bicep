// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Json.Schema;
using System;
using System.Reflection;
using System.Text.Json;

namespace Bicep.RegistryModuleTool.Schemas
{
    internal class JsonSchemaManager
    {
        private const string SchemaResourcePrefix = "Bicep.RegistryModuleTool.JsonSchemas";

        private const string ArmTemplateParametersSchemaResourceName = $"{SchemaResourcePrefix}.schema.armTemplateParameters.json";

        private const string MetadataJsonSchemaResourceName = $"{SchemaResourcePrefix}.schema.metadata.json";

        private static readonly Lazy<JsonSchema> LazyArmTemplateParametersSchema = new(() => LoadJsonSchema(ArmTemplateParametersSchemaResourceName));

        private static readonly Lazy<JsonSchema> LazyMetadataSchema = new(() => LoadJsonSchema(MetadataJsonSchemaResourceName));

        public static JsonSchema ArmTemplateParametersSchema => LazyArmTemplateParametersSchema.Value;

        public static JsonSchema MetadataSchema => LazyMetadataSchema.Value;

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
