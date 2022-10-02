// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Bicep.Core.Modules
{
    /// <summary>
    /// Represents a reference to an artifact in an OCI registry.
    /// </summary>
    public class OciArtifactModuleReference : ModuleReference
    {
        public const int MaxRegistryLength = 255;

        // must be kept in sync with the tag name regex
        public const int MaxTagLength = 128;

        public const int MaxRepositoryLength = 255;

        // obtained from https://github.com/opencontainers/distribution-spec/blob/main/spec.md#pull
        private static readonly Regex ModulePathSegmentRegex = new(@"^[a-z0-9]+([._-][a-z0-9]+)*$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        // must be kept in sync with the tag max length
        private static readonly Regex TagRegex = new(@"^[a-zA-Z0-9_][a-zA-Z0-9._-]{0,127}$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        private static readonly Regex DigestRegex = new(@"^sha256:[a-f0-9]{64}$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        // the registry component is equivalent to a host in a URI, which are case-insensitive
        public static readonly IEqualityComparer<string> RegistryComparer = StringComparer.OrdinalIgnoreCase;

        // repository component is case-sensitive (although regex blocks upper case)
        public static readonly IEqualityComparer<string> RepositoryComparer = StringComparer.Ordinal;

        // tags are case-sensitive and may contain upper and lowercase characters
        public static readonly IEqualityComparer<string?> TagComparer = StringComparer.Ordinal;

        // digests are case sensitive
        public static readonly IEqualityComparer<string?> DigestComparer = StringComparer.Ordinal;

        public OciArtifactModuleReference(string registry, string repository, string? tag, string? digest, Uri parentModuleUri)
             : base(ModuleReferenceSchemes.Oci, parentModuleUri)
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

        public override string UnqualifiedReference => this.ArtifactId;

        public override bool IsExternal => true;

        public override bool Equals(object? obj)
        {
            if (obj is not OciArtifactModuleReference other)
            {
                return false;
            }

            return
                // TODO: Are all of these case-sensitive?
                RegistryComparer.Equals(this.Registry, other.Registry) &&
                RepositoryComparer.Equals(this.Repository, other.Repository) &&
                TagComparer.Equals(this.Tag, other.Tag) &&
                DigestComparer.Equals(this.Digest, other.Digest);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(this.Registry, RegistryComparer);
            hash.Add(this.Repository, RepositoryComparer);
            hash.Add(this.Tag, TagComparer);
            hash.Add(this.Digest, DigestComparer);

            return hash.ToHashCode();
        }

        public static bool TryParse(string? aliasName, string rawValue, RootConfiguration configuration, Uri parentModuleUri, [NotNullWhen(true)] out OciArtifactModuleReference? moduleReference, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            static string GetBadReference(string referenceValue) => $"{ModuleReferenceSchemes.Oci}:{referenceValue}";

            static string UnescapeSegment(string segment) => HttpUtility.UrlDecode(segment);

            if (aliasName is not null)
            {
                if (!configuration.ModuleAliases.TryGetOciArtifactModuleAlias(aliasName, out var alias, out failureBuilder))
                {
                    moduleReference = null;
                    return false;
                }

                rawValue = $"{alias}/{rawValue}";
            }


            // the set of valid OCI artifact refs is a subset of the set of valid URIs if you remove the scheme portion from each URI
            // manually prepending any valid URI scheme allows to get free validation via the built-in URI parser
            if (!Uri.TryCreate($"{ModuleReferenceSchemes.Oci}://{rawValue}", UriKind.Absolute, out var artifactUri) ||
                artifactUri.Segments.Length <= 1 ||
                !string.Equals(artifactUri.Segments[0], "/", StringComparison.Ordinal))
            {
                failureBuilder = x => x.InvalidOciArtifactReference(aliasName, GetBadReference(rawValue));
                moduleReference = null;
                return false;
            }

            string registry = artifactUri.Authority;
            if (registry.Length > MaxRegistryLength)
            {
                failureBuilder = x => x.InvalidOciArtifactReferenceRegistryTooLong(aliasName, GetBadReference(rawValue), registry, MaxRegistryLength);
                moduleReference = null;
                return false;
            }

            // for "br://example.azurecr.io/foo/bar:v1", the segments are "/", "foo/", and "bar:v1"
            // iterate only over middle segments
            var repoBuilder = new StringBuilder();
            for (int i = 1; i < artifactUri.Segments.Length - 1; i++)
            {
                // don't try to match the last character, which is always '/'
                string current = artifactUri.Segments[i];
                var pathMatch = ModulePathSegmentRegex.Match(current, 0, current.Length - 1);
                if (!pathMatch.Success)
                {
                    var invalidSegment = UnescapeSegment(current[0..^1]);
                    failureBuilder = x => x.InvalidOciArtifactReferenceInvalidPathSegment(aliasName, GetBadReference(rawValue), invalidSegment);
                    moduleReference = null;
                    return false;
                }

                // even though chars that require URL-escaping are not part of the allowed regexes
                // users can still type them in, so error messages should contain the original text rather than an escaped version
                repoBuilder.Append(UnescapeSegment(current));
            }

            // on a valid ref it would look something like "bar:v1" or "bar@sha256:e207a69d02b3de40d48ede9fd208d80441a9e590a83a0bc915d46244c03310d4"
            var lastSegment = artifactUri.Segments[^1];

            static (int index, char? delimiter) FindLastSegmentDelimiter(string lastSegment)
            {
                for (int i = 0; i < lastSegment.Length; i++)
                {
                    var current = lastSegment[i];
                    if (current == ':' || current == '@')
                    {
                        return (i, current);
                    }
                }

                return (-1, null);
            }

            var (indexOfLastSegmentDelimiter, delimiter) = FindLastSegmentDelimiter(lastSegment);

            // users will type references from left to right, so we should validate the last component of the module path
            // before we complain about the missing tag, which is the last part of the module ref
            var name = UnescapeSegment(!delimiter.HasValue ? lastSegment : lastSegment.Substring(0, indexOfLastSegmentDelimiter));
            if (!ModulePathSegmentRegex.IsMatch(name))
            {
                failureBuilder = x => x.InvalidOciArtifactReferenceInvalidPathSegment(aliasName, GetBadReference(rawValue), name);
                moduleReference = null;
                return false;
            }

            repoBuilder.Append(name);

            string repository = repoBuilder.ToString();
            if (repository.Length > MaxRepositoryLength)
            {
                failureBuilder = x => x.InvalidOciArtifactReferenceRepositoryTooLong(aliasName, GetBadReference(rawValue), repository, MaxRepositoryLength);
                moduleReference = null;
                return false;
            }

            // now we can complain about the missing tag or digest
            if (!delimiter.HasValue)
            {
                failureBuilder = x => x.InvalidOciArtifactReferenceMissingTagOrDigest(aliasName, GetBadReference(rawValue));
                moduleReference = null;
                return false;
            }

            var tagOrDigest = UnescapeSegment(lastSegment[(indexOfLastSegmentDelimiter + 1)..]);
            if (string.IsNullOrEmpty(tagOrDigest))
            {
                failureBuilder = x => x.InvalidOciArtifactReferenceMissingTagOrDigest(aliasName, GetBadReference(rawValue));
                moduleReference = null;
                return false;
            }

            switch (delimiter.Value)
            {
                case ':':
                    var tag = tagOrDigest;
                    if (tag.Length > MaxTagLength)
                    {
                        failureBuilder = x => x.InvalidOciArtifactReferenceTagTooLong(aliasName, GetBadReference(rawValue), tag, MaxTagLength);
                        moduleReference = null;
                        return false;
                    }

                    if (!TagRegex.IsMatch(tag))
                    {
                        failureBuilder = x => x.InvalidOciArtifactReferenceInvalidTag(aliasName, GetBadReference(rawValue), tag);
                        moduleReference = null;
                        return false;
                    }

                    failureBuilder = null;
                    moduleReference = new OciArtifactModuleReference(registry, repository, tag: tag, digest: null, parentModuleUri);
                    return true;

                case '@':
                    var digest = tagOrDigest;
                    if (!DigestRegex.IsMatch(digest))
                    {
                        failureBuilder = x => x.InvalidOciArtifactReferenceInvalidDigest(aliasName, GetBadReference(rawValue), digest);
                        moduleReference = null;
                        return false;
                    }

                    failureBuilder = null;
                    moduleReference = new OciArtifactModuleReference(registry, repository, tag: null, digest: digest, parentModuleUri);
                    return true;

                default:
                    throw new NotImplementedException($"Unexpected last segment delimiter character '{delimiter.Value}'.");
            }
        }
    }
}
