// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Auth;
using OrasProject.Oras.Registry.Remote.Auth;

namespace Bicep.Core.Registry.Oci.Oras;

internal sealed class CredentialChainCredentialProvider : ICredentialProvider
{
    private static readonly IReadOnlyList<string> EmptyScopes = Array.Empty<string>();

    private readonly ICredentialChain credentialChain;

    public CredentialChainCredentialProvider(ICredentialChain credentialChain)
    {
        this.credentialChain = credentialChain;
    }

    public async Task<Credential> ResolveCredentialAsync(string hostname, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(hostname))
        {
            return default;
        }

        if (!Uri.TryCreate($"https://{hostname}", UriKind.Absolute, out var registryUri))
        {
            return default;
        }

        var registryCredential = await credentialChain.GetAsync(
            registryUri,
            EmptyScopes,
            challenge: null,
            cancellationToken).ConfigureAwait(false);

        if (registryCredential is null || string.IsNullOrEmpty(registryCredential.PasswordOrToken))
        {
            return default;
        }

        return registryCredential.Scheme switch
        {
            AuthScheme.Basic => new Credential(
                registryCredential.Username,
                registryCredential.PasswordOrToken),
            AuthScheme.Bearer => new Credential(AccessToken: registryCredential.PasswordOrToken),
            _ => default,
        };
    }
}
