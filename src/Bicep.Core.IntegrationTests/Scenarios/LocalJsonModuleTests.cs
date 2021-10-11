// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Azure.Deployments.Core.Instrumentation;
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
        [DataRow(@"{
}")]
        [DataRow(@"{
  ""$schema"": ""foo""
}")]
        [DataRow(@"{
  ""$schema"": ""foo"",
  ""contentVersion"": ""bar""
}")]
        [DataRow(@"{
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
        public void CompileWithLocalJsonModule_InvalidTemplate_FailsWithBCP188(string jsonTemplateText)
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
                diagnosticsByFileName.Should().NotContainKey("module.json");
                diagnosticsByFileName["main.bicep"].Should().HaveDiagnostics(new[]
                {
                    ("BCP188", DiagnosticLevel.Error, "The referenced ARM template has errors. Please see https://aka.ms/arm-template for information on how to diagnose and fix the template."),
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
                diagnosticsByFileName.Should().NotContainKey("module.json");
                diagnosticsByFileName["main.bicep"].Should().BeEmpty();
            }
        }

        private static IReadOnlyDictionary<string, IEnumerable<IDiagnostic>> GetDiagnosticsByFileName(Compilation compilation) =>
            compilation.GetAllDiagnosticsByBicepFile().ToDictionary(kvp => Path.GetFileName(kvp.Key.FileUri.LocalPath), kvp => kvp.Value);
    }
}
