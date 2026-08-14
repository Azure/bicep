// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Cli.Services;

/// <summary>
/// Writes generated module documentation.
/// </summary>
public interface IDocsFileWriter
{
    /// <summary>
    /// Writes a complete document to the specified file.
    /// </summary>
    /// <param name="outputUri">The destination file.</param>
    /// <param name="contents">The rendered document.</param>
    /// <param name="cancellationToken">Cancels the write.</param>
    Task WriteAsync(IOUri outputUri, string contents, CancellationToken cancellationToken = default);
}

/// <summary>
/// Writes documentation with atomic file replacement.
/// </summary>
public class DocsFileWriter(OutputWriter writer) : IDocsFileWriter
{
    /// <inheritdoc/>
    public Task WriteAsync(IOUri outputUri, string contents, CancellationToken cancellationToken = default) =>
        writer.WriteToFileAtomicallyAsync(outputUri, contents, cancellationToken);
}
