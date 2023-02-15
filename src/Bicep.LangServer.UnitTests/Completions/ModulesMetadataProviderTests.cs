// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Bicep.LanguageServer.Providers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.UnitTests.Completions
{
    [TestClass]
    public class ModulesMetadataProviderTests
    {
        private static ModulesMetadataProvider modulesMetadataProvider = new ModulesMetadataProvider();

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            await modulesMetadataProvider.Initialize();
        }

        [TestMethod]
        public void GetModuleNames_ShouldReturnAllModuleNames()
        {
            IEnumerable<string> moduleNames = modulesMetadataProvider.GetModuleNames();

            moduleNames.Should().NotBeEmpty();
            moduleNames.Should().Contain("app/dapr-containerapp");
            moduleNames.Should().Contain("app/dapr-containerapps-environment");
        }

        [DataRow("")]
        [DataRow("     ")]
        [DataRow("invalid_name")]
        [DataRow(null)]
        [DataTestMethod]
        public void GetVersions_WithInvalidModuleName_ShouldReturnEmptyList(string moduleName)
        {
            modulesMetadataProvider.GetVersions(moduleName).Should().BeEmpty();
        }

        [TestMethod]
        public void GetVersions_WithValidModuleName_ShouldVersions()
        {
            IEnumerable<string> versions = modulesMetadataProvider.GetVersions("app/dapr-containerapp");

            versions.Should().NotBeEmpty();
            versions.Should().Contain("1.0.1");
            versions.Should().Contain("1.0.2");
        }
    }
}
