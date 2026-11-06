// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using System.Web;
using Bicep.Core.Diagnostics;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceGraph.ArtifactReferences;
using Bicep.Core.SourceGraph.Artifacts;
using Bicep.Core.Syntax;
using Bicep.IO.Abstraction;
using Bicep.IO.Utils;

namespace Bicep.Core.Registry.Oci
{
    public class OciArtifactReference : ExternalArtifactReference, IOciArtifactReference, IExtensionArtifactReference
    {
        private readonly Lazy<BicepRegistryModuleArtifact> lazyBicepRegistryModuleArtifact;
        private readonly Lazy<BicepRegistryExtensionArtifact> lazyBicepRegistryExtensionArtifact;

        public OciArtifactReference(OciArtifactContext context, ArtifactType type, IOciArtifactAddressComponents artifactIdParts) :
            base(context, OciArtifactReferenceFacts.Scheme, type)
        {
            OciContext = context;
            AddressComponents = artifactIdParts;
            lazyBicepRegistryModuleArtifact = new(() => new(this.AddressComponents, this.Context.CacheRootDirectory));
            lazyBicepRegistryExtensionArtifact = new(() => new(this.AddressComponents, this.Context.CacheRootDirectory));
        }

        public OciArtifactReference(OciArtifactContext context, ArtifactType type, string registry, string repository, string? tag, string? digest) :
            base(context, OciArtifactReferenceFacts.Scheme, type)
        {
            switch (tag, digest)
            {
                case (null, null):
                    throw new ArgumentException($"Both {nameof(tag)} and {nameof(digest)} cannot be null.");
                case (not null, not null):
                    throw new ArgumentException($"Both {nameof(tag)} and {nameof(digest)} cannot be non-null.");
            }

            OciContext = context;
            AddressComponents = new OciArtifactAddressComponents(registry, repository, tag, digest);
            lazyBicepRegistryModuleArtifact = new(() => new(this.AddressComponents, this.Context.CacheRootDirectory));
            lazyBicepRegistryExtensionArtifact = new(() => new(this.AddressComponents, this.Context.CacheRootDirectory));
        }

        /// <summary>
        /// Gets the OCI-specific settings used to reach the artifact. It is the same object as <see cref="ExternalArtifactReference.Context"/>.
        /// </summary>
        public OciArtifactContext OciContext { get; }

        /// <summary>
        /// Gets whether the registry is reached with the ORAS SDK instead of the Azure SDK.
        /// </summary>
        public bool UseOrasSdk => OciContext.UseOrasSdk;

        public IOciArtifactAddressComponents AddressComponents { get; }

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

        public IFileHandle ModuleMainTemplateFile => this.lazyBicepRegistryModuleArtifact.Value.MainTemplateFile;

        public IFileHandle ModuleSourceTgzFile => this.lazyBicepRegistryModuleArtifact.Value.SourceTgzFile;

        public IFileHandle ExtensionTypesTgzFile => this.lazyBicepRegistryExtensionArtifact.Value.TypesTgzFile;

        /// <summary>
        /// Parses a fully qualified OCI reference body without the scheme, e.g. <c>example.azurecr.invalid/foo/bar:v3</c>.
        /// </summary>
        /// <param name="aliasContext">The scheme-qualified alias the body was expanded from, if any. Only used in diagnostics.</param>
        public static ResultWithDiagnosticBuilder<OciArtifactReference> TryParse(OciArtifactContext context, ArtifactType type, string? aliasContext, string unqualifiedReference)
        {
            if (OciArtifactAddressComponents.TryParse(unqualifiedReference, aliasContext).IsSuccess(out var components, out var errorBuilder))
            {
                return new(new OciArtifactReference(context, type, components.Registry, components.Repository, components.Tag, components.Digest));
            }
            else
            {
                return new(errorBuilder);
            }
        }

        public IExtensionArtifact ResolveExtensionArtifact() =>
            this.Type == ArtifactType.Extension
                ? this.lazyBicepRegistryExtensionArtifact.Value
                : throw new InvalidOperationException($"Cannot resolve extension artifact for type {this.Type}.");

        public override bool Equals(object? obj)
        {
            if (obj is not OciArtifactReference other)
            {
                return false;
            }

            return
                Type == other.Type &&
                AddressComponents.Equals(other.AddressComponents) &&
                Context.Equals(other.Context);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Type);
            hash.Add(AddressComponents);
            hash.Add(Context);

            return hash.ToHashCode();
        }
    }
}
