// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Constants
{
    public static class Command
    {
        public const string Build = "build";
        public const string Decompile = "decompile";
        public const string Root = "";
    }

    public static class Argument
    {
        public const string VersionRegex = @"^(--version|-v)$";
        public const string HelpRegex = @"^(--help|-h)$";
    }
}
