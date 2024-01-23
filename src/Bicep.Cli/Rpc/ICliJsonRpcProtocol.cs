// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
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
    ImmutableArray<CompileResponse.DiagnosticDefinition> Diagnostics,
    string? Contents)
{
    public record DiagnosticDefinition(
        Range Range,
        string Level,
        string Code,
        string Message);
}

public record GetFileReferencesRequest(
    string Path);

public record GetFileReferencesResponse(
    ImmutableArray<string> FilePaths);

public record GetMetadataRequest(
    string Path);

public record GetMetadataResponse(
    ImmutableArray<GetMetadataResponse.MetadataDefinition> Metadata,
    ImmutableArray<GetMetadataResponse.SymbolDefinition> Parameters,
    ImmutableArray<GetMetadataResponse.SymbolDefinition> Outputs)
{
    public record SymbolDefinition(
        Range Range,
        string Name,
        TypeDefinition? Type,
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

public interface ICliJsonRpcProtocol
{
    [JsonRpcMethod("bicep/version", UseSingleObjectParameterDeserialization = true)]
    Task<VersionResponse> Version(VersionRequest request, CancellationToken cancellationToken);

    [JsonRpcMethod("bicep/compile", UseSingleObjectParameterDeserialization = true)]
    Task<CompileResponse> Compile(CompileRequest request, CancellationToken cancellationToken);

    [JsonRpcMethod("bicep/getMetadata", UseSingleObjectParameterDeserialization = true)]
    Task<GetMetadataResponse> GetMetadata(GetMetadataRequest request, CancellationToken cancellationToken);

    [JsonRpcMethod("bicep/getDeploymentGraph", UseSingleObjectParameterDeserialization = true)]
    Task<GetDeploymentGraphResponse> GetDeploymentGraph(GetDeploymentGraphRequest request, CancellationToken cancellationToken);

    [JsonRpcMethod("bicep/getFileReferences", UseSingleObjectParameterDeserialization = true)]
    Task<GetFileReferencesResponse> GetFileReferences(GetFileReferencesRequest request, CancellationToken cancellationToken);
}
