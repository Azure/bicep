// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class NoSecureValuesInInsecureInputsRuleTests : LinterRuleTestsBase
{
    private const string ChildModule = """
param plain string = ''

@secure()
param secret string = ''

param plainObj object = {}

@secure()
param secretObj object = {}

param plainArr array = []

type credentials = {
  username: string
  @secure()
  password: string
}

param creds credentials?

param credsList credentials[] = []

type settingsType = {
  @secure()
  password: string
  derived: string
}

param settings settingsType?
""";

    // A compiled ARM template stands in for a registry module pinned to a version whose input isn't secure yet.
    private const string CompiledChildModule = """
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "languageVersion": "2.0",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "protectedSettings": { "type": "object", "defaultValue": {} },
    "secureSettings": { "type": "secureObject", "defaultValue": {} },
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

    private const string ParentParams = """
@secure()
param parentSecret string

@secure()
param parentObj object

param parentPlain string

type credentials = {
  username: string
  @secure()
  password: string
}

param parentCreds credentials

param parentCredsList credentials[]

param parentCredsListOpt credentials[]?

""";

    private static string Message(string input, string found)
        => $"Module parameter '{input}' is not secure, but is assigned {found}. The value will be visible in the module deployment's inputs.";

    private static void CompileAndTest(string text, params string[] expectedMessages)
        => AssertLinterRuleDiagnostics(
            NoSecureValuesInInsecureInputsRule.Code,
            ParentParams + text,
            expectedMessages,
            new Options(
                IncludePosition: IncludePosition.None,
                AdditionalFiles: [("child.bicep", ChildModule), ("child.json", CompiledChildModule)]));

    [TestMethod]
    public void Secure_param_passed_to_insecure_module_param_should_warn()
        => CompileAndTest("""
module child 'child.bicep' = {
  params: {
    plain: parentSecret
    plainObj: parentObj
  }
}
""",
            Message("plain", "secure value 'parentSecret'"),
            Message("plainObj", "secure value 'parentObj'"));

    [TestMethod]
    public void Secure_values_passed_to_secure_module_params_should_not_warn()
        => CompileAndTest("""
module child 'child.bicep' = {
  params: {
    secret: parentSecret
    plain: parentPlain
    creds: parentCreds
    credsList: parentCredsList
  }
}

module child2 'child.bicep' = {
  params: {
    creds: {
      username: parentCreds.username
      password: parentCreds.password
    }
    plain: parentCreds.username
  }
}
""");

    [TestMethod]
    public void Derived_secure_values_should_warn()
        => CompileAndTest("""
var passThrough = parentSecret
var derived = 'prefix-${parentSecret}'

module child 'child.bicep' = {
  params: {
    plain: 'prefix-${parentSecret}'
    plainObj: {
      nested: parentObj.key
    }
    plainArr: [
      passThrough
      derived
    ]
  }
}

module child2 'child.bicep' = {
  params: {
    plain: empty(parentPlain) ? parentSecret : ''
  }
}
""",
            Message("plain", "secure value 'parentSecret'"),
            Message("plainObj.nested", "secure value 'parentObj.key'"),
            Message("plainArr[0]", "secure value 'passThrough'"),
            Message("plainArr[1]", "secure value 'parentSecret'"),
            Message("plain", "secure value 'parentSecret'"));

    [TestMethod]
    public void Secure_udt_property_forwarded_through_plain_object_should_warn()
        => CompileAndTest("""
module child 'child.bicep' = {
  params: {
    plainObj: parentCreds
    plainArr: parentCredsList
  }
}
""",
            Message("plainObj.password", "secure value 'parentCreds.password'"),
            Message("plainArr[*].password", "secure value 'parentCredsList[*].password'"));

    [TestMethod]
    public void Projections_should_be_checked_against_the_sink_item_type()
        => CompileAndTest("""
module preserved 'child.bicep' = {
  params: {
    credsList: map(parentCredsList, c => {
      username: c.username
      password: c.password
    })
  }
}

module leaked 'child.bicep' = {
  params: {
    plainArr: map(parentCredsList, c => c.password)
  }
}
""",
            Message("plainArr[*]", "secure value 'c.password'"));

    [TestMethod]
    public void List_functions_passed_to_insecure_module_param_should_warn()
        => CompileAndTest("""
resource storageAccount 'Microsoft.Storage/storageAccounts@2024-01-01' existing = {
  name: 'mystorageaccount'
}

module child 'child.bicep' = {
  params: {
    plain: storageAccount.listKeys().keys[0].value
    secret: storageAccount.listKeys().keys[0].value
  }
}
""",
            Message("plain", "function 'listKeys'"));

    [TestMethod]
    public void Misplaced_getSecret_calls_should_only_report_the_compiler_error()
        => CompileAndTest("""
resource kv 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: 'kv'
}

