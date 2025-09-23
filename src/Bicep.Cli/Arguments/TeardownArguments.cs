// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments;

public class TeardownArguments : DeployArgumentsBase
{
    public TeardownArguments(string[] args) : base(args, Constants.Command.Teardown)
    {
    }
}
