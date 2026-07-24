// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.Options;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.RegistryModuleTool.UnitTests.Extensions
{
    [TestClass]
    public class CommandLineBuilderExtensionsTests
    {
        [TestMethod]
        public void UseVerboseOption_AddsVerboseOptionWithDescription()
        {
            var command = new RootCommand();

            command.UseVerboseOption();

            command.Options.Should().ContainSingle(option => option is VerboseOption)
                .Which.Description.Should().Be("Show verbose information");
        }

        [TestMethod]
        public void UseVerboseOption_DoesNotAddDuplicates()
        {
            var command = new RootCommand();

            command.UseVerboseOption();
            command.UseVerboseOption();

            command.Options.Should().ContainSingle(option => option is VerboseOption);
        }
    }
}
