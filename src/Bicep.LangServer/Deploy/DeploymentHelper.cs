// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core;
using Bicep.Core.Json;
using Bicep.LanguageServer.Handlers;

namespace Bicep.LanguageServer.Deploy
{
    public class DeploymentHelper
    {
        /// <summary>
        /// Starts a deployment at provided target scope and returns <see cref="BicepDeployStartResponse"/>.
        /// </summary>
        /// <param name="deploymentCollectionProvider">deployment collection provider</param>
        /// <param name="armClient">arm client</param>
        /// <param name="documentPath">path to bicep file used in deployment</param>
        /// <param name="template">template used in deployment</param>
        /// <param name="parameterFilePath">path to parameter file used in deployment</param>
        /// <param name="id">id string to create the ResourceIdentifier from</param>
        /// <param name="scope">target scope</param>
        /// <param name="location">location to store the deployment data</param>
        /// <param name="deploymentId">deployment id</param>
        /// <param name="portalUrl">azure management portal URL</param>
        /// <param name="deploymentName">deployment name</param>
        /// <param name="deploymentOperationsCache">deployment operations cache that needs to be updated</param>
        /// <returns><see cref="BicepDeployStartResponse"/></returns>
        public static async Task<BicepDeployStartResponse> StartDeploymentAsync(
            IDeploymentCollectionProvider deploymentCollectionProvider,
            ArmClient armClient,
            string documentPath,
            string template,
            string parameterFilePath,
            string id,
            string scope,
            string location,
            string deploymentId,
            string portalUrl,
            string deploymentName,
            IDeploymentOperationsCache deploymentOperationsCache)
        {
            if ((scope == LanguageConstants.TargetScopeTypeSubscription ||
                scope == LanguageConstants.TargetScopeTypeManagementGroup) &&
                string.IsNullOrWhiteSpace(location))
            {
                return new BicepDeployStartResponse(false, string.Format(LangServerResources.MissingLocationDeploymentFailedMessage, documentPath), null);
            }

            ArmDeploymentCollection? deploymentCollection;
            var resourceIdentifier = new ResourceIdentifier(id);

            try
            {
                deploymentCollection = deploymentCollectionProvider.GetDeploymentCollection(armClient, resourceIdentifier, scope);
            }
            catch (Exception e)
            {
                return new BicepDeployStartResponse(false, string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, documentPath, e.Message), null);
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
                    return new BicepDeployStartResponse(false, e.Message, null);
                }

                var deploymentProperties = new ArmDeploymentProperties(ArmDeploymentMode.Incremental)
                {
                    Template = new BinaryData(JsonDocument.Parse(template).RootElement),
                    Parameters = new BinaryData(parameters)
                };
                var armDeploymentContent = new ArmDeploymentContent(deploymentProperties)
                {
                    Location = location,
                };

                try
                {
                    var deploymentOperation = await deploymentCollection.CreateOrUpdateAsync(WaitUntil.Started, deploymentName, armDeploymentContent);

                    if (deploymentOperation is null)
                    {
                        return new BicepDeployStartResponse(false, string.Format(LangServerResources.DeploymentFailedMessage, documentPath), null);
                    }

                    deploymentOperationsCache.CacheDeploymentOperation(deploymentId, deploymentOperation);

                    var linkToDeploymentInAzurePortal = GetLinkToDeploymentInAzurePortal(portalUrl, Uri.EscapeDataString(id), deploymentName);

                    return new BicepDeployStartResponse(
                        true,
                        string.Format(LangServerResources.DeploymentStartedMessage, documentPath),
                        string.Format(LangServerResources.ViewDeploymentInPortalMessage, linkToDeploymentInAzurePortal));
                }
                catch (Exception e)
                {
                    return new BicepDeployStartResponse(false, string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, documentPath, e.Message), null);
                }
            }

            return new BicepDeployStartResponse(false, string.Format(LangServerResources.DeploymentFailedMessage, documentPath), null);
        }

        private static string GetLinkToDeploymentInAzurePortal(string portalUrl, string id, string deploymentName)
        {
            return $"{portalUrl}/#blade/HubsExtension/DeploymentDetailsBlade/overview/id/{id}%2Fproviders%2FMicrosoft.Resources%2Fdeployments%2F{deploymentName}";
        }

        public async static Task<BicepDeployWaitForCompletionResponse> WaitForDeploymentCompletionAsync(string deploymentId, string documentPath, IDeploymentOperationsCache deploymentOperationsCache)
        {
            var deploymentResourceOperation = deploymentOperationsCache.GetDeploymentOperation(deploymentId);

            if (deploymentResourceOperation is null)
            {
                return new BicepDeployWaitForCompletionResponse(false, string.Format(LangServerResources.DeploymentFailedMessage, documentPath));
            }

            var response = await deploymentResourceOperation.WaitForCompletionAsync();

            var status = response.GetRawResponse().Status;

            if (status == 200 || status == 201)
            {
                return new BicepDeployWaitForCompletionResponse(true, string.Format(LangServerResources.DeploymentSucceededMessage, documentPath));
            }
            else
            {
                return new BicepDeployWaitForCompletionResponse(false, string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, documentPath, response.ToString()));
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
