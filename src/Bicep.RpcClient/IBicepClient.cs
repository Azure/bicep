// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using Bicep.RpcClient.Models;

namespace Bicep.RpcClient;

public interface IBicepClient : IDisposable
{
    /// <summary>
    /// Compiles a Bicep file into an ARM template.
    /// </summary>
    Task<CompileResponse> Compile(CompileRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compiles a Bicepparam file into an ARM parameters file.
    /// </summary>
    Task<CompileParamsResponse> CompileParams(CompileParamsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Formats a Bicep file.
    /// </summary>
    Task<FormatResponse> Format(FormatRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Renders documentation for one or more Bicep modules. Rendered content is returned to the caller
    /// and is never written to disk.
    /// </summary>
    /// <remarks>
    /// This operation supports the experimental <c>bicep docs</c> command group and may change while
    /// that feature remains experimental.
    /// </remarks>
    Task<RenderDocsResponse> RenderDocs(RenderDocsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the typed documentation model for one or more Bicep modules, before any template is applied.
    /// </summary>
    /// <remarks>
    /// This operation supports the experimental <c>bicep docs</c> command group and may change while
    /// that feature remains experimental.
    /// </remarks>
    Task<GetDocsModelResponse> GetDocsModel(GetDocsModelRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the deployment graph for a Bicep file.
    /// </summary>
    Task<GetDeploymentGraphResponse> GetDeploymentGraph(GetDeploymentGraphRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns file references for a Bicep file.
    /// </summary>
    Task<GetFileReferencesResponse> GetFileReferences(GetFileReferencesRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns metadata for a Bicep file.
    /// </summary>
    Task<GetMetadataResponse> GetMetadata(GetMetadataRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a snapshot of a Bicep parameters file.
    /// </summary>
    Task<GetSnapshotResponse> GetSnapshot(GetSnapshotRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the version of the Bicep CLI.
    /// </summary>
    Task<string> GetVersion(CancellationToken cancellationToken = default);
}
