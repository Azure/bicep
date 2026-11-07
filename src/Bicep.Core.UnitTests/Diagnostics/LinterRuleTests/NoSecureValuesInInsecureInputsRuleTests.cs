// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

// Reduced from the before and after states of https://github.com/Azure/bicep-registry-modules/pull/7472.
// These retain forwarding expressions, not resource setup. Tests marked "Not in AVM" are synthetic edge cases
// around an AVM binding.
[TestClass]
public class NoSecureValuesInInsecureInputsRuleTests : ModuleParameterSecurityTestsBase
{
    [TestMethod]
    [DataRow(8)]
    [DataRow(22)]
    [Timeout(60_000)]
    public void Repeated_variable_reads_do_not_expand_a_diamond_exponentially(int depth)
        => AssertDiamondAnalysisCompletesPromptly(depth, secureInput: false);

    [TestMethod]
    public void Cached_variable_findings_keep_each_destination_path_and_target_type()
        => AssertFlow("""
            @secure()
            param secret string
            var shared = { value: secret }
            module child 'child.bicep' = {
              name: 'child'
              params: {
                settings: {
                  protected: shared
                  first: shared
                  second: shared
                }
              }
            }
            """, """
            type plain = { value: string }
            param settings {
              protected: {
                @secure()
                value: string
              }
              first: plain
              second: plain
            }
            """, [], [
                LeakWarning("settings.first.value", "secret"),
                LeakWarning("settings.second.value", "secret"),
            ]);

    [TestMethod]
    public void Cached_variable_findings_point_to_each_reference()
    {
        var parent = """
            @secure()
            param secret string
            var shared = { value: secret }
            module child 'child.bicep' = {
              name: 'child'
              params: {
                settings: [shared, shared]
              }
            }
            """;
        var result = CompileModule(parent, "param settings { value: string }[]");
        result.Diagnostics.Where(diagnostic => diagnostic.Level == DiagnosticLevel.Error).Should().BeEmpty();
        var warnings = result.Diagnostics.Where(diagnostic => diagnostic.Code == NoSecureValuesInInsecureInputsRule.Code).ToArray();

        warnings.Select(warning => warning.Message).Should().Equal(
            LeakWarning("settings[0].value", "secret"),
            LeakWarning("settings[1].value", "secret"));
        var firstRead = parent.IndexOf("[shared", StringComparison.Ordinal) + 1;
        var secondRead = parent.LastIndexOf("shared", StringComparison.Ordinal);
        warnings.Select(warning => warning.Span.Position).Should().Equal(firstRead, secondRead);
        warnings.Select(warning => warning.Span.Length).Should().OnlyContain(length => length == "shared".Length);
    }

    [TestMethod]
    [DataRow("{ value: secret }", "secret")]
    [DataRow("alias", "alias")]
    [DataRow("union({ value: secret }, {})", "secret")]
    public void Module_warnings_name_the_leaf_and_point_to_its_use(string expression, string anchor)
    {
        var parent = $$"""
            @secure()
            param secret string
            var config = { value: secret }
            var alias = config
            module child 'child.bicep' = {
              name: 'child'
              params: {
                config: {{expression}}
              }
            }
            """;
        var result = CompileModule(parent, "param config { value: string }");
        result.Diagnostics.Where(diagnostic => diagnostic.Level == DiagnosticLevel.Error).Should().BeEmpty();
        var warning = result.Diagnostics.Where(diagnostic => diagnostic.Code == NoSecureValuesInInsecureInputsRule.Code)
            .Should().ContainSingle().Subject;
        warning.Message.Should().Be(LeakWarning("config.value", "secret"));
        warning.Span.Position.Should().Be(parent.LastIndexOf(anchor, StringComparison.Ordinal));
        warning.Span.Length.Should().Be(anchor.Length);
    }

