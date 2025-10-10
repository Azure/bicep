// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Azure.Identity;
using Bicep.Core.Configuration;

namespace Bicep.Core.AzureApi
{
    public class TokenCredentialFactory : ITokenCredentialFactory
    {
        public TokenCredential CreateChain(IEnumerable<CredentialType> credentialPrecedence, CredentialOptions? credentialOptions, Uri authorityUri)
        {
            var tokenCredentials = credentialPrecedence.Select(credentialType => CreateSingle(credentialType, credentialOptions, authorityUri)).ToArray();

            return tokenCredentials.Length == 0
                ? throw new ArgumentException("At least one credential type must be provided.")
                : new ChainedTokenCredential(tokenCredentials);
        }

        public TokenCredential CreateSingle(CredentialType credentialType, CredentialOptions? credentialOptions, Uri authorityUri)
        {
            if (credentialType is CredentialType.ManagedIdentity)
            {
                var managedIdentityType = credentialOptions?.ManagedIdentity?.Type ?? ManagedIdentityType.SystemAssigned;

                if (managedIdentityType is ManagedIdentityType.SystemAssigned)
                {
                    return new ManagedIdentityCredential(options: new() { AuthorityHost = authorityUri });
                }

                // Handle user-assigned identity.
                if (credentialOptions?.ManagedIdentity?.ClientId is { } clientId)
                {
                    return new ManagedIdentityCredential(clientId, new() { AuthorityHost = authorityUri });
                }

                if (credentialOptions?.ManagedIdentity?.ResourceId is { } resourceId)
                {
                    return new ManagedIdentityCredential(new ResourceIdentifier(resourceId), new() { AuthorityHost = authorityUri });
                }

                // This is a defensive check. ClientId and ResourceId should already be validated when binding the cloud configuration.
                throw new InvalidOperationException("ClientId and ResourceId cannot be null at the same time for user-assigned identity.");
            }

            return credentialType switch
            {
                CredentialType.Environment => new EnvironmentCredential(new() { AuthorityHost = authorityUri }),
                CredentialType.VisualStudio => new VisualStudioCredential(new() { AuthorityHost = authorityUri }),
                CredentialType.VisualStudioCode => new VisualStudioCodeCredential(new() { AuthorityHost = authorityUri }),
                // AzureCLICredential does not accept options. Azure CLI has built-in cloud profiles so AuthorityHost is not needed.
                CredentialType.AzureCLI => new AzureCliCredential(),
                CredentialType.AzurePowerShell => new AzurePowerShellCredential(new() { AuthorityHost = authorityUri }),

                _ => throw new NotImplementedException($"Unexpected credential type '{credentialType}'.")
            };

        }
    }
}
