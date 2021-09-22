// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Oci;
using System.IO;

namespace Bicep.Core.Registry
{
    public class OciArtifactResult
    {
        public OciArtifactResult(string manifestDigest, OciManifest manifest, Stream manifestStream, Stream moduleStream)
        {
            this.ManifestDigest = manifestDigest;
            this.Manifest = manifest;
            this.ManifestStream = manifestStream;
            this.ModuleStream = moduleStream;
        }

        public string ManifestDigest { get; }

        /// <summary>
        /// Gets the deserialized manifest. This is useful for accessing various properties inside the manifest.
        /// </summary>
        public OciManifest Manifest { get; }

        /// <summary>
        /// Gets the original manifest bytes. This is useful for persisting the exact copy of the manifest that is agnostic of (de)serialization settings.
        /// </summary>
        public Stream ManifestStream { get; }

        /// <summary>
        /// Gets the stream containing the module contents.
        /// </summary>
        public Stream ModuleStream { get; }
    }
}
