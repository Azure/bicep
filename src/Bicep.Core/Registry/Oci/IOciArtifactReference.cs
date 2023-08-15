// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bicep.Core.Registry.Oci
{
    public interface IOciArtifactReference
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
        /// Gets the artifact ID.
        /// </summary>
        string ArtifactId { get; }

        string FullyQualifiedReference { get; }
    }
}