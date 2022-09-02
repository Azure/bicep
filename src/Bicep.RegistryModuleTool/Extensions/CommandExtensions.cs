// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.CommandLine;

namespace Bicep.RegistryModuleTool.Extensions
{
    public static class CommandExtensions
    {
        public static Command AddSubcommand(this Command command, Command subcommand)
        {
            command.AddCommand(subcommand);

            return command;
        }

        public static Command Configure(this Command command, Action<Command> configure)
        {
            configure(command);

            return command;
        }
    }
}
