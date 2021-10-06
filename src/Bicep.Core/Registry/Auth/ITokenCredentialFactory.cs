// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Bicep.Core.Configuration;
using System.Collections.Generic;

namespace Bicep.Core.Registry.Auth
{
    public interface ITokenCredentialFactory
    {
        TokenCredential CreateChain(IEnumerable<CredentialType> credentialPrecedence);

        TokenCredential CreateSingle(CredentialType credentialType);
    }
}
