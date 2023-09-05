// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.RegistryModuleTool.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.IO;
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

            builder.Command.AddGlobalOption(GlobalOptions.Verbose);

            return builder;
        }
    }
}
