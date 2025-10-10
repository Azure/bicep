// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Helpers;

namespace Bicep.Cli.Arguments;

public class DeployArguments : DeployArgumentsBase
{
    public DeployArguments(string[] args) : base(args, Constants.Command.Deploy)
    {
    }

    protected override void ParseAdditionalArgument(string[] args, ref int i)
    {
        switch (args[i].ToLowerInvariant())
        {
            case ArgumentConstants.OutputFormat:
                ArgumentHelper.ValidateNotAlreadySet(ArgumentConstants.OutputFormat, OutputFormat);
                OutputFormat = ArgumentHelper.GetEnumValueWithValidation<DeploymentOutputFormat>(ArgumentConstants.OutputFormat, args, i);
                i++;
                break;
            default:
                base.ParseAdditionalArgument(args, ref i);
                break;
        }
    }

    public DeploymentOutputFormat? OutputFormat { get; private set; }
}
