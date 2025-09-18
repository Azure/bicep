// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Constants
{
    public static class Command
    {
        public const string Build = "build";
        public const string Test = "test";
        public const string BuildParams = "build-params";
        public const string Format = "format";
        public const string JsonRpc = "jsonrpc";
        public const string LocalDeploy = "local-deploy";
        public const string GenerateParamsFile = "generate-params";
        public const string Decompile = "decompile";
        public const string DecompileParams = "decompile-params";
        public const string Publish = "publish";
        public const string PublishExtension = "publish-extension";
        public const string Restore = "restore";
        public const string Lint = "lint";
        public const string Snapshot = "snapshot";
        public const string Deploy = "deploy";
        public const string WhatIf = "what-if";
        public const string Teardown = "teardown";
        public const string Root = "";
    }

    public static class Argument
    {
        public const string VersionRegex = @"^(--version|-v)$";
        public const string HelpRegex = @"^(--help|-h)$";
        public const string LicenseRegex = @"^--license$";
        public const string ThirdPartyNoticesRegex = @"^--third-party-notices$";
    }
}
