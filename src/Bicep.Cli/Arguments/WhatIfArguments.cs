// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments;

public class WhatIfArguments : DeployArgumentsBase
{
    public WhatIfArguments(string[] args) : base(args, Constants.Command.WhatIf)
    {
    }
}
