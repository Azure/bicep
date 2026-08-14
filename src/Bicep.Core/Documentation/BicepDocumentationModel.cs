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
    ImmutableArray<BicepDocumentationFunction> ExportedFunctions,
    ImmutableArray<BicepDocumentationReference> References,
    ImmutableArray<BicepDocumentationUsageExample> UsageExamples,
    BicepDocumentationDataCollection? DataCollection);
