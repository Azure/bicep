// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.IntegrationTests.Extensibility;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ExtensibilityTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private CompilationHelper.CompilationHelperContext GetCompilationContext()
        {
            var features = BicepTestConstants.CreateFeaturesProvider(TestContext, importsEnabled: true);
            var resourceTypeLoader = BicepTestConstants.AzResourceTypeLoader;
            var namespaceProvider = new ExtensibilityNamespaceProvider(resourceTypeLoader, features);

            return new(
                AzResourceTypeLoader: resourceTypeLoader,
                Features: features,
                NamespaceProvider: namespaceProvider);
        }

        [TestMethod]
        public void Storage_import_bad_config_is_blocked()
        {
            var result = CompilationHelper.Compile(GetCompilationContext(), @"
import stg from storage {
  madeUpProperty: 'asdf'
}
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP035", DiagnosticLevel.Error, "The specified \"object\" declaration is missing the following required properties: \"connectionString\"."),
                ("BCP037", DiagnosticLevel.Error, "The property \"madeUpProperty\" is not allowed on objects of type \"configuration\". Permissible properties include \"connectionString\"."),
            });
        }

        [TestMethod]
        public void Storage_import_can_be_duplicated()
        {
            var result = CompilationHelper.Compile(GetCompilationContext(), @"
import stg1 from storage {
  connectionString: 'connectionString1'
}

import stg2 from storage {
  connectionString: 'connectionString2'
}
");
            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Storage_import_basic_test()
        {
            var result = CompilationHelper.Compile(GetCompilationContext(), @"
import stg from storage {
  connectionString: 'asdf'
}

resource container 'AzureStorage/containers@2020-01-01' = {
  name: 'myblob'
}

resource blob 'AzureStorage/blobs@2020-01-01' = {
  name: 'myblob'
  containerName: container.name
  base64Content: base64('sadfasdfd')
}
");
            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Storage_import_end_to_end_test()
        {
            var result = CompilationHelper.Compile(GetCompilationContext(), 
                ("main.bicep", @"
param accountName string

resource stgAccount 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: toLower(accountName)
  location: resourceGroup().location
  kind: 'Storage'
  sku: {
    name: 'Standard_LRS'
  }
}

var connectionString = 'DefaultEndpointsProtocol=https;AccountName=${stgAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${stgAccount.listKeys().keys[0].value}'

module website './website.bicep' = {
  name: 'website'
  params: {
    connectionString: connectionString
  }
}
"),
                ("website.bicep", @"
@secure()
param connectionString string

import stg from storage {
  connectionString: connectionString
}

resource container 'AzureStorage/containers@2020-01-01' = {
  name: 'bicep'
}

resource blob 'AzureStorage/blobs@2020-01-01' = {
  name: 'blob.txt'
  containerName: container.name
  base64Content: base64(loadTextContent('blob.txt'))
}
"),
                ("blob.txt", @"
Hello from Bicep!"));

            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().DeepEqual(JToken.Parse(@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""languageVersion"": ""1.9-experimental"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""EXPERIMENTAL_WARNING"": ""Symbolic name support in ARM is experimental, and should be enabled for testing purposes only. Do not enable this setting for any production usage, or you may be unexpectedly broken at any time!"",
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""dev"",
      ""templateHash"": ""16492559867717304205""
    }
  },
  ""parameters"": {
    ""accountName"": {
      ""type"": ""string""
    }
  },
  ""functions"": [],
  ""resources"": {
    ""stgAccount"": {
      ""type"": ""Microsoft.Storage/storageAccounts"",
      ""apiVersion"": ""2019-06-01"",
      ""name"": ""[toLower(parameters('accountName'))]"",
      ""location"": ""[resourceGroup().location]"",
      ""kind"": ""Storage"",
      ""sku"": {
        ""name"": ""Standard_LRS""
      }
    },
    ""website"": {
      ""type"": ""Microsoft.Resources/deployments"",
      ""apiVersion"": ""2020-06-01"",
      ""name"": ""website"",
      ""properties"": {
        ""expressionEvaluationOptions"": {
          ""scope"": ""inner""
        },
        ""mode"": ""Incremental"",
        ""parameters"": {
          ""connectionString"": {
            ""value"": ""[format('DefaultEndpointsProtocol=https;AccountName={0};EndpointSuffix={1};AccountKey={2}', resourceInfo('stgAccount').name, environment().suffixes.storage, listKeys(resourceId('Microsoft.Storage/storageAccounts', toLower(parameters('accountName'))), '2019-06-01').keys[0].value)]""
          }
        },
        ""template"": {
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""languageVersion"": ""1.9-experimental"",
          ""contentVersion"": ""1.0.0.0"",
          ""metadata"": {
            ""EXPERIMENTAL_WARNING"": ""Symbolic name support in ARM is experimental, and should be enabled for testing purposes only. Do not enable this setting for any production usage, or you may be unexpectedly broken at any time!"",
            ""_generator"": {
              ""name"": ""bicep"",
              ""version"": ""dev"",
              ""templateHash"": ""9407188780587710254""
            }
          },
          ""parameters"": {
            ""connectionString"": {
              ""type"": ""secureString""
            }
          },
          ""functions"": [],
          ""imports"": {
            ""stg"": {
              ""provider"": ""AzureStorage"",
              ""version"": ""1.0"",
              ""config"": {
                ""connectionString"": ""[parameters('connectionString')]""
              }
            }
          },
          ""resources"": {
            ""container"": {
              ""type"": ""AzureStorage/containers"",
              ""apiVersion"": ""2020-01-01"",
              ""name"": ""bicep""
            },
            ""blob"": {
              ""type"": ""AzureStorage/blobs"",
              ""apiVersion"": ""2020-01-01"",
              ""name"": ""blob.txt"",
              ""containerName"": ""[resourceInfo('container').name]"",
              ""base64Content"": ""[base64('\nHello from Bicep!')]"",
              ""dependsOn"": [
                ""container""
              ]
            }
          }
        }
      },
      ""dependsOn"": [
        ""stgAccount""
      ]
    }
  }
}"));
        }
    }
}