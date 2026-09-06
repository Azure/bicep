// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.Configuration
{
    public interface IBicepCloudConfiguration
    {
        ImmutableArray<CredentialType> CredentialPrecedence { get; }

        CredentialOptions? CredentialOptions { get; }

        Uri ResourceManagerEndpointUri { get; }

        Uri ActiveDirectoryAuthorityUri { get; }

        string AuthenticationScope { get; }

        string ResourceManagerAudience { get; }
    }
}
