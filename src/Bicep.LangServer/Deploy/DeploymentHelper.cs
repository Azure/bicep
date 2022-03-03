// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core;

namespace Bicep.LanguageServer.Deploy
{
    public class DeploymentHelper
    {
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

            if (scope == LanguageConstants.TargetScopeTypeResourceGroup ||
                scope == LanguageConstants.TargetScopeTypeSubscription ||
                scope == LanguageConstants.TargetScopeTypeManagementGroup)
            {
                try
                {
                    deploymentCollection = deploymentCollectionProvider.GetDeploymentCollection(armClient, resourceIdentifier, scope);
                }
                catch (Exception e)
                {
                    return string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, e.Message);
                }
            }
            else
            {
                return LangServerResources.UnsupportedScopeDeploymentFailedMessage;
            }

            if (deploymentCollection is not null)
            {
                JsonElement parameters;

                if (string.IsNullOrWhiteSpace(parameterFilePath))
                {
                    parameters = JsonDocument.Parse("{}").RootElement;
                }
                else
                {
                    try
                    {
                        string text = File.ReadAllText(parameterFilePath);
                        parameters = JsonDocument.Parse(text).RootElement;
                    }
                    catch(Exception e)
                    {
                        return string.Format(LangServerResources.InvalidParameterFileDeploymentFailedMessage, e.Message);
                    }
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

                string deployment = "deployment_" + DateTime.UtcNow.ToString("yyyyMMddHmmffff");

                try
                {
                    var deploymentCreateOrUpdateAtScopeOperation = await deploymentCollection.CreateOrUpdateAsync(deployment, input);

                    if (deploymentCreateOrUpdateAtScopeOperation.HasValue &&
                        deploymentCreateOrUpdateAtScopeOperation.GetRawResponse().Status == 200)
                    {
                        return LangServerResources.DeploymentSuccessfulMessage;
                    }
                }
                catch (Exception e)
                {
                    var errorMessage = e.Message;
                    var indexOfHeader = errorMessage.IndexOf("Headers:");

                    if (indexOfHeader > 0)
                    {
                        errorMessage = errorMessage.Substring(0, indexOfHeader).TrimEnd();
                    }

                    return string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, errorMessage);
                }
            }

            return LangServerResources.DeploymentFailedMessage;
        }
    }
}
