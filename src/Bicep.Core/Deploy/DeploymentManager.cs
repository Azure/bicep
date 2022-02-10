// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Auth;

namespace Bicep.Core.Deploy
{
    public class DeploymentManager : IDeploymentManager
    {
        private readonly IConfigurationManager configurationManager;
        private readonly ITokenCredentialFactory credentialFactory;

        public DeploymentManager(IConfigurationManager configurationManager, ITokenCredentialFactory credentialFactory)
        {
            this.configurationManager = configurationManager;
            this.credentialFactory = credentialFactory;
        }

        public async Task<string> CreateDeployment(Uri uri, string template, string parameterFilePath, string id, string scope, string location)
        {
            var configuration = configurationManager.GetConfiguration(uri);
            var credential = this.credentialFactory.CreateChain(ImmutableArray.Create(CredentialType.VisualStudioCode), configuration.Cloud.ActiveDirectoryAuthorityUri);

            ArmClient armClient = new ArmClient(credential);
            DeploymentCollection? deploymentCollection = null;
            var resourceIdentifier = new ResourceIdentifier(id);

            if (scope == DeploymentScope.ResourceGroup)
            {
                var resourceGroup = armClient.GetResourceGroup(resourceIdentifier);
                deploymentCollection = resourceGroup.GetDeployments();
            }
            else if (scope == DeploymentScope.Subscription)
            {
                var subscription = armClient.GetSubscription(resourceIdentifier);
                deploymentCollection = subscription.GetDeployments();
            }
            else if (scope == DeploymentScope.ManagementGroup)
            {
                var managementGroup = armClient.GetManagementGroup(resourceIdentifier);
                deploymentCollection = managementGroup.GetDeployments();
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

                if (scope == DeploymentScope.Subscription || scope == DeploymentScope.ManagementGroup)
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
                    return e.Message;
                }
            }

            return "Deployment failed.";
        }

    }

    public static class DeploymentScope
    {
        public const string ManagementGroup = nameof(ManagementGroup);
        public const string ResourceGroup = nameof(ResourceGroup);
        public const string Subscription = nameof(Subscription);
        public const string Tenant = nameof(Tenant);
    }
}
