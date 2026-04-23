// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class HelpTests : TestBase
    {
        [TestMethod]
        public async Task Root_Help_ShouldSucceed_WithExpectedOutput()
        {
            var (output, error, result) = await Bicep("--help");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().ContainAll(
                    "build",
                    "build-params",
                    "decompile",
                    "decompile-params",
                    "format",
                    "generate-params",
                    "lint",
                    "publish",
                    "restore",
                    "test",
                    "--version",
                    "--license",
                    "--third-party-notices");
            }
        }

        [TestMethod]
        public async Task Build_Help_ShouldSucceed_WithExpectedOutput()
        {
            var (output, error, result) = await Bicep("build", "--help");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().ContainAll(
                    "build",
                    "Builds a .bicep file.",
                    "--stdout",
                    "--no-restore",
                    "--outdir",
                    "--outfile",
                    "--pattern",
                    "--diagnostics-format");
            }
        }

        [TestMethod]
        public async Task Test_Help_ShouldSucceed_WithExpectedOutput()
        {
            var (output, error, result) = await Bicep("test", "--help");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().ContainAll(
                    "test",
                    "Runs tests in a .bicep file.",
                    "--no-restore",
                    "--diagnostics-format");
            }
        }

        [TestMethod]
        public async Task BuildParams_Help_ShouldSucceed_WithExpectedOutput()
        {
            var (output, error, result) = await Bicep("build-params", "--help");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().ContainAll(
                    "build-params",
                    "Builds a .json file from a .bicepparam file.",
                    "--stdout",
                    "--no-restore",
                    "--outdir",
                    "--outfile",
                    "--pattern",
                    "--bicep-file",
                    "--diagnostics-format");
            }
        }

        [TestMethod]
        public async Task Format_Help_ShouldSucceed_WithExpectedOutput()
        {
            var (output, error, result) = await Bicep("format", "--help");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().ContainAll(
                    "format",
                    "Formats a .bicep file.",
                    "--stdout",
                    "--outdir",
                    "--outfile",
                    "--pattern",
                    "--newline-kind",
                    "--indent-kind",
                    "--indent-size",
                    "--insert-final-newline");
            }
        }

        [TestMethod]
        public async Task GenerateParams_Help_ShouldSucceed_WithExpectedOutput()
        {
            var (output, error, result) = await Bicep("generate-params", "--help");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().ContainAll(
                    "generate-params",
                    "Builds parameters file",
                    "--stdout",
                    "--no-restore",
                    "--outdir",
                    "--outfile",
                    "--output-format",
                    "--include-params");
            }
        }

        [TestMethod]
        public async Task Decompile_Help_ShouldSucceed_WithExpectedOutput()
        {
            var (output, error, result) = await Bicep("decompile", "--help");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().ContainAll(
                    "decompile",
                    "Attempts to decompile a template .json file to .bicep.",
                    "--stdout",
                    "--force",
                    "--outdir",
                    "--outfile");
            }
        }

        [TestMethod]
        public async Task DecompileParams_Help_ShouldSucceed_WithExpectedOutput()
        {
            var (output, error, result) = await Bicep("decompile-params", "--help");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().ContainAll(
                    "decompile-params",
                    "Attempts to decompile a parameters .json file to .bicepparam.",
                    "--stdout",
                    "--force",
                    "--outdir",
                    "--outfile",
                    "--bicep-file");
            }
        }

        [TestMethod]
        public async Task Publish_Help_ShouldSucceed_WithExpectedOutput()
        {
            var (output, error, result) = await Bicep("publish", "--help");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().ContainAll(
                    "publish",
                    "Publishes the .bicep file to the module registry.",
                    "--target",
                    "--documentation-uri",
                    "--no-restore",
                    "--force",
                    "--with-source");
            }
        }

        [TestMethod]
        public async Task PublishExtension_Help_ShouldSucceed_WithExpectedOutput()
        {
            var (output, error, result) = await Bicep("publish-extension", "--help");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().ContainAll(
                    "publish-extension",
                    "Publishes a Bicep extension to a registry.",
                    "--target",
                    "--force",
                    "--bin-linux-x64",
                    "--bin-win-x64");
            }
        }

        [TestMethod]
        public async Task Restore_Help_ShouldSucceed_WithExpectedOutput()
        {
            var (output, error, result) = await Bicep("restore", "--help");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().ContainAll(
                    "restore",
                    "Restores external modules",
                    "--pattern",
                    "--force");
            }
        }

        [TestMethod]
        public async Task Lint_Help_ShouldSucceed_WithExpectedOutput()
        {
            var (output, error, result) = await Bicep("lint", "--help");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().ContainAll(
                    "lint",
                    "Lints a .bicep file.",
                    "--pattern",
                    "--no-restore",
                    "--diagnostics-format");
            }
        }

        [TestMethod]
        public async Task JsonRpc_Help_ShouldSucceed_WithExpectedOutput()
        {
            var (output, error, result) = await Bicep("jsonrpc", "--help");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().ContainAll(
                    "jsonrpc",
                    "JSONRPC",
                    "--pipe",
                    "--socket",
                    "--stdio");
            }
        }

        [TestMethod]
        public async Task LocalDeploy_Help_ShouldSucceed_WithExpectedOutput()
        {
            var (output, error, result) = await Bicep("local-deploy", "--help");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().ContainAll(
                    "local-deploy",
                    "local deployment",
                    "--no-restore",
                    "--format");
            }
        }

        [TestMethod]
        public async Task Snapshot_Help_ShouldSucceed_WithExpectedOutput()
        {
            var (output, error, result) = await Bicep("snapshot", "--help");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().ContainAll(
                    "snapshot",
                    "deployment snapshot",
                    "--mode",
                    "--tenant-id",
                    "--subscription-id",
                    "--location",
                    "--resource-group",
                    "--deployment-name");
            }
        }

        [TestMethod]
        public async Task Deploy_Help_ShouldSucceed_WithExpectedOutput()
        {
            var (output, error, result) = await Bicep("deploy", "--help");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().ContainAll(
                    "deploy",
                    "Deploys infrastructure",
                    "--no-restore",
                    "--format");
            }
        }

        [TestMethod]
        public async Task WhatIf_Help_ShouldSucceed_WithExpectedOutput()
        {
            var (output, error, result) = await Bicep("what-if", "--help");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().ContainAll(
                    "what-if",
                    "Previews the changes",
                    "--no-restore");
            }
        }

        [TestMethod]
        public async Task Teardown_Help_ShouldSucceed_WithExpectedOutput()
        {
            var (output, error, result) = await Bicep("teardown", "--help");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().ContainAll(
                    "teardown",
                    "Tears down resources",
                    "--no-restore");
            }
        }

        [TestMethod]
        public async Task Console_Help_ShouldSucceed_WithExpectedOutput()
        {
            var (output, error, result) = await Bicep("console", "--help");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().ContainAll(
                    "console",
                    "Opens an interactive Bicep console.");
            }
        }
    }
}
