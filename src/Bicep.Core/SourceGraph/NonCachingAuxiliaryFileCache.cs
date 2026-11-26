// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph;

/// <summary>
/// An <see cref="IAuxiliaryFileCache"/> that doesn't cache, for hosts that compile on request (the CLI, <c>bicep jsonrpc</c>, the MCP server).
/// A long-running host has no way to tell when a cached file changes, so files must be read again for each compilation.
/// Within a compilation, each <c>load*()</c> call reads its file once while it's type checked, and emit reuses that result.
/// </summary>
public sealed class NonCachingAuxiliaryFileCache : IAuxiliaryFileCache
{
    public FrozenSet<IOUri> Keys => [];

    public ResultWithDiagnosticBuilder<AuxiliaryFile> GetOrAdd(IOUri key, Func<ResultWithDiagnosticBuilder<AuxiliaryFile>> valueFactory) => valueFactory();

    public ResultWithDiagnosticBuilder<AuxiliaryFile>? TryRemove(IOUri key) => null;

    public IEnumerable<ResultWithDiagnosticBuilder<AuxiliaryFile>> Trim(IEnumerable<IOUri> keys) => [];
}
