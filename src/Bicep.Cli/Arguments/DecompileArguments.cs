// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Helpers;
using Bicep.Core.FileSystem;
using Bicep.IO.Abstraction;
using LanguageConstants = Bicep.Core.LanguageConstants;

namespace Bicep.Cli.Arguments
{
    public class DecompileArguments : ArgumentsBase, IInputOutputArguments<DecompileArguments>
    {
        public DecompileArguments(string[] args) : base(Constants.Command.Decompile)
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

        public static Func<DecompileArguments, IOUri, string> OutputFileExtensionResolver { get; } = (_, _) => LanguageConstants.LanguageFileExtension;

        public bool OutputToStdOut { get; }

        public string InputFile { get; }

        public string? OutputDir { get; }

        public string? OutputFile { get; }

        public bool AllowOverwrite { get; }
    }
}
