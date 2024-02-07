// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;

namespace Bicep.LanguageServer.Deploy
{
    public class CredentialFromTokenAndTimeStamp(string token, string timeStamp) : TokenCredential
    {
        private AccessToken accessToken = new AccessToken(token, DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(timeStamp)));

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
