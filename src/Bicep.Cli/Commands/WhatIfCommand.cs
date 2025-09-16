// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Entities;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Helpers.WhatIf;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.AzureApi;
using Bicep.Core.Configuration;
using Bicep.Core.Emit;
using Bicep.Core.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;

namespace Bicep.Cli.Commands;

public class WhatIfCommand(
    IOContext io,
    ILogger logger,
    IEnvironment environment,
    IArmClientProvider armClientProvider,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : ICommand
{
    public async Task<int> RunAsync(WhatIfArguments args, CancellationToken cancellationToken)
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
        await WhatIf(armClient, args, parameters, cancellationToken);

        return 0;
    }

    private async Task WhatIf(
        ArmClient armClient,
        WhatIfArguments args,
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

        if (config.StacksConfig is { })
        {
            throw new CommandLineException("What-If analysis is not currently supported for stack deployments.");
        }

        var deploymentResource = armClient.GetArmDeploymentResource(new ResourceIdentifier($"{config.Scope}/providers/Microsoft.Resources/deployments/{deploymentName}"));
        var paramsDefinition = parameters.FromJson<DeploymentParametersDefinition>();
        var deploymentProperties = new ArmDeploymentWhatIfProperties(ArmDeploymentMode.Incremental)
        {
            Template = BinaryData.FromString(template),
            Parameters = BinaryData.FromString(paramsDefinition.Parameters.ToJson()),
        };
        var armDeploymentContent = new ArmDeploymentWhatIfContent(deploymentProperties);

        var response = await deploymentResource.WhatIfAsync(Azure.WaitUntil.Completed, armDeploymentContent, cancellationToken);

        var definition = response.GetRawResponse().Content.ToString().FromJson<DeploymentWhatIfResponseDefinition>();
        var changes = definition.Properties.Changes.Where(x => x.ChangeType != DeploymentWhatIfChangeType.Ignore);

        await io.Output.WriteAsync(WhatIfOperationResultFormatter.Format([..changes]));
    }
}