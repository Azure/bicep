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
    public class UseStableResourceIdentifiersRuleTests : LinterRuleTestsBase
    {
        private void CompileAndTest(string text, params string[] expectedMessages)
        {
            AssertLinterRuleDiagnostics(UseStableResourceIdentifiersRule.Code, text, diags =>
            {
                if (expectedMessages.Any())
                {
                    diags.Where(e => e.Code == UseStableResourceIdentifiersRule.Code).Select(e => e.Message).Should().Contain(expectedMessages);
                }
                else
                {
                    diags.Where(e => e.Code == UseStableResourceIdentifiersRule.Code).Count().Should().Be(0);
                }
            });
        }

        [DataRow(@"
            param location string = resourceGroup().location

            resource storage 'Microsoft.Storage/storageAccounts@2021-09-01' = {
              name: 'literalName'
              location: location
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
            }"
        )]
        [DataRow(@"
            param location string = resourceGroup().location
            param snap string

            var crackle = 'crackle'
            var pop = '${snap}${crackle}'

            resource storage 'Microsoft.Storage/storageAccounts@2021-09-01' = {
              name: pop
              location: location
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
            }"
        )]
        [DataRow(@"
            param location string = resourceGroup().location
            param snap string = newGuid()

            var crackle = snap
            var pop = '${snap}${crackle}'

            resource storage 'Microsoft.Storage/storageAccounts@2021-09-01' = {
              name: pop
              location: location
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
            }",
            "Resource identifiers should be reproducible outside of their initial deployment context. Resource storage's 'name' identifier is potentially nondeterministic due to its use of the 'newGuid' function (storage.name -> pop -> snap (default value) -> newGuid()).",
            "Resource identifiers should be reproducible outside of their initial deployment context. Resource storage's 'name' identifier is potentially nondeterministic due to its use of the 'newGuid' function (storage.name -> pop -> crackle -> snap (default value) -> newGuid())."
        )]
        [DataRow(@"
            param location string = resourceGroup().location
            param snap string = utcNow('F')

            var crackle = snap
            var pop = '${snap}${crackle}'

            resource storage 'Microsoft.Storage/storageAccounts@2021-09-01' = {
              name: pop
              location: location
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
            }",
            "Resource identifiers should be reproducible outside of their initial deployment context. Resource storage's 'name' identifier is potentially nondeterministic due to its use of the 'utcNow' function (storage.name -> pop -> snap (default value) -> utcNow('F')).",
            "Resource identifiers should be reproducible outside of their initial deployment context. Resource storage's 'name' identifier is potentially nondeterministic due to its use of the 'utcNow' function (storage.name -> pop -> crackle -> snap (default value) -> utcNow('F'))."
        )]
        [DataRow(@"
            param location string = resourceGroup().location
            param snap string = '${newGuid()}${newGuid()}${utcNow('u')}'

            var crackle = snap
            var pop = '${snap}${crackle}'

            resource storage 'Microsoft.Storage/storageAccounts@2021-09-01' = {
              name: pop
              location: location
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }
            }",
            "Resource identifiers should be reproducible outside of their initial deployment context. Resource storage's 'name' identifier is potentially nondeterministic due to its use of the 'newGuid' function (storage.name -> pop -> snap (default value) -> newGuid()).",
            "Resource identifiers should be reproducible outside of their initial deployment context. Resource storage's 'name' identifier is potentially nondeterministic due to its use of the 'newGuid' function (storage.name -> pop -> snap (default value) -> newGuid()).",
            "Resource identifiers should be reproducible outside of their initial deployment context. Resource storage's 'name' identifier is potentially nondeterministic due to its use of the 'utcNow' function (storage.name -> pop -> snap (default value) -> utcNow('u')).",
            "Resource identifiers should be reproducible outside of their initial deployment context. Resource storage's 'name' identifier is potentially nondeterministic due to its use of the 'newGuid' function (storage.name -> pop -> crackle -> snap (default value) -> newGuid()).",
            "Resource identifiers should be reproducible outside of their initial deployment context. Resource storage's 'name' identifier is potentially nondeterministic due to its use of the 'newGuid' function (storage.name -> pop -> crackle -> snap (default value) -> newGuid()).",
            "Resource identifiers should be reproducible outside of their initial deployment context. Resource storage's 'name' identifier is potentially nondeterministic due to its use of the 'utcNow' function (storage.name -> pop -> crackle -> snap (default value) -> utcNow('u'))."
        )]
        [DataTestMethod]
        public void TestRule(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, expectedMessages);
        }
    }
}
