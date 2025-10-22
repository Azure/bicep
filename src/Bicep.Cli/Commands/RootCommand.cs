// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Compression;
using Bicep.Cli.Arguments;
using Bicep.Core.Exceptions;
using Bicep.Core.Utils;

namespace Bicep.Cli.Commands
{
    public class RootCommand(
        IOContext io,
        IEnvironment environment) : ICommand
    {
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

            if (args.PrintLicense)
            {
                PrintLicense();
                return 0;
            }

            if (args.PrintThirdPartyNotices)
            {
                PrintThirdPartyNotices();
                return 0;
            }

            return 1;
        }

        private void PrintHelp()
        {
            var exeName = ThisAssembly.AssemblyName;
            var versionString = environment.GetVersionString();

            var output =
$@"Bicep CLI version {versionString}

Usage:
  {exeName} build [options] [<file>]
    Builds a .bicep file.

    Arguments:
      <file>        The input file

    Options:
      --outdir <dir>                 Saves the output at the specified directory.
      --outfile <file>               Saves the output as the specified file path.
      --stdout                       Prints the output to stdout.
      --no-restore                   Builds the bicep file without restoring external modules.
      --diagnostics-format <format>  Sets the format with which diagnostics are displayed. Valid values are ( {string.Join(" | ", Enum.GetNames(typeof(DiagnosticsFormat)))} ).
      --pattern <pattern>            Builds all files matching the specified glob pattern.

    Examples:
      bicep build file.bicep
      bicep build file.bicep --stdout
      bicep build file.bicep --outdir dir1
      bicep build file.bicep --outfile file.json
      bicep build file.bicep --no-restore
      bicep build file.bicep --diagnostics-format sarif
      bicep build --pattern './dir/**/*.bicep'

  {exeName} format [options] [<file>]
    Formats a .bicep file.

    Arguments:
      <file>        The input file

    Options:
      --outdir <dir>            Saves the output at the specified directory.
      --outfile <file>          Saves the output as the specified file path.
      --stdout                  Prints the output to stdout.
      --newline                 Set newline char. Valid values are ( Auto | LF | CRLF | CR ).
      --indent-kind             Set indentation kind. Valid values are ( Space | Tab ).
      --indent-size             Number of spaces to indent with (Only valid with --indentKind set to Space).
      --insert-final-newline    Insert a final newline.
      --pattern <pattern>       Formats all files matching the specified glob pattern.

    Examples:
      bicep format file.bicep
      bicep format file.bicep --stdout
      bicep format file.bicep --outdir dir1
      bicep format file.bicep --outfile file.json
      bicep format file.bicep --indent-kind Tab
      bicep format --pattern './dir/**/*.bicep'

  {exeName} decompile [options] <file>
    Attempts to decompile a template .json file to .bicep.

    Arguments:
      <file>        The input file

    Options:
      --outdir <dir>    Saves the output at the specified directory.
      --outfile <file>  Saves the output as the specified file path.
      --stdout          Prints the output to stdout.
      --force           Allows overwriting the output file if it exists (applies only to 'bicep decompile' or 'bicep decompile-params').

    Examples:
      bicep decompile file.json
      bicep decompile file.json --stdout
      bicep decompile file.json --outdir dir1
      bicep decompile file.json --force
      bicep decompile file.json --outfile file.bicep

  {exeName} lint [options] [<file>]
    Lints a .bicep file.

    Arguments:
      <file>        The input file

    Options:
      --no-restore                   Skips restoring external modules.
      --diagnostics-format <format>  Sets the format with which diagnostics are displayed. Valid values are ( {string.Join(" | ", Enum.GetNames(typeof(DiagnosticsFormat)))} ).
      --pattern <pattern>            Lints all files matching the specified glob pattern.

    Examples:
      bicep lint file.bicep
      bicep lint file.bicep --no-restore
      bicep lint file.bicep --diagnostics-format sarif
      bicep lint --pattern './dir/**/*.bicep'

  {exeName} decompile-params [options] <file>
    Attempts to decompile a parameters .json file to .bicepparam.

    Arguments:
      <file>        The input file

    Options:
      --outdir <dir>    Saves the output at the specified directory.
      --outfile <file>  Saves the output as the specified file path.
      --stdout          Prints the output to stdout.
      --force           Allows overwriting the output file if it exists (applies only to 'bicep decompile' or 'bicep decompile-params').
      --bicep-file      Path to the bicep template file that will be referenced in the using declaration

    Examples:
      bicep decompile-params file.json
      bicep decompile-params file.json --bicep-file ./dir/main.bicep
      bicep decompile-params file.json --stdout
      bicep decompile-params file.json --outdir dir1
      bicep decompile-params file.json --force
      bicep decompile-params file.json --outfile file.bicepparam

  {exeName} generate-params [options] <file>
    Builds parameters file from the given bicep file, updates if there is an existing parameters file.

    Arguments:
      <file>        The input file

    Options:
      --no-restore      Generates the parameters file without restoring external modules.
      --outdir <dir>    Saves the output at the specified directory.
      --outfile <file>  Saves the output as the specified file path.
      --stdout          Prints the output to stdout.
      --output-format   Selects the output format {{json, bicepparam}}
      --include-params  Selects which parameters to include into output {{requiredonly, all}}

    Examples:
      bicep generate-params file.bicep
      bicep generate-params file.bicep --no-restore
      bicep generate-params file.bicep --stdout
      bicep generate-params file.bicep --outdir dir1
      bicep generate-params file.bicep --outfile file.parameters.json
      bicep generate-params file.bicep --output-format bicepparam --include-params all

  {exeName} publish <file> --target <ref>
    Publishes the .bicep file to the module registry.

    Arguments:
      <file>        The input file (can be a Bicep file or an ARM template file)
      <ref>         The module reference

    Options:
      --documentation-uri  Module documentation uri
      --with-source       [Experimental] Publish source code with the module
      --force             Overwrite existing published module or file

    Examples:
      bicep publish file.bicep --target br:example.azurecr.io/hello/world:v1
      bicep publish file.bicep --target br:example.azurecr.io/hello/world:v1 --force
      bicep publish file.bicep --target br:example.azurecr.io/hello/world:v1 --documentation-uri https://github.com/hello-world/README.md --with-source
      bicep publish file.json --target br:example.azurecr.io/hello/world:v1 --documentation-uri https://github.com/hello-world/README.md

  {exeName} restore [<file>]
    Restores external modules from the specified Bicep file to the local module cache.

    Arguments:
      <file>        The input file

    Options:
      --pattern <pattern>  Restores all files matching the specified glob pattern.

    Examples:
      bicep restore main.bicep
      bicep restore --pattern './dir/**/*.bicep'

  {exeName} [options]
    Options:
      --version              -v   Shows bicep version information
      --help                 -h   Shows this usage information
      --license                   Prints license information
      --third-party-notices       Prints third-party notices

  {exeName} build-params [<file>]
    Builds a .json file from a .bicepparam file.

    Arguments:
      <file>        The input Bicepparam file

    Options:
      --bicep-file <file>            Verifies if the specified bicep file path matches the one provided in the params file using declaration
      --outdir <dir>                 Saves the output of building the parameter file only (.bicepparam) as json to the specified directory.
      --outfile <file>               Saves the output of building the parameter file only (.bicepparam) as json to the specified file path.
      --stdout                       Prints the output of building both the parameter file (.bicepparam) and the template it points to (.bicep) as json to stdout.
      --no-restore                   Builds the bicep file (referenced in using declaration) without restoring external modules.
      --diagnostics-format <format>  Sets the format with which diagnostics are displayed. Valid values are ( {string.Join(" | ", Enum.GetNames(typeof(DiagnosticsFormat)))} ).
      --pattern <pattern>            Builds all files matching the specified glob pattern.

    Examples:
      bicep build-params params.bicepparam
      bicep build-params params.bicepparam --stdout
      bicep build-params params.bicepparam --outdir dir1
      bicep build-params params.bicepparam --outfile otherParams.json
      bicep build-params params.bicepparam --no-restore
      bicep build-params params.bicepparam --diagnostics-format sarif
      bicep build-params --pattern './dir/**/*.bicepparam'

  {exeName} jsonrpc [options]
    Runs a JSONRPC server for interacting with Bicep programmatically.

    Options:
      --pipe <name>   Runs the JSONRPC server using a named pipe.
      --socket <dir>  Runs the JSONRPC server on a specific port.
      --stdio         Runs the JSONRPC server over stdin/stdout.

    Examples:
      bicep jsonrpc --pipe /path/to/pipe.sock
      bicep jsonrpc --stdio

"; // this newline is intentional

            io.Output.Write(output);
            io.Output.Flush();
        }

        private void PrintVersion()
        {
            var output = $@"Bicep CLI version {environment.GetVersionString()}{System.Environment.NewLine}";

            io.Output.Write(output);
            io.Output.Flush();
        }

        private void PrintLicense()
        {
            WriteEmbeddedResource(io.Output, "LICENSE.deflated");
        }

        private void PrintThirdPartyNotices()
        {
            WriteEmbeddedResource(io.Output, "NOTICE.deflated");
        }

        private static void WriteEmbeddedResource(TextWriter writer, string streamName)
        {
            using var stream = typeof(RootCommand).Assembly.GetManifestResourceStream(streamName)
                ?? throw new BicepException($"The resource stream '{streamName}' is missing from this executable.");

            using var decompressor = new DeflateStream(stream, CompressionMode.Decompress);

            using var reader = new StreamReader(decompressor);
            string? line = null;
            while ((line = reader.ReadLine()) is not null)
            {
                writer.WriteLine(line);
            }
        }
    }
}
