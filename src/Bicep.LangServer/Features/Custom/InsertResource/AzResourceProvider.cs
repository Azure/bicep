// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text.Json;
using Azure.Core;
using Azure.ResourceManager;
using Bicep.Core.AzureApi;
using Bicep.Core.Configuration;
using Bicep.Core.Tracing;

namespace Bicep.LanguageServer.Providers
{
    public class AzResourceProvider : IAzResourceProvider
    {
        private readonly ITokenCredentialFactory credentialFactory;

        public AzResourceProvider(ITokenCredentialFactory credentialFactory)
        {
            this.credentialFactory = credentialFactory;
        }

        private ArmClient CreateArmClient(RootConfiguration configuration, string subscriptionId, IEnumerable<(string resourceType, string apiVersion)> resourceTypeApiVersionMapping)
        {
            var options = new ArmClientOptions();
            options.Diagnostics.ApplySharedResourceManagerSettings();
            options.Environment = new ArmEnvironment(configuration.Cloud.ResourceManagerEndpointUri, configuration.Cloud.AuthenticationScope);
            foreach (var (resourceType, apiVersion) in resourceTypeApiVersionMapping)
            {
                options.SetApiVersion(new ResourceType(resourceType), apiVersion);
            }

            var credential = this.credentialFactory.CreateChain(configuration.Cloud.CredentialPrecedence, configuration.Cloud.CredentialOptions, configuration.Cloud.ActiveDirectoryAuthorityUri);

            return new ArmClient(credential, subscriptionId, options);
        }

        public async Task<JsonElement> GetGenericResource(RootConfiguration configuration, IAzResourceProvider.AzResourceIdentifier resourceId, string? apiVersion, CancellationToken cancellationToken)
        {
            var resourceTypeApiVersionMapping = new List<(string resourceType, string apiVersion)>();
            if (apiVersion is not null)
            {
                // If we have an API version from the Bicep type, use it.
                // Otherwise, the SDK client will attempt to fetch the latest version from the /providers/<provider> API.
                // This is not always guaranteed to work, as child resources are not necessarily declared.
                resourceTypeApiVersionMapping.Add((
                    resourceType: resourceId.FullyQualifiedType,
                    apiVersion: apiVersion));
            }

            var armClient = CreateArmClient(configuration, resourceId.subscriptionId, resourceTypeApiVersionMapping);
            var resourceIdentifier = new ResourceIdentifier(resourceId.FullyQualifiedId);
            var response = await armClient.GetGenericResource(resourceIdentifier).GetAsync(cancellationToken);
            if (response is null ||
                response.GetRawResponse().ContentStream is not { } contentStream)
            {
                throw new Exception($"Failed to fetch resource from Id '{resourceId.FullyQualifiedId}'");
            }

            contentStream.Position = 0;
            return await JsonSerializer.DeserializeAsync<JsonElement>(contentStream);
        }
    }
}
