// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using System.Threading.Tasks;
using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.FileSystem;
using Microsoft.Extensions.Logging;

using Bicep.Core.Diagnostics;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;


// TODO: delete this when done :D
// Parse the param file
// Get the path to the linked Bicep file
// var compilation = compilationService.CompileAsync(inputPath, args.NoRestore);
// var entryPointSemanticModel = compilation.GetEntrypointSemanticModel();
// var declaredTypes = Get types from entryPointSemanticModel.Parameters

// !!!!!!
// var inferredTypes = Get types from the parameters file
//   - Create a binder to get the symbols - IParamsBinder, ParamsBinder
//   - Use a visitor to get the types of the symbols - ParamsTypeManager

// Compare declaredType with inferredTypes, emit errors if types don't match

namespace Bicep.Cli.Commands
{
    public class BuildParamCommand
    {
        private readonly ILogger logger;
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly InvocationContext invocationContext;
        private readonly CompilationService compilationService;
        private readonly CompilationWriter writer;

        public BuildParamCommand(
            ILogger logger,
            IDiagnosticLogger diagnosticLogger,
            InvocationContext invocationContext,
            CompilationService compilationService,
            CompilationWriter writer)
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.invocationContext = invocationContext;
            this.compilationService = compilationService;
            this.writer = writer;
        }

        public async Task<int> RunAsync(BuildArguments args)
        {
            var inputPath = PathHelper.ResolvePath(args.InputFile);
            var inputUri = PathHelper.FilePathToFileUrl(inputPath);

            if (PathHelper.HasExtension(inputUri, LanguageConstants.ParamsFileExtension))
            {
                var parser = new Parser(inputPath);
                var program = parser.Program();
                var usingDeclarations = program.Children.OfType<UsingDeclarationSyntax>();
                var paramPath = TryGetUsingPath(usingDeclarations.SingleOrDefault(), out var getUsingPathFailureBuilder);
            }
            
            if (invocationContext.EmitterSettings.EnableSymbolicNames)
            {
                logger.LogWarning(CliResources.SymbolicNamesDisclaimerMessage);
            }

            if (invocationContext.Features.ResourceTypedParamsAndOutputsEnabled)
            {
                logger.LogWarning(CliResources.ResourceTypesDisclaimerMessage);
            }

            if (!IsBicepFile(inputPath))
            {
                logger.LogError(CliResources.UnrecognizedFileExtensionMessage, inputPath);
                return 1;
            }

            var compilation = await compilationService.CompileAsync(inputPath, args.NoRestore);

            if (diagnosticLogger.ErrorCount < 1)
            {
                if (args.OutputToStdOut)
                {
                    writer.ToStdout(compilation);
                }
                else
                {
                    static string DefaultOutputPath(string path) => PathHelper.GetDefaultBuildOutputPath(path);

                    var outputPath = PathHelper.ResolveDefaultOutputPath(inputPath, args.OutputDir, args.OutputFile, DefaultOutputPath);

                    writer.ToFile(compilation, outputPath);
                }
            }
            if (diagnosticLogger.ErrorCount > 0) {
                return 1;
            }
            return 0;
        }

        private static string? TryGetUsingPath(UsingDeclarationSyntax? usingDeclarationSyntax, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            var pathSyntax = usingDeclarationSyntax?.TryGetPath();
            if (pathSyntax == null)
            {
                failureBuilder = null;
                return null;
            }

            var pathValue = pathSyntax.TryGetLiteralValue();
            if (pathValue == null)
            {
                failureBuilder = x => x.FilePathInterpolationUnsupported();
                return null;
            }

            failureBuilder = null;
            return pathValue;
        }
        private bool IsBicepFile(string inputPath) => PathHelper.HasBicepExtension(PathHelper.FilePathToFileUrl(inputPath));
    }
}