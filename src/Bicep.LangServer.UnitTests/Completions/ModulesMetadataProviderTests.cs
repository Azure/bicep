// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.LanguageServer.Completions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Bicep.LanguageServer.Providers;
using SharpYaml;

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
            List<CompletionItem> moduleNames = modulesMetadataProvider.GetModuleNames();

            moduleNames.Should().NotBeEmpty();
            moduleNames.Should().Contain(x => x.Label == "app/dapr-containerapp");
            moduleNames.Should().Contain(x => x.Label == "app/dapr-containerapps-environment");
        }

        [DataRow("")]
        [DataRow("     ")]
        [DataRow("invalid_name")]
        [DataRow(null)]
        [DataTestMethod]
        public void GetTags_WithInvalidModuleName_ShouldReturnEmptyList(string moduleName)
        {
            List<CompletionItem> versions = modulesMetadataProvider.GetTags(moduleName);

            versions.Should().BeEmpty();
        }

        [TestMethod]
        public void GetTags_WithValidModuleName_ShouldVersions()
        {
            List<CompletionItem> versions = modulesMetadataProvider.GetTags("app/dapr-containerapp");

            versions.Should().NotBeEmpty();
            versions.Should().Contain(x => x.Label == "1.0.1");
            versions.Should().Contain(x => x.Label == "1.0.2");
        }
    }
}
