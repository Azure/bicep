// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class UseSecureValueForSecureInputsRuleTests : LinterRuleTestsBase
{
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

    // Reduced forwarding boundaries from https://github.com/Azure/bicep-registry-modules/pull/7472.
    [TestMethod]
    [DataRow("connectionString", false)]
    [DataRow("primaryKey", true)]
    [DataRow("secondaryKey", true)]
    [DataRow("value", true)]
    [DataRow("storageAccountAccessKey", true)]
    [DataRow("contentType", true)]
    [DataRow("serviceUri", false)]
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
            """, $"@secure()\nparam {member} string{nullable}");
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
            """, "@secure()\nparam certificateValue string?");

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
            """, "@secure()\nparam value string");

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
        AssertFlow("""
            param variables {
              name: string
              @secure()
              value: string
            }[] = []
            """ + binding, "@secure()\nparam value string", 0, 0);
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
        AssertFlow("""
            param config {
              enabled: bool
              @secure()
              protectedSettings: object?
            }
            """ + binding, "@secure()\nparam protectedSettings object?", 0, 0);
    }

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
            """, "@secure()\nparam credentials resourceInput<'Microsoft.ApiManagement/service/workspaces/loggers@2024-05-01'>.properties.credentials?");

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
        AssertHardening(parent, child, "child.json");
    }

    [TestMethod]
    [DataRow("union({ first: secret }, { second: secret })", 0)]
    [DataRow("{ first: secret, second: secret }", 0)]
    [DataRow("union({ first: secret }, { second: plain })", 1)]
    [DataRow("union({ first: plain }, { first: secret })", 0)]
    [DataRow("union({ first: secret }, { first: plain })", 1)]
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

    [TestMethod]
    public void Runtime_secret_and_empty_string_are_valid_inside_secure_object()
        => AssertFlow("""
            param enabled bool
            resource workspace 'Microsoft.OperationalInsights/workspaces@2025-02-01' existing = {
              name: 'workspace'
            }
            module child 'child.bicep' = {
              name: 'monitor'
              params: {
                settings: {
                  workspaceKey: enabled ? workspace.listKeys().primarySharedKey : ''
                }
              }
            }
            """, "@secure()\nparam settings object", 0, 0);

    [TestMethod]
    public void Module_parameter_diagnostic_names_the_parameter_path()
    {
        var result = CompilationHelper.Compile(("main.bicep", """
            module child 'child.bicep' = {
              name: 'child'
              params: {
                config: {
                  password: 'public'
                }
              }
            }
            """), ("child.bicep", """
            param config {
              @secure()
              password: string
            }
            """));
        result.Diagnostics.Where(diagnostic => diagnostic.Code == UseSecureValueForSecureInputsRule.Code)
            .Should().ContainSingle().Which.Message.Should()
            .Be("Module parameter 'config.password' expects a secure value, but the value provided may not be secure.");
    }

    private static void AssertHardening(string parent, string child, string childPath = "child.bicep")
    {
        AssertFlow(parent.Replace("SECURE", ""), child, 1, 0, childPath);
        AssertFlow(parent.Replace("SECURE", "@secure()"), child, 0, 0, childPath);
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
