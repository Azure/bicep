// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Cli.Arguments;
using Bicep.Cli.Commands.Helpers.Deploy;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.AzureApi;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.Utils;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands;

public class TeardownCommand(
    IOContext io,
    ILogger logger,
    IEnvironment environment,
    IArmClientProvider armClientProvider,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : DeploymentsCommandsBase<TeardownArguments>(logger, diagnosticLogger, compiler, inputOutputArgumentsResolver)
{
    protected override async Task<int> RunInternal(TeardownArguments args, SemanticModel model, ParametersResult parameters, CancellationToken cancellationToken)
    {
        var armClient = armClientProvider.CreateArmClient(model.Configuration, null);
        await Teardown(armClient, args, parameters, cancellationToken);

        return 0;
    }

    private async Task Teardown(ArmClient armClient, TeardownArguments args, ParametersResult result, CancellationToken cancellationToken)
    {
        var (_, _, usingConfig) = await DeploymentProcessor.GetDeployCommandsConfig(environment, args.AdditionalArguments, result);
        var deploymentName = usingConfig.Name ?? "main";

        if (usingConfig.StacksConfig is not { } stacksConfig)
        {
            throw new CommandLineException("Teardown is only supported for stack deployments.");
        }

        var stackResource = armClient.GetDeploymentStackResource(new ResourceIdentifier($"{usingConfig.Scope}/providers/Microsoft.Resources/deploymentStacks/{deploymentName}"));

        await stackResource.DeleteAsync(Azure.WaitUntil.Completed,
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