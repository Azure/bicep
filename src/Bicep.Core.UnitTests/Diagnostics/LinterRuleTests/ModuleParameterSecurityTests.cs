// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

// Reduced from https://github.com/Azure/bicep-registry-modules/pull/7472, base 063ef550c5b61b46eab222f05b30693dacaf8bc9
// and head 4a6ce190e8736bba36977c7ba1ce01ee9a8d4472. These retain forwarding expressions, not resource setup.
[TestClass]
public class ModuleParameterSecurityTests : LinterRuleTestsBase
{
    [TestMethod]
    [DataRow("api-management/service subscriptions", "primaryKey", true)]
    [DataRow("api-management/service subscriptions", "secondaryKey", true)]
    [DataRow("api-management/service/workspace subscriptions", "primaryKey", true)]
    [DataRow("api-management/service/workspace subscriptions", "secondaryKey", true)]
    [DataRow("api-management/service/workspace named values", "value", true)]
    [DataRow("app/managed-environment certificates", "certificateValue", true)]
    [DataRow("key-vault/vault secrets", "contentType", true)]
    [DataRow("container-registry/registry webhooks", "serviceUri", false)]
    [DataRow("automation/automation-account variables", "value", false)]
    public void Hardening_forwarded_loop_members_removes_warning(string scenario, string member, bool optional)
    {
        var source = $$"""
type inputType = {
  SECURE
  {{member}}: string{{(optional ? "?" : "")}}
}
param inputs inputType[]?

module children 'child.bicep' = [for (item, index) in (inputs ?? []): {
  name: 'child-${index}'
  params: {
    {{member}}: item.{{(optional ? "?" : "")}}{{member}}
  }
}]
""";
        AssertHardening(source, $"@secure()\nparam {member} string?", member);
    }

    [TestMethod]
    [DataRow("compute/virtual-machine DSC")]
    [DataRow("compute/virtual-machine-scale-set DSC")]
    public void Hardening_coalesced_protected_settings_removes_warning(string scenario)
        => AssertHardening("""
type dscType = {
  SECURE
  protectedSettings: object?
}
param extensionDSCConfig dscType

module dsc 'child.bicep' = {
  name: 'dsc'
  params: {
    protectedSettings: extensionDSCConfig.?protectedSettings ?? {}
  }
}
""", "@secure()\nparam protectedSettings object", "protectedSettings");

    [TestMethod]
    public void Hardening_sql_conditional_module_input_removes_warning()
        => AssertHardening("""
type securityAlertPolicyType = {
  SECURE
  storageAccountAccessKey: string?
}
param securityAlertPolicy securityAlertPolicyType?

module policy 'child.bicep' = if (securityAlertPolicy != null) {
  name: 'policy'
  params: {
    storageAccountAccessKey: securityAlertPolicy.?storageAccountAccessKey
  }
}
""", "@secure()\nparam storageAccountAccessKey string?", "storageAccountAccessKey");

