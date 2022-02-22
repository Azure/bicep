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
            ArmClient armClient,
            string template,
            string parameterFilePath,
            string id,
            string scope,
            string location)
        {
            DeploymentCollection? deploymentCollection;
            var resourceIdentifier = new ResourceIdentifier(id);

            if (scope == LanguageConstants.TargetScopeTypeResourceGroup)
            {
                var resourceGroup = armClient.GetResourceGroup(resourceIdentifier);
                deploymentCollection = resourceGroup.GetDeployments();
            }
            else if (scope == LanguageConstants.TargetScopeTypeSubscription)
            {
                var subscription = armClient.GetSubscription(resourceIdentifier);
                deploymentCollection = subscription.GetDeployments();
            }
            else if (scope == LanguageConstants.TargetScopeTypeManagementGroup)
            {
                var managementGroup = armClient.GetManagementGroup(resourceIdentifier);
                deploymentCollection = managementGroup.GetDeployments();
            }
            else if (scope == LanguageConstants.TargetScopeTypeTenant)
            {
                return "Tenant scope deployment is not supported.";
            }
            else
            {
                return "Deployment failed. Please provide a valid scope.";
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
                    string text = File.ReadAllText(parameterFilePath);
                    parameters = JsonDocument.Parse(text).RootElement;
                }

                var input = new DeploymentInput(new DeploymentProperties(DeploymentMode.Incremental)
                {
                    Template = JsonDocument.Parse(template).RootElement,
                    Parameters = parameters
                });

                if (scope == LanguageConstants.TargetScopeTypeSubscription ||
                    scope == LanguageConstants.TargetScopeTypeManagementGroup)
                {
                    if (location is null)
                    {
                        return "Deployment failed. Location was not provided";
                    }
                    input.Location = location;
                }

                string deployment = "deployment_" + DateTime.UtcNow.ToString("yyyyMMddHmmffff");

                try
                {
                    var deploymentCreateOrUpdateAtScopeOperation = await deploymentCollection.CreateOrUpdateAsync(deployment, input);

                    if (deploymentCreateOrUpdateAtScopeOperation.HasValue &&
                        deploymentCreateOrUpdateAtScopeOperation.GetRawResponse().Status == 200)
                    {
                        return "Deployment successful.";
                    }
                }
                catch (Exception e)
                {
                    return "Deployment failed. " + e.Message;
                }
            }

            return "Deployment failed.";
        }
    }
}
