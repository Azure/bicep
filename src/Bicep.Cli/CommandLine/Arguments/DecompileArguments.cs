// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.CommandLine.Arguments
{
    public class DecompileArguments : ArgumentsBase
    {
        public DecompileArguments(string[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant()) {
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
        }

        public string InputFile { get; }
    }
}