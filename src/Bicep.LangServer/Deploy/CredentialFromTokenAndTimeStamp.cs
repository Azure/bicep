// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Deploy
{
    public class CredentialFromTokenAndTimeStamp : TokenCredential
    {
        private AccessToken accessToken;

        public CredentialFromTokenAndTimeStamp(string token, string timeStamp)
        {
            accessToken = new AccessToken(token, DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(timeStamp)));
        }

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return ValueTask.FromResult(accessToken);
        }

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return accessToken;
        }
    }
}
