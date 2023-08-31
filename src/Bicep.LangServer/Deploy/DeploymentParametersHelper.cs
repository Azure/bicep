// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Configuration;
using Bicep.LanguageServer.Handlers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bicep.LanguageServer.Deploy
{
    public class DeploymentParametersHelper
    {
        // Per documentation here- https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/parameter-files
        // parameters file should be of below format:
        //{
        //  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
        //  "contentVersion": "1.0.0.0",
        //  "parameters": {
        //    "<first-parameter-name>": {
        //      "value": "<first-value>"
        //    },
        //    "<second-parameter-name>": {
        //      "value": "<second-value>"
        //    }
        //  }
        //}
        // However, azure-sdk-for-net expects parameters to be of name and value pairs:
        // https://github.com/Azure/azure-sdk-for-net/blob/1e25b1bfc9b54df35d907aa7b2c10ff07082e845/sdk/resources/Azure.ResourceManager.Resources/src/Generated/Models/ArmDeploymentProperties.cs#L27
        // We'll work around the above issue by first detecting the format of the file.
        // If it's in the format descibed in the docs, we'll extract the parameters value and use that for actual deployment.
        // If the user chose to create a new parameters file during the deployment flow, we'll follow the format
        // mentioned in the docs as a best practise.
        public static string GetUpdatedParametersFileContents(
                string documentPath,
                string parametersFileName,
                string parametersFilePath,
                ParametersFileUpdateOption updateOrCreateParametersFile,
                IEnumerable<BicepUpdatedDeploymentParameter> updatedDeploymentParameters)
        {
            try
            {
                // Parameter file follows format mentioned here: https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/parameter-files
                var armSchemaStyleParametersFile = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
  }
}";
                // We will send across the secure param values to the azure sdk that handles deployment,
                // but will avoid writing it to the parameters file for security reasons
                var updatedParametersFile = !string.IsNullOrWhiteSpace(parametersFilePath) ?
                    File.ReadAllText(parametersFilePath) : armSchemaStyleParametersFile;
                var updatedParametersFileWithoutSecureParams = updatedParametersFile;

                var jObject = GetParametersObjectValue(updatedParametersFile, out bool isArmStyleTemplate);

                foreach (var updatedDeploymentParameter in updatedDeploymentParameters.Reverse())
                {
                    var name = updatedDeploymentParameter.name;

                    // Check to make sure parameters mentioned in parameters file are not overwritten
                    if (jObject.ContainsKey(name))
                    {
                        continue;
                    }
                    else
                    {
                        var jsonEditor = new JsonEditor(updatedParametersFile);

                        var propertyPaths = new List<string>();
                        if (isArmStyleTemplate)
                        {
                            propertyPaths.Add("parameters");
                            propertyPaths.Add(name);
                        }
                        else
                        {
                            propertyPaths.Add(name);
                        }

                        var valueObject = UpdateJObjectBasedOnParameterType(
                            updatedDeploymentParameter.parameterType,
                            updatedDeploymentParameter.value,
                            JObject.Parse("{}"));

                        (int line, int column, string text)? insertion = jsonEditor.InsertIfNotExist(propertyPaths.ToArray(), valueObject);

                        if (insertion.HasValue)
                        {
                            var (line, column, insertText) = insertion.Value;

                            updatedParametersFile = JsonEditor.ApplyInsertion(updatedParametersFile, (line, column, insertText));

                            if (!updatedDeploymentParameter.isSecure)
                            {
                                updatedParametersFileWithoutSecureParams = JsonEditor.ApplyInsertion(updatedParametersFileWithoutSecureParams, (line, column, insertText));
                            }
                        }
                    }
                }

                if (updatedDeploymentParameters.Any())
                {
                    if (updateOrCreateParametersFile == ParametersFileUpdateOption.Update)
                    {
                        File.WriteAllText(parametersFilePath, updatedParametersFileWithoutSecureParams);
                    }
                    // ParametersFileCreateOrUpdate will have a value of "Overwrite" only if the parameters
                    // file with name <bicep_file_name>.parameters.json already exists and user chose to
                    // overwrite it with values from this deployment
                    else if (updateOrCreateParametersFile == ParametersFileUpdateOption.Create ||
                        updateOrCreateParametersFile == ParametersFileUpdateOption.Overwrite)
                    {
                        var directoryContainingBicepFile = Path.GetDirectoryName(documentPath);
                        if (directoryContainingBicepFile is not null)
                        {
                            File.WriteAllText(Path.Combine(directoryContainingBicepFile, parametersFileName), updatedParametersFileWithoutSecureParams);
                        }
                    }
                }

                var updatedJObject = GetParametersObjectValue(updatedParametersFile, out _);

                return updatedJObject.ToString();
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(LangServerResources.InvalidParameterFileDeploymentFailedMessage, documentPath, parametersFilePath, e.Message));
            }
        }

        public static JObject UpdateJObjectBasedOnParameterType(ParameterType? parameterType, string? value, JObject valueObject)
        {
            if (parameterType is not null)
            {
                if (parameterType == ParameterType.Int &&
                    value is not null &&
                    value.GetType() != typeof(int))
                {
                    var updatedValue = int.Parse(value);
                    valueObject.Add("value", updatedValue);

                    return valueObject;
                }
                else if (parameterType == ParameterType.Bool &&
                    value is not null &&
                    value.GetType() != typeof(bool))
                {
                    var updatedValue = bool.Parse(value);
                    valueObject.Add("value", updatedValue);

                    return valueObject;
                }
            }

            valueObject.Add("value", value);

            return valueObject;
        }

        private static JObject GetParametersObjectValue(string text, out bool isArmStyleTemplate)
        {
            isArmStyleTemplate = false;
            var jObject = JObject.Parse(text);
            if (jObject.ContainsKey("$schema") && jObject.ContainsKey("contentVersion") && jObject.ContainsKey("parameters"))
            {
                isArmStyleTemplate = true;
                var parametersObject = jObject["parameters"];
                if (parametersObject is not null)
                {
                    return JObject.Parse(parametersObject.ToString());
                }
            }

            return jObject;
        }
    }
}
