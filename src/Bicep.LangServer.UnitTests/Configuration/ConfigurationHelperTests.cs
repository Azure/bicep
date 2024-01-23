// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Configuration;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Configuration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using ConfigurationManager = Bicep.Core.Configuration.ConfigurationManager;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.LangServer.UnitTests.Configuration
{
    [TestClass]
    public class ConfigurationHelperTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void IsBicepConfigFile_WithDocumentUriCorrespondingToNonBicepConfigFile_ShouldReturnFalse()
        {
            var documentUri = DocumentUri.FromFileSystemPath("/path/to/main.bicep");

            ConfigurationHelper.IsBicepConfigFile(documentUri).Should().Be(false);
        }

        [TestMethod]
        public void IsBicepConfigFile_WithDocumentUriCorrespondingToBicepConfigFile_ShouldReturnTrue()
        {
            var documentUri = DocumentUri.FromFileSystemPath("/path/to/bicepconfig.json");

            ConfigurationHelper.IsBicepConfigFile(documentUri).Should().Be(true);
        }

        [TestMethod]
        public void TryGetConfiguration_WithValidDocumentUri_ShoudReturnRootConfiguration()
        {
            var testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);

            var bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}";

            var bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigFileContents, testOutputPath);
            var bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath).ToUriEncoded();

            bool result = ConfigurationHelper.TryGetConfiguration(new ConfigurationManager(new IOFileSystem()), bicepConfigUri, out RootConfiguration? rootConfiguration);

            result.Should().Be(true);
            rootConfiguration.Should().NotBeNull();
        }
    }
}
