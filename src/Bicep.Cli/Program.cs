using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bicep.Cli.CommandLine;
using Bicep.Cli.Logging;
using Bicep.Cli.Utils;
using Bicep.Core.Emit;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;
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
        public static int Main(string[] args)
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
                            ArgumentParser.PrintVersion();
                            break;
                        case HelpArguments _: // --help
                            ArgumentParser.PrintUsage();
                            break;
                        case UnrecognizedArguments unrecognizedArguments: // everything else
                            var exeName = ArgumentParser.GetExeName();
                            Console.Error.WriteLine($"Unrecognized arguments '{unrecognizedArguments.SuppliedArguments}' specified. Use '{exeName} --help' to view available options.");
                            return 1;
                    }

                    // return non-zero exit code on errors
                    return logger.HasLoggedErrors ? 1 : 0;
                }
                catch (CommandLineException cliException)
                {
                    Console.Error.WriteLine(cliException.Message);
                    return 1;
                }
            }
        }

        private static ILoggerFactory CreateLoggerFactory()
        {
            // apparently logging requires a factory factory 🤦‍♀️
            return LoggerFactory.Create(builder =>
            {
                builder.AddProvider(new BicepLoggerProvider(new BicepLoggerOptions(true, ConsoleColor.Red, ConsoleColor.DarkYellow)));
            });
        }

        private static void Build(IDiagnosticLogger logger, BuildArguments arguments)
        {
            var bicepPaths = arguments.Files.Select(file => PathHelper.ResolvePath(file)).ToArray();
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

            foreach (ErrorDiagnostic diagnostic in result.Diagnostics)
            {
                logger.LogDiagnostic(bicepPath, diagnostic, lineStarts);
            }
        }

        private static void BuildManyFilesToStdOut(IDiagnosticLogger logger, string[] bicepPaths)
        {
            using var writer = new JsonTextWriter(Console.Out)
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

                foreach (ErrorDiagnostic diagnostic in result.Diagnostics)
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