module child 'child.bicep' = {
  params: {
    plain: empty(parentPlain) ? kv.getSecret('a') : parentSecret
    plainObj: {
      nested: kv.getSecret('b')
    }
    secret: kv.getSecret('c')
  }
}
""",
            """Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator.""",
            Message("plain", "secure value 'parentSecret'"),
            """Function "getSecret" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator.""");

    [TestMethod]
    public void Secure_value_passed_to_compiled_module_with_insecure_param_should_warn()
        => CompileAndTest("""
module child 'child.json' = {
  params: {
    protectedSettings: parentObj
    secureSettings: parentObj
  }
}
""",
            Message("protectedSettings", "secure value 'parentObj'"));

    [TestMethod]
    public void Module_loops_should_be_checked()
        => CompileAndTest("""
module children 'child.bicep' = [for i in range(0, 2): {
  name: 'child${i}'
  params: {
    plain: parentSecret
  }
}]
""",
            Message("plain", "secure value 'parentSecret'"));

    [TestMethod]
    public void Shared_variable_dependencies_should_be_analyzed_once()
    {
        var variables = string.Join('\n', Enumerable.Range(1, 20).Select(i => $"var v{i} = '${{v{i - 1}}}${{v{i - 1}}}'"));
        CompileAndTest($$"""
var v0 = 'prefix-${parentSecret}'
{{variables}}

module child 'child.bicep' = {
  params: {
    plain: v20
  }
}
""",
            Message("plain", "secure value 'parentSecret'"));
    }

    [TestMethod]
    public void Derived_secrets_should_be_followed_through_property_access()
        => CompileAndTest("""
var cfg = {
  token: 'prefix-${parentSecret}'
  name: 'name'
}

module leaked 'child.bicep' = {
  params: {
    plain: cfg.token
  }
}

module sibling 'child.bicep' = {
  params: {
    plain: cfg.name
  }
}
""",
            Message("plain", "secure value 'parentSecret'"));

    [TestMethod]
    public void Derived_secrets_should_be_found_beside_secure_members()
        => CompileAndTest("""
var mixed = {
  password: parentSecret
  derived: 'prefix-${parentSecret}'
}

module child 'child.bicep' = {
  params: {
    settings: mixed
  }
}
""",
            Message("settings.derived", "secure value 'parentSecret'"));

    [TestMethod]
    public void Structured_expressions_should_be_compared_against_the_sink_path()
        => CompileAndTest("""
module ternary 'child.bicep' = {
  params: {
    creds: empty(parentPlain) ? { username: 'admin', password: parentSecret } : null
  }
}

module filtered 'child.bicep' = {
  params: {
    credsList: filter(parentCredsList, c => c.username != '')
  }
}
""");

    [TestMethod]
    public void Loops_and_lambdas_over_optional_arrays_should_keep_secure_item_properties()
        => CompileAndTest("""
module looped 'child.bicep' = [for (c, i) in (parentCredsListOpt ?? []): {
  name: 'looped-${i}'
  params: {
    plain: c.password
    creds: c
  }
}]

module mapped 'child.bicep' = {
  params: {
    plainArr: map(parentCredsListOpt ?? [], c => c.password)
    credsList: map(parentCredsListOpt ?? [], c => {
      username: c.username
      password: c.password
    })
  }
}
""",
            Message("plain", "secure value 'c.password'"),
            Message("plainArr[*]", "secure value 'c.password'"));

    [TestMethod]
    public void Opaque_expressions_should_only_follow_the_members_they_read()
        => CompileAndTest("""
var cfg = {
  token: 'prefix-${parentSecret}'
  name: 'public'
}

var mixed = {
  password: parentSecret
  derived: 'prefix-${parentSecret}'
}

