// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Testing;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

                var result = TestCompiler.ForInMemoryCompilation().CompileWithoutRestore(bicepText);
                var semanticModel = result.Compilation.GetEntrypointSemanticModel();

                // Look for an output - that's what we'll use in the test
                var output = ((BicepFile)result.EntryPointFile).ProgramSyntax.Children.OfType<OutputDeclarationSyntax>()
                    .Should().HaveCount(1, "Each testcase should contain a single output with an expression to test")
                    .And.Subject.First();

                var secrets = FindPossibleSecretsVisitor.FindPossibleSecretsInExpression(semanticModel, output.Value);
                secrets.Select(s => s.FoundMessage).Should().BeEquivalentTo(expectedFoundMessages);
            }
        }

        [TestMethod]
        [DataRow("alias.number")]
        [DataRow("alias['number']")]
        [DataRow("alias.interpolated")]
        [DataRow("alias")]
        public void Followed_findings_point_to_the_read_not_the_variable_declaration(string expression)
        {
            var result = TestCompiler.ForInMemoryCompilation().CompileWithoutRestore($$"""
                @secure()
                param secureObj { number: int }
                var original = { number: secureObj.number, interpolated: '${secureObj.number}' }
                var alias = original
                output result = {{expression}}
                """);
            var output = ((BicepFile)result.EntryPointFile).ProgramSyntax.Children.OfType<OutputDeclarationSyntax>().Single();
            var secrets = FindPossibleSecretsVisitor.FindPossibleSecretsInExpression(result.Compilation.GetEntrypointSemanticModel(), output.Value).ToArray();
            secrets.Should().NotBeEmpty();
            foreach (var secret in secrets)
            {
                secret.FoundMessage.Should().Be("secure value 'secureObj'");
                secret.Syntax.Span.Position.Should().BeGreaterThanOrEqualTo(output.Value.Span.Position);
                (secret.Syntax.Span.Position + secret.Syntax.Span.Length).Should()
                    .BeLessThanOrEqualTo(output.Value.Span.Position + output.Value.Span.Length);
            }
        }

        [DataRow(@" // This is a failing example from the docs
            @secure()
            param secureParam string

            output badResult string = 'this is the value ${secureParam}'
        ",
            "secure value 'secureParam'"
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
            "secure value 'secureParam'"
        )]
        [DataRow(@"
            @secure()
            param secureParam object = {
              value: 'hello'
            }

            output test string = 'this is the value ${secureParam.value}'
        ",
            "secure value 'secureParam'"
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
            "function 'listAnything'"
        )]
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
        [DataRow(@"
                @secure()
                param secret string

                var cyclic = {
                  value: cyclic.value
                }

                output test string = cyclic.value
            "
        )]
        [TestMethod]
        public void Test(string text, params string[] expectedMessages)
        {
            CompileAndTest(text, expectedMessages);
        }
    }
}
