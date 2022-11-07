// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Auth;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Completions;
using Microsoft.Azure.Management.ResourceGraph;
using Microsoft.Azure.Management.ResourceGraph.Models;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using MicrosoftRestTokenCredentials = Microsoft.Rest.TokenCredentials;

namespace Bicep.LanguageServer.Providers
{
    public class AccessTokenProvider : IAccessTokenProvider
    {
        private readonly ITokenCredentialFactory tokenCredentialFactory;
        private readonly IConfigurationManager configurationManager;
        private readonly ConcurrentDictionary<Uri, AccessToken> accessTokens = new();

        public AccessTokenProvider(IConfigurationManager configurationManager, ITokenCredentialFactory tokenCredentialFactory)
        {
            this.configurationManager = configurationManager;
            this.tokenCredentialFactory = tokenCredentialFactory;
        }

        public async Task CacheAccessTokenAsync(Uri uri)
        {
            var configuration = configurationManager.GetConfiguration(uri);
            TokenCredential credential = tokenCredentialFactory.CreateChain(configuration.Cloud.CredentialPrecedence, configuration.Cloud.ActiveDirectoryAuthorityUri);
            var tokenRequestContext = new TokenRequestContext(new[] { configuration.Cloud.ResourceManagerEndpointUri.ToString() });
            AccessToken accessToken = await credential.GetTokenAsync(tokenRequestContext, CancellationToken.None);

            accessTokens.TryAdd(uri, accessToken);
        }

        public async Task<AccessToken> GetAccessTokenAsync(Uri uri)
        {
            if (accessTokens.TryGetValue(uri, out AccessToken accessToken))
            {
                return accessToken;
            }

            var configuration = configurationManager.GetConfiguration(uri);
            TokenCredential credential = tokenCredentialFactory.CreateChain(configuration.Cloud.CredentialPrecedence, configuration.Cloud.ActiveDirectoryAuthorityUri);
            var tokenRequestContext = new TokenRequestContext(new[] { configuration.Cloud.ResourceManagerEndpointUri.ToString() });

            AccessToken newAccessToken = await credential.GetTokenAsync(tokenRequestContext, CancellationToken.None);
            accessTokens.TryAdd(uri, newAccessToken);

            return newAccessToken;
        }
    }
}
