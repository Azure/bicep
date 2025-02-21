// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Utils;
using Bicep.Core.Workspaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.Commands;

public class BuildParamsCommand : ICommand
{
    private readonly ILogger logger;
    private readonly IEnvironment environment;
    private readonly DiagnosticLogger diagnosticLogger;
    private readonly BicepCompiler compiler;
    private readonly OutputWriter writer;
    private readonly ISourceFileFactory sourceFileFactory;

    public BuildParamsCommand(
        ILogger logger,
        IEnvironment environment,
        DiagnosticLogger diagnosticLogger,
        BicepCompiler compiler,
        OutputWriter writer,
        ISourceFileFactory sourceFileFactory)
    {
        this.logger = logger;
        this.environment = environment;
        this.diagnosticLogger = diagnosticLogger;
        this.compiler = compiler;
        this.writer = writer;
        this.sourceFileFactory = sourceFileFactory;
    }

    public async Task<int> RunAsync(BuildParamsArguments args)
    {
        if (args.InputFile is null)
        {
            var summaryMultiple = await CompileMultiple(args);
            return CommandHelper.GetExitCode(summaryMultiple);
        }

        var inputUri = ArgumentHelper.GetFileUri(args.InputFile);
        ArgumentHelper.ValidateBicepParamFile(inputUri);

        var outputUri = CommandHelper.GetJsonOutputUri(inputUri, args.OutputDir, args.OutputFile);

        var bicepFileUri = ArgumentHelper.GetFileUri(args.BicepFile);
        if (bicepFileUri != null)
        {
            ArgumentHelper.ValidateBicepFile(bicepFileUri);
        }

        var workspace = await CreateWorkspaceWithParamOverrides(inputUri);
        var summary = await Compile(workspace, inputUri, bicepFileUri, outputUri, args.NoRestore, args.DiagnosticsFormat, args.OutputToStdOut);
        return CommandHelper.GetExitCode(summary);
    }

    private async Task<DiagnosticSummary> Compile(Workspace? workspace, Uri inputUri, Uri? bicepFileUri, Uri outputUri, bool noRestore, DiagnosticsFormat? diagnosticsFormat, bool outputToStdOut)
    {
        var compilation = await compiler.CreateCompilation(
            inputUri,
            workspace,
            skipRestore: noRestore);

        ValidateBicepFile(compilation, bicepFileUri);

        if (ExperimentalFeatureWarningProvider.TryGetEnabledExperimentalFeatureWarningMessage(compilation.SourceFileGrouping) is { } message)
        {
            logger.LogWarning(message);
        }

        var summary = diagnosticLogger.LogDiagnostics(ArgumentHelper.GetDiagnosticOptions(diagnosticsFormat), compilation);

        if (!summary.HasErrors)
        {
            if (outputToStdOut)
            {
                writer.ParametersToStdout(compilation);
            }
            else
            {
                writer.ParametersToFile(compilation, outputUri);
            }
        }

        return summary;
    }

    public async Task<DiagnosticSummary> CompileMultiple(BuildParamsArguments args)
    {
        var hasErrors = false;

        foreach (var inputUri in CommandHelper.GetFilesMatchingPattern(environment, args.FilePatternRoot, args.FilePatterns))
        {
            ArgumentHelper.ValidateBicepParamFile(inputUri);

            var outputUri = CommandHelper.GetJsonOutputUri(inputUri, null, null);

            var result = await Compile(null, inputUri, null, outputUri, args.NoRestore, args.DiagnosticsFormat, false);
            hasErrors |= result.HasErrors;
        }

        return new(hasErrors);
    }

    private async Task<Workspace> CreateWorkspaceWithParamOverrides(Uri paramsFileUri)
    {
        var fileContents = await File.ReadAllTextAsync(paramsFileUri.LocalPath);
        var sourceFile = this.sourceFileFactory.CreateBicepParamFile(paramsFileUri, fileContents);
        var paramsOverridesJson = environment.GetVariable("BICEP_PARAMETERS_OVERRIDES") ?? "";

        var parameters = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(
            paramsOverridesJson,
            new JsonSerializerSettings()
            {
                DateParseHandling = DateParseHandling.None,
            });

        sourceFile = ParamsFileHelper.ApplyParameterOverrides(this.sourceFileFactory, sourceFile, parameters ?? []);

        var workspace = new Workspace();
        workspace.UpsertSourceFile(sourceFile);

        return workspace;
    }

    private static void ValidateBicepFile(Compilation compilation, Uri? bicepFileUri)
    {
        if (bicepFileUri is not null &&
            compilation.GetEntrypointSemanticModel().Root.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var usingModel))
        {
            if (usingModel is EmptySemanticModel)
            {
                // "using none" is permitted
                return;
            }

            if (usingModel is not SemanticModel bicepSemanticModel)
            {
                throw new CommandLineException($"Bicep file {bicepFileUri.LocalPath} provided with --bicep-file can only be used if the Bicep parameters \"using\" declaration refers to a Bicep file on disk.");
            }

            if (!bicepSemanticModel.Root.FileUri.Equals(bicepFileUri))
            {
                throw new CommandLineException($"Bicep file {bicepFileUri.LocalPath} provided with --bicep-file option doesn't match the Bicep file {bicepSemanticModel.Root.Name} referenced by the \"using\" declaration in the parameters file.");
            }
        }
    }
}
