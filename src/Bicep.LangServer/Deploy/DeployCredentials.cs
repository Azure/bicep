// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;

namespace Bicep.LanguageServer.Deploy
{
    public class DeployCredentials : TokenCredential
    {
        private AccessToken _accessToken;

        public DeployCredentials(string token, string timeStamp)
        {
            _accessToken = new AccessToken(token, DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(timeStamp)));
        }

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return ValueTask.FromResult(_accessToken);
        }

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return _accessToken;
        }
    }
}
