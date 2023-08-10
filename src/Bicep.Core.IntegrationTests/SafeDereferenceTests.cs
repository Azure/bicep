// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class SafeDereferenceTests
{
    private ServiceBuilder ServicesWithResourceTypedParamsAndOutputsEnabled => new ServiceBuilder()
        .WithFeatureOverrides(new(TestContext, ResourceTypedParamsAndOutputsEnabled: true));

    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void Safe_dereference_is_not_permitted_on_instance_functions()
    {
        var result = CompilationHelper.Compile(@"
resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' existing = {
  name: 'sa'
}

output secret string = storageaccount.?listKeys().keys[0].value
");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP322", DiagnosticLevel.Error, "The `.?` (safe dereference) operator may not be used on instance function invocations.")
        });
    }

    [TestMethod]
    public void Safe_dereference_is_not_permitted_on_resource_collections()
    {
        var result = CompilationHelper.Compile(@"
resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' existing = [for i in range(0, 10): {
  name: 'name${i}'
}]

output data object = {
  vm3Name: vm[?3].name
}
");

        result.Should().HaveDiagnostics(new[]
        {
            ("BCP323", DiagnosticLevel.Error, "The `[?]` (safe dereference) operator may not be used on resource or module collections.")
        });
    }

    [TestMethod]
    public void Safe_dereference_is_not_permitted_on_module_collections()
    {
        var result = CompilationHelper.Compile(
("main.bicep", @"
module mod './module.bicep' = [for (item, i) in []:  {
    name: 'test-${i}'
}]

output data object = {
  foo: mod[?0].outputs.data.foo
}
"),
("module.bicep", @"
output data object = {
  foo: 'bar'
}
"));

        result.Should().HaveDiagnostics(new[]
        {
            ("BCP323", DiagnosticLevel.Error, "The `[?]` (safe dereference) operator may not be used on resource or module collections.")
        });
    }

    [TestMethod]
    public void Safe_dereference_of_declared_resource_properties_converts_correctly()
    {
        var result = CompilationHelper.Compile(@"
resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' existing = {
  name: 'name'
}

output outputData object = {
  vmName: vm.?name
  vmId: vm.?id
  vmProperties: vm.?properties
  vmIdentity: vm.?identity
  vmPlanName: vm.?plan.name
  vmEvictionPolicy: vm.properties.?evictionPolicy
  vmMaxPrice: vm.properties.?billingProfile.maxPrice
}
");
        var compiledOutputData = result.Template?["outputs"]?["outputData"]?["value"];
        compiledOutputData.Should().NotBeNull();

        compiledOutputData!["vmName"].Should().DeepEqual("name");
        compiledOutputData!["vmId"].Should().DeepEqual("[resourceId('Microsoft.Compute/virtualMachines', 'name')]");
        compiledOutputData!["vmProperties"].Should().DeepEqual("[tryGet(reference(resourceId('Microsoft.Compute/virtualMachines', 'name'), '2020-06-01', 'full'), 'properties')]");
        compiledOutputData!["vmIdentity"].Should().DeepEqual("[tryGet(reference(resourceId('Microsoft.Compute/virtualMachines', 'name'), '2020-06-01', 'full'), 'identity')]");
        compiledOutputData!["vmPlanName"].Should().DeepEqual("[tryGet(reference(resourceId('Microsoft.Compute/virtualMachines', 'name'), '2020-06-01', 'full'), 'plan', 'name')]");
        compiledOutputData!["vmEvictionPolicy"].Should().DeepEqual("[tryGet(reference(resourceId('Microsoft.Compute/virtualMachines', 'name'), '2020-06-01'), 'evictionPolicy')]");
        compiledOutputData!["vmMaxPrice"].Should().DeepEqual("[tryGet(reference(resourceId('Microsoft.Compute/virtualMachines', 'name'), '2020-06-01'), 'billingProfile', 'maxPrice')]");
    }

    [TestMethod]
    public void Safe_dereference_of_parameter_resource_properties_converts_correctly()
    {
        var result = CompilationHelper.Compile(ServicesWithResourceTypedParamsAndOutputsEnabled, @"
param vm resource 'Microsoft.Compute/virtualMachines@2020-06-01'

output outputData object = {
  vmName: vm.?name
  vmId: vm.?id
  vmProperties: vm.?properties
  vmIdentity: vm.?identity
  vmPlanName: vm.?plan.name
  vmEvictionPolicy: vm.properties.?evictionPolicy
  vmMaxPrice: vm.properties.?billingProfile.maxPrice
  maybeVmMaxPrice: vm.?properties.billingProfile.?maxPrice
}
");
        var compiledOutputData = result.Template?["outputs"]?["outputData"]?["value"];
        compiledOutputData.Should().NotBeNull();

        compiledOutputData!["vmName"].Should().DeepEqual("[last(split(parameters('vm'), '/'))]");
        compiledOutputData!["vmId"].Should().DeepEqual("[parameters('vm')]");
        compiledOutputData!["vmProperties"].Should().DeepEqual("[tryGet(reference(parameters('vm'), '2020-06-01', 'full'), 'properties')]");
        compiledOutputData!["vmIdentity"].Should().DeepEqual("[tryGet(reference(parameters('vm'), '2020-06-01', 'full'), 'identity')]");
        compiledOutputData!["vmPlanName"].Should().DeepEqual("[tryGet(reference(parameters('vm'), '2020-06-01', 'full'), 'plan', 'name')]");
        compiledOutputData!["vmEvictionPolicy"].Should().DeepEqual("[tryGet(reference(parameters('vm'), '2020-06-01'), 'evictionPolicy')]");
        compiledOutputData!["vmMaxPrice"].Should().DeepEqual("[tryGet(reference(parameters('vm'), '2020-06-01'), 'billingProfile', 'maxPrice')]");
        compiledOutputData!["maybeVmMaxPrice"].Should().DeepEqual("[tryGet(tryGet(reference(parameters('vm'), '2020-06-01', 'full'), 'properties', 'billingProfile'), 'maxPrice')]");
    }

    [TestMethod]
    public void Safe_dereference_of_module_output_resource_properties_converts_correctly()
    {
        var result = CompilationHelper.Compile(ServicesWithResourceTypedParamsAndOutputsEnabled,
            ("mod.bicep", @"
resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' existing = {
  name: 'name'
}

output vm resource = vm
"),
            ("main.bicep", @"
module mod './mod.bicep' = {
  name: 'mod'
}

output outputData object = {
  vmMaybeName: mod.outputs.vm.?name
  maybeVmName: mod.outputs.?vm.name
  maybeVmMaybeName: mod.outputs.?vm.?name
  vmMaybeId: mod.outputs.vm.?id
  maybeVmId: mod.outputs.?vm.id
  maybeVmMaybeId: mod.outputs.?vm.?id
}
"));
        var compiledOutputData = result.Template?["outputs"]?["outputData"]?["value"];
        compiledOutputData.Should().NotBeNull();

        // there's no way for `.name` or `.id` to be null on a module output resource, hence the lack of `tryGet`
        compiledOutputData!["vmMaybeName"].Should().DeepEqual("[last(split(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs.vm.value, '/'))]");
        compiledOutputData!["vmMaybeId"].Should().DeepEqual("[reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs.vm.value]");

        // if the output resource itself uses a safe dereference, though, the generated expressions get more complex
        compiledOutputData!["maybeVmId"].Should().DeepEqual("[tryGet(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs, 'vm', 'value')]");
        compiledOutputData!["maybeVmMaybeId"].Should().DeepEqual("[tryGet(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs, 'vm', 'value')]");
        compiledOutputData!["maybeVmName"].Should().DeepEqual("[if(contains(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs, 'vm'), last(split(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs.vm.value, '/')), null())]");
        compiledOutputData!["maybeVmMaybeName"].Should().DeepEqual("[if(contains(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs, 'vm'), last(split(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs.vm.value, '/')), null())]");
    }

    [TestMethod]
    public void Safe_dereference_of_module_output_properties_converts_correctly()
    {
        var result = CompilationHelper.Compile(ServicesWithResourceTypedParamsAndOutputsEnabled,
            ("mod.bicep", @"
output outputData object = {
  key: 'value'
  nested: {
    key: 'value'
  }
}
"),
            ("main.bicep", @"
module mod './mod.bicep' = {
  name: 'mod'
}

output outputData object = {
  output: mod.outputs.outputData
  maybeOutput: mod.outputs.?outputData
  topLevelProperty: mod.outputs.outputData.key
  maybeTopLevelProperty: mod.outputs.outputData.?key
  maybeOutputTopLevelProperty: mod.outputs.?outputData.key
  maybeOutputMaybeTopLevelProperty: mod.outputs.?outputData.?key
  nestedProperty: mod.outputs.outputData.nested.key
  maybeNestedProperty: mod.outputs.outputData.?nested.key
  maybeOutputNestedProperty: mod.outputs.?outputData.nested.key
}
"));
        var compiledOutputData = result.Template?["outputs"]?["outputData"]?["value"];
        compiledOutputData.Should().NotBeNull();

        compiledOutputData!["output"].Should().DeepEqual("[reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs.outputData.value]");
        compiledOutputData!["maybeOutput"].Should().DeepEqual("[tryGet(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs, 'outputData', 'value')]");
        compiledOutputData!["topLevelProperty"].Should().DeepEqual("[reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs.outputData.value.key]");
        compiledOutputData!["maybeTopLevelProperty"].Should().DeepEqual("[tryGet(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs.outputData.value, 'key')]");
        compiledOutputData!["maybeOutputTopLevelProperty"].Should().DeepEqual("[tryGet(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs, 'outputData', 'value', 'key')]");
        compiledOutputData!["maybeOutputMaybeTopLevelProperty"].Should().DeepEqual("[tryGet(tryGet(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs, 'outputData', 'value'), 'key')]");
        compiledOutputData!["nestedProperty"].Should().DeepEqual("[reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs.outputData.value.nested.key]");
        compiledOutputData!["maybeNestedProperty"].Should().DeepEqual("[tryGet(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs.outputData.value, 'nested', 'key')]");
        compiledOutputData!["maybeOutputNestedProperty"].Should().DeepEqual("[tryGet(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs, 'outputData', 'value', 'nested', 'key')]");
    }

    [TestMethod]
    public void Access_chains_consider_safe_dereference_in_type_assignment()
    {
        var result = CompilationHelper.Compile(@"
@minLength(1)
param foo (null
  | {
      nested: {
        deeplyNested: 'value'
      }
    })[]

output topLevel object = foo[0]
output nested object = foo[0].?nested
output nestedAlt object = foo[?0].nested
output deeplyNested string = foo[0].?nested.deeplyNested
");
        result.Should().HaveDiagnostics(new[] {
            ("BCP321", DiagnosticLevel.Warning, "Expected a value of type \"object\" but the provided value is of type \"null | { nested: { deeplyNested: 'value' } }\"."),
            ("BCP321", DiagnosticLevel.Warning, "Expected a value of type \"object\" but the provided value is of type \"null | { deeplyNested: 'value' }\"."),
            ("BCP321", DiagnosticLevel.Warning, "Expected a value of type \"object\" but the provided value is of type \"null | { deeplyNested: 'value' }\"."),
            ("BCP321", DiagnosticLevel.Warning, "Expected a value of type \"string\" but the provided value is of type \"'value' | null\"."),
        });
    }

    [TestMethod]
    public void Safe_dereference_of_unknown_property_should_be_warning_not_error()
    {
        var result = CompilationHelper.Compile(@"
var foo = {
  bar: 'present'
}
var baz = 'baz'

output nulls object = {
  propertyAccess: foo.?baz
  arrayAccess: foo[?baz]
}
");
        result.Should().HaveDiagnostics(new[] {
            ("BCP083", DiagnosticLevel.Warning, @"The type ""object"" does not contain property ""baz"". Did you mean ""bar""?"),
            ("BCP083", DiagnosticLevel.Warning, @"The type ""object"" does not contain property ""baz"". Did you mean ""bar""?"),
        });
    }
}
