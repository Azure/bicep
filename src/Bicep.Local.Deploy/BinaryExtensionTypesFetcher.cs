// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.InteropServices;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Extensions;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Bicep.Local.Deploy.Extensibility;

namespace Bicep.Local.Deploy
{
    /// <summary>
    /// Fetches type information from a local extension binary by launching it via gRPC
    /// and invoking the GetTypeFiles endpoint. The result is packaged as a types.tgz payload.
    /// </summary>
    public class BinaryExtensionTypesFetcher : IBinaryExtensionTypesFetcher
    {
        private readonly ILocalExtensionFactory localExtensionFactory;

        public BinaryExtensionTypesFetcher(ILocalExtensionFactory localExtensionFactory)
        {
            this.localExtensionFactory = localExtensionFactory;
        }

        public async Task<BinaryExtensionRestoreEntity> FetchTypesTgzAndResolvePath(string rawValue, CancellationToken cancellationToken)
        {
            var binaryPath = ResolveAbsoluteBinaryPath(rawValue)
                ?? throw new InvalidOperationException(
                    $"Cannot find the binary '{rawValue}'. " +
                    $"Ensure the binary exists at the specified path or is available on the system PATH.");

            var binaryUri = IOUri.FromFilePath(binaryPath);
            var typesTgz = await FetchTypesTgz(binaryUri, cancellationToken);

            return new BinaryExtensionRestoreEntity(typesTgz, binaryPath);
        }

        private async Task<BinaryData> FetchTypesTgz(IOUri binaryUri, CancellationToken cancellationToken)
        {
            await using var extension = await localExtensionFactory.Start(binaryUri);

            var typeFiles = await extension.GetTypeFiles(cancellationToken);

            // Build an in-memory file explorer containing the index and type files returned by the binary.
            var fileExplorer = new InMemoryFileExplorer();

            var indexUri = IOUri.FromFilePath("/index.json");
            var indexHandle = fileExplorer.GetFile(indexUri);
            indexHandle.Write(typeFiles.IndexFileContent);

            foreach (var (path, content) in typeFiles.TypeFileContents)
            {
                var fileUri = IOUri.FromFilePath($"/{path}");
                fileExplorer.GetFile(fileUri).Write(content);
            }

            return await TypesV1Archive.PackIntoBinaryData(indexHandle);
        }

        /// <summary>
        /// Resolves <paramref name="rawValue"/> to an absolute file system path.
        /// Absolute paths are returned as-is; bare names are searched on the system PATH.
        /// Returns <c>null</c> if the binary cannot be found.
        /// </summary>
        private static string? ResolveAbsoluteBinaryPath(string rawValue)
        {
            if (Path.IsPathRooted(rawValue))
            {
                return File.Exists(rawValue) ? rawValue : null;
            }

            // Search the system PATH.
            var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            var pathDirs = pathEnv.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

            foreach (var dir in pathDirs)
            {
                var candidate = Path.Combine(dir, rawValue);
                if (File.Exists(candidate))
                {
                    return candidate;
                }

                // On Windows, also try appending '.exe' if not already specified.
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                    !rawValue.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    var candidateExe = candidate + ".exe";
                    if (File.Exists(candidateExe))
                    {
                        return candidateExe;
                    }
                }
            }

            return null;
        }
    }
}

