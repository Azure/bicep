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

public class ValidateCommand : ICommand
{
    private readonly DiagnosticLogger diagnosticLogger;
    private readonly BicepCompiler compiler;
    private readonly IDeploymentManagerFactory deploymentManagerFactory;
    private readonly IConfigurationManager configurationManager;
    private readonly IOContext io;
    private readonly ILogger logger;

    public ValidateCommand(
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

    public async Task<int> RunAsync(ValidateArguments args, CancellationToken cancellationToken)
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
            new ArmDeploymentProperties(ArmDeploymentMode.Incremental)
            {
                Template = new BinaryData(JsonDocument.Parse(template).RootElement),
                Parameters = new BinaryData("{}")
            });

        var rootConfiguration = configurationManager.GetConfiguration(deploymentFile);
        var deploymentManager = deploymentManagerFactory.CreateDeploymentManager(rootConfiguration);
        
        try
        {
            var validationResult = await deploymentManager.ValidateAsync(deploymentDefinition, cancellationToken);

            await WriteValidationSummary(validationResult);
        }
        catch (ValidationException ex)
        {
            await io.Error.WriteLineAsync($"Unable to validate: {ex.Message}");
            return 1;
        }
        
        return 0;
    }

    private async Task WriteValidationSummary(ArmDeploymentValidateResult validationResult)
    {
        if (validationResult.Properties.Error is { } error)
        {
            await io.Error.WriteLineAsync($"Validation failed: {error.Code} - {error.Message}");
        }

        foreach (var resource in validationResult.Properties.ValidatedResources)
        {
            await io.Output.WriteLineAsync($"Validated resource: {resource.Id}");
        }

        await io.Output.WriteLineAsync($"Result: {validationResult.Properties.ProvisioningState}");
    }
}