
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class UnnecessaryDependsOnRuleTests : LinterRuleTestsBase
    {
        private void ExpectPass(string text, string? because = null)
        {
            using (new AssertionScope($"linter errors for this code:\n{text}\n"))
            {
                var errors = GetDiagnostics(UnnecessaryDependsOnRule.Code, text);
                errors.Should().HaveCount(0, because ?? $"expecting linter rule to pass");
            }
        }

        private void ExpectDiagnostic(string text, string[] unnecessaryFragments, string? because = null)
        {
            using (new AssertionScope($"linter errors for this code:\n{text}\n"))
            {
                var errors = GetDiagnostics(UnnecessaryDependsOnRule.Code, text);
                errors.Should().HaveCount(unnecessaryFragments.Length, because);

                var actualFragments = errors.Select(e => text.Substring(e.Span.Position, e.Span.Length));
                actualFragments.Should().BeEquivalentTo(unnecessaryFragments, because);
            }
        }

        // TODO: Test with loops and loop indicies
        // TODO: test: If you have a parent and child, and the child references resource1, that does not imply that parent has a reference on resource1
        [DataRow(
            @"
                param location string

                resource VNet1 'Microsoft.Network/virtualNetworks@2018-10-01' = {
                    name: 'VNet1'
                    location: location
                    properties: {
                        addressSpace: {
                            addressPrefixes: [
                                '10.0.0.0/16'
                            ]
                        }
                    }
                }

                resource VNet1_Subnet1 'Microsoft.Network/virtualNetworks/subnets@2018-10-01' = {
                    name: '${VNet1.name}/Subnet1'
                    properties: {
                        addressPrefix: '10.0.0.0/24'
                    }
                    dependsOn: [
                        VNet1 // Reference to parent not needed in bicep
                    ]
                }
            ",
            "VNet1"
        )]
        [DataRow(
            @"
                param location string

                resource VNet1 'Microsoft.Network/virtualNetworks@2018-10-01' = {
                    name: 'VNet1'
                    location: location
                    properties: {
                        addressSpace: {
                        addressPrefixes: [
                            '10.0.0.0/16'
                        ]
                        }
                    }
                }

                resource VNet1_Subnet1 'Microsoft.Network/virtualNetworks/subnets@2018-10-01' = {
                    name: '${VNet1.name}/Subnet1'
                    properties: {
                        addressPrefix: '10.0.0.0/24'
                    }
                    dependsOn: [
                        VNet1 // ref to parent not needed in bicep
                    ]
                }
            ",
            "VNet1"
        )]
        [DataTestMethod]
        public void ReferenceToParent_Fails(string text, params string[] unnecessaryFragments)
        {
            // TODO: test with grandparent
            ExpectDiagnostic(text, unnecessaryFragments);
        }

        [TestMethod]
        public void ReferencingSymbolNameCreatesImplicitReference_Fail()
        {
            string text = @"
                resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
                    name: 'myZone'
                    location: 'global'
                }

                resource otherResource 'Microsoft.Example/examples@2020-06-01' = {
                    name: 'exampleResource'
                    dependsOn: [
                        dnsZone
                    ]
                    properties: {
                        // get read-only DNS zone property
                        nameServers: dnsZone.properties.nameServers
                    }
                }
            ";
            ExpectDiagnostic(text, new string[] { "dnsZone" }, "there's already a reference to dnsZone symbolic name in properties.nameServers");
        }

        [TestMethod]
        public void ReferencingSymbolNameCreatesImplicitReference_Passes()
        {
            string text = @"
                resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
                    name: 'myZone'
                    location: 'global'
                }

                resource otherResource2 'Microsoft.Example/examples@2020-06-01' = {
                    name: 'exampleResource2'
                    dependsOn: [
                        dnsZone
                    ]
                }
            ";
            ExpectPass(text, "there is no reference to dnsZone symbolic name otherwise");
        }
    }
}
