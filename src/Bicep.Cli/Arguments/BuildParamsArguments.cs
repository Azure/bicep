// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments
{
    public class BuildParamsArguments : ArgumentsBase
    {
        public BuildParamsArguments(string[] args) : base(Constants.Command.BuildParams)
        {
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--bicep-file":
                        if (args.Length == i + 1)
                        {
                            throw new CommandLineException($"The --bicep-file parameter expects an argument");
                        }
                        if (BicepFile is not null)
                        {
                            throw new CommandLineException($"The --bicep-file parameter cannot be specified twice");
                        }
                        BicepFile = args[i + 1];
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

                    case "--stdout":
                        OutputToStdOut = true;
                        break;

                    case "--no-restore":
                        NoRestore = true;
                        break;

                    default:
                        if (args[i].StartsWith("--"))
                        {
                            throw new CommandLineException($"Unrecognized parameter \"{args[i]}\"");
                        }
                        if (ParamsFile is not null)
                        {
                            throw new CommandLineException($"The parameters file path cannot be specified multiple times");
                        }
                        ParamsFile = args[i];
                        break;
                }
            }

            if (ParamsFile is null)
            {
                throw new CommandLineException($"The parameters file path was not specified");
            }

            if ((OutputFile is not null) && OutputToStdOut)
            {
                throw new CommandLineException($"The --stdout can not be use when --outfile is specified");
            }
        }

        public bool OutputToStdOut { get; }

        public bool NoRestore { get; }

        public string ParamsFile { get; }

        public string? BicepFile { get; }

        public string? OutputFile { get; }
    }
}
