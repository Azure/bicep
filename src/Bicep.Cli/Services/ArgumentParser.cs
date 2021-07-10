// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using System.Text.RegularExpressions;

namespace Bicep.Cli.Services
{
    public static class ArgumentParser
    {
        public static ArgumentsBase? TryParse(string[] args)
        {
            if (args.Length < 1)
            {
                return null;
            }

            // parse root arguments
            if (new Regex(Constants.Argument.VersionRegex).IsMatch(args[0]) || new Regex(Constants.Argument.HelpRegex).IsMatch(args[0]))
            {
                return new RootArguments(args[0], Constants.Command.Root);
            }    

            // parse verb
            return (args[0].ToLowerInvariant()) switch
            {
                Constants.Command.Build => new BuildArguments(args[1..], Constants.Command.Build),
                Constants.Command.Decompile => new DecompileArguments(args[1..], Constants.Command.Decompile),
                _ => null,
            };
        }
    }
}

