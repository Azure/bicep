// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.CodeAction;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class SecretsInParamsMustBeSecureTests : LinterRuleTestsBase
    {
        [TestMethod]
        public void ParameterNameInFormattedMessage()
        {
            var ruleToTest = new SecretsInParamsMustBeSecureRule();
            ruleToTest.GetMessage("myparam").Should().Be("Parameter 'myparam' may represent a secret (according to its name) and must be declared with the '@secure()' attribute.");
        }

        [TestMethod]
        public void HasFix()
        {
            var result = CompilationHelper.Compile(@"#disable-next-line no-unused-params
                param password string
            ");
            var diagnostics = result.Diagnostics;
            diagnostics.Should().HaveCount(1);
            var diag = diagnostics.First();
            diag.Message.Should().Be("Parameter 'password' may represent a secret (according to its name) and must be declared with the '@secure()' attribute.");
            diag.Code.Should().Be(SecretsInParamsMustBeSecureRule.Code);
            var fixable = diag.Should().BeAssignableTo<IBicepAnalyerFixableDiagnostic>().Which;
            fixable.Fixes.Should().HaveCount(1);
            var fix = fixable.Fixes.First();
            fix.Description.Should().Be("Mark parameter as secure");
            fix.Kind.Should().Be(CodeFixKind.QuickFix);
            fix.Replacements.Should().HaveCount(1);
            fix.Replacements.First().Text.Should().Be("@secure()\n");
        }

        private void CompileAndTest(string bicep, int numberOfExpectedErrors)
        {
            AssertLinterRuleDiagnostics(SecretsInParamsMustBeSecureRule.Code, bicep, numberOfExpectedErrors);
        }

        [DataRow(@"param pass_adminUsername string")]
        [DataRow(@"param passwort string")]
        [DataRow(@"@secure()
                   param password string")]
        [DataRow(@"param key string")]
        [DataTestMethod]
        public void ExpectingPass(string bicep)
        {
            CompileAndTest(bicep, 0);
        }

        [DataRow(@"param password string", false)]
        [DataRow(@"param password object", false)]
        [DataRow(@"param password array", true)]
        [DataRow(@"param password bool", true)]
        [DataRow(@"param password int", true)]
        [DataRow(@"@secure()
                   param password string", true)]
        [DataRow(@"@secure()
                   param password object", true)]
        [DataTestMethod]
        public void ShouldOnlyFailForStringAndObject(string bicep, bool shouldPass)
        {
            CompileAndTest(bicep, shouldPass ? 0 : 1);
        }

        // password
        [DataRow(@"param fail_adminPassword string")]
        [DataRow(@"param fail_ADMINpassword2 string")]
        [DataRow(@"param fail_adminPasswords string")]
        [DataRow(@"param fail_adminPasswordObject object")]
        // accountkey
        [DataRow(@"param fail_myAccountKey string")]
        [DataRow(@"param fail_myAcctKey string")]
        // secret
        [DataRow(@"param fail_adminSecret string")]
        [DataRow(@"param fail_mySECRET string")]
        [DataRow(@"param fail_AdminSecretValue string")]
        [TestMethod]
        public void ExpectingFail(string bicep)
        {
            CompileAndTest(bicep, 1);

            using (new AssertionScope("Adding @secure() to the testcase should make it pass"))
            {
                if (bicep.EndsWith(" string") || bicep.EndsWith(" object"))
                {
                    CompileAndTest("@secure()\n" + bicep, 0);
                }
            }
        }

        //// Exceptions allowed for certain known ARM patterns:
        //
        // secret + permissions (secret permissions is an accessPolicy property for keyvault)
        [DataRow(true, @"param pass_secretpermissions string")]
        [DataRow(false, @"param fail_secretmissions string")]
        //
        // secret + version (URL or simply the version property of a secret)
        [DataRow(true, @"param pass_secretVersion string")]
        [DataRow(false, @"param fail_secretVerse string")]
        //
        // secret + url/uri
        [DataRow(true, @"param pass_secretUri string")]
        [DataRow(true, @"param pass_secretUrl string = 'default'")]
        [DataRow(false, @"param fail_secretUrtext string = 'default'")]
        //
        // secret + name (keyvault secret's name)
        [DataRow(true, @"param pass_secretname string")]
        [DataRow(true, @"param pass_keyVaultSecretName object")]
        [DataRow(false, @"param fail_secretNombre string")]
        [DataTestMethod]
        public void AllowedListExceptions(bool shouldPass, string bicep)
        {
            CompileAndTest(bicep, shouldPass ? 0 : 1);
        }

        [TestMethod]
        public void FullExample()
        {
            string bicep = @"
                @secure()
                param stgAccountName string

                resource nested 'Microsoft.Resources/deployments@2021-04-01' = {
                  name: 'nested'
                  properties: {
                    mode: 'Incremental'
                    template: {
                      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                      contentVersion: '1.0.0.0'
                      resources: [
                        {
                          name: stgAccountName
                          type: 'Microsoft.Storage/storageAccounts'
                          apiVersion: '2021-04-01'
                          #disable-next-line no-loc-expr-outside-params
                          location: resourceGroup().location
                          kind: 'StorageV2'
                          sku: {
                            name: 'Premium_LRS'
                            tier: 'Premium'
                          }
                        }
                      ]
                    }
                  }
                }
            ";
            CompileAndTest(bicep, 0);
        }
    }
}
