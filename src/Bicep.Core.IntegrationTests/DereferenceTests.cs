// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class DereferenceTests
{
    private ServiceBuilder ServicesWithResourceTypedParamsAndOutputsEnabled => new ServiceBuilder()
        .WithFeatureOverrides(new(TestContext, ResourceTypedParamsAndOutputsEnabled: true));

    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void Safe_dereference_is_not_permitted_on_instance_functions()
    {
        var result = CompilationHelper.Compile("""
            resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' existing = {
              name: 'storageacct'
            }

            output secret string = storageaccount.?listKeys().keys[0].value
            """);

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP322", DiagnosticLevel.Error, "The `.?` (safe dereference) operator may not be used on instance function invocations.")
        });
    }

    [TestMethod]
    public void Safe_dereference_is_not_permitted_on_resource_collections()
    {
        var result = CompilationHelper.Compile("""
            resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' existing = [for i in range(0, 10): {
              name: 'name${i}'
            }]

            output data object = {
              vm3Name: vm[?3].name
            }
            """);

        result.Should().HaveDiagnostics(new[]
        {
            ("BCP323", DiagnosticLevel.Error, "The `[?]` (safe dereference) operator may not be used on resource or module collections.")
        });
    }

    [TestMethod]
    public void Safe_dereference_is_not_permitted_on_module_collections()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                module mod './module.bicep' = [for (item, i) in []:  {
                    name: 'test-${i}'
                }]

                output data object = {
                  foo: mod[?0].outputs.data.foo
                }
                """),
            ("module.bicep", """
                output data object = {
                  foo: 'bar'
                }
                """));

        result.Should().HaveDiagnostics(new[]
        {
            ("BCP323", DiagnosticLevel.Error, "The `[?]` (safe dereference) operator may not be used on resource or module collections.")
        });
    }

    [TestMethod]
    public void Safe_dereference_of_declared_resource_properties_converts_correctly()
    {
        var result = CompilationHelper.Compile("""
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
            """);
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
        var result = CompilationHelper.Compile(ServicesWithResourceTypedParamsAndOutputsEnabled, """
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
            """);
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
            ("mod.bicep", """
                resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' existing = {
                  name: 'name'
                }

                output vm resource = vm
                """),
            ("main.bicep", """
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
                """));
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
            ("mod.bicep", """
                output outputData object? = {
                  key: 'value'
                  nested: {
                    key: 'value'
                  }
                }
                """),
            ("main.bicep", """
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
                """));
        var compiledOutputData = result.Template?["outputs"]?["outputData"]?["value"];
        compiledOutputData.Should().NotBeNull();

        compiledOutputData!["output"].Should().DeepEqual("[reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs.outputData.value]");
        compiledOutputData!["maybeOutput"].Should().DeepEqual("[tryGet(tryGet(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs, 'outputData'), 'value')]");
        compiledOutputData!["topLevelProperty"].Should().DeepEqual("[reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs.outputData.value.key]");
        compiledOutputData!["maybeTopLevelProperty"].Should().DeepEqual("[tryGet(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs.outputData.value, 'key')]");
        compiledOutputData!["maybeOutputTopLevelProperty"].Should().DeepEqual("[tryGet(tryGet(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs, 'outputData'), 'value', 'key')]");
        compiledOutputData!["maybeOutputMaybeTopLevelProperty"].Should().DeepEqual("[tryGet(tryGet(tryGet(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs, 'outputData'), 'value'), 'key')]");
        compiledOutputData!["nestedProperty"].Should().DeepEqual("[reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs.outputData.value.nested.key]");
        compiledOutputData!["maybeNestedProperty"].Should().DeepEqual("[tryGet(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs.outputData.value, 'nested', 'key')]");
        compiledOutputData!["maybeOutputNestedProperty"].Should().DeepEqual("[tryGet(tryGet(reference(resourceId('Microsoft.Resources/deployments', 'mod'), '2022-09-01').outputs, 'outputData'), 'value', 'nested', 'key')]");
    }

    [TestMethod]
    public void Access_chains_consider_safe_dereference_in_type_assignment()
    {
        var result = CompilationHelper.Compile("""
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
            """);
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
        var result = CompilationHelper.Compile("""
            var foo = {
              bar: 'present'
            }
            var baz = 'baz'

            output nulls object = {
              propertyAccess: foo.?baz
              arrayAccess: foo[?baz]
            }
            """);
        result.Should().HaveDiagnostics(new[] {
            ("BCP083", DiagnosticLevel.Warning, @"The type ""object"" does not contain property ""baz"". Did you mean ""bar""?"),
            ("BCP083", DiagnosticLevel.Warning, @"The type ""object"" does not contain property ""baz"". Did you mean ""bar""?"),
        });
    }

    [TestMethod]
    public void FromEnd_access_of_object_key_raises_error()
    {
        var result = CompilationHelper.Compile("""
            param anObject object
            param propertyToAccess string
            output property string = anObject[^propertyToAccess]
            """);

        result.Should().HaveDiagnostics(
        [
            ("BCP414", DiagnosticLevel.Error, @"The ""^"" indexing operator cannot be used on base expressions of type ""object""."),
        ]);
    }

    [TestMethod]
    public void FromEnd_access_with_string_index_expression_raises_error()
    {
        var result = CompilationHelper.Compile("""
            param anObject object
            param nestedPropertyToAccess string
            output property string = anObject.property[^nestedPropertyToAccess]
            """);

        result.Should().HaveDiagnostics(
        [
            ("BCP415", DiagnosticLevel.Error, @"The ""^"" indexing operator cannot be used with index expressions of type ""string""."),
        ]);
    }

    [TestMethod]
    public void Safe_dereference_guards_against_dereferencing_properties_from_disabled_resources()
    {
        var result = CompilationHelper.Compile("""
            param condition bool

            resource storageAccount 'Microsoft.Storage/storageAccounts@2021-02-01' existing = if (condition) {
              name: 'storageacct'
            }

            output id string? = storageAccount.?id
            output name string? = storageAccount.?name
            output type string? = storageAccount.?type
            output apiVersion string? = storageAccount.?apiVersion
            output properties object? = storageAccount.?properties
            output accessTier string? = storageAccount.?properties.accessTier
            output principalId string? = storageAccount.?identity.principalId
            """);

        result.Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
        result.Template.Should().HaveValueAtPath("$.outputs.id.value", "[if(parameters('condition'), resourceId('Microsoft.Storage/storageAccounts', 'storageacct'), null())]");
        result.Template.Should().HaveValueAtPath("$.outputs.name.value", "[if(parameters('condition'), 'storageacct', null())]");
        result.Template.Should().HaveValueAtPath("$.outputs.type.value", "Microsoft.Storage/storageAccounts");
        result.Template.Should().HaveValueAtPath("$.outputs.apiVersion.value", "2021-02-01");
        result.Template.Should().HaveValueAtPath("$.outputs.properties.value", "[tryGet(if(parameters('condition'), reference('storageAccount', '2021-02-01', 'full'), null()), 'properties')]");
        result.Template.Should().HaveValueAtPath("$.outputs.accessTier.value", "[tryGet(if(parameters('condition'), reference('storageAccount', '2021-02-01', 'full'), null()), 'properties', 'accessTier')]");
        result.Template.Should().HaveValueAtPath("$.outputs.principalId.value", "[tryGet(if(parameters('condition'), reference('storageAccount', '2021-02-01', 'full'), null()), 'identity', 'principalId')]");
    }

    [TestMethod]
    public void Safe_dereference_guards_against_dereferencing_properties_from_disabled_resources_with_resourceInfo()
    {
        var result = CompilationHelper.Compile(new ServiceBuilder().WithFeatureOverrides(new(ResourceInfoCodegenEnabled: true)), """
            param condition bool

            resource storageAccount 'Microsoft.Storage/storageAccounts@2021-02-01' existing = if (condition) {
              name: 'storageacct'
            }

            output id string? = storageAccount.?id
            output name string? = storageAccount.?name
            output type string? = storageAccount.?type
            output apiVersion string? = storageAccount.?apiVersion
            output properties object? = storageAccount.?properties
            output accessTier string? = storageAccount.?properties.accessTier
            output principalId string? = storageAccount.?identity.principalId
            """);

        result.Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
        result.Template.Should().HaveValueAtPath("$.outputs.id.value", "[tryGet(if(parameters('condition'), resourceInfo('storageAccount'), null()), 'id')]");
        result.Template.Should().HaveValueAtPath("$.outputs.name.value", "[tryGet(if(parameters('condition'), resourceInfo('storageAccount'), null()), 'name')]");
        result.Template.Should().HaveValueAtPath("$.outputs.type.value", "[tryGet(if(parameters('condition'), resourceInfo('storageAccount'), null()), 'type')]");
        result.Template.Should().HaveValueAtPath("$.outputs.apiVersion.value", "[tryGet(if(parameters('condition'), resourceInfo('storageAccount'), null()), 'apiVersion')]");
        result.Template.Should().HaveValueAtPath("$.outputs.properties.value", "[tryGet(if(parameters('condition'), reference('storageAccount', '2021-02-01', 'full'), null()), 'properties')]");
        result.Template.Should().HaveValueAtPath("$.outputs.accessTier.value", "[tryGet(if(parameters('condition'), reference('storageAccount', '2021-02-01', 'full'), null()), 'properties', 'accessTier')]");
        result.Template.Should().HaveValueAtPath("$.outputs.principalId.value", "[tryGet(if(parameters('condition'), reference('storageAccount', '2021-02-01', 'full'), null()), 'identity', 'principalId')]");
    }

    [TestMethod]
    public void Safe_dereference_guards_against_dereferencing_properties_from_disabled_child_resources()
    {
        var result = CompilationHelper.Compile("""
            param condition bool
            param condition2 bool

            resource storageAccount 'Microsoft.Storage/storageAccounts@2021-02-01' existing = if (condition) {
              name: 'storageacct'

              resource bs 'blobServices' = {
                name: 'default'

                resource container 'containers' = if (condition2) {
                  name: 'container'
                }
              }
            }

            output id string? = storageAccount::bs::container.?id
            output name string? = storageAccount::bs::container.?name
            output type string? = storageAccount::bs::container.?type
            output apiVersion string? = storageAccount::bs::container.?apiVersion
            output properties object? = storageAccount::bs::container.?properties
            output defaultEncryptionScope string? = storageAccount::bs::container.?properties.defaultEncryptionScope
            """);

        result.Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
        result.Template.Should().HaveValueAtPath("$.outputs.id.value", "[if(and(parameters('condition'), parameters('condition2')), resourceId('Microsoft.Storage/storageAccounts/blobServices/containers', 'storageacct', 'default', 'container'), null())]");
        result.Template.Should().HaveValueAtPath("$.outputs.name.value", "[if(and(parameters('condition'), parameters('condition2')), 'container', null())]");
        result.Template.Should().HaveValueAtPath("$.outputs.type.value", "Microsoft.Storage/storageAccounts/blobServices/containers");
        result.Template.Should().HaveValueAtPath("$.outputs.apiVersion.value", "2021-02-01");
        result.Template.Should().HaveValueAtPath("$.outputs.properties.value", "[tryGet(if(and(parameters('condition'), parameters('condition2')), reference('storageAccount::bs::container', '2021-02-01', 'full'), null()), 'properties')]");
        result.Template.Should().HaveValueAtPath("$.outputs.defaultEncryptionScope.value", "[tryGet(if(and(parameters('condition'), parameters('condition2')), reference('storageAccount::bs::container', '2021-02-01', 'full'), null()), 'properties', 'defaultEncryptionScope')]");
    }

    [TestMethod]
    public void Safe_dereference_guards_against_dereferencing_properties_from_disabled_child_resources_with_resourceInfo()
    {
        var result = CompilationHelper.Compile(new ServiceBuilder().WithFeatureOverrides(new(ResourceInfoCodegenEnabled: true)), """
            param condition bool
            param condition2 bool
            
            resource storageAccount 'Microsoft.Storage/storageAccounts@2021-02-01' existing = if (condition) {
              name: 'storageacct'
            
              resource bs 'blobServices' = {
                name: 'default'
            
                resource container 'containers' = if (condition2) {
                  name: 'container'
                }
              }
            }
            
            output id string? = storageAccount::bs::container.?id
            output name string? = storageAccount::bs::container.?name
            output type string? = storageAccount::bs::container.?type
            output apiVersion string? = storageAccount::bs::container.?apiVersion
            output properties object? = storageAccount::bs::container.?properties
            output defaultEncryptionScope string? = storageAccount::bs::container.?properties.defaultEncryptionScope
            """);

        result.Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
        result.Template.Should().HaveValueAtPath("$.outputs.id.value", "[tryGet(if(and(parameters('condition'), parameters('condition2')), resourceInfo('storageAccount::bs::container'), null()), 'id')]");
        result.Template.Should().HaveValueAtPath("$.outputs.name.value", "[if(and(parameters('condition'), parameters('condition2')), last(split(resourceInfo('storageAccount::bs::container').name, '/')), null())]");
        result.Template.Should().HaveValueAtPath("$.outputs.type.value", "[tryGet(if(and(parameters('condition'), parameters('condition2')), resourceInfo('storageAccount::bs::container'), null()), 'type')]");
        result.Template.Should().HaveValueAtPath("$.outputs.apiVersion.value", "[tryGet(if(and(parameters('condition'), parameters('condition2')), resourceInfo('storageAccount::bs::container'), null()), 'apiVersion')]");
        result.Template.Should().HaveValueAtPath("$.outputs.properties.value", "[tryGet(if(and(parameters('condition'), parameters('condition2')), reference('storageAccount::bs::container', '2021-02-01', 'full'), null()), 'properties')]");
        result.Template.Should().HaveValueAtPath("$.outputs.defaultEncryptionScope.value", "[tryGet(if(and(parameters('condition'), parameters('condition2')), reference('storageAccount::bs::container', '2021-02-01', 'full'), null()), 'properties', 'defaultEncryptionScope')]");
    }

    [TestMethod]
    public void Safe_dereference_guards_against_dereferencing_properties_from_disabled_modules()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                param condition bool

                module mod 'mod.bicep' = if (condition) {
                  name: 'mod'
                }

                module mod2 'mod2.bicep' = if (!condition) {
                  name: 'mod2'
                }

                output name string? = mod.?name
                output outputs object? = mod.?outputs
                output outputs2 object? = mod2.?outputs
                output foo string? = mod.?outputs.foo
                output foo2 string? = mod2.?outputs.foo
                output nullableFoo string? = mod.?outputs.?foo
                output nullableFoo2 string? = mod2.?outputs.?foo
                output bar string? = mod.?outputs.bar
            
                """),
            ("mod.bicep", """
                output foo string = 'foo'

                @secure()
                output bar string = 'bar'
                """),
            ("mod2.bicep", """
                output foo string = 'foo'
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
        result.Template.Should().HaveValueAtPath("$.outputs.name.value", "[if(parameters('condition'), 'mod', null())]");
        result.Template.Should().HaveValueAtPath("$.outputs.outputs.value", "[if(parameters('condition'), listOutputsWithSecureValues('mod', '2022-09-01'), null())]");
        result.Template.Should().HaveValueAtPath("$.outputs.outputs2.value", "[tryGet(if(not(parameters('condition')), reference('mod2'), null()), 'outputs')]");
        result.Template.Should().HaveValueAtPath("$.outputs.foo.value", "[tryGet(if(parameters('condition'), listOutputsWithSecureValues('mod', '2022-09-01'), null()), 'foo')]");
        result.Template.Should().HaveValueAtPath("$.outputs.foo2.value", "[tryGet(if(not(parameters('condition')), reference('mod2'), null()), 'outputs', 'foo', 'value')]");
        result.Template.Should().HaveValueAtPath("$.outputs.nullableFoo.value", "[tryGet(if(parameters('condition'), listOutputsWithSecureValues('mod', '2022-09-01'), null()), 'foo')]");
        result.Template.Should().HaveValueAtPath("$.outputs.nullableFoo2.value", "[tryGet(tryGet(tryGet(if(not(parameters('condition')), reference('mod2'), null()), 'outputs'), 'foo'), 'value')]");
        result.Template.Should().HaveValueAtPath("$.outputs.bar.value", "[tryGet(if(parameters('condition'), listOutputsWithSecureValues('mod', '2022-09-01'), null()), 'bar')]");
    }

    [TestMethod]
    public void Possibly_disabled_resources_should_raise_no_diagnostics_when_used_as_parent_or_dependency()
    {
        var result = CompilationHelper.Compile(
            ("empty.bicep", string.Empty),
            ("main.bicep", """
                param condition bool

                resource acct 'Microsoft.Storage/storageAccounts@2021-02-01' = if (condition) {
                  name: 'acct'
                  location: resourceGroup().location
                  kind: 'StorageV2'
                  sku: {
                    name: 'Standard_LRS'
                  }
                }

                resource bs 'Microsoft.Storage/storageAccounts/blobServices@2023-05-01' = {
                  parent: acct
                  name: 'default'
                }

                module empty 'empty.bicep' = { 
                  dependsOn: [
                    acct
                  ]
                }
                """));

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void References_to_syntactic_ancestor_in_nested_resources_should_allow_regular_dereference_without_diagnostics()
    {
        var result = CompilationHelper.Compile("""
            param createParent bool
            param createChild bool
            param createGrandchild bool

            resource conditionParent 'Microsoft.Storage/storageAccounts@2021-02-01' = if (createParent) {
              name: 'parent'
              location: resourceGroup().location
              kind: 'StorageV2'
              sku: {
                name: 'Standard_LRS'
              }

              resource conditionChild 'blobServices' = if (createChild) {
                name: 'default'

                resource conditionGrandchild 'containers' = if (createGrandchild) {
                  name: 'container'
                  properties: {
                    publicAccess: conditionParent.properties.allowBlobPublicAccess ? 'Blob' : 'None'
                    metadata: {
                      versioned: conditionChild.properties.isVersioningEnabled ? 'On' : 'Off' 
                    }
                  }
                }
              }
            }
            """);

        result.Should().NotHaveAnyDiagnostics();
    }
}
