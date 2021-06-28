// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using Bicep.Cli.Arguments;

namespace Bicep.Cli.Services
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
                Constants.Command.Build => new BuildArguments(args[1..], Constants.Command.Build),
                Constants.Command.Decompile => new DecompileArguments(args[1..], Constants.Command.Decompile),
                Constants.Argument.Help => new HelpArguments(Constants.Argument.Help),
                Constants.Argument.HelpShort => new HelpArguments(Constants.Argument.HelpShort),
                Constants.Argument.Version => new VersionArguments(Constants.Argument.Version),
                Constants.Argument.VersionShort => new VersionArguments(Constants.Argument.VersionShort),
                _ => null,
            };
        }

        public static string GetExeName()
            => ThisAssembly.AssemblyName;

        private static string GetVersionString()
        {
            var versionSplit = ThisAssembly.AssemblyInformationalVersion.Split('+');

            // <major>.<minor>.<patch> (<commmithash>)
            return $"{versionSplit[0]} ({(versionSplit.Length > 1 ? versionSplit[1] : "custom")})";
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
      --no-summary      Omits the summary at the end of the build.
      --stdout          Prints the output to stdout.

    Examples:
      bicep build file.bicep
      bicep build file.bicep --stdout
      bicep build file.bicep --no-summary
      bicep build file.bicep --outdir dir1
      bicep build file.bicep --outfile file.json

  {exeName} decompile [options] <file>
    Attempts to decompile a template .json file to .bicep

    Arguments:
      <file>        The input file.

    Options:
      --outdir <dir>    Saves the output at the specified directory.
      --outfile <file>  Saves the output as the specified file path.
      --stdout          Prints the output to stdout.

    Examples:
      bicep decompile file.json
      bicep decompile file.json --stdout
      bicep decompile file.json --outdir dir1
      bicep decompile file.json --outfile file.bicep

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

