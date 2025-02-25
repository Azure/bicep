// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Helpers;
using Bicep.Core.Emit.Options;
using Bicep.Core.FileSystem;

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

                    case ArgumentConstants.OutDir:
                        ArgumentHelper.ValidateNotAlreadySet(ArgumentConstants.OutDir, OutputDir);
                        OutputDir = ArgumentHelper.GetValueWithValidation(ArgumentConstants.OutDir, args, i);
                        i++;
                        break;

                    case ArgumentConstants.OutFile:
                        ArgumentHelper.ValidateNotAlreadySet(ArgumentConstants.OutFile, OutputFile);
                        OutputFile = ArgumentHelper.GetValueWithValidation(ArgumentConstants.OutFile, args, i);
                        i++;
                        break;

                    case "--output-format":
                        if (args.Length == i + 1)
                        {
                            throw new CommandLineException($"The --output-format parameter expects an argument");
                        }
                        if (!Enum.TryParse<OutputFormatOption>(args[i + 1], true, out var outputFormat) || !Enum.IsDefined<OutputFormatOption>(outputFormat))
                        {
                            throw new CommandLineException($"The --output-format parameter only accepts values: {string.Join(" | ", Enum.GetNames(typeof(OutputFormatOption)))}");
                        }
                        OutputFormat = outputFormat;
                        i++;
                        break;

                    case "--include-params":
                        if (args.Length == i + 1)
                        {
                            throw new CommandLineException($"The --include-params parameter expects an argument");
                        }
                        if (!Enum.TryParse<IncludeParamsOption>(args[i + 1], true, out var includeParams) || !Enum.IsDefined<IncludeParamsOption>(includeParams))
                        {
                            throw new CommandLineException($"The --include-params parameter only accepts values: {string.Join(" | ", Enum.GetNames(typeof(IncludeParamsOption)))}");
                        }
                        IncludeParams = includeParams;
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

        public OutputFormatOption OutputFormat { get; } = OutputFormatOption.Json;

        public IncludeParamsOption IncludeParams { get; } = IncludeParamsOption.RequiredOnly;

        public bool NoRestore { get; }
    }
}
