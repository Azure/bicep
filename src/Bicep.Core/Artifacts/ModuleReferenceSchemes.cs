// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.References;

namespace Bicep.Core.Artifacts
{
    public static class ArtifactReferenceSchemes
    {
        public const string Local = "";

        public const string Oci = OciArtifactReferenceFacts.Scheme;

        public const string TemplateSpecs = "ts";
    }
}
