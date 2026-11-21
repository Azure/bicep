// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.IO.Abstraction;

/// <summary>
/// Marks periods during which files are assumed not to change, so caches check what they've read against the file system
/// once per epoch (see <see cref="FileDependencies"/>). Hosts start a new epoch when files may have changed:
/// <list type="bullet">
/// <item>The language server, for each batch of watched-file events and when a configuration file is saved.</item>
/// <item><c>bicep jsonrpc</c> and the MCP server, at the start of each request, because they don't watch the file system.</item>
/// <item>One-shot CLI commands never do: nothing they read changes while they run.</item>
/// </list>
/// Caches created without one never check their entries again, like a one-shot command.
/// </summary>
public sealed class FileChangeEpoch
{
    private long current;

    public long Current => Interlocked.Read(ref this.current);

    public void StartNew() => Interlocked.Increment(ref this.current);
}