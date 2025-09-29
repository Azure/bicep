// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Helpers;
using Bicep.IO.Abstraction;
using LanguageConstants = Bicep.Core.LanguageConstants;

namespace Bicep.Cli.Arguments
{
    public class DecompileParamsArguments : ArgumentsBase, IInputOutputArguments<DecompileParamsArguments>
    {
        public DecompileParamsArguments(string[] args) : base(Constants.Command.DecompileParams)
        {
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--stdout":
                        OutputToStdOut = true;
                        break;
                    case "--force":
                        AllowOverwrite = true;
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
                    case "--bicep-file":
                        if (args.Length == i + 1)
                        {
                            throw new CommandLineException($"The --bicep-file parameter expects an argument");
                        }
                        if (BicepFilePath is not null)
                        {
                            throw new CommandLineException($"The --bicep-file parameter cannot be specified twice");
                        }
                        BicepFilePath = args[i + 1];
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
        }

        public static Func<DecompileParamsArguments, IOUri, string> OutputFileExtensionResolver { get; } = (_, _) => LanguageConstants.ParamsFileExtension;

        public bool OutputToStdOut { get; }

        public string InputFile { get; }

        public string? OutputDir { get; }

        public string? OutputFile { get; }

        public bool AllowOverwrite { get; }

        public string? BicepFilePath { get; }
    }
}
