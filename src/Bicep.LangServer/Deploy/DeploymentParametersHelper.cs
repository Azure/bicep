// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bicep.LanguageServer.Configuration;
using Bicep.LanguageServer.Handlers;
using Newtonsoft.Json.Linq;

namespace Bicep.LanguageServer.Deploy
{
    public class DeploymentParametersHelper
    {
        public static string GetUpdatedParametersFileContents(
            string documentPath,
            string parametersFileName,
            string parametersFilePath,
            ParametersFileCreateOrUpdate updateOrCreateParametersFile,
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
                // We will send across the secure param values to the sdk that handles deployment,
                // but will avoid writing it to the parameters file for security reasons
                var updatedParametersFile = !string.IsNullOrWhiteSpace(parametersFilePath) ? File.ReadAllText(parametersFilePath) : armSchemaStyleParametersFile;
                var updatedParametersFileWithoutSecureParams = updatedParametersFile;

                var jObject = GetParametersObjectValue(updatedParametersFile, out bool isArmStyleTemplate);

                foreach (var updatedDeploymentParameter in updatedDeploymentParameters)
                {
                    var name = updatedDeploymentParameter.name;
                    var parameterType = updatedDeploymentParameter.parameterType;
                    var valueObject = UpdateJObjectBasedOnParameterType(
                        updatedDeploymentParameter.parameterType,
                        updatedDeploymentParameter.value,
                        JObject.Parse("{}"));

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
                            propertyPaths.Add(updatedDeploymentParameter.name);
                        }
                        else
                        {
                            propertyPaths.Add(updatedDeploymentParameter.name);
                        }

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
                    if (updateOrCreateParametersFile == ParametersFileCreateOrUpdate.Update)
                    {
                        File.WriteAllText(parametersFilePath, updatedParametersFileWithoutSecureParams);
                    }
                    else if (updateOrCreateParametersFile == ParametersFileCreateOrUpdate.Create)
                    {
                        var directoryContainingBicepFile = Path.GetDirectoryName(documentPath);
                        if (directoryContainingBicepFile is not null)
                        {
                            File.WriteAllText(Path.Combine(directoryContainingBicepFile, parametersFileName), updatedParametersFileWithoutSecureParams);
                        }
                    }
                }

                return updatedParametersFile;
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
