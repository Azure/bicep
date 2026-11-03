// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

// Reduced forwarding boundaries from https://github.com/Azure/bicep-registry-modules/pull/7472.
[TestClass]
public class NoSecureValuesInInsecureInputsRuleTests : LinterRuleTestsBase
{
    [TestMethod]
    public void Avm_cache_child_requires_secure_parameter()
    {
        const string parent = """
            @secure()
            param value string
            module child 'child.bicep' = {
              name: 'cache'
              params: {
                connectionString: value
              }
            }
            """;
        AssertFlow(parent, "param connectionString string", 0, 1);
        AssertFlow(parent, "@secure()\nparam connectionString string", 0, 0);
    }

    [TestMethod]
    public void Avm_portal_union_only_checks_properties_present_in_each_variant()
    {
        const string parent = """
            @discriminator('name')
            type setting = signIn | delegation
            type signIn = {
              name: 'signin'
              properties: {
                enabled: bool
              }
            }
            type delegation = {
              name: 'delegation'
              properties: {
                @secure()
                validationKey: string?
                url: string?
              }
            }
            param settings setting[]?
            module children 'child.bicep' = [for (setting, index) in (settings ?? []): {
              name: 'portal-${index}'
              params: {
                properties: setting.properties
              }
            }]
            """;
        AssertFlow(parent, "param properties object", 0, 1);
        AssertFlow(parent, """
            param properties {
              enabled: bool?
              @secure()
              validationKey: string?
              url: string?
            }
            """, 0, 0);
    }

    [TestMethod]
    [DataRow("union")]
    [DataRow("concat")]
    public void Avm_sql_array_composition_preserves_secure_member_shape(string function)
    {
        var parent = $$"""
            @secure()
            param secret string
            param enabled bool
            module child 'child.bicep' = {
              name: 'export'
              params: {
                secrets: {{function}}([], enabled ? [{ name: 'admin', value: secret }] : [])
              }
            }
            """;
        AssertFlow(parent, "param secrets array", 0, 1);
        AssertFlow(parent, """
            param secrets {
              name: string
              @secure()
              value: string
            }[]
            """, 0, 0);
    }

    [TestMethod]
    [DataRow(false, "vault.getSecret('value')")]
    [DataRow(true, "vault.getSecret('value')")]
    [DataRow(false, "'${vault.getSecret('value')}'")]
    [DataRow(true, "'${vault.getSecret('value')}'")]
    public void Compiler_rejected_getSecret_does_not_duplicate_leak_warning(bool includeOtherSecret, string expression)
    {
        const string parent = """
            @secure()
            param secret string
            resource vault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
              name: 'vault'
            }
            module child 'child.bicep' = {
              name: 'child'
              params: {
                values: [
                  vault.getSecret('value')
                  secret
                ]
              }
            }
            """;
        var source = (includeOtherSecret ? parent : parent.Replace("      secret", "      'public'")).Replace("vault.getSecret('value')", expression);
        var result = CompilationHelper.Compile(("main.bicep", source), ("child.bicep", "param values array"));
        result.Diagnostics.Where(diagnostic => diagnostic.Code == "BCP180").Should().ContainSingle();
        result.Diagnostics.Where(diagnostic => diagnostic.Code == NoSecureValuesInInsecureInputsRule.Code).Should().HaveCount(includeOtherSecret ? 1 : 0);
    }

    [TestMethod]
    [DataRow("storage.listKeys()", "object")]
    [DataRow("storage.listKeys().keys[0].value", "string")]
    public void Runtime_secret_functions_warn_for_plain_module_parameters(string expression, string type)
        => AssertFlow($$"""
            resource storage 'Microsoft.Storage/storageAccounts@2024-01-01' existing = {
              name: 'storage'
            }
            module child 'child.bicep' = {
              name: 'child'
              params: {
                value: {{expression}}
              }
            }
            """, $"param value {type}", 0, 1);

    [TestMethod]
    public void Interpolation_keeps_security_through_object_properties()
        => AssertFlow("""
            @secure()
            param secret string
            var values = {
              secret: 'prefix-${secret}'
              public: 'public'
            }
            module child 'child.bicep' = {
              name: 'child'
              params: {
                secret: values.secret
                public: values.public
              }
            }
            """, "param secret string\nparam public string", 0, 1);

    [TestMethod]
    public void Discriminated_destination_checks_only_the_selected_variant()
    {
        const string child = """
            @discriminator('kind')
            type configuration = secured | plain
            type secured = {
              kind: 'secured'
              @secure()
              value: string
            }
            type plain = {
              kind: 'plain'
              value: string
            }
            param config configuration
            """;
        const string parent = """
            @secure()
            param secret string
            module child 'child.bicep' = {
              name: 'child'
              params: {
                config: {
                  kind: 'KIND'
                  value: secret
                }
              }
            }
            """;
        AssertFlow(parent.Replace("KIND", "secured"), child, 0, 0);
        AssertFlow(parent.Replace("KIND", "plain"), child, 0, 1);
    }

    private static void AssertFlow(string parent, string child, int inputWarnings, int leakWarnings, string childPath = "child.bicep")
    {
        var result = CompilationHelper.Compile(("main.bicep", parent), (childPath, child));
        result.Diagnostics.Where(diagnostic => diagnostic.Level == DiagnosticLevel.Error).Should().BeEmpty();
        result.Diagnostics.Where(diagnostic => diagnostic.Code == UseSecureValueForSecureInputsRule.Code)
            .Select(diagnostic => diagnostic.Level).Should().Equal(Enumerable.Repeat(DiagnosticLevel.Warning, inputWarnings));
        result.Diagnostics.Where(diagnostic => diagnostic.Code == NoSecureValuesInInsecureInputsRule.Code)
            .Select(diagnostic => diagnostic.Level).Should().Equal(Enumerable.Repeat(DiagnosticLevel.Warning, leakWarnings));
    }
}
