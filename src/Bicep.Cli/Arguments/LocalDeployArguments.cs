// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Helpers;

namespace Bicep.Cli.Arguments
{
    public class LocalDeployArguments : ArgumentsBase
    {
        public LocalDeployArguments(string[] args) : base(Constants.Command.LocalDeploy)
        {
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--no-restore":
                        NoRestore = true;
                        break;

                    case ArgumentConstants.OutputFormat:
                        ArgumentHelper.ValidateNotAlreadySet(ArgumentConstants.OutputFormat, OutputFormat);
                        OutputFormat = ArgumentHelper.GetEnumValueWithValidation<DeploymentOutputFormat>(ArgumentConstants.OutputFormat, args, i);
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

        public DeploymentOutputFormat? OutputFormat { get; }
    }
}
