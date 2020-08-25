// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Cli.CommandLine.Arguments
{
    public class BuildArguments : ArgumentsBase
    {
        private static bool IsStdOutArgument(string argument)
            => StringComparer.OrdinalIgnoreCase.Equals(argument, CliConstants.ArgumentStdOut);

        public BuildArguments(IEnumerable<string> arguments)
        {
            this.OutputToStdOut = arguments.Where(IsStdOutArgument).Any();
            this.Files = arguments.Where(f => !IsStdOutArgument(f)).ToImmutableArray();

            if (this.Files.Any() == false)
            {
                throw new CommandLineException($"At least one file must be specified to the {CliConstants.CommandBuild} command.");
            }
        }

        public ImmutableArray<string> Files { get; }

        public bool OutputToStdOut {private set; get;}
    }
}

