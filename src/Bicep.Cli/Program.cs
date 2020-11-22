// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using System.Linq;
using System.Text;
using Bicep.Cli.CommandLine;
using Bicep.Cli.Logging;
using Bicep.Core.Emit;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Bicep.Cli.CommandLine.Arguments;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.TypeSystem;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Workspaces;
using Bicep.Decompiler;

namespace Bicep.Cli
{
    public class Program
    {
        private readonly TextWriter outputWriter;
        private readonly TextWriter errorWriter;
        private readonly IResourceTypeProvider resourceTypeProvider;

        public Program(IResourceTypeProvider resourceTypeProvider, TextWriter outputWriter, TextWriter errorWriter)
        {
            this.resourceTypeProvider = resourceTypeProvider;
            this.outputWriter = outputWriter;
            this.errorWriter = errorWriter;
        }

        public static int Main(string[] args)
        {
            BicepDeploymentsInterop.Initialize();
            var program = new Program(new AzResourceTypeProvider(), Console.Out, Console.Error);
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
                        case BuildArguments buildArguments: // build
                            return Build(logger, buildArguments);
                        case DecompileArguments decompileArguments:
                            return Decompile(logger, decompileArguments);
                        case CleanArguments cleanArguments:
                            return Clean(logger, cleanArguments);
                        case VersionArguments _: // --version
                            ArgumentParser.PrintVersion(this.outputWriter);
                            return 0;
                        case HelpArguments _: // --help
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

        private int Build(ILogger logger, BuildArguments arguments)
        {
            var diagnosticLogger = new BicepDiagnosticLogger(logger);
            var bicepPaths = arguments.Files.Select(f => PathHelper.ResolvePath(f)).ToArray();
            if (arguments.OutputToStdOut)
            {
                BuildManyFilesToStdOut(diagnosticLogger, bicepPaths);
            }
            else
            {
                foreach (string bicepPath in bicepPaths)
                {
                    string outputPath = PathHelper.GetDefaultOutputPath(bicepPath);
                    BuildSingleFile(diagnosticLogger, bicepPath, outputPath);
                }
            }

            // return non-zero exit code on errors
            return diagnosticLogger.HasLoggedErrors ? 1 : 0;
        }

        private int Clean(ILogger logger, CleanArguments arguments)
        {
            var diagnosticLogger = new BicepDiagnosticLogger(logger);
            var bicepDirectories = arguments.BicepDirectories.Select(f => PathHelper.ResolvePath(f)).ToArray();
            foreach (string bicepDirectory in bicepDirectories)
            {
                var bicepPaths = Directory.GetFiles(bicepDirectory, "*.bicep");
                foreach(string bicepPath in bicepPaths)
                {
                    CleanJsonFiles(diagnosticLogger, bicepPath);
                }
            }
            return diagnosticLogger.HasLoggedErrors ? 1 : 0;
        }

        private bool LogDiagnosticsAndCheckSuccess(IDiagnosticLogger logger, Compilation compilation)
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
        private void CleanJsonFiles(IDiagnosticLogger logger, string bicepPath)
        {
            bicepPath = Path.ChangeExtension(bicepPath, ".json");
            RemoveFile(bicepPath);
        }

        private void BuildSingleFile(IDiagnosticLogger logger, string bicepPath, string outputPath)
        {
            var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), new Workspace(), PathHelper.FilePathToFileUrl(bicepPath));
            var compilation = new Compilation(resourceTypeProvider, syntaxTreeGrouping);

            var success = LogDiagnosticsAndCheckSuccess(logger, compilation);
            if (success)
            {
                var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());

                using var outputStream = CreateFileStream(outputPath);
                emitter.Emit(outputStream);
            }
        }

        private void BuildManyFilesToStdOut(IDiagnosticLogger logger, string[] bicepPaths)
        {
            using var writer = new JsonTextWriter(this.outputWriter)
            {
                Formatting = Formatting.Indented
            };

            if (bicepPaths.Length > 1) {
                writer.WriteStartArray();
            }
            foreach(var bicepPath in bicepPaths)
            {
                var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), new Workspace(), PathHelper.FilePathToFileUrl(bicepPath));
                var compilation = new Compilation(resourceTypeProvider, syntaxTreeGrouping);

                var success = LogDiagnosticsAndCheckSuccess(logger, compilation);
                if (success)
                {
                    var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());

                    emitter.Emit(writer);
                }
            }
            if (bicepPaths.Length > 1) {
                writer.WriteEndArray();
            }
        }

        private static string ReadFile(string path)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch (Exception exception)
            {
                // I/O classes typically throw a large variety of exceptions
                // instead of handling each one separately let's just trust the message we get
                throw new BicepException(exception.Message, exception);
            }
        }
        private static void RemoveFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception exception)
            {
                throw new BicepException(exception.Message, exception);
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

        public int Decompile(ILogger logger, DecompileArguments arguments)
        {
            logger.LogWarning(
                "WARNING: Decompilation is a best-effort process, as there is no guaranteed mapping from ARM JSON to Bicep.\n" +
                "You may need to fix warnings and errors in the generated bicep file(s), or decompilation may fail entirely if an accurate conversion is not possible.\n" +
                "If you would like to report any issues or inaccurate conversions, please see https://github.com/Azure/bicep/issues.");

            var diagnosticLogger = new BicepDiagnosticLogger(logger);
            var hadErrors = false;
            var jsonPaths = arguments.Files.Select(f => PathHelper.ResolvePath(f)).ToArray();
            foreach (var jsonPath in jsonPaths)
            {
                hadErrors |= !DecompileSingleFile(diagnosticLogger, jsonPath);
            }

            return hadErrors ? 1 : 0;
        }

        private bool DecompileSingleFile(IDiagnosticLogger logger, string filePath)
        {
            try
            {
                var (bicepUri, filesToSave) = Decompiler.Decompiler.DecompileFileWithModules(new FileResolver(), PathHelper.FilePathToFileUrl(filePath));
                foreach (var (fileUri, bicepOutput) in filesToSave)
                {
                    File.WriteAllText(fileUri.LocalPath, bicepOutput);
                }

                var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), new Workspace(), bicepUri);
                var compilation = new Compilation(resourceTypeProvider, syntaxTreeGrouping);

                return LogDiagnosticsAndCheckSuccess(logger, compilation);
            }
            catch (Exception exception)
            {
                this.errorWriter.WriteLine($"{filePath}: Decompilation failed with fatal error \"{exception.Message}\"");
                return false;
            }
        }
    }
}

