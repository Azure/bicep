// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Bicep.Core.Configuration;
using System;
using System.Collections.Generic;

namespace Bicep.Core.Registry.Auth
{
    public interface ITokenCredentialFactory
    {
        TokenCredential CreateChain(IEnumerable<CredentialType> credentialPrecedence, Uri authorityUri);

        TokenCredential CreateSingle(CredentialType credentialType, Uri authorityUri);
    }
}
