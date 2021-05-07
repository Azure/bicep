// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Cli.CommandLine.Arguments
{
    public abstract class ArgumentsBase
    {
        public string CommandName { get; }

        protected ArgumentsBase(string commandName)
        {
            CommandName = commandName;
        }
    }
}
