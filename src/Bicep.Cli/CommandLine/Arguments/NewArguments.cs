// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Cli.CommandLine.Arguments
{
    public class NewArguments : ArgumentsBase
    {
        public NewArguments(string[] args)
        {
            this.IsCustomRepository = false;
            this.OutputToStdOut = false;

            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--stdout":
                        this.OutputToStdOut = true;
                        break;
                    case "--repository":
                        if (args.Length == i + 1)
                        {
                            throw new CommandLineException($"The --repository parameter expects an argument");
                        }
                        if (this.Repository is not null)
                        {
                            throw new CommandLineException($"The --repository parameter cannot be specified twice");
                        }
                        this.Repository = args[i + 1];
                        this.IsCustomRepository = true;
                        i++;
                        break;
                    case "--template":
                        if (args.Length == i + 1)
                        {
                            throw new CommandLineException($"The --template parameter expects an argument");
                        }
                        if (this.Template is not null)
                        {
                            throw new CommandLineException($"The --template parameter cannot be specified twice");
                        }
                        this.Template = args[i + 1];
                        i++;
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
                        break;
                }
            }

            if (this.OutputToStdOut && this.OutputDir is not null)
            {
                throw new CommandLineException($"The --outdir and --stdout parameters cannot both be used");
            }

            if (this.OutputToStdOut && this.OutputFile is not null)
            {
                throw new CommandLineException($"The --outfile and --stdout parameters cannot both be used");
            }

        }

        public bool IsCustomRepository { get; }

        public string? Repository { get; }

        public string? Template { get; }

        public string? OutputDir { get; }

        public string? OutputFile { get; }

        public bool OutputToStdOut { get; }
    }
}
