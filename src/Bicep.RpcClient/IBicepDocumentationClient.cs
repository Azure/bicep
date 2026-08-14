// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using Bicep.RpcClient.Models;

namespace Bicep.RpcClient;

/// <summary>
/// Provides module documentation operations for compatible Bicep clients.
/// </summary>
public interface IBicepDocumentationClient
{
    /// <summary>
    /// Generates documentation files for Bicep modules.
    /// </summary>
    /// <param name="request">The modules and rendering options.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>One result for each requested module.</returns>
    Task<GenerateDocsResponse> GenerateDocs(GenerateDocsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Renders documentation for one Bicep module.
    /// </summary>
    /// <param name="request">The module and rendering options.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>The rendered content and diagnostics.</returns>
    Task<OutputDocsResponse> OutputDocs(OutputDocsRequest request, CancellationToken cancellationToken = default);
}
