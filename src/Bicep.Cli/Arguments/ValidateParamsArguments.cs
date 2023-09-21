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
                if (args[i].StartsWith("--"))
                {
                    throw new CommandLineException($"Unrecognized parameter \"{args[i]}\"");
                }
                if (BicepFile is not null)
                {
                    throw new CommandLineException($"The bicep file path cannot be specified multiple times");
                }
                BicepFile = args[i];
            }

            if (BicepFile is null)
            {
                throw new CommandLineException($"The bicep file path was not specified");
            }
        }

        public string BicepFile { get; }
    }
}
