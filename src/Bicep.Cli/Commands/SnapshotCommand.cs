// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using Azure.Deployments.Core.Configuration;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Expression.Intermediate;
using Azure.Deployments.Expression.Intermediate.Extensions;
using Azure.Deployments.Templates.Engines;
using Azure.Deployments.Templates.Exceptions;
using Azure.Deployments.Templates.ParsedEntities;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Helpers.Snapshot;
using Bicep.Cli.Helpers.WhatIf;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Json;
using Bicep.Core.TypeSystem;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Bicep.Cli.Commands;

public class SnapshotCommand(
    IOContext io,
    ILogger logger,
    IFileExplorer fileExplorer,
    BicepCompiler compiler,
    DiagnosticLogger diagnosticLogger,
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : ICommand
{
    public async Task<int> RunAsync(SnapshotArguments args, CancellationToken cancellationToken)
    {
        logger.LogWarning($"WARNING: The '{args.CommandName}' CLI command group is an experimental feature. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.");

        var snapshotMode = args.Mode ?? SnapshotArguments.SnapshotMode.Overwrite;

        var inputUri = inputOutputArgumentsResolver.ResolveInputArguments(args);
        ArgumentHelper.ValidateBicepParamFile(inputUri);

        var newSnapshot = await GetSnapshot(args, inputUri, noRestore: false, cancellationToken)
            ?? throw new CommandLineException($"Failed to generate snapshot for file {inputUri}");

        if (newSnapshot.Diagnostics.Length > 0)
        {
            logger.LogWarning("Snapshot generation diagnotics:");
            foreach (var diagnostic in newSnapshot.Diagnostics)
            {
                logger.LogWarning($"  {diagnostic}");
            }
        }

        var outputUri = inputUri.WithExtension(".snapshot.json");
        switch (snapshotMode)
        {
            case SnapshotArguments.SnapshotMode.Overwrite:
                WriteSnapshot(outputUri, newSnapshot);
                return 0;
            case SnapshotArguments.SnapshotMode.Validate:
                {
                    var oldSnapshot = ReadSnapshot(outputUri);

                    var changes = SnapshotDiffer.CalculateChanges(oldSnapshot, newSnapshot);
                    var hasFailures = changes.Any();

                    if (hasFailures)
                    {
                        logger.LogWarning("Snapshot validation failed. Expected no changes, but found the following:");
                    }

                    await io.Output.WriteAsync(WhatIfOperationResultFormatter.Format(changes));
                    return changes.Any() ? 1 : 0;
                }
            default:
                throw new UnreachableException();
        }
    }

    private async Task<Snapshot?> GetSnapshot(SnapshotArguments arguments, IOUri inputUri, bool noRestore, CancellationToken cancellationToken)
    {
        var compilation = await compiler.CreateCompilation(inputUri.ToUri(), skipRestore: noRestore);
        CommandHelper.LogExperimentalWarning(logger, compilation);

        var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, compilation);

        if (summary.HasErrors ||
            compilation.Emitter.Parameters() is not { } result ||
            result.Template?.Template is not { } templateContent ||
            result.Parameters is not { } parametersContent)
        {
            return null;
        }

        try
        {
            return await SnapshotHelper.GetSnapshot(
                targetScope: compilation.GetEntrypointSemanticModel().TargetScope,
                templateContent: templateContent,
                parametersContent: parametersContent,
                tenantId: arguments.TenantId,
                subscriptionId: arguments.SubscriptionId,
                resourceGroup: arguments.ResourceGroup,
                location: arguments.Location,
                deploymentName: arguments.DeploymentName,
                cancellationToken: cancellationToken,
                // TODO: Add support for external input values
                externalInputs: []);
        }
        catch (TemplateValidationException e)
        {
            throw new CommandLineException(
                $"Template snapshotting could not be completed for the following reason: '{e.Message}'.",
                e);
        }
    }

    private Snapshot ReadSnapshot(IOUri uri)
    {
        var file = fileExplorer.GetFile(uri);
        if (!file.TryReadAllText().IsSuccess(out var contents, out var failureBuilder))
        {
            var message = failureBuilder(DiagnosticBuilder.ForDocumentStart()).Message;
            throw new CommandLineException($"Error opening file {uri.GetLocalFilePath()}: {message}.");
        }

        return SnapshotHelper.Deserialize(contents);
    }

    private void WriteSnapshot(IOUri uri, Snapshot newSnapshot)
    {
        var contents = SnapshotHelper.Serialize(newSnapshot);

        var file = fileExplorer.GetFile(uri);
        file.Write(contents);
    }
}
