// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.IO.Abstraction;
using Bicep.IO.Utils;

namespace Bicep.Core.SourceGraph.Artifacts
{
    internal class LocalExtensionArtifact : ExternalArtifact, IExtensionArtifact
    {
        // If another process holds the lock on the cache directory, retry until this timeout is reached.
        private static readonly TimeSpan LockContentionTimeout = TimeSpan.FromSeconds(30);

        private static readonly TimeSpan LockContentionRetryInterval = TimeSpan.FromMilliseconds(300);

        public LocalExtensionArtifact(BinaryData extensionBinaryData, IDirectoryHandle rootCacheDirectory)
            : base(ResolveCacheDirectory(extensionBinaryData, rootCacheDirectory))
        {
        }

        public IFileHandle TypesTgzFile => this.GetFile("types.tgz");

        public IFileHandle BinaryFile => this.GetFile("extension.bin");

        private IFileHandle LockFile => this.GetFile("lock");

        /// <summary>
        /// Gets a value indicating whether the extension package has been unpacked into the cache.
        /// </summary>
        public bool IsUnpacked => this.TypesTgzFile.Exists();

        /// <summary>
        /// Unpacks the extension package into the cache, unless another compilation (possibly in another process) already has.
        /// </summary>
        /// <exception cref="ExternalArtifactException">The cache directory could not be created or locked.</exception>
        /// <exception cref="InvalidOperationException">The extension does not support the current OS or architecture.</exception>
        public void Unpack(ExtensionPackage package, string extensionReference)
        {
            try
            {
                this.CacheDirectory.EnsureExists();
            }
            catch (Exception exception)
            {
                throw new ExternalArtifactException($"Unable to create the local artifact directory \"{this.CacheDirectory.Uri}\". {exception.Message}", exception);
            }

            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed < LockContentionTimeout)
            {
                using (var @lock = this.LockFile.TryLock())
                {
                    if (@lock is not null)
                    {
                        if (!this.IsUnpacked)
                        {
                            this.WriteContent(package, extensionReference);
                        }

                        return;
                    }
                }

                // Another process is writing the same content; give it time to finish.
                Thread.Sleep(LockContentionRetryInterval);
            }

            throw new ExternalArtifactException($"Exceeded the timeout of \"{LockContentionTimeout}\" to acquire the lock on file \"{this.LockFile.Uri}\".");
        }

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
