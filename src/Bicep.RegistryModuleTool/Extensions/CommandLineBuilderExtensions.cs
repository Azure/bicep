// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Options;
using System.CommandLine.Builder;
using System.Linq;

namespace Bicep.RegistryModuleTool.Extensions
{
    public static class CommandLineBuilderExtensions
    {
        public static CommandLineBuilder UseVerboseOption(this CommandLineBuilder builder)
        {
            if (builder.Command.Children.Any(x => x is VerboseOption))
            {
                return builder;
            }

            builder.AddGlobalOption(GlobalOptions.Verbose);

            return builder;
        }
    }
}
