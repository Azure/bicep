using System;
using System.IO;
using Bicep.Cli.CommandLine;
using Bicep.Cli.Utils;
using Bicep.Core.Emit;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.Core.Utils;

namespace Bicep.Cli
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Arguments? arguments = ArgumentParser.Parse(args);
            if (arguments == null)
            {
                ArgumentParser.PrintUsage();
                return 0;
            }

            switch (arguments)
            {
                case BuildArguments buildArguments:
                    Build(buildArguments);
                    break;
            }

            // TODO: If we had logging and errors were logged, exit code should be 1
            return 0;
        }

        private static void Build(BuildArguments arguments)
        {
            foreach (string file in arguments.Files)
            {
                string bicepPath = PathHelper.ResolvePath(file);
                string outputPath = PathHelper.GetDefaultOutputPath(bicepPath);

                BuildSingleFile(bicepPath, outputPath);
            }
        }

        private static void BuildSingleFile(string bicepPath, string outputPath)
        {
            string text = File.ReadAllText(bicepPath);

            var compilation = new Compilation(SyntaxFactory.CreateFromText(text));

            var emitter = new TemplateEmitter(compilation.GetSemanticModel());

            var result = emitter.Emit(outputPath);

            foreach (Error diagnostic in result.Diagnostics)
            {
                Console.WriteLine($"{diagnostic.Message} {diagnostic.Span}");
            }
        }
    }
}
