// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments;

public abstract class DeployArgumentsBase : ArgumentsBase, IInputArguments
{
    public DeployArgumentsBase(string[] args, string commandName) : base(commandName)
    {
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLowerInvariant())
            {
                case "--no-restore":
                    NoRestore = true;
                    break;

                case { } when args[i].StartsWith(ArgumentConstants.CliArgPrefix):
                    var key = args[i][ArgumentConstants.CliArgPrefix.Length..];

                    if (AdditionalArguments.ContainsKey(key))
                    {
                        throw new CommandLineException($"Parameter \"{args[i]}\" cannot be specified multiple times.");
                    }

                    AdditionalArguments[key] = args[i + 1];
                    i++;
                    break;

                default:
                    if (args[i].StartsWith("--"))
                    {
                        throw new CommandLineException($"Unrecognized parameter \"{args[i]}\"");
                    }
                    if (InputFile is not null)
                    {
                        throw new CommandLineException($"The parameters file path cannot be specified multiple times");
                    }
                    InputFile = args[i];
                    break;
            }
        }

        if (InputFile is null)
        {
            throw new CommandLineException($"The parameters file path was not specified");
        }
    }

    public string InputFile { get; }

    public bool NoRestore { get; }

    public Dictionary<string, string> AdditionalArguments { get; } = [];
}
