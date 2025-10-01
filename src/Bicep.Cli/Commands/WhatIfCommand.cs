// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Entities;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers.Deploy;
using Bicep.Cli.Helpers.Snapshot;
using Bicep.Cli.Helpers.WhatIf;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.AzureApi;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.Utils;
using Bicep.Local.Deploy.Extensibility;
using Json.More;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.Commands;

public class WhatIfCommand : DeploymentsCommandsBase<WhatIfArguments>
{
    private readonly IOContext io;
    private readonly ILogger logger;
    private readonly IEnvironment environment;
    private readonly IDeploymentProcessor deploymentProcessor;
    private readonly IArmClientProvider armClientProvider;
    private readonly DiagnosticLogger diagnosticLogger;
    private readonly BicepCompiler compiler;

    public WhatIfCommand(
        IOContext io,
        ILogger logger,
        IEnvironment environment,
        IDeploymentProcessor deploymentProcessor,
        IArmClientProvider armClientProvider,
        DiagnosticLogger diagnosticLogger,
        BicepCompiler compiler,
        InputOutputArgumentsResolver inputOutputArgumentsResolver) : base(logger, diagnosticLogger, compiler, inputOutputArgumentsResolver)
    {
        this.io = io;
        this.logger = logger;
        this.environment = environment;
        this.deploymentProcessor = deploymentProcessor;
        this.armClientProvider = armClientProvider;
        this.diagnosticLogger = diagnosticLogger;
        this.compiler = compiler;
    }

    protected override async Task<int> RunInternal(WhatIfArguments args, Compilation compilation, SemanticModel model, ParametersResult result, CancellationToken cancellationToken)
    {
        var config = await DeploymentProcessor.GetDeployCommandsConfig(environment, args.AdditionalArguments, result, model.TargetScope);

        await WhatIf(model, config, cancellationToken);

        return 0;
    }

    protected override async Task<int> RunOrchestrationInternal(WhatIfArguments args, Compilation compilation, SemanticModel model, CancellationToken cancellationToken)
    {
        var parameters = compilation.Emitter.Parameters();

        if (parameters.Template?.Template is not { } templateContent ||
            parameters.Parameters is not { } parametersContent)
        {
            return 1;
        }

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

    private async Task WhatIf(SemanticModel model, DeployCommandsConfig config, CancellationToken cancellationToken)
    {
        var result = await deploymentProcessor.WhatIf(model.Configuration, config, cancellationToken);

        var changes = result.Properties.Changes.Where(x => x.ChangeType != DeploymentWhatIfChangeType.Ignore);

        await io.Output.WriteAsync(WhatIfOperationResultFormatter.Format([.. changes]));
    }
}
