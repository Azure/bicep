// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.Runtime.InteropServices;
using Bicep.Cli.Helpers;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
        public async Task BicepVersionShouldPrintGitCommitShaAndNewerInstallations()
        {
            const string currentCommitSha = "1111111111111111111111111111111111111111";
            const string newerCommitSha = "2222222222222222222222222222222222222222";

            var environmentMock = new Mock<IEnvironment>(MockBehavior.Strict);
            environmentMock.Setup(environment => environment.CurrentVersion).Returns(new IEnvironment.BicepVersionInfo("0.30.23", currentCommitSha));
            environmentMock.Setup(environment => environment.CurrentPlatform).Returns(OSPlatform.Linux);
            environmentMock.Setup(environment => environment.OperatingSystemVersion).Returns("Test OS 1.2.3");
            environmentMock.Setup(environment => environment.OperatingSystemArchitecture).Returns(Architecture.X64);
            environmentMock.Setup(environment => environment.CurrentArchitecture).Returns(Architecture.X64);

            var settings = new InvocationSettings(Environment: environmentMock.Object);
            var newerVersions = new[]
            {
                new BicepInstallationVersion(new Version("0.40.0"), newerCommitSha, "/usr/local/bin/bicep"),
            };

            var (output, error, result) = await Bicep(
                settings,
                services => services.AddSingleton<VersionChecker>(new StaticVersionChecker(environmentMock.Object, newerVersions)),
                CancellationToken.None,
                "--version");

            using (new AssertionScope())
            {
                result.Should().Be(0);
                error.Should().BeEmpty();
                output.Should().ContainAll(
                    $"Bicep CLI version 0.30.23 ({currentCommitSha})",
                    "OS: Linux",
                    "OS version: Test OS 1.2.3",
                    "Architecture: X64",
                    $"Git commit SHA: {currentCommitSha}",
                    "Newer Bicep CLI installation(s) found:",
                    $"Version 0.40.0 (Git commit SHA: {newerCommitSha}) at /usr/local/bin/bicep");
            }
        }

        [TestMethod]
        public async Task BicepHelpShouldPrintHelp()
        {
            var settings = new InvocationSettings() { FeatureOverrides = new(RegistryEnabled: true) };

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
                    "--diagnostics-format",
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
                    "(c) Microsoft Corporation.",
                    "---------------------------------------------------------",
                    "(c) .NET Foundation and Contributors",
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
            var settings = new InvocationSettings() { FeatureOverrides = new(RegistryEnabled: false) };

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

        private class StaticVersionChecker(IEnvironment environment, IReadOnlyList<BicepInstallationVersion> newerVersions) : VersionChecker(environment, new FileSystem())
        {
            public override IReadOnlyList<BicepInstallationVersion> FindNewerVersions(CancellationToken cancellationToken = default)
                => newerVersions;
        }
    }
}
