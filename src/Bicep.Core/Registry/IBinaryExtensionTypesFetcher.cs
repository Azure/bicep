// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry
{
    /// <summary>
    /// Result returned by <see cref="IBinaryExtensionTypesFetcher"/> after fetching types from a
    /// local extension binary.
    /// </summary>
    /// <param name="TypesTgz">The types.tgz payload to be written to the Bicep cache.</param>
    /// <param name="ResolvedBinaryPath">The absolute file system path of the resolved binary (after PATH lookup).</param>
    public record BinaryExtensionRestoreEntity(BinaryData TypesTgz, string ResolvedBinaryPath);

    /// <summary>
    /// Fetches the types.tgz payload from a local extension binary by launching it and querying its gRPC endpoint.
    /// Implemented in Bicep.Local.Deploy so that Bicep.Core does not take a direct dependency on the gRPC stack.
    /// </summary>
    public interface IBinaryExtensionTypesFetcher
    {
        /// <summary>
        /// Resolves the binary specified by <paramref name="rawValue"/> (absolute path or PATH name),
        /// starts it, queries its type index, and returns the packaged types.tgz together with
        /// the resolved absolute binary path.
        /// </summary>
        Task<BinaryExtensionRestoreEntity> FetchTypesTgzAndResolvePath(string rawValue, CancellationToken cancellationToken);
    }
}

