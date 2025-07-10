// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Cli.Extensions;

namespace Bicep.Cli.Arguments
{
    public class PublishArguments : ArgumentsBase, IInputArguments
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

                    case "--documentation-uri":
                        if (isLast)
                        {
                            throw new CommandLineException("The --documentation-uri parameter expects an argument.");
                        }

                        if (this.DocumentationUri is not null)
                        {
                            throw new CommandLineException("The --documentation-uri parameter cannot be specified more than once.");
                        }

                        DocumentationUri = args[i + 1];

                        if (!Uri.IsWellFormedUriString(DocumentationUri, UriKind.Absolute))
                        {
                            throw new CommandLineException("The --documentation-uri should be a well formed uri string.");
                        }

                        i++;
                        break;

                    case "--with-source":
                        WithSource = true;
                        break;

                    case "--force":
                        Force = true;
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
        }

        public string? DocumentationUri { get; }

        public string InputFile { get; }

        public string TargetModuleReference { get; }

        public bool NoRestore { get; }

        public bool Force { get; }

        public bool WithSource { get; }
    }
}