    [TestMethod]
    public void Secure_values_forwarded_through_object_properties_leak_into_plain_module_parameters()
        => AssertFlow("""
            @secure()
            param secret string
            var original = {
              direct: secret
              interpolated: '${secret}'
              public: 'not secret'
            }
            var alias = original
            var chainedAlias = alias
            module child 'child.bicep' = {
              name: 'child'
              params: {
                direct: chainedAlias.direct
                indexed: chainedAlias['direct']
                interpolated: chainedAlias.interpolated
                public: chainedAlias.public
              }
            }
            """, """
            param direct string
            param indexed string
            param interpolated string
            param public string
            """, [], [
                LeakWarning("direct", "chainedAlias.direct"),
                LeakWarning("indexed", "chainedAlias['direct']"),
                LeakWarning("interpolated", "chainedAlias.interpolated"),
            ]);

    [TestMethod]
    [DataRow("chained.number", "int")]
    [DataRow("chained['number']", "int")]
    [DataRow("chained.flag", "bool")]
    [DataRow("chained.text", "string")]
    [DataRow("chained.interpolated", "string")]
    [DataRow("chained.nested.number", "int")]
    [DataRow("union(chained, {})", "object")]
    [DataRow("sys.union(chained, {})", "object")]
    public void Copied_secure_object_members_are_reported_on_the_module_argument(string expression, string type)
    {
        var parent = $$"""
            @secure()
            param secureObj { number: int, flag: bool, text: string }
            var original = {
              number: secureObj.number
              flag: secureObj.flag
              text: secureObj.text
              interpolated: '${secureObj.number}'
              nested: { number: secureObj.number }
              public: 'not secret'
            }
            var alias = original
            var chained = alias
            module child 'child.bicep' = {
              name: 'child'
              params: {
                value: {{expression}}
                public: chained.public
              }
            }
            """;
        var result = CompileModule(parent, $"param value {type}\nparam public string");
        result.Diagnostics.Where(diagnostic => diagnostic.Level == DiagnosticLevel.Error).Should().BeEmpty();
        var warning = result.Diagnostics.Where(diagnostic => diagnostic.Code == NoSecureValuesInInsecureInputsRule.Code)
            .Should().ContainSingle().Subject;
        warning.Span.Position.Should().BeGreaterThanOrEqualTo(parent.LastIndexOf(expression, StringComparison.Ordinal));
        warning.Span.Position.Should().BeLessThan(parent.LastIndexOf(expression, StringComparison.Ordinal) + expression.Length);
    }

    [TestMethod]
    [DataRow("empty(secret)", "bool")]
    [DataRow("empty(secrets)", "bool")]
    [DataRow("empty(secureObj)", "bool")]
    [DataRow("length(secret)", "int")]
    [DataRow("length(secrets)", "int")]
    [DataRow("length(secureObj)", "int")]
    [DataRow("contains(secret, 'x')", "bool")]
    [DataRow("contains(secrets, secret)", "bool")]
    [DataRow("contains(secureObj, 'flag')", "bool")]
    [DataRow("startsWith(secret, 'x')", "bool")]
    [DataRow("endsWith(secret, 'x')", "bool")]
    [DataRow("indexOf(secret, 'x')", "int")]
    [DataRow("indexOf(secrets, secret)", "int")]
    [DataRow("lastIndexOf(secret, 'x')", "int")]
    [DataRow("lastIndexOf(secrets, secret)", "int")]
    [DataRow("uniqueString(secret)", "string")]
    [DataRow("guid(secret)", "string")]
    [DataRow("nameof(secret)", "string")]
    [DataRow("sys.empty(secret)", "bool")]
    [DataRow("sys.length(secret)", "int")]
    [DataRow("sys.contains(secureObj, 'flag')", "bool")]
    [DataRow("sys.uniqueString(secret)", "string")]
    [DataRow("sys.guid(secret)", "string")]
    [DataRow("sys.nameof(secret)", "string")]
    [DataRow("secret == 'x'", "bool")]
    [DataRow("secret != 'x'", "bool")]
    [DataRow("secret =~ 'x'", "bool")]
    [DataRow("secret !~ 'x'", "bool")]
    [DataRow("secret < 'x'", "bool")]
    [DataRow("secret <= 'x'", "bool")]
    [DataRow("secret > 'x'", "bool")]
    [DataRow("secret >= 'x'", "bool")]
    [DataRow("secureObj.flag && enabled", "bool")]
    [DataRow("secureObj.flag || enabled", "bool")]
    [DataRow("!secureObj.flag", "bool")]
    public void Non_secret_results_do_not_warn_directly_or_through_other_expressions(string expression, string type)
        => AssertFlow($$"""
            @secure()
            param secret string
            @secure()
            param secureObj {
              flag: bool
              number: int
            }
            @secure()
            type secretString = string
            param secrets secretString[]
            param enabled bool
            var result = {{expression}}
            var settings = {
              value: result
            }
            module child 'child.bicep' = {
              name: 'child'
              params: {
                direct: {{expression}}
                alias: result
                nested: settings
                text: '${settings.value}'
              }
            }
            """, $$"""
            param direct {{type}}
            param alias {{type}}
            param nested {
              value: {{type}}
            }
            param text string
            """, 0, 0);

