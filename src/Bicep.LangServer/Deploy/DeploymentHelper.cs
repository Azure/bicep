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
        /// <param name="documentPath">path to bicep file used in deployment</param>
        /// <param name="template">template used in deployment</param>
        /// <param name="parameterFilePath">path to parameter file used in deployment</param>
        /// <param name="id">id string to create the ResourceIdentifier from</param>
        /// <param name="scope">target scope</param>
        /// <param name="location">location to store the deployment data</param>
        /// <returns>deployment result and succeeded/failed message </returns>
        public static async Task<(bool isSuccess, string outputMessage)> CreateDeployment(
            IDeploymentCollectionProvider deploymentCollectionProvider,
            ArmClient armClient,
            string documentPath,
            string template,
            string parameterFilePath,
            string id,
            string scope,
            string location,
            string portalUrl)
        {
            if ((scope == LanguageConstants.TargetScopeTypeSubscription ||
                scope == LanguageConstants.TargetScopeTypeManagementGroup) &&
                string.IsNullOrWhiteSpace(location))
            {
                return (false, string.Format(LangServerResources.MissingLocationDeploymentFailedMessage, documentPath));
            }

            DeploymentCollection? deploymentCollection;
            var resourceIdentifier = new ResourceIdentifier(id);

            try
            {
                deploymentCollection = deploymentCollectionProvider.GetDeploymentCollection(armClient, resourceIdentifier, scope);
            }
            catch (Exception e)
            {
                return (false, string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, documentPath, e.Message));
            }

            if (deploymentCollection is not null)
            {
                JsonElement parameters;

                try
                {
                    parameters = GetParameters(documentPath, parameterFilePath);
                }
                catch (Exception e)
                {
                    return (false, e.Message);
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

                string deploymentName = "bicep_deployment_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");

                try
                {
                    var deploymentCreateOrUpdateOperation = await deploymentCollection.CreateOrUpdateAsync(waitForCompletion:true, deploymentName, input);

                    var linkToDeploymentInAzurePortal = GetLinkToDeploymentInAzurePortal(portalUrl, Uri.EscapeDataString(id), deploymentName);

                    return GetDeploymentResultMessage(deploymentCreateOrUpdateOperation, documentPath, linkToDeploymentInAzurePortal);
                }
                catch (Exception e)
                {
                    return (false, string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, documentPath, e.Message));
                }
            }

            return (false, string.Format(LangServerResources.DeploymentFailedMessage, documentPath));
        }

        private static string GetLinkToDeploymentInAzurePortal(string portalUrl, string id, string deploymentName)
        {
            return $"{portalUrl}/#blade/HubsExtension/DeploymentDetailsBlade/overview/id/{id}%2Fproviders%2FMicrosoft.Resources%2Fdeployments%2F{deploymentName}";
        }

        private static (bool isSuccess, string outputMessage) GetDeploymentResultMessage(DeploymentCreateOrUpdateOperation deploymentCreateOrUpdateOperation, string documentPath, string linkToDeploymentInAzurePortal)
        {
            if (!deploymentCreateOrUpdateOperation.HasValue)
            {
                return (false, string.Format(LangServerResources.DeploymentFailedMessage, documentPath));
            }

            var response = deploymentCreateOrUpdateOperation.GetRawResponse();
            var status = response.Status;

            if (status == 200 || status == 201)
            {
                var outputMessage = string.Format(LangServerResources.DeploymentSucceededMessage, documentPath, linkToDeploymentInAzurePortal);
                return (true, outputMessage);
            }
            else
            {
                return (false, string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, documentPath, response.ToString()));
            }
        }

        private static JsonElement GetParameters(string documentPath, string parameterFilePath)
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
                    throw new Exception(string.Format(LangServerResources.InvalidParameterFileDeploymentFailedMessage, documentPath, e.Message));
                }
            }
        }
    }
}
