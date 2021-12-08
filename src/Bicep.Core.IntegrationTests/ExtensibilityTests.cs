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
import storage as stg {
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
import storage as stg1 {
  connectionString: 'connectionString1'
}

import storage as stg2 {
  connectionString: 'connectionString2'
}
");
            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Storage_import_basic_test()
        {
            var result = CompilationHelper.Compile(GetCompilationContext(), @"
import storage as stg {
  connectionString: 'asdf'
}

resource container 'container' = {
  name: 'myblob'
}

resource blob 'blob' = {
  name: 'myblob'
  containerName: container.name
  base64Content: base64('sadfasdfd')
}
");
            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Storage_import_basic_test_loops_and_referencing()
        {
            var result = CompilationHelper.Compile(GetCompilationContext(), @"
import storage as stg {
  connectionString: 'asdf'
}

resource container 'container' = {
  name: 'myblob'
}

resource blobs 'blob' = [for i in range(0, 10): {
  name: 'myblob-${i}.txt'
  containerName: container.name
  base64Content: base64('Hello blob ${i}!')
}]

resource blobs2 'blob' = [for i in range(10, 10): {
  name: blobs[i - 10].name
  containerName: container.name
  base64Content: base64('Hello blob ${i}!')
}]

output sourceContainerName string = container.name
output sourceContainerNameSquare string = container['name']
output miscBlobContainerName string = blobs[13 % 10].containerName
output containerName string = blobs[5].containerName
output base64Content string = blobs[3]['base64Content']
");
            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['sourceContainerName'].value", "[reference('container').name]");
            result.Template.Should().HaveValueAtPath("$.outputs['sourceContainerNameSquare'].value", "[reference('container').name]");
            result.Template.Should().HaveValueAtPath("$.outputs['miscBlobContainerName'].value", "[reference(format('blobs[{0}]', mod(13, 10))).containerName]");
            result.Template.Should().HaveValueAtPath("$.outputs['containerName'].value", "[reference(format('blobs[{0}]', 5)).containerName]");
            result.Template.Should().HaveValueAtPath("$.outputs['base64Content'].value", "[reference(format('blobs[{0}]', 3)).base64Content]");
        }

        [TestMethod]
        public void Aad_import_basic_test_loops_and_referencing()
        {
            var result = CompilationHelper.Compile(GetCompilationContext(), @"
import aad as aad
param numApps int

resource myApp 'application' = {
  uniqueName: 'foo'
}

resource myAppsLoop 'application' = [for i in range(0, numApps): {
  uniqueName: '${myApp.appId}-bar-${i}'
}]

output myAppId string = myApp.appId
output myAppId2 string = myApp['appId']
output myAppsLoopId string = myAppsLoop[13 % numApps].appId
output myAppsLoopId2 string = myAppsLoop[3]['appId']
");
            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['myAppId'].value", "[reference('myApp').appId]");
            result.Template.Should().HaveValueAtPath("$.outputs['myAppId2'].value", "[reference('myApp').appId]");
            result.Template.Should().HaveValueAtPath("$.outputs['myAppsLoopId'].value", "[reference(format('myAppsLoop[{0}]', mod(13, parameters('numApps')))).appId]");
            result.Template.Should().HaveValueAtPath("$.outputs['myAppsLoopId2'].value", "[reference(format('myAppsLoop[{0}]', 3)).appId]");
        }

        [TestMethod]
        public void Aad_import_existing_requires_uniqueName()
        {
            // we've accidentally used 'name' even though this resource type doesn't support it
            var result = CompilationHelper.Compile(GetCompilationContext(), @"
import aad as aad

resource myApp 'application' existing = {
  name: 'foo'
}
");

            result.Should().NotGenerateATemplate();
            result.Should().HaveDiagnostics(new[] {
                ("BCP035", DiagnosticLevel.Error, "The specified \"resource\" declaration is missing the following required properties: \"uniqueName\"."),
                ("BCP037", DiagnosticLevel.Error, "The property \"name\" is not allowed on objects of type \"application\". Permissible properties include \"uniqueName\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
            });

            // oops! let's change it to 'uniqueName'
            result = CompilationHelper.Compile(GetCompilationContext(), @"
import aad as aad

resource myApp 'application' existing = {
  uniqueName: 'foo'
}
");

            result.Should().GenerateATemplate();
            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Storage_import_basic_test_with_qualified_type()
        {
            var result = CompilationHelper.Compile(GetCompilationContext(), @"
import storage as stg {
  connectionString: 'asdf'
}

resource container 'stg:container' = {
  name: 'myblob'
}

resource blob 'stg:blob' = {
  name: 'myblob'
  containerName: container.name
  base64Content: base64('sadfasdfd')
}
");
            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Invalid_namespace_qualifier_returns_error()
        {
            var result = CompilationHelper.Compile(GetCompilationContext(), @"
import storage as stg {
  connectionString: 'asdf'
}

resource container 'foo:container' = {
  name: 'myblob'
}

resource blob 'bar:blob' = {
  name: 'myblob'
  containerName: container.name
  base64Content: base64('sadfasdfd')
}
");

            result.Should().HaveDiagnostics(new[] {
                ("BCP208", DiagnosticLevel.Error, "The specified namespace \"foo\" is not recognized. Specify a resource reference using one of the following namespaces: \"az\", \"stg\", \"sys\"."),
                ("BCP208", DiagnosticLevel.Error, "The specified namespace \"bar\" is not recognized. Specify a resource reference using one of the following namespaces: \"az\", \"stg\", \"sys\"."),
            });
        }

        [TestMethod]
        public void Child_resource_with_parent_namespace_mismatch_returns_error()
        {
            var result = CompilationHelper.Compile(GetCompilationContext(), @"
import storage as stg {
  connectionString: 'asdf'
}

resource parent 'az:Microsoft.Storage/storageAccounts@2020-01-01' existing = {
  name: 'stgParent'

  resource container 'stg:container' = {
    name: 'myblob'
  }
}
");

            result.Should().HaveDiagnostics(new[] {
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"Microsoft.Storage/storageAccounts@2020-01-01\" does not have types available."),
                ("BCP210", DiagnosticLevel.Error, "Resource type belonging to namespace \"stg\" cannot have a parent resource type belonging to different namespace \"az\"."),
            });
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

import storage as stg {
  connectionString: connectionString
}

resource container 'container' = {
  name: 'bicep'
}

resource blob 'blob' = {
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
      ""templateHash"": ""4434568493241081408""
    }
  },
  ""parameters"": {
    ""accountName"": {
      ""type"": ""string""
    }
  },
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
      ""apiVersion"": ""2020-10-01"",
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
              ""templateHash"": ""10447496472055117853""
            }
          },
          ""parameters"": {
            ""connectionString"": {
              ""type"": ""secureString""
            }
          },
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
              ""import"": ""stg"",
              ""type"": ""container"",
              ""properties"": {
                ""name"": ""bicep""
              }
            },
            ""blob"": {
              ""import"": ""stg"",
              ""type"": ""blob"",
              ""properties"": {
                ""name"": ""blob.txt"",
                ""containerName"": ""[reference('container').name]"",
                ""base64Content"": ""[base64('\nHello from Bicep!')]""
              },
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
