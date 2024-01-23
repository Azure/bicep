// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry.Oci
{
    public class ArtifactAddressComponents : IArtifactAddressComponents
    {
        public ArtifactAddressComponents(string registry, string repository, string? tag, string? digest)
        {
            switch (tag, digest)
            {
                case (null, null):
                    throw new ArgumentException($"Both {nameof(tag)} and {nameof(digest)} cannot be null.");
                case (not null, not null):
                    throw new ArgumentException($"Both {nameof(tag)} and {nameof(digest)} cannot be non-null.");
            }

            this.Registry = registry;
            this.Repository = repository;
            this.Tag = tag;
            this.Digest = digest;
        }

        /// <summary>
        /// Gets the registry URI.
        /// </summary>
        public string Registry { get; init; }

        /// <summary>
        /// Gets the repository name. The repository name is the path to an artifact in the registry without the tag.
        /// </summary>
        public string Repository { get; init; }

        /// <summary>
        /// Gets the tag. Either tag or digest is set but not both.
        /// </summary>
        public string? Tag { get; init; }

        /// <summary>
        /// Gets the digest. Either tag or digest is set but not both.
        /// </summary>
        public string? Digest { get; init; }

        /// <summary>
        /// Gets the artifact ID.
        /// </summary>
        public string ArtifactId => this.Digest is null
            ? $"{this.Registry}/{this.Repository}:{this.Tag}"
            : $"{this.Registry}/{this.Repository}@{this.Digest}";


        public override bool Equals(object? obj)
        {
            if (obj is not ArtifactAddressComponents other)
            {
                return false;
            }

            return
                OciArtifactReferenceFacts.RegistryComparer.Equals(this.Registry, other.Registry) &&
                OciArtifactReferenceFacts.RepositoryComparer.Equals(this.Repository, other.Repository) &&
                OciArtifactReferenceFacts.TagComparer.Equals(this.Tag, other.Tag) &&
                OciArtifactReferenceFacts.DigestComparer.Equals(this.Digest, other.Digest);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(this.Registry, OciArtifactReferenceFacts.RegistryComparer);
            hash.Add(this.Repository, OciArtifactReferenceFacts.RepositoryComparer);
            hash.Add(this.Tag, OciArtifactReferenceFacts.TagComparer);
            hash.Add(this.Digest, OciArtifactReferenceFacts.DigestComparer);

            return hash.ToHashCode();
        }
    }
}

