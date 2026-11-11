// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Configuration;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class OutputsShouldNotContainSecretsRuleTests : LinterRuleTestsBase
    {
        private void CompileAndTest(string text, OnCompileErrors onCompileErrors, string[] expectedMessages, Func<IBicepConfiguration, IBicepConfiguration>? configurationPatchFunc = null)
        {
            AssertLinterRuleDiagnostics(OutputsShouldNotContainSecretsRule.Code, text, expectedMessages, new Options(onCompileErrors, IncludePosition.None, ConfigurationPatch: configurationPatchFunc));
        }

        const string description = "Outputs should not contain secrets.";

        [TestMethod]
        [DataRow("length(secret)")]
        [DataRow("sys.length(secret)")]
        [DataRow("length(settings)")]
        public void Secret_length_can_be_output_directly_or_indirectly(string expression)
            => CompileAndTest($$"""
                @secure()
                @minLength(2)
                @maxLength(20)
                param secret string
                @secure()
                param settings object
                var count = {{expression}}
                var values = { count: count }
                output n int = {{expression}}
                output indirect int = values.count
                output text string = '${count}'
                """, OnCompileErrors.IncludeErrors, []);

        [DataRow(@" // This is a failing example from the docs
            @secure()
            param secureParam string

            output badResult string = 'this is the value ${secureParam}'
        ",
            $"{description} Found possible secret: secure value 'secureParam'"
        )]
        [DataRow(@"
            @secure()
            param secureParam string

            output badResult object = {
                value1: {
                    value2: 'this is the value ${secureParam}'
                }
            }
        ",
            $"{description} Found possible secret: secure value 'secureParam'"
        )]
        [DataRow(@"
            @secure()
            param secureParam object = {
              value: 'hello'
            }

            output badResult string = 'this is the value ${secureParam.value}'
        ",
            // TTK output:
            // [-] Outputs Must Not Contain Secrets
            // Output contains secureObject parameter: badResult
            $"{description} Found possible secret: secure value 'secureParam'"
        )]
        [DataRow(@"
            @secure()
            param secureParam string

            var indirection = secureParam

            output badResult string = 'this is the value ${indirection}'
        ",
            $"{description} Found possible secret: secure value 'indirection'"
        )]
        [TestMethod]
        public void If_OutputReferencesSecureParam_ShouldFail(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, OnCompileErrors.IncludeErrors, expectedMessages);
        }

        [DataRow(@"
            param obj {
                @secure()
                property: string
            }

            output badResult string = 'this is the value ${obj.property}'
        ",
            $"{description} Found possible secret: secure value 'obj.property'"
        )]
        [DataRow(@"
            param obj {
                @secure()
                *: string
            }

            output badResult string = 'this is the value ${obj.property}'
        ",
            $"{description} Found possible secret: secure value 'obj.property'"
        )]
        [DataRow(@"
            @secure()
            type secureString = string
            param arr secureString[]

            output badResult string = 'this is the value ${arr[0]}'
        ",
            $"{description} Found possible secret: secure value 'arr[0]'"
        )]
        [DataRow(@"
            param arr [
                @secure()
                string
                string
            ]

            output badResult string = 'this is the value ${arr}'
        ",
            $"{description} Found possible secret: secure value 'arr[0]'"
        )]
        [DataRow(@"
            param arr [
                @secure()
                string
                string
            ]
            param idx int

            output badResult string = 'this is the value ${arr[idx]}'
        ",
            $"{description} Found possible secret: secure value 'arr[idx]'"
        )]
        [DataRow(@"
            type recursiveType = {
                @secure()
                secureProp: string
                recur: recursiveType?
            }
            param obj recursiveType

            output badResult string = 'this is the value ${obj}'
        ",
            $"{description} Found possible secret: secure value 'obj.secureProp'"
        )]
        [TestMethod]
        public void If_OutputReferencesSecureParamProperty_ShouldFail(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, OnCompileErrors.IncludeErrors, expectedMessages);
        }

        [DataRow(@"
            param obj {
                @secure()
                property: string
            }

            output badResult string = 'this is the value ${obj}'
        ",
            $"{description} Found possible secret: secure value 'obj.property'"
        )]
        [DataRow(@"
            param obj {
                @secure()
                *: string
            }

            output badResult string = 'this is the value ${obj}'
        ",
            $"{description} Found possible secret: secure value 'obj.*'"
        )]
        [DataRow(@"
            @secure()
            type secureString = string
            param arr secureString[]

            output badResult string = 'this is the value ${arr}'
        ",
            $"{description} Found possible secret: secure value 'arr[*]'"
        )]
        [DataRow(@"
            @secure()
            type secureString = string
            param arr [string, secureString]

            output badResult string = 'this is the value ${arr}'
        ",
            $"{description} Found possible secret: secure value 'arr[1]'"
        )]
        [DataRow(@"
            param p {
              prop: {
                @secure()
                nestedSecret: string
                nestedInnocuousProperty: string
              }
            }

            output badResult object = p
        ",
            $"{description} Found possible secret: secure value 'p.prop.nestedSecret'"
        )]
        [DataRow(@"
            param p {
              prop: {
                @secure()
                nestedSecret: string
                nestedInnocuousProperty: string
              }
            }

            output badResult object = p.prop
        ",
            $"{description} Found possible secret: secure value 'p.prop.nestedSecret'"
        )]
        [DataRow(@"
            param p {
              prop: {
                @secure()
                nestedSecret: string
                nestedInnocuousProperty: string
              }
            }

            output badResult string = p.prop.nestedSecret
        ",
            $"{description} Found possible secret: secure value 'p.prop.nestedSecret'"
        )]
        [TestMethod]
        public void If_OutputReferencesParamWithSecureProperty_ShouldFail(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, OnCompileErrors.IncludeErrors, expectedMessages);
        }

        [DataRow(@"
            param obj {
                id: int
                @secure()
                property: string
            }

            output badResult string = 'this is the value ${obj.id}'
        ")]
        [DataRow(@"
            param obj {
                id: int
                @secure()
                *: string
            }

            output badResult string = 'this is the value ${obj.id}'
        ")]
        [DataRow(@"
            @secure()
            type secureString = string
            param arr [string, secureString]

            output badResult string = 'this is the value ${arr[0]}'
        ")]
        [DataRow(@"
            param p {
              prop: {
                @secure()
                nestedSecret: string
                nestedInnocuousProperty: string
              }
            }

            output badResult string = p.prop.nestedInnocuousProperty
        "
        )]
        [TestMethod]
        public void If_OutputReferencesNonSecureParamProperty_ShouldPass(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, OnCompileErrors.IncludeErrors, expectedMessages);
        }

        [DataRow(@"
            param secureParam object = {
              value: 'hello'
            }

            output badResult string = 'this is the value ${secureParam}'
        "
        )]
        [DataRow(@"
            param secureParam string

            output badResult object = {
                value1: {
                    value2: 'this is the value ${secureParam}'
                }
            }
        "
        )]
        [TestMethod]
        public void If_ParamNotSecure_ShouldPass(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, OnCompileErrors.IncludeErrors, expectedMessages);
        }

        [DataRow(@"
            param storageName string

            resource stg 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
              name: storageName
            }

            output badResult object = {
              value: stg.listKeys().keys[0].value
            }
        ",
            $"{description} Found possible secret: function 'listKeys'"
        )]
        [DataRow(@"
            param storageName string

            resource stg 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
              name: storageName
            }

            output badResult object = {
              value: stg.listAnything().keys[0].value
            }
        ",
            $"{description} Found possible secret: function 'listAnything'"
        )]
        [DataRow(@"
            param storageName string

            var v = {}

            output badResult object = {
            value: v.listAnything().keys[0].value // variable is not a resource, so no failure
            }
        "
        )]
        [TestMethod]
        public void If_ListFunctionInOutput_AsResourceMethod_ShouldPass(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, OnCompileErrors.Ignore, expectedMessages);
        }

        [DataRow(@"
            param storageName string

            resource stg 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
              name: storageName
            }

            var storage = stg

            output badResult object = {
              value: storage.listAnything().keys[0].value
            }
        ",
            $"{description} Found possible secret: function 'listAnything'"
        )]
        [TestMethod]
        public void If_ListFunctionInOutput_AsResourceMethod_ThroughVariable_ShouldFail(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, OnCompileErrors.Ignore, expectedMessages);
        }

        [DataRow(@"
            param storageName string

            resource stg 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
              name: storageName
            }

            output badResult object = listKeys(resourceId('Microsoft.Storage/storageAccounts', 'storageName'), '2021-02-01')
        ",
            // TTK output:
            // [-] Outputs Must Not Contain Secrets(6 ms)
            // Output contains secret: badResult
            $"{description} Found possible secret: function 'listKeys'"
        )]
        [DataRow(@"
            param storageName string

            resource stg 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
              name: storageName
            }

            output badResult object = listAnything(resourceId('Microsoft.Storage/storageAccounts', 'storageName'), '2021-02-01')
        ",
            // TTK output:
            // [-] Outputs Must Not Contain Secrets(6 ms)
            // Output contains secret: badResult
            $"{description} Found possible secret: function 'listAnything'"
        )]
        [TestMethod]
        public void If_ListFunctionInOutput_AsStandaloneFunction_ShouldFail(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, OnCompileErrors.Ignore, expectedMessages);
        }

        [DataRow(@"
            param storageName string

            resource stg 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
              name: storageName
            }

            output badResult object = az.listAnything(resourceId('Microsoft.Storage/storageAccounts', 'storageName'), '2021-02-01')
        ",
            // TTK output:
            // [-] Outputs Must Not Contain Secrets(6 ms)
            // Output contains secret: badResult
            $"{description} Found possible secret: function 'listAnything'"
        )]
        [TestMethod]
        public void If_ListFunctionInOutput_AsAzInstanceFunction_ShouldFail(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, OnCompileErrors.Ignore, expectedMessages);
        }

        [DataRow(@"
            output badResultPassword string = 'hello'
        ",
            // TTK output:
            // [-] Outputs Must Not Contain Secrets(6 ms)
            //  Output name suggests secret: badResultPassword
            $"{description} Found possible secret: output name 'badResultPassword' suggests a secret"
        )]
        [DataRow(@"
            output possiblepassword string = 'hello'
        ",
            $"{description} Found possible secret: output name 'possiblepassword' suggests a secret"
        )]
        [DataRow(@"
            output password string = 'hello'
        ",
            $"{description} Found possible secret: output name 'password' suggests a secret"
        )]
        [DataRow(@"
            output passwordNumber1 string = 'hello'
        ",
            $"{description} Found possible secret: output name 'passwordNumber1' suggests a secret"
        )]
        [TestMethod]
        public void If_OutputNameLooksLikePassword_ShouldFail(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, OnCompileErrors.Ignore, expectedMessages);
        }

        [TestMethod]
        [DataRow("{ name: 'public', password: secret, ...extra }", 0)]
        [DataRow("{ name: secret, ...extra }", 1)]
        [DataRow("{ name: 'public', '${key}': secret }", 1)]
        [DataRow("{ name: 'public', ...json(secret) }", 1)]
        [DataRow("{ name: 'public', ...{ password: secret, ...extra } }", 0)]
        [DataRow("{ name: 'public', ...extra, ...{ name: 'replaced' } }", 0)]
        [DataRow("{ ...{ name: secret }, ...extra, name: 'replaced' }", 0)]
        [DataRow("{ ...{ name: secret }, ...container.settings }", 0)]
        [DataRow("union({ name: 'public', password: secret }, extra)", 0)]
        [DataRow("sys.union({ name: 'public', password: secret }, extra)", 0)]
        [DataRow("union({ name: secret }, extra)", 1)]
        [DataRow("union({ name: 'public' }, json(secret))", 1)]
        [DataRow("union({ name: secret }, extra, { name: 'replaced' })", 0)]
        [DataRow("union({ name: 'public' }, { '${key}': secret })", 1)]
        public void Uncertain_property_reads_only_follow_values_that_can_supply_the_property(string initializer, int warnings)
        {
            var result = CompilationHelper.Compile($$"""
                @secure()
                param secret string
                param extra object
                param key string
                var container = { settings: { name: 'replaced' } }
                var original = {{initializer}}
                var alias = original
                output result string = alias.name
                """);

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Diagnostics.Count(diagnostic => diagnostic.Code == OutputsShouldNotContainSecretsRule.Code).Should().Be(warnings);
        }

        [TestMethod]
        [DataRow("{ password: secret, sku: 'S' }", "sku", 0)]
        [DataRow("{ password: secret, sku: 'S' }", "password", 1)]
        [DataRow("union({ password: secret, sku: 'S' }, overrides)", "sku", 0)]
        [DataRow("union({ password: secret, sku: 'S' }, overrides)", "password", 1)]
        public void Inline_and_aliased_property_reads_report_each_secret_once(string initializer, string property, int warnings)
        {
            var result = CompilationHelper.Compile($$"""
                @secure()
                param secret string
                param overrides { name: string }
                var cfg = {{initializer}}
                output inline string = ({{initializer}}).{{property}}
                output aliased string = cfg.{{property}}
                """);

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Diagnostics.Count(diagnostic => diagnostic.Code == OutputsShouldNotContainSecretsRule.Code).Should().Be(2 * warnings);
        }

        [TestMethod]
        [DataRow("union(cfg, extra)", "password", 1)]
        [DataRow("union(extra, cfg)", "password", 1)]
        [DataRow("union(cfg, extra)", "name", 0)]
        [DataRow("union(extra, cfg)", "name", 0)]
        [DataRow("union(cfg, extra, { password: 'public' })", "password", 0)]
        [DataRow("union({ password: secret }, extra, cfg)", "name", 0)]
        [DataRow("union(cfg, extra)", "nested.password", 1)]
        [DataRow("union(cfg, extra)", "nested.name", 0)]
        [DataRow("union(cfg, extra, { nested: { name: 'public' } })", "nested.password", 1)]
        [DataRow("union(cfg, extra, { nested: { password: 'public' } })", "nested.password", 0)]
        [DataRow("union(cfg, extra, { nested: 'public' })", "nested", 0)]
        [DataRow("union(cfg, { nested: 'public' }, { nested: { name: 'public' } }, extra)", "nested.?password", 0)]
        [DataRow("union(cfg, extra)", "optional.?password", 1)]
        [DataRow("union(cfg, extra)", "optional.?name", 0)]
        [DataRow("union(cfg, extra, { optional: { password: 'public' } })", "optional.?password", 0)]
        [DataRow("union(cfg, extra)", "names[0]", 0)]
        [DataRow("union(cfg, extra)", "items[0].name", 0)]
        [DataRow("union(cfg, extra)", "items[0].password", 1)]
        public void Union_preserves_selected_secure_parameter_members_without_tainting_siblings(string initializer, string property, int warnings)
        {
            var result = CompilationHelper.Compile($$"""
                @secure()
                param secret string
                param cfg {
                  name: string
                  @secure()
                  password: string
                  nested: {
                    name: string
                    @secure()
                    password: string
                  }
                  optional: {
                    name: string
                    @secure()
                    password: string
                  }?
                  names: string[]
                  items: {
                    name: string
                    @secure()
                    password: string
                  }[]
                }
                param extra object
                var merged = {{initializer}}
                var alias = merged
                output inline string? = ({{initializer}}).{{property}}
                output aliased string? = alias.{{property}}
                """);

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Diagnostics.Count(diagnostic => diagnostic.Code == OutputsShouldNotContainSecretsRule.Code).Should().Be(2 * warnings);
        }

        [TestMethod]
        [DataRow("union({ nested: { value: secret } }, extra, { nested: { name: 'public' } })", "secret")]
        [DataRow("union({ nested: { value: secret } }, { nested: { name: 'public' }, ...json('{}') })", "cfg.nested.value")]
        public void Uncertain_union_properties_preserve_recursively_merged_secrets(string initializer, string secretName)
            => CompileAndTest($$"""
                @secure()
                param secret string
                param extra object
                var cfg = {{initializer}}
                output result string = cfg.nested.value
                """, OnCompileErrors.IncludeErrors, [$"{description} Found possible secret: secure value '{secretName}'"]);

        [TestMethod]
        public void Union_with_a_discriminated_object_preserves_recursively_merged_secrets()
            => CompileAndTest("""
                @secure()
                param secret string
                @discriminator('kind')
                type Settings = { kind: 'a', x: string } | { kind: 'b', y: string }
                param settings Settings
                var cfg = union({ nested: { value: secret } }, { nested: settings })
                output result string = cfg.nested.value
                """, OnCompileErrors.IncludeErrors, [$"{description} Found possible secret: secure value 'secret'"]);

        [TestMethod]
        [DataRow("union(secureObj, { extra: 1 })")]
        [DataRow("union({ extra: 1 }, typedSecureObj)")]
        [DataRow("union(enabled ? secureObj : {}, { extra: 1 })")]
        public void Union_with_a_secure_object_is_secure(string expression)
            => CompileAndTest($$"""
                @secure()
                param secureObj object
                @secure()
                param typedSecureObj {
                  flag: bool
                }
                param enabled bool
                var merged = {{expression}}
                output o object = merged
                """, OnCompileErrors.IncludeErrors, [$"{description} Found possible secret: secure value 'merged'"]);

        [TestMethod]
        [DataRow("""
            @discriminator('kind')
            param p { kind: 'a', x: string } | { kind: 'b', y: string }
            """)]
        [DataRow("param p resourceInput<'Microsoft.DataFactory/factories/linkedservices@2018-06-01'>.properties")]
        public void Secure_discriminated_object_is_secure(string declaration)
            => CompileAndTest($"""
                @secure()
                {declaration}
                output o object = p
                """, OnCompileErrors.IncludeErrors, [$"{description} Found possible secret: secure value 'p'"]);

        [TestMethod]
        public void Secure_values_forwarded_through_object_properties_are_still_found()
            => CompileAndTest("""
                @secure()
                param secret string
                var original = {
                  direct: secret
                  interpolated: '${secret}'
                  public: 'not secret'
                }
                var alias = original
                var chainedAlias = alias
                output direct string = chainedAlias.direct
                output indexed string = chainedAlias['direct']
                output interpolated string = chainedAlias.interpolated
                output public string = chainedAlias.public
                output whole object = chainedAlias
                """, OnCompileErrors.IncludeErrors, [
                    $"{description} Found possible secret: secure value 'chainedAlias.direct'",
                    $"{description} Found possible secret: secure value 'chainedAlias['direct']'",
                    $"{description} Found possible secret: secure value 'chainedAlias.interpolated'",
                    $"{description} Found possible secret: secure value 'chainedAlias.direct'",
                    $"{description} Found possible secret: secure value 'chainedAlias.interpolated'",
                ]);

        [TestMethod]
        [DataRow("chained.number", "int", 1)]
        [DataRow("chained['number']", "int", 1)]
        [DataRow("chained.flag", "bool", 1)]
        [DataRow("chained.text", "string", 1)]
        [DataRow("chained.interpolated", "string", 1)]
        [DataRow("chained.nested.number", "int", 1)]
        [DataRow("chained.public", "string", 0)]
        [DataRow("chained", "object", 1)]
        public void Copied_secure_object_members_are_still_found(string expression, string type, int warnings)
            => CompileAndTest($$"""
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
                output result {{type}} = {{expression}}
                """, OnCompileErrors.IncludeErrors,
                Enumerable.Repeat($"{description} Found possible secret: secure value 'secureObj'", warnings).ToArray());

        [TestMethod]
        [DataRow("split(secret, ',')[0]", 1)]
        [DataRow("parts[0]", 1)]
        [DataRow("cfg.password", 1)]
        [DataRow("wrapped[0].value", 1)]
        [DataRow("merged.name", 0)]
        [DataRow("spreadConfig.other[0]", 1)]
        [DataRow("overwritten.password", 0)]
        [DataRow("spreadJson.x[0]", 1)]
        [DataRow("passwords[key]", 1)]
        [DataRow("spreadSibling.user", 1)]
        [DataRow("dynamicKey.user", 1)]
        [DataRow("dynamicSpread.user", 1)]
        [DataRow("typedSpread.user", 1)]
        public void Reads_through_variables_whose_types_hide_secrets_are_found(string expression, int warnings)
            => CompileAndTest($$"""
                @secure()
                param secret string
                @secure()
                param secureObj object
                param key string
                @sealed()
                type Closed = { name: string }
                var typed Closed = json(secret)
                var passwords = { dev: secret, prod: secret }
                var spreadSibling = { password: secret, ...json(secret) }
                var dynamicKey = { ...{ user: 'public' }, '${key}': secret }
                var dynamicSpread = { user: 'public', ...{ '${key}': secret } }
                var typedSpread = { user: 'public', ...typed }
                var parts = split(secret, ',')
                var cfg = json(secret)
                var wrapped = map(parts, p => { value: p })
                var merged = union({ password: secret }, { name: 'public' })
                var overrides = { other: split(secret, ',') }
                var spreadConfig = { password: secureObj, ...overrides }
                var overwritten = { password: secret, ...{ password: 'public' } }
                var spreadJson = { ...json(secret) }
                output result string = {{expression}}
                """, OnCompileErrors.IncludeErrors,
                Enumerable.Repeat($"{description} Found possible secret: secure value 'secret'", warnings).ToArray());
    }
}
