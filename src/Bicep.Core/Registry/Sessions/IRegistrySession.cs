// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry.Sessions;

public interface IRegistrySession : IAsyncDisposable
{
    Task PushAsync(RegistryRef target, RegistryArtifact artifact, CancellationToken cancellationToken);

    Task<RegistryArtifact> PullAsync(RegistryRef reference, CancellationToken cancellationToken);

    Task<RegistryManifestInfo> ResolveAsync(RegistryRef reference, CancellationToken cancellationToken);
}
