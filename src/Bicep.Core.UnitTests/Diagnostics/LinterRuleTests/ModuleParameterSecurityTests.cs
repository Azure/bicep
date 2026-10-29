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

    private const string SqlSecretsExport = """
SECURE
param administratorLoginPassword string
param secretsExportConfiguration {
  sqlAdminPasswordSecretName: string?
}

module export 'child.bicep' = {
  name: 'export'
  params: {
    secretsToSet: union(
      [],
      contains(secretsExportConfiguration, 'sqlAdminPasswordSecretName')
        ? [
            {
              name: secretsExportConfiguration.?sqlAdminPasswordSecretName
              value: administratorLoginPassword
            }
          ]
        : []
    )
  }
}
""";

    [TestMethod]
    public void Sql_secret_export_union_is_a_secure_source_for_a_plain_child()
        => AssertFlow(SqlSecretsExport.Replace("SECURE", "@secure()"), "param secretsToSet array", [], [LeakWarning("secretsToSet[*].value", "administratorLoginPassword")]);

    [TestMethod]
    public void Sql_secret_export_union_to_a_secure_child_member_needs_only_a_secure_source()
        => AssertHardening(SqlSecretsExport, """
param secretsToSet {
  name: string
  @secure()
  value: string
}[]
""", "secretsToSet[*].value");

    private const string ApimPortalSettings = """
@discriminator('name')
type portalSettingsType = delegationType | signInType | signUpType

type delegationType = {
  name: 'delegation'
  properties: {
    @secure()
    validationKey: string?
  }
}
type signInType = {
  name: 'signin'
  properties: {
    enabled: bool
  }
}
type signUpType = {
  name: 'signup'
  properties: {
    enabled: bool?
  }
}
param portalsettings portalSettingsType[]

module children 'child.bicep' = [for (portalsetting, index) in portalsettings: {
  name: 'portal-${index}'
  params: {
    properties: portalsetting.properties
  }
}]
""";

    [TestMethod]
    public void Apim_portal_validation_key_forwarded_as_properties_warns()
        => AssertFlow(ApimPortalSettings, "param properties resourceInput<'Microsoft.ApiManagement/service/portalsettings@2024-05-01'>.properties",
            [], [LeakWarning("properties.validationKey", "portalsetting.properties.validationKey")]);

    [TestMethod]
    public void Apim_portal_variants_without_validation_key_do_not_warn_for_a_secure_child_member()
        => AssertFlow(ApimPortalSettings, """
param properties {
  enabled: bool?
  @secure()
  validationKey: string?
}
""", [], []);

    [TestMethod]
    public void Apim_portal_variant_open_to_other_properties_may_supply_a_plain_validation_key()
        => AssertFlow(ApimPortalSettings.Replace("""
  properties: {
    enabled: bool
  }
""", """
  properties: {
    enabled: bool
    *: string
  }
"""), """
param properties {
  enabled: bool?
  @secure()
  validationKey: string?
}
""", [InputWarning("properties.validationKey")], []);

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

    [TestMethod]
    [DataRow("compute/virtual-machine domain join", false)]
    [DataRow("compute/virtual-machine-scale-set domain join", true)]
    public void Whole_secure_domain_join_object_marks_all_forwarded_members_sensitive(string scenario, bool scaleSet)
    {
        var source = $$"""
@secure()
param extensionDomainJoinConfig object

module extension 'child.bicep' = {
  name: 'extension'
  params: {
    {{(scaleSet ? "" : "name: extensionDomainJoinConfig.?name")}}
    typeHandlerVersion: extensionDomainJoinConfig.?typeHandlerVersion
    autoUpgradeMinorVersion: extensionDomainJoinConfig.?autoUpgradeMinorVersion
    enableAutomaticUpgrade: extensionDomainJoinConfig.?enableAutomaticUpgrade
    settings: {{(scaleSet ? "extensionDomainJoinConfig.settings" : """
{
      ...extensionDomainJoinConfig.?settings
      Name: extensionDomainJoinConfig.?domainName
      OUPath: extensionDomainJoinConfig.?ouPath
      User: extensionDomainJoinConfig.?user
      Restart: extensionDomainJoinConfig.?restart
      Options: extensionDomainJoinConfig.?options
    }
""")}}
    {{(scaleSet ? "provisionAfterExtensions: extensionDomainJoinConfig.?provisionAfterExtensions" : """
supressFailures: extensionDomainJoinConfig.?supressFailures
    tags: extensionDomainJoinConfig.?tags
""")}}
  }
}
""";
        string[] memberWarnings = scaleSet ?
        [
            LeakWarning("settings", "extensionDomainJoinConfig.settings"),
            LeakWarning("provisionAfterExtensions", "extensionDomainJoinConfig.?provisionAfterExtensions"),
        ] :
        [
            LeakWarning("name", "extensionDomainJoinConfig.?name"),
            LeakWarning("settings", "extensionDomainJoinConfig.?settings"),
            LeakWarning("settings.Name", "extensionDomainJoinConfig.?domainName"),
            LeakWarning("settings.OUPath", "extensionDomainJoinConfig.?ouPath"),
            LeakWarning("settings.User", "extensionDomainJoinConfig.?user"),
            LeakWarning("settings.Restart", "extensionDomainJoinConfig.?restart"),
            LeakWarning("settings.Options", "extensionDomainJoinConfig.?options"),
            LeakWarning("supressFailures", "extensionDomainJoinConfig.?supressFailures"),
            LeakWarning("tags", "extensionDomainJoinConfig.?tags"),
        ];
        AssertFlow(source, ExtensionChild, [],
        [
            LeakWarning("typeHandlerVersion", "extensionDomainJoinConfig.?typeHandlerVersion"),
            LeakWarning("autoUpgradeMinorVersion", "extensionDomainJoinConfig.?autoUpgradeMinorVersion"),
            LeakWarning("enableAutomaticUpgrade", "extensionDomainJoinConfig.?enableAutomaticUpgrade"),
            .. memberWarnings,
        ]);
    }

    private const string ExtensionChild = """
param name string?
param typeHandlerVersion string?
param autoUpgradeMinorVersion bool?
param enableAutomaticUpgrade bool?
param settings object?
param provisionAfterExtensions array?
param supressFailures bool?
param tags object?
""";

    [TestMethod]
    public void Whole_secure_host_pool_object_marks_non_token_members_sensitive()
        => AssertFlow("""
@secure()
param extensionHostPoolRegistration object

module extension 'child.bicep' = {
  name: 'extension'
  params: {
    name: extensionHostPoolRegistration.?name
    typeHandlerVersion: extensionHostPoolRegistration.?typeHandlerVersion
    autoUpgradeMinorVersion: extensionHostPoolRegistration.?autoUpgradeMinorVersion
    enableAutomaticUpgrade: extensionHostPoolRegistration.?enableAutomaticUpgrade
    settings: {
      modulesUrl: extensionHostPoolRegistration.modulesUrl
      configurationFunction: extensionHostPoolRegistration.configurationFunction
      properties: {
        hostPoolName: extensionHostPoolRegistration.hostPoolName
      }
    }
    supressFailures: extensionHostPoolRegistration.?supressFailures
    tags: extensionHostPoolRegistration.?tags
  }
}
""", ExtensionChild, [],
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
""", "param osProfile resourceInput<'Microsoft.Compute/virtualMachines@2024-07-01'>.properties.osProfile",
            [], [LeakWarning("osProfile.adminPassword", "password")]);

    [TestMethod]
    [Ignore("Deferred: compiled resourceInput items inherit sensitivity without ARM parameter protection.")]
    public void Aks_flux_array_should_warn_for_unprotected_resource_input_child()
        => AssertLinterRuleDiagnostics(NoSecureValuesInInsecureInputsRule.Code, """
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
""",
            [
                .. new[]
                {
                    "azureBlob.accountKey",
                    "azureBlob.sasToken",
                    "azureBlob.servicePrincipal.clientCertificate",
                    "azureBlob.servicePrincipal.clientCertificatePassword",
                    "azureBlob.servicePrincipal.clientSecret",
                    "ociRepository.tlsConfig.caCertificate",
                    "ociRepository.tlsConfig.clientCertificate",
                    "ociRepository.tlsConfig.privateKey",
                }.Select(path => LeakWarning($"fluxConfigurations[*].{path}", $"fluxExtension.?fluxConfigurations[*].{path}")),
            ],
            new Options(IncludePosition: IncludePosition.None, AdditionalFiles:
            [
                ("child.json", """
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
"""),
            ]));

    private static void AssertHardening(string source, string child, string input, string childPath = "child.bicep")
    {
        AssertFlow(source.Replace("SECURE", ""), child, [InputWarning(input)], [], childPath);
        AssertFlow(source.Replace("SECURE", "@secure()"), child, [], [], childPath);
    }

    private static void AssertFlow(string source, string child, string[] inputWarnings, string[] leakWarnings, string childPath = "child.bicep")
    {
        var options = new Options(IncludePosition: IncludePosition.None, AdditionalFiles: [(childPath, child)]);
        AssertLinterRuleDiagnostics(UseSecureValueForSecureInputsRule.Code, source, inputWarnings, options);
        AssertLinterRuleDiagnostics(NoSecureValuesInInsecureInputsRule.Code, source, leakWarnings, options);
    }

    private static string InputWarning(string input)
        => $"Property '{input}' expects a secure value, but the value provided may not be secure.";

    private static string LeakWarning(string input, string source)
        => $"Module parameter '{input}' is not secure, but is assigned secure value '{source}'. The value will be visible in the module deployment's inputs.";
}
