// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments;

public class JsonRpcArguments : ArgumentsBase
{
    public JsonRpcArguments(string[] args) : base(Constants.Command.JsonRpc)
    {
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLowerInvariant())
            {
                case "--pipe":
                    if (args.Length == i + 1)
                    {
                        throw new CommandLineException($"The --pipe parameter expects an argument");
                    }
                    if (Pipe is not null)
                    {
                        throw new CommandLineException($"The --pipe parameter cannot be specified twice");
                    }
                    Pipe = args[i + 1];
                    i++;
                    break;
                case "--socket":
                    if (args.Length == i + 1)
                    {
                        throw new CommandLineException($"The --socket parameter expects an argument");
                    }
                    if (Socket is not null)
                    {
                        throw new CommandLineException($"The --socket parameter cannot be specified twice");
                    }
                    if (!int.TryParse(args[i + 1], out var socket))
                    {
                        throw new CommandLineException($"The --socket parameter only accepts integer values");
                    }
                    Socket = socket;
                    i++;
                    break;

                case "--stdio":
                    if (Stdio is not null)
                    {
                        throw new CommandLineException($"The --stdio parameter cannot be specified twice");
                    }
                    Stdio = true;
                    break;
                default:
                    throw new CommandLineException($"Unrecognized parameter \"{args[i]}\"");
            }
        }
    }

    public string? Pipe { get; set; }

    public int? Socket { get; set; }

    public bool? Stdio { get; set; }
}
