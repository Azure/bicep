// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Helpers;

namespace Bicep.Cli.Arguments;

public abstract class LiveDeploymentArgumentsBase : ArgumentsBase
{
    public const string ArgPrefix = "--arg-";

    public LiveDeploymentArgumentsBase(string[] args, string commandName) : base(commandName)
    {
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLowerInvariant())
            {
                case "--no-restore":
                    NoRestore = true;
                    break;

                case { } when args[i].StartsWith(ArgPrefix):
                    var key = args[i][ArgPrefix.Length..];

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
    }

    public string ParamsFile { get; }

    public bool NoRestore { get; }

    public Dictionary<string, string> AdditionalArguments { get; } = [];
}

public class WhatIfArguments : LiveDeploymentArgumentsBase
{
    public WhatIfArguments(string[] args) : base(args, Constants.Command.WhatIf)
    {
    }
}

public class DeployArguments : LiveDeploymentArgumentsBase
{
    public DeployArguments(string[] args) : base(args, Constants.Command.Deploy)
    {
    }
}

public class DestroyArguments : LiveDeploymentArgumentsBase
{
    public DestroyArguments(string[] args) : base(args, Constants.Command.Destroy)
    {
    }
}