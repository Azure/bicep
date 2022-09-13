// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;

namespace Bicep.Cli.Arguments
{
    public class RootArguments : ArgumentsBase
    {
        public RootArguments(string arg, string commandName, string[] additionalArgs) : base(commandName)
        {
            switch (arg)
            {
                case var a when new Regex(Constants.Argument.VersionRegex).IsMatch(a):
                    PrintVersion = true;
                    break;

                case var a when new Regex(Constants.Argument.HelpRegex).IsMatch(a):
                    PrintHelp = true;
                    break;

                case var a when new Regex(Constants.Argument.LicenseRegex).IsMatch(a):
                    PrintLicense = true;
                    break;

                case var a when new Regex(Constants.Argument.ThirdPartyNoticesRegex).IsMatch(a):
                    PrintThirdPartyNotices = true;
                    break;
            };

            for (var i = 0; i < additionalArgs.Length; i++)
            {
                switch (additionalArgs[i].ToLowerInvariant())
                {
                    case string maybeFeatureArg when IsFeatureArg(maybeFeatureArg):
                        i += HandleFeatureArg(additionalArgs, i);
                        break;

                    default:
                        if (additionalArgs[i].StartsWith("--"))
                        {
                            throw new CommandLineException($"Unrecognized parameter \"{additionalArgs[i]}\"");
                        }
                        break;
                }
            }
        }

        public bool PrintHelp { get; }
        public bool PrintVersion { get; }
        public bool PrintLicense { get; }
        public bool PrintThirdPartyNotices { get; }
    }
}
