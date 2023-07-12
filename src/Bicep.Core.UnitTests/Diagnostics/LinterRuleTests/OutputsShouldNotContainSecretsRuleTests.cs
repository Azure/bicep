// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class OutputsShouldNotContainSecretsRuleTests : LinterRuleTestsBase
    {
        private void CompileAndTest(string text, OnCompileErrors onCompileErrors, string[] expectedMessages, Func<RootConfiguration, RootConfiguration>? configurationPatchFunc = null)
        {
            AssertLinterRuleDiagnostics(OutputsShouldNotContainSecretsRule.Code, text, expectedMessages, new Options(onCompileErrors, IncludePosition.None, ConfigurationPatch: configurationPatchFunc));
        }

        const string description = "Outputs should not contain secrets.";

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
        [DataTestMethod]
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
        [DataTestMethod]
        public void If_OutputReferencesSecureParamProperty_ShouldFail(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, OnCompileErrors.IncludeErrors, expectedMessages, config => new(
                config.Cloud,
                config.ModuleAliases,
                config.Analyzers,
                config.CacheRootDirectory,
                config.ExperimentalFeaturesEnabled with { UserDefinedTypes = true },
                config.Formatting,
                config.ConfigurationPath,
                config.DiagnosticBuilders));
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
        [DataTestMethod]
        public void If_OutputReferencesParamWithSecureProperty_ShouldFail(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, OnCompileErrors.IncludeErrors, expectedMessages, config => new(
                config.Cloud,
                config.ModuleAliases,
                config.Analyzers,
                config.CacheRootDirectory,
                config.ExperimentalFeaturesEnabled with { UserDefinedTypes = true },
                config.Formatting,
                config.ConfigurationPath,
                config.DiagnosticBuilders));
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
        [DataTestMethod]
        public void If_OutputReferencesNonSecureParamProperty_ShouldPass(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, OnCompileErrors.IncludeErrors, expectedMessages, config => new(
                config.Cloud,
                config.ModuleAliases,
                config.Analyzers,
                config.CacheRootDirectory,
                config.ExperimentalFeaturesEnabled with { UserDefinedTypes = true },
                config.Formatting,
                config.ConfigurationPath,
                config.DiagnosticBuilders));
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
        [DataTestMethod]
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
        [DataTestMethod]
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
        [DataTestMethod]
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
        [DataTestMethod]
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
        [DataTestMethod]
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
        [DataTestMethod]
        public void If_OutputNameLooksLikePassword_ShouldFail(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, OnCompileErrors.Ignore, expectedMessages);
        }
    }
}
