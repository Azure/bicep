// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class LocationSetByParameterRulesTests : LinterRuleTestsBase
    {
        private void CompileAndTest(string text, int expectedDiagnosticCount)
        {
            base.CompileAndTest(LocationSetByParameterRule.Code, text, expectedDiagnosticCount);
        }

        [TestMethod]
        public void PassesIfLocationParamDefaultsToRGLocation()
        {
            // From https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/test-cases#location-uses-parameter
            // NOTE: In the Azure TTK, this will pass if the template is the main template, but fail if it's a nested template.
            // In this linter, we don't make the distinction between main templates and nested/linked templates

            string text = @"
                @description('Location for the resources.')
                param location string = resourceGroup().location

                resource storageaccount1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
                    name: 'storageaccount1'
                    location: location
                    kind: 'StorageV2'
                    sku: {
                        name: 'Premium_LRS'
                        tier: 'Premium'
                    }
                }";

            CompileAndTest(text, 0);
        }

        [TestMethod]
        public void PassesIfLocationParamHasNoDefault()
        {
            // From https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/test-cases#location-uses-parameter
            // NOTE: In the Azure TTK, this will pass if the template is the main template, but fail if it's a nested template.
            // In this linter, we don't make the distinction between main templates and nested/linked templates

            string text = @"
                @description('Location for the resources.')
                param location string // no default

                resource storageaccount1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
                    name: 'storageaccount1'
                    location: location
                    kind: 'StorageV2'
                    sku: {
                        name: 'Premium_LRS'
                        tier: 'Premium'
                    }
                }";

            CompileAndTest(text, 0);
        }

        [TestMethod]
        public void PassesIfLocationParamNotNamedLocation()
        {
            string text = @"
                @description('Location for the resources.')
                param lage string // no default

                resource storageaccount1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
                    name: 'storageaccount1'
                    location: lage
                    kind: 'StorageV2'
                    sku: {
                        name: 'Premium_LRS'
                        tier: 'Premium'
                    }
                }";

            CompileAndTest(text, 0);
        }

        [TestMethod]
        public void PassesIfLocationParamDefaultsToGlobal()
        {
            string text = @"
            param location string = 'global'

            resource storageaccount1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
                name: 'storageaccount1'
                location: location
                kind: 'StorageV2'
                sku: {
                    name: 'Premium_LRS'
                    tier: 'Premium'
                }
            }";

            CompileAndTest(text, 0);
        }

        [TestMethod]
        public void PassesIfSpelledWithDifferentCasing()
        {
            string text = @"
                param location string = ResourceGROUP().LOCATion

                resource storageaccount1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
                    name: 'storageaccount1'
                    location: location
                    kind: 'StorageV2'
                    sku: {
                        name: 'Premium_LRS'
                        tier: 'Premium'
                    }
                }";

            CompileAndTest(text, 0);
        }

        [TestMethod]
        public void FailsIfLocationOnResourceSetToNonParamExpression()
        {
            // From https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/test-cases#location-uses-parameter
            // The following example fails this test because location on the resource is set to resourceGroup().location instead of location param
            string text = @"
                resource storageaccount1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
                name: 'storageaccount1'
                location: resourceGroup().location
                kind: 'StorageV2'
                    sku: {
                        name: 'Premium_LRS'
                        tier: 'Premium'
                    }
                }";

            CompileAndTest(text, 1);
        }

        [TestMethod]
        public void FailsIfLocationOnResourceSetToVarReference()
        {
            string text = @"
                var location string
                resource storageaccount1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
                name: 'storageaccount1'
                location: location // Variable ref not allowed, only param ref
                kind: 'StorageV2'
                    sku: {
                        name: 'Premium_LRS'
                        tier: 'Premium'
                    }
                }";

            CompileAndTest(text, 1);
        }

        [TestMethod]
        public void FailsIfLocationParamDefaultsToHardcodedStringOtherThanGlobal()
        {
            // From https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/test-cases#location-uses-parameter
            // Uses a location parameter but fails this test because the location parameter defaults to a hardcoded location.
            string text = @"
                resource storageaccount1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
                    name: 'storageaccount1'
                    location: 'westus'
                    kind: 'StorageV2'
                    sku: {
                        name: 'Premium_LRS'
                        tier: 'Premium'
                    }
                }";

            CompileAndTest(text, 1);
        }

        [TestMethod]
        public void FailsIfLocationParamDefaultsToUnapprovedExpression()
        {
            string text = @"
                param location string = resourceGroup() // Supposed to default to resourceGroup().location

                resource storageaccount1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
                    name: 'storageaccount1'
                    location: location
                    kind: 'StorageV2'
                    sku: {
                        name: 'Premium_LRS'
                        tier: 'Premium'
                    }
                }";

            CompileAndTest(text, 1);
        }

        [TestMethod]
        public void FailsIfLocationParamDefaultsToUnapprovedExpression2()
        {
            string text = @"
                param location string = resourceGroup().location.anything // Supposed to default to resourceGroup().location

                resource storageaccount1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
                    name: 'storageaccount1'
                    location: location
                    kind: 'StorageV2'
                    sku: {
                        name: 'Premium_LRS'
                        tier: 'Premium'
                    }
                }";

            CompileAndTest(text, 1);
        }

        [TestMethod]
        public void DoesNotApplyToPropertiesOfAnObject()
        {
            string text = @"
                var var1 = {
                    location: 'westus'
                }";

            CompileAndTest(text, 0);
        }

        [TestMethod]
        public void DoesNotApplyToDeepPropertiesOfAResource()
        {
            string text = @"
                param location string

                resource createDevopsPipeline 'Microsoft.Unknown/whatever@2019-10-01-preview' = {
                    name: 'createDevopsPipeline'
                    location: location // This one passes
                    kind: 'AzureCLI'
                    properties: {
                        location: 'westus' // This one does not apply and is not tested (not a top-level property of a resource)
                    }
                }";

            CompileAndTest(text, 0);
        }

        [TestMethod]
        public void AppliesToParentAndChildResources()
        {
            string text = @"
                resource myParent 'My.Rp/parentType@2020-01-01' = {
                    name: 'myParent'
                    location: 'West US'

                    // declares a resource of type 'My.Rp/parentType/childType@2020-01-01'
                    resource myChild 'childType' = {
                        name: 'myChild'
                        location: 'West US'
                        properties: {
                        displayName: 'child in ${myParent.location}'
                        }
                    }
                }";

            CompileAndTest(text, 2);
        }

        [TestMethod]
        public void PassesForValidUsageInModuleReference()
        {
            string text = @"
                @minLength(3)
                @maxLength(11)
                param namePrefix string
                param location string = resourceGroup().location

                module stgModule './storageAccount.bicep' = {
                    name: 'storageDeploy'
                    params: {
                        storagePrefix: namePrefix
                        location: location
                    }
                }

                output location object = stgModule.outputs.storageEndpoint
                output location string = location
            ";

            CompileAndTest(text, 0);
        }

        [TestMethod]
        public void AppliesToLoops()
        {
            string text = @"
                param location string = 'westus'

                var nsgNames = [
                'nsg1'
                'nsg2'
                'nsg3'
                ]

                resource nsg 'Microsoft.Network/networkSecurityGroups@2020-06-01' = [for name in nsgNames: {
                    name: name
                    location: location
                }]
            ";

            CompileAndTest(text, 1);
        }

        [TestMethod]
        public void AppliesToLoops2()
        {
            string text = @"
                var nsgNames = [
                'nsg1'
                'nsg2'
                'nsg3'
                ]

                resource nsg 'Microsoft.Network/networkSecurityGroups@2020-06-01' = [for name in nsgNames: {
                    name: name
                    location: 'westus'
                }]
            ";

            CompileAndTest(text, 1);
        }

        [TestMethod]
        public void UnaffectedBySyntaxErrors()
        {
            string text = @"
                resource abc 'Microsoft.AAD/domainServices@2021-03-01' // missing body
            ";

            CompileAndTest(text, 0);
        }

        [TestMethod]
        public void armTtk347()
        {
            string text = @"
                targetScope = 'subscription'

                param workspaceId string

                module diagnosticSettings './activity-log-diagnostics.bicep' = {
                name: 'DiagnosticSettings'
                scope: subscription()
                params: {
                        workspaceId: workspaceId
                    }
                }";

            CompileAndTest(text, 0);
        }
    }
}
