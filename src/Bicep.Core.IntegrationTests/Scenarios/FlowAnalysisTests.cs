// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.Scenarios;

[TestClass]
public class FlowAnalysisTests
{
    [TestMethod]
    public void Issue_20355_matching_resource_conditions_are_safe_but_unguarded_access_is_not()
    {
        var result = CompilationHelper.Compile("""
            param enablePublicIp bool = true

            resource lbIP 'Microsoft.Network/publicIPAddresses@2018-08-01' = if (enablePublicIp) {
              name: 'lbip'
              location: 'eastus'
              sku: {
                name: 'Standard'
              }
              properties: {
                dnsSettings: {
                  domainNameLabel: 'dnsName'
                }
                publicIPAllocationMethod: 'Static'
                publicIPAddressVersion: 'IPv4'
              }
            }

            var unguarded = lbIP.properties.dnsSettings.domainNameLabel

            resource lbIP2 'Microsoft.Network/publicIPAddresses@2018-08-01' = if (enablePublicIp) {
              name: 'lbip2'
              location: 'eastus'
              sku: {
                name: 'Standard'
              }
              properties: {
                dnsSettings: {
                  domainNameLabel: '${lbIP.properties.dnsSettings.domainNameLabel}2'
                }
                publicIPAllocationMethod: 'Static'
                publicIPAddressVersion: 'IPv4'
              }
            }
            """);

        result.Template.Should().NotBeNull();
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP318", DiagnosticLevel.Warning, "The value of type \"Microsoft.Network/publicIPAddresses | null\" may be null at the start of the deployment, which would cause this access expression (and the overall deployment with it) to fail."),
        ]);
        var warning = result.ExcludingLinterDiagnostics().Diagnostics.Single();
        warning.Span.Position.Should().Be(result.SourceFile.Text.IndexOf("lbIP.properties", StringComparison.Ordinal) + "lbIP".Length);
    }

    [TestMethod]
    [DataRow("enabled", "enabled", 0)]
    [DataRow("((enabled))", "(enabled)", 0)]
    [DataRow("!enabled", "!enabled", 0)]
    [DataRow("!(!enabled)", "enabled", 0)]
    [DataRow("enabled", "other", 1)]
    [DataRow("enabled", "!enabled", 1)]
    [DataRow("enabled", "alias", 1)]
    [DataRow("enabled", "enabled && other", 0)]
    [DataRow("enabled", "enabled || other", 1)]
    [DataRow("enabled || other", "enabled", 0)]
    [DataRow("enabled && other", "enabled && other", 0)]
    [DataRow("enabled && other", "enabled", 1)]
    [DataRow("mode == 'prod'", "mode == 'prod'", 0)]
    [DataRow("mode == 'prod'", "!(mode != 'prod')", 0)]
    [DataRow("mode == 'prod'", "'prod' == mode", 0)]
    [DataRow("mode == 'prod'", "mode != 'dev'", 1)]
    [DataRow("enabled == true", "enabled", 0)]
    [DataRow("enabled == false", "!enabled", 0)]
    [DataRow("count == -1", "-1 == count", 0)]
    [DataRow("count == -1", "count == 1", 1)]
    [DataRow("count == 0", "count == -0", 0)]
    [DataRow("mode == 'true'", "enabled == true", 1)]
    [DataRow("enabled == true", "enabled != false", 0)]
    [DataRow("enabled != true", "!enabled", 0)]
    [DataRow("!(enabled == false)", "enabled", 0)]
    [DataRow("config.value != null", "config.value != null", 0)]
    [DataRow("config.value != null", "config.?value != null", 0)]
    [DataRow("config != null", "config.?value != null", 0)]
    [DataRow("config.value == null", "config.value != null", 1)]
    public void Resource_conditions_match_only_supported_symbol_predicates(string resourceCondition, string guard, int warnings)
    {
        var result = CompilationHelper.Compile($$"""
            param enabled bool
            param other bool
            param mode string
            param count int
            param config { value: string? }
            var alias = enabled
            resource storage 'Microsoft.Storage/storageAccounts@2021-04-01' = if ({{resourceCondition}}) {
              name: 'storage'
              location: 'eastus'
              kind: 'StorageV2'
              sku: { name: 'Standard_LRS' }
            }
            output location string = {{guard}} ? storage.location : ''
            """);

        AssertNullableAccessWarnings(result, warnings);
    }

    [TestMethod]
    [DataRow("enabled == true", "enabled != false", 1)]
    [DataRow("enabled == false", "enabled != true", 1)]
    [DataRow("enabled == true", "!(enabled == false)", 1)]
    [DataRow("enabled == false", "!(enabled == true)", 1)]
    [DataRow("enabled == true", "enabled == true", 0)]
    [DataRow("enabled == false", "enabled == false", 0)]
    [DataRow("enabled != true", "enabled != true", 0)]
    [DataRow("enabled != false", "enabled != false", 0)]
    [DataRow("enabled == true", "!(enabled != true)", 0)]
    [DataRow("enabled == false", "!(enabled != false)", 0)]
    [DataRow("enabled == true", "enabled == null", 1)]
    [DataRow("enabled == false", "enabled == null", 1)]
    public void Nullable_boolean_comparisons_do_not_assume_two_valued_logic(string resourceCondition, string guard, int warnings)
    {
        var result = CompilationHelper.Compile($$"""
            param enabled bool?
            resource storage 'Microsoft.Storage/storageAccounts@2021-04-01' = if ({{resourceCondition}}) {
              name: 'storage'
              location: 'eastus'
              kind: 'StorageV2'
              sku: { name: 'Standard_LRS' }
            }
            output location string = {{guard}} ? storage.location : ''
            """);

        AssertNullableAccessWarnings(result, warnings);
        result.Template.Should().NotBeNull();
    }

    [TestMethod]
    [DataRow(false, "input", 1)]
    [DataRow(true, "input", 1)]
    [DataRow(false, "input ?? false", 0)]
    [DataRow(true, "input ?? false", 0)]
    [DataRow(false, "any(input)", 1)]
    [DataRow(true, "any(input)", 1)]
    public void Boolean_comparison_predicates_are_independent_of_type_query_order(bool reverse, string initializer, int warnings)
    {
        var source = $$"""
            param input bool?
            var enabled = {{initializer}}
            resource storage 'Microsoft.Storage/storageAccounts@2021-04-01' = if (enabled == true) {
              name: 'storage'
              location: 'eastus'
              kind: 'StorageV2'
              sku: { name: 'Standard_LRS' }
            }
            output location string = enabled != false ? storage.location : ''
            """;
        var compilation = new ServiceBuilder().WithFeatureOverrides(BicepTestConstants.FeatureOverrides).BuildCompilation(source);
        var model = compilation.GetEntrypointSemanticModel();
        var accesses = SyntaxAggregator.AggregateByType<VariableAccessSyntax>(model.SourceFile.ProgramSyntax).ToArray();

        foreach (var access in reverse ? accesses.Reverse() : accesses)
        {
            model.GetTypeInfo(access);
        }

        var result = CompilationHelper.GetCompilationResult(compilation);
        AssertNullableAccessWarnings(result, warnings);
        result.Template.Should().NotBeNull();
        result.Template.Should().DeepEqual(CompilationHelper.Compile(source).Template!);
    }

    [TestMethod]
    [DataRow("var first = second == true ? second : false\nvar second = first", "BCP080")]
    [DataRow("var first = first == true ? first : false", "BCP079")]
    public void Boolean_comparison_guards_preserve_cycle_diagnostics(string source, string code)
    {
        var result = CompilationHelper.Compile(source);

        result.Template.Should().BeNull();
        result.Diagnostics.Should().Contain(d => d.Code == code && d.Level == DiagnosticLevel.Error);
    }

    [TestMethod]
    public void Boolean_comparison_type_queries_support_loop_and_lambda_locals()
    {
        var result = CompilationHelper.Compile("""
            param input string?
            output loop array = [for enabled in [true, false]: enabled == true && input != null ? length(input) : 0]
            output lambda array = map([true, false], enabled => enabled == true && input != null ? length(input) : 0)
            output reduction bool = reduce([true, false], true, (acc, enabled) => acc == true && enabled == true)
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
    }

    [TestMethod]
    public void Logical_flow_proves_only_the_implications_of_and_and_or()
    {
        var result = CompilationHelper.Compile("""
            param enabled bool
            param other bool
            resource storage 'Microsoft.Storage/storageAccounts@2021-04-01' = if (enabled) {
              name: 'storage'
              location: 'eastus'
              kind: 'StorageV2'
              sku: { name: 'Standard_LRS' }
            }
            resource compound 'Microsoft.Storage/storageAccounts@2021-04-01' = if (enabled && other) {
              name: 'compound'
              location: 'eastus'
              kind: 'StorageV2'
              sku: { name: 'Standard_LRS' }
            }
            output andRight bool = enabled && !empty(storage.location)
            output orRight bool = !enabled || !empty(storage.location)
            output andBranch string = (enabled && other) ? storage.location : ''
            output reorderedAndBranch string = (other && enabled) ? storage.location : ''
            output exactCompound string = (enabled && other) ? compound.location : ''
            output disjunctionBranch string = (enabled || other) ? storage.location : ''
            output falseConjunctionBranch string = (enabled && other) ? '' : storage.location
            output incompleteCompound string = enabled ? compound.location : ''
            """);

        AssertNullableAccessWarnings(result, 3);
    }

    [TestMethod]
    public void Resource_flow_applies_to_indexed_properties_safe_access_and_instance_functions()
    {
        const string source = """
            param enabled bool
            resource storage 'Microsoft.Storage/storageAccounts@2021-04-01' = if (enabled) {
              name: 'storage'
              location: 'eastus'
              kind: 'StorageV2'
              sku: { name: 'Standard_LRS' }
            }
            output indexed string = enabled ? storage['location'] : ''
            output safe string = enabled ? storage.?location : ''
            output method object = enabled ? storage.listKeys() : {}
            output unsafeMethod object = storage.listKeys()
            """;
        var result = CompilationHelper.Compile(source);
        var control = CompilationHelper.Compile(source.Replace("storage[", "storage![", StringComparison.Ordinal)
            .Replace("storage.", "storage!.", StringComparison.Ordinal));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP422", DiagnosticLevel.Warning, "A resource of type \"Microsoft.Storage/storageAccounts | null\" may or may not exist when this function is called, which could cause the deployment to fail."),
        ]);
        result.Template.Should().NotBeNull();
        control.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        control.Template.Should().NotBeNull();
        result.Template.Should().DeepEqual(control.Template!);
    }

    [TestMethod]
    public void Module_conditions_apply_to_bodies_and_ternaries_without_leaking_to_other_declarations()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                param enabled bool
                param include bool
                module source './child.bicep' = if (enabled) {
                  name: 'source'
                  params: { value: 'value' }
                }
                module consumer './child.bicep' = if (enabled && include) {
                  name: 'consumer'
                  params: { value: enabled && include ? source.outputs.result : '' }
                }
                output guarded string = enabled && include ? source.outputs.result : ''
                output unguarded string = source.outputs.result
                """),
            ("child.bicep", """
                param value string
                output result string = value
                """));

        AssertNullableAccessWarnings(result, 1);
    }

    [TestMethod]
    public void Nested_resources_require_both_parent_and_child_conditions()
    {
        var result = CompilationHelper.Compile("""
            param parentEnabled bool
            param childEnabled bool
            resource storage 'Microsoft.Storage/storageAccounts@2021-04-01' = if (parentEnabled) {
              name: 'storage'
              location: 'eastus'
              kind: 'StorageV2'
              sku: { name: 'Standard_LRS' }
              resource service 'blobServices' = if (childEnabled) {
                name: 'default'
              }
            }
            output safe object = parentEnabled ? (childEnabled ? storage::service.properties : {}) : {}
            output unsafe object = childEnabled ? storage::service.properties : {}
            """);

        AssertNullableAccessWarnings(result, 1);
    }

    [TestMethod]
    public void A_matching_deployment_condition_does_not_prove_a_nullable_existing_resource_exists()
    {
        var result = CompilationHelper.Compile("""
            param enabled bool
            @nullIfNotFound()
            resource storage 'Microsoft.Storage/storageAccounts@2021-04-01' existing = if (enabled) {
              name: 'storage'
            }
            output location string = enabled ? storage.location : ''
            """);

        AssertNullableAccessWarnings(result, 1);
    }

    [TestMethod]
    public void Matching_collection_filters_do_not_prove_an_indexed_resource_exists()
    {
        var result = CompilationHelper.Compile("""
            param enabled bool
            resource storage 'Microsoft.Storage/storageAccounts@2021-04-01' = [for i in range(0, 2): if (enabled) {
              name: 'storage${i}'
              location: 'eastus'
              kind: 'StorageV2'
              sku: { name: 'Standard_LRS' }
            }]
            output location string = enabled ? storage[0].location : ''
            """);

        AssertNullableAccessWarnings(result, 1);
    }

    [TestMethod]
    public void A_shadowed_boolean_does_not_prove_a_resource_condition()
    {
        var result = CompilationHelper.Compile("""
            param enabled bool
            resource storage 'Microsoft.Storage/storageAccounts@2021-04-01' = if (enabled) {
              name: 'storage'
              location: 'eastus'
              kind: 'StorageV2'
              sku: { name: 'Standard_LRS' }
            }
            output locations array = map([true], enabled => enabled ? storage.location : '')
            """);

        AssertNullableAccessWarnings(result, 1);
    }

    [TestMethod]
    [DataRow("input != null", "input", "''")]
    [DataRow("input == null", "''", "input")]
    [DataRow("null != ((input))", "input", "''")]
    [DataRow("null == input", "''", "input")]
    [DataRow("!(input == null)", "input", "''")]
    [DataRow("!((input != null))", "''", "input")]
    public void Explicit_null_checks_narrow_parameter_references(string condition, string trueBranch, string falseBranch)
    {
        var result = CompilationHelper.Compile($$"""
            param input string?
            output result string = {{condition}} ? {{trueBranch}} : {{falseBranch}}
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
    }

    [TestMethod]
    public void Null_checks_narrow_variables_for_properties_arguments_and_indices()
    {
        var result = CompilationHelper.Compile("""
            param objects object[]
            param indices int[]
            var objectValue = first(objects)
            var index = first(indices)
            output property string = objectValue != null ? string(objectValue.value) : ''
            output argument string = objectValue == null ? '' : string(length(objectValue))
            output item int = index != null ? indices[index] : 0
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
    }

    [TestMethod]
    public void Null_checks_flow_into_short_circuited_and_or_operands()
    {
        var result = CompilationHelper.Compile("""
            param name string?
            output hasName bool = name != null && length(name) > 0
            output emptyOrNull bool = name == null || length(name) == 0
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
    }

    [TestMethod]
    [DataRow("int", "int", "0", "input + 1")]
    [DataRow("int", "int", "0", "-input")]
    [DataRow("int", "bool", "false", "input > 60")]
    [DataRow("bool", "bool", "false", "!input")]
    public void Null_guarded_function_operands_need_no_assertions_and_preserve_emission(
        string inputType, string outputType, string defaultValue, string expression)
    {
        string Source(string body) => $$"""
            func evaluate(input {{inputType}}?) {{outputType}} => input == null ? {{defaultValue}} : {{body}}
            """;
        var result = CompilationHelper.Compile(Source(expression));
        var control = CompilationHelper.Compile(Source(expression.Replace("input", "input!", StringComparison.Ordinal)));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
        control.Template.Should().NotBeNull();
        result.Template.Should().DeepEqual(control.Template!);
    }

    [TestMethod]
    public void Null_checks_do_not_narrow_aliases_or_unrelated_parameters()
    {
        var result = CompilationHelper.Compile("""
            param input object?
            param other object?
            var alias = input
            output aliases object = input != null ? alias.value : {}
            output unrelated object = input != null ? other.value : {}
            """);

        AssertNullableAccessWarnings(result, 2);
    }

    [TestMethod]
    [DataRow("config.value != null", "config.value", "''")]
    [DataRow("config.value == null", "''", "config.value")]
    [DataRow("null != ((config.value))", "(config).value", "''")]
    [DataRow("!(null == config.value)", "config.value", "''")]
    [DataRow("config['value'] != null", "config.value", "''")]
    [DataRow("config.value != null", "config['value']", "''")]
    [DataRow("config[('value')] != null", "config.value", "''")]
    [DataRow("config.nested.value != null", "config['nested']['value']", "''")]
    [DataRow("config['nested']['value'] == null", "''", "(config.nested).value")]
    [DataRow("config['a\\'b'] != null", "config['a\\'b']", "''")]
    [DataRow("config['a.b'] != null", "config['a.b']", "''")]
    public void Repeated_nullable_property_paths_are_narrowed(string condition, string trueBranch, string falseBranch)
    {
        var result = CompilationHelper.Compile($$"""
            param config {
              value: string?
              nested: { value: string? }
              'a\'b': string?
              'a.b': string?
            }
            output result string = {{condition}} ? {{trueBranch}} : {{falseBranch}}
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
    }

    [TestMethod]
    public void Property_path_facts_apply_to_chained_accesses_arguments_indices_and_logical_operands()
    {
        const string source = """
            param config { value: { count: int }?, name: string?, index: int? }
            param items int[]
            output count int = config.value != null ? config.value.count : 0
            output indexedCount int = config['value'] == null ? 0 : config['value']['count']
            output length int = config.name != null ? length(config.name) : 0
            output item int = config.index != null ? items[config.index] : 0
            output andResult bool = config.name != null && length(config.name) > 0
            output orResult bool = config.name == null || length(config.name) == 0
            output compound string = config.name != null && config.index != null ? config.name : ''
            """;
        var result = CompilationHelper.Compile(source);
        var control = CompilationHelper.Compile(source
            .Replace("config.value.count", "config.value!.count", StringComparison.Ordinal)
            .Replace("config['value']['count']", "config['value']!['count']", StringComparison.Ordinal)
            .Replace("length(config.name)", "length(config.name!)", StringComparison.Ordinal)
            .Replace("items[config.index]", "items[config.index!]", StringComparison.Ordinal)
            .Replace("? config.name :", "? config.name! :", StringComparison.Ordinal));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
        control.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().DeepEqual(control.Template!);
    }

    [TestMethod]
    public void Property_path_facts_apply_to_conditional_resource_and_module_bodies()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                param config { name: string? }
                resource storage 'Microsoft.Storage/storageAccounts@2021-04-01' = if (config.name != null) {
                  name: config.name
                  location: 'eastus'
                  kind: 'StorageV2'
                  sku: { name: 'Standard_LRS' }
                }
                module child './child.bicep' = if (config.name != null) {
                  name: 'child'
                  params: { name: config.name }
                }
                """),
            ("child.bicep", """
                param name string
                output result string = name
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
    }

    [TestMethod]
    public void Property_path_guards_prove_matching_resource_conditions()
    {
        var result = CompilationHelper.Compile("""
            param config { name: string? }
            resource storage 'Microsoft.Storage/storageAccounts@2021-04-01' existing = if (config.name != null) {
              name: config.name
            }
            output location string = config.name != null ? storage.location : ''
            """);

        AssertNullableAccessWarnings(result, 0);
    }

    // https://github.com/Azure/bicep/issues/19641
    [TestMethod]
    public void Null_checked_parameter_is_non_null_in_existing_resource_body()
    {
        var result = CompilationHelper.Compile("""
            param vnet { name: string, resourceGroupName: string }?
            resource vnetResource 'Microsoft.Network/virtualNetworks@2024-01-01' existing = if (vnet != null) {
              name: vnet.name
              scope: resourceGroup(vnet.resourceGroupName)
            }
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Property_path_facts_do_not_leak_to_aliases_other_paths_or_sibling_branches()
    {
        var result = CompilationHelper.Compile("""
            param config { value: { count: int }?, other: { count: int }? }
            param other { value: { count: int }? }
            var alias = config
            output aliases int = config.value != null ? alias.value.count : 0
            output otherRoot int = config.value != null ? other.value.count : 0
            output otherPath int = config.value != null ? config.other.count : 0
            output falseBranch int = config.value != null ? 0 : config.value.count
            output unguarded int = config.value.count
            output uncertain int = config.value != null || other.value != null ? config.value.count : 0
            """);

        AssertNullableAccessWarnings(result, 6);
    }

    [TestMethod]
    [DataRow("config[key] != null", "config[key].count")]
    [DataRow("items[0] != null", "items[0].count")]
    [DataRow("items[^1] != null", "items[^1].count")]
    [DataRow("first([config]).value != null", "first([config]).value.count")]
    public void Unsupported_path_guards_retain_nullable_access_warnings(string guard, string access)
    {
        var result = CompilationHelper.Compile($$"""
            param config { value: { count: int }? }
            param key 'value'
            param items ({ count: int }?)[]
            output result int = {{guard}} ? {{access}} : 0
            """);

        AssertNullableAccessWarnings(result, 1);
    }

    [TestMethod]
    public void Successful_safe_access_null_check_proves_parent_and_property_non_null()
    {
        var result = CompilationHelper.Compile("""
            param config { value: { name: string }? }?
            output name string = config.?value != null
              ? config.value.name
              : ''
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
    }

    [TestMethod]
    public void Null_safe_access_branch_does_not_assume_parent_or_property_exists()
    {
        var result = CompilationHelper.Compile("""
            param config { value: { name: string }? }?
            output name string = config.?value == null
              ? config.value.name
              : ''
            """);

        AssertNullableAccessWarnings(result, 2);
    }

    [TestMethod]
    public void Child_path_checks_do_not_remove_nullable_parent_diagnostics()
    {
        var result = CompilationHelper.Compile("""
            param config { value: { count: int }? }?
            output count int = config.value != null ? config.value.count : 0
            """);

        AssertNullableAccessWarnings(result, 2);
    }

    [TestMethod]
    public void Parent_and_child_checks_narrow_each_path_without_erasing_other_nullability()
    {
        var result = CompilationHelper.Compile("""
            param config { value: { count: int, nested: { count: int }? }? }?
            output count int = config != null && config.value != null ? config.value.count : 0
            output safeCount int? = config != null && config.value != null ? config.value.nested.?count : null
            output guardedSafeCount int? = config != null && config.value != null ? config.value.?count : null
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
        var model = result.Compilation.GetEntrypointSemanticModel();
        var outputs = model.Root.OutputDeclarations.ToDictionary(o => o.Name);
        var safeCount = (TernaryOperationSyntax)outputs["safeCount"].DeclaringOutput.Value;
        var guardedSafeCount = (TernaryOperationSyntax)outputs["guardedSafeCount"].DeclaringOutput.Value;
        model.GetTypeInfo(safeCount.TrueExpression).Name.Should().Be("int | null");
        model.GetTypeInfo(guardedSafeCount.TrueExpression).Name.Should().Be("int");
    }

    [TestMethod]
    public void Property_path_guards_do_not_hide_cyclic_reference_errors()
    {
        var result = CompilationHelper.Compile("""
            var first = second.value != null ? second.value : {}
            var second = first
            """);

        result.Template.Should().BeNull();
        result.Diagnostics.Should().Contain(d => d.Code == "BCP080" && d.Level == DiagnosticLevel.Error);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Property_path_types_are_independent_of_query_order(bool reverse)
    {
        const string source = """
            param config { value: { count: int }? }
            var result = config.value != null ? config.value.count : 0
            """;
        var uri = new Uri("file:///main.bicep");
        var compilation = new ServiceBuilder().WithFeatureOverrides(BicepTestConstants.FeatureOverrides)
            .BuildCompilation(new Dictionary<Uri, string> { [uri] = source }, uri);
        var model = compilation.GetEntrypointSemanticModel();
        var accesses = SyntaxAggregator.AggregateByType<PropertyAccessSyntax>(
            compilation.SourceFileGrouping.EntryPoint.ProgramSyntax).ToArray();

        foreach (var access in reverse ? accesses.Reverse() : accesses)
        {
            model.GetTypeInfo(access);
        }

        var guarded = accesses.Single(a => a.PropertyName.IdentifierName == "count");
        model.GetTypeInfo(guarded.BaseExpression).Name.Should().NotContain("null");
        model.GetDeclaredType(guarded.BaseExpression)!.Name.Should().Contain("null");
        var result = CompilationHelper.GetCompilationResult(compilation);
        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().DeepEqual(CompilationHelper.Compile(source).Template!);
    }

    [TestMethod]
    public void Optional_properties_and_local_variable_paths_can_be_narrowed()
    {
        var result = CompilationHelper.Compile("""
            param config { value: string? } = {}
            output optional string = config.value != null ? config.value : ''
            func lengthOrZero(arg { value: string? }) int => arg.value == null ? 0 : length(arg.value)
            output local int = lengthOrZero({ value: null })
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
    }

    [TestMethod]
    public void Guarding_a_reference_does_not_guard_the_referenced_variables_initializer()
    {
        var result = CompilationHelper.Compile("""
            param input object?
            var value = input.value
            output result object = input != null ? value : {}
            """);

        AssertNullableAccessWarnings(result, 1);
    }

    [TestMethod]
    public void Null_guarded_cyclic_references_still_report_cycle_errors()
    {
        var result = CompilationHelper.Compile("""
            var first = second != null ? second : ''
            var second = first
            """);

        result.Template.Should().BeNull();
        result.Diagnostics.Should().Contain(x => x.Code == "BCP080" && x.Level == DiagnosticLevel.Error);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Type_queries_before_diagnostics_preserve_narrowing_and_emission(bool reverse)
    {
        const string source = """
            param input int?
            param flag bool
            var result = input != null ? (flag ? input + 1 : -input) : 0
            output result int = result
            """;
        var uri = new Uri("file:///main.bicep");
        var compilation = new ServiceBuilder().WithFeatureOverrides(BicepTestConstants.FeatureOverrides)
            .BuildCompilation(new Dictionary<Uri, string> { [uri] = source }, uri);
        var model = compilation.GetEntrypointSemanticModel();
        var accesses = SyntaxAggregator.AggregateByType<VariableAccessSyntax>(
            compilation.SourceFileGrouping.EntryPoint.ProgramSyntax).ToArray();

        foreach (var access in reverse ? accesses.Reverse() : accesses)
        {
            model.GetTypeInfo(access);
        }

        var result = CompilationHelper.GetCompilationResult(compilation);
        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().DeepEqual(CompilationHelper.Compile(source).Template!);
        model.Root.ParameterDeclarations.Single(x => x.Name == "input").Type.Name.Should().Be("int | null");
    }

    private static void AssertNullableAccessWarnings(CompilationHelper.CompilationResult result, int warnings)
    {
        var diagnostics = result.ExcludingLinterDiagnostics().Diagnostics.ToArray();
        diagnostics.Should().HaveCount(warnings);
        diagnostics.Select(x => (x.Code, x.Level)).Should()
            .Equal(Enumerable.Repeat(("BCP318", DiagnosticLevel.Warning), warnings));
        result.Template.Should().NotBeNull();
    }
}
