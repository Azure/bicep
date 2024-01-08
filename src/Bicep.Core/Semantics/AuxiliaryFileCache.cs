// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Utils;
using System.Diagnostics;
using System.Linq;
using Bicep.Core.Extensions;

namespace Bicep.Core.Semantics;

public record AuxiliaryFile(
    Uri FileUri,
    byte[] Content);

public interface IReadableFileCache
{
    ResultWithDiagnostic<AuxiliaryFile> Read(ISemanticModel sourceModel, Uri uri);
}

public class AuxiliaryFileCache : IReadableFileCache
{
    public record CacheEntry(
        ImmutableHashSet<ISemanticModel> References,
        ResultWithDiagnostic<AuxiliaryFile> Result);

    public AuxiliaryFileCache(IFileResolver fileResolver)
    {
        this.fileResolver = fileResolver;
    }

    private readonly ConcurrentDictionary<Uri, CacheEntry> fileCache = new();
    private readonly IFileResolver fileResolver;

    /// <summary>
    /// Reads a given file from the file system, utilizing the cache where possible.
    /// </summary>
    public ResultWithDiagnostic<AuxiliaryFile> Read(ISemanticModel sourceModel, Uri uri)
    {
        var cacheEntry = fileCache.AddOrUpdate(
            uri,
            uri => {
                var result = fileResolver.TryReadAsBytes(uri)
                    .Transform(bytes => new AuxiliaryFile(uri, bytes));

                Trace.WriteLine($"Loaded auxiliary file {uri}. Success: {result.IsSuccess()}");

                return new(
                    References: sourceModel.AsEnumerable().ToImmutableHashSet(),
                    Result: result);
            },
            (uri, entry) => {
                return new(
                    References: entry.References.Add(sourceModel),
                    Result: entry.Result);
            });

        return cacheEntry.Result;
    }

    /// <summary>
    /// Removes references to semantic models that are no longer active.
    /// </summary>
    public void RemoveStaleEntries(IEnumerable<ISemanticModel> activeModels)
    {
        foreach (var kvp in fileCache)
        {
            var newEntry = fileCache.AddOrUpdate(
                kvp.Key,
                kvp.Value,
                (uri, entry) => new(
                    References: entry.References.Intersect(activeModels),
                    Result: entry.Result));

            // There's a race condition where the above could re-add an entry that was removed by another thread.
            // To mitigate this, we can remove it here if the entry is the same reference.
            if (object.ReferenceEquals(newEntry, kvp.Value) ||
                newEntry.References.IsEmpty)
            {
                fileCache.TryRemove(kvp);
            }
        }
    }

    /// <summary>
    /// Clears the cache for the specified files, to force them to be reloaded from the file system.
    /// </summary>
    public ImmutableArray<ISemanticModel> ClearEntries(IEnumerable<Uri> fileUris)
    {
        HashSet<ISemanticModel> affectedModels = new();
        foreach (var fileUri in fileUris)
        {
            if (fileCache.TryRemove(fileUri, out var cacheEntry))
            {
                affectedModels.UnionWith(cacheEntry.References);
            }
        }

        return affectedModels.ToImmutableArray();
    }
}