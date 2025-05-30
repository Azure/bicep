// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Newtonsoft.Json.Linq;
using StreamJsonRpc;

namespace Bicep.Cli.Rpc;

public record Position(
    int Line,
    int Char);

public record Range(
    Position Start,
    Position End);

public record VersionRequest();

public record VersionResponse(
    string Version);

public record CompileRequest(
    string Path);

public record CompileResponse(
    bool Success,
    ImmutableArray<DiagnosticDefinition> Diagnostics,
    string? Contents);

public record CompileParamsRequest(
    string Path,
    Dictionary<string, JToken> ParameterOverrides);

public record CompileParamsResponse(
    bool Success,
    ImmutableArray<DiagnosticDefinition> Diagnostics,
    string? Parameters,
    string? Template,
    string? TemplateSpecId);

public record DiagnosticDefinition(
    string Source,
    Range Range,
    string Level,
    string Code,
    string Message);

public record GetFileReferencesRequest(
    string Path);

public record GetFileReferencesResponse(
    ImmutableArray<string> FilePaths);

public record GetMetadataRequest(
    string Path);

public record GetSnapshotRequest(
    string Path,
    GetSnapshotRequest.MetadataDefinition Metadata,
    ImmutableArray<GetSnapshotRequest.ExternalInputValue>? ExternalInputs)
{
    public record MetadataDefinition(
        string? TenantId,
        string? SubscriptionId,
        string? ResourceGroup,
        string? Location,
        string? DeploymentName);

    public record ExternalInputValue(
        string Kind,
        JToken? Config,
        JToken Value);
}

public record GetSnapshotResponse(
    string Snapshot);

public record GetMetadataResponse(
    ImmutableArray<GetMetadataResponse.MetadataDefinition> Metadata,
    ImmutableArray<GetMetadataResponse.SymbolDefinition> Parameters,
    ImmutableArray<GetMetadataResponse.SymbolDefinition> Outputs,
    ImmutableArray<GetMetadataResponse.ExportDefinition> Exports)
{
    public record SymbolDefinition(
        Range Range,
        string Name,
        TypeDefinition? Type,
        string? Description);

    public record ExportDefinition(
        Range Range,
        string Name,
        string Kind,
        string? Description);

    public record TypeDefinition(
        Range? Range,
        string Name);

    public record MetadataDefinition(
        string Name,
        string Value);
}

public record GetDeploymentGraphRequest(
    string Path);

public record GetDeploymentGraphResponse(
    ImmutableArray<GetDeploymentGraphResponse.Node> Nodes,
    ImmutableArray<GetDeploymentGraphResponse.Edge> Edges)
{
    public record Node(
        Range Range,
        string Name,
        string Type,
        bool IsExisting,
        string? RelativePath);

    public record Edge(
        string Source,
        string Target);
}

/// <summary>
/// The definition for the Bicep CLI JSONRPC interface.
/// </summary>
/// <remarks>
/// As of Bicep 0.29, this interface is no longer "experimental". Please consider carefully whether you are making a change that may break backwards compatibility.
/// </remarks>
public interface ICliJsonRpcProtocol
{
    /// <summary>
    /// Returns the version of the Bicep CLI.
    /// </summary>
    [JsonRpcMethod("bicep/version", UseSingleObjectParameterDeserialization = true)]
    Task<VersionResponse> Version(VersionRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Compiles a specified .bicep file.
    /// </summary>
    [JsonRpcMethod("bicep/compile", UseSingleObjectParameterDeserialization = true)]
    Task<CompileResponse> Compile(CompileRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Compiles a specified .bicepparam file.
    /// </summary>
    [JsonRpcMethod("bicep/compileParams", UseSingleObjectParameterDeserialization = true)]
    Task<CompileParamsResponse> CompileParams(CompileParamsRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Returns metadata about a specified .bicep file.
    /// </summary>
    [JsonRpcMethod("bicep/getMetadata", UseSingleObjectParameterDeserialization = true)]
    Task<GetMetadataResponse> GetMetadata(GetMetadataRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the deployment graph for a specified .bicep file.
    /// </summary>
    [JsonRpcMethod("bicep/getDeploymentGraph", UseSingleObjectParameterDeserialization = true)]
    Task<GetDeploymentGraphResponse> GetDeploymentGraph(GetDeploymentGraphRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the full list of file paths that are referenced by a compilation. Useful to determine a set of files to watch for changes.
    /// </summary>
    [JsonRpcMethod("bicep/getFileReferences", UseSingleObjectParameterDeserialization = true)]
    Task<GetFileReferencesResponse> GetFileReferences(GetFileReferencesRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a snapshot for a given parameter file.
    /// </summary>
    [JsonRpcMethod("bicep/getSnapshot", UseSingleObjectParameterDeserialization = true)]
    Task<GetSnapshotResponse> GetSnapshot(GetSnapshotRequest request, CancellationToken cancellationToken);
}