    [TestMethod]
    [DataRow("toUpper(secret)", "string")]
    [DataRow("sys.toUpper(secret)", "string")]
    [DataRow("first(secrets)!", "string")]
    [DataRow("secureObj.flag", "bool")]
    [DataRow("secureObj.number + 1", "int")]
    [DataRow("-secureObj.number", "int")]
    [DataRow("secret == 'x' ? secret : 'public'", "string")]
    [DataRow("concat(uniqueString(secret), secret)", "string")]
    public void Expressions_that_still_reveal_secrets_warn(string expression, string type)
        => AssertFlow($$"""
            @secure()
            param secret string
            @secure()
            param secureObj {
              flag: bool
              number: int
            }
            @secure()
            type secretString = string
            param secrets secretString[]
            module child 'child.bicep' = {
              name: 'child'
              params: {
                value: {{expression}}
              }
            }
            """, $"param value {type}", 0, 1);

    [TestMethod]
    [DataRow("split(secret, ',')")]
    [DataRow("items(secureObj)")]
    [DataRow("array(secret)")]
    public void Function_results_that_hide_secrets_leak_as_whole_values(string expression)
        => AssertFlow($$"""
            @secure()
            param secret string
            @secure()
            param secureObj object
            module child 'child.bicep' = {
              name: 'child'
              params: {
                value: {{expression}}
              }
            }
            """, "param value array", 0, 1);

    [TestMethod]
    public void Union_of_a_secure_object_and_an_empty_object_satisfies_a_secure_module_parameter()
        => AssertFlow("""
            @secure()
            param secureObj object
            module child 'child.bicep' = {
              name: 'child'
              params: {
                config: union(secureObj, {})
              }
            }
            """, "@secure()\nparam config object", [], []);

    [TestMethod]
    [DataRow("""
        @discriminator('kind')
        param value { kind: 'a', x: string } | { kind: 'b', y: string }
        """)]
    [DataRow("param value resourceInput<'Microsoft.DataFactory/factories/linkedservices@2018-06-01'>.properties")]
    public void Secure_discriminated_parameters_leak_into_plain_module_parameters(string declaration)
        => AssertFlow($$"""
            @secure()
            {{declaration}}
            module child 'child.bicep' = {
              name: 'child'
              params: {
                config: value
              }
            }
            """, "param config object", [], [LeakWarning("config", "value")]);

    [TestMethod]
    public void Cyclic_property_initializers_do_not_produce_security_warnings()
    {
        var result = CompileModule("""
            var cyclic = { value: cyclic.value }
            module child 'child.bicep' = {
              name: 'child'
              params: {
                value: cyclic.value
              }
            }
            """, "param value string");
        result.Diagnostics.Where(diagnostic => diagnostic.Level == DiagnosticLevel.Error).Should().NotBeEmpty();
        result.Diagnostics.Where(diagnostic => diagnostic.Code is NoSecureValuesInInsecureInputsRule.Code or UseSecureValueForSecureInputsRule.Code)
            .Should().BeEmpty();
    }

