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
        public async Task GetModuleNames_ShouldReturnAllModuleNames()
        {
            IEnumerable<string> moduleNames = await modulesMetadataProvider.GetModuleNames();

            moduleNames.Should().NotBeEmpty();
            moduleNames.Should().Contain("app/dapr-containerapp");
            moduleNames.Should().Contain("app/dapr-containerapps-environment");
        }

        [DataRow("")]
        [DataRow("     ")]
        [DataRow("invalid_name")]
        [DataRow(null)]
        [DataTestMethod]
        public async Task GetVersions_WithInvalidModuleName_ShouldReturnNothing(string moduleName)
        {
            var versions = await modulesMetadataProvider.GetVersions(moduleName);
            versions.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetVersions_WithValidModuleName_ShouldReturnVersions()
        {
            IEnumerable<string> versions = await modulesMetadataProvider.GetVersions("app/dapr-containerapp");

            versions.Should().NotBeEmpty();
            versions.Should().Contain("1.0.1");
            versions.Should().Contain("1.0.2");
        }
    }
}
