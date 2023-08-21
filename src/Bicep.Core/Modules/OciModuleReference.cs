// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry.Oci;
using System;
using System.Diagnostics.CodeAnalysis;


namespace Bicep.Core.Modules
{
    /// <summary>
    /// Represents a reference to an artifact in an OCI registry.
    /// </summary>
    public class OciModuleReference : ModuleReference, IOciArtifactReference
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

        public static bool TryParse(
            string? aliasName,
            string rawValue,
            RootConfiguration configuration,
            Uri parentModuleUri,
            [NotNullWhen(true)] out OciModuleReference? moduleReference,
            [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            if (OciArtifactReference.TryParse(aliasName, rawValue, configuration, out var artifactReference, out failureBuilder))
            {
                moduleReference = new OciModuleReference(artifactReference, parentModuleUri);
                return true;
            }
            moduleReference = null;
            return false;
        }
    }
}