    // api-management/service/cache: the parent and the child both gained `@secure()`, so each side is checked
    // against the other. Before the change neither was secure, and that state has to stay clean too.
    [TestMethod]
    public void Avm_cache_requires_securing_parent_and_child_together()
    {
        const string parent = """
            type cacheType = {
              SECURE
              connectionString: string
            }
            param caches cacheType[]?
            module children 'child.bicep' = [for (cache, index) in (caches ?? []): {
              name: 'cache-${index}'
              params: {
                connectionString: cache.connectionString
              }
            }]
            """;
        AssertHardening(parent, "@secure()\nparam connectionString string", "connectionString", ["cache.connectionString"]);
        AssertFlow(parent.Replace("SECURE", ""), "param connectionString string", [], []);
    }

    // api-management/service/portalsetting: the child went from a resource-derived type, which is not secure
    // in the emitted module parameter, to a type with a secure `validationKey`.
    private const string ApimPortalSettings = """
        @discriminator('name')
        type portalSettingsType = signInPropertiesType | signUpPropertiesType | delegationPropertiesType
        type signInPropertiesType = {
          name: 'signin'
          properties: {
            enabled: bool
          }
        }
        type signUpPropertiesType = {
          name: 'signup'
          properties: {
            enabled: bool?
            termsOfService: {
              consentRequired: bool?
              enabled: bool?
              text: string?
            }?
          }
        }
        type delegationPropertiesType = {
          name: 'delegation'
          properties: {
            subscriptions: {
              enabled: bool
            }?
            url: string?
            userRegistration: {
              enabled: bool
            }?
            @secure()
            validationKey: string?
          }
        }
        param portalsettings portalSettingsType[]?
        module children 'child.bicep' = [for (portalsetting, index) in (portalsettings ?? []): {
          name: 'portal-${index}'
          params: {
            properties: portalsetting.properties
          }
        }]
        """;

    private const string PortalSettingPropertiesChild = """
        param properties {
          enabled: bool?
          termsOfService: {
            consentRequired: bool?
            enabled: bool?
            text: string?
          }?
          subscriptions: {
            enabled: bool?
          }?
          url: string?
          userRegistration: {
            enabled: bool?
          }?
          @secure()
          validationKey: string?
        }
        """;

    [TestMethod]
    public void Avm_portal_settings_union_checks_only_properties_present_in_each_variant()
    {
        AssertFlow(ApimPortalSettings, "param properties resourceInput<'Microsoft.ApiManagement/service/portalsettings@2024-05-01'>.properties", [], [LeakWarning("properties.validationKey", "portalsetting.properties")]);
        AssertFlow(ApimPortalSettings, PortalSettingPropertiesChild, [], []);
    }

    // Not in AVM: its sign-in variant has no `validationKey`. Giving it a plain one makes the secure child field
    // warn, because the value for that variant could be plain.
    [TestMethod]
    public void Portal_settings_variant_with_a_plain_validation_key_does_not_satisfy_the_secure_child_field()
        => AssertFlow(ApimPortalSettings.Replace("""
              properties: {
                enabled: bool
              }
            """, """
              properties: {
                enabled: bool
                validationKey: string?
              }
            """), PortalSettingPropertiesChild, [InputWarning("properties.validationKey")], []);

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

    // Not in AVM: its export child declares a secure value member, so a plain `array` child is a leak.
    [TestMethod]
    public void Sql_secrets_export_is_a_secure_source_for_a_plain_child()
        => AssertFlow(SqlSecretsExport.Replace("SECURE", "@secure()"), "param secretsToSet array", [], [LeakWarning("secretsToSet[*].value", "administratorLoginPassword")]);

    // Not in AVM: a resource-derived dictionary forwarded on its own, with nothing secure to infer from.
    [TestMethod]
    public void Flux_resource_derived_dictionary_alone_is_not_a_secure_source()
        => AssertFlow("""
            param settings resourceInput<'Microsoft.KubernetesConfiguration/fluxConfigurations@2025-04-01'>.properties.configurationProtectedSettings?
            module child 'child.bicep' = {
              name: 'child'
              params: {
                settings: settings
              }
            }
            """, "param settings object?", 0, 0);

