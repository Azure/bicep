// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.Text.RegularExpressions;
using Bicep.Cli.Arguments;

namespace Bicep.Cli.Services
{
    public static class ArgumentParser
    {
        public static ArgumentsBase? TryParse(string[] args, IFileSystem fileSystem)
        {
            if (args.Length < 1)
            {
                return null;
            }

            // parse root arguments
            if (new Regex(Constants.Argument.VersionRegex).IsMatch(args[0]) ||
                new Regex(Constants.Argument.HelpRegex).IsMatch(args[0]) ||
                new Regex(Constants.Argument.LicenseRegex).IsMatch(args[0]) ||
                new Regex(Constants.Argument.ThirdPartyNoticesRegex).IsMatch(args[0]))
            {
                return new RootArguments(args[0], Constants.Command.Root);
            }

            // parse verb
            return (args[0].ToLowerInvariant()) switch
            {
                Constants.Command.Build => new BuildArguments(args[1..]),
                Constants.Command.Test => new TestArguments(args[1..]),
                Constants.Command.BuildParams => new BuildParamsArguments(args[1..]),
                Constants.Command.Format => new FormatArguments(args[1..]),
                Constants.Command.GenerateParamsFile => new GenerateParametersFileArguments(args[1..]),
                Constants.Command.Decompile => new DecompileArguments(args[1..]),
                Constants.Command.DecompileParams => new DecompileParamsArguments(args[1..]),
                Constants.Command.PublishExtension => new PublishExtensionArguments(args[1..]),
                Constants.Command.Publish => new PublishArguments(args[1..]),
                Constants.Command.Restore => new RestoreArguments(args[1..]),
                Constants.Command.Lint => new LintArguments(args[1..]),
                Constants.Command.JsonRpc => new JsonRpcArguments(args[1..]),
                Constants.Command.LocalDeploy => new LocalDeployArguments(args[1..]),
                Constants.Command.Snapshot => new SnapshotArguments(args[1..]),
                Constants.Command.Deploy => new DeployArguments(args[1..]),
                Constants.Command.WhatIf => new WhatIfArguments(args[1..]),
                Constants.Command.Teardown => new TeardownArguments(args[1..]),
                Constants.Command.Console => new ConsoleArguments(args[1..]),
                _ => null,
            };
        }
    }
}
