// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Text.Encodings.Web;
using System.Text.Json;
using Azure.Deployments.Core.Constants;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Core.Helpers;
using Azure.Deployments.Core.Interfaces;
using Azure.Deployments.Engine.Definitions;
using Azure.Deployments.Engine.DeploymentExpanderV2;
using Azure.Deployments.Templates.Engines;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Helpers.WhatIf;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Json;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.ResourceStack.Common.EventSources;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Bicep.Cli.Commands;

public class SnapshotCommand(
    IOContext io,
    ILogger logger,
    IFileExplorer fileExplorer,
    BicepCompiler compiler,
    DiagnosticLogger diagnosticLogger) : ICommand
{
    public record Snapshot(
        ImmutableArray<JsonElement> PredictedResources,
        ImmutableArray<string> Diagnostics);

    public async Task<int> RunAsync(SnapshotArguments args, CancellationToken cancellationToken)
    {
        logger.LogWarning($"WARNING: The '{args.CommandName}' CLI command group is an experimental feature. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.");

        var snapshotMode = args.Mode ?? SnapshotArguments.SnapshotMode.Overwrite;

        var inputUri = ArgumentHelper.GetFileUri(args.InputFile);
        ArgumentHelper.ValidateBicepParamFile(inputUri);

        var deploymentContext = PopulateDeploymentContext(
            tenantId: args.TenantId ?? "<tenant-id>",
            subscriptionId: args.SubscriptionId ?? "<subscription-id>",
            rgName: args.ResourceGroup ?? "<resource-group>",
            location: args.Location ?? "<location>",
            deploymentName: "main");

        var newSnapshot = await GetSnapshot(deploymentContext, inputUri, noRestore: false, cancellationToken)
            ?? throw new CommandLineException($"Failed to generate snapshot for file {inputUri.LocalPath}");

        if (newSnapshot.Diagnostics.Length > 0)
        {
            logger.LogWarning("Diagnostics:");
            foreach (var diagnostic in newSnapshot.Diagnostics)
            {
                logger.LogWarning($"  {diagnostic}");
            }
        }

        var outputUri = PathHelper.ChangeExtension(inputUri, ".snapshot.json").ToIOUri();
        switch (snapshotMode)
        {
            case SnapshotArguments.SnapshotMode.Overwrite:
                WriteSnapshot(outputUri, newSnapshot);
                return 0;
            case SnapshotArguments.SnapshotMode.Validate:
                {
                    var oldSnapshot = ReadSnapshot(outputUri);

                    var changes = SnapshotDiffer.CalculateChanges(oldSnapshot, newSnapshot);
                    await io.Output.WriteAsync(WhatIfOperationResultFormatter.Format(changes));
                    return changes.Any() ? 1 : 0;
                }
            default:
                throw new UnreachableException();
        }
    }

    private async Task<Snapshot?> GetSnapshot(DeploymentRequestContextWithScopeDefinition deploymentContext, Uri inputUri, bool noRestore, CancellationToken cancellationToken)
    {
        var compilation = await compiler.CreateCompilation(inputUri, skipRestore: noRestore);
        CommandHelper.LogExperimentalWarning(logger, compilation);

        var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, compilation);

        if (summary.HasErrors ||
            compilation.Emitter.Parameters() is not { } result ||
            result.Template?.Template is not { } templateContent ||
            result.Parameters is not { } parametersContent)
        {
            return null;
        }

        var expander = new NestedDeploymentExpander(
            eventSource: new EmptyEventSource(),
            contentLinkResolver: new EmptyContentLinkResolver(),
            deploymentMetadataProvider: new SynthesizedDeploymentMetadataProvider(),
            apiProfileResolver: new EmptyApiProfileResolver(),
            preflightSettings: new(
                nestedDeploymentExpansionLimit: 500,
                resourceGroupLimit: 800,
                nestedDeploymentExpansionTimeout: TimeSpan.MaxValue,
                referenceFunctionPreflightEnabled: true,
                optOutDoubleEvaluationFix: false));

        var deploymentId = deploymentContext.GetDeploymentFullyQualifiedId();

        var parameters = parametersContent.FromJson<DeploymentParametersDefinition>();
        var template = TemplateEngine.ParseTemplate(templateContent);

        DeploymentContent deploymentContent = new()
        {
            Properties = new()
            {
                Mode = DeploymentMode.Incremental,
                Parameters = parameters.Parameters,
                Template = template,
                TemplateHash = TemplateHelpers.ComputeTemplateHash(JToken.Parse(templateContent)),
            },
        };

        var expansionResult = await expander.ExpandNestedDeployments(
            deploymentContext.TenantId,
            deploymentContext.DeploymentName,
            validateOnly: true,
            deploymentId,
            CoreConstants.ApiVersion20240701,
            deploymentContext,
            deploymentContent,
            metricsRecorder: null,
            cancellationToken: cancellationToken);

        return new(
            [
                ..expansionResult.PreflightResources.Select(x => JsonElementFactory.CreateElement(JsonExtensions.ToJson(x))),
                ..expansionResult.ExtensibleResources.Select(x => JsonElementFactory.CreateElement(JsonExtensions.ToJson(x))),
                ..expansionResult.ResourcesWithUnevaluableIdsOrApiVersions.Select(x => JsonElementFactory.CreateElement(JsonExtensions.ToJson(x))),
            ],
            [
                ..expansionResult.Diagnostics.Select(d => $"{d.Target} {d.Level} {d.Code}: {d.Message}")
            ]);
    }

    public class EmptyContentLinkResolver : IContentLinkResolver
    {
        public Task<(Template template, string templateHash)> DownloadAndHashTemplateForLink(DeploymentTemplateContentLink templateLink, string deploymentId, CancellationToken cancellationToken)
            => throw new InvalidOperationException();

        public Task<DeploymentParametersDefinition> DownloadAndValidateParameters(DeploymentContentLink parametersLink, CancellationToken cancellation)
            => throw new InvalidOperationException();
    }

    private class EmptyApiProfileResolver : IApiProfileResolver
    {
        public Task<string?> GetApiVersionForRoutingResourceType(string? subscriptionId, string resourceId, string apiProfileId)
            => throw new InvalidOperationException();
    }

    private class EmptyEventSource : IGeneralEventSource
    {
        public void Critical(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string message, string exception, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        {
        }

        public void Debug(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string message, string exception, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        {
        }

        public void Error(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string message, string exception, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        {
        }

        public void Warning(string subscriptionId, string correlationId, string principalOid, string principalPuid, string tenantId, string operationName, string message, string exception, string organizationId, string activityVector, string realPuid, string altSecId, string additionalProperties)
        {
        }
    }

    public static DeploymentRequestContextWithScopeDefinition PopulateDeploymentContext(
        string tenantId,
        string? subscriptionId,
        string? rgName,
        string? location,
        string deploymentName)
    {
        var tenantDefinition = new JObject(new JProperty("tenantId", tenantId));

        if (subscriptionId is not null)
        {
            var subscriptionDefinition = new JObject
            {
                { "subscriptionId", subscriptionId },
                { "id", $"/subscriptions/{subscriptionId}" },
                { "tenantId", tenantId },
                { "displayName", "Azure deployment scenario test" },
            };

            if (rgName is not null)
            {
                var resourceGroupDefinition = new JObject
                {
                    { "subscriptionId", subscriptionId },
                    { "id", $"/subscriptions/{subscriptionId}/resourceGroups/{rgName}" },
                    { "name", rgName },
                    { "location", location },
                    { "properties", new JObject { { "provisioningState", ProvisioningState.Ready.ToString() }, } },
                };

                return DeploymentRequestContextWithScopeDefinition.CreateAtResourceGroup(
                    tenantId: tenantId,
                    subscriptionId: subscriptionId,
                    resourceGroupName: rgName,
                    resourceGroupLocation: location,
                    deploymentName: deploymentName,
                    subscriptionDefinition: subscriptionDefinition,
                    resourceGroupDefinition: resourceGroupDefinition,
                    tenantDefinition: tenantDefinition);
            }

            return DeploymentRequestContextWithScopeDefinition.CreateAtSubscription(
                tenantId: tenantId,
                subscriptionId: subscriptionId,
                deploymentName: deploymentName,
                subscriptionDefinition: subscriptionDefinition,
                tenantDefinition: tenantDefinition);
        }

        return DeploymentRequestContextWithScopeDefinition.CreateAtTenant(tenantId, deploymentName, tenantDefinition);
    }

    private Snapshot ReadSnapshot(IOUri uri)
    {
        var file = fileExplorer.GetFile(uri);
        if (!file.TryReadAllText().IsSuccess(out var contents, out var failureBuilder))
        {
            var message = failureBuilder(DiagnosticBuilder.ForDocumentStart()).Message;
            throw new CommandLineException($"Error opening file {uri.GetLocalFilePath()}: {message}.");
        }

#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        return JsonSerializer.Deserialize<Snapshot>(contents, serializerOptions) ?? throw new InvalidOperationException($"Failed to read from {uri.GetLocalFilePath()}");
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
    }

    private void WriteSnapshot(IOUri uri, Snapshot newSnapshot)
    {
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        var contents = JsonSerializer.Serialize(newSnapshot, serializerOptions) ?? throw new InvalidOperationException($"Failed to write to {uri.GetLocalFilePath()}");
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

        var file = fileExplorer.GetFile(uri);
        file.Write(contents);
    }

    private static readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web)
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };
}