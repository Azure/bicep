// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Cli.Arguments
{
    public class PublishProviderArguments : ArgumentsBase
    {
        public PublishProviderArguments(string[] args) : base(Constants.Command.PublishProvider)
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

                        if (this.TargetProviderReference is not null)
                        {
                            throw new CommandLineException("The --target parameter cannot be specified twice.");
                        }

                        TargetProviderReference = args[i + 1];
                        i++;
                        break;

                    case "--documentationuri":
                        if (isLast)
                        {
                            throw new CommandLineException("The --documentationUri parameter expects an argument.");
                        }

                        if (this.DocumentationUri is not null)
                        {
                            throw new CommandLineException("The --documentationUri parameter cannot be specified more than once.");
                        }

                        DocumentationUri = args[i + 1];

                        if (!Uri.IsWellFormedUriString(DocumentationUri, UriKind.Absolute))
                        {
                            throw new CommandLineException("The --documentationUri should be a well formed uri string.");
                        }

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

            if (IndexFile is null)
            {
                throw new CommandLineException($"The input file path was not specified.");
            }

            if (TargetProviderReference is null)
            {
                throw new CommandLineException("The target provider was not specified.");
            }
        }

        public string? DocumentationUri { get; }

        public string IndexFile { get; }

        public string TargetProviderReference { get; }

        public bool NoRestore { get; }

        public bool Force { get; }
    }
}
