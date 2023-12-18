// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Azure.Bicep.LocalDeploy;
using Azure.Bicep.LocalDeploy.Extensibility;
using Azure.Deployments.Core.Json;
using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;

namespace Bicep.Cli.Commands;

public class LocalDeployCommand : ICommand
{
    private readonly IOContext io;
    private readonly ILogger logger;
    private readonly DiagnosticLogger diagnosticLogger;
    private readonly CompilationService compilationService;

    public LocalDeployCommand(
        IOContext io,
        ILogger logger,
        DiagnosticLogger diagnosticLogger,
        CompilationService compilationService)
    {
        this.io = io;
        this.logger = logger;
        this.diagnosticLogger = diagnosticLogger;
        this.compilationService = compilationService;
    }

    public async Task<int> RunAsync(LocalDeployArguments args, CancellationToken cancellationToken)
    {
        var paramsInputPath = PathHelper.ResolvePath(args.ParamsFile);

        if (!PathHelper.HasBicepparamsExension(PathHelper.FilePathToFileUrl(paramsInputPath)))
        {
            logger.LogError(CliResources.UnrecognizedBicepparamsFileExtensionMessage, paramsInputPath);
            return 1;
        }

        var paramsCompilation = await compilationService.CompileAsync(paramsInputPath, args.NoRestore);

        var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, paramsCompilation);
        if (summary.HasErrors)
        {
            return 1;
        }

        var paramsModel = paramsCompilation.GetEntrypointSemanticModel();
        //Failure scenario is ignored since a diagnostic for it would be emitted during semantic analysis
        if (!paramsModel.Root.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var usingModel))
        {
            return 1;
        }

        if (paramsModel.Features.LocalDeployEnabled)
        {
            logger.LogError("Experimental feature 'localDeploy' is not enabled.");
            return 1;
        }

        var (_, parametersString) = CompilationWriter.EmitParameters(paramsModel);
        var (_, templateString) = CompilationWriter.EmitTemplate(paramsModel, usingModel);

        var extensibilityHandler = new LocalExtensibilityHandler();
        extensibilityHandler.Register(UtilsNamespaceType.Settings.ArmTemplateProviderName, UtilsNamespaceType.Settings.ArmTemplateProviderVersion, () => new UtilsExtensibilityProvider());
        extensibilityHandler.Register(K8sNamespaceType.Settings.ArmTemplateProviderName, K8sNamespaceType.Settings.ArmTemplateProviderVersion, () => new K8sExtensibilityProvider());

        var result = await LocalDeployment.Deploy(extensibilityHandler, templateString, parametersString, cancellationToken);

        await WriteSummary(result);
        return 0;
    }

    private async Task WriteSummary(LocalDeployment.Result result)
    {
        if (result.Deployment.Properties.Outputs is {} outputs)
        {
            foreach (var output in outputs)
            {
                await io.Output.WriteLineAsync($"Output {output.Key}: {JsonConvert.SerializeObject(output.Value.Value, Formatting.Indented, SerializerSettings.SerializerObjectTypeSettings)}");
            }
        }

        if (result.Deployment.Properties.Error is {} error)
        {
            foreach (var subError in error.Details)
            {
                await io.Output.WriteLineAsync($"Error: {subError.Code} - {subError.Message}");
            }
        }

        foreach (var operation in result.Operations)
        {
            await io.Output.WriteLineAsync($"Resource {operation.Properties.TargetResource.SymbolicName} ({operation.Properties.ProvisioningOperation}): {operation.Properties.ProvisioningState}");
        }

        await io.Output.WriteLineAsync($"Result: {result.Deployment.Properties.ProvisioningState}");
    }
}