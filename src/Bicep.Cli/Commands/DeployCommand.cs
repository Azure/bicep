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

public class DeployCommand : ICommand
{
    private readonly DiagnosticLogger diagnosticLogger;
    private readonly BicepCompiler compiler;
    private readonly IDeploymentManagerFactory deploymentManagerFactory;
    private readonly IConfigurationManager configurationManager;
    private readonly IOContext io;
    private readonly ILogger logger;

    public DeployCommand(
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

    public async Task<int> RunAsync(DeployArguments args, CancellationToken cancellationToken)
    {
        var deploymentFile = ArgumentHelper.GetFileUri(args.InputFile);
        ArgumentHelper.ValidateBicepDeployFile(deploymentFile);
        var compilation = await compiler.CreateCompilation(
            deploymentFile,
            skipRestore: args.NoRestore);

        var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, compilation);

        // TODO: Use bicepdeploy compilation emission here
        var result = compilation.Emitter.Deploy();

        if (!result.Success)
        {
            return 1;
        }

        if (!compilation.GetEntrypointSemanticModel().Features.DeploymentFileEnabled)
        {
            logger.LogError("Experimental feature 'deployment file' is not enabled.");
            return 1;
        }

        // TODO: Use deploy file emission to build deployment definition
        //var deploymentDefinition = new ArmDeploymentDefinition(
        //    null,
        //    "92722693-40f1-44fe-8c39-9cf6b6353750",
        //    "levi-bicep-deploy",
        //    args.Name ?? "main",
        //    new ArmDeploymentProperties(ArmDeploymentMode.Incremental)
        //    {
        //        Template = new BinaryData(JsonDocument.Parse(template).RootElement),
        //        Parameters = new BinaryData("{}")
        //    });
        var deploymentDefinition = result.DeploymentDefinition!;

        var rootConfiguration = configurationManager.GetConfiguration(deploymentFile);
        var deploymentManager = deploymentManagerFactory.CreateDeploymentManager(rootConfiguration);

        try
        {
            var deployment = await deploymentManager.CreateOrUpdateAsync(
                deploymentDefinition,
                WriteDeploymentOperationSummary,
                cancellationToken);

            await WriteDeploymentSummary(deployment.Data);
        }
        catch (DeploymentException ex)
        {
            await io.Error.WriteLineAsync($"Unable to deploy: {ex.Message}");
            return 1;
        }
        
        return 0;
    }

    private void WriteDeploymentOperationSummary(IEnumerable<ArmDeploymentOperation> operations)
    {
        foreach (var operation in operations)
        {
            var resource = operation.Properties.TargetResource;
            if (resource is null)
            {
                continue;
            }

            io.Output.WriteLine($"Resource {resource.ResourceType} '{resource.ResourceName}' provisioning status is {operation.Properties.ProvisioningState}");
        }
    }

    private async Task WriteDeploymentSummary(ArmDeploymentData deployment)
    {
        if (deployment.Properties.Outputs is { } outputs)
        {
            var outputsDict = outputs.ToObjectFromJson<Dictionary<string, object>>();
            foreach (var output in outputsDict)
            {
                await io.Output.WriteLineAsync($"Output: {output.Key} = {output.Value}");
            }
        }

        if (deployment.Properties.Error is { } error)
        {
            await io.Error.WriteLineAsync($"Deployment failed: {error.Code} - {error.Message}");
        }

        foreach (var resource in deployment.Properties.OutputResources)
        {
            await io.Output.WriteLineAsync($"Deployed resource: {resource.Id}");
        }

        await io.Output.WriteLineAsync($"Result: {deployment.Properties.ProvisioningState}");
    }
}
