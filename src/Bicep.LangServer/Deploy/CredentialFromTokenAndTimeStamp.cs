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
            // VSCode does not currently provide the expiration time we can provide here. What we provide here
            //   doesn't really matter, so we just provide a time that is 1 hour in the future if not available.
            //   We're expected to not keep the token around for long anyway, so that VS Code can handle refreshes
            //   when a new one is requested by us.
            var expiresOn = timeStamp is string
                ? DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(timeStamp))
                : new DateTimeOffset(DateTime.UtcNow.AddHours(1));
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
