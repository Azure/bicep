// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.LangServer.UnitTests.Mocks;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.JsonRpc;
using System;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using static Bicep.LangServer.UnitTests.Handlers.BicepDecompileForPasteCommandHandlerTests;
using static Bicep.LanguageServer.Telemetry.BicepTelemetryEvent;
using IOFileSystem = System.IO.Abstractions.FileSystem;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics.CodeAnalysis;
using SharpYaml.Tokens;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepDecompileForPasteCommandHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private BicepDecompileForPasteCommandHandler CreateHandler(LanguageServerMock server)
        {
            var builder = ServiceBuilder.Create(services => services
                .AddSingleton(StrictMock.Of<ISerializer>().Object)
                .AddSingleton(BicepTestConstants.CreateMockTelemetryProvider().Object)
                .AddSingleton(server.Mock.Object)
                .AddSingleton<BicepDecompileForPasteCommandHandler>()
                );

            return builder.Construct<BicepDecompileForPasteCommandHandler>();
        }

        public enum PasteType
        {
            None,
            FullTemplate,
            SingleResource,
            ResourceList,
            JsonValue,
        }

        private async Task TestDecompileForPaste(
            string json,
            PasteType expectedPasteType,
            string? expectedBicep,
            string? expectedErrorMessage = null)
        {
            LanguageServerMock server = new LanguageServerMock();
            var handler = CreateHandler(server);
            var result = await handler.Handle(new BicepDecompileForPasteCommandParams(json, queryCanPaste: false), CancellationToken.None);

            result.ErrorMessage.Should().Be(expectedErrorMessage);

            expectedBicep = expectedBicep?.Trim('\n');
            string? actualBicep = result.Bicep?.Trim('\n');
            actualBicep.Should().EqualTrimmedLines(expectedBicep);
            result.PasteType.Should().Be(expectedPasteType switch
            {
                PasteType.None => BicepDecompileForPasteCommandHandler.PasteType_None,
                PasteType.FullTemplate => BicepDecompileForPasteCommandHandler.PasteType_FullTemplate,
                PasteType.SingleResource => BicepDecompileForPasteCommandHandler.PasteType_SingleResource,
                PasteType.ResourceList => BicepDecompileForPasteCommandHandler.PasteType_ResourceList,
                PasteType.JsonValue => BicepDecompileForPasteCommandHandler.PasteType_JsonValue,
                _ => throw new NotImplementedException(),
            });
        }

        #region JSON/Bicep Constants

        private const string jsonFullTemplateMembers = @"
                  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
                  ""contentVersion"": ""1.0.0.0"",
                  ""parameters"": {
                    ""location"": {
                      ""type"": ""string"",
                      ""defaultValue"": ""[resourceGroup().location]""
                    }
                  },
                  ""resources"": [
                    {
                      ""type"": ""Microsoft.Storage/storageAccounts"",
                      ""apiVersion"": ""2021-02-01"",
                      ""name"": ""name"",
                      ""location"": ""[parameters('location')]"",
                      ""kind"": ""StorageV2"",
                      ""sku"": {
                        ""name"": ""Premium_LRS""
                      }
                    }
                  ]";
        private const string jsonFullTemplate = $@"{{
{jsonFullTemplateMembers}
}}";

        const string Resource1Json = @" {
                      ""type"": ""Microsoft.Storage/storageAccounts"",
                      ""apiVersion"": ""2021-02-01"",
                      ""name"": ""name1"",
                      ""location"": ""eastus"",
                      ""kind"": ""StorageV2"",
                      ""sku"": {
                        ""name"": ""Premium_LRS""
                      }
                    }";
        const string Resource2Json = @" {
                      ""type"": ""Microsoft.Storage/storageAccounts"",
                      ""apiVersion"": ""2021-02-01"",
                      ""name"": ""name2"",
                      ""location"": ""eastus"",
                      ""kind"": ""StorageV2"",
                      ""sku"": {
                        ""name"": ""Premium_LRS""
                      }
                    }";

        const string Resource1Bicep = @"resource name1 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'name1'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Premium_LRS'
  }
}";
        const string Resource2Bicep = @"resource name2 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'name2'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Premium_LRS'
  }
}";

        #endregion

        [DataRow(
            jsonFullTemplate,
            PasteType.FullTemplate,
            @"
                param location string = resourceGroup().location

                resource name 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name'
                  location: location
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }",
            DisplayName = "Full template"
        )]
        [DataRow(
            $@"{{
{jsonFullTemplateMembers}
    , extraProperty: ""hello""
}}",
            PasteType.FullTemplate,
            @"
                param location string = resourceGroup().location

                resource name 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name'
                  location: location
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }",
            DisplayName = "Extra property"
        )]
        [DataRow(
            $@"{{
{jsonFullTemplateMembers}
}}
}} // extra",
            PasteType.FullTemplate,
            @"
                param location string = resourceGroup().location

                resource name 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name'
                  location: location
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }",
            DisplayName = "Extra brace at end (succeeds)"
        )]
        [DataRow(
            $@"{{
{jsonFullTemplateMembers}
}}
random characters",
            PasteType.FullTemplate,
            @"
                param location string = resourceGroup().location

                resource name 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name'
                  location: location
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }",
            DisplayName = "Extra random characters at end"
        )]
        [DataRow(
            $@"{{
{{ // extra
{jsonFullTemplateMembers}
}}
random characters",
            PasteType.None,
            null,
            DisplayName = "Extra brace at beginning (can't paste)"
        )]
        [DataRow(
            $@"
random characters
{{
{jsonFullTemplateMembers}
}}",
            PasteType.None,
            null,
            DisplayName = "Extra random characters at beginning (can't paste)"
        )]
        [DataRow(
            $@"{{
                  ""$schema"": {{}},
                  ""parameters"": {{
                    ""location"": {{
                      ""type"": ""string"",
                      ""defaultValue"": ""[resourceGroup().location]""
                    }}
                  }}
            }}",
            PasteType.JsonValue,
            // Treats it simply as a JSON object
            $@"{{
                '$schema': {{
                }}
                parameters: {{
                    location: {{
                        type: 'string'
                        defaultValue: resourceGroup().location
                    }}
                }}
            }}",
            DisplayName = "Schema not a string"
        )]
        [DataTestMethod]
        public async Task FullTemplate(string json, PasteType expectedPasteType, string expectedBicep, string? errorMessage = null)
        {
            await TestDecompileForPaste(
                    json: json,
                    expectedPasteType: expectedPasteType,
                    expectedBicep: expectedBicep,
                    errorMessage);
        }

        public async Task PasteFullTemplate_ButNoSchema_CantConvert()
        {
            const string json = @"
                {
                  ""contentVersion"": ""1.0.0.0"",
                  ""parameters"": {
                    ""location"": {
                      ""type"": ""string"",
                      ""defaultValue"": ""[resourceGroup().location]""
                    }
                  },
                  ""resources"": [
                    {
                      ""type"": ""Microsoft.Storage/storageAccounts"",
                      ""apiVersion"": ""2021-02-01"",
                      ""name"": ""name"",
                      ""location"": ""[parameters('location')]"",
                      ""kind"": ""StorageV2"",
                      ""sku"": {
                        ""name"": ""Premium_LRS""
                      }
                    }
                  ]
                }";
            await TestDecompileForPaste(
                    json: json,
                    expectedPasteType: PasteType.None,
                    expectedErrorMessage: null,
                    expectedBicep: null);
        }

        [TestMethod]
        public async Task FullTemplate_ButMissingParameter_ShouldGiveError()
        {
            const string json = @"
                {
                  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
                  ""contentVersion"": ""1.0.0.0"",
                  ""parameters"": {
                  },
                  ""resources"": [
                    {
                      ""type"": ""Microsoft.Storage/storageAccounts"",
                      ""apiVersion"": ""2021-02-01"",
                      ""name"": ""name"",
                      ""location"": ""[parameters('location')]"",
                      ""kind"": ""StorageV2"",
                      ""sku"": {
                        ""name"": ""Premium_LRS""
                      }
                    }
                  ]
                }";
            await TestDecompileForPaste(
                    json,
                    expectedPasteType: PasteType.FullTemplate,
                    expectedErrorMessage: "[12:60]: Unable to find parameter location",
                    expectedBicep: null);
        }

        [DataRow(
            @"
                {
                  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
                  ""contentVersion"": ""1.0.0.0"",
                  ""metadata"": {
                    ""_generator"": {
                      ""name"": ""bicep"",
                      ""version"": ""0.10.61.36676"",
                      ""templateHash"": ""8521684133784798165""
                    }
                  },
                  ""resources"": { // whoops
                    {
                      ""type"": ""Microsoft.Compute/virtualMachines/providers/configurationProfileAssignments"",
                      ""apiVersion"": ""2022-05-04"",
                      ""name"": ""vmName/Microsoft.Automanage/default"",
                      ""properties"": {
                        ""configurationProfile"": ""/providers/Microsoft.Automanage/bestPractices/AzureBestPracticesDevTest""
                      }
                    }
                  ]",
            PasteType.None,
            null,
            null,
            DisplayName = "{ instead of ["
            )]
        [DataRow(
            @"{
                ""type"": ""Microsoft.Compute/virtualMachines/providers/configurationProfileAssignments"",
                ""apiVersion"": ""2022-12-12"",
                ""name"": ""name"",
                ""properties"": {
                    ""configurationProfile"": ""[bad-expression]""
                }
            }",
            PasteType.SingleResource,
            null,
            "[6:46]: The language expression 'bad-expression' is not valid: the string character 'x' at position '5' is not expected.",
            DisplayName = "Bad expression"
            )]
        [DataTestMethod]
        public async Task Errors(string json, PasteType pasteType, string? expectedBicep, string? expectedErrorMessage)
        {
            await TestDecompileForPaste(json, pasteType, expectedBicep, expectedErrorMessage);
        }

        [TestMethod]
        public async Task JustString_WithNoQuotes_CantConvert()
        {
            string json = @"just a string";
            await TestDecompileForPaste(
                    json: json,
                    PasteType.None,
                    expectedErrorMessage: null,
                    expectedBicep: null);
        }

        [TestMethod]
        public async Task NonResourceObject_WrongPropertyType_Object_PastesAsSimpleObject()
        {
            string json = @$"
                {Resource1Json.Replace("\"2021-02-01\"", "{}")}
            ";
            await TestDecompileForPaste(
                    json: json,
                    PasteType.JsonValue,
                    expectedErrorMessage: null,
                    expectedBicep: @"
                        {
                            type: 'Microsoft.Storage/storageAccounts'
                            apiVersion: {
                            }
                            name: 'name1'
                            location: 'eastus'
                            kind: 'StorageV2'
                            sku: {
                                name: 'Premium_LRS'
                            }
                        }");
        }

        [TestMethod]
        public async Task NonResourceObject_WrongPropertyType_Number_PastesAsSimpleObject()
        {
            string json = @$"
                {Resource1Json.Replace("\"2021-02-01\"", "1234")}
            ";
            await TestDecompileForPaste(
                    json: json,
                    PasteType.JsonValue,
                    expectedErrorMessage: null,
                    expectedBicep: @"
                        {
                            type: 'Microsoft.Storage/storageAccounts'
                            apiVersion: 1234
                            name: 'name1'
                            location: 'eastus'
                            kind: 'StorageV2'
                            sku: {
                                name: 'Premium_LRS'
                            }
                        }");
        }

        [TestMethod]
        public async Task MissingParametersAndVars()
        {
            string json = @"
                {
                        ""type"": ""Microsoft.Storage/storageAccounts"",
                        ""apiVersion"": ""2021-02-01"",
                        ""name"": ""name"",
                        ""location"": ""[parameters('location')]"",
                        ""kind"": ""[variables('storageKind')]"",
                        ""sku"": {
                          ""name"": ""[variables('sku')]""
                        }
                }
            ";
            string expected = @"resource name 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'name'
  location: location
  kind: storageKind
  sku: {
    name: sku
  }
}";

            await TestDecompileForPaste(
                    json: json,
                    expectedPasteType: PasteType.SingleResource,
                    expectedBicep: expected);
        }

        [TestMethod]
        public async Task MissingParametersAndVars_Conflict()
        {
            string json = @"
                {
                        ""type"": ""Microsoft.Storage/storageAccounts"",
                        ""apiVersion"": ""2021-02-01"",
                        ""name"": ""name"",
                        ""location"": ""[concat(parameters('location'), variables('location'), parameters('location_var'), variables('location_var'), parameters('location_param'), variables('location_param'))]"",
                        ""kind"": ""[variables('location')]"",
                        ""sku"": {
                          ""name"": ""Premium_LRS""
                        }
                }
            ";

            await TestDecompileForPaste(
                    json: json,
                    expectedErrorMessage: null,
                    expectedPasteType: PasteType.SingleResource,
                    expectedBicep: @"
                    resource name 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                        name: 'name'
                        location: concat(location, location_var, location_var, location_var_var, location_param, location_param_var)
                        kind: location_var
                        sku: {
                            name: 'Premium_LRS'
                        }
                    }");
        }

        [TestMethod]
        public async Task SingleResourceObject_ShouldSucceed()
        {
            string json = @"
                    {
                      ""type"": ""Microsoft.Storage/storageAccounts"",
                      ""apiVersion"": ""2021-02-01"",
                      ""name"": ""name"",
                      ""location"": ""eastus"",
                      ""kind"": ""StorageV2"",
                      ""sku"": {
                        ""name"": ""Premium_LRS""
                      }
                    }";
            string expected = @"
                resource name 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name'
                  location: 'eastus'
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }";

            await TestDecompileForPaste(
                    json: json,
                    PasteType.SingleResource,
                    expectedErrorMessage: null,
                    expectedBicep: expected);
        }

        [TestMethod]
        public async Task MultipleResourceObjects_ShouldSucceed()
        {
            string json = @"
                    {
                      ""type"": ""Microsoft.Storage/storageAccounts"",
                      ""apiVersion"": ""2021-02-01"",
                      ""name"": ""name1"",
                      ""location"": ""eastus"", // comment and blank line

                      ""kind"": ""StorageV2"",
                      ""sku"": {
                        ""name"": ""Premium_LRS""
                      }
                    },
                    {
                      ""type"": ""Microsoft.Storage/storageAccounts"",
                      ""apiVersion"": ""2021-02-01"",
                      ""name"": ""name2"",
                      ""location"": ""eastus"",
                      ""kind"": ""StorageV2"",
                      ""sku"": {
                        ""name"": ""Premium_LRS""
                      }
                    },
                    {
                      ""type"": ""Microsoft.Storage/storageAccounts"",
                      ""apiVersion"": ""2021-02-01"",
                      ""name"": ""name3"",
                      ""location"": ""eastus"",
                      ""kind"": ""StorageV2"",
                      ""sku"": {
                        ""name"": ""Premium_LRS""
                      }
                    }";
            string expected = @"
                resource name1 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name1'
                  location: 'eastus'
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }

                resource name2 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name2'
                  location: 'eastus'
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }

                resource name3 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name3'
                  location: 'eastus'
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }";

            await TestDecompileForPaste(
                    json: json,
                    PasteType.ResourceList,
                    expectedErrorMessage: null,
                    expectedBicep: expected);
        }

        [TestMethod]
        public async Task MultipleResourceObjects_SkipTrivia_ShouldSucceed()
        {
            string json = $@"

                    // This is a comment
                    // So is this
                    /* And this
                    also */

            {Resource1Json}
                    // This is a comment
                    // So is this
                    /* And this
                    also */
,
                    // This is a comment
                    // So is this
                    /* And this
                    also */

                    {Resource2Json} // This is a comment
                    // So is this
                    /* And this
                    also */";
            ;
            string expected = $@"
                {Resource1Bicep}

                {Resource2Bicep}";

            await TestDecompileForPaste(
                    json: json,
                    PasteType.ResourceList,
                    expectedErrorMessage: null,
                    expectedBicep: expected);
        }

        [TestMethod]
        public async Task MultipleResourceObjects_NoComma_ShouldSucceed()
        {
            string json = @"
                    {
                      ""type"": ""Microsoft.Storage/storageAccounts"",
                      ""apiVersion"": ""2021-02-01"",
                      ""name"": ""name1"",
                      ""location"": ""eastus"", // comment and blank line

                      ""kind"": ""StorageV2"",
                      ""sku"": {
                        ""name"": ""Premium_LRS""
                      }
                    }
                    {
                      ""type"": ""Microsoft.Storage/storageAccounts"",
                      ""apiVersion"": ""2021-02-01"",
                      ""name"": ""name2"",
                      ""location"": ""eastus"",
                      ""kind"": ""StorageV2"",
                      ""sku"": {
                        ""name"": ""Premium_LRS""
                      }
                    }";
            string expected = @"
                resource name1 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name1'
                  location: 'eastus'
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }

                resource name2 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name2'
                  location: 'eastus'
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }";

            await TestDecompileForPaste(
                    json: json,
                    PasteType.ResourceList,
                    expectedErrorMessage: null,
                    expectedBicep: expected);
        }

        [TestMethod]
        public async Task Modules()
        {
            const string json = @"
            {
                ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
                ""contentVersion"": ""1.0.0.0"",
                ""resources"": [
                {
                    ""name"": ""nestedDeploymentInner"",
                    ""type"": ""Microsoft.Resources/deployments"",
                    ""apiVersion"": ""2021-04-01"",
                    ""properties"": {
                    ""expressionEvaluationOptions"": {
                        ""scope"": ""inner""
                    },
                    ""mode"": ""Incremental"",
                    ""parameters"": {},
                    ""template"": {
                        ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
                        ""contentVersion"": ""1.0.0.0"",
                        ""parameters"": {},
                        ""variables"": {},
                        ""resources"": [
                        {
                            ""name"": ""storageaccount1"",
                            ""type"": ""Microsoft.Storage/storageAccounts"",
                            ""apiVersion"": ""2021-04-01"",
                            ""tags"": {
                            ""displayName"": ""storageaccount1""
                            },
                            ""location"": ""[resourceGroup().location]"",
                            ""kind"": ""StorageV2"",
                            ""sku"": {
                            ""name"": ""Premium_LRS"",
                            ""tier"": ""Premium""
                            }
                        }
                        ],
                        ""outputs"": {}
                    }
                    }
                },
                {
                    ""name"": ""nestedDeploymentOuter"",
                    ""type"": ""Microsoft.Resources/deployments"",
                    ""apiVersion"": ""2021-04-01"",
                    ""properties"": {
                    ""mode"": ""Incremental"",
                    ""template"": {
                        ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
                        ""contentVersion"": ""1.0.0.0"",
                        ""variables"": {},
                        ""resources"": [
                        {
                            ""name"": ""storageaccount2"",
                            ""type"": ""Microsoft.Storage/storageAccounts"",
                            ""apiVersion"": ""2021-04-01"",
                            ""tags"": {
                            ""displayName"": ""storageaccount2""
                            },
                            ""location"": ""[resourceGroup().location]"",
                            ""kind"": ""StorageV2"",
                            ""sku"": {
                            ""name"": ""Premium_LRS"",
                            ""tier"": ""Premium""
                            }
                        }
                        ],
                        ""outputs"": {}
                    }
                    }
                },
                {
                    ""name"": ""storageaccount"",
                    ""type"": ""Microsoft.Storage/storageAccounts"",
                    ""apiVersion"": ""2021-04-01"",
                    ""tags"": {
                    ""displayName"": ""storageaccount""
                    },
                    ""location"": ""[resourceGroup().location]"",
                    ""kind"": ""StorageV2"",
                    ""sku"": {
                    ""name"": ""Premium_LRS"",
                    ""tier"": ""Premium""
                    }
                },
                {
                    ""name"": ""nestedDeploymentInner2"",
                    ""type"": ""Microsoft.Resources/deployments"",
                    ""apiVersion"": ""2021-04-01"",
                    ""properties"": {
                    ""expressionEvaluationOptions"": {
                        ""scope"": ""inner""
                    },
                    ""mode"": ""Incremental"",
                    ""parameters"": {},
                    ""template"": {
                        ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
                        ""contentVersion"": ""1.0.0.0"",
                        ""parameters"": {},
                        ""variables"": {},
                        ""resources"": [],
                        ""outputs"": {}
                    }
                    }
                }
                ]
            }";

            string expected = @"
                module nestedDeploymentInner './nested_nestedDeploymentInner.bicep' = {
                  name: 'nestedDeploymentInner'
                  params: {
                  }
                }

                module nestedDeploymentOuter './nested_nestedDeploymentOuter.bicep' = {
                  name: 'nestedDeploymentOuter'
                  params: {
                  }
                }

                resource storageaccount 'Microsoft.Storage/storageAccounts@2021-04-01' = {
                  name: 'storageaccount'
                  tags: {
                    displayName: 'storageaccount'
                  }
                  location: resourceGroup().location
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                    tier: 'Premium'
                  }
                }

                module nestedDeploymentInner2 './nested_nestedDeploymentInner2.bicep' = {
                  name: 'nestedDeploymentInner2'
                  params: {
                  }
                }";

            await TestDecompileForPaste(
                json: json,
                PasteType.FullTemplate,
                expectedErrorMessage: null,
                expectedBicep: expected);
        }

        [TestMethod]
        public async Task SingleResource_WithNestedInnerScopedTemplateWithMissingParam_ShouldSucceed()
        {
            const string json = @"
            {
                ""name"": ""nestedDeploymentInner"",
                ""type"": ""Microsoft.Resources/deployments"",
                ""apiVersion"": ""2021-04-01"",
                ""properties"": {
                    ""mode"": ""Incremental"",
                    ""expressionEvaluationOptions"": {
                        ""scope"": ""inner""
                    },
                    ""template"": {
                        ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
                        ""contentVersion"": ""1.0.0.0"",
                        ""resources"": [
                            {
                                ""name"": ""storageaccount1"",
                                ""type"": ""Microsoft.Storage/storageAccounts"",
                                ""apiVersion"": ""2021-04-01"",
                                // Refers to a local parameter in the module, which is missing.
                                // This should cause error during conversion (because the nested template should be valid),
                                // although a missing parameter at the top level would not (because the top level is
                                //   not expected to be complete).
                                ""location"": ""[parameters('location')]""
                            }
                        ]
                    }
                }
            }";

            await TestDecompileForPaste(
                json: json,
                PasteType.SingleResource,
                expectedErrorMessage: "[18:48]: Unable to find parameter location",
                expectedBicep: null);
        }

        [TestMethod]
        public async Task SingleResource_WithNestedOuterScopedTemplate_WithMissingParam_ShouldBePastable_ButShouldGiveError()
        {
            const string json = @"
            {
                ""name"": ""nestedDeploymentInner"",
                ""type"": ""Microsoft.Resources/deployments"",
                ""apiVersion"": ""2021-04-01"",
                ""properties"": {
                    ""mode"": ""Incremental"",
                    ""template"": {
                        ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
                        ""contentVersion"": ""1.0.0.0"",
                        ""resources"": [
                            {
                                ""name"": ""storageaccount1"",
                                ""type"": ""Microsoft.Storage/storageAccounts"",
                                ""apiVersion"": ""2021-04-01"",
                                // Refers to a local parameter in the module, which is missing.
                                // This should cause error during conversion (because the nested template should be valid),
                                // although a missing parameter at the top level would not (because the top level is
                                //   not expected to be complete).
                                ""location"": ""[parameters('location')]""
                            }
                        ]
                    }
                }
            }";

            await TestDecompileForPaste(
                json: json,
                PasteType.SingleResource,
                expectedErrorMessage: null,
                expectedBicep: @"
                    module nestedDeploymentInner './nested_nestedDeploymentInner.bicep' = {
                        name: 'nestedDeploymentInner'
                        params: {
                            location: location
                        }
                    }");
        }

        [TestMethod]
        public async Task MultipleResourceObjects_ExtraBraceAfterwards_ShouldSucceed()
        {
            string json = @$"
                    {Resource1Json}
                    {Resource2Json}
                }}}} // extra";

            await TestDecompileForPaste(
                    json: json,
                    expectedPasteType: PasteType.ResourceList,
                    expectedErrorMessage: null,
                    expectedBicep: @$"
                        {Resource1Bicep}

                        {Resource2Bicep}");
        }

        [TestMethod]
        public async Task MultipleResourceObjects_ExtraOpenBraceAfterwards_ShouldSucceed()
        {
            string json = @$"
                    {Resource1Json}
                    {Resource2Json}
                {{ // extra";

            await TestDecompileForPaste(
                    json: json,
                    expectedPasteType: PasteType.ResourceList,
                    expectedErrorMessage: null,
                    expectedBicep: @$"
                        {Resource1Bicep}

                        {Resource2Bicep}");
        }

        [TestMethod]
        public async Task MultipleResourceObjects_ExtraEmptyObjectAfterwards_ShouldSucceed()
        {
            string json = @$"
                    {Resource1Json}
                    {Resource2Json}
                {{}} // extra";

            await TestDecompileForPaste(
                    json: json,
                    expectedPasteType: PasteType.ResourceList,
                    expectedErrorMessage: null,
                    expectedBicep: @$"
                        {Resource1Bicep}

                        {Resource2Bicep}");
        }

        [TestMethod]
        public async Task MultipleResourceObjects_NameConflict_ShouldAllowPaste_ButGiveError()
        {
            string json = @$"
                    {Resource1Json}
                    {Resource1Json}
                    {Resource1Json}";

            await TestDecompileForPaste(
                    json: json,
                    expectedPasteType: PasteType.ResourceList,
                    expectedErrorMessage: "[21:1]: Unable to pick unique name for resource Microsoft.Storage/storageAccounts name1",
                    expectedBicep: null);
        }

        [TestMethod]
        public async Task MultipleResourceObjects_RandomCharactersAfterwards_ShouldSucceed_AndIgnoreRemaining()
        {
            string json = @$"
                    {Resource1Json}
                    {Resource2Json}
                something else {{ // extra";

            await TestDecompileForPaste(
                    json: json,
                    expectedPasteType: PasteType.ResourceList,
                    expectedErrorMessage: null,
                    expectedBicep: @$"
                        {Resource1Bicep}

                        {Resource2Bicep}"
            );
        }

        [TestMethod]
        public async Task MultipleResourceObjects_NonResourceInMiddle_ShouldSucceed_AndIgnoreNonResources()
        {
            string json = @$"
                    {Resource1Json}
                    {{
                        ""notAResource"": ""honest""
                    }}
                    {Resource2Json}
";

            await TestDecompileForPaste(
                    json: json,
                    expectedPasteType: PasteType.ResourceList,
                    expectedErrorMessage: null,
                    expectedBicep: $@"
                        {Resource1Bicep}

                        {Resource2Bicep}"
            );
        }

        [TestMethod]
        public async Task MultipleResourceObjects_ExtraCommaAtEnd_ShouldSucceed()
        {
            string json = @$"
                {Resource1Json}
                {Resource2Json}
                ,,, // extra";
            string expected = @$"
                {Resource1Bicep}

                {Resource2Bicep}";

            await TestDecompileForPaste(
                    json: json,
                    expectedPasteType: PasteType.ResourceList,
                    expectedErrorMessage: null,
                    expectedBicep: expected);
        }

        [TestMethod]
        public async Task MissingVariable_UsedMultipleTimes_ShouldSucceed()
        {
            string json = @" {
                      ""type"": ""Microsoft.Storage/storageAccounts"",
                      ""apiVersion"": ""2021-02-01"",
                      ""name"": ""name1"",
                      ""location"": ""[variables('v1')]"",
                      ""kind"": ""[variables('v1')]"",
                      ""sku"": {
                        ""name"": ""[variables('v1')]""
                      }
                    }";

            await TestDecompileForPaste(
                    json: json,
                    expectedPasteType: PasteType.SingleResource,
                    expectedErrorMessage: null,
                    expectedBicep: @"
                        resource name1 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                            name: 'name1'
                            location: v1
                            kind: v1
                            sku: {
                                name: v1
                            }
                        }");
        }

        [TestMethod]
        public async Task MissingVariable_UsedMultipleTimes_CasedDifferently_ShouldSucceed()
        {
            string json = @" {
                      ""type"": ""Microsoft.Storage/storageAccounts"",
                      ""apiVersion"": ""2021-02-01"",
                      ""name"": ""name1"",
                      ""location"": ""[variables('v1')]"",
                      ""kind"": ""[variables('v1')]"",
                      ""sku"": {
                        ""name"": ""[variables('V1')]""
                      }
                    }";

            await TestDecompileForPaste(
                    json: json,
                    expectedPasteType: PasteType.SingleResource,
                    expectedErrorMessage: null,
                    expectedBicep: @"
                        resource name1 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                            name: 'name1'
                            location: v1
                            kind: v1
                            sku: {
                                name: v1
                            }
                        }");
        }

        [TestMethod]
        public async Task MissingParameter_UsedMultipleTimes_ShouldSucceed()
        {
            string json = @"{
                      ""type"": ""Microsoft.Storage/storageAccounts"",
                      ""apiVersion"": ""2021-02-01"",
                      ""name"": ""name1"",
                      ""location"": ""[parameters('p1')]"",
                      ""kind"": ""[parameters('p1')]"",
                      ""sku"": {
                        ""name"": ""[parameters('p1')]""
                      }
                    }";

            await TestDecompileForPaste(
                    json: json,
                    expectedPasteType: PasteType.SingleResource,
                    expectedErrorMessage: null,
                    expectedBicep: @"
                        resource name1 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                            name: 'name1'
                            location: p1
                            kind: p1
                            sku: {
                                name: p1
                            }
                        }");
        }

        [TestMethod]
        public async Task MissingParameter_UsedMultipleTimes_CasedDifferently_ShouldSucceed()
        {
            string json = @" {
                      ""type"": ""Microsoft.Storage/storageAccounts"",
                      ""apiVersion"": ""2021-02-01"",
                      ""name"": ""name1"",
                      ""location"": ""[parameters('p1')]"",
                      ""kind"": ""[parameters('p1')]"",
                      ""sku"": {
                        ""name"": ""[parameters('P1')]""
                      }
                    }";

            await TestDecompileForPaste(
                    json: json,
                    expectedPasteType: PasteType.SingleResource,
                    expectedErrorMessage: null,
                    expectedBicep: @"
                        resource name1 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                            name: 'name1'
                            location: p1
                            kind: p1
                            sku: {
                                name: p1
                            }
                        }");
        }

        [TestMethod]
        public async Task MissingParameterVariable_CollidesWithResourceName_ShouldSucceed()
        {
            string json = @" {
                      ""type"": ""Microsoft.Storage/storageAccounts"",
                      ""apiVersion"": ""2021-02-01"",
                      ""name"": ""v1"",
                      ""location"": ""[variables('v1')]"",
                      ""kind"": ""[parameters('v1')]"",
                      ""sku"": {
                        ""name"": ""Premium_LRS""
                      }
                    }";

            await TestDecompileForPaste(
                    json: json,
                    expectedPasteType: PasteType.SingleResource,
                    expectedErrorMessage: null,
                    expectedBicep: @"
                        resource v1 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                            name: 'v1'
                            location: v1_var
                            kind: v1_param
                            sku: {
                                name: 'Premium_LRS'
                            }
                        }");
        }

        [TestMethod]
        public async Task MultilineStrings_ShouldSucceed()
        {
            string json = @"{
  ""type"": ""Microsoft.Compute/virtualMachines"",
  ""apiVersion"": ""2018-10-01"",
  ""name"": ""[variables('vmName')]"", // to customize name, change it in variables
  ""location"": ""[
    parameters('location')
    ]"",
}";

            await TestDecompileForPaste(
                    json: json,
                    expectedPasteType: PasteType.SingleResource,
                    expectedErrorMessage: null,
                    expectedBicep: @"
                        resource vm 'Microsoft.Compute/virtualMachines@2018-10-01' = {
                          name: vmName
                          location: location
                        }
");
        }

        [DataRow(
            @"'just a string with single quotes'",
            null, // Already valid bicep so doesn't get converted
            DisplayName = "String with single quotes"
        )]
        [DataRow(
            @"""just a string with double quotes""",
            @"'just a string with double quotes'",
            DisplayName = "String with double quotes"
        )]
        [DataRow(
            @"{""hello"": ""there""}",
            @"{
                hello: 'there'
            }",
            DisplayName = "simple object"
        )]
        [DataRow(
            @"{""hello there"": ""again""}",
            @"{
                'hello there': 'again'
            }",
            DisplayName = "object with properties needing quotes"
        )]
        [DataRow(
            @"""[resourceGroup().location]""",
            @"resourceGroup().location",
            DisplayName = "String with ARM expression"
        )]
        [DataRow(
            @"[""[resourceGroup().location]""]",
            @"[
  resourceGroup().location
]",
            DisplayName = "Array with string expression"
        )]
        [DataRow(
            @"""[concat(variables('leftBracket'), 'dbo', variables('rightBracket'), '.', variables('leftBracket'), 'table', variables('rightBracket')) ]""",
            @"'${leftBracket}dbo${rightBracket}.${leftBracket}table${rightBracket}'",
            DisplayName = "concat changes to interpolated string"
        )]
        [DataRow(
            @"""[concat('Correctly escaped single quotes ''here'' and ''''here'''' ', variables('and'), ' ''wherever''')]""",
            @"'Correctly escaped single quotes \'here\' and \'\'here\'\' ${and} \'wherever\''",
            DisplayName = "Correctly escaped single quotes"
        )]
        [DataRow(
            @"""[concat('string', ' ', 'string')]""",
            @"'string string'",
            DisplayName = "Concat is simplified"
        )]
        [DataRow(
            @"""[concat('''Something in single quotes - '' ', 'and something not ', variables('v1'))]""",
            @"'\'Something in single quotes - \' and something not ${v1}'",
            DisplayName = "Escaped and unescaped single quotes in string"
        )]
        [DataRow(
            @"""[[this will be in brackets, not an expression - variables('blobName') should not be converted to Bicep, but single quotes should be escaped]""",
            @"'[this will be in brackets, not an expression - variables(\'blobName\') should not be converted to Bicep, but single quotes should be escaped]'",
            DisplayName = "string starting with [[ is not an expression, [[ should get converted to single ["
        )]
        [DataRow(
            @"""[json(concat('{\""storageAccountType\"": \""Premium_LRS\""}'))]""",
            @"json('{""storageAccountType"": ""Premium_LRS""}')",
            DisplayName = "double quotes inside strings inside object"
        )]
        [DataRow(
            @"""[concat(variables('blobName'),parameters('blobName'))]""",
            "concat(blobName, blobName_param)",
            DisplayName = "param and variable with same name"
        )]
        [DataRow(
            @"""Double quotes \""here\""""",
            @"'Double quotes ""here""'",
            DisplayName = "Double quotes in string"
        )]
        [DataRow(
            @"'Double quotes \""here\""'",
            @"'Double quotes ""here""'",
            DisplayName = "Double quotes in single-quote string"
        )]
        [DataRow(
            @"""['Double quotes \""here\""']""",
            @"'Double quotes ""here""'",
            DisplayName = "Double quotes in string inside string expression"
        )]
        [DataRow(
            @""" [A string that has whitespace before the bracket is not an expression]""",
            @"' [A string that has whitespace before the bracket is not an expression]'",
            DisplayName = "Whitespace before the bracket"
        )]
        [DataRow(
            @"""[A string that has whitespace after the bracket is not an expression] """,
            @"'[A string that has whitespace after the bracket is not an expression] '",
            DisplayName = "Whitespace after the bracket"
        )]
        [DataRow(
            @"[
    1, 2,
    3
]",
            @"[
  1
  2
  3
]",
            DisplayName = "Multiline array"
        )]
        [DataRow(
            "  \t \"abc\" \t  ",
            "'abc'",
            DisplayName = "Whitespace before/after")]
        [DataRow(
            "  /*comment*/\n // another comment\r\n /*hi*/\"abc\"// there\n/*and another*/ // but wait, there's more!\n// end",
            "'abc'",
            DisplayName = "Comments before/after")]
        [DataRow(
            "\t\"abc\"\t",
            "'abc'",
            DisplayName = "Tabs before/after")]
        [DataRow(
            "\"2012-03-21T05:40Z\"",
            "'2012-03-21T05:40Z'",
            DisplayName = "datetime string")]
        [DataTestMethod]
        public async Task JsonValue_Valid_ShouldSucceed(string json, string expectedBicep)
        {
            await TestDecompileForPaste(
                    json: json,
                    expectedBicep is null ? PasteType.None : PasteType.JsonValue,
                    expectedErrorMessage: null,
                    expectedBicep: expectedBicep);
        }

        [DataTestMethod]
        [DataRow(
            @"{
              ipConfigurations: [
                {
                  name: 'ipconfig1'
                  properties: {
                    subnet: {
                      id: 'subnetRef'
                    }
                    privateIPAllocationMethod: 'Dynamic'
                    publicIpAddress: {
                      id: resourceId('Microsoft.Network/publicIPAddresses', 'publicIPAddressName')
                    }
                  }
                }
              ]
              networkSecurityGroup: {
                id: resourceId('Microsoft.Network/networkSecurityGroups', 'networkSecurityGroupName')
              }
            }",
            DisplayName = "Bicep object"
        )]
        [DataRow(
            @"3/14/2001",
            DisplayName = "Date"
        )]
        [DataRow(
            @"""kubernetesVersion"": ""1.15.7""",
            DisplayName = "\"property\": \"value\""
        )]
        [DataRow(
            @"// hello there",
            DisplayName = "single-line comment"
        )]
        [DataRow(
            @"/* hello
                there */",
            DisplayName = "multi-line comment"
        )]
        [DataRow(
            @"resourceGroup().location",
            DisplayName = "Invalid JSON expression"
        )]
        [DataRow(
            @"[resourceGroup().location]",
            DisplayName = "Invalid JSON expression inside array"
        )]
        [DataRow(
            @"[concat('Unescaped single quotes 'here' ')]",
            DisplayName = "Invalid unescaped single quotes"
        )]
        [DataRow(
            @"",
            DisplayName = "Empty")]
        [DataRow(
            @"null",
            DisplayName = "null")]
        [DataRow(
            "  \t  ",
            DisplayName = "just whitespace")]
        [DataRow(
            @" /* hello there! */ // more comment",
            DisplayName = "just a comment")]
        [DataRow(
            "2012-03-21T05:40Z",
            DisplayName = "datetime")]
        public async Task JsonValue_Invalid_CantConvert(string json)
        {
            await TestDecompileForPaste(
                    json: json,
                    PasteType.None,
                    expectedErrorMessage: null,
                    expectedBicep: null);
        }

        [DataTestMethod]
        [DataRow(@"{ abc: 1, def: 'def' }")] // this is not technically valid JSON but the Newtonsoft parser accepts it anyway and it is already valid Bicep so shouldn't be converted
        [DataRow(@"{ abc: 1, /*hi*/ def: 'def' }")] // this is not technically valid JSON but the Newtonsoft parser accepts it anyway and it is already valid Bicep so shouldn't be converted
        [DataRow(@"{
abc: 1
// comment
  def: 'def'
}")]
        [DataRow(@"[1]")]
        [DataRow(@"[1 1]")]
        [DataRow(@"[      /* */  ]")]
        [DataRow(@"[
/* */  ]")]
        [DataRow(@"[
  1]")]
        public async Task JsonValue_IsAlreadyLegalBicep_DontConvert_BecauseItWouldChangeJustFormatting(string json)
        {
            await TestDecompileForPaste(
                    json,
                    PasteType.None,
                    expectedErrorMessage: null,
                    expectedBicep: null);
        }

        [TestMethod]
        public async Task Template_CreatesEmptyBicep()
        {
            await TestDecompileForPaste(
                    @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": """",
  ""apiProfile"": """",
  ""parameters"": {  },
  ""variables"": {  },
  ""functions"": [  ],
  ""resources"": [  ],
  ""outputs"": {  }
}",
                    PasteType.None,
                    expectedErrorMessage: null,
                    expectedBicep: null);
        }
    }
}

