// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.Schemas;
using Json.Pointer;
using Json.Schema;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Bicep.RegistryModuleTool.ModuleValidators
{
    public sealed class JsonSchemaValidator : IModuleFileValidator
    {
        private const string AdditionalPropertiesSchemaLocationSuffix = "/additionalProperties";

        private readonly ILogger logger;

        static JsonSchemaValidator()
        {
            ErrorMessages.Pattern = @"Value does not match the pattern of [[pattern]]";
        }

        public JsonSchemaValidator(ILogger logger)
        {
            this.logger = logger;
        }

        public void Validate(MetadataFile file) => this.Validate(file.Path, JsonSchemaManager.MetadataFileSchema, file.RootElement);

        public void Validate(VersionFile file) => this.Validate(file.Path, JsonSchemaManager.VersionFileSchema, file.RootElement);

        private void Validate(string filePath, JsonSchema schema, JsonElement element)
        {
            this.logger.LogInformation("Validating \"{FilePath}\" against JSON schema...", filePath);

            var results = schema.Evaluate(element, new EvaluationOptions
            {
                OutputFormat = OutputFormat.List, // Indicates that all nodes will be listed as children of the top node.
                EvaluateAs = SpecVersion.Draft7,
            });

            if (!results.IsValid)
            {

                var errorMessageBuilder = new StringBuilder();
                errorMessageBuilder.AppendLine($"The file \"{filePath}\" is invalid:");

                IEnumerable<EvaluationResults> invalidResults = results.Details.Count == 0 ? new[] { results } : results.Details;
                invalidResults = invalidResults.Where(x => !x.IsValid && x.HasErrors);

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
                    errorMessageBuilder.Append(result.InstanceLocation.ToString(JsonPointerStyle.UriEncoded));
                    errorMessageBuilder.Append(": ");

                    if (isAdditionalPropertyError)
                    {
                        // The built-in error message is "All values fail against the false schema" which is not very intuitive.
                        errorMessageBuilder.AppendLine("The property is not allowed");

                        // All errors are additional property errors. Only keep the first one as the others could be on the parent
                        // properties which are triggered by the child property, e.g., an additional property /properties/foo/bar
                        // can make /properties/foo fail against the "additionalProperties": false check if it is also declared on
                        // the property foo. Technically this complies with what the spec defines and is what Json.Schema implements,
                        // but it may confuse users.
                        break;
                    }
                    else if (result.Errors is not null)
                    {
                        foreach (var (key, value) in result.Errors)
                        {
                            errorMessageBuilder.AppendLine(value);
                        }
                    }
                }

                throw new InvalidModuleException(errorMessageBuilder.ToString());
            }
        }

        private static bool IsAdditionalPropertyError(EvaluationResults results) =>
            results.EvaluationPath.ToString().EndsWith(AdditionalPropertiesSchemaLocationSuffix);
    }
}
