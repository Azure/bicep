// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Configuration;
using Azure.Core;
using Azure.ResourceManager.Resources;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers.Deploy;
using Bicep.Cli.Helpers.Snapshot;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.AzureApi;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.Utils;
using Bicep.Local.Deploy.Extensibility;
using Json.More;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.Commands;

public class TeardownCommand : DeploymentsCommandsBase<TeardownArguments>
{
    private readonly IOContext io;
    private readonly ILogger logger;
    private readonly IEnvironment environment;
    private readonly IDeploymentProcessor deploymentProcessor;
    private readonly IArmClientProvider armClientProvider;
    private readonly DiagnosticLogger diagnosticLogger;
    private readonly BicepCompiler compiler;
    private readonly DeploymentRenderer deploymentRenderer;

    public TeardownCommand(
        IOContext io,
        ILogger logger,
        IEnvironment environment,
        IDeploymentProcessor deploymentProcessor,
        IArmClientProvider armClientProvider,
        DiagnosticLogger diagnosticLogger,
        BicepCompiler compiler,
        InputOutputArgumentsResolver inputOutputArgumentsResolver,
        DeploymentRenderer deploymentRenderer) : base(logger, diagnosticLogger, compiler, inputOutputArgumentsResolver)
    {
        this.io = io;
        this.logger = logger;
        this.environment = environment;
        this.deploymentProcessor = deploymentProcessor;
        this.armClientProvider = armClientProvider;
        this.diagnosticLogger = diagnosticLogger;
        this.compiler = compiler;
        this.deploymentRenderer = deploymentRenderer;
    }

    protected override async Task<int> RunInternal(TeardownArguments args, Compilation compilation, SemanticModel model, ParametersResult result, CancellationToken cancellationToken)
    {
        var config = await DeploymentProcessor.GetDeployCommandsConfig(environment, args.AdditionalArguments, result, model.TargetScope);

        var success = await deploymentRenderer.RenderOperation(
            DeploymentRenderer.RefreshInterval,
            (onUpdate) => deploymentProcessor.Teardown(model.Configuration, config, onUpdate, cancellationToken),
            cancellationToken);

        return success ? 0 : 1;
    }

    protected override async Task<int> RunOrchestrationInternal(TeardownArguments args, Compilation compilation, SemanticModel model, CancellationToken cancellationToken)
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

        var success = await deploymentRenderer.RenderOperation(
            DeploymentRenderer.RefreshInterval,
            async (onUpdate) =>
            {
                var hasFailures = false;
                onUpdate(new("Teardown", "Running", null));
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

                    var resourceId = $"{paramsResult.UsingConfig.Scope}/providers/Microsoft.Resources/deploymentStacks/{deploymentName}";
                    var deploymentResource = armClient.GetDeploymentStackResource(new ResourceIdentifier(resourceId));

                    await deploymentResource.DeleteAsync(Azure.WaitUntil.Completed, cancellationToken: cancellationToken);
                }

                onUpdate(new("Teardown", hasFailures ? "Failed" : "Succeeded", null));
            },
            cancellationToken);

        return success ? 0 : 1;
    }
}
