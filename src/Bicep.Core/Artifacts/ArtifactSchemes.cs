// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Artifacts
{
    public static class ArtifactSchemes
    {
        /// <summary>
        /// The original scheme for OCI artifacts. Fully qualified OCI references are persisted with it (source archives,
        /// ext-src URIs and the <c>br</c> folder of the artifact cache), so it never changes.
        /// </summary>
        public const string Br = "br";

        /// <summary>
        /// The recommended scheme for OCI artifacts; another spelling of <see cref="Br"/>.
        /// </summary>
        public const string Oci = "oci";

        public const string TemplateSpecs = "ts";

        public const string File = "file";
    }
}