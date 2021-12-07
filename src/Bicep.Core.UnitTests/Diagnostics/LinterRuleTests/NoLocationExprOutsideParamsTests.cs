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
    [TestClass]
    public class NoLocationExprOutsideParamsTests : LinterRuleTestsBase
    {
        /* asdfg
            no-location-expr-outside-params

            resourceGroup().location and deployment().location should only be used as the default value of a parameter.

            Template users may have limited access to regions where they can create resources. The expressions resourceGroup().location or deployment().location could block users if the resource group or deployment was created in a region the user can't access, thus preventing them from using the template.

            Best practice suggests that to set your resources' locations, your template should have a string parameter named location. By providing a location parameter that defaults to the resource group or deployment location instead of calling these directly elsewhere in the template, users can use the default value when convenient but also specify a different location.

            AUTO-FIXES AVAILABLE
            Change to parameter '{existing-parameter-with-same-default-value}' with matching default value
            Create new parameter 'location{,2,3,...}' with default value '{location value}'
            Change to existing parameter '{existing-parameter-that-is-used-as-a-location-in-other-resources-in-the-file} (default value is {default-value})
            ISSUE: Note that this last fix may be a change in default behavior if the existing parameter's default value is different than the current value
        */

        [DataRow(@"
            targetScope = 'managementGroup'
            param Location string = deployment().location
          ")]
        [DataRow(@"
            param whatever string = resourceGroup().location
          ")]
        [DataTestMethod]
        public void If_LocationExprUsedIn_DefaultForParameter_ShouldPass(string text)
        {
            ExpectPass(text);
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
        public void If_DeploymentOrResourceGroup_Object_WithoutLocationProperty_ShouldPass(string text)
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
        public void If_Not_DeploymentOrResourceGroup_OrWithIncorrectNamespace_ShouldPass(string text, OnCompileErrors onCompileErrors = OnCompileErrors.Fail)
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
            "Use a parameter here instead of 'deployment().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters.")]
        [DataRow(
    @"
                targetScope = 'subscription'

                resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
                    name: 'rg'
                    location: '${deployment().location}'
                }
            ",
            "Use a parameter here instead of 'deployment().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters.")]
        [DataRow(
    @"
            resource availabilitySet 'Microsoft.Compute/availabilitySets@2020-12-01' = {
                    name: 'aset'
                    location: resourceGroup().location
                }
            ",
            "Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters.")]
        [DataRow(
    @"
            resource availabilitySet 'Microsoft.Compute/availabilitySets@2020-12-01' = {
                    name: 'aset'
                    location: '${resourceGroup().location}'
                }
            ",
            "Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters.")]
        [DataTestMethod]
        public void If_DeploymentLocationOrResourceGroup_OutsideParam_ShouldFail(string text, string expectedMessage)
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
            "Use a parameter here instead of 'deployment().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters.")]
        [DataRow(
            @"
            resource availabilitySet 'Microsoft.Compute/availabilitySets@2020-12-01' = {
                    name: 'aset'
                    location: az.resourceGroup().location
                }
            ",
            "Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters.")]
        [DataTestMethod]
        public void If_DeploymentLocationOrResourceGroup_WithAzNamespace_ShouldFail(string text, string expectedMessage)
        {
            ExpectFail(text, expectedMessage);
        }

        [TestMethod]
        public void If_ResLocIs_ResourceGroupLocation_ShouldFail()
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
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters.")
            });
        }

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
                (NoHardcodedLocationRule.Code, DiagnosticLevel.Warning, "Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters.")
            });
        }

    }
}
