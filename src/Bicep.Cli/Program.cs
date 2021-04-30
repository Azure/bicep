// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using System.Linq;
using Bicep.Cli.CommandLine;
using Bicep.Cli.CommandLine.Arguments;
using Bicep.Cli.Logging;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
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
                        case (CliConstants.CommandBuild, BuildOrDecompileArguments buildArguments): // build
                            return Build(logger, buildArguments);
                        case (CliConstants.CommandDecompile, BuildOrDecompileArguments decompileArguments): // decompile
                            return Decompile(logger, decompileArguments);
                        case (_, VersionArguments _): // --version
                            ArgumentParser.PrintVersion(this.outputWriter);
                            return 0;
                        case (_, HelpArguments _): // --help
                            ArgumentParser.PrintUsage(this.outputWriter);
                            return 0;
                        default:
                            var exeName = ArgumentParser.GetExeName();
                            var arguments = string.Join(' ', args);
                            this.errorWriter.WriteLine($"Unrecognized arguments '{arguments}' specified. Use '{exeName} --help' to view available options.");
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
                    throw new CommandLineException($"The specified output directory \"{outputDir}\" does not exist.");
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

            // return non-zero exit code on errors
            return diagnosticLogger.HasLoggedErrors ? 1 : 0;
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
            var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), new Workspace(), PathHelper.FilePathToFileUrl(bicepPath));
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

            var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), new Workspace(), PathHelper.FilePathToFileUrl(bicepPath));
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
                var (_, filesToSave) = TemplateDecompiler.DecompileFileWithModules(resourceTypeProvider, new FileResolver(), PathHelper.FilePathToFileUrl(jsonPath));
                foreach (var (_, bicepOutput) in filesToSave)
                {
                    File.WriteAllText(outputPath, bicepOutput);
                }

                var outputPathToCheck = Path.GetFullPath(outputPath);
                var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), new Workspace(), PathHelper.FilePathToFileUrl(outputPathToCheck));
                var compilation = new Compilation(resourceTypeProvider, syntaxTreeGrouping);

                return LogDiagnosticsAndCheckSuccess(logger, compilation) ? 0 : 1;
            }
            catch (Exception exception)
            {
                this.errorWriter.WriteLine($"{jsonPath}: Decompilation failed with fatal error \"{exception.Message}\"");
                return 1;
            }
        }

        private int DecompileToStdout(IDiagnosticLogger logger, string jsonPath)
        {
            var tempOutputPath = Path.ChangeExtension(Path.GetTempFileName(), "bicep");
            try
            {
                var (_, filesToSave) = TemplateDecompiler.DecompileFileWithModules(resourceTypeProvider, new FileResolver(), PathHelper.FilePathToFileUrl(jsonPath));
                foreach (var (_, bicepOutput) in filesToSave)
                {
                    this.outputWriter.Write(bicepOutput);
                    File.WriteAllText(tempOutputPath, bicepOutput);
                }

                var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), new Workspace(), PathHelper.FilePathToFileUrl(tempOutputPath));
                var compilation = new Compilation(resourceTypeProvider, syntaxTreeGrouping);

                return LogDiagnosticsAndCheckSuccess(logger, compilation) ? 0 : 1;
            }
            catch (Exception exception)
            {
                this.errorWriter.WriteLine($"{jsonPath}: Decompilation failed with fatal error \"{exception.Message}\"");
                return 1;
            } finally
            {
                if (File.Exists(tempOutputPath))
                {
                    File.Delete(tempOutputPath);
                }
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
            logger.LogWarning(
                "WARNING: Decompilation is a best-effort process, as there is no guaranteed mapping from ARM JSON to Bicep.\n" +
                "You may need to fix warnings and errors in the generated bicep file(s), or decompilation may fail entirely if an accurate conversion is not possible.\n" +
                "If you would like to report any issues or inaccurate conversions, please see https://github.com/Azure/bicep/issues.");

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
                    throw new CommandLineException($"The specified output directory \"{outputDir}\" does not exist.");
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

