// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;

namespace Bicep.Core.Registry.Oci
{

    public class OciArtifactReference : IOciArtifactReference
    {
        private OciArtifactReference(string registry, string repository, string? tag, string? digest)
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
        public string Registry { get; }

        /// <summary>
        /// Gets the repository name. The repository name is the path to an artifact in the registry without the tag.
        /// </summary>
        public string Repository { get; }

        /// <summary>
        /// Gets the tag. Either tag or digest is set but not both.
        /// </summary>
        public string? Tag { get; }

        /// <summary>
        /// Gets the digest. Either tag or digest is set but not both.
        /// </summary>
        public string? Digest { get; }

        /// <summary>
        /// Gets the artifact ID.
        /// </summary>
        public string ArtifactId => this.Digest is null
            ? $"{this.Registry}/{this.Repository}:{this.Tag}"
            : $"{this.Registry}/{this.Repository}@{this.Digest}";

        public string FullyQualifiedReference => ArtifactId;

        public static ResultWithDiagnostic<OciArtifactReference> TryParse(string? aliasName, string rawValue, RootConfiguration configuration)
        {
            static string GetBadReference(string referenceValue) => $"{OciArtifactReferenceFacts.Scheme}:{referenceValue}";

            static string DecodeSegment(string segment) => HttpUtility.UrlDecode(segment);

            if (aliasName is not null)
            {
                if (!configuration.ModuleAliases.TryGetOciArtifactModuleAlias(aliasName).IsSuccess(out var alias, out var failureBuilder))
                {
                    return new(failureBuilder);
                }

                rawValue = $"{alias}/{rawValue}";
            }

            // the set of valid OCI artifact refs is a subset of the set of valid URIs if you remove the scheme portion from each URI
            // manually prepending any valid URI scheme allows to get free validation via the built-in URI parser
            if (!Uri.TryCreate($"{OciArtifactReferenceFacts.Scheme}://{rawValue}", UriKind.Absolute, out var artifactUri) ||
                artifactUri.Segments.Length <= 1 ||
                !string.Equals(artifactUri.Segments[0], "/", StringComparison.Ordinal))
            {
                return new(x => x.InvalidOciArtifactReference(aliasName, GetBadReference(rawValue)));
            }

            string registry = artifactUri.Authority;
            if (registry.Length > OciArtifactReferenceFacts.MaxRegistryLength)
            {
                return new(x => x.InvalidOciArtifactReferenceRegistryTooLong(
                    aliasName,
                    GetBadReference(rawValue),
                    registry,
                    OciArtifactReferenceFacts.MaxRegistryLength));
            }

            // for "br://example.azurecr.io/foo/bar:v1", the segments are "/", "foo/", and "bar:v1"
            // iterate only over middle segments
            var repoBuilder = new StringBuilder();
            for (int i = 1; i < artifactUri.Segments.Length - 1; i++)
            {
                // don't try to match the last character, which is always '/'
                string segment = artifactUri.Segments[i];
                var segmentWithoutTrailingSlash = segment[..^1];
                if (!OciArtifactReferenceFacts.IsOciNamespaceSegment(segmentWithoutTrailingSlash))
                {
                    var invalidSegment = DecodeSegment(segmentWithoutTrailingSlash);
                    return new(x => x.InvalidOciArtifactReferenceInvalidPathSegment(aliasName, GetBadReference(rawValue), invalidSegment));
                }

                // even though chars that require URL-escaping are not part of the allowed regexes
                // users can still type them in, so error messages should contain the original text rather than an escaped version
                repoBuilder.Append(DecodeSegment(segment));
            }

            // on a valid ref it would look something like "bar:v1" or "bar@sha256:e207a69d02b3de40d48ede9fd208d80441a9e590a83a0bc915d46244c03310d4"
            var lastSegment = artifactUri.Segments[^1];

            static (int index, char? delimiter) FindLastSegmentDelimiter(string lastSegment)
            {
                char[] delimiters = { ':', '@' };
                int index = lastSegment.IndexOfAny(delimiters);

                return (index, index == -1 ? null : lastSegment[index]);
            }

            var (indexOfLastSegmentDelimiter, delimiter) = FindLastSegmentDelimiter(lastSegment);

            // users will type references from left to right, so we should validate the last component of the module path
            // before we complain about the missing tag, which is the last part of the module ref
            var name = DecodeSegment(!delimiter.HasValue ? lastSegment : lastSegment[..indexOfLastSegmentDelimiter]);
            if (!OciArtifactReferenceFacts.IsOciNamespaceSegment(name))
            {
                return new(x => x.InvalidOciArtifactReferenceInvalidPathSegment(aliasName, GetBadReference(rawValue), name));
            }

            repoBuilder.Append(name);

            string repository = repoBuilder.ToString();
            if (repository.Length > OciArtifactReferenceFacts.MaxRepositoryLength)
            {
                return new(x => x.InvalidOciArtifactReferenceRepositoryTooLong(
                    aliasName,
                    GetBadReference(rawValue),
                    repository,
                    OciArtifactReferenceFacts.MaxRepositoryLength));
            }

            // now we can complain about the missing tag or digest
            if (!delimiter.HasValue)
            {
                return new(x => x.InvalidOciArtifactReferenceMissingTagOrDigest(aliasName, GetBadReference(rawValue)));
            }

            var tagOrDigest = DecodeSegment(lastSegment.Substring(indexOfLastSegmentDelimiter + 1));
            if (string.IsNullOrEmpty(tagOrDigest))
            {
                return new(x => x.InvalidOciArtifactReferenceMissingTagOrDigest(aliasName, GetBadReference(rawValue)));
            }

            switch (delimiter.Value)
            {
                case ':':
                    var tag = tagOrDigest;
                    if (tag.Length > OciArtifactReferenceFacts.MaxTagLength)
                    {
                        return new(x => x.InvalidOciArtifactReferenceTagTooLong(
                            aliasName,
                            GetBadReference(rawValue),
                            tag,
                            OciArtifactReferenceFacts.MaxTagLength));
                    }

                    if (!OciArtifactReferenceFacts.IsOciTag(tag))
                    {
                        return new(x => x.InvalidOciArtifactReferenceInvalidTag(
                            aliasName,
                            GetBadReference(rawValue),
                            tag));
                    }

                    return new(new OciArtifactReference(registry, repository, tag: tag, digest: null));

                case '@':
                    var digest = tagOrDigest;
                    if (!OciArtifactReferenceFacts.IsOciDigest(digest))
                    {
                        return new(x => x.InvalidOciArtifactReferenceInvalidDigest(aliasName, GetBadReference(rawValue), digest));
                    }

                    return new(new OciArtifactReference(registry, repository, tag: null, digest: digest));

                default:
                    throw new NotImplementedException($"Unexpected last segment delimiter character '{delimiter.Value}'.");
            }

        }

        public override bool Equals(object? obj)
        {
            if (obj is not OciArtifactReference other)
            {
                return false;
            }

            return
                // TODO: Are all of these case-sensitive?
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
