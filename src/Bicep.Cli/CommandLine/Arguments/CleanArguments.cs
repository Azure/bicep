// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Cli.CommandLine.Arguments
{
    public class CleanArguments : ArgumentsBase
    {
        public ImmutableArray<string> BicepDirectories { get; }

        public CleanArguments(IEnumerable<string> arguments)
        {
            this.BicepDirectories = arguments.ToImmutableArray();

            if (!this.BicepDirectories.Any())
            {
                throw new CommandLineException($"At least one directory must be specified to the {CliConstants.CommandClean} command.");
            }
        }

    }
}

