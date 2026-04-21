// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json.Nodes;

namespace Bicep.RpcClient.Models;

// Data models for all RPC requests and responses.
// These models should be kept in sync with the corresponding models in the Bicep CLI, but must also be backwards-compatible,
// as the JSONRPC client is able to communicate with multiple versions of the Bicep CLI.

/// <summary>
/// Represents a zero-based position within a Bicep source file.
/// </summary>
/// <param name="Line">The zero-based line number.</param>
/// <param name="Char">The zero-based character offset within the line.</param>
public record Position(
    int Line,
    int Char);

/// <summary>
/// Represents a range within a Bicep source file, defined by a start and end position.
/// </summary>
/// <param name="Start">The start position of the range (inclusive).</param>
/// <param name="End">The end position of the range (exclusive).</param>
public record Range(
    Position Start,
    Position End);

/// <summary>
/// Request to retrieve the version of the Bicep CLI. This is a parameterless request.
/// </summary>
public record VersionRequest();

/// <summary>
/// Response containing the Bicep CLI version.
/// </summary>
/// <param name="Version">The semantic version string of the Bicep CLI (e.g., <c>"0.38.5"</c>).</param>
public record VersionResponse(
    string Version);

/// <summary>
/// Request to compile a Bicep file (<c>.bicep</c>) into an ARM template.
/// </summary>
/// <param name="Path">The file path to the Bicep file to compile (e.g., <c>"./main.bicep"</c>).</param>
public record CompileRequest(
    string Path);

/// <summary>
/// Response from compiling a Bicep file.
/// </summary>
/// <param name="Success">Whether the compilation completed without errors.</param>
/// <param name="Diagnostics">The collection of diagnostics (errors, warnings, informational messages) produced during compilation.</param>
/// <param name="Contents">The compiled ARM template JSON string, or <see langword="null"/> if compilation failed.</param>
public record CompileResponse(
    bool Success,
    ImmutableArray<DiagnosticDefinition> Diagnostics,
    string? Contents);

/// <summary>
/// Request to compile a Bicep parameters file (<c>.bicepparam</c>) into ARM deployment parameters.
/// </summary>
/// <param name="Path">The file path to the <c>.bicepparam</c> file to compile.</param>
/// <param name="ParameterOverrides">A dictionary of parameter names to JSON values that override the defaults specified in the parameters file.</param>
public record CompileParamsRequest(
    string Path,
    Dictionary<string, JsonNode> ParameterOverrides);

/// <summary>
/// Response from compiling a Bicep parameters file.
/// </summary>
/// <param name="Success">Whether the compilation completed without errors.</param>
/// <param name="Diagnostics">The collection of diagnostics produced during compilation.</param>
/// <param name="Parameters">The compiled ARM parameters JSON string, or <see langword="null"/> if compilation failed.</param>
/// <param name="Template">The compiled ARM template JSON string referenced by the parameters file, or <see langword="null"/> if not resolvable.</param>
/// <param name="TemplateSpecId">The Azure resource ID of the template spec, if the parameters file references one; otherwise <see langword="null"/>.</param>
public record CompileParamsResponse(
    bool Success,
    ImmutableArray<DiagnosticDefinition> Diagnostics,
    string? Parameters,
    string? Template,
    string? TemplateSpecId);

/// <summary>
/// Represents a single diagnostic message produced during Bicep compilation or analysis.
/// </summary>
/// <param name="Source">The source of the diagnostic (e.g., <c>"bicep"</c> for compiler diagnostics or a linter rule name).</param>
/// <param name="Range">The source location range where the diagnostic applies.</param>
/// <param name="Level">The severity level of the diagnostic: <c>"Error"</c>, <c>"Warning"</c>, or <c>"Info"</c>.</param>
/// <param name="Code">The diagnostic code (e.g., <c>"BCP001"</c>) that uniquely identifies the diagnostic type.</param>
/// <param name="Message">The human-readable diagnostic message.</param>
public record DiagnosticDefinition(
    string Source,
    Range Range,
    string Level,
    string Code,
    string Message);

