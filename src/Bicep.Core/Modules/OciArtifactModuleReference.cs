// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using System;
using System.Collections.Generic;

namespace Bicep.Core.Modules
{
    /// <summary>
    /// Represents a reference to an artifact in an OCI registry.
    /// </summary>
    public class OciArtifactModuleReference : ModuleReference
    {
        // these exist to keep equals and hashcode implementations in sync
        public static readonly IEqualityComparer<string> RegistryComparer = StringComparer.OrdinalIgnoreCase;
        public static readonly IEqualityComparer<string> RepositoryComparer = StringComparer.Ordinal;
        public static readonly IEqualityComparer<string> TagComparer = StringComparer.Ordinal;

        public OciArtifactModuleReference(string registry, string repository, string tag)
             : base(ModuleReferenceSchemes.Oci)
        {
            this.Registry = registry;
            this.Repository = repository;
            this.Tag = tag;
        }

        /// <summary>
        /// Gets the registry URI.
        /// </summary>
        public string Registry { get; }

        /// <summary>
        /// Gets the repository name. The repository name is the path to an artifact in the registry without the tag.
        /// </summary>
        public string Repository { get; }

        /// <summary>
        /// Gets the tag. 
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// Gets the artifact ID.
        /// </summary>
        public string ArtifactId => $"{this.Registry}/{this.Repository}:{this.Tag}";

        public override string UnqualifiedReference => this.ArtifactId;

        public override bool Equals(object obj)
        {
            if(obj is not OciArtifactModuleReference other)
            {
                return false;
            }

            return
                // TODO: Are all of these case-sensitive?
                RegistryComparer.Equals(this.Registry, other.Registry) &&
                RepositoryComparer.Equals(this.Repository, other.Repository) &&
                TagComparer.Equals(this.Tag, other.Tag);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(this.Registry, RegistryComparer);
            hash.Add(this.Repository, RepositoryComparer);
            hash.Add(this.Tag, TagComparer);

            return hash.ToHashCode();
        }

        public static OciArtifactModuleReference? TryParse(string rawValue, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            static DiagnosticBuilder.ErrorBuilderDelegate CreateErrorFunc(string rawValue) => x => x.InvalidOciArtifactReference($"oci:{rawValue}");

            // split tag from the uri
            var lastColonIndex = rawValue.LastIndexOf(':');
            if(lastColonIndex < 0)
            {
                failureBuilder = CreateErrorFunc(rawValue);
                return null;
            }

            var artifactStr = rawValue.Substring(0, lastColonIndex);
            var tag = rawValue.Substring(lastColonIndex + 1);

            // TODO: Replace this with a parser that matches the OCI rules exactly
            // docker image refs (incl. OCI artifact refs) are not valid URIs because they do not include the scheme followed by "://" as a prefix
            // however manually prepending any valid URI scheme allows to get free validation via the built-in URI parser.
            if(!Uri.TryCreate("oci://" + artifactStr, UriKind.Absolute, out var artifactUri) || string.IsNullOrWhiteSpace(tag))
            {
                failureBuilder = CreateErrorFunc(rawValue);
                return null;
            }

            // TODO: Do we need more validation?
            failureBuilder = null;
            var registry = artifactUri.Port < 0
                ? artifactUri.Host
                : $"{artifactUri.Host}:{artifactUri.Port}";
            var repo = artifactUri.PathAndQuery.TrimStart('/');
            return new OciArtifactModuleReference(registry, repo, tag);
        }
    }
}
