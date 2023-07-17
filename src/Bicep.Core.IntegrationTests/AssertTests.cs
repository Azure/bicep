// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
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
                ("BCP349", DiagnosticLevel.Error, "Using an assert declaration requires enabling EXPERIMENTAL feature \"Assertions\".")
            });

            result = CompilationHelper.Compile(ServicesWithAsserts, @"
                assert a1 = true
            ");

            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Asserts_are_parsed_with_diagnostics()
        {
            var result = CompilationHelper.Compile(ServicesWithAsserts, @"
                assert
            ");
            result.Should().HaveDiagnostics(new[] {
                ("BCP344", DiagnosticLevel.Error, "Expected an assert identifier at this location."),
            });

            result = CompilationHelper.Compile(ServicesWithAsserts, @"
                assert a1
            ");
            result.Should().HaveDiagnostics(new[] {
                ("BCP018", DiagnosticLevel.Error, "Expected the \"=\" character at this location."),
            });

            result = CompilationHelper.Compile(ServicesWithAsserts, @"
                assert a1 =
            ");
            result.Should().HaveDiagnostics(new[] {
                ("BCP009", DiagnosticLevel.Error, "Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location."),
            });
        }

        [TestMethod]
        public void Assert_symbolic_names_must_be_unique()
        {
            var result = CompilationHelper.Compile(ServicesWithAsserts, @"
                param location string

                assert location = contains(location, 'west')
            ");

            result.Should().HaveDiagnostics(new[] {
                ("BCP028", DiagnosticLevel.Error, "Identifier \"location\" is declared multiple times. Remove or rename the duplicates."),
                ("BCP028", DiagnosticLevel.Error, "Identifier \"location\" is declared multiple times. Remove or rename the duplicates."),
            });
        }

        [TestMethod]
        public void Asserts_only_take_bool()
        {
            var result = CompilationHelper.Compile(ServicesWithAsserts,
                ("main.bicep", @"
                    param accountName string
                    param environment string

                    assert a1 = 1
                    assert a2 = 1 + 2 
                    
                    assert a3 = contains(accountName, 'stgA')
                    assert a4 = true
                    assert a5 = environment == 'dev'
                    assert a6 = null
                    assert a7 = [1, 2, 3]
                    assert a8 = {
                        x: 'y'
                    }
                    assert a9 = concat('a', 'b')
                "));

            result.Should().NotGenerateATemplate();
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP350", DiagnosticLevel.Error, "Value of type \"1\" cannot be assigned to an assert. Asserts can take values of type 'bool' only."),
                ("BCP350", DiagnosticLevel.Error, "Value of type \"3\" cannot be assigned to an assert. Asserts can take values of type 'bool' only."),
                ("BCP350", DiagnosticLevel.Error, "Value of type \"null\" cannot be assigned to an assert. Asserts can take values of type 'bool' only."),
                ("BCP350", DiagnosticLevel.Error, "Value of type \"[1, 2, 3]\" cannot be assigned to an assert. Asserts can take values of type 'bool' only."),
                ("BCP350", DiagnosticLevel.Error, "Value of type \"object\" cannot be assigned to an assert. Asserts can take values of type 'bool' only."),
                ("BCP350", DiagnosticLevel.Error, "Value of type \"string\" cannot be assigned to an assert. Asserts can take values of type 'bool' only."),
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
                    ""languageVersion"": ""1.10-experimental"",
                    ""contentVersion"": ""1.0.0.0"",
                    ""metadata"": {
                        ""_EXPERIMENTAL_WARNING"": ""Symbolic name support in ARM is experimental, and should be enabled for testing purposes only. Do not enable this setting for any production usage, or you may be unexpectedly broken at any time!"",
                        ""_generator"": {
                            ""name"": ""bicep"",
                            ""version"": ""dev"",
                            ""templateHash"": ""17284967353931057894""
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
