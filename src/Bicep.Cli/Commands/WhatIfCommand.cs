// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.Text;
using System.Text.Json;
using Azure.Deployments.Core.Definitions;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.Models;
using Bicep.Core.Registry;
using Bicep.Deploy;
using Bicep.Deploy.Exceptions;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands;

public class WhatIfCommand : ICommand
{
    private readonly DiagnosticLogger diagnosticLogger;
    private readonly BicepCompiler compiler;
    private readonly IDeploymentManagerFactory deploymentManagerFactory;
    private readonly IConfigurationManager configurationManager;
    private readonly IOContext io;
    private readonly ILogger logger;

    public WhatIfCommand(
        DiagnosticLogger diagnosticLogger,
        BicepCompiler compiler,
        IOContext io,
        ILogger logger,
        IDeploymentManagerFactory deploymentManagerFactory,
        IConfigurationManager configurationManager)
    {
        this.diagnosticLogger = diagnosticLogger;
        this.compiler = compiler;
        this.deploymentManagerFactory = deploymentManagerFactory;
        this.configurationManager = configurationManager;
        this.io = io;
        this.logger = logger;
    }

    public async Task<int> RunAsync(WhatIfArguments args, CancellationToken cancellationToken)
    {
        var deploymentFile = ArgumentHelper.GetFileUri(args.InputFile);
        ArgumentHelper.ValidateBicepDeployFile(deploymentFile);
        var compilation = await compiler.CreateCompilation(
            deploymentFile,
            skipRestore: args.NoRestore);

        var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, compilation);

        // TODO: Use bicepdeploy compilation emission here
        var result = compilation.Emitter.Template();

        if (result.Template is not { } template)
        {
            return 1;
        }

        if (!compilation.GetEntrypointSemanticModel().Features.DeploymentFileEnabled)
        {
            logger.LogError("Experimental feature 'deployment file' is not enabled.");
            return 1;
        }

        // TODO: Use deploy file emission to build deployment definition
        var deploymentDefinition = new ArmDeploymentDefinition(
            null,
            "92722693-40f1-44fe-8c39-9cf6b6353750",
            "levi-bicep-deploy",
            args.Name ?? "main",
            new ArmDeploymentWhatIfProperties(ArmDeploymentMode.Incremental)
            {
                Template = new BinaryData(JsonDocument.Parse(template).RootElement),
                Parameters = new BinaryData("{}"),
                WhatIfResultFormat = args.ResultFormat
            });

        var rootConfiguration = configurationManager.GetConfiguration(deploymentFile);
        var deploymentManager = deploymentManagerFactory.CreateDeploymentManager(rootConfiguration);

        try
        {
            var whatIfResult = await deploymentManager.WhatIfAsync(deploymentDefinition, cancellationToken);

            await WriteWhatIfSummary(whatIfResult);
        }
        catch (WhatIfException ex)
        {
            await io.Error.WriteLineAsync($"Unable to run what-if: {ex.Message}");
            return 1;
        }
        
        return 0;
    }

    private async Task WriteWhatIfSummary(WhatIfOperationResult whatIfResult)
    {
        if (whatIfResult.Error is { } error)
        {
            await io.Error.WriteLineAsync($"What-if failed: {error.Code} - {error.Message}");
        }

        // TODO: there's probably a better way to print these out
        foreach (var change in whatIfResult.Changes)
        {
            switch (change.ChangeType)
            {
                case WhatIfChangeType.Create:
                    await io.Output.WriteLineAsync($"Create: + {change.ResourceId}");
                    break;
                case WhatIfChangeType.Delete:
                    await io.Output.WriteLineAsync($"Delete: - {change.ResourceId}");
                    break;
                case WhatIfChangeType.Modify:
                    await io.Output.WriteLineAsync($"Modify: ~ {change.ResourceId}");
                    foreach (var prop in change.Delta)
                    {
                        await io.Output.WriteLineAsync($"\t- {prop.Before}{Environment.NewLine}\t+ {prop.After}");
                    }
                    break;
                case WhatIfChangeType.Ignore:
                    await io.Output.WriteLineAsync($"Ignore: {change.ResourceId}");
                    break;
                case WhatIfChangeType.NoChange:
                    await io.Output.WriteLineAsync($"No change: {change.ResourceId}");
                    break;
            }
        }
    }
}
