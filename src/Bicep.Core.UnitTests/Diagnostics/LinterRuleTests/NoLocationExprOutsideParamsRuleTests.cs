// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class NoLocationExprOutsideParamsRuleTests : LinterRuleTestsBase
    {
        public record ExpectedCodeFix
        {
            public string Description;
            public string ReplacementText;

            public ExpectedCodeFix(string description, string replacementText)
            {
                this.Description = description;
                this.ReplacementText = replacementText;
            }
        }

        protected void ExpectPass(string bicepText, Options? options = null)
        {
            AssertLinterRuleDiagnostics(NoLocationExprOutsideParamsRule.Code, bicepText, new string[] { }, options);
        }

        protected void ExpectFail(string bicepText, string expectedMessage)
        {
            AssertLinterRuleDiagnostics(NoLocationExprOutsideParamsRule.Code, bicepText, new string[] { expectedMessage });
        }

        protected void ExpectFailWithFix(string bicepText, string expectedMessage, ExpectedCodeFix expectedFix)
        {
            AssertLinterRuleDiagnostics(
              NoLocationExprOutsideParamsRule.Code,
              bicepText,
              diagnostics =>
              {
                  diagnostics.Should().HaveCount(1);
                  diagnostics.Should().HaveFixableDiagnostics(new[] {
                    (NoLocationExprOutsideParamsRule.Code, DiagnosticLevel.Warning, expectedMessage, expectedFix.Description, expectedFix.ReplacementText)
                  }); ;
              }
            );
        }

        [DataRow(@"
            targetScope = 'managementGroup'
            param Location string = deployment().location
          ")]
        [DataRow(@"
            param whatever string = resourceGroup().location
          ")]
        [DataRow(@"
            targetScope = 'managementGroup'
            param Location string = az.deployment().location
          ")]
        [DataRow(@"
            param Location string = az.resourceGroup().location
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
        public void If_Not_DeploymentOrResourceGroup_OrWithIncorrectNamespace_ShouldPass(string text, OnCompileErrors onCompileErrors = OnCompileErrors.IncludeErrors)
        {
            ExpectPass(text, new Options( onCompileErrors));
        }

        [DataRow(
    @"
                targetScope = 'subscription'

                resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
                    name: 'rg'
                    location: deployment().location
                }
            ",
            "[6] Use a parameter here instead of 'deployment().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters.")]
        [DataRow(
    @"
                targetScope = 'subscription'

                resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
                    name: 'rg'
                    location: '${deployment().location}'
                }
            ",
            "[6] Use a parameter here instead of 'deployment().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters.")]
        [DataRow(
    @"
            resource availabilitySet 'Microsoft.Compute/availabilitySets@2020-12-01' = {
                    name: 'aset'
                    location: resourceGroup().location
                }
            ",
            "[4] Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters.")]
        [DataRow(
    @"
            resource availabilitySet 'Microsoft.Compute/availabilitySets@2020-12-01' = {
                    name: 'aset'
                    location: '${resourceGroup().location}'
                }
            ",
            "[4] Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters.")]
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
            "[6] Use a parameter here instead of 'deployment().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters.")]
        [DataRow(
            @"
            resource availabilitySet 'Microsoft.Compute/availabilitySets@2020-12-01' = {
                    name: 'aset'
                    location: az.resourceGroup().location
                }
            ",
            "[4] Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters.")]
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
                (NoLocationExprOutsideParamsRule.Code, DiagnosticLevel.Warning, "Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters.")
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
                (NoLocationExprOutsideParamsRule.Code, DiagnosticLevel.Warning, "Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters.")
            });
        }

        [TestMethod]
        public void CantBeFooledByStealthyString()
        {
            var result = CompilationHelper.Compile(@"
                output o string = 'resourceGroup().location'
            ");

            result.Diagnostics.Should().NotHaveAnyDiagnostics();
        }
    }
}
