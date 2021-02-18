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
            return (args[0].ToLowerInvariant()) switch
            {
                CliConstants.CommandBuild => new BuildArguments(args[1..]),
                CliConstants.CommandDecompile => new DecompileArguments(args[1..]),
                CliConstants.ArgumentHelp => new HelpArguments(),
                CliConstants.ArgumentHelpShort => new HelpArguments(),
                CliConstants.ArgumentVersion => new VersionArguments(),
                CliConstants.ArgumentVersionShort => new VersionArguments(),
                _ => null,
            };
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
  {exeName} build [options] <file>
    Builds a .bicep file

    Arguments:
      <file>        The input file.

    Options:
      --outdir <dir>    Saves the output at the specified directory.
      --outfile <file>  Saves the output as the specified file path.
      --stdout          Prints the output to stdout.

    Examples:
      bicep build file.bicep
      bicep build file.bicep --stdout
      bicep build file.bicep --outdir dir1
      bicep build file.bicep --outfile file.json

  {exeName} decompile [options] <file>
    Attempts to decompile a template .json file to .bicep

    Arguments:
      <file>        The input file.

  {exeName} [options]
    Options:
      --version  -v   Shows bicep version information
      --help     -h   Shows this usage information
"; // this newline is intentional

            writer.Write(output);
            writer.Flush();
        }
    }
}

