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
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands
{
    public class DecompileCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly DiagnosticLogger diagnosticLogger;
        private readonly IOContext io;
        private readonly IFileResolver fileResolver;
        private readonly BicepDecompiler decompiler;
        private readonly BicepCompiler compiler;
        private readonly OutputWriter writer;
        private readonly ISourceFileFactory sourceFileFactory;

        public DecompileCommand(
            ILogger logger,
            DiagnosticLogger diagnosticLogger,
            IOContext io,
            IFileResolver fileResolver,
            BicepDecompiler decompiler,
            BicepCompiler compiler,
            OutputWriter writer,
            ISourceFileFactory sourceFileFactory)
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.io = io;
            this.fileResolver = fileResolver;
            this.decompiler = decompiler;
            this.compiler = compiler;
            this.writer = writer;
            this.sourceFileFactory = sourceFileFactory;
        }

        public async Task<int> RunAsync(DecompileArguments args)
        {
            logger.LogWarning(BicepDecompiler.DecompilerDisclaimerMessage);

            var inputUri = PathHelper.FilePathToFileUrl(PathHelper.ResolvePath(args.InputFile));
            var outputPath = PathHelper.ResolveOutputPath(inputUri.LocalPath, args.OutputDir, args.OutputFile, PathHelper.GetBicepOutputPath);
            var outputUri = PathHelper.FilePathToFileUrl(outputPath);

            try
            {
                if (!fileResolver.TryRead(inputUri).IsSuccess(out var jsonContents))
                {
                    throw new InvalidOperationException($"Failed to read {inputUri}");
                }

                var decompilation = await decompiler.Decompile(outputUri, jsonContents);

                var workspace = new Workspace();
                foreach (var (fileUri, bicepOutput) in decompilation.FilesToSave)
                {
                    workspace.UpsertSourceFile(this.sourceFileFactory.CreateBicepFile(fileUri, bicepOutput));
                }

                // to verify success we recompile and check for syntax errors.
                var compilation = await compiler.CreateCompilation(decompilation.EntrypointUri, skipRestore: true, workspace: workspace);
                var summary = diagnosticLogger.LogDiagnostics(DiagnosticOptions.Default, compilation);

                if (args.OutputToStdOut)
                {
                    writer.DecompileResultToStdout(decompilation);
                }
                else
                {
                    writer.DecompileResultToFile(decompilation);
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
