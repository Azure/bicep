// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using System.Linq;
using System.Text;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Semantics;

[TestClass]
public class DescriptionHelperTests
{
    [DataTestMethod]
    [DataRow(
        @"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""metadata"": {
            ""_generator"": {
              ""name"": ""bicep"",
              ""version"": ""0.17.61.37575"",
              ""templateHash"": ""15672661467945462838""
            },
            ""description"": ""template1.json description""
          },
          ""resources"": []
        }",
        "template1.json description"
    )]
    [DataRow(
        @"{
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""metadata"": {
            ""_generator"": {
              ""name"": ""bicep"",
              ""version"": ""0.17.61.37575"",
              ""templateHash"": ""15672661467945462838""
            },
            ""descriptionNotReally"": ""template1.json description""
          },
          ""resources"": []
        }",
        null
    )]
    public void TryGetFromArmTemplate(string json, string? expectedDescription)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var description = DescriptionHelper.TryGetFromArmTemplate(stream);

        description.Should().Be(expectedDescription);
    }

    [DataTestMethod]
    [DataRow(
        @"{
          ""location"": ""westus2"",
          ""tags"": {},
          ""properties"": {
            ""mainTemplate"": {
              ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
              ""contentVersion"": ""1.0.0.0"",
              ""metadata"": {
                ""_generator"": {
                  ""name"": ""bicep"",
                  ""version"": ""0.17.1.54307"",
                  ""templateHash"": ""3268788020119860428""
                },
                ""description"": ""templatespec description""
              },
              ""parameters"": {
                ""storageAccountType"": {
                  ""type"": ""string"",
                  ""defaultValue"": ""Standard_LRS"",
                  ""allowedValues"": [
                    ""Standard_LRS"",
                    ""Standard_GRS"",
                    ""Standard_ZRS"",
                    ""Premium_LRS""
                  ]
                },
                ""loc"": {
                  ""type"": ""string"",
                  ""defaultValue"": ""[resourceGroup().location]""
                }
              },
              ""variables"": {
                ""prefix"": ""user""
              },
              ""resources"": [
                {
                  ""type"": ""Microsoft.Storage/storageAccounts"",
                  ""apiVersion"": ""2021-04-01"",
                  ""name"": ""[format('{0}{1}', variables('prefix'), uniqueString(resourceGroup().id))]"",
                  ""location"": ""[parameters('loc')]"",
                  ""sku"": {
                    ""name"": ""[parameters('storageAccountType')]""
                  },
                  ""kind"": ""StorageV2""
                }
              ]
            }
          },
          ""systemData"": {
            ""createdBy"": ""user@microsoft.com"",
            ""createdByType"": ""User"",
            ""createdAt"": ""2023-05-20T20:59:12.912719Z"",
            ""lastModifiedBy"": ""user@microsoft.com"",
            ""lastModifiedByType"": ""User"",
            ""lastModifiedAt"": ""2023-05-20T20:59:12.912719Z""
          },
          ""id"": ""/subscriptions/e5ef2b13-6478-4887-ad57-1aa6b9475040/resourceGroups/sawbicep/providers/Microsoft.Resources/templateSpecs/storageSpec/versions/2.0a"",
          ""type"": ""Microsoft.Resources/templateSpecs/versions"",
          ""name"": ""2.0a""
        }",
        "templatespec description"
    )]
    [DataRow(
        @"{
          ""location"": ""westus2"",
          ""tags"": {},
          ""properties"": {
            ""mainTemplate"": {
              ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
              ""contentVersion"": ""1.0.0.0"",
              ""metadata"": {
                ""_generator"": {
                  ""name"": ""bicep"",
                  ""version"": ""0.17.1.54307"",
                  ""templateHash"": ""3268788020119860428""
                }
              },
              ""parameters"": {
                ""storageAccountType"": {
                  ""type"": ""string"",
                  ""defaultValue"": ""Standard_LRS"",
                  ""allowedValues"": [
                    ""Standard_LRS"",
                    ""Standard_GRS"",
                    ""Standard_ZRS"",
                    ""Premium_LRS""
                  ]
                },
                ""loc"": {
                  ""type"": ""string"",
                  ""defaultValue"": ""[resourceGroup().location]""
                }
              },
              ""variables"": {
                ""prefix"": ""user""
              },
              ""resources"": [
                {
                  ""type"": ""Microsoft.Storage/storageAccounts"",
                  ""apiVersion"": ""2021-04-01"",
                  ""name"": ""[format('{0}{1}', variables('prefix'), uniqueString(resourceGroup().id))]"",
                  ""location"": ""[parameters('loc')]"",
                  ""sku"": {
                    ""name"": ""[parameters('storageAccountType')]""
                  },
                  ""kind"": ""StorageV2""
                }
              ]
            }
          },
          ""systemData"": {
            ""createdBy"": ""user@microsoft.com"",
            ""createdByType"": ""User"",
            ""createdAt"": ""2023-05-20T20:59:12.912719Z"",
            ""lastModifiedBy"": ""user@microsoft.com"",
            ""lastModifiedByType"": ""User"",
            ""lastModifiedAt"": ""2023-05-20T20:59:12.912719Z""
          },
          ""id"": ""/subscriptions/e5ef2b13-6478-4887-ad57-1aa6b9475040/resourceGroups/sawbicep/providers/Microsoft.Resources/templateSpecs/storageSpec/versions/2.0a"",
          ""type"": ""Microsoft.Resources/templateSpecs/versions"",
          ""name"": ""2.0a""
        }",
        null
    )]
    public void TryGetFromTemplateSpec(string json, string? expectedDescription)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var description = DescriptionHelper.TryGetFromTemplateSpec(stream);

        description.Should().Be(expectedDescription);
    }
}
