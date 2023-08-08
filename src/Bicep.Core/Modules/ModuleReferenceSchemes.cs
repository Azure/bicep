// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Oci;

namespace Bicep.Core.Modules
{
    public static class ModuleReferenceSchemes
    {
        public const string Local = "";

        public const string Oci = OciArtifactReference.Scheme;

        public const string TemplateSpecs = "ts";
    }
}
