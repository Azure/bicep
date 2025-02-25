// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands
{
    public class GenerateParametersFileCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly DiagnosticLogger diagnosticLogger;
        private readonly BicepCompiler compiler;
        private readonly PlaceholderParametersWriter writer;

        public GenerateParametersFileCommand(
            ILogger logger,
            DiagnosticLogger diagnosticLogger,
            BicepCompiler compiler,
            PlaceholderParametersWriter writer)
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.compiler = compiler;
            this.writer = writer;
        }

        public async Task<int> RunAsync(GenerateParametersFileArguments args)
        {
            var inputUri = ArgumentHelper.GetFileUri(args.InputFile);
            ArgumentHelper.ValidateBicepFile(inputUri);

            var compilation = await compiler.CreateCompilation(inputUri, forceRestore: args.NoRestore);
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
                    var outputPath = string.Empty;
                    if (!string.IsNullOrWhiteSpace(args.OutputDir))
                    {
                        outputPath = args.OutputDir;
                    }
                    else if (!string.IsNullOrWhiteSpace(args.OutputFile))
                    {
                        outputPath = args.OutputFile;
                    }
                    else
                    {
                        outputPath = inputUri.LocalPath;
                    }

                    outputPath = PathHelper.ResolveParametersFileOutputPath(outputPath, args.OutputFormat);

                    writer.ToFile(compilation, outputPath, args.OutputFormat, args.IncludeParams);
                }
            }

            // return non-zero exit code on errors
            return summary.HasErrors ? 1 : 0;
        }
    }
}
