// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.IntegrationTests.Extensibility;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class AssertTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private ServiceBuilder ServicesWithAsserts => new ServiceBuilder()
            .WithFeatureOverrides(new(TestContext, AssertsEnabled: true));


        [TestMethod]
        public void Asserts_are_disabled_unless_feature_is_enabled()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new());

            var result = CompilationHelper.Compile(services, @"
                assert a1 = true
            ");

            result.Should().HaveDiagnostics(new[] {
                ("BCP345", DiagnosticLevel.Error, "Using an assert declaration requires enabling EXPERIMENTAL feature \"Asserts\".")
            });

            result = CompilationHelper.Compile(ServicesWithAsserts, @"
                assert a1 = true
            ");

            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Asserts_only_take_boolean()
        {
            var result = CompilationHelper.Compile(ServicesWithAsserts,
                ("main.bicep", @"
                    param accountName string
                    param environment string

                    assert a1 = 1
                    
                    assert a3 = contains(accountName, 'dev')
                    assert a4 = true
                    assert devEnv = environment == 'dev'
                "));

            result.Should().NotGenerateATemplate();
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP346", DiagnosticLevel.Error, "Value of type \"IntegerLiteral\" cannot be assigned to an assert. Asserts can take values of type boolean only.")
            });
        }

        [TestMethod]
        public void Assert_end_to_end_test()
        {
            var result = CompilationHelper.Compile(ServicesWithAsserts,
                ("main.bicep", @"
                    param accountName string
                    param environment string
                    param location string

                    resource stgAccount 'Microsoft.Storage/storageAccounts@2019-06-01' = {
                        name: toLower(accountName)
                        location: resourceGroup().location
                        kind: 'Storage'
                        sku: {
                            name: 'Standard_LRS'
                        }
                    }

                    var myInt = 24

                    assert a1 = length(accountName) < myInt
                    assert a2 = contains(location, 'us')
                    assert a3 = environment == 'dev'    
                "));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should().DeepEqual(JToken.Parse(@"
                {
                    ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
                    ""languageVersion"": ""1.11-experimental"",
                    ""contentVersion"": ""1.0.0.0"",
                    ""metadata"": {
                        ""_EXPERIMENTAL_WARNING"": ""Symbolic name support in ARM is experimental, and should be enabled for testing purposes only. Do not enable this setting for any production usage, or you may be unexpectedly broken at any time!"",
                        ""_generator"": {
                            ""name"": ""bicep"",
                            ""version"": ""dev"",
                            ""templateHash"": ""7560528332789115725""
                        }
                    },
                    ""parameters"": {
                        ""accountName"": {
                            ""type"": ""string""
                        },
                        ""environment"": {
                            ""type"": ""string""
                        },
                        ""location"": {
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
                        }
                    },
                    ""variables"": {
                            ""myInt"": 24
                        },
                    ""asserts"": {
                            ""a1"": ""[less(length(parameters('accountName')), variables('myInt'))]"",
                            ""a2"": ""[contains(parameters('location'), 'us')]"",
                            ""a3"": ""[equals(parameters('environment'), 'dev')]""
                    }
                }
            "));
        }
    }
}
