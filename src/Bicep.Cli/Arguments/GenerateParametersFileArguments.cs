// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.FileSystem;
using System.IO;

namespace Bicep.Cli.Arguments
{
    public class GenerateParametersFileArguments : ArgumentsBase
    {
        public GenerateParametersFileArguments(string[] args) : base(Constants.Command.GenerateParamsFile)
        {
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--stdout":
                        OutputToStdOut = true;
                        break;

                    case "--no-restore":
                        NoRestore = true;
                        break;

                    case "--outdir":
                        if (args.Length == i + 1)
                        {
                            throw new CommandLineException($"The --outdir parameter expects an argument");
                        }
                        if (OutputDir is not null)
                        {
                            throw new CommandLineException($"The --outdir parameter cannot be specified twice");
                        }
                        OutputDir = args[i + 1];
                        i++;
                        break;

                    case "--output-format":
                        if (args.Length == i + 1)
                        {
                            throw new CommandLineException($"The --output-format parameter expects an argument");
                        }
                        if (OutputFormat is not null)
                        {
                            throw new CommandLineException($"The --output-format parameter cannot be specified twice");
                        }
                        OutputFormat = args[i + 1];
                        i++;
                        break;

                    case "--include-params":
                        if (args.Length == i + 1)
                        {
                            throw new CommandLineException($"The --include-params parameter expects an argument");
                        }
                        if (IncludeParams is not null)
                        {
                            throw new CommandLineException($"The --include-params parameter cannot be specified twice");
                        }
                        IncludeParams = args[i + 1];
                        i++;
                        break;

                    case "--outfile":
                        if (args.Length == i + 1)
                        {
                            throw new CommandLineException($"The --outfile parameter expects an argument");
                        }
                        if (OutputFile is not null)
                        {
                            throw new CommandLineException($"The --outfile parameter cannot be specified twice");
                        }
                        OutputFile = args[i + 1];
                        i++;
                        break;

                    default:
                        if (args[i].StartsWith("--"))
                        {
                            throw new CommandLineException($"Unrecognized parameter \"{args[i]}\"");
                        }
                        if (InputFile is not null)
                        {
                            throw new CommandLineException($"The input file path cannot be specified multiple times");
                        }
                        InputFile = args[i];
                        break;
                }
            }

            if (InputFile is null)
            {
                throw new CommandLineException($"The input file path was not specified");
            }

            if (OutputToStdOut && OutputDir is not null)
            {
                throw new CommandLineException($"The --outdir and --stdout parameters cannot both be used");
            }

            if (OutputToStdOut && OutputFile is not null)
            {
                throw new CommandLineException($"The --outfile and --stdout parameters cannot both be used");
            }

            if (OutputDir is not null && OutputFile is not null)
            {
                throw new CommandLineException($"The --outdir and --outfile parameters cannot both be used");
            }

            if (OutputFormat is null)
            {
                OutputFormat = "json";
            }

            if (IncludeParams is null)
            {
                IncludeParams = "required";
            }

            if (OutputDir is not null)
            {
                var outputDir = PathHelper.ResolvePath(OutputDir);

                if (!Directory.Exists(outputDir))
                {
                    throw new CommandLineException(string.Format(CliResources.DirectoryDoesNotExistFormat, outputDir));
                }
            }
        }

        public bool OutputToStdOut { get; }

        public string InputFile { get; }

        public string? OutputDir { get; }

        public string? OutputFile { get; }

        public string OutputFormat { get; }

        public string IncludeParams { get; }

        public bool NoRestore { get; }
    }
}
