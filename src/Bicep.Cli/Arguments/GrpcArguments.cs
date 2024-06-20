// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments;

public class GrpcArguments : ArgumentsBase
{
    public GrpcArguments(string[] args) : base(Constants.Command.Grpc)
    {
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLowerInvariant())
            {
                case "--socket":
                    if (args.Length == i + 1)
                    {
                        throw new CommandLineException($"The --socket parameter expects an argument");
                    }
                    if (Socket is not null)
                    {
                        throw new CommandLineException($"The --socket parameter cannot be specified twice");
                    }
                    Socket = args[i + 1];
                    i++;
                    break;
                default:
                    throw new CommandLineException($"Unrecognized parameter \"{args[i]}\"");
            }
        }

        if (Socket is null)
        {
            throw new CommandLineException("The socket was not specified.");
        }
    }

    public string? Socket { get; set; }
}
