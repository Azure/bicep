// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.ResourceManager;
using Bicep.Core.Tracing;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Configuration;
using System.Threading;

namespace Bicep.LanguageServer.Providers
{
    public class AzResourceProvider : IAzResourceProvider
    {
        private readonly ITokenCredentialFactory credentialFactory;

        public AzResourceProvider(ITokenCredentialFactory credentialFactory)
        {
            this.credentialFactory = credentialFactory;
        }

        private ArmClient CreateArmClient(RootConfiguration configuration, string resourceType, string apiVersion)
        {
            var options = new ArmClientOptions();
            options.Diagnostics.ApplySharedResourceManagerSettings();
            options.ApiVersions.SetApiVersion(resourceType, apiVersion);
            options.Scope = configuration.Cloud.AuthenticationScope;

            var credential = this.credentialFactory.CreateChain(configuration.Cloud.CredentialPrecedence, configuration.Cloud.ActiveDirectoryAuthorityUri);

            return new ArmClient(configuration.Cloud.ResourceManagerEndpointUri, credential, options);
        }

        public async Task<JsonElement> GetGenericResource(RootConfiguration configuration, IAzResourceProvider.AzResourceIdentifier resourceId, string apiVersion, CancellationToken cancellationToken)
        {
            var armClient = CreateArmClient(configuration, resourceId.FullyQualifiedType, apiVersion);

            var response = await armClient.GetGenericResource(resourceId.FullyQualifiedId).GetAsync(cancellationToken);
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
