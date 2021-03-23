// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
namespace Bicep.Cli.CommandLine.Arguments
{
    public class DecompileArguments : ArgumentsBase
    {
        public DecompileArguments(string[] args)
        {
            this.InputFiles = new List<string>();
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    throw new CommandLineException($"Unrecognized parameter \"{args[i]}\"");
                }
                this.InputFiles.Add(args[i]);
            }

            if (this.InputFiles.Count == 0)
            {
                throw new CommandLineException($"The input file path was not specified");
            }
        }

        public List<string> InputFiles { get; }
    }
}