// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments
{
    public class DecompileArguments : ArgumentsBase, IArguments
    {
        public DecompileArguments(string[] args, string commandName) : base(commandName)
        {
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant()) {
                    case "--stdout":
                        this.OutputToStdOut = true;
                        break;
                    case "--no-summary":
                        this.NoSummary = true;
                        break;
                    case "--outdir":
                        if (args.Length == i + 1)
                        {
                            throw new CommandLineException($"The --outdir parameter expects an argument");
                        }
                        if (this.OutputDir is not null)
                        {
                            throw new CommandLineException($"The --outdir parameter cannot be specified twice");
                        }
                        this.OutputDir = args[i + 1];
                        i++;
                        break;
                    case "--outfile":
                        if (args.Length == i + 1)
                        {
                            throw new CommandLineException($"The --outfile parameter expects an argument");
                        }
                        if (this.OutputFile is not null)
                        {
                            throw new CommandLineException($"The --outfile parameter cannot be specified twice");
                        }
                        this.OutputFile = args[i + 1];
                        i++;
                        break;
                    default:
                        if (args[i].StartsWith("--"))
                        {
                            throw new CommandLineException($"Unrecognized parameter \"{args[i]}\"");
                        }
                        if (this.InputFile is not null)
                        {
                            throw new CommandLineException($"The input file path cannot be specified multiple times");
                        }
                        this.InputFile = args[i];
                        break;
                }
            }

            if (this.InputFile is null)
            {
                throw new CommandLineException($"The input file path was not specified");
            }

            if (this.OutputToStdOut && this.OutputDir is not null)
            {
                throw new CommandLineException($"The --outdir and --stdout parameters cannot both be used");
            }

            if (this.OutputToStdOut && this.OutputFile is not null)
            {
                throw new CommandLineException($"The --outfile and --stdout parameters cannot both be used");
            }

            if (this.OutputDir is not null && this.OutputFile is not null)
            {
                throw new CommandLineException($"The --outdir and --outfile parameters cannot both be used");
            }
        }

        public bool OutputToStdOut { get; }
        
        public bool NoSummary { get; }

        public string InputFile { get; }

        public string? OutputDir { get; }

        public string? OutputFile { get; }
        
        public bool PrintUsage { get; }
    }
}
