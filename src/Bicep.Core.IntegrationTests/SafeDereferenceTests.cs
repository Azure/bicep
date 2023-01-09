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
    private ServiceBuilder ServicesWithUserDefinedTypes => new ServiceBuilder()
        .WithFeatureOverrides(new(TestContext, UserDefinedTypesEnabled: true));

    [NotNull]
    public TestContext? TestContext { get; set; }

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
        compiledOutputData!["vmProperties"].Should().DeepEqual("[reference(resourceId('Microsoft.Compute/virtualMachines', 'name'), '2020-06-01')]");
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
}
");
        var compiledOutputData = result.Template?["outputs"]?["outputData"]?["value"];
        compiledOutputData.Should().NotBeNull();

        compiledOutputData!["vmName"].Should().DeepEqual("[last(split(parameters('vm'), '/'))]");
        compiledOutputData!["vmId"].Should().DeepEqual("[parameters('vm')]");
        compiledOutputData!["vmProperties"].Should().DeepEqual("[reference(parameters('vm'), '2020-06-01')]");
        compiledOutputData!["vmIdentity"].Should().DeepEqual("[tryGet(reference(parameters('vm'), '2020-06-01', 'full'), 'identity')]");
        compiledOutputData!["vmPlanName"].Should().DeepEqual("[tryGet(reference(parameters('vm'), '2020-06-01', 'full'), 'plan', 'name')]");
        compiledOutputData!["vmEvictionPolicy"].Should().DeepEqual("[tryGet(reference(parameters('vm'), '2020-06-01'), 'evictionPolicy')]");
        compiledOutputData!["vmMaxPrice"].Should().DeepEqual("[tryGet(reference(parameters('vm'), '2020-06-01'), 'billingProfile', 'maxPrice')]");
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
  vmName: mod.outputs.vm.?name
  vmId: mod.outputs.vm.?id
  vmProperties: mod.outputs.vm.?properties
  vmIdentity: mod.outputs.vm.?identity
  vmPlanName: mod.outputs.vm.?plan.name
  vmEvictionPolicy: mod.outputs.vm.properties.?evictionPolicy
  vmMaxPrice: mod.outputs.vm.properties.?billingProfile.maxPrice
}
"));
        var compiledOutputData = result.Template?["outputs"]?["outputData"]?["value"];
        compiledOutputData.Should().NotBeNull();

        compiledOutputData!["vmName"].Should().DeepEqual("[last(split(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2020-10-01').outputs.vm.value, '/'))]");
        compiledOutputData!["vmId"].Should().DeepEqual("[reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2020-10-01').outputs.vm.value]");
        compiledOutputData!["vmProperties"].Should().DeepEqual("[reference(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2020-10-01').outputs.vm.value, '2020-06-01')]");
        compiledOutputData!["vmIdentity"].Should().DeepEqual("[tryGet(reference(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2020-10-01').outputs.vm.value, '2020-06-01', 'full'), 'identity')]");
        compiledOutputData!["vmPlanName"].Should().DeepEqual("[tryGet(reference(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2020-10-01').outputs.vm.value, '2020-06-01', 'full'), 'plan', 'name')]");
        compiledOutputData!["vmEvictionPolicy"].Should().DeepEqual("[tryGet(reference(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2020-10-01').outputs.vm.value, '2020-06-01'), 'evictionPolicy')]");
        compiledOutputData!["vmMaxPrice"].Should().DeepEqual("[tryGet(reference(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2020-10-01').outputs.vm.value, '2020-06-01'), 'billingProfile', 'maxPrice')]");
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

        compiledOutputData!["output"].Should().DeepEqual("[reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2020-10-01').outputs.outputData.value]");
        compiledOutputData!["maybeOutput"].Should().DeepEqual("[tryGet(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2020-10-01').outputs, 'outputData', 'value')]");
        compiledOutputData!["topLevelProperty"].Should().DeepEqual("[reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2020-10-01').outputs.outputData.value.key]");
        compiledOutputData!["maybeTopLevelProperty"].Should().DeepEqual("[tryGet(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2020-10-01').outputs.outputData.value, 'key')]");
        compiledOutputData!["maybeOutputTopLevelProperty"].Should().DeepEqual("[tryGet(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2020-10-01').outputs, 'outputData', 'value', 'key')]");
        compiledOutputData!["maybeOutputMaybeTopLevelProperty"].Should().DeepEqual("[tryGet(tryGet(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2020-10-01').outputs, 'outputData', 'value'), 'key')]");
        compiledOutputData!["nestedProperty"].Should().DeepEqual("[reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2020-10-01').outputs.outputData.value.nested.key]");
        compiledOutputData!["maybeNestedProperty"].Should().DeepEqual("[tryGet(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2020-10-01').outputs.outputData.value, 'nested', 'key')]");
        compiledOutputData!["maybeOutputNestedProperty"].Should().DeepEqual("[tryGet(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2020-10-01').outputs, 'outputData', 'value', 'nested', 'key')]");
    }

    [TestMethod]
    public void Access_chains_consider_safe_dereference_in_type_assignment()
    {
        var result = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
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
            ("BCP026", DiagnosticLevel.Error, "The output expects a value of type \"object\" but the provided value is of type \"null | { nested: { deeplyNested: 'value' } }\"."),
            ("BCP026", DiagnosticLevel.Error, "The output expects a value of type \"object\" but the provided value is of type \"null | { deeplyNested: 'value' }\"."),
            ("BCP026", DiagnosticLevel.Error, "The output expects a value of type \"object\" but the provided value is of type \"null | { deeplyNested: 'value' }\"."),
            ("BCP026", DiagnosticLevel.Error, "The output expects a value of type \"string\" but the provided value is of type \"'value' | null\"."),
        });
    }
}
