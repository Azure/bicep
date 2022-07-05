// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.FileSystem;
using Microsoft.Extensions.Logging;
using System.Linq;

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
            // Get the path to the linked Bicep file

            var inputPath = PathHelper.ResolvePath(args.InputFile);
            var inputUri = PathHelper.FilePathToFileUrl(inputPath);

            if (PathHelper.HasExtension(inputUri, LanguageConstants.ParamsFileExtension))
            {
                // 1. Parse the params file
                var parser = new Parser(inputPath);
                var program = parser.Program();

                var usingDeclarations = program.Children.OfType<UsingDeclarationSyntax>();
                
                // 2. Check if there is only one using statement.
                if (!(usingDeclarations.Count() == 1)) {
                    // emit an error
                    // TODO: create a new error log type?
                    logger.LogError(CliResources.ResourceTypesDisclaimerMessage, inputPath);
                }
                // 3. Extract the path to the Bicep file from the using statement

                var paramPath = program.Children[0];//.Path.SegmentValues[0];
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

            // return non-zero exit code on errors
            if (diagnosticLogger.ErrorCount > 0) {
                return 1;
            }

            // 4. Invoke the binder to collect all parameter symbols (need a new version of Binder, NameBindingVisitor, need to update CyclicCheckVisitor).
            // 5. Assign types to the parameter symbols (need a new TypeManager and TypeAssignmentVisitor. Refer to VisitVariableDeclarationSyntax.)
            // 6. Create a new FileSymbol, ParameterAssignmentSymbol. Create a new semantic model to expose FileSymbol as Root.
            // 7. Do validations
            // 7.1. Query the Bicep compilation to get all required parameters, and check if they are all present in the params file
            // 7.2. For each parameter assignment, check if it is declared in the linked Bicep file
            // 7.3. For each parameter assignment, check if its type matches the type of the declared parameter in the linked Bicep file.

            // TODO: make sure GetEntrypointSemanticModel() is being called on param version
            // still need this?
            //var entryPointSemanticModel = compilation.GetEntrypointSemanticModel();

            return 0;
        }
        private bool IsBicepFile(string inputPath) => PathHelper.HasBicepExtension(PathHelper.FilePathToFileUrl(inputPath));
    }
}