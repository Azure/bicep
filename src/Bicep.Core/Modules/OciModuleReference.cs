// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;


namespace Bicep.Core.Modules
{
    /// <summary>
    /// Represents a reference to an artifact in an OCI registry.
    /// </summary>
    public class OciModuleReference : ArtifactReference, IOciArtifactReference
    {
        private readonly IOciArtifactReference ociArtifactRef;
        public OciModuleReference(IOciArtifactReference ociArtifactReference, Uri parentModuleUri)
             : base(ModuleReferenceSchemes.Oci, parentModuleUri)
        {
            this.ociArtifactRef = ociArtifactReference;
        }
        public override string UnqualifiedReference => this.ArtifactId;
        public override bool IsExternal => true;
        public override bool Equals(object? obj) => obj is OciModuleReference other && this.ociArtifactRef.Equals(other.ociArtifactRef);
        public override int GetHashCode() => this.ociArtifactRef.GetHashCode();
        public string Registry => this.ociArtifactRef.Registry;
        public string Repository => this.ociArtifactRef.Repository;
        public string? Tag => this.ociArtifactRef.Tag;
        public string? Digest => this.ociArtifactRef.Digest;
        public string ArtifactId => this.ociArtifactRef.ArtifactId;

        public static ResultWithDiagnostic<OciModuleReference> TryParse(string? aliasName, string rawValue, RootConfiguration configuration, Uri parentModuleUri)
        {
            if (!OciArtifactReference.TryParse(aliasName, rawValue, configuration).IsSuccess(out var artifactReference, out var failureBuilder))
            {
                return new(failureBuilder);
            }

            return new(new OciModuleReference(artifactReference, parentModuleUri));
        }
    }
}
