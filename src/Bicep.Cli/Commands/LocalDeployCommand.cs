// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Json;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.IO.Abstraction;
using Bicep.Local.Deploy;
using Bicep.Local.Deploy.Extensibility;
using Bicep.Local.Extension.Rpc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bicep.Cli.Commands;

public class LocalDeployCommand : ICommand
{
    private readonly IFileExplorer fileExplorer;
    private readonly IModuleDispatcher moduleDispatcher;
    private readonly IConfigurationManager configurationManager;
    private readonly ITokenCredentialFactory credentialFactory;
    private readonly IOContext io;
    private readonly ILogger logger;
    private readonly DiagnosticLogger diagnosticLogger;
    private readonly BicepCompiler compiler;

    public LocalDeployCommand(
        IFileExplorer fileExplorer,
        IModuleDispatcher moduleDispatcher,
        IConfigurationManager configurationManager,
        ITokenCredentialFactory credentialFactory,
        IOContext io,
        ILogger logger,
        DiagnosticLogger diagnosticLogger,
        BicepCompiler compiler)
    {
        this.fileExplorer = fileExplorer;
        this.moduleDispatcher = moduleDispatcher;
        this.configurationManager = configurationManager;
        this.credentialFactory = credentialFactory;
        this.io = io;
        this.logger = logger;
        this.diagnosticLogger = diagnosticLogger;
        this.compiler = compiler;
    }

    public async Task<int> RunAsync(LocalDeployArguments args, CancellationToken cancellationToken)
    {
        var paramsFileUri = ArgumentHelper.GetFileUri(args.ParamsFile);
        ArgumentHelper.ValidateBicepParamFile(paramsFileUri);

        var compilation = await compiler.CreateCompilation(
            paramsFileUri,
            skipRestore: args.NoRestore);

        var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, compilation);
        var parameters = compilation.Emitter.Parameters();

        if (summary.HasErrors ||
            parameters.Parameters is not { } parametersString ||
            parameters.Template?.Template is not { } templateString)
        {
            return 1;
        }

        if (!compilation.GetEntrypointSemanticModel().Features.LocalDeployEnabled)
        {
            logger.LogError("Experimental feature 'localDeploy' is not enabled.");
            return 1;
        }

        if (compilation.GetEntrypointSemanticModel().Root.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var usingModel) &&
            usingModel.TargetScope is not ResourceScope.Local)
        {
            logger.LogError($"The referenced .bicep file must have targetScope set to 'local'.");
            return 1;
        }

        await using LocalExtensionHostManager extensionHostManager = new(fileExplorer, moduleDispatcher, configurationManager, credentialFactory, GrpcBuiltInLocalExtension.Start);
        await extensionHostManager.InitializeExtensions(compilation);
        var result = await extensionHostManager.Deploy(templateString, parametersString, cancellationToken);

        await WriteSummary(result);
        return 0;
    }

    private async Task WriteSummary(LocalDeploymentResult result)
    {
        if (result.Deployment.Properties.Outputs is { } outputs)
        {
            foreach (var output in outputs)
            {
                await io.Output.WriteLineAsync($"Output {output.Key}: {JsonConvert.SerializeObject(output.Value.Value, Formatting.Indented, SerializerSettings.SerializerObjectTypeSettings)}");
            }
        }

        if (result.Deployment.Properties.Error is { } error)
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
