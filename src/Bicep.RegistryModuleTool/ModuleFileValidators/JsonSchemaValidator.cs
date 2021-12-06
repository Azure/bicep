// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.Core.Json;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.Schemas;
using Json.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Bicep.RegistryModuleTool.ModuleFileValidators
{
    internal class JsonSchemaValidator : IModuleFileValidator
    {
        private const string AdditionalPropertiesSchemaLocationSuffix = "/additionalProperties/$false";

        public void Validate(MetadataFile file) => this.Validate(file.Path, JsonSchemaManager.MetadataSchema, file.RootElement);

        public void Validate(MainArmTemplateParametersFile file)
        {

        }

        private void Validate(string filePath, JsonSchema schema, JsonElement element)
        {
            var results = schema.Validate(element, new ValidationOptions
            {
                OutputFormat = OutputFormat.Basic,
            });

            if (!results.IsValid)
            {
                var invalidResults = results.NestedResults;
                var shouldExcludeAdditionalPropertyErrors = invalidResults.Any(x => !IsAdditionalPropertyError(x));

                var errorMessageBuilder = new StringBuilder();

                errorMessageBuilder.AppendLine($"The file \"{filePath}\" is invalid:");

                foreach (var result in invalidResults)
                {
                    if (IsAdditionalPropertyError(result) && shouldExcludeAdditionalPropertyErrors)
                    {
                        continue;
                    }

                    errorMessageBuilder.Append("  ");
                    errorMessageBuilder.Append(result.InstanceLocation.Source);
                    errorMessageBuilder.Append(": ");

                    if (IsAdditionalPropertyError(result))
                    {
                        errorMessageBuilder.Append("The property is not allowed");
                    }
                    else if (IsRegexError(result))
                    {
                        var schemaElement = JsonElementFactory.CreateElement(JsonSerializer.Serialize(schema));
                        var regex = result.SchemaLocation.Evaluate(schemaElement);
                        errorMessageBuilder.Append($"Value does not match the pattern of \"{regex}\"");
                    }
                    else
                    {
                        errorMessageBuilder.Append(result.Message);
                    }

                    errorMessageBuilder.AppendLine();
                }

                throw new BicepException(errorMessageBuilder.ToString());
            }

        }

        private static bool IsAdditionalPropertyError(ValidationResults results) =>
            results.SchemaLocation.Source.EndsWith("/additionalProperties/$false");

        private static bool IsRegexError(ValidationResults results) =>
            results.SchemaLocation.Source.EndsWith("/pattern");
    }
}
