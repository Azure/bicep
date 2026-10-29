// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests.Utils;
using Bicep.Testing;
using Bicep.Testing.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class NoSecretsInResourcePropertiesRuleTests : LinterRuleTestsBase
{
    private const string Parameters = """
@secure()
param secret string

param plain string

""";

    private static readonly Options RuleOptions = new(
        IncludePosition: IncludePosition.None,
        ConfigurationPatch: configuration => TestConfigurationBuilder.Create(configuration)
            .WithAnalyzer(NoSecretsInResourcePropertiesRule.Code, DiagnosticLevel.Warning)
            .Build());

    private static string Message(string property, string source)
        => $"Resource property '{property}' is not marked secure, but is assigned {source}. The value may be exposed by the resource provider. Resource type definitions do not always mark sensitive properties as secure.";

    private static void CompileAndTest(string text, params string[] expected)
        => AssertLinterRuleDiagnostics(NoSecretsInResourcePropertiesRule.Code, Parameters + text, expected, RuleOptions);

    [TestMethod]
    public void Rule_is_off_by_default()
    {
        new NoSecretsInResourcePropertiesRule().DefaultDiagnosticLevel.Should().Be(DiagnosticLevel.Off);
        AssertLinterRuleDiagnostics(NoSecretsInResourcePropertiesRule.Code, Parameters + """
resource account 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: 'account'
  tags: {
    exposed: secret
  }
}
""", [], new Options(IncludePosition: IncludePosition.None));
    }

    [TestMethod]
    [DataRow("2023-05-01", "2023-05-01")]
    [DataRow("2026-04-01", "2026-07-01")]
    public void Container_group_repro_reports_both_sources(string storageVersion, string containerVersion)
        => CompileAndTest($$"""
param aciContainerName string
param storageAccountName string

resource storageAccount 'Microsoft.Storage/storageAccounts@{{storageVersion}}' existing = {
  name: storageAccountName
}

resource container 'Microsoft.ContainerInstance/containerGroups@{{containerVersion}}' = {
  name: aciContainerName
  properties: {
    sku: secret
    priority: storageAccount.listKeys().keys[0].value
    containers: [
      {
        name: 'container'
        properties: {
          image: 'mcr.microsoft.com/azuredocs/aci-helloworld:latest'
        }
      }
    ]
  }
}
""",
            Message("container.properties.sku", "secure value 'secret'"),
            Message("container.properties.priority", "function 'listKeys'"));

    [TestMethod]
    public void Nested_arrays_distinguish_secure_and_plain_properties()
        => CompileAndTest("""
resource script 'Microsoft.Resources/deploymentScripts@2023-08-01' = {
  name: 'script'
  location: 'westus'
  kind: 'AzureCLI'
  properties: {
    azCliVersion: '2.0.81'
    scriptContent: 'echo hello'
    retentionInterval: 'PT1H'
    environmentVariables: [
      {
        name: 'PLAIN'
        value: secret
      }
      {
        name: 'SECURE'
        secureValue: secret
      }
    ]
  }
}
""", Message("script.properties.environmentVariables[0].value", "secure value 'secret'"));

    [TestMethod]
    public void Unmarked_sensitive_properties_are_reported_when_opted_in()
        => CompileAndTest("""
resource extension 'Microsoft.Compute/virtualMachines/extensions@2023-09-01' = {
  name: 'vm/extension'
  properties: {
    publisher: 'publisher'
    type: 'extension'
    typeHandlerVersion: '1.0'
    protectedSettings: {
      nested: {
        values: [
          secret
        ]
      }
    }
    settings: {
      password: secret
    }
  }
}
""",
            Message("extension.properties.protectedSettings.nested.values[0]", "secure value 'secret'"),
            Message("extension.properties.settings.password", "secure value 'secret'"));

    [TestMethod]
    public void Secure_parent_protects_all_nested_values()
    {
        var loader = TestTypeHelper.CreateResourceTypeLoaderWithTypes([
            TestTypeHelper.CreateCustomResourceType("Test.Rp/secure", "2023-01-01", TypeSymbolValidationFlags.Default,
                new NamedTypeProperty("protectedSettings", LanguageConstants.SecureObject),
                new NamedTypeProperty("settings", LanguageConstants.Object)),
        ]);
        AssertLinterRuleDiagnostics(NoSecretsInResourcePropertiesRule.Code, Parameters + """
resource example 'Test.Rp/secure@2023-01-01' = {
  name: 'example'
  properties: {
    protectedSettings: {
      nested: {
        values: [
          secret
        ]
      }
    }
    settings: {
      password: secret
    }
  }
}
""", [Message("example.properties.settings.password", "secure value 'secret'")],
            RuleOptions with { AzResourceTypeLoader = loader });
    }

    [DataRow("settings", "settings.exposed")]
    [DataRow("{ ...settings }", "settings.exposed")]
    [DataRow("union({}, settings)", "settings.exposed")]
    [DataRow("true ? settings : {}", "settings.exposed")]
    [TestMethod]
    public void Whole_objects_keep_secret_paths(string expression, string source)
        => CompileAndTest($$"""
var settings = {
  exposed: secret
  safe: plain
}

resource account 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: 'account'
  tags: {{expression}}
}
""", Message("account.tags.exposed", $"secure value '{source}'"));

    [TestMethod]
    public void Overwritten_spread_properties_do_not_flow()
        => CompileAndTest("""
var settings = {
  exposed: secret
}

resource account 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: 'account'
  tags: {
    ...settings
    exposed: plain
  }
}
""");

    [DataRow("[for item in [secret]: { name: 'VALUE', value: item }]", "item")]
    [DataRow("map([secret], item => { name: 'VALUE', value: item })", "item")]
    [TestMethod]
    public void Loop_and_lambda_items_keep_secret_paths(string expression, string source)
        => CompileAndTest($$"""
resource script 'Microsoft.Resources/deploymentScripts@2023-08-01' = {
  name: 'script'
  location: 'westus'
  kind: 'AzureCLI'
  properties: {
    azCliVersion: '2.0.81'
    scriptContent: 'echo hello'
    retentionInterval: 'PT1H'
    environmentVariables: {{expression}}
  }
}
""", Message("script.properties.environmentVariables[*].value", $"secure value '{source}'"));

    [TestMethod]
    public void Conditional_resource_collections_are_checked()
        => CompileAndTest("""
resource accounts 'Microsoft.Storage/storageAccounts@2023-05-01' = [for i in range(0, 2): if (true) {
  name: 'account${i}'
  tags: {
    exposed: secret
  }
}]
""", Message("accounts.tags.exposed", "secure value 'secret'"));

    [TestMethod]
    public void Existing_resources_are_not_writes()
        => CompileAndTest("""
resource account 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
  name: secret
}
""");

    [TestMethod]
    public void Unknown_resource_types_are_not_assumed_insecure()
        => CompileAndTest("""
resource unknown 'Contoso.Unknown/widgets@2023-01-01' = {
  name: 'unknown'
  properties: {
    exposed: secret
  }
}
""");

    [TestMethod]
    public void Plain_values_are_not_reported()
        => CompileAndTest("""
resource account 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: 'account'
  tags: {
    value: plain
    literal: 'value'
  }
}
""");

    [TestMethod]
    public void Enabling_warning_does_not_change_compilation_or_emitted_template()
    {
        var source = Parameters + """
resource account 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: 'account'
  tags: {
    exposed: secret
  }
}
""";
        var disabled = TestCompiler.ForInMemoryCompilation().CompileWithoutRestore(source);
        var enabled = TestCompiler.ForInMemoryCompilation()
            .WithConfiguration(TestConfigurationBuilder.Create()
                .WithAnalyzer(NoSecretsInResourcePropertiesRule.Code, DiagnosticLevel.Warning).Build())
            .CompileWithoutRestore(source);

        disabled.Diagnostics.Should().NotContain(diagnostic => diagnostic.IsError());
        enabled.Diagnostics.Should().NotContain(diagnostic => diagnostic.IsError());
        enabled.Diagnostics.Where(diagnostic => diagnostic.Code == NoSecretsInResourcePropertiesRule.Code)
            .Should().ContainSingle();
        enabled.Template.Should().NotBeNull();
        enabled.Template?.ToString().Should().Be(disabled.Template?.ToString());
        enabled.Diagnostics.Where(diagnostic => diagnostic.Code != NoSecretsInResourcePropertiesRule.Code)
            .Should().BeEquivalentTo(disabled.Diagnostics);
    }
}
