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
    //asdfg test fixes

    [TestClass]
    public class NoHardcodedLocationRuleTests : LinterRuleTestsBase
    {
        /* asdfg
        no-hardcoded-resource-location
        A resource's location should not use a hard-coded string or variable value. It should use a parameter, an expression (but not resourceGroup().location or deployment().location) or the string 'global'.

        Best practice suggests that to set your resources' locations, your template should have a string parameter named location. This parameter may default to the resource group or deployment location.

        Template users may have limited access to regions where they can create resources. A hard-coded resource location might block users from creating a resource, thus preventing them from using the template. By providing a location parameter that defaults to the resource group location, users can use the default value when convenient but also specify a different location.

        AUTO-FIXES AVAILABLE
        If using a variable instead of a parameter
        Change variable '{variable}' into a parameter
        Otherwise
        Change to parameter '{existing-parameter-with-same-default-value}' with matching default value
        Create new parameter 'location{,2,3,...}' with default value '{location value}'
        Change to existing parameter '{existing-parameter-that-is-used-as-a-location-in-other-resources-in-the-file} (default value is {default-value})
        ISSUE: Note that this last fix may be a change in default behavior if the existing parameter's default value is different than the current value
        */

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
        public void If_ResLocationIs_VariableAsGlobal_ShouldPass()
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
        public void If_ResLocationIs_AnyOtherStringLiteral_ShouldFail()
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
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should not use a hard-coded string or variable value. It should use a parameter value, an expression, or the string 'global'. Found: 'non-global'")

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
            {//asdfg fixes
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should not use a hard-coded string or variable value. It should use a parameter value, an expression, or the string 'global'. Found: 'westus'" +
                "")
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

            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should not use a hard-coded string or variable value. Change variable 'location' into a parameter.")
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
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should not use a hard-coded string or variable value. Change variable 'location' into a parameter.")
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
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should not use a hard-coded string or variable value. Change variable 'location' into a parameter.")
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
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should not use a hard-coded string or variable value. Change variable 'location' into a parameter.")
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
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should not use a hard-coded string or variable value. Change variable 'location' into a parameter."),
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
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should not use a hard-coded string or variable value. Change variable 'location' into a parameter.")
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
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "A resource location should not use a hard-coded string or variable value. Change variable 'v1' into a parameter.")
            });
        }
    }
}
