// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Rewriters;
using Bicep.Core.Workspaces;
using Microsoft.Diagnostics.Tracing.Parsers.FrameworkEventSource;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SharpYaml.Tokens;

namespace Bicep.Cli.Commands
{
    public class BuildParamsCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly IOContext io;
        private readonly CompilationService compilationService;
        private readonly CompilationWriter writer;
        private readonly IFeatureProviderFactory featureProviderFactory;

        public BuildParamsCommand(
            ILogger logger,
            IDiagnosticLogger diagnosticLogger,
            IOContext io,
            CompilationService compilationService,
            CompilationWriter writer,
            IFeatureProviderFactory featureProviderFactory)
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.io = io;
            this.compilationService = compilationService;
            this.writer = writer;
            this.featureProviderFactory = featureProviderFactory;
        }

        public async Task<int> RunAsync(BuildParamsArguments args)
        {
            var paramsInputPath = PathHelper.ResolvePath(args.ParamsFile);
            var bicepFileArgPath = args.BicepFile != null ? PathHelper.ResolvePath(args.BicepFile) : null;

            if (bicepFileArgPath != null && !IsBicepFile(bicepFileArgPath))
            {
                throw new CommandLineException($"{bicepFileArgPath} is not a bicep file");
            }

            if (!IsBicepparamsFile(paramsInputPath))
            {
                logger.LogError(CliResources.UnrecognizedBicepparamsFileExtensionMessage, paramsInputPath);
                return 1;
            }

            var paramsOverrides = System.Environment.GetEnvironmentVariable("BICEP_PARAMETERS_OVERRIDES");

            if (paramsOverrides is { })
            {
                var paramsOverridesJson = JObject.Parse(paramsOverrides);

                var fileContents = await File.ReadAllTextAsync(paramsInputPath);
                var sourceFile = SourceFileFactory.CreateBicepParamFile(PathHelper.FilePathToFileUrl(paramsInputPath), fileContents);

                var newProgramSyntax = CallbackRewriter.Rewrite(sourceFile.ProgramSyntax, syntax =>
                {
                    if (syntax is not ParameterAssignmentSyntax paramSyntax)
                    {
                        return syntax;
                    }

                    var overrideJsonValue = paramsOverridesJson.GetValue(paramSyntax.Name.IdentifierName);

                    if(overrideJsonValue is {})
                    {
                        var replacementValue = ConvertJsonToBicepSyntax(overrideJsonValue);

                        return new ParameterAssignmentSyntax(
                            paramSyntax.Keyword,
                            paramSyntax.Name,
                            paramSyntax.Assignment,
                            replacementValue
                        );
                    }
                    
                    return syntax;
                });

                fileContents = newProgramSyntax.ToTextPreserveFormatting();
                sourceFile = SourceFileFactory.CreateBicepParamFile(PathHelper.FilePathToFileUrl(paramsInputPath), fileContents);
                compilationService.Workspace.UpsertSourceFile(sourceFile);
            }

            var paramsCompilation = await compilationService.CompileAsync(
                paramsInputPath,
                args.NoRestore,
                compilation =>
                {
                    if (bicepFileArgPath is not null &&
                        compilation.GetEntrypointSemanticModel().Root.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var usingModel))
                    {
                        if (usingModel is not SemanticModel bicepSemanticModel)
                        {
                            throw new CommandLineException($"Bicep file {bicepFileArgPath} provided with --bicep-file can only be used if the Bicep parameters \"using\" declaration refers to a Bicep file on disk.");
                        }

                        if (!bicepSemanticModel.Root.FileUri.Equals(PathHelper.FilePathToFileUrl(bicepFileArgPath)))
                        {
                            throw new CommandLineException($"Bicep file {bicepFileArgPath} provided with --bicep-file option doesn't match the Bicep file {bicepSemanticModel.Root.Name} referenced by the \"using\" declaration in the parameters file.");
                        }
                    }
                });

            if (ExperimentalFeatureWarningProvider.TryGetEnabledExperimentalFeatureWarningMessage(paramsCompilation.SourceFileGrouping, featureProviderFactory) is { } message)
            {
                logger.LogWarning(message);
            }

            var paramsModel = paramsCompilation.GetEntrypointSemanticModel();

            //Failure scenario is ignored since a diagnostic for it would be emitted during semantic analysis
            if (paramsModel.Root.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var usingModel))
            {
                if (diagnosticLogger.ErrorCount < 1)
                {
                    if (args.OutputToStdOut)
                    {
                        writer.ToStdout(paramsModel, usingModel);
                    }
                    else
                    {
                        static string DefaultOutputPath(string path) => PathHelper.GetDefaultBuildOutputPath(path);
                        var paramsOutputPath = PathHelper.ResolveDefaultOutputPath(paramsInputPath, null, args.OutputFile, DefaultOutputPath);

                        writer.ToFile(paramsCompilation, paramsOutputPath);
                    }
                }
            }

            return diagnosticLogger.ErrorCount > 0 ? 1 : 0;
        }

        private bool IsBicepFile(string inputPath) => PathHelper.HasBicepExtension(PathHelper.FilePathToFileUrl(inputPath));

        private bool IsBicepparamsFile(string inputPath) => PathHelper.HasBicepparamsExension(PathHelper.FilePathToFileUrl(inputPath));

        private static readonly ImmutableHashSet<JTokenType> SupportedJsonTokenTypes = new[] { JTokenType.Object, JTokenType.Array, JTokenType.String, JTokenType.Integer, JTokenType.Float, JTokenType.Boolean, JTokenType.Null }.ToImmutableHashSet();

        private static SyntaxBase ConvertJsonToBicepSyntax(JToken token) =>
        token switch
        {
            JObject @object => SyntaxFactory.CreateObject(@object.Properties().Where(x => SupportedJsonTokenTypes.Contains(x.Value.Type)).Select(x => SyntaxFactory.CreateObjectProperty(x.Name, ConvertJsonToBicepSyntax(x.Value)))),
            JArray @array => SyntaxFactory.CreateArray(@array.Where(x => SupportedJsonTokenTypes.Contains(x.Type)).Select(ConvertJsonToBicepSyntax)),
            JValue value => value.Type switch
            {
                JTokenType.String => SyntaxFactory.CreateStringLiteral(value.ToString(CultureInfo.InvariantCulture)),
                JTokenType.Integer => SyntaxFactory.CreatePositiveOrNegativeInteger(value.Value<long>()),
                // Floats are currently not supported in Bicep, so fall back to the default behavior of "any"
                JTokenType.Float => SyntaxFactory.CreateFunctionCall("json", SyntaxFactory.CreateStringLiteral(value.ToObject<double>().ToString(CultureInfo.InvariantCulture))),
                JTokenType.Boolean => SyntaxFactory.CreateBooleanLiteral(value.ToObject<bool>()),
                JTokenType.Null => SyntaxFactory.CreateFunctionCall("null"),
                _ => throw new InvalidOperationException($"Cannot parse JSON object. Unsupported value token type: {value.Type}"),
            },
            _ => throw new InvalidOperationException($"Cannot parse JSON object. Unsupported token: {token.Type}")
        };
    }
}
