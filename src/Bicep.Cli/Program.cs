// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using System.Linq;
using Bicep.Cli.CommandLine;
using Bicep.Cli.Logging;
using Bicep.Cli.Utils;
using Bicep.Core.Emit;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Bicep.Cli.CommandLine.Arguments;

namespace Bicep.Cli
{
    public class Program
    {
        private readonly TextWriter outputWriter;
        private readonly TextWriter errorWriter;

        public Program(TextWriter outputWriter, TextWriter errorWriter)
        {
            this.outputWriter = outputWriter;
            this.errorWriter = errorWriter;
        }

        public static int Main(string[] args)
        {
            var program = new Program(Console.Out, Console.Error);
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
                    switch (ArgumentParser.Parse(args))
                    {
                        case BuildArguments buildArguments: // build
                            Build(logger, buildArguments);
                            break;
                        case VersionArguments _: // --version
                            ArgumentParser.PrintVersion(this.outputWriter);
                            break;
                        case HelpArguments _: // --help
                            ArgumentParser.PrintUsage(this.outputWriter);
                            break;
                        case UnrecognizedArguments unrecognizedArguments: // everything else
                            var exeName = ArgumentParser.GetExeName();
                            this.errorWriter.WriteLine($"Unrecognized arguments '{unrecognizedArguments.SuppliedArguments}' specified. Use '{exeName} --help' to view available options.");
                            return 1;
                    }

                    // return non-zero exit code on errors
                    return logger.HasLoggedErrors ? 1 : 0;
                }
                catch (CommandLineException cliException)
                {
                    this.errorWriter.WriteLine(cliException.Message);
                    return 1;
                }
            }
        }

        private ILoggerFactory CreateLoggerFactory()
        {
            // apparently logging requires a factory factory ???
            return LoggerFactory.Create(builder =>
            {
                builder.AddProvider(new BicepLoggerProvider(new BicepLoggerOptions(true, ConsoleColor.Red, ConsoleColor.DarkYellow, this.errorWriter)));
            });
        }

        private void Build(IDiagnosticLogger logger, BuildArguments arguments)
        {
            var bicepPaths = arguments.Files.Select(PathHelper.ResolvePath).ToArray();
            if (arguments.OutputToStdOut)
            {
                BuildManyFilesToStdOut(logger, bicepPaths);
                return;
            }

            foreach (string bicepPath in bicepPaths)
            {
                string outputPath = PathHelper.GetDefaultOutputPath(bicepPath);
                BuildSingleFile(logger, bicepPath, outputPath);
            }
        }

        private static void BuildSingleFile(IDiagnosticLogger logger, string bicepPath, string outputPath)
        {
            string text = File.ReadAllText(bicepPath);
            var lineStarts = TextCoordinateConverter.GetLineStarts(text);

            var compilation = new Compilation(SyntaxFactory.CreateFromText(text));

            var emitter = new TemplateEmitter(compilation.GetSemanticModel());

            var result = emitter.Emit(outputPath);

            foreach (var diagnostic in result.Diagnostics)
            {
                logger.LogDiagnostic(bicepPath, diagnostic, lineStarts);
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
                string text = File.ReadAllText(bicepPath);
                var lineStarts = TextCoordinateConverter.GetLineStarts(text);

                var compilation = new Compilation(SyntaxFactory.CreateFromText(text));

                var emitter = new TemplateEmitter(compilation.GetSemanticModel());

                var result = emitter.Emit(writer);

                foreach (var diagnostic in result.Diagnostics)
                {
                    logger.LogDiagnostic(bicepPath, diagnostic, lineStarts);
                }
            }
            if (bicepPaths.Length > 1) {
                writer.WriteEndArray();
            }
        }
    }
}

