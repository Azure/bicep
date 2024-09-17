// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments;

public class ValidateArguments : ArgumentsBase
{
    public ValidateArguments(string[] args) : base(Constants.Command.Validate)
    {
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLowerInvariant())
            {
                case "--no-restore":
                    NoRestore = true;
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
    }

    public string InputFile { get; }

    public string? Name { get; }

    public bool NoRestore { get; }
}
