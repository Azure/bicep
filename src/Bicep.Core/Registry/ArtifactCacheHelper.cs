// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Core.Registry.Oci;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry;

/// <summary>
/// Provides shared utilities for artifact cache operations including file-system locking and cache path encoding.
/// Used by both <see cref="ExternalArtifactRegistry{TArtifactReference, TArtifactEntity}"/> and
/// other consumers (e.g., MCP server) to ensure consistent concurrency handling when writing to the shared artifact cache.
/// </summary>
public static class ArtifactCacheHelper
{
    // if we're unable to acquire a lock on the artifact directory in the cache, we will retry until this timeout is reached
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

    // interval at which we will retry acquiring the lock on the artifact directory in the cache
    public static readonly TimeSpan DefaultRetryInterval = TimeSpan.FromMilliseconds(300);

    /// <summary>
    /// Executes a write operation under a file-based lock, with retry and double-check semantics.
    /// If the lock cannot be acquired within the timeout, throws <see cref="ExternalArtifactException"/>.
    /// </summary>
    /// <param name="lockFile">The file handle to use as the lock. Created via <see cref="IFileHandle.TryLock"/>.</param>
    /// <param name="isWriteRequired">Returns false if another process has already completed the write (double-check pattern).</param>
    /// <param name="writeContent">The action to execute while holding the lock.</param>
    /// <param name="timeout">Maximum time to wait for the lock. Defaults to <see cref="DefaultTimeout"/>.</param>
    /// <param name="retryInterval">Interval between lock acquisition retries. Defaults to <see cref="DefaultRetryInterval"/>.</param>
    public static async Task WriteWithLockAsync(
        IFileHandle lockFile,
        Func<bool> isWriteRequired,
        Action writeContent,
        TimeSpan? timeout = null,
        TimeSpan? retryInterval = null)
    {
        var effectiveTimeout = timeout ?? DefaultTimeout;
        var effectiveRetryInterval = retryInterval ?? DefaultRetryInterval;
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < effectiveTimeout)
        {
            using (var @lock = lockFile.TryLock())
            {
                // the placement of "if" inside "using" guarantees that even an exception thrown by the condition results in the lock being released
                // (current condition can't throw, but this potentially avoids future regression)
                if (@lock is not null)
                {
                    if (!isWriteRequired())
                    {
                        // the other instance has already written out the content to disk - we can discard the content we downloaded
                        return;
                    }

                    // write the contents to disk
                    writeContent();
                    return;
                }
            }

            // we have not acquired the lock - let's give the instance that has the lock some time to finish writing the content to the directory
            // (the operation involves only writing the already downloaded content to disk, so it "should" complete fairly quickly)
            await Task.Delay(effectiveRetryInterval);
        }

        // we have exceeded the timeout
        throw new ExternalArtifactException($"Exceeded the timeout of \"{effectiveTimeout}\" to acquire the lock on file \"{lockFile.Uri}\".");
    }

    /// <summary>
    /// Encodes OCI artifact address components into a relative cache path.
    /// We need to split each component of the reference into a sub directory to fit within the max file name length limit on linux and mac.
    /// </summary>
    /// <returns>A relative path like "mcr.microsoft.com/bicep$extensions$microsoftgraph$v1.0/1.0.0$".</returns>
    public static string EncodeCachePathSegments(string registry, string repository, string? tag, string? digest)
    {
        // TODO: Need to deal with IDNs (internationalized domain names)
        // domain names can only contain alphanumeric chars, _, -, and numbers (example.azurecr.io or example.azurecr.io:443)
        // IPV4 addresses only contain dots and numbers (127.0.0.1 or 127.0.0.1:5000)
        // IPV6 addresses are hex digits with colons (2001:db8:3333:4444:CCCC:DDDD:EEEE:FFFF or [2001:db8:3333:4444:CCCC:DDDD:EEEE:FFFF]:5000)
        // the only problematic character is the colon, which we will replace with $ which is not allowed in any of the possible registry values
        // we will also normalize casing for registries since they are case-insensitive
        var encodedRegistry = registry.Replace(':', '$').ToLowerInvariant();

        // modules can have mixed hierarchy depths so we will flatten a repository into a single directory name
        // however to do this we must get rid of slashes (not a valid file system char on windows and a directory separator on linux/mac)
        // the replacement char must one that is not valid in a repository string but is valid in common type systems
        var encodedRepository = repository.Replace('/', '$');

        // Tag or digest: exactly one must be provided
        string encodedTagOrDigest;
        if (tag is not null)
        {
            // tags are case-sensitive with length up to 128
            encodedTagOrDigest = TagEncoder.Encode(tag);
        }
        else if (digest is not null)
        {
            // digests are strings like "sha256:e207a69d02b3de40d48ede9fd208d80441a9e590a83a0bc915d46244c03310d4"
            // and are already guaranteed to be lowercase
            // the only problematic character is the : which we will replace with a #
            // however the encoding we choose must not be ambiguous with the tag encoding
            encodedTagOrDigest = digest.Replace(':', '#');
        }
        else
        {
            throw new InvalidOperationException("Reference is missing both tag and digest.");
        }

        return $"{encodedRegistry}/{encodedRepository}/{encodedTagOrDigest}";
    }
}
