// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Oci;
using System;

namespace Bicep.Core.TypeSystem.ResourceTypeProviders
{
    public class OcResourceTypeProviderReference : ResourceTypeProviderReference, IOciArtifactReference
    {
        private OciArtifactReference ociArtifactRef;

        public OcResourceTypeProviderReference(OciArtifactReference ociArtifactReference, Uri parentModuleUri) : base(ProviderReferenceSchemes.Oci, parentModuleUri)
        {
            this.ociArtifactRef = ociArtifactReference;
        }

        public string Registry => throw new NotImplementedException();

        public string Repository => throw new NotImplementedException();

        public string? Tag => throw new NotImplementedException();

        public string? Digest => throw new NotImplementedException();

        public string ArtifactId => throw new NotImplementedException();
        
        public override string UnqualifiedReference => throw new NotImplementedException();

        public override bool IsExternal => true;

    }
}
