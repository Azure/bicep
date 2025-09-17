// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Entities;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Cli.Arguments;
using Bicep.Cli.Commands.Helpers.Deploy;
using Bicep.Cli.Helpers.WhatIf;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.AzureApi;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
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
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : DeploymentsCommandsBase<WhatIfArguments>(logger, diagnosticLogger, compiler, inputOutputArgumentsResolver)
{
    protected override async Task<int> RunInternal(WhatIfArguments args, SemanticModel model, ParametersResult parameters, CancellationToken cancellationToken)
    {
        var armClient = armClientProvider.CreateArmClient(model.Configuration, null);
        await WhatIf(armClient, args, parameters, cancellationToken);

        return 0;
    }

    private async Task WhatIf(ArmClient armClient, WhatIfArguments args, ParametersResult result, CancellationToken cancellationToken)
    {
        var (template, parameters, config) = await DeploymentProcessor.GetDeployCommandsConfig(environment, args.AdditionalArguments, result);
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