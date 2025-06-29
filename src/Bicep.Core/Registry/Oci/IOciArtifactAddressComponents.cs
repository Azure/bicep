// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry.Oci
{
    /// <summary>
    /// Represents a module/provider reference without the scheme (e.g. "br:") and without any aliases
    /// </summary>
    /// <example>test.azurecr.io/foo/bar:latest</example>
    public interface IOciArtifactAddressComponents
    {
        /// <summary>
        /// Gets the registry URI.
        /// </summary>
        string Registry { get; }

        /// <summary>
        /// Gets the repository name. The repository name is the path to an artifact in the registry without the tag.
        /// </summary>
        string Repository { get; }

        /// <summary>
        /// Gets the tag. Either tag or digest is set but not both.
        /// </summary>
        string? Tag { get; }

        /// <summary>
        /// Gets the digest. Either tag or digest is set but not both.
        /// </summary>
        string? Digest { get; }

        /// <summary>
        /// Gets the full artifact ID (does not include the "br:" or "ts:" scheme)
        /// </summary>
        /// <example>test.azurecr.io/foo/bar:latest</example>
        string ArtifactId { get; }
    }
}
