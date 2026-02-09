// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json.Nodes;

namespace Bicep.RpcClient.Models;

// Data models for all RPC requests and responses.
// These models should be kept in sync with the corresponding models in the Bicep CLI, but must also be backwards-compatible,
// as the JSONRPC client is able to communicate with multiple versions of the Bicep CLI.

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
    Dictionary<string, JsonNode> ParameterOverrides);

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
        JsonNode? Config,
        JsonNode Value);
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

public record FormatRequest(
    string Path);

public record FormatResponse(
    string Contents);
