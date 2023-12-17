// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Azure.Bicep.LocalDeploy;
using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Utils;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands;

public class LocalDeployCommand : ICommand
{
    private readonly ILogger logger;
    private readonly IEnvironment environment;
    private readonly DiagnosticLogger diagnosticLogger;
    private readonly CompilationService compilationService;
    private readonly CompilationWriter writer;
    private readonly IFeatureProviderFactory featureProviderFactory;

    public LocalDeployCommand(
        ILogger logger,
        IEnvironment environment,
        DiagnosticLogger diagnosticLogger,
        CompilationService compilationService,
        CompilationWriter writer,
        IFeatureProviderFactory featureProviderFactory)
    {
        this.logger = logger;
        this.environment = environment;
        this.diagnosticLogger = diagnosticLogger;
        this.compilationService = compilationService;
        this.writer = writer;
        this.featureProviderFactory = featureProviderFactory;
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

        var (_, parametersString) = CompilationWriter.EmitParameters(paramsModel);
        var (_, templateString) = CompilationWriter.EmitTemplate(paramsModel, usingModel);

        var result = await LocalDeployment.Deploy(templateString, parametersString, cancellationToken);
        return 0;
    }
}