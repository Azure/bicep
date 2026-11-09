// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class UseSecureValueForSecureInputsRuleTests : SecureAssignmentCheckerTestsBase
{
    [TestMethod]
    [DataRow(8)]
    [DataRow(22)]
    [Timeout(60_000)]
    public void Repeated_variable_reads_do_not_expand_a_diamond_exponentially(int depth)
        => AssertDiamondAnalysisCompletesPromptly(depth, SecureAssignmentChecker.EnumerateInsecureValuesForSecureParameters);

    [TestMethod]
    public void Cached_variable_findings_keep_each_destination_path_and_target_type()
        => AssertFlow("""
            var shared = { value: 'public' }
            module child 'child.bicep' = {
              name: 'child'
              params: {
                settings: {
                  public: shared
                  first: shared
                  second: shared
                }
              }
            }
            """, """
            type protected = {
              @secure()
              value: string
            }
            param settings {
              public: { value: string }
              first: protected
              second: protected
            }
            """, [
                InputWarning("settings.first.value"),
                InputWarning("settings.second.value"),
            ], []);

    [TestMethod]
    [DataRow("{ value: source }", "source")]
    [DataRow("alias", "alias")]
    public void Module_warnings_point_to_the_leaf_or_module_variable_reference(string expression, string anchor)
    {
        var parent = $$"""
            param source string
            var config = { value: source }
            var alias = config
            module child 'child.bicep' = {
              name: 'child'
              params: {
                config: {{expression}}
              }
            }
            """;
        var result = CompileModule(parent, """
            param config {
              @secure()
              value: string
            }
            """);
        result.Diagnostics.Where(diagnostic => diagnostic.Level == DiagnosticLevel.Error).Should().BeEmpty();
        var warning = result.Diagnostics.Where(diagnostic => diagnostic.Code == UseSecureValueForSecureInputsRule.Code)
            .Should().ContainSingle().Subject;
        warning.Message.Should().Be(InputWarning("config.value"));
        warning.Span.Position.Should().Be(parent.LastIndexOf(anchor, StringComparison.Ordinal));
        warning.Span.Length.Should().Be(anchor.Length);
    }

    private void CompileAndTest(string text, int expectedDiagnosticCount, Options? options = null)
    {
        AssertLinterRuleDiagnostics(UseSecureValueForSecureInputsRule.Code, text, expectedDiagnosticCount, options);
    }

    [DataRow("""
resource ubuntuVM 'Microsoft.Compute/virtualMachineScaleSets@2023-09-01' = {
  name: 'name'
  location: 'West US'
  properties: {
    virtualMachineProfile: {
      osProfile: {
        adminUsername: 'adminUsername'
        adminPassword: 'adminPassword'
      }
    }
  }
}
""")]
    [DataRow("""
resource ubuntuVM 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name'
  location: 'West US'
  properties: {
    osProfile: {
      computerName: 'computerName'
      adminUsername: 'adminUsername'
      adminPassword: 'adminPassword'
    }
  }
}
""")]
    [DataRow("""
param adminPassword string

resource ubuntuVM 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name'
  location: 'West US'
  properties: {
    osProfile: {
      computerName: 'computerName'
      adminUsername: 'adminUsername'
      adminPassword: adminPassword
    }
  }
}
""")]
    [DataRow("""
resource server 'Microsoft.DBforMySQL/flexibleServers@2023-12-30' = {
  name: 'myServer'
  location: 'westus'
  properties: {
    administratorLoginPassword: 'hardCodedValue'
  }
}
""")]
    [TestMethod]
    public void Linter_validation_should_warn_for_insecure_fields(string text)
        => CompileAndTest(text, 1);

    [DataRow("""
@secure()
param adminPassword string

resource ubuntuVM 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'name'
  location: 'West US'
  properties: {
    osProfile: {
      computerName: 'computerName'
      adminUsername: 'adminUsername'
      adminPassword: adminPassword
    }
  }
}
""")]
    [TestMethod]
    public void Linter_validation_should_not_warn_for_secure_password_field(string text)
        => CompileAndTest(text, 0);

    [DataRow("""
param sqlLogicalServer object

resource sqlLogicalServerRes 'Microsoft.Sql/servers@2024-05-01-preview' = {
  name: sqlLogicalServer.name
  location: resourceGroup().location
  properties: {
    administratorLogin: null
    administratorLoginPassword: null
  }
}
""")]
    [DataRow("""
param sqlLogicalServer object

resource sqlLogicalServerRes 'Microsoft.Sql/servers@2024-05-01-preview' = {
  name: sqlLogicalServer.name
  location: resourceGroup().location
  properties: {
    administratorLogin: null
    administratorLoginPassword: ''
  }
}
""")]
    [TestMethod]
    public void Linter_validation_should_not_warn_for_null_or_empty_string_assignment(string text)
        => CompileAndTest(text, 0);

    [DataRow("""
resource storageAccount 'Microsoft.Storage/storageAccounts@2024-01-01' existing = {
  name: 'mystorageaccount'
}

resource script 'Microsoft.Resources/deploymentScripts@2023-08-01' = {
  name: 'azcli-script'
  location: 'westus'
  kind: 'AzureCLI'
  properties: {
    azCliVersion: '2.0.81'
    scriptContent: 'echo "Hello, World!"'
    retentionInterval: 'PT1H'
    storageAccountSettings: {
      storageAccountName: storageAccount.name
      storageAccountKey: storageAccount.listKeys().keys[0].value
    }
  }
}
""")]
    [TestMethod]
    public void Linter_validation_should_not_warn_for_chained_values(string text)
        => CompileAndTest(text, 0);

    // https://github.com/Azure/bicep/issues/17371
    [DataRow("""
param sqlLogicalServer object
@secure()
param password string

resource sqlLogicalServerRes 'Microsoft.Sql/servers@2024-05-01-preview' = {
  name: sqlLogicalServer.name
  location: resourceGroup().location
  properties: {
    administratorLogin: null
    administratorLoginPassword: sqlLogicalServer.login =~ 'Entra' ? null : password
  }
}
""")]
    [TestMethod]
    public void Linter_validation_should_not_warn_for_conditional_null_assignment(string text)
          => CompileAndTest(text, 0);

    // Reduced from the before and after states of https://github.com/Azure/bicep-registry-modules/pull/7472.
    // These retain forwarding expressions, not resource setup. Tests marked "Not in AVM" are synthetic
    // edge cases around an AVM binding.

    // api-management/service and api-management/service/workspace use the same subscription binding.
    [TestMethod]
    [DataRow("primaryKey", true, DisplayName = "api-management subscriptions primaryKey")]
    [DataRow("secondaryKey", true, DisplayName = "api-management subscriptions secondaryKey")]
    [DataRow("value", true, DisplayName = "api-management/service/workspace named values")]
    [DataRow("storageAccountAccessKey", true, DisplayName = "sql/server security alert policies")]
    [DataRow("contentType", true, DisplayName = "key-vault/vault secrets")]
    [DataRow("serviceUri", false, DisplayName = "container-registry/registry webhooks")]
    public void Avm_forwarded_members_require_secure_parent_fields(string member, bool optional)
    {
        var nullable = optional ? "?" : "";
        AssertHardening($$"""
            type inputType = {
              SECURE
              {{member}}: string{{nullable}}
            }
            param inputs inputType[]?
            module children 'child.bicep' = [for (item, index) in (inputs ?? []): {
              name: 'child-${index}'
              params: {
                {{member}}: item.{{nullable}}{{member}}
              }
            }]
            """, $"@secure()\nparam {member} string{nullable}", member, [$"item.{nullable}{member}"]);
    }

    [TestMethod]
    public void Avm_certificate_requires_secure_parent_field()
        => AssertHardening("""
            type certificateType = {
              SECURE
              certificateValue: string?
            }
            param certificate certificateType?
            module child 'child.bicep' = if (!empty(certificate)) {
              name: 'certificate'
              params: {
                certificateValue: certificate.?certificateValue
              }
            }
            """, "@secure()\nparam certificateValue string?", "certificateValue", ["certificate.?certificateValue"]);

    [TestMethod]
    public void Avm_named_value_fallback_requires_secure_parameter()
        => AssertHardening("""
            SECURE
            param newGuidValue string = newGuid()
            param namedValues {
              @secure()
              value: string?
            }[]?
            module children 'child.bicep' = [for (item, index) in (namedValues ?? []): {
              name: 'named-${index}'
              params: {
                value: item.?value ?? newGuidValue
              }
            }]
            """, "@secure()\nparam value string", "value", ["item.?value", "newGuidValue"]);

    [TestMethod]
    public void Avm_automation_variables_require_typed_secure_value()
    {
        const string binding = """

            module children 'child.bicep' = [for (item, index) in variables: {
              name: 'variable-${index}'
              params: {
                value: item.value
              }
            }]
            """;
        AssertFlow("param variables array = []" + binding, "@secure()\nparam value string", 1, 0);
        const string typed = """
            param variables {
              name: string
              @secure()
              value: string
            }[] = []
            """;
        AssertFlow(typed + binding, "@secure()\nparam value string", 0, 0);
        AssertFlow(typed + binding, "param value string", [], [LeakWarning("value", "item.value")]);
    }

    [TestMethod]
    public void Avm_dsc_settings_require_secure_parent_field()
    {
        const string binding = """

            module dsc 'child.bicep' = if (config.enabled) {
              name: 'dsc'
              params: {
                protectedSettings: config.?protectedSettings ?? {}
              }
            }
            """;
        AssertFlow("param config object = { enabled: false }" + binding, "@secure()\nparam protectedSettings object?", 1, 0);
        const string typed = """
            param config {
              enabled: bool
              @secure()
              protectedSettings: object?
            }
            """;
        AssertFlow(typed + binding, "@secure()\nparam protectedSettings object?", 0, 0);
        AssertFlow(typed + binding, "param protectedSettings object?", [], [LeakWarning("protectedSettings", "config.?protectedSettings")]);
    }

    [TestMethod]
    public void Avm_sql_secrets_export_requires_secure_administrator_password()
        => AssertHardening(SqlSecretsExport, """
            param secretsToSet {
              name: string
              @secure()
              value: string
            }[]
            """, "secretsToSet[*].value", ["administratorLoginPassword"]);

    // compute/virtual-machine and compute/virtual-machine-scale-set use the same custom script binding. The AVM
    // change left it alone because its parent field was already `@secure()`, so it has to stay free of warnings.
    [TestMethod]
    public void Avm_custom_script_conditional_spreads_are_valid_for_secure_parent_field()
        => AssertFlow("""
            type scriptType = {
              @secure()
              protectedSettings: {
                commandToExecute: string?
                storageAccountName: string?
                storageAccountKey: string?
                fileUris: string[]?
                managedIdentityResourceId: string?
              }?
            }
            param extensionCustomScriptConfig scriptType?
            resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2024-11-30' existing = {
              name: 'identity'
            }
            module script 'child.bicep' = {
              name: 'script'
              params: {
                protectedSettings: {
                  ...(!empty(extensionCustomScriptConfig!.?protectedSettings.?commandToExecute)
                    ? { commandToExecute: extensionCustomScriptConfig!.protectedSettings!.?commandToExecute }
                    : {})
                  ...(!empty(extensionCustomScriptConfig!.?protectedSettings.?storageAccountName)
                    ? { storageAccountName: extensionCustomScriptConfig!.protectedSettings!.storageAccountName! }
                    : {})
                  ...(!empty(extensionCustomScriptConfig!.?protectedSettings.?storageAccountKey)
                    ? { storageAccountKey: extensionCustomScriptConfig!.protectedSettings!.storageAccountKey! }
                    : {})
                  ...(!empty(extensionCustomScriptConfig!.?protectedSettings.?fileUris)
                    ? { fileUris: extensionCustomScriptConfig!.protectedSettings!.fileUris! }
                    : {})
                  ...(extensionCustomScriptConfig!.?protectedSettings.?managedIdentityResourceId != null
                    ? {
                        managedIdentity: !empty(extensionCustomScriptConfig!.protectedSettings!.?managedIdentityResourceId)
                          ? {
                              clientId: identity.properties.clientId
                            }
                          : {}
                      }
                    : {})
                }
              }
            }
            """, "@secure()\nparam protectedSettings object?", 0, 0);

    [TestMethod]
    public void Avm_workspace_credentials_require_secure_resource_derived_parent_field()
        => AssertHardening("""
            param loggers {
              SECURE
              credentials: resourceInput<'Microsoft.ApiManagement/service/workspaces/loggers@2024-05-01'>.properties.credentials?
            }[]?
            module children 'child.bicep' = [for (logger, index) in (loggers ?? []): {
              name: 'logger-${index}'
              params: {
                credentials: logger.?credentials
              }
            }]
            """, "@secure()\nparam credentials resourceInput<'Microsoft.ApiManagement/service/workspaces/loggers@2024-05-01'>.properties.credentials?", "credentials.*", ["logger.?credentials"], leakPath: "credentials");

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Avm_flux_settings_preserve_compiled_secure_resource_derived_type(bool loop)
    {
        var parent = $$"""
            param inputs {
              SECURE
              configurationProtectedSettings: resourceInput<'Microsoft.KubernetesConfiguration/fluxConfigurations@2025-04-01'>.properties.configurationProtectedSettings?
            }[]?
            module child 'child.json' = {{(loop ? "[for (item, index) in (inputs ?? []):" : "")}} {
              name: {{(loop ? "'flux-${index}'" : "'extension'")}}
              params: {
                configurationProtectedSettings: {{(loop ? "item" : "inputs![0]")}}.?configurationProtectedSettings
              }
            }{{(loop ? "]" : "")}}
            """;
        const string child = """
            {
              "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
              "languageVersion": "2.0",
              "contentVersion": "1.0.0.0",
              "parameters": {
                "configurationProtectedSettings": {
                  "type": "secureObject",
                  "nullable": true,
                  "metadata": {
                    "__bicep_resource_derived_type!": {
                      "source": "Microsoft.KubernetesConfiguration/fluxConfigurations@2025-04-01#properties/properties/properties/configurationProtectedSettings"
                    }
                  }
                }
              },
              "resources": {}
            }
            """;
        AssertHardening(parent, child, "configurationProtectedSettings.*", [$"{(loop ? "item" : "inputs![0]")}.?configurationProtectedSettings"],
            "child.json", child.Replace("secureObject", "object"), "configurationProtectedSettings");
    }

    [TestMethod]
    [DataRow("union({ first: secret }, { second: secret })", 0)]
    [DataRow("{ first: secret, second: secret }", 0)]
    [DataRow("union({ first: secret }, { second: plain })", 1)]
    [DataRow("union({ first: plain }, { first: secret })", 0)]
    [DataRow("union({ first: secret }, { first: plain })", 1)]
    [DataRow("sys.union({ first: secret }, { first: plain })", 1)]
    [DataRow("{ nested: [secret] }", 0)]
    [DataRow("{ nested: [secret, plain] }", 1)]
    public void Object_union_and_literal_have_equivalent_security(string expression, int warnings)
        => AssertFlow($$"""
            @secure()
            param secret string
            param plain string
            module child 'child.bicep' = {
              name: 'combined'
              params: {
                config: {{expression}}
              }
            }
            """, "@secure()\nparam config object", warnings, 0);

    // compute/virtual-machine-scale-set monitoring extension: a runtime key, or an empty string, is a valid
    // value inside a secure object, even though the key is the only secret.
    [TestMethod]
    public void Avm_monitor_runtime_key_or_empty_string_is_valid_inside_secure_object()
        => AssertFlow("""
            param monitoringWorkspaceResourceId string
            resource workspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' existing = {
              name: 'workspace'
            }
            module monitor 'child.bicep' = {
              name: 'monitor'
              params: {
                protectedSettings: {
                  workspaceKey: !empty(monitoringWorkspaceResourceId) ? workspace.listKeys().primarySharedKey : ''
                }
              }
            }
            """, "@secure()\nparam protectedSettings object?", 0, 0);

    [TestMethod]
    public void Module_parameter_diagnostic_names_the_parameter_path()
        => AssertFlow("""
            module child 'child.bicep' = {
              name: 'child'
              params: {
                config: {
                  password: 'public'
                }
              }
            }
            """, """
            param config {
              @secure()
              password: string
            }
            """, [InputWarning("config.password")], []);

    [TestMethod]
    [DataRow("uniqueString(secret)")]
    [DataRow("guid(secret)")]
    [DataRow("nameof(secret)")]
    [DataRow("string(length(secret))")]
    [DataRow("'${length(secret)}'")]
    public void Non_secret_results_do_not_satisfy_secure_module_parameters(string expression)
        => AssertFlow($$"""
            @secure()
            param secret string
            module child 'child.bicep' = {
              name: 'child'
              params: {
                value: {{expression}}
              }
            }
            """, "@secure()\nparam value string", 1, 0);
}
