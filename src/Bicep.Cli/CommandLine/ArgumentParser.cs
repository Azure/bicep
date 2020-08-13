using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Bicep.Cli.CommandLine.Arguments;

namespace Bicep.Cli.CommandLine
{
    public static class ArgumentParser
    {
        public static ArgumentsBase Parse(string[] args)
        {
            if (args == null || args.Any() == false)
            {
                return new UnrecognizedArguments("");
            }

            // parse verb
            switch (args[0].ToLowerInvariant())
            {
                case CliConstants.CommandBuild:
                    return ParseBuild(args[1..]);
                case CliConstants.ArgumentHelp:
                    return new HelpArguments();
                case CliConstants.ArgumentVersion:
                    return new VersionArguments();
                default:
                    return new UnrecognizedArguments(string.Join(' ', args));
            }
        }

        public static string GetExeName()
            => Path.GetFileNameWithoutExtension(Path.GetFileName(Assembly.GetExecutingAssembly().Location));

        private static string GetVersion()
            => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "<unknown>";

        public static void PrintVersion()
        {
            var exeVersion = GetVersion();
            var output =
$@"Bicep CLI version {exeVersion}
"; //newline is intentional

            Console.Out.Write(output);
            Console.Out.Flush();
        }

        public static void PrintUsage()
        {
            var exeName = GetExeName();
            var exeVersion = GetVersion();
            var output = 
$@"Bicep CLI version {exeVersion}

Usage:
  {exeName} build [options] [<files>...]
    Builds one or more .bicep files

    Arguments:
      <files>     The list of one or more .bicep files to build

    Options:
      --stdout    Prints all output to stdout instead of corresponding files

  {exeName} [options]
    Options:
      --version    Shows bicep version information
      --help       Shows this usage information
"; // this newline is intentional

            Console.Out.Write(output);
            Console.Out.Flush();
        }

        private static BuildArguments ParseBuild(string[] files)
        {
            return new BuildArguments(files);
        }
    }
}
