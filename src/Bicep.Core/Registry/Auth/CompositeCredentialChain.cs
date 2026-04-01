// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.Registry.Auth;

public class CompositeCredentialChain : ICredentialChain
{
    private readonly ImmutableArray<ICredentialSource> sources;

    public CompositeCredentialChain(IEnumerable<ICredentialSource> sources)
    {
        ArgumentNullException.ThrowIfNull(sources);
        this.sources = sources.ToImmutableArray();
    }

    public async ValueTask<RegistryCredential?> GetAsync(
        Uri registry,
        IReadOnlyList<string> requestedScopes,
        AuthMetadata? challenge,
        CancellationToken cancellationToken)
    {
        foreach (var source in sources)
        {
            var credential = await source.TryGetAsync(registry, requestedScopes, challenge, cancellationToken).ConfigureAwait(false);
            if (credential is not null)
            {
                return credential;
            }
        }

        return null;
    }
}
