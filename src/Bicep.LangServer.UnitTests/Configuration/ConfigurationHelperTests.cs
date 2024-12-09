// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Configuration;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.FileSystem;
using Bicep.LanguageServer.Configuration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using ConfigurationManager = Bicep.Core.Configuration.ConfigurationManager;
using LocalFileSystem = System.IO.Abstractions.FileSystem;

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
    }
}
