// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.ComponentModel;
using System.Text.Json;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Utils.Snapshots;
using Bicep.IO.Abstraction;
using ModelContextProtocol.Server;

namespace Bicep.McpServer;

[McpServerToolType]
public sealed class BicepDeploymentTools(
    BicepCompiler compiler)
{
    public record SnapshotResult(
        [Description("The predicted resources in the deployment snapshot")]
        ImmutableArray<JsonElement> PredictedResources,
        [Description("The diagnostics produced during snapshot generation")]
        ImmutableArray<string> Diagnostics);

    [McpServerTool(Title = "Get deployment snapshot", Destructive = false, Idempotent = true, OpenWorld = true, ReadOnly = true, UseStructuredContent = true)]
    [Description("""
    Creates a deployment snapshot from a Bicep parameters file (.bicepparam) by compiling it and pre-expanding the resulting ARM template.
    The snapshot contains the predicted resources (as they would appear in a deployment) and any diagnostics produced during preflight.

    Use this tool to:
    - Preview what resources a deployment would create without running a deployment
    - Validate that parameters resolve as expected
    - Troubleshoot why a deployment would produce unexpected resource IDs, names, or locations
    - Inspect preflight diagnostics produced by template expansion

    This tool can also be used to perform a semantic diff between two Bicep implementations:
    - Generate a snapshot for each version (using the same parameter values and metadata)
    - Compare the resulting predicted resources and diagnostics to verify both produce the same deployment outcome

    This is especially useful for automated refactoring, where text-level diffs are noisy but the intended deployment result should remain unchanged.

    The file path must be absolute and must point to a .bicepparam file.
    The optional tenant/subscription/resource group/location/deployment name values are used as deployment metadata during snapshot generation.
    If a value is omitted, the snapshot may contain unresolved placeholder expressions for the corresponding metadata.
    """)]
    public async Task<SnapshotResult> GetDeploymentSnapshot(
        [Description("The absolute path to the .bicepparam file")] string filePath,
        [Description("Optional Azure tenant ID to use as deployment metadata")] string? tenantId = null,
        [Description("Optional Azure subscription ID to use as deployment metadata")] string? subscriptionId = null,
        [Description("Optional Azure resource group name to use as deployment metadata")] string? resourceGroup = null,
        [Description("Optional Azure location to use as deployment metadata")] string? location = null,
        [Description("Optional deployment name to use as deployment metadata")] string? deploymentName = null,
        CancellationToken cancellationToken = default)
    {
        var fileUri = IOUri.FromFilePath(filePath);
        if (!fileUri.HasBicepParamExtension())
        {
            throw new ArgumentException("The specified file must have a .bicepparam extension.", nameof(filePath));
        }

        var compilation = await compiler.CreateCompilation(fileUri);

        if (compilation.Emitter.Parameters() is not { } result ||
            result.Template?.Template is not { } templateContent ||
            result.Parameters is not { } parametersContent)
        {
            throw new InvalidOperationException("The parameters file could not be compiled.");
        }

        var snapshot = await SnapshotHelper.GetSnapshot(
            targetScope: compilation.GetEntrypointSemanticModel().TargetScope,
            templateContent: templateContent,
            parametersContent: parametersContent,
            tenantId: tenantId,
            subscriptionId: subscriptionId,
            resourceGroup: resourceGroup,
            location: location,
            deploymentName: deploymentName,
            cancellationToken: cancellationToken,
            externalInputs: []);

        return new(
             PredictedResources: snapshot.PredictedResources,
             Diagnostics: snapshot.Diagnostics);
    }
}
