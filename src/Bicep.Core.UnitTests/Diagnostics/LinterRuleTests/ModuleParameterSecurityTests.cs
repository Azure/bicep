// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

// Reduced from the before and after states of https://github.com/Azure/bicep-registry-modules/pull/7472.
// These retain forwarding expressions, not resource setup.
// Tests marked "Not in AVM" are synthetic edge cases around an AVM binding.
[TestClass]
public class ModuleParameterSecurityTests : LinterRuleTestsBase
{
    // api-management/service and api-management/service/workspace use the same subscription binding.
    [TestMethod]
    [DataRow("primaryKey", true, DisplayName = "api-management subscriptions primaryKey")]
    [DataRow("secondaryKey", true, DisplayName = "api-management subscriptions secondaryKey")]
    [DataRow("value", true, DisplayName = "api-management/service/workspace named values")]
    [DataRow("storageAccountAccessKey", true, DisplayName = "sql/server security alert policies")]
    [DataRow("contentType", true, DisplayName = "key-vault/vault secrets")]
    [DataRow("serviceUri", false, DisplayName = "container-registry/registry webhooks")]
    public void Hardening_forwarded_loop_members_removes_warning(string member, bool optional)
    {
        var nullable = optional ? "?" : "";
        var source = $$"""
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
""";
        AssertHardening(source, $"@secure()\nparam {member} string{nullable}", member);
    }

    [TestMethod]
    public void Hardening_automation_variables_type_removes_warning()
    {
        const string forwarding = """

module children 'child.bicep' = [for (variable, index) in variables: {
  name: 'variable-${index}'
  params: {
    value: variable.value
  }
}]
""";
        const string child = "@secure()\nparam value string";
        AssertFlow("param variables array = []" + forwarding, child, [InputWarning("value")], []);
        AssertFlow("""
type variableType = {
  name: string
  @secure()
  value: string
  description: string?
  isEncrypted: bool?
}
param variables variableType[] = []
""" + forwarding, child, [], []);
    }

    // compute/virtual-machine and compute/virtual-machine-scale-set use the same DSC binding.
    [TestMethod]
    public void Hardening_dsc_config_type_removes_warning()
    {
        const string forwarding = """

module dsc 'child.bicep' = if (extensionDSCConfig.enabled) {
  name: 'dsc'
  params: {
    protectedSettings: extensionDSCConfig.?protectedSettings ?? {}
  }
}
""";
        const string child = "@secure()\nparam protectedSettings object?";
        AssertFlow("""
param extensionDSCConfig object = {
  enabled: false
}
""" + forwarding, child, [InputWarning("protectedSettings")], []);
        AssertFlow("""
type extensionDSCConfigType = {
  enabled: bool
  settings: object?
  @secure()
  protectedSettings: object?
}
param extensionDSCConfig extensionDSCConfigType = {
  enabled: false
}
""" + forwarding, child, [], []);
    }

    [TestMethod]
    public void Hardening_managed_environment_conditional_certificate_removes_warning()
        => AssertHardening("""
type certificateType = {
  SECURE
  certificateValue: string?
}
param certificate certificateType?

module managedEnvironment_certificate 'child.bicep' = if (!empty(certificate)) {
  name: 'certificate'
  params: {
    certificateValue: certificate.?certificateValue
  }
}
""", "@secure()\nparam certificateValue string?", "certificateValue");

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
    [DataRow(false, DisplayName = "container-service/managed-cluster to extension")]
    [DataRow(true, DisplayName = "kubernetes-configuration/extension to flux-configuration")]
    public void Compiled_resource_derived_secure_object_keeps_protection(bool loop)
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
    public void Hardening_apim_named_value_fallback_removes_warning()
        => AssertHardening("""
SECURE
param newGuidValue string = newGuid()
type namedValueType = {
  @secure()
  value: string?
}
param namedValues namedValueType[]?

module children 'child.bicep' = [for (namedValue, index) in (namedValues ?? []): {
  name: 'named-value-${index}'
  params: {
    value: namedValue.?value ?? newGuidValue
  }
}]
""", "@secure()\nparam value string = newGuid()", "value");

    [TestMethod]
    public void Hardening_workspace_logger_credentials_removes_warning()
        => AssertHardening("""
type loggerType = {
  SECURE
  credentials: resourceInput<'Microsoft.ApiManagement/service/workspaces/loggers@2024-05-01'>.properties.credentials?
}
param loggers loggerType[]?

module children 'child.bicep' = [for (logger, index) in (loggers ?? []): {
  name: 'logger-${index}'
  params: {
    credentials: logger.?credentials
  }
}]
""", "@secure()\nparam credentials resourceInput<'Microsoft.ApiManagement/service/workspaces/loggers@2024-05-01'>.properties.credentials?", "credentials");

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

    // Not in AVM: its export child declares a secure value member.
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
    public void Hardening_apim_portal_child_properties_type_removes_warning()
    {
        AssertFlow(ApimPortalSettings, "param properties resourceInput<'Microsoft.ApiManagement/service/portalsettings@2024-05-01'>.properties",
            [], [LeakWarning("properties.validationKey", "portalsetting.properties.validationKey")]);
        AssertFlow(ApimPortalSettings, PortalSettingPropertiesChild, [], []);
    }

    // Not in AVM: its sign-in variant is closed.
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
"""), PortalSettingPropertiesChild, [InputWarning("properties.validationKey")], []);

    // compute/virtual-machine and compute/virtual-machine-scale-set use the same custom script binding.
    [TestMethod]
    public void Custom_script_conditional_spreads_keep_secure_ancestor()
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
""", "@secure()\nparam protectedSettings object?", [], []);

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
""", "@secure()\nparam protectedSettings object?", [], []);

    [TestMethod]
    public void Hardening_apim_cache_requires_securing_parent_and_child_together()
    {
        const string source = """
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
        AssertFlow(source.Replace("SECURE", ""), "param connectionString string", [], []);
        AssertFlow(source.Replace("SECURE", "@secure()"), "@secure()\nparam connectionString string", [], []);
        AssertFlow(source.Replace("SECURE", ""), "@secure()\nparam connectionString string", [InputWarning("connectionString")], []);
    }

    // Not in AVM: a resource-derived dictionary forwarded on its own.
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

    [TestMethod]
    public void Whole_secure_vm_domain_join_object_marks_all_forwarded_members_sensitive()
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

    [TestMethod]
    public void Whole_secure_vmss_domain_join_object_marks_all_forwarded_members_sensitive()
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

    [TestMethod]
    public void Whole_secure_host_pool_object_marks_non_token_members_sensitive()
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

    // Not in AVM: a synthetic resourceInput child.
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
