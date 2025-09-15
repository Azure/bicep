// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments;

public class ConsoleArguments : ArgumentsBase
{
    public ConsoleArguments(string[] args) : base(Constants.Command.Console)
    {
        // Currently no options. Future flags (e.g. --subscription, --resource-group) can be added.
    }
}
