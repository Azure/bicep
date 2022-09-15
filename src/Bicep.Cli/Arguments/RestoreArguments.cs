// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments
{
    public class RestoreArguments : ArgumentsBase
    {
        public RestoreArguments(string[] args) : base(Constants.Command.Restore)
        {
            foreach (var argument in args)
            {
                switch (argument.ToLowerInvariant())
                {
                    case "--force":
                        ForceModulesRestore = true;
                        break;

                    default:
                        if (argument.StartsWith("--"))
                        {
                            throw new CommandLineException($"Unrecognized parameter \"{argument}\"");
                        }

                        if (InputFile is not null)
                        {
                            throw new CommandLineException($"The input file path cannot be specified multiple times.");
                        }

                        InputFile = argument;
                        break;
                }
            }

            if (InputFile is null)
            {
                throw new CommandLineException($"The input file path was not specified.");
            }
        }

        public string InputFile { get; }

        public bool ForceModulesRestore { get; }
    }
}
