// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Oci.Oras;
using OrasProject.Oras.Registry.Remote.Auth;

namespace Bicep.Core.Registry.Auth;

public class DockerCredentialSource : ICredentialSource
{
    private readonly DockerCredentialProvider dockerCredentialProvider;

    public DockerCredentialSource(DockerCredentialProvider dockerCredentialProvider)
    {
        this.dockerCredentialProvider = dockerCredentialProvider;
    }

    public async ValueTask<RegistryCredential?> TryGetAsync(
        Uri registry,
        IReadOnlyList<string> requestedScopes,
        AuthMetadata? challenge,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(registry);

        var credential = await dockerCredentialProvider.ResolveCredentialAsync(registry.Host, cancellationToken).ConfigureAwait(false);
        if (credential.IsEmpty())
        {
            return null;
        }

        if (!string.IsNullOrEmpty(credential.AccessToken))
        {
            return new RegistryCredential(AuthScheme.Bearer, credential.Username ?? string.Empty, credential.AccessToken, null);
        }

        if (!string.IsNullOrEmpty(credential.Username) && credential.Password is not null)
        {
            return new RegistryCredential(AuthScheme.Basic, credential.Username!, credential.Password, null);
        }

        return null;
    }
}
