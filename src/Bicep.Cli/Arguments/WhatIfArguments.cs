// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.ResourceManager.Resources.Models;

namespace Bicep.Cli.Arguments;

public class WhatIfArguments : ArgumentsBase
{
    public WhatIfArguments(string[] args) : base(Constants.Command.WhatIf)
    {
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLowerInvariant())
            {
                case "--result-format":
                    if (args.Length == i + 1)
                    {
                        throw new CommandLineException($"The --result-format parameter expects an argument");
                    }
                    if (ResultFormat is not null)
                    {
                        throw new CommandLineException($"The --result-format parameter cannot be specified twice");
                    }
                    ResultFormat = GetResultFormat(args[i + 1]);
                    i++;
                    break;

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

    public WhatIfResultFormat? ResultFormat { get; }

    public bool NoRestore { get; }

    private static WhatIfResultFormat? GetResultFormat(string? format)
    {
        if (format is null)
        {
            return null;
        }

        if (format.Equals("FullResourcePayloads", StringComparison.OrdinalIgnoreCase))
        {
            return WhatIfResultFormat.FullResourcePayloads;
        }
        if (format.Equals("ResourceIdOnly", StringComparison.OrdinalIgnoreCase))
        {
            return WhatIfResultFormat.ResourceIdOnly;
        }

        throw new CommandLineException($"Unrecognized result format \"{format}\"");
    }
}