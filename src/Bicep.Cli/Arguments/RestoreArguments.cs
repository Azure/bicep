// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments
{
    public class RestoreArguments : ArgumentsBase
    {
        public RestoreArguments(string[] args) : base(Constants.Command.Restore)
        {
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--force":
                        ForceRestore = true;
                        break;

                    default:
                        if (args[i].StartsWith("--"))
                        {
                            throw new CommandLineException($"Unrecognized parameter \"{args[i]}\"");
                        }

                        if (InputFile is not null)
                        {
                            throw new CommandLineException($"The input file path cannot be specified multiple times.");
                        }

                        InputFile = args[i];
                        break;
                }
            }

            if (InputFile is null)
            {
                throw new CommandLineException($"The input file path was not specified.");
            }
        }

        public string InputFile { get; }

        public bool ForceRestore { get; }
    }
}
