// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using System;
using System.Collections.Generic;
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
        // obtained from https://github.com/opencontainers/distribution-spec/blob/main/spec.md#pull
        private static readonly Regex ModulePathSegmentRegex = new(@"^[a-z0-9]+([._-][a-z0-9]+)*$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        private static readonly Regex TagRegex = new(@"^[a-zA-Z0-9_][a-zA-Z0-9._-]{0,127}$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

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
            static DiagnosticBuilder.ErrorBuilderDelegate CreateErrorFunc(string rawValue, string? innerError) => x => x.InvalidOciArtifactReference($"{ModuleReferenceSchemes.Oci}:{rawValue}", innerError);

            static DiagnosticBuilder.ErrorBuilderDelegate CreateInvalidSegmentErrorFunc(string rawValue, string badSegment) =>
                CreateErrorFunc(
                    rawValue,
                    $"The mpdule path segment \"{badSegment}\" is not valid. Each module name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".");

            static string UnescapeSegment(string segment) => HttpUtility.UrlDecode(segment);

            // the set of valid OCI artifact refs is a subset of the set of valid URIs if you remove the scheme portion from each URI
            // manually prepending any valid URI scheme allows to get free validation via the built-in URI parser
            if (!Uri.TryCreate($"{ModuleReferenceSchemes.Oci}://{rawValue}", UriKind.Absolute, out var artifactUri) ||
                artifactUri.Segments.Length <= 1 ||
                !string.Equals(artifactUri.Segments[0], "/", StringComparison.Ordinal))
            {
                failureBuilder = CreateErrorFunc(rawValue, null);
                return null;
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
                    var invalidSegment = UnescapeSegment(current.Substring(0, current.Length - 1));
                    failureBuilder = CreateInvalidSegmentErrorFunc(rawValue, invalidSegment);
                    return null;
                }

                // even though chars that require URL-escaping are not part of the allowed regexes
                // users can still type them in, so error messages should contain the original text rather than an escaped version
                repoBuilder.Append(UnescapeSegment(current));
            }

            // on a valid ref it would look something like "bar:v1"
            var lastSegment = artifactUri.Segments[^1];
            var indexOfColon = lastSegment.IndexOf(':');

            static bool HasTag(int indexOfColon) => indexOfColon >= 0;

            // users will type references from left to right, so we should validate the last component of the module path
            // before we complain about the missing tag, which is the last part of the module ref
            var name = UnescapeSegment(!HasTag(indexOfColon) ? lastSegment : lastSegment.Substring(0, indexOfColon));
            if (!ModulePathSegmentRegex.IsMatch(name))
            {
                failureBuilder = CreateInvalidSegmentErrorFunc(rawValue, name);
                return null;
            }

            repoBuilder.Append(name);

            // now we can complain about the missing tag
            if (!HasTag(indexOfColon))
            {
                failureBuilder = CreateErrorFunc(rawValue, "The module tag is missing.");
                return null;
            }

            var tag = UnescapeSegment(lastSegment.Substring(indexOfColon + 1));
            if (!TagRegex.IsMatch(tag))
            {
                failureBuilder = CreateErrorFunc(rawValue, $"The tag \"{tag}\" is not valid. The tag must be a string with maximum length 128 characters. Valid characters are alphanumeric, \".\", \"_\", or \"-\" but the tag cannot begin with \".\", \"_\", or \"-\".");
                return null;
            }

            failureBuilder = null;
            return new OciArtifactModuleReference(artifactUri.Authority, repoBuilder.ToString(), tag);
        }
    }
}
