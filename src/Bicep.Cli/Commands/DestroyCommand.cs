// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.AzureApi;
using Bicep.Core.Configuration;
using Bicep.Core.Emit;
using Bicep.Core.Utils;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands;

public class DestroyCommand(
    IOContext io,
    ILogger logger,
    IEnvironment environment,
    IArmClientProvider armClientProvider,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : ICommand
{
    public async Task<int> RunAsync(DestroyArguments args, CancellationToken cancellationToken)
    {
        var paramsFileUri = inputOutputArgumentsResolver.PathToUri(args.ParamsFile);
        ArgumentHelper.ValidateBicepParamFile(paramsFileUri);

        var compilation = await compiler.CreateCompilation(paramsFileUri, skipRestore: args.NoRestore);
        CommandHelper.LogExperimentalWarning(logger, compilation);

        if (!compilation.GetEntrypointSemanticModel().Features.DeployCommandEnabled)
        {
            throw new CommandLineException($"The '{nameof(ExperimentalFeaturesEnabled.DeployCommand)}' experimental feature must be enabled to use this command.");
        }

        var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, compilation);
        var parameters = compilation.Emitter.Parameters();

        if (summary.HasErrors)
        {
            return 1;
        }

        var model = compilation.GetEntrypointSemanticModel();

        var armClient = armClientProvider.CreateArmClient(model.Configuration, null);
        await Destroy(armClient, args, parameters, cancellationToken);

        return 0;
    }

    private async Task Destroy(
        ArmClient armClient,
        DestroyArguments args,
        ParametersResult result,
        CancellationToken cancellationToken = default)
    {
        if (result.Template?.Template is not { } template ||
            result.Parameters is not { } parameters)
        {
            throw new Exception($"Failed to compile Bicep parameters");
        }

        (parameters, var config) = await DeploymentProcessor.ProcessParameters(environment, args, parameters);
        var deploymentName = config.Name ?? "main";

        if (config.StacksConfig is not { } stacksConfig)
        {
            throw new CommandLineException("Destroy is only supported for stack deployments.");
        }

        var stackResource = armClient.GetDeploymentStackResource(new ResourceIdentifier($"{config.Scope}/providers/Microsoft.Resources/deploymentStacks/{deploymentName}"));

        var response = await stackResource.DeleteAsync(Azure.WaitUntil.Completed,
            unmanageActionResources: stacksConfig.ActionOnUnmanage?.Resources switch
            {
                // TODO simplify
                { } val when val.Equals(DeploymentStacksDeleteDetachEnum.Delete) => UnmanageActionResourceMode.Delete,
                { } val when val.Equals(DeploymentStacksDeleteDetachEnum.Detach) => UnmanageActionResourceMode.Detach,
                _ => (UnmanageActionResourceMode?)null,
            },
            unmanageActionResourceGroups: stacksConfig.ActionOnUnmanage?.ResourceGroups switch
            {
                // TODO simplify
                { } val when val.Equals(DeploymentStacksDeleteDetachEnum.Delete) => UnmanageActionResourceGroupMode.Delete,
                { } val when val.Equals(DeploymentStacksDeleteDetachEnum.Detach) => UnmanageActionResourceGroupMode.Detach,
                _ => (UnmanageActionResourceGroupMode?)null,
            },
            unmanageActionManagementGroups: stacksConfig.ActionOnUnmanage?.ManagementGroups switch
            {
                // TODO simplify
                { } val when val.Equals(DeploymentStacksDeleteDetachEnum.Delete) => UnmanageActionManagementGroupMode.Delete,
                { } val when val.Equals(DeploymentStacksDeleteDetachEnum.Detach) => UnmanageActionManagementGroupMode.Detach,
                _ => (UnmanageActionManagementGroupMode?)null,
            },
            // TODO figure out what to do with this
            // bypassStackOutOfSyncError: true,
            cancellationToken: cancellationToken);

        await io.Output.WriteAsync($"Stack {deploymentName} deleted successfully.");
    }
}