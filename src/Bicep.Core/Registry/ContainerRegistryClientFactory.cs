// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Containers.ContainerRegistry;
using Azure.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Tracing;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.Core.Registry
{
    public class ContainerRegistryClientFactory : IContainerRegistryClientFactory
    {
        private readonly ITokenCredentialFactory credentialFactory;

        public ContainerRegistryClientFactory(ITokenCredentialFactory credentialFactory)
        {
            this.credentialFactory = credentialFactory;
        }

        public ContainerRegistryContentClient CreateAuthenticatedBlobClient(RootConfiguration configuration, Uri registryUri, string repository)
        {
            var options = new ContainerRegistryClientOptions();
            options.Diagnostics.ApplySharedContainerRegistrySettings();
            options.Audience = new ContainerRegistryAudience(configuration.Cloud.ResourceManagerAudience);

            var credential = this.credentialFactory.CreateChain(configuration.Cloud.CredentialPrecedence, configuration.Cloud.ActiveDirectoryAuthorityUri);

            return new ContainerRegistryContentClient(registryUri, repository, credential, options);
        }

        public ContainerRegistryContentClient CreateAnonymousBlobClient(RootConfiguration configuration, Uri registryUri, string repository)
        {
            var options = new ContainerRegistryClientOptions();
            options.Diagnostics.ApplySharedContainerRegistrySettings();
            options.Audience = new ContainerRegistryAudience(configuration.Cloud.ResourceManagerAudience);

            return new ContainerRegistryContentClient(registryUri, repository, options);
        }

        public ContainerRegistryClient CreateContainerRegistryClient(RootConfiguration configuration, Uri registryUri, bool anonymous)
        {
            var options = new ContainerRegistryClientOptions();
            options.Diagnostics.ApplySharedContainerRegistrySettings();
            options.Audience = new ContainerRegistryAudience(configuration.Cloud.ResourceManagerAudience);

            if (anonymous)
            {
                return new ContainerRegistryClient(registryUri, options);
            }
            else
            {
                var credential = this.credentialFactory.CreateChain(configuration.Cloud.CredentialPrecedence, configuration.Cloud.ActiveDirectoryAuthorityUri);
                return new ContainerRegistryClient(registryUri, credential, options);
            }
        }

        public async Task<HttpClient> CreateHttpClientAsdfgAsync(RootConfiguration configuration, /*asdfg?*/bool anonymous) //asdfg
        {
            var credential = this.credentialFactory.CreateChain(configuration.Cloud.CredentialPrecedence, configuration.Cloud.ActiveDirectoryAuthorityUri);
            var httpClient = new HttpClient();

            // Create an instance of TokenRequestContext specifying the scopes
            var tokenRequestContext = new TokenRequestContext(new[] { "https://management.azure.com/.default" }); //asdfg readonly?

            var accessToken = await credential.GetTokenAsync(tokenRequestContext, CancellationToken.None); //cancel?

            // Set the Bearer Token in the Authorization header
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Token); ; // new AuthenticationHeaderValue("Bearer", token.Token);
            return httpClient;

            // Use the authenticated HttpClient for making requests
            //HttpResponseMessage response = await httpClient.GetAsync("https://api.example.com/endpoint");


            //var options = new ContainerRegistryClientOptions();
            //options.Diagnostics.ApplySharedContainerRegistrySettings();
            //options.Audience = new ContainerRegistryAudience(configuration.Cloud.ResourceManagerAudience);

            //// Create an instance of TokenCredentialAdapter, which adapts the DefaultAzureCredential to the TokenCredential interface
            //var tokenCredentialAdapter = new TokenCredentialAdapter(credential);

            //// Create an HttpClient instance with TokenCredentialAdapter
            //HttpClient client = new HttpClient(new BearerTokenAuthenticationHttpMessageHandler(tokenCredentialAdapter));

            //// Use the authenticated HttpClient for making requests

            //if (anonymous)
            //{
            //    return new HttpClient(registryUri, options);
            //}
            //else
            //{
            //    var credential = this.credentialFactory.CreateChain(configuration.Cloud.CredentialPrecedence, configuration.Cloud.ActiveDirectoryAuthorityUri);
            //    return new ContainerRegistryClient(registryUri, credential, options);
            //}
        }

    }
}
