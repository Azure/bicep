// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                string text = !string.IsNullOrWhiteSpace(parametersFilePath) ? File.ReadAllText(parametersFilePath) : "{}";

                var jObject = JObject.Parse(text);

                foreach (var updatedDeploymentParameter in updatedDeploymentParameters)
                {
                    var name = updatedDeploymentParameter.name;
                    var parameterType = updatedDeploymentParameter.parameterType;

                    try
                    {
                        var valueObject = UpdateJObjectBasedOnParameterType(
                            updatedDeploymentParameter.parameterType,
                            name,
                            updatedDeploymentParameter.value,
                            JObject.Parse("{}"));

                        if (jObject.ContainsKey(name))
                        {
                            var nameObject = jObject[name];
                            jObject[name] = valueObject;
                        }
                        else
                        {
                            jObject.Add(updatedDeploymentParameter.name, JToken.Parse(valueObject.ToString()));
                        }
                    }
                    catch (Exception)
                    {
                        throw new Exception(string.Format(LangServerResources.InvalidParameterValueDeploymentFailedMessage, documentPath, name));
                    }
                }

                var updatedParametersFileContents = jObject.ToString();
                if (updatedDeploymentParameters.Any())
                {
                    if (updateOrCreateParametersFile == ParametersFileCreateOrUpdate.Update)
                    {
                        File.WriteAllText(parametersFilePath, updatedParametersFileContents);
                    }
                    else if(updateOrCreateParametersFile == ParametersFileCreateOrUpdate.Create)
                    {
                        var directoryContainingBicepFile = Path.GetDirectoryName(documentPath);
                        if (directoryContainingBicepFile is not null)
                        {
                            File.WriteAllText(Path.Combine(directoryContainingBicepFile, parametersFileName), updatedParametersFileContents);
                        }
                    }
                }

                return updatedParametersFileContents;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(LangServerResources.InvalidParameterFileDeploymentFailedMessage, documentPath, e.Message));
            }
        }

        public static JObject UpdateJObjectBasedOnParameterType(ParameterType? parameterType, string name, string? value, JObject valueObject)
        {
            try
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
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(LangServerResources.InvalidParameterFileDeploymentFailedMessage, "", e.Message));
            }

            valueObject.Add("value", value);

            return valueObject;
        }
    }
}
