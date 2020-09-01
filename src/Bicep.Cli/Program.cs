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
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;

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
                IDiagnosticLogger logger = new BicepDiagnosticLogger(loggerFactory.CreateLogger("bicep"));
                try
                {
                    switch (ArgumentParser.TryParse(args))
                    {
                        case BuildArguments buildArguments: // build
                            return Build(logger, buildArguments);
                        case DecompileArguments decompileArguments:
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

        private int Build(IDiagnosticLogger logger, BuildArguments arguments)
        {
            var bicepPaths = arguments.Files.Select(f => PathHelper.ResolvePath(f)).ToArray();
            if (arguments.OutputToStdOut)
            {
                BuildManyFilesToStdOut(logger, bicepPaths);
            }
            else
            {
                foreach (string bicepPath in bicepPaths)
                {
                    string outputPath = PathHelper.GetDefaultOutputPath(bicepPath);
                    BuildSingleFile(logger, bicepPath, outputPath);
                }
            }

            // return non-zero exit code on errors
            return logger.HasLoggedErrors ? 1 : 0;
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

        public int Decompile(IDiagnosticLogger logger, DecompileArguments arguments)
        {
            var hadErrors = false;
            foreach (var filePath in arguments.Files)
            {
                hadErrors |= !DecompileSingleFile(logger, filePath);
            }

            return hadErrors ? 1 : 0;
        }

        private bool DecompileSingleFile(IDiagnosticLogger logger, string filePath)
        {
            try
            {
                var jsonInput = File.ReadAllText(filePath);
                var outputFile = Path.ChangeExtension(filePath, "bicep");

                var program = TemplateConverter.DecompileTemplate(jsonInput);
                var bicepOutput = PrettyPrinter.PrintProgram(program, new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, 2, false));
                File.WriteAllText(outputFile, bicepOutput);

                var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), new Workspace(), PathHelper.FilePathToFileUrl(outputFile));
                var compilation = new Compilation(resourceTypeProvider, syntaxTreeGrouping);
                var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics().ToArray();

                return LogDiagnosticsAndCheckSuccess(logger, compilation);
            }
            catch (Exception exception)
            {
                this.errorWriter.WriteLine($"{filePath}: {exception.Message}");
                return false;
            }
        }
    }
}