    private const string VmExtensionChild = """
        param name string
        param typeHandlerVersion string
        param autoUpgradeMinorVersion bool
        param enableAutomaticUpgrade bool
        param settings object?
        @secure()
        param protectedSettings object?
        param supressFailures bool = false
        param tags object?
        """;

    // compute/virtual-machine: the whole `@secure()` object is a secure source for every member forwarded from
    // it, not only the password that is put in `protectedSettings`.
    [TestMethod]
    public void Avm_vm_domain_join_secure_object_marks_every_forwarded_member()
        => AssertFlow("""
            @secure()
            param extensionDomainJoinPassword string = ''
            @secure()
            param extensionDomainJoinConfig object = {}
            param tags object?
            module vm_domainJoinExtension 'child.bicep' = if (contains(extensionDomainJoinConfig, 'enabled') && extensionDomainJoinConfig.enabled) {
              name: 'domain-join'
              params: {
                name: extensionDomainJoinConfig.?name ?? 'DomainJoin'
                typeHandlerVersion: extensionDomainJoinConfig.?typeHandlerVersion ?? '1.3'
                autoUpgradeMinorVersion: extensionDomainJoinConfig.?autoUpgradeMinorVersion ?? true
                enableAutomaticUpgrade: extensionDomainJoinConfig.?enableAutomaticUpgrade ?? false
                settings: extensionDomainJoinConfig.?settings ?? {
                  Name: extensionDomainJoinConfig.?domainName
                  OUPath: extensionDomainJoinConfig.?ouPath
                  User: extensionDomainJoinConfig.?user
                  Restart: extensionDomainJoinConfig.?restart
                  Options: extensionDomainJoinConfig.?options
                }
                supressFailures: extensionDomainJoinConfig.?supressFailures ?? false
                tags: extensionDomainJoinConfig.?tags ?? tags
                protectedSettings: {
                  Password: extensionDomainJoinPassword
                }
              }
            }
            """, VmExtensionChild, [],
            [
                LeakWarning("name", "extensionDomainJoinConfig.?name"),
                LeakWarning("typeHandlerVersion", "extensionDomainJoinConfig.?typeHandlerVersion"),
                LeakWarning("autoUpgradeMinorVersion", "extensionDomainJoinConfig.?autoUpgradeMinorVersion"),
                LeakWarning("enableAutomaticUpgrade", "extensionDomainJoinConfig.?enableAutomaticUpgrade"),
                LeakWarning("settings", "extensionDomainJoinConfig.?settings"),
                LeakWarning("settings.Name", "extensionDomainJoinConfig.?domainName"),
                LeakWarning("settings.OUPath", "extensionDomainJoinConfig.?ouPath"),
                LeakWarning("settings.User", "extensionDomainJoinConfig.?user"),
                LeakWarning("settings.Restart", "extensionDomainJoinConfig.?restart"),
                LeakWarning("settings.Options", "extensionDomainJoinConfig.?options"),
                LeakWarning("supressFailures", "extensionDomainJoinConfig.?supressFailures"),
                LeakWarning("tags", "extensionDomainJoinConfig.?tags"),
            ]);

