// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Registry;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class TestFrameworkCommandTests : TestBase
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task Test_ZeroFiles_ShouldFail_WithExpectedErrorMessage()
        {
            var (output, error, result) = await Bicep("test");

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain($"The input file path was not specified");
            }
        }

        [TestMethod]
        public async Task Test_NonBicepFiles_ShouldFail_WithExpectedErrorMessage()
        {
            var settings = new InvocationSettings(new(TestContext, TestFrameworkEnabled: true), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);

            // var (output, error, result) = await Bicep(settings, "test");
            var (output, error, result) = await Bicep(settings, "test", "/dev/zero");

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain($@"The specified input ""/dev/zero"" was not recognized as a Bicep file. Bicep files must use the {LanguageConstants.LanguageFileExtension} extension.");
            }
        }

        [TestMethod]
        public async Task Test_WithoutTestFrameworkEnabled_ShouldFail()
        {
            var settings = new InvocationSettings(new(TestContext, TestFrameworkEnabled: false), BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory);

            // var (output, error, result) = await Bicep(settings, "test");
            var (output, error, result) = await Bicep(settings, "test", "/dev/zero");

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain("TestFrameWork not enabled");
            }
        }
    }
}
