// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices.Protocols;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.Decompiler;
using Bicep.LangServer.UnitTests.Mocks;
using Bicep.LanguageServer;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Providers;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using static Bicep.LanguageServer.Telemetry.BicepTelemetryEvent;
using IOFileSystem = System.IO.Abstractions.FileSystem;

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
            Resources,
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
            actualBicep.Should().EqualIgnoringIndent(expectedBicep);
            result.PasteType.Should().Be(expectedPasteType switch
            {
                PasteType.None => BicepDecompileForPasteCommandHandler.PasteType_None,
                PasteType.FullTemplate => BicepDecompileForPasteCommandHandler.PasteType_FullTemplate,
                PasteType.Resources => BicepDecompileForPasteCommandHandler.PasteType_ResourceObject,
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

        [DataTestMethod]
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
            PasteType.None,
            null,
            DisplayName = "Schema not a string"
        )]
        public async Task FullTemplate(string json, PasteType expectedPasteType, string expectedBicep, string? errorMessage = null)
        {
            await TestDecompileForPaste(
                    json: json,
                    expectedPasteType: expectedPasteType,
                    expectedBicep: expectedBicep,
                    errorMessage);
        }

        public async Task FullTemplate_ButNoSchema_CantPaste()
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

        [DataTestMethod]
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
            PasteType.Resources,
            null,
            "[6:46]: The language expression 'bad-expression' is not valid: the string character 'x' at position '5' is not expected.",
            DisplayName = "Bad expression"
            )]
        public async Task Errors(string json, PasteType pasteType, string? expectedBicep, string? expectedErrorMessage)
        {
            await TestDecompileForPaste(json, pasteType, expectedBicep, expectedErrorMessage);
        }

        [TestMethod]
        public async Task JustString_WithNoQuotes_CantPaste()
        {
            string json = @"just a string";
            await TestDecompileForPaste(
                    json: json,
                    PasteType.None,
                    expectedErrorMessage: null,
                    expectedBicep: null);
        }

        [TestMethod]
        public async Task JustString_WithDoubleQuotes_CantPaste()
        {
            string json = @"""just a string with double quotes""";
            await TestDecompileForPaste(
                    json: json,
                    PasteType.None,
                    expectedErrorMessage: null,
                    expectedBicep: null);
        }

        [TestMethod]
        public async Task JustString_WithSingleQuotes_CantPaste()
        {
            string json = @"'just a string with double quotes'";
            await TestDecompileForPaste(
                    json: json,
                    PasteType.None,
                    expectedErrorMessage: null,
                    expectedBicep: null);
        }

        [TestMethod]
        public async Task NonResourceObject_CantPaste()
        {
            string json = @"{""hello"": ""there""}";
            await TestDecompileForPaste(
                    json: json,
                    PasteType.None,
                    expectedErrorMessage: null,
                    expectedBicep: null);
        }

        [TestMethod]
        public async Task NonResourceObject_WrongPropertyType_Object_CantPaste()
        {
            string json = @$"
                {Resource1Json.Replace("\"2021-02-01\"", "{}")}
            ";
            await TestDecompileForPaste(
                    json: json,
                    PasteType.None,
                    expectedErrorMessage: null,
                    expectedBicep: null);
        }

        [TestMethod]
        public async Task NonResourceObject_WrongPropertyType_Number_CantPaste2()
        {
            string json = @$"
                {Resource1Json.Replace("\"2021-02-01\"", "1234")}
            ";
            await TestDecompileForPaste(
                    json: json,
                    PasteType.None,
                    expectedErrorMessage: null,
                    expectedBicep: null);
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
                    expectedPasteType: PasteType.Resources,
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
                        ""location"": ""[parameters('location')]"",
                        ""kind"": ""[variables('location')]"",
                        ""sku"": {
                          ""name"": ""Premium_LRS""
                        }
                }
            ";            

            await TestDecompileForPaste(
                    json: json,
                    expectedErrorMessage: "[6:35]: Unable to pick unique name for missing variable location",
                    expectedPasteType: PasteType.Resources,
                    expectedBicep: null);
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
                    PasteType.Resources,
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
                    PasteType.Resources,
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

            { Resource1Json}
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
                    PasteType.Resources,
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
                    PasteType.Resources,
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
                PasteType.Resources,
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
                PasteType.Resources,
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
                    expectedPasteType: PasteType.Resources,
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
                    expectedPasteType: PasteType.Resources,
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
                    expectedPasteType: PasteType.Resources,
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
                    expectedPasteType: PasteType.Resources,
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
                    expectedPasteType: PasteType.Resources,
                    expectedErrorMessage: null,
                    expectedBicep: expected);
        }
    }
}
