// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class RootCommandTests : TestBase
    {
        [TestMethod]
        public void Build_WithWrongArgs_ShouldFail_WithExpectedErrorMessage()
        {
            var (output, error, result) = Bicep("wrong", "fake", "broken");

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain($"Unrecognized arguments \"wrong fake broken\" specified. Use \"bicep --help\" to view available options.");
            }
        }

        [TestMethod]
        public void BicepVersionShouldPrintVersionInformation()
        {

            var (output, error, result) = Bicep("--version");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();

                output.Should().NotBeEmpty();
                output.Should().StartWith("Bicep CLI version");
            }
        }

        [TestMethod]
        public void BicepHelpShouldPrintHelp()
        {
            var (output, error, result) = Bicep("--help");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();

                output.Should().NotBeEmpty();
                output.Should().ContainAll(
                    "build",
                    "[options]",
                    "<file>",
                    ".bicep",
                    "Arguments:",
                    "Options:",
                    "--outdir",
                    "--outfile",
                    "--stdout",
                    "--version",
                    "--help",
                    "information",
                    "version",
                    "bicep",
                    "usage");
            }
        }
    }
}

