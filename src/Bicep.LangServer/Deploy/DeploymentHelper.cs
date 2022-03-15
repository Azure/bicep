// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core;
using Bicep.Core.Json;

namespace Bicep.LanguageServer.Deploy
{
    public class DeploymentHelper
    {
        /// <summary>
        /// Creates a deployment at provided target scope and returns deployment succeeded/failed message.
        /// </summary>
        /// <param name="deploymentCollectionProvider">deployment collection provider</param>
        /// <param name="armClient">arm client</param>
        /// <param name="template">template used in deployment</param>
        /// <param name="parameterFilePath">path to parameter file used in deployment</param>
        /// <param name="id">id string to create the ResourceIdentifier from</param>
        /// <param name="scope">target scope</param>
        /// <param name="location">location to store the deployment data</param>
        /// <returns>deployment succeeded/failed message</returns>
        public static async Task<string> CreateDeployment(
            IDeploymentCollectionProvider deploymentCollectionProvider,
            ArmClient armClient,
            string template,
            string parameterFilePath,
            string id,
            string scope,
            string location)
        {
            if ((scope == LanguageConstants.TargetScopeTypeSubscription ||
                scope == LanguageConstants.TargetScopeTypeManagementGroup) &&
                string.IsNullOrWhiteSpace(location))
            {
                return LangServerResources.MissingLocationDeploymentFailedMessage;
            }

            DeploymentCollection? deploymentCollection;
            var resourceIdentifier = new ResourceIdentifier(id);

            try
            {
                deploymentCollection = deploymentCollectionProvider.GetDeploymentCollection(armClient, resourceIdentifier, scope);
            }
            catch (Exception e)
            {
                return string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, e.Message);
            }

            if (deploymentCollection is not null)
            {
                JsonElement parameters;

                try
                {
                    parameters = GetParameters(parameterFilePath);
                }
                catch (Exception e)
                {
                    return e.Message;
                }

                var deploymentProperties = new DeploymentProperties(DeploymentMode.Incremental)
                {
                    Template = JsonDocument.Parse(template).RootElement,
                    Parameters = parameters
                };
                var input = new DeploymentInput(deploymentProperties)
                {
                    Location = location,
                };

                string deployment = "deployment_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");

                try
                {
                    var deploymentCreateOrUpdateAtScopeOperation = await deploymentCollection.CreateOrUpdateAsync(waitForCompletion:true, deployment, input);

                    if (deploymentCreateOrUpdateAtScopeOperation.HasValue &&
                        deploymentCreateOrUpdateAtScopeOperation.GetRawResponse().Status == 200)
                    {
                        return LangServerResources.DeploymentSucceededMessage;
                    }
                }
                catch (Exception e)
                {
                    return string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, e.Message);
                }
            }

            return LangServerResources.DeploymentFailedMessage;
        }

        private static JsonElement GetParameters(string parameterFilePath)
        {
            if (string.IsNullOrWhiteSpace(parameterFilePath))
            {
                return JsonElementFactory.CreateElement("{}");
            }
            else
            {
                try
                {
                    string text = File.ReadAllText(parameterFilePath);
                    return JsonElementFactory.CreateElement(text);
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format(LangServerResources.InvalidParameterFileDeploymentFailedMessage, e.Message));
                }
            }
        }
    }
}
