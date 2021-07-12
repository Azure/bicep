// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;

namespace Bicep.Cli.Arguments
{
    public class RootArguments : ArgumentsBase
    {
        public RootArguments(string arg, string commandName) : base(commandName)
        {
            switch (arg)
            {
                case var a when new Regex(Constants.Argument.VersionRegex).IsMatch(a):
                    PrintVersion = true;
                    break;

                case var a when new Regex(Constants.Argument.HelpRegex).IsMatch(a):
                    PrintHelp = true;
                    break;
            };
        }

        public bool PrintHelp { get; }
        public bool PrintVersion { get; }
    }
}
