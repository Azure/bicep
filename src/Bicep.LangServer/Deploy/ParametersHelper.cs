// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using Bicep.LanguageServer.Handlers;
using Newtonsoft.Json.Linq;

namespace Bicep.LanguageServer.Deploy
{
    public class ParametersHelper
    {
        public static string GetParametersFileContents(string documentPath, string parametersFilePath, IEnumerable<BicepDeploymentMissingParams> missingParams)
        {
            try
            {
                string text = string.IsNullOrWhiteSpace(parametersFilePath) ?
                    "{}" : File.ReadAllText(parametersFilePath);

                var jObject = JObject.Parse(text);

                foreach (BicepDeploymentMissingParams bicepDeploymentMissingParam in missingParams)
                {
                    var valueObject = JObject.Parse("{}");
                    valueObject.Add("value", bicepDeploymentMissingParam.value);

                    jObject.Add(bicepDeploymentMissingParam.name, JToken.Parse(valueObject.ToString()));
                }

                return jObject.ToString();
            }
            catch (Exception e)
            {
                throw new Exception(string.Format(LangServerResources.InvalidParameterFileDeploymentFailedMessage, documentPath, e.Message));
            }
        }
    }
}
