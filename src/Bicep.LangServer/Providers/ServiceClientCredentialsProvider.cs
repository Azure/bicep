// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Auth;
using Microsoft.Azure.Management.ResourceGraph;

namespace Bicep.LanguageServer.Providers
{
    /// <summary>
    /// Creates <see cref="ClientCredentials"/> that is used for authenticating <see cref="ResourceGraphClient"/> to Azure.
    /// <see cref="AccessToken"/> used for credentials creation is cached and reused as long as it's valid.
    /// New <see cref="AccessToken"/> is fetched on expiration.
    /// </summary>
    public class ServiceClientCredentialsProvider : IServiceClientCredentialsProvider
    {
        private readonly ITokenCredentialFactory tokenCredentialFactory;
        private readonly IConfigurationManager configurationManager;
        private readonly ConcurrentDictionary<Uri, AccessToken> accessTokens = new();

        public ServiceClientCredentialsProvider(IConfigurationManager configurationManager, ITokenCredentialFactory tokenCredentialFactory)
        {
            this.configurationManager = configurationManager;
            this.tokenCredentialFactory = tokenCredentialFactory;
        }

        private async Task<AccessToken> GetAccessTokenAsync(Uri uri)
        {
            RootConfiguration configuration = configurationManager.GetConfiguration(uri);
            TokenCredential credential = tokenCredentialFactory.CreateChain(configuration.Cloud.CredentialPrecedence, configuration.Cloud.ActiveDirectoryAuthorityUri);
            var tokenRequestContext = new TokenRequestContext(new[] { configuration.Cloud.ResourceManagerEndpointUri.ToString() });

            return await credential.GetTokenAsync(tokenRequestContext, CancellationToken.None);
        }

        public async Task CacheAccessTokenAsync(Uri uri)
        {
            AccessToken accessToken = await GetAccessTokenAsync(uri);

            accessTokens.AddOrUpdate(uri, (uri) => accessToken, (uri, accessToken) => accessToken);
        }

        public async Task<ClientCredentials> GetServiceClientCredentials(Uri uri)
        {
            if (accessTokens.TryGetValue(uri, out AccessToken cachedAccessToken) && IsTokenValid(cachedAccessToken))
            {
                return new ClientCredentials(cachedAccessToken.Token);
            }

            AccessToken accessToken = await GetAccessTokenAsync(uri);
            accessTokens.AddOrUpdate(uri, (uri) => accessToken, (uri, accessToken) => accessToken);

            return new ClientCredentials(accessToken.Token);
        }

        private bool IsTokenValid(AccessToken? accessToken)
        {
            return accessToken is not null && accessToken.Value.ExpiresOn > DateTime.UtcNow;
        }
    }
}
