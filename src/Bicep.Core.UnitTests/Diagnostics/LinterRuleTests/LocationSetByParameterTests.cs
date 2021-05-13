// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRules
{
    [TestClass]
    public class LocationSetByParameterTests
    {
        private void CompileAndTest(string text, int expectedDiagnosticCount)
        {
            string ruleCode = new LocationSetByParameterRule().Code;
            var compilationResult = CompilationHelper.Compile(text);
            var ruleErrors = compilationResult.Diagnostics.Where(d => d.Code == ruleCode).ToArray();
            Assert.AreEqual(expectedDiagnosticCount, ruleErrors.Count());
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

    }
}
