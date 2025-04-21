// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class CliScenarioTests : TestBase
    {
        // https://github.com/azure/bicep/issues/3182
        [TestMethod]
        public async Task Test_Issue3182()
        {
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""dev"",
      ""templateHash"": ""9136367247469226369""
    }
  },
  ""parameters"": {
    ""rgName"": {
      ""type"": ""string""
    },
    ""rgLocation"": {
      ""type"": ""string""
    },
    ""principalId"": {
      ""type"": ""string""
    },
    ""roleDefinitionId"": {
      ""type"": ""string"",
      ""defaultValue"": ""b24988ac-6180-42a0-ab88-20f7382dd24c""
    },
    ""roleAssignmentName"": {
      ""type"": ""string"",
      ""defaultValue"": ""[guid(parameters('principalId'), parameters('roleDefinitionId'), parameters('rgName'))]""
    }
  },
  ""functions"": [],
  ""resources"": [
    {
      ""type"": ""Microsoft.Resources/resourceGroups"",
      ""apiVersion"": ""2019-10-01"",
      ""name"": ""[parameters('rgName')]"",
      ""location"": ""[parameters('rgLocation')]"",
      ""properties"": {}
    },
    {
      ""type"": ""Microsoft.Resources/deployments"",
      ""apiVersion"": ""2019-10-01"",
      ""name"": ""applyLock"",
      ""resourceGroup"": ""[parameters('rgName')]"",
      ""properties"": {
        ""expressionEvaluationOptions"": {
          ""scope"": ""inner""
        },
        ""mode"": ""Incremental"",
        ""parameters"": {
          ""principalId"": {
            ""value"": ""[parameters('principalId')]""
          },
          ""roleDefinitionId"": {
            ""value"": ""[parameters('roleDefinitionId')]""
          },
          ""roleAssignmentName"": {
            ""value"": ""[parameters('roleAssignmentName')]""
          }
        },
        ""template"": {
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""metadata"": {
            ""_generator"": {
              ""name"": ""bicep"",
              ""version"": ""dev"",
              ""templateHash"": ""5867672624469865160""
            }
          },
          ""parameters"": {
            ""principalId"": {
              ""type"": ""string""
            },
            ""roleDefinitionId"": {
              ""type"": ""string""
            },
            ""roleAssignmentName"": {
              ""type"": ""string""
            }
          },
          ""functions"": [],
          ""resources"": [
            {
              ""type"": ""Microsoft.Authorization/locks"",
              ""apiVersion"": ""2016-09-01"",
              ""name"": ""DontDelete"",
              ""properties"": {
                ""level"": ""CanNotDelete"",
                ""notes"": ""Prevent deletion of the resourceGroup""
              }
            },
            {
              ""type"": ""Microsoft.Authorization/roleAssignments"",
              ""apiVersion"": ""2020-04-01-preview"",
              ""name"": ""[guid(parameters('roleAssignmentName'))]"",
              ""properties"": {
                ""roleDefinitionId"": ""[subscriptionResourceId('Microsoft.Authorization/roleDefinitions', parameters('roleDefinitionId'))]"",
                ""principalId"": ""[parameters('principalId')]""
              }
            }
          ]
        }
      },
      ""dependsOn"": [
        ""[subscriptionResourceId('Microsoft.Resources/resourceGroups', parameters('rgName'))]""
      ]
    }
  ]
}";

            var fileName = FileHelper.GetResultFilePath(TestContext, "main.json");
            File.WriteAllText(fileName, template);

            var (_, _, result) = await Bicep("decompile", fileName);

            // This example has errors, but files should still have been generated
            result.Should().Be(1);

            var mainFile = Path.Combine(Path.GetDirectoryName(fileName)!, "main.bicep");
            File.ReadAllText(mainFile).Should().BeEquivalentToIgnoringNewlines("""
                targetScope = 'subscription'
                param rgName string
                param rgLocation string
                param principalId string
                param roleDefinitionId string = 'b24988ac-6180-42a0-ab88-20f7382dd24c'
                param roleAssignmentName string = guid(principalId, roleDefinitionId, rgName)

                resource rg 'Microsoft.Resources/resourceGroups@2019-10-01' = {
                  name: rgName
                  location: rgLocation
                  properties: {}
                }

                module applyLock './nested_applyLock.bicep' = {
                  name: 'applyLock'
                  scope: resourceGroup(rgName)
                  params: {
                    principalId: principalId
                    roleDefinitionId: roleDefinitionId
                    roleAssignmentName: roleAssignmentName
                  }
                  dependsOn: [
                    subscriptionResourceId('Microsoft.Resources/resourceGroups', rgName)
                  ]
                }

                """);

            var moduleFile = Path.Combine(Path.GetDirectoryName(fileName)!, "nested_applyLock.bicep");
            File.ReadAllText(moduleFile).Should().BeEquivalentToIgnoringNewlines("""
                param principalId string
                param roleDefinitionId string
                param roleAssignmentName string

                resource DontDelete 'Microsoft.Authorization/locks@2016-09-01' = {
                  name: 'DontDelete'
                  properties: {
                    level: 'CanNotDelete'
                    notes: 'Prevent deletion of the resourceGroup'
                  }
                }

                resource roleAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
                  name: guid(roleAssignmentName)
                  properties: {
                    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roleDefinitionId)
                    principalId: principalId
                  }
                }

                """);
        }

        [TestMethod]
        public async Task Test_Issue13785()
        {
            var paramFile =
              """
              {
                "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
                "contentVersion": "1.0.0.0",
                "parameters": {
                  "foo": {
                    "value": "bar"
                  }
                }
              }
              """;
            var expectedOutput =
                """
                using '../main.bicep'

                param foo = 'bar'

                """;

            var (jsonPath, bicepparamPath) = Setup(TestContext, paramFile);
            Directory.SetCurrentDirectory(Path.GetDirectoryName(jsonPath)!);
            var (output, _, result) = await Bicep("decompile-params", jsonPath, "--bicep-file", "../main.bicep");

            using (new AssertionScope())
            {
                output.Should().BeEmpty();
                result.Should().Be(0);
                File.ReadAllText(bicepparamPath).Should().BeEquivalentToIgnoringNewlines(expectedOutput);
            }
        }

        private static (string jsonPath, string bicepparamPath) Setup(TestContext context, string template, string? inputFile = null, string? outputDir = null)
        {
            var jsonPath = FileHelper.SaveResultFile(context, inputFile ?? "param.json", template);

            var bicepparamPath = outputDir is null
              ? PathHelper.GetBicepparamOutputPath(jsonPath)
              : FileHelper.GetResultFilePath(context, outputDir);

            return (jsonPath, bicepparamPath);
        }
    }
}