    // compute/virtual-machine-scale-set
    [TestMethod]
    public void Avm_vmss_domain_join_secure_object_marks_every_forwarded_member()
        => AssertFlow("""
            @secure()
            param extensionDomainJoinPassword string = ''
            @secure()
            param extensionDomainJoinConfig object = {
              enabled: false
            }
            module vmss_domainJoinExtension 'child.bicep' = if (extensionDomainJoinConfig.enabled) {
              name: 'domain-join'
              params: {
                name: 'DomainJoin'
                typeHandlerVersion: extensionDomainJoinConfig.?typeHandlerVersion ?? '1.3'
                autoUpgradeMinorVersion: extensionDomainJoinConfig.?autoUpgradeMinorVersion ?? true
                enableAutomaticUpgrade: extensionDomainJoinConfig.?enableAutomaticUpgrade ?? false
                settings: extensionDomainJoinConfig.settings
                protectedSettings: {
                  Password: extensionDomainJoinPassword
                }
                provisionAfterExtensions: extensionDomainJoinConfig.?provisionAfterExtensions
              }
            }
            """, """
            param name string
            param typeHandlerVersion string
            param autoUpgradeMinorVersion bool
            param enableAutomaticUpgrade bool
            param settings object?
            @secure()
            param protectedSettings object?
            param provisionAfterExtensions string[]?
            """, [],
            [
                LeakWarning("typeHandlerVersion", "extensionDomainJoinConfig.?typeHandlerVersion"),
                LeakWarning("autoUpgradeMinorVersion", "extensionDomainJoinConfig.?autoUpgradeMinorVersion"),
                LeakWarning("enableAutomaticUpgrade", "extensionDomainJoinConfig.?enableAutomaticUpgrade"),
                LeakWarning("settings", "extensionDomainJoinConfig.settings"),
                LeakWarning("provisionAfterExtensions", "extensionDomainJoinConfig.?provisionAfterExtensions"),
            ]);

