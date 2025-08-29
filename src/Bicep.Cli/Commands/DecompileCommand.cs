// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.SourceGraph;
using Bicep.Decompiler;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands
{
    public class DecompileCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly DiagnosticLogger diagnosticLogger;
        private readonly IOContext io;
        private readonly BicepDecompiler decompiler;
        private readonly BicepCompiler compiler;
        private readonly OutputWriter writer;
        private readonly ISourceFileFactory sourceFileFactory;
        private readonly InputOutputArgumentsResolver inputOutputArgumentsResolver;
        private readonly IFileExplorer fileExplorer;

        public DecompileCommand(
            ILogger logger,
            DiagnosticLogger diagnosticLogger,
            IOContext io,
            BicepDecompiler decompiler,
            BicepCompiler compiler,
            OutputWriter writer,
            ISourceFileFactory sourceFileFactory,
            InputOutputArgumentsResolver inputOutputArgumentsResolver,
            IFileExplorer fileExplorer)
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.io = io;
            this.decompiler = decompiler;
            this.compiler = compiler;
            this.writer = writer;
            this.sourceFileFactory = sourceFileFactory;
            this.inputOutputArgumentsResolver = inputOutputArgumentsResolver;
            this.fileExplorer = fileExplorer;
        }

        public async Task<int> RunAsync(DecompileArguments args)
        {
            logger.LogWarning(BicepDecompiler.DecompilerDisclaimerMessage);

            var (inputUri, outputUri) = inputOutputArgumentsResolver.ResolveInputOutputArguments(args);

            if (!args.OutputToStdOut && !args.AllowOverwrite && this.fileExplorer.GetFile(outputUri).Exists())
            {
                throw new CommandLineException($"The output file \"{outputUri}\" already exists. Use --force to overwrite the existing file.");
            }

            try
            {
                var jsonContents = await this.fileExplorer.GetFile(inputUri).ReadAllTextAsync();
                var decompilation = await decompiler.Decompile(outputUri.ToUri(), jsonContents);

                // TODO(low-priority): It would be ideal to remove Workspace and use InMemoryFileExplorer instead.
                // This is something that should be done after the core part of file I/O abstraction migration is complete.
                var workspace = new Workspace();
                foreach (var (fileUri, bicepOutput) in decompilation.FilesToSave)
                {
                    workspace.UpsertSourceFile(this.sourceFileFactory.CreateBicepFile(fileUri.ToIOUri(), bicepOutput));
                }

                // to verify success we recompile and check for syntax errors.
                var compilation = await compiler.CreateCompilation(decompilation.EntrypointUri.ToIOUri(), skipRestore: true, workspace: workspace);
                var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, compilation);

                if (args.OutputToStdOut)
                {
                    writer.DecompileResultToStdout(decompilation);
                }
                else
                {
                    await writer.DecompileResultToFileAsync(decompilation);
                }

                // return non-zero exit code on errors
                return summary.HasErrors ? 1 : 0;
            }
            catch (Exception exception)
            {
                await io.Error.WriteLineAsync(string.Format(CliResources.DecompilationFailedFormat, PathHelper.ResolvePath(args.InputFile), exception.Message));
                return 1;
            }
        }
    }
}
