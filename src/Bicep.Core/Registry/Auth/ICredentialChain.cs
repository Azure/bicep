// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry.Auth;

public interface ICredentialChain
{
    ValueTask<RegistryCredential?> GetAsync(
        Uri registry,
        IReadOnlyList<string> requestedScopes,
        AuthMetadata? challenge,
        CancellationToken cancellationToken);
}
