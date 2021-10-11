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
        public TokenCredential CreateChain(IEnumerable<CredentialType> credentialPrecedence)
        {
            var tokenCredentials = credentialPrecedence.Select(CreateSingle).ToArray();
            if(tokenCredentials.Length == 0)
            {
                throw new ArgumentException("At least one credential type must be provided.");
            }

            return new ChainedTokenCredential(tokenCredentials);
        }

        public TokenCredential CreateSingle(CredentialType credentialType) =>
            credentialType switch
            {
                CredentialType.Environment => new EnvironmentCredential(),
                CredentialType.ManagedIdentity => new ManagedIdentityCredential(),
                CredentialType.VisualStudio => new VisualStudioCredential(),
                CredentialType.VisualStudioCode => new VisualStudioCodeCredential(),
                CredentialType.AzureCLI => new AzureCliCredential(),
                CredentialType.AzurePowerShell => new AzurePowerShellCredential(),

                _ => throw new NotImplementedException($"Unexpected credential type '{credentialType}'.")
            };
    }
}
