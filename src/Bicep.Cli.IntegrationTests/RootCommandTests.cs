// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class RootCommandTests : TestBase
    {
        [TestMethod]
        public async Task Build_WithWrongArgs_ShouldFail_WithExpectedErrorMessage()
        {
            var (output, error, result) = await Bicep("wrong", "fake", "broken");

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain($"Unrecognized arguments \"wrong fake broken\" specified. Use \"bicep --help\" to view available options.");
            }
        }

        [TestMethod]
        public async Task BicepVersionShouldPrintVersionInformation()
        {
            var (output, error, result) = await Bicep("--version");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();

                output.Should().NotBeEmpty();
                output.Should().StartWith("Bicep CLI version");
            }
        }

        [TestMethod]
        public async Task BicepHelpShouldPrintHelp()
        {
            var featuresMock = Repository.Create<IFeatureProvider>();
            featuresMock.Setup(m => m.RegistryEnabled).Returns(true);

            var settings = CreateDefaultSettings() with { Features = featuresMock.Object };

            var (output, error, result) = await Bicep(settings, "--help");

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
                    "usage",
                    "--license",
                    "--third-party-notices",
                    "license information",
                    "third-party notices");
            }
        }

        [TestMethod]
        public async Task BicepLicenseShouldPrintLicense()
        {
            var (output, error, result) = await Bicep("--license");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();

                output.Should().NotBeEmpty();
                output.Should().ContainAll(
                    "MIT License",
                    "Copyright (c) Microsoft Corporation.",
                    "Permission is hereby granted",
                    "The above copyright notice",
                    "THE SOFTWARE IS PROVIDED \"AS IS\"");
            }
        }

        [TestMethod]
        public async Task BicepThirdPartyNoticesShouldPrintNotices()
        {
            var (output, error, result) = await Bicep("--third-party-notices");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();

                output.Should().NotBeEmpty();
                output.Should().ContainAll(
                    "MIT License",
                    "Copyright (c) Microsoft Corporation.",
                    "---------------------------------------------------------",
                    "Copyright (c) .NET Foundation and Contributors",
                    "THE SOFTWARE IS PROVIDED \"AS IS\"",
                    "MIT");

                // the notice file should be long
                output.Length.Should().BeGreaterThan(100000);
            }
        }

        [TestMethod]
        public async Task BicepHelpShouldAlwaysIncludePublish()
        {
            // disable registry to ensure `bicep --help` is not consulting the feature provider before
            // preparing the help text (as features can only be determined when an input file is specified)
            var featuresMock = Repository.Create<IFeatureProvider>();
            featuresMock.Setup(m => m.RegistryEnabled).Returns(false);

            var settings = CreateDefaultSettings() with { Features = featuresMock.Object };

            var (output, error, result) = await Bicep(settings, "--help");

            result.Should().Be(0);
            error.Should().BeEmpty();

            output.Should().NotBeEmpty();
            output.Should().ContainAll(
                "publish",
                "Publishes",
                "registry",
                "reference",
                "azurecr.io",
                "br",
                "--target");
        }
    }
}