module sibling 'child.bicep' = {
  params: {
    plain: '${cfg.name}-suffix'
  }
}

module derived 'child.bicep' = {
  params: {
    plainObj: {
      value: '${mixed.derived}-suffix'
    }
  }
}
""",
            Message("plainObj.value", "secure value 'parentSecret'"));

    [TestMethod]
    public void Tuple_positions_should_be_checked_individually()
        => CompileAndTest("""
var securePair = [parentSecret, 'public']

module secureFirst 'child.json' = {
  params: {
    pair: [parentSecret, 'public']
  }
}

module secureFirstVariable 'child.json' = {
  params: {
    pair: securePair
  }
}

module secureSecond 'child.json' = {
  params: {
    pair: ['public', parentSecret]
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
            Message("pair[1]", "secure value 'parentSecret'"));

    [TestMethod]
    public void Values_under_computed_keys_should_be_checked()
        => CompileAndTest("""
module child 'child.bicep' = {
  params: {
    plainObj: {
      '${parentPlain}': parentSecret
    }
  }
}
""",
            Message("plainObj.*", "secure value 'parentSecret'"));

    [TestMethod]
    public void Property_paths_should_be_case_sensitive()
        => CompileAndTest("""
var cfg = {
  Password: 'prefix-${parentSecret}'
  password: 'public'
}

module child 'child.bicep' = {
  params: {
    plain: cfg.password
  }
}
""");

    [TestMethod]
    public void Secrets_in_computed_property_names_should_require_a_secure_container()
        => CompileAndTest("""
var keyed = {
  '${parentSecret}': 'public'
}

var nested = {
  credentials: keyed
  public: 'public'
}

module direct 'child.bicep' = {
  params: {
    plainObj: {
      '${parentSecret}': 'public'
    }
    secretObj: {
      '${parentSecret}': 'public'
    }
  }
}

module indirect 'child.bicep' = {
  params: {
    plainObj: nested
    secretObj: nested
    plain: nested.public
  }
}

module valueOnly 'child.bicep' = {
  params: {
    plain: keyed[parentSecret]
  }
}
""",
            Message("plainObj", "secure value 'parentSecret'"),
            Message("plainObj.credentials", "secure value 'parentSecret'"));

    [TestMethod]
    public void Secure_members_of_function_results_should_be_checked()
        => CompileAndTest("""
func makeCredentials() credentials => {
  username: 'admin'
  password: 'from-helper'
}

module member 'child.bicep' = {
  params: {
    plain: makeCredentials().password
  }
}

module whole 'child.bicep' = {
  params: {
    plainObj: makeCredentials()
    creds: makeCredentials()
  }
}
""",
            Message("plain", "secure value 'makeCredentials().password'"),
            Message("plainObj.password", "secure value 'makeCredentials().password'"));

    [TestMethod]
    public void Members_of_secure_function_result_containers_should_be_checked()
        => CompileAndTest("""
@secure()
type secretObject = object

type wrapped = {
  nested: secretObject
}

func make() secretObject => {
  token: 'secret'
}

func makeWrapped() wrapped => {
  nested: {
    token: 'secret'
  }
}

module child 'child.bicep' = {
  params: {
    plain: make().token
    plainObj: {
      token: makeWrapped().nested.token
    }
    secret: make().token
  }
}
""",
            Message("plain", "secure value 'make().token'"),
            Message("plainObj.token", "secure value 'makeWrapped().nested.token'"));

    [TestMethod]
    public void Overwritten_spread_values_should_not_be_treated_as_secrets()
        => CompileAndTest("""
var cfg = {
  token: 'prefix-${parentSecret}'
}

var replaced = {
  ...cfg
  token: 'public'
}

module direct 'child.bicep' = {
  params: {
    plainObj: {
      ...cfg
      token: 'public'
    }
  }
}

module indirect 'child.bicep' = {
  params: {
    plainObj: replaced
  }
}

module retained 'child.bicep' = {
  params: {
    plainObj: {
      token: 'public'
      ...cfg
    }
  }
}
""",
            Message("plainObj.token", "secure value 'parentSecret'"));

