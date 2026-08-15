// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.Documentation;

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
