// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
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
        /// Starts a deployment at provided target scope and returns <see cref="BicepDeploymentStartResponse"/>.
        /// </summary>
        /// <param name="deploymentCollectionProvider">deployment collection provider</param>
        /// <param name="armClient">arm client</param>
        /// <param name="documentPath">path to bicep file used in deployment</param>
        /// <param name="template">template used in deployment</param>
        /// <param name="parametersFilePath">path to parameter file used in deployment</param>
        /// <param name="id">id string to create the ResourceIdentifier from</param>
        /// <param name="scope">target scope</param>
        /// <param name="location">location to store the deployment data</param>
        /// <param name="deploymentId">deployment id</param>
        /// <param name="parametersFileName">parameters file name</param>
        /// <param name="parametersFileUpdateOption"><see cref="ParametersFileUpdateOption"/>update, create or overwrite parameters file</param>
        /// <param name="updatedDeploymentParameters">parameters that were updated during deployment flow</param>
        /// <param name="portalUrl">azure management portal URL</param>
        /// <param name="deploymentName">deployment name</param>
        /// <param name="deploymentOperationsCache">deployment operations cache that needs to be updated</param>
        /// <returns><see cref="BicepDeploymentStartResponse"/></returns>
        public static async Task<BicepDeploymentStartResponse> StartDeploymentAsync(
            IDeploymentCollectionProvider deploymentCollectionProvider,
            ArmClient armClient,
            string documentPath,
            string template,
            string parametersFilePath,
            string id,
            string scope,
            string location,
            string deploymentId,
            string? parametersFileName,
            ParametersFileUpdateOption parametersFileUpdateOption,
            List<BicepUpdatedDeploymentParameter> updatedDeploymentParameters,
            string portalUrl,
            string deploymentName,
            IDeploymentOperationsCache deploymentOperationsCache,
            string? bicepparamJsonOutput)
        {
            if ((scope == LanguageConstants.TargetScopeTypeSubscription ||
                scope == LanguageConstants.TargetScopeTypeManagementGroup) &&
                string.IsNullOrWhiteSpace(location))
            {
                return new BicepDeploymentStartResponse(false, string.Format(LangServerResources.MissingLocationDeploymentFailedMessage, documentPath), null);
            }

            ArmDeploymentCollection? deploymentCollection;
            var resourceIdentifier = new ResourceIdentifier(id);

            try
            {
                deploymentCollection = deploymentCollectionProvider.GetDeploymentCollection(armClient, resourceIdentifier, scope);
            }
            catch (Exception e)
            {
                return new BicepDeploymentStartResponse(false, string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, documentPath, e.Message), null);
            }

            if (deploymentCollection is not null)
            {
                JsonElement parameters;

                try
                {
                    if(parametersFilePath.EndsWith(".bicepparam") && bicepparamJsonOutput is {})
                    {
                        parameters = JsonElementFactory.CreateElement(bicepparamJsonOutput);
                    }
                    else if(parametersFilePath.EndsWith(".json") && parametersFileName is {})
                    {
                        var updatedParametersFileContents = DeploymentParametersHelper.GetUpdatedParametersFileContents(documentPath, parametersFileName, parametersFilePath, parametersFileUpdateOption, updatedDeploymentParameters);
                        parameters = JsonElementFactory.CreateElement(updatedParametersFileContents);
                    }
                    else
                    {
                        return new BicepDeploymentStartResponse(false, $"Parameter file with file extension {Path.GetExtension(parametersFilePath)} is not allowed", null);
                    }

                }
                catch (Exception e)
                {
                    return new BicepDeploymentStartResponse(false, e.Message, null);
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
                        return new BicepDeploymentStartResponse(false, string.Format(LangServerResources.DeploymentFailedMessage, documentPath), null);
                    }

                    deploymentOperationsCache.CacheDeploymentOperation(deploymentId, deploymentOperation);

                    var linkToDeploymentInAzurePortal = GetLinkToDeploymentInAzurePortal(portalUrl, id, deploymentName);

                    return new BicepDeploymentStartResponse(
                        true,
                        string.Format(LangServerResources.DeploymentStartedMessage, documentPath),
                        string.Format(LangServerResources.ViewDeploymentInPortalMessage, linkToDeploymentInAzurePortal));
                }
                catch (Exception e)
                {
                    return new BicepDeploymentStartResponse(false, string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, documentPath, e.Message), null);
                }
            }

            return new BicepDeploymentStartResponse(false, string.Format(LangServerResources.DeploymentFailedMessage, documentPath), null);
        }

        private static string GetLinkToDeploymentInAzurePortal(string portalUrl, string id, string deploymentName)
        {
            id = Uri.EscapeDataString(id);
            return $"{portalUrl}/#blade/HubsExtension/DeploymentDetailsBlade/overview/id/{id}%2Fproviders%2FMicrosoft.Resources%2Fdeployments%2F{deploymentName}";
        }

        /// <summary>
        /// Waits for deployment operation to complete
        /// </summary>
        /// <param name="deploymentId">deployment id</param>
        /// <param name="documentPath">path to bicep file used in deployment</param>
        /// <param name="deploymentOperationsCache"><see cref="IDeploymentOperationsCache"/></param>
        /// <returns><see cref="BicepDeploymentWaitForCompletionResponse"/></returns>
        public async static Task<BicepDeploymentWaitForCompletionResponse> WaitForDeploymentCompletionAsync(string deploymentId, string documentPath, IDeploymentOperationsCache deploymentOperationsCache)
        {
            var deploymentResourceOperation = deploymentOperationsCache.FindAndRemoveDeploymentOperation(deploymentId);

            if (deploymentResourceOperation is null)
            {
                return new BicepDeploymentWaitForCompletionResponse(false, string.Format(LangServerResources.DeploymentFailedMessage, documentPath));
            }

            try
            {
                var response = await deploymentResourceOperation.WaitForCompletionAsync();

                var status = response.GetRawResponse().Status;

                if (status == 200 || status == 201)
                {
                    return new BicepDeploymentWaitForCompletionResponse(true, string.Format(LangServerResources.DeploymentSucceededMessage, documentPath));
                }
                else
                {
                    return new BicepDeploymentWaitForCompletionResponse(false, string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, documentPath, response.ToString()));
                }
            }
            catch (Exception e)
            {
                return new BicepDeploymentWaitForCompletionResponse(false, string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, documentPath, e.Message));
            }
        }
    }
}
