// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry.Auth;

public interface ICredentialSource
{
    ValueTask<RegistryCredential?> TryGetAsync(
        Uri registry,
        IReadOnlyList<string> requestedScopes,
        AuthMetadata? challenge,
        CancellationToken cancellationToken);
}
