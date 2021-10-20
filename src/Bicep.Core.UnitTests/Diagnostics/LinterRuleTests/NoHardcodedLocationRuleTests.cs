// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Deployments.Core.Extensions;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.CodeAction;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    record ExpectedCodeFix
    {
        public string Description;
        public string ReplacementText;

        public ExpectedCodeFix(string description, string replacementText)
        {
            this.Description = description;
            this.ReplacementText = replacementText;
        }
    }

    [TestClass]
    public class NoHardcodedLocationRuleTests : LinterRuleTestsBase
    {
        private void ExpectPass(string bicepText, OnCompileErrors onCompileErrors = OnCompileErrors.Fail)
        {
            AssertLinterRuleDiagnostics(NoHardcodedLocationRule.Code, bicepText, new string[] { }, onCompileErrors);
        }

        private void ExpectPass(string bicepText, string module1Text, OnCompileErrors onCompileErrors = OnCompileErrors.Fail)
        {
            AssertLinterRuleDiagnostics(NoHardcodedLocationRule.Code, bicepText, new string[] { }, onCompileErrors);
        }

        private void ExpectFail(string bicepText, string expectedMessage, OnCompileErrors onCompileErrors = OnCompileErrors.Fail)
        {
            AssertLinterRuleDiagnostics(NoHardcodedLocationRule.Code, bicepText, new string[] { expectedMessage }, onCompileErrors);
        }

        private void ExpectFailWithFix(string bicepText, string expectedMessage, ExpectedCodeFix expectedFix, OnCompileErrors onCompileErrors = OnCompileErrors.Fail)
        {
            AssertLinterRuleDiagnostics(
              NoHardcodedLocationRule.Code,
              bicepText,
              diagnostics =>
              {
                  diagnostics.Should().HaveCount(1);
                  diagnostics.Should().HaveFixableDiagnostics(new[] {
                    (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, expectedMessage, expectedFix.Description, expectedFix.ReplacementText)
                  }); ;
              }
            );
        }

        /////////////////////////////////////////////////////////////////////////////////////
        // SUBRULE: The expressions `deployment().location` and `resourceGroup().location` may only be used as the default value of a parameter named `location` (case-insensitive), and nowhere else in the template
        /////////////////////////////////////////////////////////////////////////////////////

        [DataRow(@"
            targetScope = 'managementGroup'
            param Location string = deployment().location
          ")]
        [DataRow(@"
            param LOCATION string = resourceGroup().location
          ")]
        [DataTestMethod]
        public void NoRGLocOrDeplLoc_If_DeploymentOrResourceGroupLocation_UsedInDefaultForLocationParameter_CaseInsensitive_ShouldPass(string text)
        {
            ExpectPass(text);
        }

        [DataRow(@"
            targetScope = 'managementGroup'
            param NotLocation string = deployment().location
          ",
            "Use a parameter named `location` here instead of 'deployment().location'. 'deployment().location' should only be used as a default for parameter `location`.")]
        [DataTestMethod]
        public void NoRGLocOrDeplLoc_If_DeploymentOrResourceGroupLocation_UsedInDefaultForDifferentParameter_ShouldFail(string text, string expectedMessage)
        {
            ExpectFail(text, expectedMessage);
        }

        [DataRow(@"
            targetScope = 'managementGroup'
            var notAParam = deployment()
          ")]
        [DataRow(@"
            targetScope = 'managementGroup'
            var notAParam = '${deployment().properties.templateLink.id}${deployment().name}'
          ")]
        [DataRow(@"
            targetScope = 'managementGroup'
            var notAParam = '${az.deployment().properties.templateLink.id}${deployment().name}'
          ")]
        [DataRow(@"
            targetScope = 'managementGroup'
            var notAParam = resourceGroup('subscriptionid', 'rgName')
          ")]
        [DataRow(@"
            var notAParam = '${resourceGroup().properties.provisioningState}'
          ")]
        [DataRow(@"
            var notAParam = '${az.resourceGroup().properties.provisioningState}'
          ")]
        [DataTestMethod]
        public void NoRGLocOrDeplLoc_If_DeploymentOrResourceGroup_Used_WithoutLocationProperty_ShouldPass(string text)
        {
            ExpectPass(text);
        }

        [DataRow(@"
            targetScope = 'managementGroup'
            var notLocation object = deployment2().location
          ",
            OnCompileErrors.Ignore)]
        [DataRow(@"
            targetScope = 'managementGroup'
            var notLocation  string = sys.deployment().location
          ",
            OnCompileErrors.Ignore)]
        [DataRow(@"
            targetScope = 'managementGroup'
            var notLocation  string = DEPLOYMENT().location
          ",
            OnCompileErrors.Ignore)]
        [DataRow(@"
            var notLocation  object = myresourceGroup().location
          ",
            OnCompileErrors.Ignore)]
        [DataRow(@"
            var notLocation  string = sys.resourceGroup().location
          ",
            OnCompileErrors.Ignore)]
        [DataRow(@"
            var notLocation string = ResourceGroup().location
          ",
            OnCompileErrors.Ignore)]
        [DataTestMethod]
        public void NoRGLocOrDeplLoc_If_Not_DeploymentOrResourceGroup_OrWithIncorrectNamespace_ShouldPass(string text, OnCompileErrors onCompileErrors = OnCompileErrors.Fail)
        {
            ExpectPass(text, onCompileErrors);
        }

        [DataRow(
    @"
                targetScope = 'subscription'

                resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
                    name: 'rg'
                    location: deployment().location
                }
            ",
            "Use a parameter named `location` here instead of 'deployment().location'. 'deployment().location' should only be used as a default for parameter `location`.")]
        [DataRow(
    @"
                targetScope = 'subscription'

                resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
                    name: 'rg'
                    location: '${deployment().location}'
                }
            ",
            "Use a parameter named `location` here instead of 'deployment().location'. 'deployment().location' should only be used as a default for parameter `location`.")]
        [DataRow(
    @"
            resource availabilitySet 'Microsoft.Compute/availabilitySets@2020-12-01' = {
                    name: 'aset'
                    location: resourceGroup().location
                }
            ",
            "Use a parameter named `location` here instead of 'resourceGroup().location'. 'resourceGroup().location' should only be used as a default for parameter `location`.")]
        [DataRow(
    @"
            resource availabilitySet 'Microsoft.Compute/availabilitySets@2020-12-01' = {
                    name: 'aset'
                    location: '${resourceGroup().location}'
                }
            ",
            "Use a parameter named `location` here instead of 'resourceGroup().location'. 'resourceGroup().location' should only be used as a default for parameter `location`.")]
        [DataTestMethod]
        public void NoRGLocOrDeplLoc_If_DeploymentLocationOrResourceGroup_ShouldFail(string text, string expectedMessage)
        {
            ExpectFail(text, expectedMessage);
        }

        [DataRow(
            @"
                targetScope = 'subscription'

                resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
                    name: 'rg'
                    location: az.deployment().location
                }
            ",
            "Use a parameter named `location` here instead of 'deployment().location'. 'deployment().location' should only be used as a default for parameter `location`.")]
        [DataRow(
            @"
            resource availabilitySet 'Microsoft.Compute/availabilitySets@2020-12-01' = {
                    name: 'aset'
                    location: az.resourceGroup().location
                }
            ",
            "Use a parameter named `location` here instead of 'resourceGroup().location'. 'resourceGroup().location' should only be used as a default for parameter `location`.")]
        [DataTestMethod]
        public void NoRGLocOrDeplLoc_If_DeploymentLocationOrResourceGroup_WithAzNamespace_ShouldFail(string text, string expectedMessage)
        {
            ExpectFail(text, expectedMessage);
        }

        /////////////////////////////////////////////////////////////////////////////////////
        // SUBRULE: The parameter `location`, if it exists, must be of type `string`
        /////////////////////////////////////////////////////////////////////////////////////

        [DataRow(@"
            param location object
          ",
          "Location parameter must be of type 'string'.",
          "Change parameter type to string",
          "string")]
        [DataTestMethod]
        public void LocIsString_If_LocationParameter_NotOfTypeString_ShouldFail(string text, string message, string fixDescription, string fixReplacement)
        {
            ExpectFailWithFix(text, message, new ExpectedCodeFix(fixDescription, fixReplacement));
        }

        /////////////////////////////////////////////////////////////////////////////////////
        // SUBRULE: The parameter `location`, if it exists, must have one of the following default values:
        //   <none>, resourceGroup().location, deployment().location, 'global' (last one case-insensitive)
        /////////////////////////////////////////////////////////////////////////////////////

        [DataRow(@"
            param location string
          ")]
        [DataRow(@"
            param location string = resourceGroup().location
          ")]
        [DataRow(@"
            param location string = az.resourceGroup().location
          ")]
        [DataRow(@"
            targetScope = 'subscription'
            param location string = deployment().location
          ")]
        [DataRow(@"
            targetScope = 'subscription'
            param location string = az.deployment().location
          ")]
        [DataRow(@"
            param location string = 'global'
          ")]
        [DataRow(@"
            param location string = 'GLOBAL'
          ")]
        [DataRow(@"
            param location string = 'Global'
          ")]
        [DataTestMethod]
        public void LocDefault_If_LocationParameter_DefaultValue_IsInAllowedList_ShouldPass(string text)
        {
            ExpectPass(text);
        }

        [DataRow(@"
            param location string = 'westus'
          ",
            "The default value for the location parameter must be resourceGroup().location, deployment().location or 'global'."
         )]
        [DataRow(@"
            param location string = concat(resourceGroup().location, '')
          ",
            "The default value for the location parameter must be resourceGroup().location, deployment().location or 'global'."
         )]
        [DataTestMethod]
        public void LocDefault_If_LocationParameter_DefaultValue_Not_InAllowedList_ShouldFail(string text, string message)
        {
            ExpectFail(text, message);
        }

        /////////////////////////////////////////////////////////////////////////////////////
        // SUBRULE: When consuming a module, the module's `location` parameter must be given a value (it may not be left as its default value)
        /////////////////////////////////////////////////////////////////////////////////////3

        [TestMethod]
        public void ModLoc_If_ModuleHas_NoLocationParam_ShouldPass()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
                    param location string

                    module m1 'module1.bicep' = {
                      name: 'm1'
                    }

                    output o string = location
                    "),
                ("module1.bicep", @"
                    param noLocationParameter string = 'hello'
                    output o string = noLocationParameter
                   ")
            );
            result.Diagnostics.Should().BeEmpty();
        }

        [TestMethod]
        public void ModLoc_If_ModuleHas_LocationParam_WithoutDefault_AndValuePassedIn_ShouldPass()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
                    param location string

                    module m1 'module1.bicep' = {
                      name: 'm1'
                      params: {
                        location: location
                      }
                    }

                    output o string = location
                    "),
                ("module1.bicep", @"
                    param location string
                    output o string = location
                   ")
            );
            result.Diagnostics.Should().BeEmpty();
        }

        [TestMethod]
        public void ModLoc_If_ModuleHas_LocationParam_WithoutDefault_AndValueNotPassedIn_ShouldFail()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
                    param location string

                    module m2 'module1.bicep' = {
                      name: 'm1'
                      params: {
                      }
                    }

                    output o string = location
                    "),
                ("module1.bicep", @"
                    param location string
                    output o string = location
                   ")
            );
            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                ("BCP035", DiagnosticLevel.Error, "The specified \"object\" declaration is missing the following required properties: \"location\"."),
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "The 'location' parameter for module 'm2' should be assigned an explicit value.")
            });
        }

        [TestMethod]
        public void ModLoc_If_ModuleHas_LocationParam_WithDefault_AndValuePassedIn_ShouldPass()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
                    param location string

                    module m1 'module1.bicep' = {
                      name: 'm1'
                      params: {
                        location: location
                      }
                    }

                    output o string = location
                    "),
                ("module1.bicep", @"
                    param location string
                    output o string = location
                   ")
            );
            result.Diagnostics.Should().BeEmpty();
        }

        [TestMethod]
        public void ModLoc_If_ModuleHas_LocationParam_WithDefault_AndValuePassedIn_CaseInsensitive_ShouldPass()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
                    param location string

                    module m1 'module1.bicep' = {
                      name: 'm1'
                      params: {
                        LOCATION: location
                      }
                    }

                    output o string = location
                    "),
                ("module1.bicep", @"
                    param LOCATION string
                    output o string = LOCATION
                   ")
            );
            result.Diagnostics.Should().BeEmpty();
        }

        [TestMethod]
        public void ModLoc_If_ModuleHas_LocationParam_WithDefault_AndValueNotPassedIn_ShouldFail()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
                    param location string

                    module m3 'module1.bicep' = {
                      name: 'm1'
                      params: {
                      }
                    }

                    output o string = location
                    "),
                ("module1.bicep", @"
                    param Location string = resourceGroup().location
                    output o string = Location
                   ")
            );
            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "The 'Location' parameter for module 'm3' should be assigned an explicit value.")
            });
        }

        [TestMethod]
        public void ModLoc_If_ModuleHas_LocationParam_WithDefault_AndValueNotPassedIn_CaseInsensitive_ShouldFail()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
                    param location string

                    module m1 'module1.bicep' = {
                      name: 'm1'
                      params: {
                      }
                    }

                    output o string = location
                    "),
                ("module1.bicep", @"
                    param LOCATION string = resourceGroup().location
                    output o string = LOCATION
                   ")
            );
            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "The 'LOCATION' parameter for module 'm1' should be assigned an explicit value.")

            });
        }

        [TestMethod]
        public void ModLoc_If_Module_HasErrors_LocationParam_WithDefault_AndValuePassedIn_CaseInsensitive_ShouldPass()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
                    param location string

                    module m1 'module1.bicep' = {
                      name: 'm1'
                      params: {
                        LOCATION: location
                      }
                    }

                    output o string = location
                    "),
                ("module1.bicep", @"
                    param LOCATION string
                    output o string = whoops // error
                   ")
            );
            result.Diagnostics.Should().HaveDiagnostics(new[]
{
                ("BCP104", DiagnosticLevel.Error, "The referenced module has errors.")
            });
        }

        /////////////////////////////////////////////////////////////////////////////////////
        // SUBRULE: Each resource's `location` property and each module's `location` parameter must be either an expression or 'global'
        // Note: Usually this expression should a parameter, e.g. `location`, but may not be `deployment().location` or
        //   `resourceGroup().location` by another above subrule)
        /////////////////////////////////////////////////////////////////////////////////////

        [TestMethod]
        public void ResLoc_If_Resource_HasLocation_AsResourceGroupLocation_ShouldFail()
        {
            var result = CompilationHelper.Compile(@"
                resource appInsightsComponents 'Microsoft.Insights/components@2020-02-02-preview' = {
                  name: 'name'
                  location: resourceGroup().location
                  kind: 'web'
                  properties: {
                    Application_Type: 'web'
                  }
                }
                "
            );

            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "Use a parameter named `location` here instead of 'resourceGroup().location'. 'resourceGroup().location' should only be used as a default for parameter `location`.")
            });
        }

        [TestMethod]
        public void ResLoc_If_Resource_HasLocation_AsExpression_ShouldPass()
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
        public void ResLoc_If_Resource_HasLocation_AsGlobal_ShouldPass()
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
        public void ResLoc_If_Resource_HasLocation_AsGlobal_CaseInsensitive_ShouldPass()
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
        public void ResLoc_If_Resource_HasLocation_AsOtherStringLiteral_ShouldFail()
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

            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should be either an expression or the string 'global'. Found 'non-global'")

            });
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
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should be either an expression or the string 'global'. Found 'non-global'")

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
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should be either an expression or the string 'global'. Found 'westus'")
            });
        }

        /////////////////////////////////////////////////////////////////////////////////////
        // Miscellaneous
        /////////////////////////////////////////////////////////////////////////////////////

        [TestMethod]
        public void ForLoop1_Resource()
        {
            var result = CompilationHelper.Compile(@"
                resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = [for i in range(0, 10): {
                  name: 'name${i}'
                  location: resourceGroup().location
                  kind: 'StorageV2'
                  sku: {
                    name: 'Premium_LRS'
                  }
                }]                "
            );

            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "Use a parameter named `location` here instead of 'resourceGroup().location'. 'resourceGroup().location' should only be used as a default for parameter `location`.")
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
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should be either an expression or the string 'global'. Found 'westus'")
            });
        }

        [TestMethod]
        public void ForLoop3_Module()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
                    module m2 'module1.bicep' = [for i in range(0, 10): {
                        name: 'name${i}'
                    }]"),
                ("module1.bicep", @"
                    param location string = resourceGroup().location
                    output o string = location
                   ")
            );

            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "The 'location' parameter for module 'm2' should be assigned an explicit value.")
            });
        }

        [TestMethod]
        public void Conditional1_Module()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
                    param deploy bool
                    module m3 'module1.bicep' = [for i in range(0, 10): if (deploy) {
                      name: 'name${i}'
                    }]
                "),
                ("module1.bicep", @"
                    param location string = resourceGroup().location
                    output o string = location
                   ")
            );

            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "The 'location' parameter for module 'm3' should be assigned an explicit value.")
            });
        }
    }
}
