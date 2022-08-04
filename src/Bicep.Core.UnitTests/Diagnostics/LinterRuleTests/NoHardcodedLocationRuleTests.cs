// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class NoHardcodedLocationRuleTests : LinterRuleTestsBase
    {
        [TestMethod]
        public void If_ResLocationIs_Global_ShouldPass()
        {
            var result = CompilationHelper.Compile(@"
                resource appInsightsComponents 'Microsoft.Insights/components@2020-02-02-preview' = {
                  name: 'name'
                  location: 'global'
                  kind: 'web'
                  properties: {
                    Application_Type: 'web'
                  }
                }"
            );

            result.Diagnostics.Should().BeEmpty();
        }

        [TestMethod]
        public void If_ResLocationIs_Global_CaseInsensitive_ShouldPass()
        {
            var result = CompilationHelper.Compile(@"
                resource appInsightsComponents 'Microsoft.Insights/components@2020-02-02-preview' = {
                  name: 'name'
                  location: 'GLOBAL'
                  kind: 'web'
                  properties: {
                    Application_Type: 'web'
                  }
                }"
            );

            result.Diagnostics.Should().BeEmpty();
        }

        [TestMethod]
        public void If_ResLocationIs_VariableWithGlobal_ShouldPass()
        {
            var result = CompilationHelper.Compile(@"
                var location = 'Global'
                resource appInsightsComponents 'Microsoft.Insights/components@2020-02-02-preview' = {
                  name: 'name'
                  location: location
                  kind: 'web'
                  properties: {
                    Application_Type: 'web'
                  }
                }"
            );

            result.Diagnostics.Should().BeEmpty();
        }

        [TestMethod]
        public void If_ResLocationIs_AnyOtherStringLiteral_ShouldFail_AndOfferToCreateNewParameter()
        {
            var result = CompilationHelper.Compile(@"
                resource appInsightsComponents 'Microsoft.Insights/components@2020-02-02-preview' = {
                  name: 'name'
                  location: 'non-global'
                  kind: 'web'
                  properties: {
                    Application_Type: 'web'
                  }
                }"
            );

            CodeFix x = result.Diagnostics.First().Should().BeAssignableTo<IFixable>()
                .Which.Fixes.Single();
            x.Should().BeEquivalentTo(
                new
                {
                    Description = "Create new parameter 'location' with default value 'non-global'",
                    Replacements = new[] {
                        // Replacement 1: change 'non-global' to 'location'
                        new { Text = "location" },
                        // Replacement 1: add new location param
                        new { Text = "@description('Specifies the location for resources.')\nparam location string = 'non-global'\n\n" }
                    }
                });
        }

        [TestMethod]
        public void If_NameLocationAlreadyInUse_ShouldChooseAnotherNameForFix()
        {
            var result = CompilationHelper.Compile(@"
                var location = 'fee fie'
                param location2 string
                resource location3 'Microsoft.Insights/components@2020-02-02-preview' = {
                  name: 'name'
                  location: 'non-global'
                  kind: 'web'
                  properties: {
                    Application_Type: 'web'
                  }
                }
                output location4 string = '${location}${location2}'
                ");

            CodeFix x = result.Diagnostics.First().Should().BeAssignableTo<IFixable>()
                .Which.Fixes.Single();
            x.Should().BeEquivalentTo(
                new
                {
                    Description = "Create new parameter 'location5' with default value 'non-global'",
                    Replacements = new[] {
                        // Replacement 1: change 'non-global' to 'location5'
                        new { Text = "location5" },
                        // Replacement 1: add new location param
                        new { Text = "@description('Specifies the location for resources.')\nparam location5 string = 'non-global'\n\n" }
                    }
                });
        }

        [TestMethod]
        public void If_ResLocationIs_StringLiteral_ShouldFail_WithFixes()
        {
            var result = CompilationHelper.Compile(@"
                resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name'
                  location: 'westus'
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }
            ");

            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'westus'")
            });
        }

        [TestMethod]
        public void If_ResLocationIs_VariableDefinedAsLiteral_ShouldFail_WithFixToChangeToParam()
        {
            var result = CompilationHelper.Compile(@"
                var location = 'westus'

                resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name'
                  location: location
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }
            ");

            result.Diagnostics.Should().HaveFixableDiagnostics(new[]
                       {
                (
                    NoHardcodedLocationRule.Code,
                    DiagnosticLevel.Warning,
                    "A resource location should not use a hard-coded string or variable value. Change variable 'location' into a parameter instead.",
                    "Change variable 'location' into a parameter instead",
                    "param location string = 'westus'"
                )
            });
        }

        [TestMethod]
        public void If_ResLocationIs_VariableDefinedAsLiteral_ShouldFail_WithFixToChangeToParam_OnlyOneErrorPerVariable()
        {
            var result = CompilationHelper.Compile(@"
                var location = 'westus'
                var location2 = 'westus'

                resource storageaccount1 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name1'
                  location: location
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }

                resource storageaccount2 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name2'
                  location: location
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }

                resource storageaccount3 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name3'
                  location: location2
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }

                resource storageaccount4 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name4'
                  location: location2
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }

                resource storageaccount5 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name5'
                  location: location
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }
            ");

            result.Diagnostics.Should().HaveFixableDiagnostics(new[]
            {
                (
                    NoHardcodedLocationRule.Code,
                    DiagnosticLevel.Warning,
                    "A resource location should not use a hard-coded string or variable value. Change variable 'location' into a parameter instead.",
                    "Change variable 'location' into a parameter instead",
                    "param location string = 'westus'"
                ),
                (
                    NoHardcodedLocationRule.Code,
                    DiagnosticLevel.Warning,
                    "A resource location should not use a hard-coded string or variable value. Change variable 'location2' into a parameter instead.",
                    "Change variable 'location2' into a parameter instead",
                    "param location2 string = 'westus'"
                )
            });
        }

        [TestMethod]
        public void If_ResLocationIs_VariableDefinedAsLiteral_Used2Times_ShouldFailJustOnVariableDef__WithFixToChangeToParam()
        {
            var result = CompilationHelper.Compile(@"
                var location = 'westus'

                resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name'
                  location: location
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }

                resource storageaccount2 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name2'
                  location: location
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }
            ");

            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should not use a hard-coded string or variable value. Change variable 'location' into a parameter instead.")
            });
        }

        [TestMethod]
        public void If_ResLocationIs_VariableDefinedAsLiteral_UsedInResourcesAndModules_ShouldFailJustOnVariableDef_WithFixToChangeToParam()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
                    module m1 'module1.bicep' = [for i in range(0, 10): {
                      name: 'm1${i}'
                      params: {
                          beebop: location
                      }
                    }]

                    var location = 'westus'

                    resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                      name: 'name'
                      location: location
                      kind: 'StorageV2'
                      sku: {
                      name: 'Premium_LRS'
                      }
                    }

                    resource storageaccount2 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                      name: 'name2'
                      location: location
                      kind: 'StorageV2'
                      sku: {
                      name: 'Premium_LRS'
                      }
                    }

                    module m2 'module1.bicep' = {
                      name: 'm2'
                      params: {
                        beebop: location
                      }
                    }
                "),
                ("module1.bicep", @"
                    param beebop string = resourceGroup().location
                    output o string = beebop
                   ")
                );

            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should not use a hard-coded string or variable value. Change variable 'location' into a parameter instead.")
            });
        }

        [TestMethod]
        public void If_ResLocationIs_IndirectVariableDefinedAsLiteral_ShouldFail()
        {
            var result = CompilationHelper.Compile(@"
                var location = 'westus'
                var location2 = location

                resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name'
                  location: location2
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }
            ");

            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should not use a hard-coded string or variable value. Change variable 'location' into a parameter instead.")
            });
        }

        [TestMethod]
        public void If_ResLocationIs_IndirectVariableDefinedAsLiteral_UsedIn2Places_ShouldFailJustOnVariableDef_WithFixToChangeToParam()
        {
            var result = CompilationHelper.Compile(@"
                var location = 'westus'
                var location2 = location

                resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name'
                  location: location2
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }

                resource storageaccount2 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name2'
                  location: location2
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }
            ");

            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should not use a hard-coded string or variable value. Change variable 'location' into a parameter instead.")
            });
        }

        [TestMethod]
        public void If_ResLocationIs_IndirectVariableDefinedAsLiteral_UsedIn2PlacesDifferently_ShouldFailJustOnVariableDefinition_WithFixToChangeToParam()
        {
            var result = CompilationHelper.Compile(@"
                var location = 'westus'
                var location2 = location

                resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name'
                  location: location
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }

                resource storageaccount2 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name2'
                  location: location2
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }
            ");

            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should not use a hard-coded string or variable value. Change variable 'location' into a parameter instead."),
            });
        }

        [TestMethod]
        public void If_ResLocationIs_VariableDefinedAsLiteral_UsedMultipleTimes_ThenOneDisableNextLineShouldFixIt()
        {
            var result = CompilationHelper.Compile(@"
                #disable-next-line no-hardcoded-location
                var location = 'westus'

                resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name'
                  location: location
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }

                resource storageaccount2 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name2'
                  location: location
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }

                resource storageaccount3 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name3'
                  location: location
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }
            ");

            result.Diagnostics.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void If_ResLocationIs_TwiceIndirectedVariableDefinedAsLiteral_ShouldFail_WithFixToChangeToParam()
        {
            var result = CompilationHelper.Compile(@"
                var location = 'westus'
                var location2 = location
                var location3 = location2

                resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name'
                  location: location3
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }
            ");

            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should not use a hard-coded string or variable value. Change variable 'location' into a parameter instead.")
            });
        }

        [TestMethod]
        public void If_ResLocationIs_VariablePointingToParameter_ShouldPass()
        {
            var result = CompilationHelper.Compile(@"
                param location string = 'global'
                var location2 = location
                var location3 = location2

                resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name'
                  location: location3
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }
            ");

            result.Diagnostics.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void If_ResLocationIs_VariableWithExpression_ShouldPass()
        {
            var result = CompilationHelper.Compile(@"
                var location = true ? 'a' : 'b'

                resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name'
                  location: location
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }
            ");

            result.Diagnostics.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void If_ResLocationIs_IndirectedVariableWithInterpolation_ShouldPass()
        {
            var result = CompilationHelper.Compile(@"
                var location = 'westus'
                var location2 = '${location}2'
                var location3 = location2

                resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name'
                  location: location3
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }
            ");

            result.Diagnostics.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void If_ResLocationIs_IndirectedVariableWithExpression_ShouldPass()
        {
            var result = CompilationHelper.Compile(@"
                var location = 'westus'
                var location2 = true ? location : location
                var location3 = location2

                resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  name: 'name'
                  location: location3
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }
            ");

            result.Diagnostics.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void If_ResLocationIs_Expression_ShouldPass()
        {
            var result = CompilationHelper.Compile(@"
                param location1 string
                param location2 string

                resource appInsightsComponents 'Microsoft.Insights/components@2020-02-02-preview' = {
                  name: 'name'
                  location: '${location1}${location2}'
                  kind: 'web'
                  properties: {
                    Application_Type: 'web'
                  }
                }"
            );

            result.Diagnostics.Should().BeEmpty();
        }

        [TestMethod]
        public void ResLoc_If_Resource_HasLocation_AsIndirectStringLiteral_ShouldFail()
        {
            var result = CompilationHelper.Compile(@"
                var v1 = 'non-global'

                resource appInsightsComponents 'Microsoft.Insights/components@2020-02-02-preview' = {
                  name: 'name'
                  location: v1
                  kind: 'web'
                  properties: {
                    Application_Type: 'web'
                  }
                }"
            );

            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should not use a hard-coded string or variable value. Change variable 'v1' into a parameter instead.")
            });
        }


        [TestMethod]
        public void ForLoop2_Module()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
                  module m2 'module1.bicep' = [for i in range(0, 10): {
                    name: 'name${i}'
                    params: {
                      location: 'westus'
                    }
                  }]
                    "),
                ("module1.bicep", @"
                    param location string = resourceGroup().location
                    output o string = location
                   ")
            );

            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "Parameter 'location' may be used as a resource location in the module and should not be assigned a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'westus'")
            });
        }

        [TestMethod]
        public void ResLoc_If_Module_HasLocationProperty_WithDefault_AndStringLiteralPassedIn_ShouldFail()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
                    module m1 'module1.bicep' = {
                      name: 'name'
                      params: {
                        location: 'westus'
                      }
                    }
                    "),
                ("module1.bicep", @"
                    param location string = resourceGroup().location
                    output o string = location
                   ")
            );

            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "Parameter 'location' may be used as a resource location in the module and should not be assigned a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'westus'")
            });
        }

    }
}
