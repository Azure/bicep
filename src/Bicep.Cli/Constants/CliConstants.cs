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
        public const string Console = "console";
        public const string Root = "";
    }

    public static class Argument
    {
        public const string InputFile = "Input file";
        public const string IndexFile = "Index file";
        public const string ParametersFile = "Parameters file";
    }

    public static class Option
    {
        // Root options
        public const string Version = "--version";
        public const string VersionShort = "-v";
        public const string License = "--license";
        public const string ThirdPartyNotices = "--third-party-notices";

        // Common output options
        public const string Stdout = "--stdout";
        public const string OutDir = "--outdir";
        public const string OutFile = "--outfile";
        public const string Pattern = "--pattern";
        public const string NoRestore = "--no-restore";
        public const string Force = "--force";
        public const string DiagnosticsFormat = "--diagnostics-format";

        // Build / BuildParams
        public const string BicepFile = "--bicep-file";

        // GenerateParams
        public const string OutputFormat = "--output-format";
        public const string IncludeParams = "--include-params";

        // Format
        public const string NewlineKind = "--newline-kind";
        public const string IndentKind = "--indent-kind";
        public const string IndentSize = "--indent-size";
        public const string InsertFinalNewline = "--insert-final-newline";

        // Publish
        public const string Target = "--target";
        public const string DocumentationUri = "--documentation-uri";
        public const string WithSource = "--with-source";

        // PublishExtension binaries
        public const string BinLinuxX64 = "--bin-linux-x64";
        public const string BinLinuxArm64 = "--bin-linux-arm64";
        public const string BinOsxX64 = "--bin-osx-x64";
        public const string BinOsxArm64 = "--bin-osx-arm64";
        public const string BinWinX64 = "--bin-win-x64";
        public const string BinWinArm64 = "--bin-win-arm64";

        // JsonRpc
        public const string Pipe = "--pipe";
        public const string Socket = "--socket";
        public const string Stdio = "--stdio";

        // Deploy / local-deploy / what-if / teardown
        public const string Format = "--format";

        // Snapshot
        public const string Mode = "--mode";
        public const string TenantId = "--tenant-id";
        public const string SubscriptionId = "--subscription-id";
        public const string Location = "--location";
        public const string ResourceGroup = "--resource-group";
        public const string DeploymentName = "--deployment-name";
    }
}
