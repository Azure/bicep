// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments
{
    public class ValidateParamsArguments : ArgumentsBase
    {
        public ValidateParamsArguments(string[] args) : base(Constants.Command.ValidateParams)
        {
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--params":
                        if (args.Length == i + 1)
                        {
                            throw new CommandLineException($"The --params parameter expects an argument");
                        }
                        if (Parameters is not null)
                        {
                            throw new CommandLineException($"The --params parameter cannot be specified twice");
                        }
                        Parameters = args[i + 1];
                        i++;
                        break;

                    default:
                        if (args[i].StartsWith("--"))
                        {
                            throw new CommandLineException($"Unrecognized parameter \"{args[i]}\"");
                        }
                        if (BicepFile is not null)
                        {
                            throw new CommandLineException($"The bicep file path cannot be specified multiple times");
                        }
                        BicepFile = args[i];
                        break;
                }
            }

            if (BicepFile is null)
            {
                throw new CommandLineException($"The bicep file path was not specified");
            }

            if (Parameters is null)
            {
                throw new CommandLineException($"The parameters to validate were not specified");
            }
        }

        public string Parameters { get; }

        public string BicepFile { get; }
    }
}
