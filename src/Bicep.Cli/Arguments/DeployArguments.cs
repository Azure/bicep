// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Helpers;

namespace Bicep.Cli.Arguments;

public class DeployArguments : DeployArgumentsBase
{
    public DeployArguments(string[] args) : base(args, Constants.Command.Deploy)
    {
    }
}
