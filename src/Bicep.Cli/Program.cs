using System;
using System.IO;
using Bicep.Cli.CommandLine;
using Bicep.Cli.Logging;
using Bicep.Cli.Utils;
using Bicep.Core.Emit;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Microsoft.Extensions.Logging;

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

                Arguments? arguments = ArgumentParser.Parse(args);
                if (arguments == null)
                {
                    ArgumentParser.PrintUsage();
                    return 1;
                }

                switch (arguments)
                {
                    case BuildArguments buildArguments:
                        Build(logger, buildArguments);
                        break;
                }

                // return non-zero exit code on errors
                return logger.HasLoggedErrors ? 1 : 0;
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
            foreach (string file in arguments.Files)
            {
                string bicepPath = PathHelper.ResolvePath(file);

                if (arguments.OutputToStdOut) {
                    BuildSingleFile(logger, bicepPath);
                    continue;
                }

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

            foreach (Error diagnostic in result.Diagnostics)
            {
                logger.LogDiagnostic(bicepPath, diagnostic, lineStarts);
            }
        }

        private static void BuildSingleFile(IDiagnosticLogger logger, string bicepPath)
        {
            string text = File.ReadAllText(bicepPath);
            var lineStarts = TextCoordinateConverter.GetLineStarts(text);

            var compilation = new Compilation(SyntaxFactory.CreateFromText(text));

            var emitter = new TemplateEmitter(compilation.GetSemanticModel());

            var result = emitter.Emit(Console.Out);

            foreach (Error diagnostic in result.Diagnostics)
            {
                logger.LogDiagnostic(bicepPath, diagnostic, lineStarts);
            }
        }
    }
}
