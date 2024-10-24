// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;

namespace Bicep.LanguageServer.Deploy
{
    public class CredentialFromTokenAndTimeStamp : TokenCredential
    {
        private AccessToken accessToken;

        public CredentialFromTokenAndTimeStamp(string token, string? timeStamp)
        {
            var expiresOn = timeStamp is string
                ? DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(timeStamp))
                : new DateTimeOffset(DateTime.UtcNow.AddHours(1)); //asdfg long deploy??
            accessToken = new AccessToken(token, expiresOn);
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
