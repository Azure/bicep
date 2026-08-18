// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.Documentation;

/// <summary>
/// Represents documentation data for one Bicep module.
/// </summary>
public record BicepDocumentationModel(
    string Name,
    string? Description,
    string Path,
    string TargetScope,
    ImmutableSortedDictionary<string, string> Custom,
    ImmutableArray<BicepDocumentationResourceType> ResourceTypes,
    ImmutableArray<BicepDocumentationParameter> Parameters,
    ImmutableArray<BicepDocumentationOutput> Outputs,
    ImmutableArray<BicepDocumentationExport> ExportedTypes,
    ImmutableArray<BicepDocumentationExport> ExportedVariables,
    ImmutableArray<BicepDocumentationFunction> ExportedFunctions,
    ImmutableArray<BicepDocumentationReference> References,
    ImmutableArray<BicepDocumentationUsageExample> UsageExamples);

/// <summary>
/// A resource type declared within a module.
/// </summary>
public record BicepDocumentationResourceType(string Type, bool IsExisting);

/// <summary>
/// Represents a module parameter or nested property.
/// </summary>
public record BicepDocumentationParameter(
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
    ImmutableArray<BicepDocumentationParameter> NestedProperties,
    BicepDocumentationDiscriminator? Discriminator);

/// <summary>
/// Represents a discriminated object type.
/// </summary>
public record BicepDocumentationDiscriminator(
    string PropertyName,
    ImmutableArray<BicepDocumentationDiscriminatorCase> Cases);

/// <summary>
/// Represents one discriminator case.
/// </summary>
public record BicepDocumentationDiscriminatorCase(
    string Value,
    ImmutableArray<BicepDocumentationParameter> Properties);

/// <summary>
/// A module output.
/// </summary>
public record BicepDocumentationOutput(string Name, string TypeName, bool IsSecure, string? Description);

/// <summary>
/// An exported type or variable.
/// </summary>
public record BicepDocumentationExport(
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
    ImmutableArray<BicepDocumentationParameter> NestedProperties,
    BicepDocumentationDiscriminator? Discriminator);

/// <summary>
/// A user-defined function exported from a module.
/// </summary>
public record BicepDocumentationFunction(
    string Name,
    ImmutableArray<BicepDocumentationFunctionParameter> Parameters,
    string ReturnTypeName,
    string? Description);

/// <summary>
/// A parameter of an exported user-defined function.
/// </summary>
public record BicepDocumentationFunctionParameter(string Name, string TypeName, string? Description);

/// <summary>
/// A cross-referenced module in the entrypoint file.
/// </summary>
public record BicepDocumentationReference(string SymbolicName, string? Path, string? Description);

/// <summary>
/// Represents a local usage example.
/// </summary>
public record BicepDocumentationUsageExample(string Name, string RelativePath, string? Description, string Contents);
