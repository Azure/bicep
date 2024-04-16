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

namespace Bicep.Cli.Commands
{
    public class BuildParamsCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly IEnvironment environment;
        private readonly DiagnosticLogger diagnosticLogger;
        private readonly BicepCompiler compiler;
        private readonly OutputWriter writer;
        private readonly IFeatureProviderFactory featureProviderFactory;

        public BuildParamsCommand(
            ILogger logger,
            IEnvironment environment,
            DiagnosticLogger diagnosticLogger,
            BicepCompiler compiler,
            OutputWriter writer,
            IFeatureProviderFactory featureProviderFactory)
        {
            this.logger = logger;
            this.environment = environment;
            this.diagnosticLogger = diagnosticLogger;
            this.compiler = compiler;
            this.writer = writer;
            this.featureProviderFactory = featureProviderFactory;
        }

        public async Task<int> RunAsync(BuildParamsArguments args)
        {
            var paramsFileUri = ArgumentHelper.GetFileUri(args.ParamsFile);
            ArgumentHelper.ValidateBicepParamFile(paramsFileUri);
            var bicepFileUri = ArgumentHelper.GetFileUri(args.BicepFile);

            if (bicepFileUri != null)
            {
                ArgumentHelper.ValidateBicepFile(bicepFileUri);
            }

            var workspace = await CreateWorkspaceWithParamOverrides(paramsFileUri);
            var compilation = await compiler.CreateCompilation(
                paramsFileUri,
                workspace,
                skipRestore: args.NoRestore);

            if (bicepFileUri is not null &&
                compilation.GetEntrypointSemanticModel().Root.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var usingModel))
            {
                if (usingModel is not SemanticModel bicepSemanticModel)
                {
                    throw new CommandLineException($"Bicep file {bicepFileUri.LocalPath} provided with --bicep-file can only be used if the Bicep parameters \"using\" declaration refers to a Bicep file on disk.");
                }

                if (!bicepSemanticModel.Root.FileUri.Equals(bicepFileUri))
                {
                    throw new CommandLineException($"Bicep file {bicepFileUri.LocalPath} provided with --bicep-file option doesn't match the Bicep file {bicepSemanticModel.Root.Name} referenced by the \"using\" declaration in the parameters file.");
                }
            }

            if (ExperimentalFeatureWarningProvider.TryGetEnabledExperimentalFeatureWarningMessage(compilation.SourceFileGrouping, featureProviderFactory) is { } message)
            {
                logger.LogWarning(message);
            }

            var summary = diagnosticLogger.LogDiagnostics(GetDiagnosticOptions(args), compilation);

            var paramsModel = compilation.GetEntrypointSemanticModel();

            //Failure scenario is ignored since a diagnostic for it would be emitted during semantic analysis
            if (!summary.HasErrors)
            {
                if (args.OutputToStdOut)
                {
                    writer.ParametersToStdout(compilation);
                }
                else
                {
                    var outputPath = PathHelper.ResolveDefaultOutputPath(paramsFileUri.LocalPath, args.OutputDir, args.OutputFile, PathHelper.GetDefaultBuildOutputPath);

                    writer.ParametersToFile(compilation, PathHelper.FilePathToFileUrl(outputPath));
                }
            }

            // return non-zero exit code on errors
            return summary.HasErrors ? 1 : 0;
        }

        private async Task<Workspace> CreateWorkspaceWithParamOverrides(Uri paramsFileUri)
        {
            var fileContents = await File.ReadAllTextAsync(paramsFileUri.LocalPath);
            var sourceFile = SourceFileFactory.CreateBicepParamFile(paramsFileUri, fileContents);
            var paramsOverridesJson = environment.GetVariable("BICEP_PARAMETERS_OVERRIDES") ?? "";

            var parameters = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(
                paramsOverridesJson,
                new JsonSerializerSettings()
                {
                    DateParseHandling = DateParseHandling.None,
                });

            sourceFile = ParamsFileHelper.ApplyParameterOverrides(sourceFile, parameters ?? new());

            var workspace = new Workspace();
            workspace.UpsertSourceFile(sourceFile);

            return workspace;
        }

        private DiagnosticOptions GetDiagnosticOptions(BuildParamsArguments args)
            => new(
                Format: args.DiagnosticsFormat ?? DiagnosticsFormat.Default,
                SarifToStdout: false);
    }
}
