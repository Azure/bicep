// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using System.Linq;
using Bicep.Cli.CommandLine.Arguments;

namespace Bicep.Cli.CommandLine
{
    public static class ArgumentParser
    {
        public static ArgumentsBase? TryParse(string[] args)
        {
            if (args.Length < 1)
            {
                return null;
            }

            // parse verb
            switch (args[0].ToLowerInvariant())
            {
                case CliConstants.CommandBuild:
                    return ParseBuild(args[1..]);
                case CliConstants.CommandDecompile:
                    return ParseDecompile(args[1..]);
                case CliConstants.ArgumentHelp:
                case CliConstants.ArgumentHelpShort:
                    return new HelpArguments();
                case CliConstants.ArgumentVersion:
                case CliConstants.ArgumentVersionShort:
                    return new VersionArguments();
            }
            
            return null;
        }

        public static string GetExeName()
            => ThisAssembly.AssemblyName;

        private static string GetVersionString()
        {
            var versionSplit = ThisAssembly.AssemblyInformationalVersion.Split('+');

            // <major>.<minor>.<patch> (<commmithash>)
            return $"{versionSplit[0]} ({versionSplit[1]})";
        }

        public static void PrintVersion(TextWriter writer)
        {
            var output = $@"Bicep CLI version {GetVersionString()}{Environment.NewLine}";

            writer.Write(output);
            writer.Flush();
        }

        public static void PrintUsage(TextWriter writer)
        {
            var exeName = GetExeName();
            var output = 
$@"Bicep CLI version {GetVersionString()}

Usage:
  {exeName} build [options] [<files>...]
    Builds one or more .bicep files

    Arguments:
      <files>     The list of one or more .bicep files to build

    Options:
      --stdout    Prints all output to stdout instead of corresponding files

  {exeName} decompile [options] [<files>...]
    Attempts to decompile one or more template .json files to .bicep

    Arguments:
      <files>     The list of one or more .json files to decompile

  {exeName} [options]
    Options:
      --version  -v   Shows bicep version information
      --help     -h   Shows this usage information
"; // this newline is intentional

            writer.Write(output);
            writer.Flush();
        }

        private static BuildArguments ParseBuild(string[] files)
        {
            return new BuildArguments(files);
        }

        private static DecompileArguments ParseDecompile(string[] files)
        {
            return new DecompileArguments(files);
        }
    }
}