    private const string CompiledFluxChild = """
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

    [TestMethod]
    [DataRow("container-service/managed-cluster to extension", false)]
    [DataRow("kubernetes-configuration/extension to flux-configuration", true)]
    public void Compiled_resource_derived_secure_object_keeps_protection(string scenario, bool loop)
    {
        var source = $$"""
type fluxType = {
  SECURE
  configurationProtectedSettings: resourceInput<'Microsoft.KubernetesConfiguration/fluxConfigurations@2025-04-01'>.properties.configurationProtectedSettings?
}
param fluxExtension fluxType
param fluxConfigurations fluxType[]?

module child 'child.json' = {{(loop ? "[for (fluxConfiguration, index) in (fluxConfigurations ?? []):" : "")}} {
  name: {{(loop ? "'flux-${index}'" : "'extension'")}}
  params: {
    configurationProtectedSettings: {{(loop ? "fluxConfiguration" : "fluxExtension")}}.?configurationProtectedSettings
  }
}{{(loop ? "]" : "")}}
""";
        AssertHardening(source, CompiledFluxChild, "configurationProtectedSettings", "child.json");
    }

    [TestMethod]
    public void Apim_named_value_plain_fallback_still_warns_after_member_hardening()
    {
        const string source = """
param newGuidValue string = newGuid()
type namedValueType = {
  SECURE
  value: string?
}
param namedValues namedValueType[]?

module children 'child.bicep' = [for (namedValue, index) in (namedValues ?? []): {
  name: 'named-value-${index}'
  params: {
    value: namedValue.?value ?? newGuidValue
  }
}]
""";
        var child = "@secure()\nparam value string";
        AssertFlow(source.Replace("SECURE", ""), child, [InputWarning("value")], []);
        AssertFlow(source.Replace("SECURE", "@secure()"), child, [InputWarning("value")], []);
    }

    [TestMethod]
    public void Workspace_logger_plain_credentials_still_warn()
        => AssertFlow("""
type loggerType = {
  credentials: object?
}
param loggers loggerType[]?

module children 'child.bicep' = [for (logger, index) in (loggers ?? []): {
  name: 'logger-${index}'
  params: {
    credentials: logger.?credentials
  }
}]
""", "@secure()\nparam credentials object?", [InputWarning("credentials")], []);

    [TestMethod]
    [DataRow("compute/virtual-machine custom script")]
    [DataRow("compute/virtual-machine-scale-set custom script")]
    public void Custom_script_conditional_spreads_keep_secure_ancestor(string scenario)
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
resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2024-11-30' existing = {
  name: 'identity'
}
""", "@secure()\nparam protectedSettings object", [], []);

    [TestMethod]
    public void Vmss_monitor_runtime_key_or_empty_string_is_not_an_insecure_caller_input()
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
""", "@secure()\nparam protectedSettings object", [], []);

    [TestMethod]
    public void Apim_cache_plain_to_plain_has_no_sensitivity_to_infer()
    {
        const string source = """
type cacheType = {
  SECURE
  connectionString: string?
}
param caches cacheType[]?

module children 'child.bicep' = [for (cache, index) in (caches ?? []): {
  name: 'cache-${index}'
  params: {
    connectionString: cache.?connectionString
  }
}]
""";
        AssertFlow(source.Replace("SECURE", ""), "param connectionString string?", [], []);
        AssertFlow(source.Replace("SECURE", "@secure()"), "@secure()\nparam connectionString string?", [], []);
        AssertFlow(source.Replace("SECURE", ""), "@secure()\nparam connectionString string?", [InputWarning("connectionString")], []);
    }

    [TestMethod]
    public void Flux_resource_derived_dictionary_alone_has_no_sensitivity_to_infer()
        => AssertFlow("""
param settings resourceInput<'Microsoft.KubernetesConfiguration/fluxConfigurations@2025-04-01'>.properties.configurationProtectedSettings?

module child 'child.bicep' = {
  name: 'child'
  params: {
    settings: settings
  }
}
""", "param settings object?", [], []);

    private static void AssertHardening(string source, string child, string input, string childPath = "child.bicep")
    {
        AssertFlow(source.Replace("SECURE", ""), child, [InputWarning(input)], [], childPath);
        AssertFlow(source.Replace("SECURE", "@secure()"), child, [], [], childPath);
    }

    private static void AssertFlow(string source, string child, string[] inputWarnings, string[] leakWarnings, string childPath = "child.bicep")
    {
        var options = new Options(IncludePosition: IncludePosition.None, AdditionalFiles: [(childPath, child)]);
        AssertLinterRuleDiagnostics(UseSecureValueForSecureInputsRule.Code, source, inputWarnings, options);
    }

    private static string InputWarning(string input)
        => $"Property '{input}' expects a secure value, but the value provided may not be secure.";
}