    [TestMethod]
    [DataRow("cfg.nested.token", "cfg.nested.name")]
    [DataRow("cfg['nested']['token']", "cfg['nested']['name']")]
    [DataRow("(cfg.nested)!.token", "(cfg.nested)!.name")]
    [DataRow("(cfg).nested.?token", "(cfg).nested.?name")]
    public void Access_chains_preserve_derived_secrets_without_tainting_siblings(string secretAccess, string publicAccess)
        => CompileAndTest($$"""
var cfg = {
  nested: {
    token: 'prefix-${parentSecret}'
    name: 'public'
  }
}

module secret 'child.bicep' = {
  params: {
    plain: '${{{secretAccess}}}-suffix'
  }
}

module public 'child.bicep' = {
  params: {
    plain: '${{{publicAccess}}}-suffix'
  }
}
""",
            Message("plain", "secure value 'parentSecret'"));

    [TestMethod]
    public void Derived_secrets_should_be_followed_through_lambda_items()
        => CompileAndTest("""
var items = [
  {
    token: 'prefix-${parentSecret}'
    name: 'public'
  }
]

module token 'child.bicep' = {
  params: {
    plainArr: map(items, item => item.token)
  }
}

module name 'child.bicep' = {
  params: {
    plainArr: map(items, item => item.name)
  }
}
""",
            Message("plainArr[*]", "secure value 'parentSecret'"));

    [TestMethod]
    [DataRow("*")]
    [DataRow("[0]")]
    [DataRow("[*]")]
    [DataRow("a.b")]
    [DataRow("")]
    public void Literal_property_names_do_not_taint_public_siblings(string name)
        => CompileAndTest($$"""
var config = {
  '{{name}}': 'prefix-${parentSecret}'
  public: 'public'
}

module child 'child.bicep' = {
  params: {
    plain: config.public
    plainArr: [config['{{name}}']]
  }
}
""",
            Message("plainArr[0]", "secure value 'parentSecret'"));

    [TestMethod]
    public void Secure_properties_with_path_punctuation_can_be_forwarded_to_matching_inputs()
    {
        const string declarations = """
type credentials = {
  @secure()
  '*': string
  @secure()
  '[0]': string
  public: string
}
param value credentials
""";
        AssertLinterRuleDiagnostics(NoSecureValuesInInsecureInputsRule.Code, declarations + """

module child 'child.bicep' = {
  params: {
    value: value
  }
}
""", [], new Options(AdditionalFiles: [("child.bicep", declarations)]));
    }

    [TestMethod]
    public void Object_union_only_reports_secret_members_that_survive()
        => CompileAndTest("""
module child 'child.bicep' = {
  params: {
    plainObj: union({
      password: parentSecret
      nested: {
        retained: parentSecret
        replaced: parentSecret
      }
      items: [parentSecret]
    }, {
      password: 'public'
      nested: {
        replaced: 'public'
      }
      items: []
    })
  }
}
""",
            Message("plainObj.nested.retained", "secure value 'parentSecret'"));

    [TestMethod]
    public void Object_union_keeps_secrets_when_replacement_is_not_proven()
        => CompileAndTest("""
param optional { password: string? }
param unknown object

module child 'child.bicep' = {
  params: {
    plainObj: union({ password: parentSecret }, optional)
    plainArr: [
      union({ password: parentSecret }, unknown)
      union({ password: parentSecret }, empty(parentPlain) ? { password: 'public' } : {})
      union({ settings: parentObj }, { settings: { public: 'public' } })
      union({ password: 'public' }, { password: parentSecret })
    ]
  }
}
""",
            Message("plainObj.password", "secure value 'parentSecret'"),
            Message("plainArr[0].password", "secure value 'parentSecret'"),
            Message("plainArr[1].password", "secure value 'parentSecret'"),
            Message("plainArr[2].settings", "secure value 'parentObj'"),
            Message("plainArr[3].password", "secure value 'parentSecret'"));

    [TestMethod]
    public void Array_union_and_concat_do_not_overwrite_earlier_secrets()
        => CompileAndTest("""
module unioned 'child.bicep' = {
  params: {
    plainArr: union([parentSecret], ['public'])
  }
}

module concatenated 'child.bicep' = {
  params: {
    plainArr: concat([parentSecret], ['public'])
  }
}
""",
            Message("plainArr[*]", "secure value 'parentSecret'"),
            Message("plainArr[*]", "secure value 'parentSecret'"));
}
