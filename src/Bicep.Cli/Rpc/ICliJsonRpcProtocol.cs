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
        string? ManagementGroupId,
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

public record FormatRequest(
    string Path);

public record FormatResponse(
    string Contents);

/// <summary>
/// Requests rendered documentation for one or more modules.
/// </summary>
/// <remarks>
/// This record supports the experimental <c>bicep docs</c> command group and may change while
/// that feature remains experimental, notwithstanding the stability guarantee for the rest of <see cref="ICliJsonRpcProtocol"/>.
/// </remarks>
public record RenderDocsRequest(
    ImmutableArray<string> Paths,
    string? TemplateFile,
    string? TemplateRoot,
    Dictionary<string, string>? CustomTemplateValues,
    bool NoRestore);

/// <summary>
/// Contains rendered documentation and diagnostics for one module.
/// </summary>
/// <remarks>
/// This record supports the experimental <c>bicep docs</c> command group and may change while
/// that feature remains experimental, notwithstanding the stability guarantee for the rest of <see cref="ICliJsonRpcProtocol"/>.
/// </remarks>
public record DocsResult(
    string Path,
    bool Success,
    ImmutableArray<DiagnosticDefinition> Diagnostics,
    string? Contents);

/// <summary>
/// Contains rendered documentation for all requested modules, in request order.
/// </summary>
/// <remarks>
/// This record supports the experimental <c>bicep docs</c> command group and may change while
/// that feature remains experimental, notwithstanding the stability guarantee for the rest of <see cref="ICliJsonRpcProtocol"/>.
/// </remarks>
public record RenderDocsResponse(
    ImmutableArray<DocsResult> Results);

/// <summary>
/// Requests the typed documentation model for one or more modules.
/// </summary>
/// <remarks>
/// This record supports the experimental <c>bicep docs</c> command group and may change while
/// that feature remains experimental, notwithstanding the stability guarantee for the rest of <see cref="ICliJsonRpcProtocol"/>.
/// </remarks>
public record GetDocsModelRequest(
    ImmutableArray<string> Paths,
    bool NoRestore);

/// <summary>
/// Contains the documentation model and diagnostics for one module.
/// </summary>
/// <remarks>
/// This record supports the experimental <c>bicep docs</c> command group and may change while
/// that feature remains experimental, notwithstanding the stability guarantee for the rest of <see cref="ICliJsonRpcProtocol"/>.
/// </remarks>
public record DocsModelResult(
    string Path,
    bool Success,
    ImmutableArray<DiagnosticDefinition> Diagnostics,
    DocsModelDefinition? Model);

/// <summary>
/// Contains documentation models for all requested modules, in request order.
/// </summary>
/// <remarks>
/// This record supports the experimental <c>bicep docs</c> command group and may change while
/// that feature remains experimental, notwithstanding the stability guarantee for the rest of <see cref="ICliJsonRpcProtocol"/>.
/// </remarks>
public record GetDocsModelResponse(
    ImmutableArray<DocsModelResult> Results);

/// <summary>
/// The documentation model for one Bicep module. This is a projection of the compiler's internal
/// documentation model onto an explicit protocol contract.
/// </summary>
/// <remarks>
/// This record supports the experimental <c>bicep docs</c> command group and may change while
/// that feature remains experimental, notwithstanding the stability guarantee for the rest of <see cref="ICliJsonRpcProtocol"/>.
/// </remarks>
public record DocsModelDefinition(
    string Name,
    string? Description,
    string Path,
    string TargetScope,
    ImmutableSortedDictionary<string, string> Custom,
    ImmutableArray<DocsModelDefinition.ResourceTypeDefinition> ResourceTypes,
    ImmutableArray<DocsModelDefinition.ParameterDefinition> Parameters,
    ImmutableArray<DocsModelDefinition.OutputDefinition> Outputs,
    ImmutableArray<DocsModelDefinition.ExportDefinition> ExportedTypes,
    ImmutableArray<DocsModelDefinition.ExportDefinition> ExportedVariables,
    ImmutableArray<DocsModelDefinition.FunctionDefinition> ExportedFunctions,
    ImmutableArray<DocsModelDefinition.ReferenceDefinition> References,
    ImmutableArray<DocsModelDefinition.UsageExampleDefinition> UsageExamples)
{
    public record ResourceTypeDefinition(
        string Type,
        bool IsExisting);

    public record ParameterDefinition(
        string Name,
        string TypeName,
        bool IsRequired,
        bool IsSecure,
        string? Description,
        string? DefaultValue,
        ImmutableArray<string> AllowedValues,
        long? MinValue,
        long? MaxValue,
        long? MinLength,
        long? MaxLength,
        string? Pattern,
        bool IsTruncated,
        ImmutableArray<ParameterDefinition> NestedProperties,
        DiscriminatorDefinition? Discriminator);

    public record DiscriminatorDefinition(
        string PropertyName,
        ImmutableArray<DiscriminatorCaseDefinition> Cases);

    public record DiscriminatorCaseDefinition(
        string Value,
        ImmutableArray<ParameterDefinition> Properties);

    public record OutputDefinition(
        string Name,
        string TypeName,
        bool IsSecure,
        string? Description);

    public record ExportDefinition(
        string Name,
        string TypeName,
        bool IsSecure,
        string? Description,
        ImmutableArray<string> AllowedValues,
        long? MinValue,
        long? MaxValue,
        long? MinLength,
        long? MaxLength,
        string? Pattern,
        bool IsTruncated,
        ImmutableArray<ParameterDefinition> NestedProperties,
        DiscriminatorDefinition? Discriminator);

    public record FunctionDefinition(
        string Name,
        ImmutableArray<FunctionParameterDefinition> Parameters,
        string ReturnTypeName,
        string? Description);

    public record FunctionParameterDefinition(
        string Name,
        string TypeName,
        string? Description);

    public record ReferenceDefinition(
        string SymbolicName,
        string? Path,
        string? Description);

    public record UsageExampleDefinition(
        string Name,
        string RelativePath,
        string? Description,
        string Contents);
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

    /// <summary>
    /// Formats a specified .bicep file.
    /// </summary>
    [JsonRpcMethod("bicep/format", UseSingleObjectParameterDeserialization = true)]
    Task<FormatResponse> Format(FormatRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Renders documentation for one or more Bicep modules. Rendered content is returned to the caller
    /// and is never written to disk.
    /// </summary>
    /// <remarks>
    /// This operation supports the experimental <c>bicep docs</c> command group and may change while
    /// that feature remains experimental, notwithstanding the stability guarantee for the rest of this interface.
    /// </remarks>
    [JsonRpcMethod("bicep/renderDocs", UseSingleObjectParameterDeserialization = true)]
    Task<RenderDocsResponse> RenderDocs(RenderDocsRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the typed documentation model for one or more Bicep modules, before any template is applied.
    /// </summary>
    /// <remarks>
    /// This operation supports the experimental <c>bicep docs</c> command group and may change while
    /// that feature remains experimental, notwithstanding the stability guarantee for the rest of this interface.
    /// </remarks>
    [JsonRpcMethod("bicep/getDocsModel", UseSingleObjectParameterDeserialization = true)]
    Task<GetDocsModelResponse> GetDocsModel(GetDocsModelRequest request, CancellationToken cancellationToken);
}
