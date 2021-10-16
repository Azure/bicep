// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class UseStableVMImageRuleTests : LinterRuleTestsBase
    {
        private void CompileAndTest(string text, params string[] useStableVMImage)
        {
            AssertLinterRuleDiagnostics(UseStableVMImageRule.Code, text, diags =>
            {
                if (useStableVMImage.Any())
                {
                    var rule = new UseStableVMImageRule();
                    string[] expectedMessages = useStableVMImage.Select(p => rule.GetMessage(p)).ToArray();
                    diags.Select(e => e.Message).Should().ContainInOrder(expectedMessages);
                }
                else
                {
                    diags.Should().BeEmpty();
                }
            });
        }

        [DataRow(@"
            resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
              name: 'virtualMachineName'
              location: resourceGroup().location
              properties: {
                storageProfile: {
                  imageReference: {
                    offer: 'WindowsServer'
                    sku: '2019-Datacenter'
                    version: 'latest'
                  }
                }
              }
            }")]
        [DataRow(@"
            resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
              name: 'virtualMachineName'
              location: resourceGroup().location
              properties: {
                storageProfile: {
                }
              }
            }")]
        [DataRow(@"
            resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
              name: 'virtualMachineName'
              location: resourceGroup().location
              properties: {
                storageProfile: {
                  imageReference: {
                    offer: 'WindowsServer'
                    sku: '2019-Datacenter'
                    version: 'preview'
                  }
                }
              }
            }", "version")]
        [DataRow(@"
            resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
              name: 'virtualMachineName'
              location: resourceGroup().location
              properties: {
                storageProfile: {
                  imageReference: {
                    offer: 'WindowsServer-preview'
                    sku: '2019-Datacenter'
                    version: 'latest'
                  }
                }
              }
            }", "offer")]
        [DataRow(@"
            resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
              name: 'virtualMachineName'
              location: resourceGroup().location
              properties: {
                storageProfile: {
                  imageReference: {
                    offer: 'WindowsServer'
                    sku: '2019-Datacenter-preview'
                    version: 'latest'
                  }
                }
              }
            }", "sku")]
        [DataRow(@"
            resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
              name: 'virtualMachineName'
              location: resourceGroup().location
              properties: {
                storageProfile: {
                  imageReference: {
                    offer: 'WindowsServer-preview'
                    sku: '2019-Datacenter-preview'
                    version: 'preview'
                  }
                }
              }
            }", "offer", "sku", "version")]
        [DataRow(@"
            var imageReference = {
              offer: 'WindowsServer-preview'
              sku: '2019-Datacenter'
              version: 'preview'
             }
             resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
              name: 'virtualMachineName'
              location: resourceGroup().location
              properties: {
                storageProfile: {
                  imageReference: imageReference
                }
              }
            }", "offer", "version")]
        [DataTestMethod]
        public void TestRule(string text, params string[] useRecentApiVersions)
        {
            CompileAndTest(text, useRecentApiVersions);
        }
    }
}
