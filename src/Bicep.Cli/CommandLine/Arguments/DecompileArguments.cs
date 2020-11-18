// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Cli.CommandLine.Arguments
{
    public class DecompileArguments : ArgumentsBase
    {
        public DecompileArguments(IEnumerable<string> arguments)
        {
            this.Files = arguments.ToImmutableArray();

            if (!this.Files.Any())
            {
                throw new CommandLineException($"At least one file must be specified to the {CliConstants.CommandDecompile} command.");
            }
        }

        public ImmutableArray<string> Files { get; }
    }
}