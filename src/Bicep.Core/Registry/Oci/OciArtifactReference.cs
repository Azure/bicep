// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using System.Web;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceGraph.ArtifactReferences;
using Bicep.Core.SourceGraph.Artifacts;
using Bicep.Core.Syntax;
using Bicep.IO.Abstraction;
using Bicep.IO.Utils;

namespace Bicep.Core.Registry.Oci
{
    public class OciArtifactReference : ArtifactReference, IOciArtifactReference, IExtensionArtifactReference
    {
        private readonly Lazy<BicepRegistryModuleArtifact> lazyBicepRegistryModuleArtifact;
        private readonly Lazy<BicepRegistryExtensionArtifact> lazyBicepRegistryExtensionArtifact;

        public OciArtifactReference(BicepSourceFile referencingFile, ArtifactType type, IOciArtifactAddressComponents artifactIdParts) :
            base(referencingFile, OciArtifactReferenceFacts.Scheme)
        {
            Type = type;
            AddressComponents = artifactIdParts;
            lazyBicepRegistryModuleArtifact = new(() => new(this.AddressComponents, this.ReferencingFile.Features.CacheRootDirectory));
            lazyBicepRegistryExtensionArtifact = new(() => new(this.AddressComponents, this.ReferencingFile.Features.CacheRootDirectory));
        }

        public OciArtifactReference(BicepSourceFile referencingFile, ArtifactType type, string registry, string repository, string? tag, string? digest) :
            base(referencingFile, OciArtifactReferenceFacts.Scheme)
        {
            switch (tag, digest)
            {
                case (null, null):
                    throw new ArgumentException($"Both {nameof(tag)} and {nameof(digest)} cannot be null.");
                case (not null, not null):
                    throw new ArgumentException($"Both {nameof(tag)} and {nameof(digest)} cannot be non-null.");
            }

            Type = type;
            AddressComponents = new OciArtifactAddressComponents(registry, repository, tag, digest);
            lazyBicepRegistryModuleArtifact = new(() => new(this.AddressComponents, this.ReferencingFile.Features.CacheRootDirectory));
            lazyBicepRegistryExtensionArtifact = new(() => new(this.AddressComponents, this.ReferencingFile.Features.CacheRootDirectory));
        }

        public IOciArtifactAddressComponents AddressComponents { get; }

        /// <summary>
        /// Gets the type of artifact reference.
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

        public IFileHandle ModuleMainTemplateFile => this.lazyBicepRegistryModuleArtifact.Value.MainTemplateFile;

        public IFileHandle ModuleSourceTgzFile => this.lazyBicepRegistryModuleArtifact.Value.SourceTgzFile;

        public IFileHandle ExtensionTypesTgzFile => this.lazyBicepRegistryExtensionArtifact.Value.TypesTgzFile;

        public override ResultWithDiagnosticBuilder<IFileHandle> TryGetEntryPointFileHandle() => this.Type switch
        {
            ArtifactType.Module => new(this.ModuleMainTemplateFile),
            ArtifactType.Extension => new(this.ExtensionTypesTgzFile),
            _ => throw new UnreachableException(),
        };

        // unqualifiedReference is the reference without a scheme or alias, e.g. "example.azurecr.invalid/foo/bar:v3"
        // The referencingFile is needed to resolve aliases and experimental features
        public static ResultWithDiagnosticBuilder<OciArtifactReference> TryParse(BicepSourceFile referencingFile, ArtifactType type, string? aliasName, string unqualifiedReference)
        {
            if (TryParseComponents(referencingFile, type, aliasName, unqualifiedReference).IsSuccess(out var components, out var errorBuilder))
            {
                return new(new OciArtifactReference(referencingFile, type, components.Registry, components.Repository, components.Tag, components.Digest));
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

        private static ResultWithDiagnosticBuilder<OciArtifactAddressComponents> TryParseComponents(BicepSourceFile referencingFile, ArtifactType type, string? aliasName, string unqualifiedReference)
        {
            if (aliasName is { })
            {
                switch (type)
                {
                    case ArtifactType.Module:
                        if (!referencingFile.Configuration.ModuleAliases.TryGetOciArtifactModuleAlias(aliasName).IsSuccess(out var moduleAlias, out var moduleFailureBuilder))
                        {
                            return new(moduleFailureBuilder);
                        }
                        unqualifiedReference = $"{moduleAlias}/{unqualifiedReference}";
                        break;
                    default:
                        return new(x => x.UnsupportedArtifactType(type));
                }
            }

            return OciArtifactAddressComponents.TryParse(unqualifiedReference, aliasName);
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
