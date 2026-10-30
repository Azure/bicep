// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
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

    private const string ChildModule = """
@secure()
param secret string = ''

param plain string = ''

type credentials = {
  username: string
  @secure()
  password: string
}

param creds credentials?

param credsList credentials[] = []

@secure()
param secureObj object = {}

@discriminator('kind')
type tokenUnion = tokenA | tokenB

type tokenA = {
  kind: 'a'
  @secure()
  token: string
}

type tokenB = {
  kind: 'b'
  @secure()
  token: string
}

param tokens tokenUnion?

type optionalCredentials = {
  username: string
  @secure()
  password: string?
}

param optionalCreds optionalCredentials?
""";

    [TestMethod]
    [DataRow("choice ? secret : plain", 1)]
    [DataRow("choice ? secret : ''", 0)]
    [DataRow("choice ? storage.listKeys().keys[0].value : plain", 1)]
    [DataRow("choice ? storage.listKeys().keys[0].value : secret", 0)]
    public void Resource_inputs_check_each_branch_without_rejecting_runtime_values(string expression, int expected)
        => CompileAndTest($$"""
param choice bool
param plain string
@secure()
param secret string

resource storage 'Microsoft.Storage/storageAccounts@2024-01-01' existing = {
  name: 'storage'
}

resource server 'Microsoft.DBforMySQL/flexibleServers@2023-12-30' = {
  name: 'server'
  location: 'westus'
  properties: {
    administratorLoginPassword: {{expression}}
  }
}
""", expected);

    private void CompileAndTestWithChild(string text, params string[] expectedMessages)
        => AssertLinterRuleDiagnostics(
            UseSecureValueForSecureInputsRule.Code,
            text,
            expectedMessages,
            new Options(AdditionalFiles: [("child.bicep", ChildModule)]));

    [TestMethod]
    public void Module_param_object_union_only_checks_members_that_survive()
        => AssertLinterRuleDiagnostics(UseSecureValueForSecureInputsRule.Code, """
@secure()
param secret string
param plain string

module protected 'child.bicep' = {
  params: {
    creds: union({ username: 'admin', password: plain }, { password: secret })
  }
}

module unprotected 'child.bicep' = {
  params: {
    creds: union({ username: 'admin', password: secret }, { password: plain })
  }
}
""",
            ["Property 'creds.password' expects a secure value, but the value provided may not be secure."],
            new Options(IncludePosition: IncludePosition.None, AdditionalFiles: [("child.bicep", ChildModule)]));

    [TestMethod]
    public void Module_param_expecting_secure_value_should_warn_for_literal()
        => CompileAndTestWithChild("""
module child 'child.bicep' = {
  params: {
    secret: 'hardCodedValue'
  }
}
""",
            "[3] Property 'secret' expects a secure value, but the value provided may not be secure.");

    [TestMethod]
    public void Module_param_expecting_secure_value_should_warn_for_insecure_param()
        => CompileAndTestWithChild("""
param parentValue string

module child 'child.bicep' = {
  params: {
    secret: parentValue
    secureObj: { key: parentValue }
  }
}
""",
            "[5] Property 'secret' expects a secure value, but the value provided may not be secure.",
            "[6] Property 'secureObj' expects a secure value, but the value provided may not be secure.");

    [TestMethod]
    public void Module_param_expecting_secure_value_should_not_warn_for_secure_or_empty_values()
        => CompileAndTestWithChild("""
@secure()
param parentSecret string

@secure()
param parentObj object

param plainValue string

module child 'child.bicep' = {
  params: {
    secret: parentSecret
    plain: plainValue
    secureObj: parentObj
  }
}

module empty 'child.bicep' = {
  params: {
    secret: ''
    secureObj: {}
  }
}
""");

    [TestMethod]
    public void Module_param_expecting_secure_value_should_not_warn_for_runtime_values()
        => CompileAndTestWithChild("""
resource storageAccount 'Microsoft.Storage/storageAccounts@2024-01-01' existing = {
  name: 'mystorageaccount'
}

module child 'child.bicep' = {
  params: {
    secret: storageAccount.listKeys().keys[0].value
  }
}
""");

    [TestMethod]
    public void Module_param_should_warn_for_insecure_value_in_secure_udt_property()
        => CompileAndTestWithChild("""
param parentPassword string

module child 'child.bicep' = {
  params: {
    creds: {
      username: 'admin'
      password: parentPassword
    }
    credsList: [
      {
        username: 'admin'
        password: 'hardCodedValue'
      }
    ]
  }
}
""",
            "[7] Property 'creds.password' expects a secure value, but the value provided may not be secure.",
            "[12] Property 'credsList[0].password' expects a secure value, but the value provided may not be secure.");

    [TestMethod]
    public void Module_param_should_not_warn_for_secure_udt_forwarded_whole()
        => CompileAndTestWithChild("""
type credentials = {
  username: string
  @secure()
  password: string
}

param parentCreds credentials

@secure()
param parentObj object

module child 'child.bicep' = {
  params: {
    creds: parentCreds
    credsList: [parentCreds]
  }
}

module child2 'child.bicep' = {
  params: {
    creds: {
      username: parentCreds.username
      password: parentCreds.password
    }
  }
}
""");

    [TestMethod]
    public void Module_param_should_warn_for_plain_object_forwarded_to_udt_with_secure_property()
        => CompileAndTestWithChild("""
param parentCreds object

module child 'child.bicep' = {
  params: {
    creds: parentCreds
  }
}
""",
            "[5] Property 'creds.password' expects a secure value, but the value provided may not be secure.");

    [TestMethod]
    public void Module_param_should_check_each_branch_of_fallbacks()
        => CompileAndTestWithChild("""
@secure()
param parentSecret string = ''

param parentPlain string = ''

@secure()
param parentObj object

module secureFallbacks 'child.bicep' = {
  params: {
    secret: parentObj.?secret ?? parentSecret
  }
}

module secureOrEmpty 'child.bicep' = {
  params: {
    secret: empty(parentSecret) ? parentObj.?secret : ''
  }
}

module insecureFallback 'child.bicep' = {
  params: {
    secret: parentObj.?secret ?? parentPlain
  }
}
""",
            "[23] Property 'secret' expects a secure value, but the value provided may not be secure.");

    [TestMethod]
    public void Module_loop_param_expecting_secure_value_should_warn_for_insecure_value()
        => CompileAndTestWithChild("""
param names string[]

module children 'child.bicep' = [for name in names: {
  name: name
  params: {
    secret: name
  }
}]
""",
            "[6] Property 'secret' expects a secure value, but the value provided may not be secure.");

    // A compiled ARM template stands in for a registry or template spec module.
    private const string CompiledChildModule = """
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "languageVersion": "2.0",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "secret": { "type": "securestring", "defaultValue": "" },
    "settings": {
      "type": "object",
      "properties": {
        "name": { "type": "string" },
        "token": { "type": "securestring" }
      },
      "nullable": true
    },
    "pair": {
      "type": "array",
      "prefixItems": [
        { "type": "securestring" },
        { "type": "string" }
      ],
      "items": false,
      "defaultValue": []
    }
  },
  "resources": {}
}
""";

    [TestMethod]
    public void Compiled_module_param_expecting_secure_value_should_warn_for_insecure_value()
        => AssertLinterRuleDiagnostics(
            UseSecureValueForSecureInputsRule.Code,
            """
param parentToken string

@secure()
param parentSecret string

module child 'child.json' = {
  params: {
    secret: parentSecret
    settings: {
      name: 'settings'
      token: parentToken
    }
  }
}
""",
            ["[11] Property 'settings.token' expects a secure value, but the value provided may not be secure."],
            new Options(AdditionalFiles: [("child.json", CompiledChildModule)]));

    private void CompileAndTestWithChildNoPosition(string text, params string[] expectedMessages)
        => AssertLinterRuleDiagnostics(
            UseSecureValueForSecureInputsRule.Code,
            text,
            expectedMessages,
            new Options(IncludePosition: IncludePosition.None, AdditionalFiles: [("child.bicep", ChildModule)]));

    private static string Message(string property)
        => $"Property '{property}' expects a secure value, but the value provided may not be secure.";

    [TestMethod]
    public void Module_param_should_check_discriminated_unions_with_secure_properties()
        => CompileAndTestWithChildNoPosition("""
param parentPlain string

module child 'child.bicep' = {
  params: {
    tokens: {
      kind: 'a'
      token: parentPlain
    }
  }
}
""",
            Message("tokens.token"));

    [TestMethod]
    public void Module_param_runtime_branch_should_not_mask_insecure_branch()
        => CompileAndTestWithChildNoPosition("""
param parentPlain string

resource storageAccount 'Microsoft.Storage/storageAccounts@2024-01-01' existing = {
  name: 'mystorageaccount'
}

module child 'child.bicep' = {
  params: {
    secret: empty(parentPlain) ? parentPlain : storageAccount.listKeys().keys[0].value
  }
}
""",
            Message("secret"));

    [TestMethod]
    public void Module_param_should_check_spreads_into_udt_with_secure_property()
        => CompileAndTestWithChildNoPosition("""
param parentPlain string

var cfg = {
  username: 'admin'
  password: parentPlain
}

module child 'child.bicep' = {
  params: {
    creds: {
      ...cfg
    }
  }
}
""",
            Message("creds.password"));

    [TestMethod]
    public void Module_param_should_allow_empty_or_absent_secure_members()
        => CompileAndTestWithChildNoPosition("""
param parentPlain string

var noPassword = {
  username: 'admin'
}

module ternary 'child.bicep' = {
  params: {
    creds: empty(parentPlain) ? { username: 'admin', password: '' } : null
  }
}

module absent 'child.bicep' = {
  params: {
    optionalCreds: noPassword
  }
}
""");

    [TestMethod]
    public void Module_loop_over_optional_array_should_keep_secure_item_properties()
        => CompileAndTestWithChildNoPosition("""
type cacheType = {
  name: string
  @secure()
  connectionString: string
}

param caches cacheType[]?

param plains string[]?

module secureItems 'child.bicep' = [for (cache, i) in (caches ?? []): {
  name: 'secure-${i}'
  params: {
    secret: cache.connectionString
    creds: {
      username: cache.name
      password: cache.connectionString
    }
  }
}]

module plainItems 'child.bicep' = [for (plain, i) in (plains ?? []): {
  name: 'plain-${i}'
  params: {
    secret: plain
  }
}]
""",
            Message("secret"));

    [TestMethod]
    public void Module_param_runtime_branch_in_variable_should_not_mask_insecure_branch()
        => CompileAndTestWithChildNoPosition("""
param parentPlain string

@secure()
param parentSecret string

resource storageAccount 'Microsoft.Storage/storageAccounts@2024-01-01' existing = {
  name: 'mystorageaccount'
}

var mixed = empty(parentPlain) ? parentPlain : storageAccount.listKeys().keys[0].value
var runtime = storageAccount.listKeys().keys[0].value
var inferred = empty(parentPlain) ? parentSecret : parentPlain
var cfg = {
  password: parentPlain
  token: parentSecret
}

module mixedChild 'child.bicep' = {
  params: {
    secret: mixed
  }
}

module runtimeChild 'child.bicep' = {
  params: {
    secret: runtime
  }
}

module inferredChild 'child.bicep' = {
  params: {
    secret: inferred
  }
}

module memberChild 'child.bicep' = {
  params: {
    secret: cfg.token
    creds: {
      username: 'admin'
      password: cfg.password
    }
  }
}
""",
            Message("secret"),
            Message("secret"),
            Message("creds.password"));

    [TestMethod]
    public void Module_param_tuple_positions_should_be_checked_individually()
        => AssertLinterRuleDiagnostics(
            UseSecureValueForSecureInputsRule.Code,
            """
@secure()
param parentSecret string

module secureFirst 'child.json' = {
  params: {
    pair: [parentSecret, 'public']
  }
}

module literalFirst 'child.json' = {
  params: {
    pair: ['hardCodedValue', 'public']
  }
}

module trailingSpread 'child.json' = {
  params: {
    pair: [
      parentSecret
      ...['public']
    ]
  }
}
""",
            [Message("pair[0]")],
            new Options(IncludePosition: IncludePosition.None, AdditionalFiles: [("child.json", CompiledChildModule)]));

    [TestMethod]
    public void Module_param_property_paths_should_be_case_sensitive()
        => CompileAndTestWithChildNoPosition("""
@secure()
param parentSecret string

var cfg = {
  Password: 'public'
  password: parentSecret
}

module child 'child.bicep' = {
  params: {
    secret: cfg.password
  }
}
""");

    [TestMethod]
    public void Module_param_should_ignore_overwritten_spread_values()
        => CompileAndTestWithChildNoPosition("""
@secure()
param parentSecret string

var cfg = {
  username: 'admin'
  password: 'public'
}

var replaced = {
  ...cfg
  password: parentSecret
}

module direct 'child.bicep' = {
  params: {
    creds: {
      ...cfg
      password: parentSecret
    }
  }
}

module indirect 'child.bicep' = {
  params: {
    creds: replaced
  }
}

module retained 'child.bicep' = {
  params: {
    creds: {
      username: 'admin'
      password: parentSecret
      ...cfg
    }
  }
}
""",
            Message("creds.password"));

    [TestMethod]
    public void Secure_object_input_should_only_check_surviving_spread_members()
        => CompileAndTestWithChildNoPosition("""
@secure()
param parentSecret string

var cfg = {
  password: 'public'
}

module child 'child.bicep' = {
  params: {
    secureObj: {
      ...cfg
      password: parentSecret
    }
  }
}
""");

    [TestMethod]
    public void Secure_object_input_should_keep_overwrite_masks_through_nullable_spreads()
        => CompileAndTestWithChildNoPosition("""
@secure()
param parentSecret string

param includeConfig bool

var cfg = includeConfig ? { password: 'public' } : null
var other = includeConfig ? { password: 'public', remaining: 'public' } : null

module replaced 'child.bicep' = {
  params: {
    secureObj: {
      ...(cfg ?? {})
      password: parentSecret
    }
  }
}

module retained 'child.bicep' = {
  params: {
    secureObj: {
      ...(other ?? {})
      password: parentSecret
    }
  }
}
""",
            Message("secureObj"));
}
