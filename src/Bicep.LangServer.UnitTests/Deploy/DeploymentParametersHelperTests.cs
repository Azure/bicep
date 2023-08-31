// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

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
                ParametersFileUpdateOption.None,
                new List<BicepUpdatedDeploymentParameter>());

            result.Should().BeEquivalentToIgnoringNewlines(parametersFileContents);
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
            var bicepUpdatedDeploymentParameter1 = new BicepUpdatedDeploymentParameter("location", "eastus", false, ParameterType.String);
            var bicepUpdatedDeploymentParameter2 = new BicepUpdatedDeploymentParameter("sku", "testSku", false, ParameterType.String);
            var bicepUpdatedDeploymentParameters =
                new List<BicepUpdatedDeploymentParameter> { bicepUpdatedDeploymentParameter1, bicepUpdatedDeploymentParameter2 };

            var result = DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "parameters.json",
                parametersFilePath,
                ParametersFileUpdateOption.None,
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
            var testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty, testOutputPath);
            var bicepUpdatedDeploymentParameter1 = new BicepUpdatedDeploymentParameter("location", "eastus", false, ParameterType.String);
            var bicepUpdatedDeploymentParameter2 = new BicepUpdatedDeploymentParameter("sku", "testSku", false, ParameterType.String);
            var bicepUpdatedDeploymentParameters =
                new List<BicepUpdatedDeploymentParameter> { bicepUpdatedDeploymentParameter1, bicepUpdatedDeploymentParameter2 };

            var result = DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "input.parameters.json",
                string.Empty,
                ParametersFileUpdateOption.Create,
                bicepUpdatedDeploymentParameters);
            var expectedParametersFileContentsWrittenToDisk = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""location"": {
      ""value"": ""eastus""
    },
    ""sku"": {
      ""value"": ""testSku""
    }
  }
}";
            var expectedParametersFileContentsUsedInDeployment = @"{
  ""location"": {
    ""value"": ""eastus""
  },
  ""sku"": {
    ""value"": ""testSku""
  }
}";


            result.Should().BeEquivalentToIgnoringNewlines(expectedParametersFileContentsUsedInDeployment);

            var expectedParametersFilePath = Path.Combine(testOutputPath, "input.parameters.json");
            File.Exists(expectedParametersFilePath).Should().BeTrue();
            File.ReadAllText(expectedParametersFilePath).Should().BeEquivalentToIgnoringNewlines(expectedParametersFileContentsWrittenToDisk);
        }

        [TestMethod]
        public void GetUpdatedParametersFileContents_WithNonEmptyBicepUpdatedDeploymentParametersAndParametersFile_ShouldUpdateParametersFile()
        {
            var testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
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
            var bicepUpdatedDeploymentParameter1 = new BicepUpdatedDeploymentParameter("name", "test", false, ParameterType.String);
            var bicepUpdatedDeploymentParameter2 = new BicepUpdatedDeploymentParameter("id", "testId", false, ParameterType.String);
            var bicepUpdatedDeploymentParameters =
                new List<BicepUpdatedDeploymentParameter> { bicepUpdatedDeploymentParameter1, bicepUpdatedDeploymentParameter2 };

            var result = DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "input.parameters.json",
                parametersFilePath,
                ParametersFileUpdateOption.Update,
                bicepUpdatedDeploymentParameters);
            var expectedParametersFileContents = @"{
  ""name"": {
    ""value"": ""test""
  },
  ""id"": {
    ""value"": ""testId""
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
            var testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty, testOutputPath);
            var parametersFileContents = @"{
  ""location"": {
    ""value"": ""eastus""
  },
  ""sku"": {
    ""value"": ""testSku""";
            var parametersFilePath = FileHelper.SaveResultFile(TestContext, "input.parameters.json", parametersFileContents, testOutputPath);
            var bicepUpdatedDeploymentParameter1 = new BicepUpdatedDeploymentParameter("name", "test", false, ParameterType.String);
            var bicepUpdatedDeploymentParameter2 = new BicepUpdatedDeploymentParameter("id", "testId", false, ParameterType.String);
            var bicepUpdatedDeploymentParameters =
                new List<BicepUpdatedDeploymentParameter> { bicepUpdatedDeploymentParameter1, bicepUpdatedDeploymentParameter2 };

            Action action = () => DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "input.parameters.json",
                parametersFilePath,
                ParametersFileUpdateOption.Update,
                bicepUpdatedDeploymentParameters);

            action.Should().Throw<Exception>();
        }

        [TestMethod]
        public void GetUpdatedParametersFileContents_WithParametersOfTypeIntAndBool_ShouldDoAppropriateConversionAndUpdateParametersFile()
        {
            var testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty, testOutputPath);
            var parametersFileContents = @"{
  ""location"": {
    ""value"": ""eastus""
  }
}";
            var parametersFilePath = FileHelper.SaveResultFile(TestContext, "input.parameters.json", parametersFileContents, testOutputPath);
            var bicepUpdatedDeploymentParameter1 = new BicepUpdatedDeploymentParameter("id", "test", false, ParameterType.String);
            var bicepUpdatedDeploymentParameter2 = new BicepUpdatedDeploymentParameter("isSku", "false", false, ParameterType.Bool);
            var bicepUpdatedDeploymentParameter3 = new BicepUpdatedDeploymentParameter("count", "2", false, ParameterType.Int);
            var bicepUpdatedDeploymentParameters =
                new List<BicepUpdatedDeploymentParameter> { bicepUpdatedDeploymentParameter1, bicepUpdatedDeploymentParameter2, bicepUpdatedDeploymentParameter3 };

            var result = DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "input.parameters.json",
                parametersFilePath,
                ParametersFileUpdateOption.Update,
                bicepUpdatedDeploymentParameters);
            var expectedParametersFileContents = @"{
  ""id"": {
    ""value"": ""test""
  },
  ""isSku"": {
    ""value"": false
  },
  ""count"": {
    ""value"": 2
  },
  ""location"": {
    ""value"": ""eastus""
  }
}";

            result.Should().BeEquivalentToIgnoringNewlines(expectedParametersFileContents);
            File.ReadAllText(parametersFilePath).Should().BeEquivalentToIgnoringNewlines(expectedParametersFileContents);
        }

        [TestMethod]
        public void GetUpdatedParametersFileContents_WithBicepDeploymentParameterThatIsAlreadyInParametersFile_DoesNothing()
        {
            var testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
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
            var bicepUpdatedDeploymentParameter = new BicepUpdatedDeploymentParameter("location", "westus", false, ParameterType.String);
            var bicepUpdatedDeploymentParameters =
                new List<BicepUpdatedDeploymentParameter> { bicepUpdatedDeploymentParameter };

            var result = DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "input.parameters.json",
                parametersFilePath,
                ParametersFileUpdateOption.Update,
                bicepUpdatedDeploymentParameters);

            result.Should().BeEquivalentToIgnoringNewlines(parametersFileContents);
        }

        [TestMethod]
        public void GetUpdatedParametersFileContents_WithArmTemplateStyleParametersFile_ShouldUpdateParametersFile()
        {
            var testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty, testOutputPath);
            var parametersFileContents = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""location"": {
      ""value"": ""eastus""
    }
  }
}";
            var parametersFilePath = FileHelper.SaveResultFile(TestContext, "input.parameters.json", parametersFileContents, testOutputPath);
            var bicepUpdatedDeploymentParameter1 = new BicepUpdatedDeploymentParameter("name", "test", false, ParameterType.String);
            var bicepUpdatedDeploymentParameter2 = new BicepUpdatedDeploymentParameter("isSku", "true", false, ParameterType.Bool);
            var bicepUpdatedDeploymentParameter3 = new BicepUpdatedDeploymentParameter("count", "3", false, ParameterType.Int);
            var bicepUpdatedDeploymentParameters =
                new List<BicepUpdatedDeploymentParameter> { bicepUpdatedDeploymentParameter1, bicepUpdatedDeploymentParameter2, bicepUpdatedDeploymentParameter3 };

            var result = DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "input.parameters.json",
                parametersFilePath,
                ParametersFileUpdateOption.Update,
                bicepUpdatedDeploymentParameters);
            var expectedParametersFileContentsWrittenToDisk = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""name"": {
      ""value"": ""test""
    },
    ""isSku"": {
      ""value"": true
    },
    ""count"": {
      ""value"": 3
    },
    ""location"": {
      ""value"": ""eastus""
    }
  }
}";
            var expectedParametersFileUsedInDeployment = @"{
  ""name"": {
    ""value"": ""test""
  },
  ""isSku"": {
    ""value"": true
  },
  ""count"": {
    ""value"": 3
  },
  ""location"": {
    ""value"": ""eastus""
  }
}";

            result.Should().BeEquivalentToIgnoringNewlines(expectedParametersFileUsedInDeployment);
            File.ReadAllText(parametersFilePath).Should().BeEquivalentToIgnoringNewlines(expectedParametersFileContentsWrittenToDisk);
        }

        [TestMethod]
        public void GetUpdatedParametersFileContents_WithNonEmptyBicepUpdatedDeploymentParametersAndParametersFileWithComments_ShouldNotRemoveComments()
        {
            var testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
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
            var bicepUpdatedDeploymentParameter1 = new BicepUpdatedDeploymentParameter("name", "test", false, ParameterType.String);
            var bicepUpdatedDeploymentParameter2 = new BicepUpdatedDeploymentParameter("id", "testId", false, ParameterType.String);
            var bicepUpdatedDeploymentParameters =
                new List<BicepUpdatedDeploymentParameter> { bicepUpdatedDeploymentParameter1, bicepUpdatedDeploymentParameter2 };

            var result = DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "input.parameters.jsonc",
                parametersFilePath,
                ParametersFileUpdateOption.Update,
                bicepUpdatedDeploymentParameters);
            var expectedParametersFileContentsWrittenToDisk = @"{
  ""name"": {
    ""value"": ""test""
  },
  ""id"": {
    ""value"": ""testId""
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
            var expectedParametersFileContentsUsedInDeployments = @"{
  ""name"": {
    ""value"": ""test""
  },
  ""id"": {
    ""value"": ""testId""
  },
  ""location"": {
    ""value"": ""eastus""
  },
  ""sku"": {
    ""value"": ""testSku""
  }
}";

            result.Should().BeEquivalentToIgnoringNewlines(expectedParametersFileContentsUsedInDeployments);
            File.ReadAllText(parametersFilePath).Should().BeEquivalentToIgnoringNewlines(expectedParametersFileContentsWrittenToDisk);
        }

        [TestMethod]
        public void GetUpdatedParametersFileContents_WithOverwriteOptionAndParametersFileWithSameNameInCurrentDirectory_ShouldOverwriteParametersFile()
        {
            var testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty, testOutputPath);
            var parametersFileContents = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""location"": {
      ""value"": ""eastus""
    }
  }
}";
            FileHelper.SaveResultFile(TestContext, "input.parameters.json", parametersFileContents, testOutputPath);
            var bicepUpdatedDeploymentParameter1 = new BicepUpdatedDeploymentParameter("location", "westus", false, ParameterType.String);
            var bicepUpdatedDeploymentParameter2 = new BicepUpdatedDeploymentParameter("sku", "testSku", false, ParameterType.String);
            var bicepUpdatedDeploymentParameters =
                new List<BicepUpdatedDeploymentParameter> { bicepUpdatedDeploymentParameter1, bicepUpdatedDeploymentParameter2 };

            var result = DeploymentParametersHelper.GetUpdatedParametersFileContents(
                bicepFilePath,
                "input.parameters.json",
                string.Empty,
                ParametersFileUpdateOption.Create,
                bicepUpdatedDeploymentParameters);
            var expectedParametersFileContentsWrittenToDisk = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""location"": {
      ""value"": ""westus""
    },
    ""sku"": {
      ""value"": ""testSku""
    }
  }
}";
            var expectedParametersFileContentsUsedInDeployment = @"{
  ""location"": {
    ""value"": ""westus""
  },
  ""sku"": {
    ""value"": ""testSku""
  }
}";


            result.Should().BeEquivalentToIgnoringNewlines(expectedParametersFileContentsUsedInDeployment);

            var expectedParametersFilePath = Path.Combine(testOutputPath, "input.parameters.json");
            File.Exists(expectedParametersFilePath).Should().BeTrue();
            File.ReadAllText(expectedParametersFilePath).Should().BeEquivalentToIgnoringNewlines(expectedParametersFileContentsWrittenToDisk);
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
