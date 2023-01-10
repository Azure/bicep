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

namespace Bicep.LangServer.UnitTests.Providers
{
    [TestClass]
    public class McrCompletionProviderTests
    {
        private static McrCompletionProvider mcrCompletionProvider = new McrCompletionProvider();

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            await mcrCompletionProvider.Initialize();
        }

        [TestMethod]
        public void GetModuleNames_ShouldReturnAllModuleNames()
        {
            List<CompletionItem> moduleNames = mcrCompletionProvider.GetModuleNames();

            moduleNames.Should().NotBeEmpty();
            moduleNames.Should().Contain(x => x.Label == "app/dapr-containerapp");
            moduleNames.Should().Contain(x => x.Label == "app/dapr-containerapps-environment");
        }

        [DataRow("")]
        [DataRow("     ")]
        [DataRow("invalid_name")]
        [DataRow(null)]
        [DataTestMethod]
        public void GetTags_WithInvalidModuleName_ShouldReturnEmptyListOfCompletionItems(string moduleName)
        {
            List<CompletionItem> versions = mcrCompletionProvider.GetVersions(moduleName);

            versions.Should().BeEmpty();
        }

        [TestMethod]
        public void GetTags_WithValidModuleName_ShouldVersions()
        {
            List<CompletionItem> versions = mcrCompletionProvider.GetVersions("app/dapr-containerapp");

            versions.Should().NotBeEmpty();
            versions.Should().Contain(x => x.Label == "1.0.1");
            versions.Should().Contain(x => x.Label == "1.0.2");
        }


        [DataRow("")]
        [DataRow("     ")]
        [DataRow("invalid_path")]
        [DataRow(null)]
        [DataTestMethod]
        public void GetReadmeLink_WithInvalidModulePath_ShouldReturnNull(string modulePath)
        {
            string? readmeLink = mcrCompletionProvider.GetReadmeLink(modulePath);

            readmeLink.Should().BeNull();
        }

        [DataRow("br/public:app/dapr-containerapp:1.0.1")]
        [DataRow("br:mcr.microsoft.com/bicep/app/dapr-containerapp:1.0.1")]
        [DataTestMethod]
        public void GetReadmeLink_WithValidModulePath_ShouldReturnReadmeLink(string modulePath)
        {
            string? readmeLink = mcrCompletionProvider.GetReadmeLink(modulePath);

            readmeLink.Should().NotBeNull();
        }
    }
}
