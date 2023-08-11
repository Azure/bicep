// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bicep.Core.Registry.Oci
{
    public interface IOciArtifactReference
    {
        public const string Scheme = "br";

        public const int MaxRegistryLength = 255;

        // must be kept in sync with the tag name regex
        public const int MaxTagLength = 128;

        public const int MaxRepositoryLength = 255;

        // the registry component is equivalent to a host in a URI, which are case-insensitive
        public static readonly IEqualityComparer<string> RegistryComparer = StringComparer.OrdinalIgnoreCase;

        // repository component is case-sensitive (although regex blocks upper case)
        public static readonly IEqualityComparer<string> RepositoryComparer = StringComparer.Ordinal;

        // tags are case-sensitive and may contain upper and lowercase characters
        public static readonly IEqualityComparer<string?> TagComparer = StringComparer.Ordinal;

        // digests are case sensitive
        public static readonly IEqualityComparer<string?> DigestComparer = StringComparer.Ordinal;

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
        /// Gets the artifact ID.
        /// </summary>
        string ArtifactId { get; }

        string FullyQualifiedReference { get; }
    }
}