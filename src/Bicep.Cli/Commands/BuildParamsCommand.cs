// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.Commands;

public class BuildParamsCommand(
    ILogger logger,
    IEnvironment environment,
    DiagnosticLogger diagnosticLogger,
    BicepCompiler compiler,
    OutputWriter writer,
    ISourceFileFactory sourceFileFactory,
    IFileExplorer fileExplorer,
    InputOutputArgumentsResolver inputOutputArgumentsResolver) : ICommand
{
    public async Task<int> RunAsync(BuildParamsArguments args)
    {
        var inputOutputUriPairs = inputOutputArgumentsResolver.ResolveFilePatternInputOutputArguments(args);

        if (inputOutputUriPairs.Count != 1)
        {
            var summaryMultiple = await CompileMultiple(args, inputOutputUriPairs);
            return CommandHelper.GetExitCode(summaryMultiple);
        }

        var inputUri = inputOutputUriPairs[0].InputUri;
        ArgumentHelper.ValidateBicepParamFile(inputUri);

        var outputUri = inputOutputUriPairs[0].OutputUri;

        var bicepFileUri = args.BicepFile is not null ? inputOutputArgumentsResolver.PathToUri(args.BicepFile) : null;
        if (bicepFileUri is not null)
        {
            ArgumentHelper.ValidateBicepFile(bicepFileUri);
        }

        var workspace = await CreateWorkspaceWithParameterOverridesIfPresent(inputUri);
        var summary = await Compile(workspace, inputUri, bicepFileUri, outputUri, args.NoRestore, args.DiagnosticsFormat, args.OutputToStdOut);
        return CommandHelper.GetExitCode(summary);
    }

    private async Task<DiagnosticSummary> Compile(ActiveSourceFileSet? workspace, IOUri inputUri, IOUri? bicepFileUri, IOUri outputUri, bool noRestore, DiagnosticsFormat? diagnosticsFormat, bool outputToStdOut)
    {
        var compilation = await compiler.CreateCompilation(
            inputUri,
            workspace,
            skipRestore: noRestore);

        ValidateBicepFile(compilation, bicepFileUri);
        CommandHelper.LogExperimentalWarning(logger, compilation);

        var summary = diagnosticLogger.LogDiagnostics(ArgumentHelper.GetDiagnosticOptions(diagnosticsFormat), compilation);

        if (!summary.HasErrors)
        {
            if (outputToStdOut)
            {
                writer.ParametersToStdout(compilation);
            }
            else
            {
                await writer.ParametersToFileAsync(compilation, outputUri);
            }
        }

        return summary;
    }

    public async Task<DiagnosticSummary> CompileMultiple(BuildParamsArguments args, IEnumerable<(IOUri, IOUri)> inputOuputPairs)
    {
        var hasErrors = false;

        foreach (var (inputUri, outputUri) in inputOuputPairs)
        {
            ArgumentHelper.ValidateBicepParamFile(inputUri);

            var result = await Compile(null, inputUri, null, outputUri, args.NoRestore, args.DiagnosticsFormat, outputToStdOut: false);
            hasErrors |= result.HasErrors;
        }

        return new(hasErrors);
    }

    private async Task<ActiveSourceFileSet?> CreateWorkspaceWithParameterOverridesIfPresent(IOUri paramsFileUri)
    {
        var parameterOverridesJson = environment.GetVariable("BICEP_PARAMETERS_OVERRIDES");

        if (string.IsNullOrEmpty(parameterOverridesJson))
        {
            return null;
        }

        string paramsFileText;

        try
        {
            paramsFileText = await fileExplorer.GetFile(paramsFileUri).ReadAllTextAsync();
        }
        catch
        {
            // We cannot read the input file. Returning null to let Bicep.Core handle the error later.
            return null;
        }

        var sourceFile = sourceFileFactory.CreateBicepParamFile(paramsFileUri, paramsFileText);
        var parameterOverrides = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(parameterOverridesJson, new JsonSerializerSettings()
        {
            DateParseHandling = DateParseHandling.None,
        });

        if (parameterOverrides is null || parameterOverrides.Count == 0)
        {
            return null;
        }

        sourceFile = ParamsFileHelper.ApplyParameterOverrides(sourceFileFactory, sourceFile, parameterOverrides);

        var workspace = new ActiveSourceFileSet();
        workspace.UpsertSourceFile(sourceFile);

        return workspace;
    }

    private static void ValidateBicepFile(Compilation compilation, IOUri? bicepFileUri)
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
                throw new CommandLineException($"Bicep file {bicepFileUri} provided with --bicep-file can only be used if the Bicep parameters \"using\" declaration refers to a Bicep file on disk.");
            }

            if (!bicepSemanticModel.SourceFile.FileHandle.Uri.Equals(bicepFileUri))
            {
                throw new CommandLineException($"Bicep file {bicepFileUri} provided with --bicep-file option doesn't match the Bicep file {bicepSemanticModel.Root.Name} referenced by the \"using\" declaration in the parameters file.");
            }
        }
    }
}
