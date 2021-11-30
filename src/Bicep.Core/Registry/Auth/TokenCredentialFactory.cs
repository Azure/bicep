// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Azure.Identity;
using Bicep.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Registry.Auth
{
    public class TokenCredentialFactory : ITokenCredentialFactory
    {
        public TokenCredential CreateChain(IEnumerable<CredentialType> credentialPrecedence, Uri authorityUri)
        {
            if (!credentialPrecedence.Any())
            {
                throw new ArgumentException("At least one credential type must be provided.");
            }

            var tokenCredentials = credentialPrecedence.Select(credentialType => CreateSingle(credentialType, authorityUri)).ToArray();

            return new ChainedTokenCredential(tokenCredentials);
        }

        public TokenCredential CreateSingle(CredentialType credentialType, Uri authorityUri) =>
            credentialType switch
            {
                CredentialType.Environment => new EnvironmentCredential(new() { AuthorityHost = authorityUri }),
                CredentialType.ManagedIdentity => new ManagedIdentityCredential(options: new() { AuthorityHost = authorityUri }),
                CredentialType.VisualStudio => new VisualStudioCredential(new() { AuthorityHost = authorityUri }),
                CredentialType.VisualStudioCode => new VisualStudioCodeCredential(new() { AuthorityHost = authorityUri }),
                // AzureCLICrediential does not accept options. Azure CLI has built-in cloud profiles so AuthorityHost is not needed.
                CredentialType.AzureCLI => new AzureCliCredential(),
                CredentialType.AzurePowerShell => new AzurePowerShellCredential(new() { AuthorityHost = authorityUri }),

                _ => throw new NotImplementedException($"Unexpected credential type '{credentialType}'.")
            };
    }
}
