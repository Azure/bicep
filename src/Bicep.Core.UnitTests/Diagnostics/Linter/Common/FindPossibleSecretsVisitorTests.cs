// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.Linter.Common
{
    [TestClass]
    public class FindPossibleSecretsVisitorTests
    {
        private void CompileAndTest(string bicepText, string[] expectedFoundMessages)
        {
            using (var scope = new AssertionScope())
            {
                scope.AddReportable("bicepText", bicepText);

                var result = CompilationHelper.Compile(bicepText);
                var semanticModel = result.Compilation.GetEntrypointSemanticModel();

                // Look for an output - that's what we'll use in the test
                var output = result.BicepFile.ProgramSyntax.Children.OfType<OutputDeclarationSyntax>()
                    .Should().HaveCount(1, "Each testcase should contain a single output with an expression to test")
                    .And.Subject.First();

                var secrets = FindPossibleSecretsVisitor.FindPossibleSecretsInExpression(semanticModel, output.Value);
                secrets.Select(s => s.FoundMessage).Should().BeEquivalentTo(expectedFoundMessages);
            }
        }

        [DataRow(@" // This is a failing example from the docs
            @secure()
            param secureParam string

            output badResult string = 'this is the value ${secureParam}'
        ",
            "secure parameter 'secureParam'"
        )]
        [DataRow(@"
            @secure()
            param secureParam string

            output test object = {
                value1: {
                    value2: 'this is the value ${secureParam}'
                }
            }
        ",
            "secure parameter 'secureParam'"
        )]
        [DataRow(@"
            @secure()
            param secureParam object = {
              value: 'hello'
            }

            output test string = 'this is the value ${secureParam.value}'
        ",
            "secure parameter 'secureParam'"
        )]
        [DataRow(@"
            param nonSecureParam object = {
              value: 'hello'
            }

            output test string = 'this is the value ${nonSecureParam.value}'
        "
        )]
        [DataRow(@"
                param storageName string

                resource stg 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
                  name: storageName
                }

                output test object = {
                  value: stg.listKeys().keys[0].value
                }
            ",
                "function 'listKeys'"
            )]
        [DataRow(@"
                param storageName string

                resource stg 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
                  name: storageName
                }

                output test object = {
                  value: stg.listAnything().keys[0].value
                }
            ",
                "function 'listAnything'"
            )]
        [DataRow(@"
                param storageName string

                var v = {}

                output test = v.listAnything().keys[0].value
            "
            )]
        /* TODO: blocked by https://github.com/Azure/bicep/issues/4833
        [DataRow(@"
                param storageName string

                resource stg 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
                  name: storageName
                }

                var storage = stg

                output test object = {
                  value: storage.listAnything().keys[0].value
                }
            ",
            "Outputs should not contain secrets. function 'listAnything'"
        )]*/
        [DataRow(
            @"
                param storageName string

                resource stg 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
                    name: storageName
                }

                output test object = listKeys(resourceId('Microsoft.Storage/storageAccounts', 'storageName'), '2021-02-01')
            ",
                "function 'listKeys'"
        )]
        [DataRow(
            @"
                param storageName string

                resource stg 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
                  name: storageName
                }

                output badResult object = listAnything(resourceId('Microsoft.Storage/storageAccounts', 'storageName'), '2021-02-01')
            ",
            "function 'listAnything'"
        )]
        [DataRow(
            @"
                param storageName string

                resource stg 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
                  name: storageName
                }

                output badResult object = az.listAnything(resourceId('Microsoft.Storage/storageAccounts', 'storageName'), '2021-02-01')
            ",
            "function 'listAnything'"
        )]
        [DataTestMethod]
        public void Test(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, expectedMessages);
        }
    }
}
