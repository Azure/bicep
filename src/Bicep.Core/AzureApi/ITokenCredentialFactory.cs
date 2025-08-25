// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Bicep.Core.Configuration;

namespace Bicep.Core.AzureApi
{
    public interface ITokenCredentialFactory
    {
        TokenCredential CreateChain(IEnumerable<CredentialType> credentialPrecedence, CredentialOptions? credentialOptions, Uri authorityUri);

        TokenCredential CreateSingle(CredentialType credentialType, CredentialOptions? credentialOptions, Uri authorityUri);
    }
}
