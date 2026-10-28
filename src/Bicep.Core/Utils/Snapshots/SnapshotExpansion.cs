// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Definitions.Extensibility;
using Azure.Deployments.Templates.ParsedEntities;

namespace Bicep.Core.Utils.Snapshots;

/// <summary>
/// The engine objects from one nested-deployment expansion.
/// </summary>
public sealed record SnapshotExpansion(
    ImmutableArray<DeploymentDiagnosticDefinition> Diagnostics,
    ImmutableArray<DeploymentResourceWithParsedExpressions> PreflightResources,
    ImmutableArray<ExtensibleResource> ExtensibleResources,
    ImmutableDictionary<string, TemplateOutputParameterWithParsedExpressions>? Outputs);
