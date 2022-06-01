// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

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
                ParametersFileCreateOrUpdate.None,
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
  }
}";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty);
            var parametersFilePath = FileHelper.SaveResultFile(TestContext, "parameters.json", parametersFileContents);
            var bicepUpdatedDeploymentParameter1 = new BicepUpdatedDeploymentParameter("location", "eastus", ParameterType.String);
            var bicepUpdatedDeploymentParameter2 = new BicepUpdatedDeploymentParameter("sku", "testSku", ParameterType.String);
            var bicepUpdatedDeploymentParameters =
                new List<BicepUpdatedDeploymentParameter> { bicepUpdatedDeploymentParameter1, bicepUpdatedDeploymentParameter2 };

            var result = DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "parameters.json",
                parametersFilePath,
                ParametersFileCreateOrUpdate.None,
                bicepUpdatedDeploymentParameters);
            var expected = @"{
  ""sku"": {
    ""value"": ""testSku""
  },
  ""location"": {
    ""value"": ""westus""
  },
  ""name"": {
    ""value"": ""test""
  }
}";

            result.Should().BeEquivalentToIgnoringNewlines(expected);
        }

        [TestMethod]
        public void GetUpdatedParametersFileContents_WithNonEmptyBicepUpdatedDeploymentParametersAndNoParametersFile_ShouldCreateParametersFile()
        {
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty, testOutputPath);
            var bicepUpdatedDeploymentParameter1 = new BicepUpdatedDeploymentParameter("location", "eastus", ParameterType.String);
            var bicepUpdatedDeploymentParameter2 = new BicepUpdatedDeploymentParameter("sku", "testSku", ParameterType.String);
            var bicepUpdatedDeploymentParameters =
                new List<BicepUpdatedDeploymentParameter> { bicepUpdatedDeploymentParameter1, bicepUpdatedDeploymentParameter2 };

            var result = DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "input.parameters.json",
                string.Empty,
                ParametersFileCreateOrUpdate.Create,
                bicepUpdatedDeploymentParameters);
            var expectedParametersFileContents = @"{
  ""sku"": {
    ""value"": ""testSku""
  },
  ""location"": {
    ""value"": ""eastus""
  }
}";

            result.Should().BeEquivalentToIgnoringNewlines(expectedParametersFileContents);

            var expectedParametersFilePath = Path.Combine(testOutputPath, "input.parameters.json");
            File.Exists(expectedParametersFilePath).Should().BeTrue();
            File.ReadAllText(expectedParametersFilePath).Should().BeEquivalentToIgnoringNewlines(expectedParametersFileContents);
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
            var bicepUpdatedDeploymentParameter1 = new BicepUpdatedDeploymentParameter("name", "test", ParameterType.String);
            var bicepUpdatedDeploymentParameter2 = new BicepUpdatedDeploymentParameter("id", "testId", ParameterType.String);
            var bicepUpdatedDeploymentParameters =
                new List<BicepUpdatedDeploymentParameter> { bicepUpdatedDeploymentParameter1, bicepUpdatedDeploymentParameter2 };

            var result = DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "input.parameters.json",
                parametersFilePath,
                ParametersFileCreateOrUpdate.Update,
                bicepUpdatedDeploymentParameters);
            var expectedParametersFileContents = @"{
  ""id"": {
    ""value"": ""testId""
  },
  ""name"": {
    ""value"": ""test""
  },
  ""location"": {
    ""value"": ""eastus""
  },
  ""sku"": {
    ""value"": ""testSku""
  }
}";

            result.Should().BeEquivalentToIgnoringNewlines(expectedParametersFileContents);
            File.ReadAllText(parametersFilePath).Should().BeEquivalentToIgnoringNewlines(expectedParametersFileContents);
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
            var bicepUpdatedDeploymentParameter1 = new BicepUpdatedDeploymentParameter("name", "test", ParameterType.String);
            var bicepUpdatedDeploymentParameter2 = new BicepUpdatedDeploymentParameter("id", "testId", ParameterType.String);
            var bicepUpdatedDeploymentParameters =
                new List<BicepUpdatedDeploymentParameter> { bicepUpdatedDeploymentParameter1, bicepUpdatedDeploymentParameter2 };

            Action action = () => DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "input.parameters.json",
                parametersFilePath,
                ParametersFileCreateOrUpdate.Update,
                bicepUpdatedDeploymentParameters);

            action.Should().Throw<Exception>();
        }

        [TestMethod]
        public void GetUpdatedParametersFileContents_WithParametersOfTypeIntAndBool_ShouldDoAppropriateConversionAndUpdateParametersFile()
        {
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty, testOutputPath);
            var parametersFileContents = @"{
  ""location"": {
    ""value"": ""eastus""
  }
}";
            var parametersFilePath = FileHelper.SaveResultFile(TestContext, "input.parameters.json", parametersFileContents, testOutputPath);
            var bicepUpdatedDeploymentParameter1 = new BicepUpdatedDeploymentParameter("id", "test", ParameterType.String);
            var bicepUpdatedDeploymentParameter2 = new BicepUpdatedDeploymentParameter("isSku", "false", ParameterType.Bool);
            var bicepUpdatedDeploymentParameter3 = new BicepUpdatedDeploymentParameter("count", "2", ParameterType.Int);
            var bicepUpdatedDeploymentParameters =
                new List<BicepUpdatedDeploymentParameter> {  bicepUpdatedDeploymentParameter1, bicepUpdatedDeploymentParameter2, bicepUpdatedDeploymentParameter3 };

            var result = DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "input.parameters.json",
                parametersFilePath,
                ParametersFileCreateOrUpdate.Update,
                bicepUpdatedDeploymentParameters);
            var expectedParametersFileContents = @"{
  ""count"": {
    ""value"": 2
  },
  ""isSku"": {
    ""value"": false
  },
  ""id"": {
    ""value"": ""test""
  },
  ""location"": {
    ""value"": ""eastus""
  }
}";

            result.Should().BeEquivalentToIgnoringNewlines(expectedParametersFileContents);
            File.ReadAllText(parametersFilePath).Should().BeEquivalentToIgnoringNewlines(expectedParametersFileContents);
        }

        [TestMethod]
        public void GetUpdatedParametersFileContents_WithBicepUpdatedDeploymentParameterThatIsAlreadyInParametersFile_DoesNothing()
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
            var bicepUpdatedDeploymentParameter = new BicepUpdatedDeploymentParameter("location", "westus", ParameterType.String);
            var bicepUpdatedDeploymentParameters =
                new List<BicepUpdatedDeploymentParameter> { bicepUpdatedDeploymentParameter };

            var result = DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "input.parameters.json",
                parametersFilePath,
                ParametersFileCreateOrUpdate.Update,
                bicepUpdatedDeploymentParameters);

            result.Should().Be(parametersFileContents);
        }

        [TestMethod]
        public void GetUpdatedParametersFileContents_WithNonEmptyBicepUpdatedDeploymentParametersAndParametersFileWithComments_ShouldNotRemoveComments()
        {
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty, testOutputPath);
            var parametersFileContents = @"{
  // comment 1
  ""location"": {
    ""value"": ""eastus""
  },
  ""sku"": {
    ""value"": ""testSku""
  }
  // comment 2
}";
            var parametersFilePath = FileHelper.SaveResultFile(TestContext, "input.parameters.jsonc", parametersFileContents, testOutputPath);
            var bicepUpdatedDeploymentParameter1 = new BicepUpdatedDeploymentParameter("name", "test", ParameterType.String);
            var bicepUpdatedDeploymentParameter2 = new BicepUpdatedDeploymentParameter("id", "testId", ParameterType.String);
            var bicepUpdatedDeploymentParameters =
                new List<BicepUpdatedDeploymentParameter> { bicepUpdatedDeploymentParameter1, bicepUpdatedDeploymentParameter2 };

            var result = DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "input.parameters.jsonc",
                parametersFilePath,
                ParametersFileCreateOrUpdate.Update,
                bicepUpdatedDeploymentParameters);
            var expectedParametersFileContents = @"{
  ""id"": {
    ""value"": ""testId""
  },
  ""name"": {
    ""value"": ""test""
  },
  // comment 1
  ""location"": {
    ""value"": ""eastus""
  },
  ""sku"": {
    ""value"": ""testSku""
  }
  // comment 2
}";

            result.Should().BeEquivalentToIgnoringNewlines(expectedParametersFileContents);
            File.ReadAllText(parametersFilePath).Should().BeEquivalentToIgnoringNewlines(expectedParametersFileContents);
        }

        [TestMethod]
        public void UpdateJObjectBasedOnParameterType_WithValidInputOfTypeInt_ReturnsUpdatedJObject()
        {
            var result = DeploymentParametersHelper.UpdateJObjectBasedOnParameterType(ParameterType.Int, "1", JObject.Parse("{}"));

            result.ToString().Should().BeEquivalentToIgnoringNewlines(@"{
  ""value"": 1
}");
        }

        [TestMethod]
        public void UpdateJObjectBasedOnParameterType_WithValidInputOfTypeBool_ReturnsUpdatedJObject()
        {
            var result = DeploymentParametersHelper.UpdateJObjectBasedOnParameterType(ParameterType.Bool, "true", JObject.Parse("{}"));

            result.ToString().Should().BeEquivalentToIgnoringNewlines(@"{
  ""value"": true
}");
        }

        [TestMethod]
        public void UpdateJObjectBasedOnParameterType_WithValidInputOfTypeString_ReturnsUpdatedJObject()
        {
            var result = DeploymentParametersHelper.UpdateJObjectBasedOnParameterType(ParameterType.String, "test", JObject.Parse("{}"));

            result.ToString().Should().BeEquivalentToIgnoringNewlines(@"{
  ""value"": ""test""
}");
        }

        [TestMethod]
        public void UpdateJObjectBasedOnParameterType_WithInvalidInput_ThrowsException()
        {
            Action action = () => DeploymentParametersHelper.UpdateJObjectBasedOnParameterType(ParameterType.Int, "test", JObject.Parse("{}"));

            action.Should().Throw<Exception>();
        }
    }
}
