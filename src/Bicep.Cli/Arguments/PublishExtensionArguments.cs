// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Cli.Extensions;
using Bicep.Core.Registry.Oci;

namespace Bicep.Cli.Arguments
{
    public class PublishExtensionArguments : ArgumentsBase
    {
        public PublishExtensionArguments(string[] args)
            : base(Constants.Command.PublishExtension)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var isLast = args.Length == i + 1;
                switch (args[i].ToLowerInvariant())
                {
                    case "--target":
                        if (isLast)
                        {
                            throw new CommandLineException("The --target parameter expects an argument.");
                        }

                        if (this.TargetExtensionReference is not null)
                        {
                            throw new CommandLineException("The --target parameter cannot be specified twice.");
                        }

                        TargetExtensionReference = args[i + 1];
                        i++;
                        break;

                    case { } when args[i].StartsWith("--bin-"):
                        var architectureName = args[i].Substring("--bin-".Length);

                        if (!SupportedArchitectures.All.Any(x => x.Name == architectureName))
                        {
                            throw new CommandLineException($"Unrecognized parameter \"{args[i]}\"");
                        }

                        if (Binaries.ContainsKey(architectureName))
                        {
                            throw new CommandLineException($"Parameter \"{args[i]}\" cannot be specified multiple times.");
                        }

                        Binaries[architectureName] = args[i + 1];
                        i++;
                        break;

                    case "--force":
                        Force = true;
                        break;

                    default:
                        if (args[i].StartsWith("--"))
                        {
                            throw new CommandLineException($"Unrecognized parameter \"{args[i]}\"");
                        }

                        if (IndexFile is not null)
                        {
                            throw new CommandLineException($"The input file path cannot be specified multiple times.");
                        }

                        IndexFile = args[i];
                        break;
                }
            }

            if (TargetExtensionReference is null)
            {
                throw new CommandLineException("The target extension was not specified.");
            }
        }

        public Dictionary<string, string> Binaries { get; } = new();

        public string? IndexFile { get; }

        public string TargetExtensionReference { get; }

        public bool Force { get; }
    }
}
