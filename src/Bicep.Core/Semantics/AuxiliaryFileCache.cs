// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Utils;

namespace Bicep.Core.Semantics;

public record AuxiliaryFile(
    Uri FileUri,
    BinaryData Content);

public interface IReadableFileCache
{
    ResultWithDiagnostic<AuxiliaryFile> Read(Uri uri);
}

public class AuxiliaryFileCache : IReadableFileCache
{
    public AuxiliaryFileCache(IFileResolver fileResolver)
    {
        this.fileResolver = fileResolver;
    }

    private readonly ConcurrentDictionary<Uri, ResultWithDiagnostic<AuxiliaryFile>> fileCache = new();
    private readonly IFileResolver fileResolver;

    /// <summary>
    /// Reads a given file from the file system, utilizing the cache where possible.
    /// </summary>
    public ResultWithDiagnostic<AuxiliaryFile> Read(Uri uri)
        => fileCache.GetOrAdd(
            uri,
            uri =>
            {
                var result = fileResolver.TryReadAsBinaryData(uri)
                    .Transform(data => new AuxiliaryFile(uri, data));

                Trace.WriteLine($"Loaded auxiliary file result {uri}. Success: {result.IsSuccess()}");

                return result;
            });

    public ImmutableArray<Uri> GetEntries()
        => [.. fileCache.Keys];

    public void ClearEntries(IEnumerable<Uri> uris)
    {
        foreach (var uri in uris)
        {
            Trace.WriteLine($"Removing auxiliary file result {uri}.");

            fileCache.TryRemove(uri, out _);
        }
    }

    public void PruneInactiveEntries(IEnumerable<Uri> activeEntries)
        => ClearEntries(fileCache.Keys.Except(activeEntries));
}