    // compute/virtual-machine: the host pool registration token is the secret, but the whole object is secure.
    [TestMethod]
    public void Avm_host_pool_registration_secure_object_marks_every_forwarded_member()
        => AssertFlow("""
            @secure()
            param extensionHostPoolRegistration object = {
              enabled: false
            }
            param tags object?
            module vm_hostPoolRegistrationExtension 'child.bicep' = if (extensionHostPoolRegistration.enabled) {
              name: 'host-pool'
              params: {
                name: extensionHostPoolRegistration.?name ?? 'HostPoolRegistration'
                typeHandlerVersion: extensionHostPoolRegistration.?typeHandlerVersion ?? '2.77'
                autoUpgradeMinorVersion: extensionHostPoolRegistration.?autoUpgradeMinorVersion ?? true
                enableAutomaticUpgrade: extensionHostPoolRegistration.?enableAutomaticUpgrade ?? false
                settings: {
                  modulesUrl: extensionHostPoolRegistration.modulesUrl
                  configurationFunction: extensionHostPoolRegistration.configurationFunction
                  properties: {
                    hostPoolName: extensionHostPoolRegistration.hostPoolName
                    aadJoin: true
                  }
                }
                protectedSettings: {
                  properties: {
                    registrationInfoToken: extensionHostPoolRegistration.registrationInfoToken
                  }
                }
                supressFailures: extensionHostPoolRegistration.?supressFailures ?? false
                tags: extensionHostPoolRegistration.?tags ?? tags
              }
            }
            """, VmExtensionChild, [],
            [
                LeakWarning("name", "extensionHostPoolRegistration.?name"),
                LeakWarning("typeHandlerVersion", "extensionHostPoolRegistration.?typeHandlerVersion"),
                LeakWarning("autoUpgradeMinorVersion", "extensionHostPoolRegistration.?autoUpgradeMinorVersion"),
                LeakWarning("enableAutomaticUpgrade", "extensionHostPoolRegistration.?enableAutomaticUpgrade"),
                LeakWarning("settings.modulesUrl", "extensionHostPoolRegistration.modulesUrl"),
                LeakWarning("settings.configurationFunction", "extensionHostPoolRegistration.configurationFunction"),
                LeakWarning("settings.properties.hostPoolName", "extensionHostPoolRegistration.hostPoolName"),
                LeakWarning("supressFailures", "extensionHostPoolRegistration.?supressFailures"),
                LeakWarning("tags", "extensionHostPoolRegistration.?tags"),
            ]);

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
        var result = CompileModule(source, "param values array");
        result.Diagnostics.Where(diagnostic => diagnostic.Code == "BCP180").Should().ContainSingle();
        result.Diagnostics.Where(diagnostic => diagnostic.Code == NoSecureValuesInInsecureInputsRule.Code).Should().HaveCount(includeOtherSecret ? 1 : 0);
    }

    [TestMethod]
    [DataRow("storage.listKeys()", "object", "function 'listKeys'")]
    [DataRow("storage.listKeys().keys[0].value", "string", "secure value 'storage.listKeys().keys[0].value'")]
    public void Runtime_secret_functions_warn_for_plain_module_parameters(string expression, string type, string found)
    {
        var parent = $$"""
            resource storage 'Microsoft.Storage/storageAccounts@2024-01-01' existing = {
              name: 'storage'
            }
            module child 'child.bicep' = {
              name: 'child'
              params: {
                value: {{expression}}
              }
            }
            """;
        var result = CompileModule(parent, $"param value {type}");
        result.Diagnostics.Where(diagnostic => diagnostic.Level == DiagnosticLevel.Error).Should().BeEmpty();
        var warning = result.Diagnostics.Where(diagnostic => diagnostic.Code == NoSecureValuesInInsecureInputsRule.Code)
            .Should().ContainSingle().Subject;
        warning.Message.Should().Be($"Module parameter 'value' is not secure, but is assigned {found}. The value will be visible in the module deployment's inputs.");
        warning.Span.Position.Should().Be(parent.LastIndexOf(expression, StringComparison.Ordinal));
        warning.Span.Length.Should().Be(expression.Length);
    }

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

    // Not in AVM: a synthetic resourceInput child. The resource type marks `adminPassword` secure, and the retry
    // takes that as protection, though the emitted module parameter does not protect it.
    [TestMethod]
    [Ignore("Deferred: resourceInput sensitivity is not proof of emitted module parameter protection.")]
    public void Resource_input_os_profile_should_not_hide_a_plain_child_parameter()
        => AssertFlow("""
            @secure()
            param password string
            module child 'child.bicep' = {
              name: 'child'
              params: {
                osProfile: {
                  adminPassword: password
                }
              }
            }
            """, "param osProfile resourceInput<'Microsoft.Compute/virtualMachines@2024-07-01'>.properties.osProfile", [], [LeakWarning("osProfile.adminPassword", "password")]);

    // container-service/managed-cluster forwards `fluxExtension.?fluxConfigurations` to
    // kubernetes-configuration/extension.
    [TestMethod]
    [Ignore("Deferred: compiled resourceInput items inherit sensitivity without ARM parameter protection.")]
    public void Aks_flux_array_should_warn_for_unprotected_resource_input_child()
    {
        const string child = """
            {
              "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
              "languageVersion": "2.0",
              "contentVersion": "1.0.0.0",
              "parameters": {
                "fluxConfigurations": {
                  "type": "array",
                  "nullable": true,
                  "items": {
                    "type": "object",
                    "metadata": {
                      "__bicep_resource_derived_type!": {
                        "source": "Microsoft.KubernetesConfiguration/fluxConfigurations@2025-04-01#properties/properties"
                      }
                    }
                  }
                }
              },
              "resources": {}
            }
            """;
        string[] secretPaths =
        [
            "azureBlob.accountKey",
            "azureBlob.sasToken",
            "azureBlob.servicePrincipal.clientCertificate",
            "azureBlob.servicePrincipal.clientCertificatePassword",
            "azureBlob.servicePrincipal.clientSecret",
            "ociRepository.tlsConfig.caCertificate",
            "ociRepository.tlsConfig.clientCertificate",
            "ociRepository.tlsConfig.privateKey",
        ];
        AssertFlow("""
            type extensionType = {
              fluxConfigurations: resourceInput<'Microsoft.KubernetesConfiguration/fluxConfigurations@2025-04-01'>.properties[]?
            }
            param fluxExtension extensionType
            module extension 'child.json' = {
              name: 'extension'
              params: {
                fluxConfigurations: fluxExtension.?fluxConfigurations
              }
            }
            """, child, [], [.. secretPaths.Select(path => LeakWarning($"fluxConfigurations[*].{path}", "fluxExtension.?fluxConfigurations"))], "child.json");
    }
}
