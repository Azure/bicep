// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments
{
    public class PublishArguments : ArgumentsBase
    {
        public PublishArguments(string[] args) : base(Constants.Command.Publish)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var isLast = args.Length == i + 1;
                switch (args[i].ToLowerInvariant())
                {
                    case "--no-restore":
                        NoRestore = true;
                        break;

                    case "--target":
                        if (isLast)
                        {
                            throw new CommandLineException("The --target parameter expects an argument.");
                        }

                        if (this.TargetModuleReference is not null)
                        {
                            throw new CommandLineException("The --target parameter cannot be specified twice.");
                        }

                        TargetModuleReference = args[i + 1];
                        i++;
                        break;

                    case "--documentationurl":
                        if (isLast)
                        {
                            throw new CommandLineException("The --documentationUrl parameter expects an argument.");
                        }

                        if (this.DocumentationUrl is not null)
                        {
                            throw new CommandLineException("The --documentationUrl parameter cannot be specified twice.");
                        }

                        DocumentationUrl = args[i + 1];
                        i++;
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

            if (TargetModuleReference is null)
            {
                throw new CommandLineException("The target module was not specified.");
            }

            //if (DocumentationUrl is null)
            //{
            //    throw new CommandLineException("The documentation url was not specified.");
            //}
        }

        public string InputFile { get; }

        public string TargetModuleReference { get; }

        public string? DocumentationUrl { get; }

        public bool NoRestore { get; }
    }
}
