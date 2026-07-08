// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Bicep.RegistryModuleTool.Options;

namespace Bicep.RegistryModuleTool.Extensions
{
    public static class CommandLineBuilderExtensions
    {
        /// <summary>Adds <see cref="GlobalOptions.Verbose"/> as a global option on the root command.</summary>
        public static Command UseVerboseOption(this Command command)
        {
            if (!command.Options.Any(x => x is VerboseOption))
            {
                command.Add(GlobalOptions.Verbose);
            }

            return command;
        }
    }
}
