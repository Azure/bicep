// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.CommandLine;
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
using Bicep.Cli.Constants;
using Bicep.Cli.Helpers;
using Bicep.Cli.Helpers.Snapshots;
using Bicep.Cli.Helpers.WhatIf;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Json;
using Bicep.Core.TypeSystem;
using Bicep.Core.Utils.Snapshots;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.DependencyInjection;
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
        var snapshotMode = args.Mode ?? SnapshotArguments.SnapshotMode.Overwrite;

        var (inputUri, outputUri) = inputOutputArgumentsResolver.ResolveInputOutputArguments(args);
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

                    await io.Output.Writer.WriteAsync(WhatIfOperationResultFormatter.Format(changes));
                    return changes.Any() ? 1 : 0;
                }
            default:
                throw new UnreachableException();
        }
    }

    private async Task<Snapshot?> GetSnapshot(SnapshotArguments arguments, IOUri inputUri, bool noRestore, CancellationToken cancellationToken)
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

        try
        {
            return await SnapshotHelper.GetSnapshot(
                targetScope: compilation.GetEntrypointSemanticModel().TargetScope,
                templateContent: templateContent,
                parametersContent: parametersContent,
                tenantId: arguments.TenantId,
                managementGroupId: arguments.ManagementGroupId,
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
            throw new CommandLineException($"Error opening file {uri.GetFilePath()}: {message}.");
        }

        return SnapshotHelper.Deserialize(contents);
    }

    private void WriteSnapshot(IOUri uri, Snapshot newSnapshot)
    {
        var contents = SnapshotHelper.Serialize(newSnapshot);

        var file = fileExplorer.GetFile(uri);
        file.Write(contents);
    }

    internal static Command CreateCommand(CommandLineBuilderContext context)
    {
        var inputFileArgument = CommonArguments.ParametersFile();
        var outDirOption = CommonOptions.OutDir("The directory containing the snapshot file to generate or validate.");
        var outFileOption = CommonOptions.OutFile("The snapshot file path to use for generation or validation.");
        var modeOption = new Option<SnapshotArguments.SnapshotMode?>(OptionNames.Mode)
        {
            Description = "Sets the snapshot mode. Valid values are (overwrite, validate).",
        };
        var tenantIdOption = new Option<string?>(OptionNames.TenantId)
        {
            Description = "The tenant ID to use for the deployment.",
        };
        var subscriptionIdOption = new Option<string?>(OptionNames.SubscriptionId)
        {
            Description = "The subscription ID to use for the deployment.",
        };
        var locationOption = new Option<string?>(OptionNames.Location)
        {
            Description = "The location to use for the deployment.",
        };
        var resourceGroupOption = new Option<string?>(OptionNames.ResourceGroup)
        {
            Description = "The resource group name to use for the deployment.",
        };
        var managementGroupIdOption = new Option<string?>(OptionNames.ManagementGroupId)
        {
            Description = "The management group ID to use for the deployment.",
        };
        var deploymentNameOption = new Option<string?>(OptionNames.DeploymentName)
        {
            Description = "The deployment name to use.",
        };
        var configOption = CommonOptions.Config();

        var command = new Command(CommandNames.Snapshot, "Generates or validates a deployment snapshot from a .bicepparam file.")
        {
            inputFileArgument,
            outDirOption,
            outFileOption,
            modeOption,
            tenantIdOption,
            subscriptionIdOption,
            managementGroupIdOption,
            locationOption,
            resourceGroupOption,
            deploymentNameOption,
            configOption,
        };
        command.Validators.Add(result => CommandLineBuilderContext.ValidateRequiredPositionalArgument(result, inputFileArgument));

        command.SetAction((result, ct) => context.RunCommandAsync(result.GetValue(configOption), async () =>
        {
            var outputDir = result.GetValue(outDirOption);
            var outputFile = result.GetValue(outFileOption);
            ArgumentHelper.ValidateOutputOptions(outputToStdOut: false, outputDir, outputFile);

            var args = new SnapshotArguments(
                result.GetRequiredValue(inputFileArgument),
                result.GetValue(modeOption),
                result.GetValue(tenantIdOption),
                result.GetValue(subscriptionIdOption),
                result.GetValue(managementGroupIdOption),
                result.GetValue(locationOption),
                result.GetValue(resourceGroupOption),
                result.GetValue(deploymentNameOption),
                outputDir,
                outputFile);

            return await context.GetCommand<SnapshotCommand>().RunAsync(args, ct);
        }));

        return command;
    }
}
