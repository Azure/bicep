// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.Codecs;
using Bicep.Core.Artifacts.Packages;
using Bicep.Core.Extensions;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceGraph.Artifacts;
using Bicep.IO.Abstraction;
using Bicep.IO.Utils;

namespace Bicep.Core.Artifacts.Cache
{
    /// <summary>
    /// The cache entry of a local extension package, unpacked under <c>local/sha256_&lt;digest of the package&gt;/</c>.
    /// </summary>
    internal class LocalExtensionArtifact : ExternalArtifact, IExtensionArtifact
    {
        public LocalExtensionArtifact(BinaryData extensionBinaryData, IDirectoryHandle rootCacheDirectory)
            : base(ResolveCacheDirectory(extensionBinaryData, rootCacheDirectory))
        {
        }

        public IFileHandle TypesTgzFile => this.GetFile("types.tgz");

        public IFileHandle BinaryFile => this.GetFile("extension.bin");

        public override IFileHandle EntryPoint => this.TypesTgzFile;

        public override bool IsComplete => this.TypesTgzFile.Exists();

        /// <summary>
        /// Unpacks the extension package into the cache, unless another compilation (possibly in another process) already has.
        /// </summary>
        /// <exception cref="ExternalArtifactException">The cache directory could not be created or locked.</exception>
        /// <exception cref="InvalidOperationException">The extension does not support the current OS or architecture.</exception>
        public void Unpack(ExtensionPackage package, string extensionReference) => this.Write(() => this.WriteContent(package, extensionReference));

        private void WriteContent(ExtensionPackage package, string extensionReference)
        {
            if (package.LocalDeployEnabled)
            {
                if (SupportedArchitectures.TryGetCurrent() is not { } architecture)
                {
                    throw new InvalidOperationException($"Failed to determine the system OS or architecture to execute extension \"{extensionReference}\".");
                }

                if (package.Binaries.SingleOrDefault(x => x.Architecture.Name == architecture.Name) is not { } binary)
                {
                    throw new InvalidOperationException($"The extension \"{extensionReference}\" does not support architecture {architecture.Name}.");
                }

                this.BinaryFile.Write(binary.Data);
                this.BinaryFile.MakeExecutable();
            }

            // types.tgz is written last: its presence marks the cache entry as complete.
            this.TypesTgzFile.Write(package.Types);
        }

        private static IDirectoryHandle ResolveCacheDirectory(BinaryData extensionBinaryData, IDirectoryHandle rootCacheDirectory)
        {
            // The cache directory is structured as "local/{extensionDigest}"
            // where extensionDigest is a unique identifier for the extension.
            var digest = OciDescriptor.ComputeDigest(OciDescriptor.AlgorithmIdentifierSha256, extensionBinaryData, separator: '_');

            return rootCacheDirectory.GetDirectory($"local/{digest}");
        }
    }
}
