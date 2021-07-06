// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using System.Runtime;
using Bicep.Cli.CommandLine;
using Bicep.Cli.CommandLine.Arguments;
using Bicep.Cli.Logging;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.Utils;
using Bicep.Core.Workspaces;
using Bicep.Decompiler;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bicep.Cli
{
    public class Program
    {
        private readonly TextWriter outputWriter;
        private readonly TextWriter errorWriter;
        private readonly IResourceTypeProvider resourceTypeProvider;

        private readonly string assemblyFileVersion;

        public Program(IResourceTypeProvider resourceTypeProvider, TextWriter outputWriter, TextWriter errorWriter, string assemblyFileVersion)
        {
            this.resourceTypeProvider = resourceTypeProvider;
            this.outputWriter = outputWriter;
            this.errorWriter = errorWriter;
            this.assemblyFileVersion = assemblyFileVersion;
        }

        public static int Main(string[] args)
        {
            string profilePath = MulticoreJIT.GetMulticoreJITPath();
            ProfileOptimization.SetProfileRoot(profilePath);
            ProfileOptimization.StartProfile("bicep.profile");
            Console.OutputEncoding = TemplateEmitter.UTF8EncodingWithoutBom;

            BicepDeploymentsInterop.Initialize();
            var program = new Program(AzResourceTypeProvider.CreateWithAzTypes(), Console.Out, Console.Error, ThisAssembly.AssemblyFileVersion);
            return program.Run(args);
        }

        public int Run(string[] args)
        {
            // ReSharper disable once ConvertToUsingDeclaration
            using (var loggerFactory = CreateLoggerFactory())
            {
                // the only value in using the dotnet logging framework is that we can easily implement filters
                // and logging to multiple targets in the future (stdout AND a log file, for example)
                // it does not help us with formatting of the messages however, so we will have to workaround that
                var logger = loggerFactory.CreateLogger("bicep");
                try
                {
                    switch (ArgumentParser.TryParse(args))
                    {
                        case BuildOrDecompileArguments buildArguments when buildArguments.CommandName == CliConstants.CommandBuild: // build
                            return Build(logger, buildArguments);
                        case BuildOrDecompileArguments decompileArguments when decompileArguments.CommandName == CliConstants.CommandDecompile: // decompile
                            return Decompile(logger, decompileArguments);
                        case VersionArguments _: // --version
                            ArgumentParser.PrintVersion(this.outputWriter);
                            return 0;
                        case HelpArguments _: // --help
                            ArgumentParser.PrintUsage(this.outputWriter);
                            return 0;
                        default:
                            var exeName = ArgumentParser.GetExeName();
                            var arguments = string.Join(' ', args);
                            this.errorWriter.WriteLine(string.Format(CliResources.UnrecognizedArgumentsFormat, arguments, exeName));
                            return 1;
                    }
                }
                catch (CommandLineException exception)
                {
                    this.errorWriter.WriteLine(exception.Message);
                    return 1;
                }
                catch (BicepException exception)
                {
                    this.errorWriter.WriteLine(exception.Message);
                    return 1;
                }
                catch (ErrorDiagnosticException exception)
                {
                    this.errorWriter.WriteLine(exception.Message);
                    return 1;
                }
            }
        }

        private ILoggerFactory CreateLoggerFactory()
        {
            // apparently logging requires a factory factory 🤦‍
            return LoggerFactory.Create(builder =>
            {
                builder.AddProvider(new BicepLoggerProvider(new BicepLoggerOptions(true, ConsoleColor.Red, ConsoleColor.DarkYellow, this.errorWriter)));
            });
        }

        private int Build(ILogger logger, BuildOrDecompileArguments arguments)
        {
            var diagnosticLogger = new BicepDiagnosticLogger(logger);
            var bicepPath = PathHelper.ResolvePath(arguments.InputFile);

            if (arguments.OutputToStdOut)
            {
                BuildToStdout(diagnosticLogger, bicepPath);
            }
            else if (arguments.OutputDir is not null)
            {
                var outputDir = PathHelper.ResolvePath(arguments.OutputDir);
                if (!Directory.Exists(outputDir))
                {
                    throw new CommandLineException(string.Format(CliResources.DirectoryDoesNotExistFormat, outputDir));
                }

                var outputPath = Path.Combine(outputDir, Path.GetFileName(bicepPath));

                BuildToFile(diagnosticLogger, bicepPath, PathHelper.GetDefaultBuildOutputPath(outputPath));
            }
            else if (arguments.OutputFile is not null)
            {
                BuildToFile(diagnosticLogger, bicepPath, arguments.OutputFile);
            }
            else
            {
                BuildToFile(diagnosticLogger, bicepPath, PathHelper.GetDefaultBuildOutputPath(bicepPath));
            }

            if(!arguments.NoSummary)
            {
                diagnosticLogger.LogSummary();
            }
            
            // return non-zero exit code on errors
            return diagnosticLogger.ErrorCount > 0 ? 1 : 0;
        }

        private static bool LogDiagnosticsAndCheckSuccess(IDiagnosticLogger logger, Compilation compilation)
        {
            var success = true;
            foreach (var (syntaxTree, diagnostics) in compilation.GetAllDiagnosticsBySyntaxTree())
            {
                foreach (var diagnostic in diagnostics)
                {
                    logger.LogDiagnostic(syntaxTree.FileUri, diagnostic, syntaxTree.LineStarts);
                    success &= diagnostic.Level != DiagnosticLevel.Error;
                }
            }

            return success;
        }

        private void BuildToFile(IDiagnosticLogger logger, string bicepPath, string outputPath)
        {
            var fileResolver = new FileResolver();
            var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(fileResolver, new Workspace(), PathHelper.FilePathToFileUrl(bicepPath));
            var compilation = new Compilation(resourceTypeProvider, syntaxTreeGrouping);

            var success = LogDiagnosticsAndCheckSuccess(logger, compilation);
            if (success)
            {
                var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), this.assemblyFileVersion);

                using var outputStream = CreateFileStream(outputPath);
                emitter.Emit(outputStream);
            }
        }

        private void BuildToStdout(IDiagnosticLogger logger, string bicepPath)
        {
            using var writer = new JsonTextWriter(this.outputWriter)
            {
                Formatting = Formatting.Indented
            };

            var fileResolver = new FileResolver();
            var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(fileResolver, new Workspace(), PathHelper.FilePathToFileUrl(bicepPath));
            var compilation = new Compilation(resourceTypeProvider, syntaxTreeGrouping);

            var success = LogDiagnosticsAndCheckSuccess(logger, compilation);
            if (success)
            {
                var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), this.assemblyFileVersion);

                emitter.Emit(writer);
            }
        }

        private int DecompileToFile(IDiagnosticLogger logger, string jsonPath, string outputPath)
        {
            try
            {
                var jsonUri = PathHelper.FilePathToFileUrl(jsonPath);
                var bicepUri = PathHelper.FilePathToFileUrl(outputPath);
                var fileResolver = new FileResolver();

                var (entrypointUri, filesToSave) = TemplateDecompiler.DecompileFileWithModules(resourceTypeProvider, fileResolver, jsonUri, bicepUri);
                var workspace = new Workspace();
                foreach (var (fileUri, bicepOutput) in filesToSave)
                {
                    File.WriteAllText(fileUri.LocalPath, bicepOutput);
                    workspace.UpsertSyntaxTrees(SyntaxTreeFactory.CreateSyntaxTree(fileUri, bicepOutput).AsEnumerable());
                }

                var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(fileResolver, workspace, entrypointUri);
                var compilation = new Compilation(resourceTypeProvider, syntaxTreeGrouping);

                return LogDiagnosticsAndCheckSuccess(logger, compilation) ? 0 : 1;
            }
            catch (Exception exception)
            {
                this.errorWriter.WriteLine(string.Format(CliResources.DecompilationFailedFormat, jsonPath, exception.Message));
                return 1;
            }
        }

        private int DecompileToStdout(IDiagnosticLogger logger, string jsonPath)
        {
            try
            {
                var jsonUri = PathHelper.FilePathToFileUrl(jsonPath);
                var bicepUri = PathHelper.ChangeToBicepExtension(jsonUri);
                var fileResolver = new FileResolver();

                var (entrypointUri, filesToSave) = TemplateDecompiler.DecompileFileWithModules(resourceTypeProvider, fileResolver, jsonUri, bicepUri);
                var workspace = new Workspace();
                foreach (var (fileUri, bicepOutput) in filesToSave)
                {
                    this.outputWriter.Write(bicepOutput);
                    workspace.UpsertSyntaxTrees(SyntaxTreeFactory.CreateSyntaxTree(fileUri, bicepOutput).AsEnumerable());
                }

                var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(fileResolver, workspace, entrypointUri);
                var compilation = new Compilation(resourceTypeProvider, syntaxTreeGrouping);

                return LogDiagnosticsAndCheckSuccess(logger, compilation) ? 0 : 1;
            }
            catch (Exception exception)
            {
                this.errorWriter.WriteLine(string.Format(CliResources.DecompilationFailedFormat, jsonPath, exception.Message));
                return 1;
            }
        }

        private static FileStream CreateFileStream(string path)
        {
            try
            {
                return new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            }
            catch (Exception exception)
            {
                throw new BicepException(exception.Message, exception);
            }
        }

        public int Decompile(ILogger logger, BuildOrDecompileArguments arguments)
        {
            logger.LogWarning(CliResources.DecompilerDisclaimerMessage);
            var diagnosticLogger = new BicepDiagnosticLogger(logger);
            var jsonPath = PathHelper.ResolvePath(arguments.InputFile);

            if (arguments.OutputToStdOut)
            {
                return DecompileToStdout(diagnosticLogger, jsonPath);
            }
            else if (arguments.OutputDir is not null)
            {
                var outputDir = PathHelper.ResolvePath(arguments.OutputDir);
                if (!Directory.Exists(outputDir))
                {
                    throw new CommandLineException(string.Format(CliResources.DirectoryDoesNotExistFormat, outputDir));
                }

                var outputPath = Path.Combine(outputDir, Path.GetFileName(jsonPath));

                return DecompileToFile(diagnosticLogger, jsonPath, PathHelper.GetDefaultDecompileOutputPath(outputPath));
            }
            else if (arguments.OutputFile is not null)
            {
                return DecompileToFile(diagnosticLogger, jsonPath, arguments.OutputFile);
            }
            else
            {
                return DecompileToFile(diagnosticLogger, jsonPath, PathHelper.GetDefaultDecompileOutputPath(jsonPath));
            }
        }
    }
}

