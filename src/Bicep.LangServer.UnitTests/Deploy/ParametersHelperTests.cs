// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Deploy
{
    [TestClass]
    public class ParametersHelperTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataRow(null)]
        [DataRow("")]
        [DataRow("    ")]
        [DataTestMethod]
        public void GetParametersFileContents_WithNullOrEmptyParametersFilePathAndEmptyMissingParams_ReturnsEmptyJObject(string parametersFilePath)
        {
            var result = ParametersHelper.GetParametersFileContents("some_document_path", parametersFilePath, Enumerable.Empty<BicepDeploymentMissingParam>());

            result.Should().Be("{}");
        }

        [TestMethod]
        public void GetParametersFileContents_WithEmptyParametersFilePathAndNonEmptyMissingParams_ReturnsUpdatedJObject()
        {
            var missingParams = new List<BicepDeploymentMissingParam>{ new BicepDeploymentMissingParam("storageAccountName", "stgAcc") };
            var result = ParametersHelper.GetParametersFileContents("some_document_path", string.Empty, missingParams);

            result.Should().BeEquivalentToIgnoringNewlines(@"{
  ""storageAccountName"": {
    ""value"": ""stgAcc""
  }
}");
        }

        [TestMethod]
        public void GetParametersFileContents_WithParametersFilePathAndNonEmptyMissingParams_ReturnsUpdatedJObject()
        {
            var missingParams = new List<BicepDeploymentMissingParam> { new BicepDeploymentMissingParam("storageAccountName", "stgAcc") };
            var parametersFilePath = FileHelper.SaveResultFile(this.TestContext, "parameters.json", @"{
  ""location"": {
    ""value"": ""eastus""
  }
}");
            var result = ParametersHelper.GetParametersFileContents("some_document_path", parametersFilePath, missingParams);

            result.Should().BeEquivalentToIgnoringNewlines(@"{
  ""location"": {
    ""value"": ""eastus""
  },
  ""storageAccountName"": {
    ""value"": ""stgAcc""
  }
}");
        }
    }
}