/// <summary>
/// Request to retrieve all file references (modules, extensions, and other dependencies) used by a Bicep file.
/// </summary>
/// <param name="Path">The file path to the Bicep file to analyze.</param>
public record GetFileReferencesRequest(
    string Path);

/// <summary>
/// Response containing all file references used by a Bicep file.
/// </summary>
/// <param name="FilePaths">The collection of absolute file paths referenced by the Bicep file, including modules, loaded files, and the file itself.</param>
public record GetFileReferencesResponse(
    ImmutableArray<string> FilePaths);

/// <summary>
/// Request to retrieve metadata about a Bicep file, including its parameters, outputs, and exports.
/// </summary>
/// <param name="Path">The file path to the Bicep file to analyze.</param>
public record GetMetadataRequest(
    string Path);

/// <summary>
/// Request to generate a snapshot of a Bicep parameters file with deployment metadata.
/// The snapshot captures the resolved state of the parameters file along with contextual deployment information.
/// Requires Bicep CLI version 0.36.1 or later.
/// </summary>
/// <param name="Path">The file path to the <c>.bicepparam</c> file.</param>
/// <param name="Metadata">Deployment metadata providing Azure context such as subscription, resource group, and location.</param>
/// <param name="ExternalInputs">Optional collection of external input values to inject into the snapshot. Pass <see langword="null"/> if no external inputs are needed.</param>
public record GetSnapshotRequest(
    string Path,
    GetSnapshotRequest.MetadataDefinition Metadata,
    ImmutableArray<GetSnapshotRequest.ExternalInputValue>? ExternalInputs)
{
    /// <summary>
    /// Azure deployment metadata providing context for snapshot generation.
    /// All fields are optional and may be <see langword="null"/> if not applicable.
    /// </summary>
    /// <param name="TenantId">The Azure Active Directory tenant ID (e.g., <c>"00000000-0000-0000-0000-000000000001"</c>).</param>
    /// <param name="SubscriptionId">The Azure subscription ID (e.g., <c>"00000000-0000-0000-0000-000000000002"</c>).</param>
    /// <param name="ResourceGroup">The target resource group name (e.g., <c>"my-rg"</c>).</param>
    /// <param name="Location">The Azure region for the deployment (e.g., <c>"eastus"</c>).</param>
    /// <param name="DeploymentName">The name of the deployment (e.g., <c>"my-deployment"</c>).</param>
    public record MetadataDefinition(
        string? TenantId,
        string? SubscriptionId,
        string? ResourceGroup,
        string? Location,
        string? DeploymentName);

    /// <summary>
    /// Represents an external input value to be injected into a snapshot.
    /// </summary>
    /// <param name="Kind">The kind of external input (e.g., the input provider type).</param>
    /// <param name="Config">Optional JSON configuration for the external input, or <see langword="null"/> if no configuration is needed.</param>
    /// <param name="Value">The JSON value for the external input.</param>
    public record ExternalInputValue(
        string Kind,
        JsonNode? Config,
        JsonNode Value);
}

/// <summary>
/// Response containing the generated snapshot of a Bicep parameters file.
/// </summary>
/// <param name="Snapshot">The snapshot content as a JSON string representing the resolved deployment state.</param>
public record GetSnapshotResponse(
    string Snapshot);

