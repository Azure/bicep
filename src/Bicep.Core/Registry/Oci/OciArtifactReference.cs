// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
using System.Web;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.Registry.Oci
{
    // Currently this can be a module or a provider.
    public class OciArtifactReference : ArtifactReference, IOciArtifactReference
    {
        public OciArtifactReference(ArtifactType type, IArtifactAddressComponents artifactIdParts, Uri parentModuleUri) :
            base(OciArtifactReferenceFacts.Scheme, parentModuleUri)
        {
            Type = type;
            AddressComponents = artifactIdParts;
        }

        public OciArtifactReference(ArtifactType type, string registry, string repository, string? tag, string? digest, Uri parentModuleUri) :
            base(OciArtifactReferenceFacts.Scheme, parentModuleUri)
        {
            switch (tag, digest)
            {
                case (null, null):
                    throw new ArgumentException($"Both {nameof(tag)} and {nameof(digest)} cannot be null.");
                case (not null, not null):
                    throw new ArgumentException($"Both {nameof(tag)} and {nameof(digest)} cannot be non-null.");
            }

            Type = type;
            AddressComponents = new ArtifactAddressComponents(registry, repository, tag, digest);
        }

        public IArtifactAddressComponents AddressComponents { get; }

        /// <summary>
        /// Gets the type of artifact reference. Either module or provider.
        /// </summary>
        public ArtifactType Type { get; }

        /// <summary>
        /// Gets the registry URI.
        /// </summary>
        public string Registry => AddressComponents.Registry;

        /// <summary>
        /// Gets the repository name. The repository name is the path to an artifact in the registry without the tag.
        /// </summary>
        public string Repository => AddressComponents.Repository;

        /// <summary>
        /// Gets the tag. Either tag or digest is set but not both.
        /// </summary>
        public string? Tag => AddressComponents.Tag;

        /// <summary>
        /// Gets the digest. Either tag or digest is set but not both.
        /// </summary>
        public string? Digest => AddressComponents.Digest;

        /// <summary>
        /// Gets the artifact ID.
        /// </summary>
        public string ArtifactId => AddressComponents.ArtifactId;

        public override string UnqualifiedReference => ArtifactId;

        public override bool IsExternal => true;

        // unqualifiedReference is the reference without a scheme or alias, e.g. "example.azurecr.invalid/foo/bar:v3"
        // The configuration and parentModuleUri are needed to resolve aliases and experimental features
        public static ResultWithDiagnostic<OciArtifactReference> TryParseModuleAndAlias(string? aliasName, string unqualifiedReference, RootConfiguration configuration, Uri parentModuleUri)
            => TryParse(ArtifactType.Module, aliasName, unqualifiedReference, configuration, parentModuleUri);

        public static ResultWithDiagnostic<OciArtifactReference> TryParseModule(string unqualifiedReference)
            => TryParse(ArtifactType.Module, null, unqualifiedReference, null, null);

        public static ResultWithDiagnostic<OciArtifactReference> TryParse(ArtifactType type, string? aliasName, string unqualifiedReference, RootConfiguration? configuration, Uri? parentModuleUri)
        {
            if (TryParseParts(type, aliasName, unqualifiedReference, configuration).IsSuccess(out var parts, out var errorBuilder))
            {
                return new(new OciArtifactReference(type, parts.Registry, parts.Repository, parts.Tag, parts.Digest, parentModuleUri ?? new Uri("file:///no-parent-file-is-available.bicep")));
            }
            else
            {
                return new(errorBuilder);
            }
        }

        // Doesn't handle aliases
        public static ResultWithDiagnostic<IArtifactAddressComponents> TryParseFullyQualifiedComponents(string rawValue)
        {
            return TryParseParts(ArtifactType.Module, aliasName: null, rawValue, configuration: null);
        }

        // TODO: Completely remove aliasName and configuration dependencies and move the non-dependent portion to a static method on ArtifactAddressComponents
        private static ResultWithDiagnostic<IArtifactAddressComponents> TryParseParts(ArtifactType type, string? aliasName, string unqualifiedReference, RootConfiguration? configuration)
        {
            static string GetBadReference(string referenceValue) => $"{OciArtifactReferenceFacts.Scheme}:{referenceValue}";

            static string DecodeSegment(string segment) => HttpUtility.UrlDecode(segment);

            if (configuration is { } && aliasName is { })
            {
                switch (type)
                {
                    case ArtifactType.Module:
                        if (!configuration.ModuleAliases.TryGetOciArtifactModuleAlias(aliasName).IsSuccess(out var moduleAlias, out var moduleFailureBuilder))
                        {
                            return new(moduleFailureBuilder);
                        }
                        unqualifiedReference = $"{moduleAlias}/{unqualifiedReference}";
                        break;
                    case ArtifactType.Provider:
                        if (!configuration.ProviderAliases.TryGetOciArtifactProviderAlias(aliasName).IsSuccess(out var providerAlias, out var providerFailureBuilder))
                        {
                            return new(providerFailureBuilder);
                        }
                        unqualifiedReference = $"{providerAlias}/{unqualifiedReference}";
                        break;
                    default:
                        return new(x => x.UnsupportedArtifactType(type));
                }
            }

            // the set of valid OCI artifact refs is a subset of the set of valid URIs if you remove the scheme portion from each URI
            // manually prepending any valid URI scheme allows to get free validation via the built-in URI parser
            if (!Uri.TryCreate($"{OciArtifactReferenceFacts.Scheme}://{unqualifiedReference}", UriKind.Absolute, out var artifactUri) ||
                artifactUri.Segments.Length <= 1 ||
                !string.Equals(artifactUri.Segments[0], "/", StringComparison.Ordinal))
            {
                return new(x => x.InvalidOciArtifactReference(aliasName, GetBadReference(unqualifiedReference)));
            }

            string registry = artifactUri.Authority;
            if (registry.Length > OciArtifactReferenceFacts.MaxRegistryLength)
            {
                return new(x => x.InvalidOciArtifactReferenceRegistryTooLong(
                    aliasName,
                    GetBadReference(unqualifiedReference),
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
                    return new(x => x.InvalidOciArtifactReferenceInvalidPathSegment(aliasName, GetBadReference(unqualifiedReference), invalidSegment));
                }

                // even though chars that require URL-escaping are not part of the allowed regexes
                // users can still type them in, so error messages should contain the original text rather than an escaped version
                repoBuilder.Append(DecodeSegment(segment));
            }

            // on a valid ref it would look something like "bar:v1" or "bar@sha256:e207a69d02b3de40d48ede9fd208d80441a9e590a83a0bc915d46244c03310d4"
            var lastSegment = artifactUri.Segments[^1];

            static (int index, char? delimiter) FindLastSegmentDelimiter(string lastSegment)
            {
                char[] delimiters = [':', '@'];
                int index = lastSegment.IndexOfAny(delimiters);

                return (index, index == -1 ? null : lastSegment[index]);
            }

            var (indexOfLastSegmentDelimiter, delimiter) = FindLastSegmentDelimiter(lastSegment);

            // users will type references from left to right, so we should validate the last component of the module path
            // before we complain about the missing tag, which is the last part of the module ref
            var name = DecodeSegment(!delimiter.HasValue ? lastSegment : lastSegment[..indexOfLastSegmentDelimiter]);
            if (!OciArtifactReferenceFacts.IsOciNamespaceSegment(name))
            {
                return new(x => x.InvalidOciArtifactReferenceInvalidPathSegment(aliasName, GetBadReference(unqualifiedReference), name));
            }

            repoBuilder.Append(name);

            string repository = repoBuilder.ToString();
            if (repository.Length > OciArtifactReferenceFacts.MaxRepositoryLength)
            {
                return new(x => x.InvalidOciArtifactReferenceRepositoryTooLong(
                    aliasName,
                    GetBadReference(unqualifiedReference),
                    repository,
                    OciArtifactReferenceFacts.MaxRepositoryLength));
            }

            // now we can complain about the missing tag or digest
            if (!delimiter.HasValue)
            {
                return new(x => x.InvalidOciArtifactReferenceMissingTagOrDigest(aliasName, GetBadReference(unqualifiedReference)));
            }

            var tagOrDigest = DecodeSegment(lastSegment.Substring(indexOfLastSegmentDelimiter + 1));
            if (string.IsNullOrEmpty(tagOrDigest))
            {
                return new(x => x.InvalidOciArtifactReferenceMissingTagOrDigest(aliasName, GetBadReference(unqualifiedReference)));
            }

            switch (delimiter.Value)
            {
                case ':':
                    var tag = tagOrDigest;
                    if (tag.Length > OciArtifactReferenceFacts.MaxTagLength)
                    {
                        return new(x => x.InvalidOciArtifactReferenceTagTooLong(
                            aliasName,
                            GetBadReference(unqualifiedReference),
                            tag,
                            OciArtifactReferenceFacts.MaxTagLength));
                    }

                    if (!OciArtifactReferenceFacts.IsOciTag(tag))
                    {
                        return new(x => x.InvalidOciArtifactReferenceInvalidTag(
                            aliasName,
                            GetBadReference(unqualifiedReference),
                            tag));
                    }

                    return new(new ArtifactAddressComponents(registry, repository, tag, digest: null));

                case '@':
                    var digest = tagOrDigest;
                    if (!OciArtifactReferenceFacts.IsOciDigest(digest))
                    {
                        return new(x => x.InvalidOciArtifactReferenceInvalidDigest(aliasName, GetBadReference(unqualifiedReference), digest));
                    }

                    return new(new ArtifactAddressComponents(registry, repository, tag: null, digest: digest));

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
                Type == other.Type &&
                AddressComponents.Equals(other.AddressComponents);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Type);
            hash.Add(AddressComponents);

            return hash.ToHashCode();
        }
    }
}
