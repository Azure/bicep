using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bicep.Cli.CommandLine
{
    public static class ArgumentParser
    {
        public static Arguments? Parse(string[] args)
        {
            if (args == null || args.Any() == false)
            {
                return null;
            }

            // parse verb
            switch (ParseCommand(args[0]))
            {
                case Command.Build:
                    return ParseBuild(args[1..]);

                default:
                    throw new NotImplementedException($"Unexpected verb '{args[0]}'");
            }
        }

        public static void PrintUsage()
        {
            string exeName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);

            Console.WriteLine($"Usage: {exeName} build <file 1> <file 2> ... <file n>");
        }

        private static Command ParseCommand(string arg)
        {
            if (Enum.TryParse<Command>(arg, true, out var verb))
            {
                return verb;
            }

            throw new CommandLineException($"Unexpected command '{arg}' was specified. Valid command include: {string.Join(", ", Enum.GetNames(typeof(Command)))}");
        }

        private static BuildArguments ParseBuild(string[] files)
        {
            if (files.Any() == false)
            {
                throw new CommandLineException($"At least one file must be specified to the {Command.Build} command.");
            }

            return new BuildArguments(files);
        }
    }
}
