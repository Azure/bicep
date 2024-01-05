// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.Semantics;
using Bicep.Core.FileSystem;
using Bicep.Core.Utils;
using System.Diagnostics;

namespace Bicep.Core.Semantics;

public record AuxiliaryFile(
    Uri FileUri,
    byte[] Content);

public interface IReadableFileCache
{
    ResultWithDiagnostic<AuxiliaryFile> Read(SemanticModel sourceModel, Uri uri);
}

public class AuxiliaryFileCache : IReadableFileCache
{
    public record CacheEntry(
        // Use a ConcurrentDictionary with a dummy value because there's no built-in ConcurrentHashSet
        ConcurrentDictionary<SemanticModel, byte> References,
        ResultWithDiagnostic<AuxiliaryFile> Result);

    public AuxiliaryFileCache(IFileResolver fileResolver)
    {
        this.fileResolver = fileResolver;
    }

    private readonly ConcurrentDictionary<Uri, CacheEntry> fileCache = new();
    private readonly IFileResolver fileResolver;

    public ResultWithDiagnostic<AuxiliaryFile> Read(SemanticModel sourceModel, Uri uri)
    {
        var cacheEntry = fileCache.AddOrUpdate(
            uri,
            uri => {
                var result = fileResolver.TryReadAsBytes(uri)
                    .Transform(bytes => new AuxiliaryFile(uri, bytes));

                Trace.WriteLine($"Loaded auxiliary file {uri}. Success: {result.IsSuccess()}");

                return new(
                    References: new() {
                        [sourceModel] = 0,
                    },
                    Result: result);
            },
            (uri, entry) => {
                entry.References.AddOrUpdate(
                    sourceModel,
                    0,
                    (_, _) => 0);

                return entry;
            });

        return cacheEntry.Result;
    }

    public void RemoveReferences(SemanticModel model)
    {
        foreach (var kvp in fileCache)
        {
            if (kvp.Value.References.TryRemove(model, out _) &&
                kvp.Value.References.IsEmpty)
            {
                fileCache.TryRemove(kvp);
            }
        }
    }
}