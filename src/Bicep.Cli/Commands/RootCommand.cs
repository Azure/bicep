// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Core.Exceptions;
using System;
using System.IO;
using System.IO.Compression;

namespace Bicep.Cli.Commands
{
    public class RootCommand : ICommand
    {
        private readonly InvocationContext invocationContext;

        public RootCommand(InvocationContext invocationContext)
        {
            this.invocationContext = invocationContext;
        }

        public int Run(RootArguments args)
        {
            if (args.PrintVersion)
            {
                PrintVersion();
                return 0;
            }

            if (args.PrintHelp)
            {
                PrintHelp();
                return 0;
            }

            if(args.PrintLicense)
            {
                PrintLicense();
                return 0;
            }

            if(args.PrintThirdPartyNotices)
            {
                PrintThirdPartyNotices();
                return 0;
            }

            return 1;
        }

        private void PrintHelp()
        {
            var exeName = ThisAssembly.AssemblyName;
            var versionString = GetVersionString();

            var output =
$@"Bicep CLI version {versionString}

Usage:
  {exeName} build [options] <file>
    Builds a .bicep file.

    Arguments:
      <file>        The input file

    Options:
      --outdir <dir>    Saves the output at the specified directory.
      --outfile <file>  Saves the output as the specified file path.
      --stdout          Prints the output to stdout.
      --no-restore      Builds the bicep file without restoring external modules.

    Examples:
      bicep build file.bicep
      bicep build file.bicep --stdout
      bicep build file.bicep --outdir dir1
      bicep build file.bicep --outfile file.json
      bicep build file.bicep --no-restore

    {exeName} format [options] <file>
    Formats a .bicep file.

    Arguments:
      <file>        The input file

    Options:
      --outdir <dir>        Saves the output at the specified directory.
      --outfile <file>      Saves the output as the specified file path.
      --stdout              Prints the output to stdout.
      --newline             Set newline char. Valid values are ( Auto | LF | CRLF | CR ).
      --indentKind          Set indentation kind. Valid values are ( Space | Tab ).
      --indentSize          Number of spaces to indent with (Only valid with --indentKind set to Space).
      --insertFinalNewline  Insert a final newline.

    Examples:
      bicep format file.bicep
      bicep format file.bicep --stdout
      bicep format file.bicep --outdir dir1
      bicep format file.bicep --outfile file.json
      bicep format file.bicep --indentKind Tab

  {exeName} decompile [options] <file>
    Attempts to decompile a template .json file to .bicep.

    Arguments:
      <file>        The input file

    Options:
      --outdir <dir>    Saves the output at the specified directory.
      --outfile <file>  Saves the output as the specified file path.
      --stdout          Prints the output to stdout.
      --force           Allows overwriting the output file if it exists (applies only to 'bicep decompile').

    Examples:
      bicep decompile file.json
      bicep decompile file.json --stdout
      bicep decompile file.json --outdir dir1
      bicep decompile file.json --force
      bicep decompile file.json --outfile file.bicep

  {exeName} generate-params [options] <file>
    Builds .parameters.json file from the given bicep file, updates if there is an existing parameters.json file.

    Arguments:
      <file>        The input file

    Options:
      --no-restore      Generates the parameters file without restoring external modules.
      --outdir <dir>    Saves the output at the specified directory.
      --outfile <file>  Saves the output as the specified file path.
      --stdout          Prints the output to stdout.

    Examples:
      bicep generate-params file.bicep
      bicep generate-params file.bicep --no-restore
      bicep generate-params file.bicep --stdout
      bicep generate-params file.bicep --outdir dir1
      bicep generate-params file.bicep --outfile file.parameters.json


  {exeName} publish <file> --target <ref>
    Publishes the .bicep file to the module registry.

    Arguments:
      <file>        The input file (can be a Bicep file or an ARM template file)
      <ref>         The module reference

    Examples:
      bicep publish file.bicep --target br:example.azurecr.io/hello/world:v1
      bicep publish file.json --target br:example.azurecr.io/hello/world:v1

  {exeName} restore <file>
    Restores external modules from the specified Bicep file to the local module cache.

    Arguments:
      <file>        The input file

 {exeName} [options]
    Options:
      --version              -v   Shows bicep version information
      --help                 -h   Shows this usage information
      --license                   Prints license information
      --third-party-notices       Prints third-party notices

"; // this newline is intentional

            invocationContext.OutputWriter.Write(output);
            invocationContext.OutputWriter.Flush();
        }

        private void PrintVersion()
        {
            var output = $@"Bicep CLI version {GetVersionString()}{Environment.NewLine}";

            invocationContext.OutputWriter.Write(output);
            invocationContext.OutputWriter.Flush();
        }

        private void PrintLicense()
        {
            WriteEmbeddedResource(invocationContext.OutputWriter, "LICENSE.deflated");
        }

        private void PrintThirdPartyNotices()
        {
            WriteEmbeddedResource(invocationContext.OutputWriter, "NOTICE.deflated");
        }

        private static string GetVersionString()
        {
            var versionSplit = ThisAssembly.AssemblyInformationalVersion.Split('+');

            // <major>.<minor>.<patch> (<commmithash>)
            return $"{versionSplit[0]} ({(versionSplit.Length > 1 ? versionSplit[1] : "custom")})";
        }

        private static void WriteEmbeddedResource(TextWriter writer, string streamName)
        {
            using var stream = typeof(RootCommand).Assembly.GetManifestResourceStream(streamName)
                ?? throw new BicepException($"The resource stream '{streamName}' is missing from this executable.");

            using var decompressor = new DeflateStream(stream, CompressionMode.Decompress);

            using var reader = new StreamReader(decompressor);
            string? line = null;
            while((line = reader.ReadLine()) is not null)
            {
                writer.WriteLine(line);
            }
        }
    }
}
