// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class OutputsShouldNotContainSecretsRuleTests : LinterRuleTestsBase
    {
        private void CompileAndTest(string text, OnCompileErrors onCompileErrors, string[] expectedMessages)
        {
            AssertLinterRuleDiagnostics(OutputsShouldNotContainSecretsRule.Code, text, expectedMessages, onCompileErrors);
        }

        [DataRow(@" // This is a failing example from the docs
            @secure()
            param secureParam string

            output badResult string = 'this is the value ${secureParam}'
        ",
            "Don't include secrets in an output. Found: 'secureParam'"
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
            "Don't include secrets in an output. Found: 'secureParam'"
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
            "Don't include secrets in an output. Found: 'secureParam'"
        )]
        [DataTestMethod]
        public void If_OutputReferencesSecureParam_ShouldFail(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, OnCompileErrors.Fail, expectedMessages);
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
            CompileAndTest(text, OnCompileErrors.Fail, expectedMessages);
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
            "Don't include secrets in an output. Found: 'listKeys'"
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
            "Don't include secrets in an output. Found: 'listAnything'"
        )]
        [DataTestMethod]
        public void If_ListFunctionInOutput_AsResourceMethod_ShouldFail(string text, params string[] expectedMessages)
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
            "Don't include secrets in an output. Found: 'listKeys'"
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
            "Don't include secrets in an output. Found: 'listAnything'"
        )]
        [DataTestMethod]
        public void If_ListFunctionInOutput_AsStandaloneFunction_ShouldFail(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, OnCompileErrors.Ignore, expectedMessages);
        }

        [DataRow(@"
            output badResultPassword string = 'hello'
        ",
            // TTK output:
            // [-] Outputs Must Not Contain Secrets(6 ms)
            //  Output name suggests secret: badResultPassword
            "Don't include secrets in an output. Found: 'badResultPassword'"
        )]
        [DataRow(@"
            output possiblepassword string = 'hello'
        ",
            "Don't include secrets in an output. Found: 'possiblepassword'"
        )]
        [DataRow(@"
            output password string = 'hello'
        ",
            "Don't include secrets in an output. Found: 'password'"
        )]
        [DataRow(@"
            output passwordNumber1 string = 'hello'
        ",
            "Don't include secrets in an output. Found: 'passwordNumber1'"
        )]
        [DataTestMethod]
        public void If_OutputNameLooksLikePassword_ShouldFail(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, OnCompileErrors.Ignore, expectedMessages);
        }
    }
}
