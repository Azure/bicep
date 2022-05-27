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
            bool parametersFileExists,
            bool shouldUpdateOrCreateParametersFile,
            IEnumerable<BicepUpdatedDeploymentParameter> updatedDeploymentParameters)
        {
            try
            {
                string text = parametersFileExists ? File.ReadAllText(parametersFilePath) : "{}";
                var jObject = JObject.Parse(text);

                foreach (var updatedDeploymentParameter in updatedDeploymentParameters)
                {
                    var name = updatedDeploymentParameter.name;
                    if (jObject.ContainsKey(name))
                    {
                        var nameObject = jObject[name];
                        var valueObject = JObject.Parse("{}");
                        valueObject.Add("value", updatedDeploymentParameter.value);
                        jObject[name] = valueObject;
                    }
                    else
                    {
                        var valueObject = JObject.Parse("{}");
                        valueObject.Add("value", updatedDeploymentParameter.value);
                        jObject.Add(updatedDeploymentParameter.name, JToken.Parse(valueObject.ToString()));
                    }
                }

                var updatedParametersFileContents = jObject.ToString();
                if (updatedDeploymentParameters.Any() && shouldUpdateOrCreateParametersFile)
                {
                    if (parametersFileExists)
                    {
                        File.WriteAllText(parametersFilePath, updatedParametersFileContents);
                    }
                    else
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
    }
}
