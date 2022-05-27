// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Deploy
{
    [TestClass]
    public class DeploymentParametersHelperTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void GetUpdatedParametersFileContents_WithEmptyBicepUpdatedDeploymentParameters_ShouldReturnParametersFileContentsAsIs()
        {
            var parametersFileContents = @"{
  ""location"": {
    ""value"": ""westus""
  }
}";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty);
            var parametersFilePath = FileHelper.SaveResultFile(TestContext, "parameters.json", parametersFileContents);

            var result = DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "parameters.json",
                parametersFilePath,
                true,
                false,
                new List<BicepUpdatedDeploymentParameter>());

            result.Should().Be(parametersFileContents);
        }

        [TestMethod]
        public void GetUpdatedParametersFileContents_WithNonEmptyBicepUpdatedDeploymentParameters_ShouldReturnUpdatedParametersFileContents()
        {
            var parametersFileContents = @"{
  ""location"": {
    ""value"": ""westus""
  },
  ""name"": {
    ""value"": ""test""
  },
}";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty);
            var parametersFilePath = FileHelper.SaveResultFile(TestContext, "parameters.json", parametersFileContents);
            var bicepUpdatedDeploymentParameter1 = new BicepUpdatedDeploymentParameter("location", "eastus");
            var bicepUpdatedDeploymentParameter2 = new BicepUpdatedDeploymentParameter("sku", "testSku");
            var bicepUpdatedDeploymentParameters =
                new List<BicepUpdatedDeploymentParameter> { bicepUpdatedDeploymentParameter1, bicepUpdatedDeploymentParameter2 };

            var result = DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "parameters.json",
                parametersFilePath,
                true,
                false,
                bicepUpdatedDeploymentParameters);
            var expected = @"{
  ""location"": {
    ""value"": ""eastus""
  },
  ""name"": {
    ""value"": ""test""
  },
  ""sku"": {
    ""value"": ""testSku""
  }
}";

            result.Should().Be(expected);
        }

        [TestMethod]
        public void GetUpdatedParametersFileContents_WithNonEmptyBicepUpdatedDeploymentParametersAndNoParametersFile_ShouldCreateParametersFile()
        {
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty, testOutputPath);
            var bicepUpdatedDeploymentParameter1 = new BicepUpdatedDeploymentParameter("location", "eastus");
            var bicepUpdatedDeploymentParameter2 = new BicepUpdatedDeploymentParameter("sku", "testSku");
            var bicepUpdatedDeploymentParameters =
                new List<BicepUpdatedDeploymentParameter> { bicepUpdatedDeploymentParameter1, bicepUpdatedDeploymentParameter2 };

            var result = DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "input.parameters.json",
                string.Empty,
                false,
                true,
                bicepUpdatedDeploymentParameters);
            var expectedParametersFileContents = @"{
  ""location"": {
    ""value"": ""eastus""
  },
  ""sku"": {
    ""value"": ""testSku""
  }
}";

            result.Should().Be(expectedParametersFileContents);

            var expectedParametersFilePath = Path.Combine(testOutputPath, "input.parameters.json");
            File.Exists(expectedParametersFilePath).Should().BeTrue();
            File.ReadAllText(expectedParametersFilePath).Should().Be(expectedParametersFileContents);
        }

        [TestMethod]
        public void GetUpdatedParametersFileContents_WithNonEmptyBicepUpdatedDeploymentParametersAndParametersFile_ShouldUpdateParametersFile()
        {
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty, testOutputPath);
            var parametersFileContents = @"{
  ""location"": {
    ""value"": ""eastus""
  },
  ""sku"": {
    ""value"": ""testSku""
  }
}";
            var parametersFilePath = FileHelper.SaveResultFile(TestContext, "input.parameters.json", parametersFileContents, testOutputPath);
            var bicepUpdatedDeploymentParameter1 = new BicepUpdatedDeploymentParameter("name", "test");
            var bicepUpdatedDeploymentParameter2 = new BicepUpdatedDeploymentParameter("id", "testId");
            var bicepUpdatedDeploymentParameters =
                new List<BicepUpdatedDeploymentParameter> { bicepUpdatedDeploymentParameter1, bicepUpdatedDeploymentParameter2 };

            var result = DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "input.parameters.json",
                parametersFilePath,
                true,
                true,
                bicepUpdatedDeploymentParameters);
            var expectedParametersFileContents = @"{
  ""location"": {
    ""value"": ""eastus""
  },
  ""sku"": {
    ""value"": ""testSku""
  },
  ""name"": {
    ""value"": ""test""
  },
  ""id"": {
    ""value"": ""testId""
  }
}";

            result.Should().Be(expectedParametersFileContents);
            File.ReadAllText(parametersFilePath).Should().Be(expectedParametersFileContents);
        }

        [TestMethod]
        public void GetUpdatedParametersFileContents_WithInvalidParametersFile_ShouldThrowException()
        {
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty, testOutputPath);
            var parametersFileContents = @"{
  ""location"": {
    ""value"": ""eastus""
  },
  ""sku"": {
    ""value"": ""testSku""";
            var parametersFilePath = FileHelper.SaveResultFile(TestContext, "input.parameters.json", parametersFileContents, testOutputPath);
            var bicepUpdatedDeploymentParameter1 = new BicepUpdatedDeploymentParameter("name", "test");
            var bicepUpdatedDeploymentParameter2 = new BicepUpdatedDeploymentParameter("id", "testId");
            var bicepUpdatedDeploymentParameters =
                new List<BicepUpdatedDeploymentParameter> { bicepUpdatedDeploymentParameter1, bicepUpdatedDeploymentParameter2 };

            Action action = () => DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "input.parameters.json",
                parametersFilePath,
                true,
                true,
                bicepUpdatedDeploymentParameters);

            action.Should().Throw<Exception>();
        }
    }
}
