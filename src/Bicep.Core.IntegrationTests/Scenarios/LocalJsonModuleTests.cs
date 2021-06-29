// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.Scenarios
{
    [TestClass]
    public class LocalJsonModuleTests
    {
        [DataTestMethod]
        [DataRow("Invalid JSON template: Cannot locate $schema for the template.", @"{
}")]
        [DataRow("Invalid JSON template: Cannot locate contentVersion for the template.", @"{
  ""$schema"": ""foo""
}")]
        [DataRow("Invalid JSON template: Cannot locate resources for the template.", @"{
  ""$schema"": ""foo"",
  ""contentVersion"": ""bar""
}")]
        [DataRow("Invalid JSON template: $schema value \"foo\" is not a valid URI.", @"{
  ""$schema"": ""foo"",
  ""contentVersion"": ""1.0.0.0"",
  ""resources"": []
}")]
        [DataRow("Invalid JSON template: Cannot convert the parameter name \"123foo\" to an identifier.", @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""123foo"": {
      ""type"": ""string""
    }
  },
  ""resources"": []
}")]
        [DataRow("Invalid JSON template: Cannot convert the output name \"foo&bar\" to an identifier.", @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""resources"": [],
  ""outputs"": {
    ""foo&bar"": {
      ""type"": ""string"",
      ""value"": ""foobar""
    }
  }
}")]
        [DataRow("Invalid JSON template: The parameter name \"[concat('foo', 'bar')]\" cannot be an expression.", @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""[concat('foo', 'bar')]"": {
      ""type"": ""string""
    }
  },
  ""resources"": []
}")]
        [DataRow("Invalid JSON template: The output name \"[resourceGroup().id]\" cannot be an expression.", @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""resources"": [],
  ""outputs"": {
    ""[resourceGroup().id]"": {
      ""type"": ""string"",
      ""value"": ""hello""
    }
  }
}")]
        [DataRow("Invalid JSON template: The type value \"something\" is invalid.", @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""foo"": {
      ""type"": ""something""
    }
  },
  ""resources"": []
}")]
        [DataRow("Invalid JSON template: The type value \"apple\" is invalid.", @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""resources"": [],
  ""outputs"": {
    ""foo"": {
      ""type"": ""apple"",
      ""value"": ""hello""
    }
  }
}")]
        [DataRow("Invalid JSON template: Expected the value 12345 to be a string.", @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""foo"": {
      ""type"": 12345
    }
  },
  ""resources"": []
}")]
        [DataRow("Invalid JSON template: Expected the value true to be a string.", @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""resources"": [],
  ""outputs"": {
    ""foo"": {
      ""type"": true,
      ""value"": ""hello""
    }
  }
}")]
        [DataRow("Invalid JSON template: The value \"[add(1, 1)]\" cannot be an expression.", @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""foo"": {
      ""type"": ""[add(1, 1)]""
    }
  },
  ""resources"": []
}")]
        [DataRow("Invalid JSON template: The value \"[add(1, 1)]\" cannot be an expression.", @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""resources"": [],
  ""outputs"": {
    ""foo"": {
      ""type"": ""[add(1, 1)]"",
      ""value"": ""hello""
    }
  }
}")]
        [DataRow("Invalid JSON template: The value \"[toLower('VALUE3')]\" cannot be an expression.", @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""foo"": {
      ""type"": ""object"",
      ""allowedValues"": [
        {
            ""key"": ""value1""
        },
        {
            ""key"": ""value2""
        },
        {
            ""key"": ""[toLower('VALUE3')]""
        }
      ]
    }
  },
  ""resources"": []
}")]
        [DataRow("Invalid JSON template: The \"allowedValues\" array must contain one or more items.", @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""foo"": {
      ""type"": ""array"",
      ""allowedValues"": [
      ]
    }
  },
  ""resources"": []
}")]
        [DataRow("Invalid JSON template: Expected the value \"something\" to be a boolean.", @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""foo"": {
      ""type"": ""bool"",
      ""allowedValues"": [
        true,
        ""something"",
        false
      ]
    }
  },
  ""resources"": []
}")]
        public void CompileWithLocalJsonModule_InvalidTemplate_FailsWithBCP184(string errorMessage, string jsonTemplateText)
        {
            var (template, _, compilation) = CompilationHelper.Compile(
                ("main.bicep", @"
module mod 'module.json' = {
  name: 'myMod'
}
"),
                ("module.json", jsonTemplateText));

            var diagnosticsByFileName = GetDiagnosticsByFileName(compilation);

            using (new AssertionScope())
            {
                template.Should().BeNull();
                diagnosticsByFileName["main.bicep"].Should().HaveDiagnostics(new[]
                {
                    ("BCP104", DiagnosticLevel.Error, "The referenced module has errors."),
                });
                diagnosticsByFileName["module.json"].Should().HaveDiagnostics(new[]
                {
                    ("BCP184", DiagnosticLevel.Error, errorMessage),
                });
            }
        }

        [TestMethod]
        public void CompileWithLocalJsonModule_ValidTemplate_Succeeds()
        {
            var (template, _, compilation) = CompilationHelper.Compile(
                ("main.bicep", @"
module mod 'module.json' = {
  name: 'myMod'
  params: {
    storageAccountName: 'mystorage123xyz'
  }
}
"),
                ("module.json", @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""storageAccountName"": {
      ""type"": ""string""
    },
    ""containerName"": {
      ""type"": ""string"",
      ""defaultValue"": ""logs""
    },
    ""location"": {
      ""type"": ""string"",
      ""defaultValue"": ""[resourceGroup().location]""
    }
  },
  ""resources"": [
    {
      ""type"": ""Microsoft.Storage/storageAccounts"",
      ""apiVersion"": ""2019-06-01"",
      ""name"": ""[parameters('storageAccountName')]"",
      ""location"": ""[parameters('location')]"",
      ""sku"": {
                    ""name"": ""Standard_LRS"",
        ""tier"": ""Standard""
      },
      ""kind"": ""StorageV2"",
      ""properties"": {
        ""accessTier"": ""Hot""
      }
    },
    {
      ""type"": ""Microsoft.Storage/storageAccounts/blobServices/containers"",
      ""apiVersion"": ""2019-06-01"",
      ""name"": ""[format('{0}/default/{1}', parameters('storageAccountName'), parameters('containerName'))]"",
      ""dependsOn"": [
        ""[resourceId('Microsoft.Storage/storageAccounts', parameters('storageAccountName'))]""
      ]
    }
  ]
}"));

            var diagnosticsByFileName = GetDiagnosticsByFileName(compilation);

            using (new AssertionScope())
            {
                template.Should().NotBeNull();
                diagnosticsByFileName["main.bicep"].Should().BeEmpty();
                diagnosticsByFileName["module.json"].Should().BeEmpty();
            }
        }

        private static IReadOnlyDictionary<string, IEnumerable<IDiagnostic>> GetDiagnosticsByFileName(Compilation compilation) =>
            compilation.GetAllDiagnosticsBySyntaxTree().ToDictionary(kvp => Path.GetFileName(kvp.Key.FileUri.LocalPath), kvp => kvp.Value);
    }
}
