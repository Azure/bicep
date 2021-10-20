// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.Scenarios
{
    [TestClass]
    public class ResourceListFunctionTests
    {
        [TestMethod]
        public void List_wildcard_function_on_resource_references()
        {
            var result = CompilationHelper.Compile(@"
resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'testacc'
  location: 'West US'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

output pkStandard string = listKeys(stg.id, stg.apiVersion).keys[0].value
output pkMethod string = stg.listKeys().keys[0].value
output pkMethodVersionOverride string = stg.listKeys('2021-01-01').keys[0].value
output pkMethodPayload string = stg.listKeys(stg.apiVersion, {
  key1: 'val1'
})
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['pkStandard'].value", "[listKeys(resourceId('Microsoft.Storage/storageAccounts', 'testacc'), '2019-06-01').keys[0].value]");
            result.Template.Should().HaveValueAtPath("$.outputs['pkMethod'].value", "[listKeys(resourceId('Microsoft.Storage/storageAccounts', 'testacc'), '2019-06-01').keys[0].value]");
            result.Template.Should().HaveValueAtPath("$.outputs['pkMethodVersionOverride'].value", "[listKeys(resourceId('Microsoft.Storage/storageAccounts', 'testacc'), '2021-01-01').keys[0].value]");
            result.Template.Should().HaveValueAtPath("$.outputs['pkMethodPayload'].value", "[listKeys(resourceId('Microsoft.Storage/storageAccounts', 'testacc'), '2019-06-01', createObject('key1', 'val1'))]");
        }

        [TestMethod]
        public void List_wildcard_function_on_cross_scope_resource_references()
        {
            var result = CompilationHelper.Compile(@"
resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' existing = {
  scope: resourceGroup('other')
  name: 'testacc'
}

output pkStandard string = listKeys(stg.id, stg.apiVersion).keys[0].value
output pkMethod string = stg.listKeys().keys[0].value
output pkMethodVersionOverride string = stg.listKeys('2021-01-01').keys[0].value
output pkMethodPayload string = stg.listKeys(stg.apiVersion, {
  key1: 'val1'
})
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['pkStandard'].value", "[listKeys(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'other'), 'Microsoft.Storage/storageAccounts', 'testacc'), '2019-06-01').keys[0].value]");
            result.Template.Should().HaveValueAtPath("$.outputs['pkMethod'].value", "[listKeys(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'other'), 'Microsoft.Storage/storageAccounts', 'testacc'), '2019-06-01').keys[0].value]");
            result.Template.Should().HaveValueAtPath("$.outputs['pkMethodVersionOverride'].value", "[listKeys(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'other'), 'Microsoft.Storage/storageAccounts', 'testacc'), '2021-01-01').keys[0].value]");
            result.Template.Should().HaveValueAtPath("$.outputs['pkMethodPayload'].value", "[listKeys(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'other'), 'Microsoft.Storage/storageAccounts', 'testacc'), '2019-06-01', createObject('key1', 'val1'))]");
        }

        [TestMethod]
        public void Only_list_methods_are_permitted()
        {
            var result = CompilationHelper.Compile(@"
resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' existing = {
  name: 'testacc'
}

var allowed = {
  a: stg.list()
  b: stg.listA()
  c: stg.listTotallyMadeUpMethod()
}

var disallowed = {
  a: stg.lis()
  b: stg.lsit()
  c: stg.totallyMadeUpMethod()
}
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP109", DiagnosticLevel.Error, "The type \"Microsoft.Storage/storageAccounts\" does not contain function \"lis\"."),
                ("BCP109", DiagnosticLevel.Error, "The type \"Microsoft.Storage/storageAccounts\" does not contain function \"lsit\"."),
                ("BCP109", DiagnosticLevel.Error, "The type \"Microsoft.Storage/storageAccounts\" does not contain function \"totallyMadeUpMethod\"."),
            });
        }
    }
}