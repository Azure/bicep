// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using Azure.Core;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Entities;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Helpers.Snapshot;
using Bicep.Cli.Helpers.WhatIf;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.AzureApi;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.TypeSystem;
using Bicep.Core.Utils;
using Bicep.Local.Deploy.Extensibility;
using Json.More;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

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

        var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, compilation);
        var parameters = compilation.Emitter.Parameters();

        if (summary.HasErrors ||
            compilation.Emitter.Parameters() is not { } result ||
            result.Template?.Template is not { } templateContent ||
            result.Parameters is not { } parametersContent)
        {
            return 1;
        }

        var model = compilation.GetEntrypointSemanticModel();

        var snapshot = await SnapshotHelper.GetSnapshot(
            targetScope: model.TargetScope,
            templateContent: templateContent,
            parametersContent: parametersContent,
            tenantId: null,
            subscriptionId: null,
            resourceGroup: null,
            location: null,
            deploymentName: null,
            cancellationToken: cancellationToken,
            externalInputs: []);
            
        var armClient = armClientProvider.CreateArmClient(model.Configuration, null);

        if (model.TargetScope == ResourceScope.Orchestrator)
        {
            var hasFailures = false;
            foreach (var stackResource in snapshot.PredictedResources)
            {
                var symbolicName = stackResource.GetPropertyByPath("symbolicName").GetString()!;
                var stackParameters = stackResource.GetPropertyByPath("properties.parameters").GetString()!;
                var sourceUriString = stackResource.GetPropertyByPath("properties.sourceUri").GetString()!;
                var region = stackResource.GetPropertyByPath("properties.body.region").GetString()!;
                var inputs = stackResource.GetPropertyByPath("properties.body.inputs");

                var sourceUri = new Uri(sourceUriString).ToIOUri();

                var paramsResult = await NestedDeploymentExtension.ProcessParameters(stackParameters, inputs.ToJsonString().FromJson<JObject>());
                var deploymentName = paramsResult.UsingConfig.Name ?? "main";

                var stackCompilation = await compiler.CreateCompilation(sourceUri);
                var stackSummary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, stackCompilation);

                if (stackSummary.HasErrors ||
                    stackCompilation.Emitter.Parameters() is not { } stackResult ||
                    stackResult.Template?.Template is not { } stackTemplateContent ||
                    stackResult.Parameters is not { } stackParametersContent)
                {
                    hasFailures = true;
                    continue;
                }

                var deploymentResource = armClient.GetArmDeploymentResource(new ResourceIdentifier($"{paramsResult.UsingConfig.Scope}/providers/Microsoft.Resources/deployments/{deploymentName}"));
                var paramsDefinition = paramsResult.Parameters.FromJson<DeploymentParametersDefinition>();
                var deploymentProperties = new ArmDeploymentWhatIfProperties(ArmDeploymentMode.Incremental)
                {
                    Template = BinaryData.FromString(stackTemplateContent),
                    Parameters = BinaryData.FromString(paramsDefinition.Parameters.ToJson()),
                };
                var armDeploymentContent = new ArmDeploymentWhatIfContent(deploymentProperties);

                var response = await deploymentResource.WhatIfAsync(Azure.WaitUntil.Completed, armDeploymentContent, cancellationToken);

                var definition = response.GetRawResponse().Content.ToString().FromJson<DeploymentWhatIfResponseDefinition>();
                var changes = definition.Properties.Changes.Where(x => x.ChangeType != DeploymentWhatIfChangeType.Ignore);

                logger.LogInformation($"WhatIf results for Stack {symbolicName} - {sourceUri}:");
                await io.Output.WriteAsync(WhatIfOperationResultFormatter.Format([.. changes]));
            }

            return hasFailures ? 1 : 0;
        }

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