/// <summary>
/// Response containing metadata about a Bicep file's structure and exported symbols.
/// </summary>
/// <param name="Metadata">The collection of file-level metadata entries (e.g., metadata declared with <c>metadata</c> keyword).</param>
/// <param name="Parameters">The collection of parameter definitions declared in the Bicep file.</param>
/// <param name="Outputs">The collection of output definitions declared in the Bicep file.</param>
/// <param name="Exports">The collection of exported symbols (types, variables, functions) declared with the <c>@export()</c> decorator.</param>
public record GetMetadataResponse(
    ImmutableArray<GetMetadataResponse.MetadataDefinition> Metadata,
    ImmutableArray<GetMetadataResponse.SymbolDefinition> Parameters,
    ImmutableArray<GetMetadataResponse.SymbolDefinition> Outputs,
    ImmutableArray<GetMetadataResponse.ExportDefinition> Exports)
{
    /// <summary>
    /// Represents a parameter or output symbol in a Bicep file.
    /// </summary>
    /// <param name="Range">The source location range of the symbol declaration.</param>
    /// <param name="Name">The name of the parameter or output.</param>
    /// <param name="Type">The type definition of the symbol, or <see langword="null"/> if the type could not be resolved.</param>
    /// <param name="Description">The description from the <c>@description()</c> decorator, or <see langword="null"/> if not specified.</param>
    public record SymbolDefinition(
        Range Range,
        string Name,
        TypeDefinition? Type,
        string? Description);

    /// <summary>
    /// Represents an exported symbol declared with the <c>@export()</c> decorator.
    /// </summary>
    /// <param name="Range">The source location range of the export declaration.</param>
    /// <param name="Name">The name of the exported symbol.</param>
    /// <param name="Kind">The kind of export (e.g., <c>"Type"</c>, <c>"Variable"</c>, <c>"Function"</c>).</param>
    /// <param name="Description">The description from the <c>@description()</c> decorator, or <see langword="null"/> if not specified.</param>
    public record ExportDefinition(
        Range Range,
        string Name,
        string Kind,
        string? Description);

    /// <summary>
    /// Represents a type reference for a parameter or output.
    /// </summary>
    /// <param name="Range">The source location range of the type reference, or <see langword="null"/> for built-in types.</param>
    /// <param name="Name">The name of the type (e.g., <c>"string"</c>, <c>"int"</c>, <c>"object"</c>, or a user-defined type name).</param>
    public record TypeDefinition(
        Range? Range,
        string Name);

    /// <summary>
    /// Represents a file-level metadata entry declared with the <c>metadata</c> keyword in a Bicep file.
    /// </summary>
    /// <param name="Name">The metadata key name (e.g., <c>"description"</c>, <c>"owner"</c>).</param>
    /// <param name="Value">The metadata value as a string.</param>
    public record MetadataDefinition(
        string Name,
        string Value);
}

/// <summary>
/// Request to retrieve the deployment graph for a Bicep file, representing resource dependencies.
/// </summary>
/// <param name="Path">The file path to the Bicep file to analyze.</param>
public record GetDeploymentGraphRequest(
    string Path);

/// <summary>
/// Response containing the deployment graph for a Bicep file.
/// </summary>
/// <param name="Nodes">The collection of resource nodes in the deployment graph.</param>
/// <param name="Edges">The collection of dependency edges between resource nodes.</param>
public record GetDeploymentGraphResponse(
    ImmutableArray<GetDeploymentGraphResponse.Node> Nodes,
    ImmutableArray<GetDeploymentGraphResponse.Edge> Edges)
{
    /// <summary>
    /// Represents a resource node in the deployment graph.
    /// </summary>
    /// <param name="Range">The source location range of the resource declaration.</param>
    /// <param name="Name">The symbolic name of the resource in the Bicep file.</param>
    /// <param name="Type">The fully qualified Azure resource type (e.g., <c>"Microsoft.Storage/storageAccounts"</c>).</param>
    /// <param name="IsExisting">Whether this is a reference to an existing resource (declared with the <c>existing</c> keyword) rather than a new deployment.</param>
    /// <param name="RelativePath">The relative file path if this resource is defined in a module; otherwise <see langword="null"/>.</param>
    public record Node(
        Range Range,
        string Name,
        string Type,
        bool IsExisting,
        string? RelativePath);

    /// <summary>
    /// Represents a directed dependency edge between two resource nodes in the deployment graph.
    /// </summary>
    /// <param name="Source">The symbolic name of the dependent resource (the resource that depends on the target).</param>
    /// <param name="Target">The symbolic name of the dependency (the resource being depended upon).</param>
    public record Edge(
        string Source,
        string Target);
}

/// <summary>
/// Request to format a Bicep file according to Bicep formatting standards.
/// Requires Bicep CLI version 0.37.1 or later.
/// </summary>
/// <param name="Path">The file path to the Bicep file to format.</param>
public record FormatRequest(
    string Path);

/// <summary>
/// Response containing the formatted Bicep file content.
/// </summary>
/// <param name="Contents">The formatted Bicep source code as a string.</param>
public record FormatResponse(
    string Contents);
