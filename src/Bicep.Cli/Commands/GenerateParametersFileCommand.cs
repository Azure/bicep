// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands
{
    public class GenerateParametersFileCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly IFileExplorer fileExplorer;
        private readonly DiagnosticLogger diagnosticLogger;
        private readonly BicepCompiler compiler;
        private readonly PlaceholderParametersWriter writer;
        private readonly InputOutputArgumentsResolver inputOutputArgumentsResolver;

        public GenerateParametersFileCommand(
            ILogger logger,
            IFileExplorer fileExplorer,
            DiagnosticLogger diagnosticLogger,
            BicepCompiler compiler,
            PlaceholderParametersWriter writer,
            InputOutputArgumentsResolver inputOutputArgumentsResolver)
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.compiler = compiler;
            this.writer = writer;
            this.inputOutputArgumentsResolver = inputOutputArgumentsResolver;
            this.fileExplorer = fileExplorer;
        }

        public async Task<int> RunAsync(GenerateParametersFileArguments args)
        {
            var (inputUri, outputUri) = this.inputOutputArgumentsResolver.ResolveInputOutputArguments(args);
            ArgumentHelper.ValidateBicepFile(inputUri);

            var compilation = await compiler.CreateCompilation(inputUri.ToUri(), forceRestore: args.NoRestore);
            CommandHelper.LogExperimentalWarning(logger, compilation);

            var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, compilation);

            if (!summary.HasErrors)
            {
                if (args.OutputToStdOut)
                {
                    writer.ToStdout(compilation, args.OutputFormat, args.IncludeParams);
                }
                else
                {
                    var outputFile = this.fileExplorer.GetFile(outputUri);
                    writer.ToFile(compilation, outputFile, args.OutputFormat, args.IncludeParams);
                }
            }

            // return non-zero exit code on errors
            return summary.HasErrors ? 1 : 0;
        }
    }
}
