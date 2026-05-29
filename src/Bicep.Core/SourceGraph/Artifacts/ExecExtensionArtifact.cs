// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Security.Cryptography;
using System.Text;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph.Artifacts
{
    public class ExecExtensionArtifact : ExternalArtifact, IExtensionArtifact
    {
        private readonly IFileExplorer fileExplorer;

        public ExecExtensionArtifact(string rawValue, IDirectoryHandle rootCacheDirectory, IFileExplorer fileExplorer)
            : base(ResolveCacheDirectory(rawValue, rootCacheDirectory))
        {
            this.fileExplorer = fileExplorer;
        }

        public IFileHandle TypesTgzFile => this.GetFile("types.tgz");

        /// <summary>
        /// Returns a file handle for the resolved binary.
        /// The binary path is read from 'binary-path.txt' in the cache directory, which is written
        /// during restore by <see cref="Bicep.Core.Registry.ExecExtensionRegistry"/>.
        /// Returns a handle whose <c>Exists()</c> is <c>false</c> when the extension has not been
        /// restored yet, or when the binary no longer exists at the cached path.
        /// </summary>
        public IFileHandle BinaryFile
        {
            get
            {
                var binaryPathFile = this.GetFile("binary-path.txt");

                if (!binaryPathFile.Exists())
                {
                    // Not yet restored – return the path file as proxy so that callers that only
                    // inspect .Exists() (e.g. LocalExtensionDispatcher) skip this extension gracefully.
                    return binaryPathFile;
                }

                var absolutePath = binaryPathFile.ReadAllText().Trim();
                return fileExplorer.GetFile(IOUri.FromFilePath(absolutePath));
            }
        }

        /// <summary>
        /// Returns the cache directory for a given raw value, keyed on a SHA-256 hash of the
        /// raw value string so that different binaries are cached separately.
        /// </summary>
        internal static IDirectoryHandle ResolveCacheDirectory(string rawValue, IDirectoryHandle rootCacheDirectory)
        {
            var hash = ComputeRawValueHash(rawValue);
            return rootCacheDirectory.GetDirectory($"exec/{hash}");
        }

        private static string ComputeRawValueHash(string rawValue)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawValue));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}

