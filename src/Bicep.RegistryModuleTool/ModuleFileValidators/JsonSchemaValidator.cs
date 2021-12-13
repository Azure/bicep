// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.Core.Extensions;
using Bicep.Core.Json;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.Schemas;
using Json.Schema;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Bicep.RegistryModuleTool.ModuleFileValidators
{
    public sealed class JsonSchemaValidator : IModuleFileValidator
    {
        private const string AdditionalPropertiesSchemaLocationSuffix = "/additionalProperties/$false";

        private const string RegexSchemaLocationSuffix = "/pattern";

        private readonly ILogger logger;

        public JsonSchemaValidator(ILogger logger)
        {
            this.logger = logger;
        }

        public void Validate(MetadataFile file) => this.Validate(file.Path, JsonSchemaManager.MetadataSchema, file.RootElement);

        public void Validate(MainArmTemplateParametersFile file) => this.Validate(file.Path, JsonSchemaManager.ArmTemplateParametersSchema, file.RootElement);

        private void Validate(string filePath, JsonSchema schema, JsonElement element)
        {
            this.logger.LogDebug($"Validating \"{filePath}\" against JSON schema...");

            var results = schema.Validate(element, new ValidationOptions
            {
                OutputFormat = OutputFormat.Basic, // Indicates that all nodes will be listed as children of the top node.
                ValidateAs = Draft.Draft7
            });

            if (!results.IsValid)
            {

                var errorMessageBuilder = new StringBuilder();
                errorMessageBuilder.AppendLine($"The file \"{filePath}\" is invalid:");

                var invalidResults = results.NestedResults.Count == 0 ? results.AsEnumerable() : results.NestedResults;
                var shouldSkipAdditionalPropertyError = invalidResults.Any(x => !IsAdditionalPropertyError(x));

                foreach (var result in invalidResults)
                {
                    var isAdditionalPropertyError = IsAdditionalPropertyError(result);

                    if (isAdditionalPropertyError && shouldSkipAdditionalPropertyError)
                    {
                        // According to the JSON schema spec, if any non-additional property is not valid, "properties" doesn't
                        // generate any annotaion. As a result, "additionalProperties": false applies to all properties, which
                        // creates additional property errors even for non-additional properties. To make it less confusing,
                        // we skip additional property errors if there are other error types.
                        // See https://github.com/gregsdennis/json-everything/issues/39#issuecomment-730116851 for details.
                        continue;
                    }

                    errorMessageBuilder.Append("  ");
                    errorMessageBuilder.Append(result.InstanceLocation.Source);
                    errorMessageBuilder.Append(": ");

                    if (IsAdditionalPropertyError(result))
                    {
                        // The built-in error message is "All values fail against the false schema" which is not very intuitive.
                        errorMessageBuilder.Append("The property is not allowed");
                    }
                    else if (IsRegexError(result))
                    {
                        // The built-in error message does not include the regex pattern.
                        var schemaElement = JsonElementFactory.CreateElement(JsonSerializer.Serialize(schema));
                        var regex = result.SchemaLocation.Evaluate(schemaElement);
                        errorMessageBuilder.Append($"Value does not match the pattern of \"{regex}\"");
                    }
                    else
                    {
                        errorMessageBuilder.Append(result.Message);
                    }

                    errorMessageBuilder.AppendLine();

                    if (isAdditionalPropertyError)
                    {
                        // All errors are additional property errors. Only keep the first one as the others could be on the parent
                        // properties which are triggered by the child property, e.g., an additional property /properties/foo/bar
                        // can make /properties/foo fail against the "additionalProperties": false check if it is also declared on
                        // the property foo. Technically this complies with what the spec defines and is what Json.Schema implements,
                        // but it may confuse users.
                        break;
                    }
                }

                throw new BicepException(errorMessageBuilder.ToString());
            }

        }

        private static bool IsAdditionalPropertyError(ValidationResults results) =>
            results.SchemaLocation.Source.EndsWith(AdditionalPropertiesSchemaLocationSuffix);

        private static bool IsRegexError(ValidationResults results) =>
            results.SchemaLocation.Source.EndsWith(RegexSchemaLocationSuffix);
    }
}
