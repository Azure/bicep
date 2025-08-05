// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// TODO: Test with different configs
namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class NoHardcodedEnvironmentUrlsRuleTests : LinterRuleTestsBase
    {
        [DataRow(0, @"
        param password string
        var sum = 1 + 3
        output sub int = sum
        ")]
        [DataRow(0, @"
        param param1 string
        var location = 'somehost.com'
        output sub int = sum
        ")]
        [DataRow(1, @"
        param param1 string
        var location = 'management.core.windows.net'
        output sub int = sum
        ")]
        [DataRow(1, @"
        param param1 string
        var location = 'http://management.core.windows.net'
        output sub int = sum
        ")]
        [DataRow(0, @"
        param param1 string
        var location = 'http://schema.management.azure.com'
        output sub int = sum
        ")]
        [DataRow(1, @"
        param param1 string
        var location = 'https://management.core.windows.net'
        output sub int = sum
        ")]
        [DataRow(0, @"
        param param1 string
        var location = 'https://schema.management.azure.com'
        output sub int = sum
        ")]
        [DataRow(0, @"
        param param1 string
        var location = 'https://zzzzzz.schema.management.azure.com'
        output sub int = sum
        ")]
        [DataRow(1, @"
        param param1 string
        var location = 'http://MANAGEMENT.core.windows.net'
        output sub int = sum
        ")]
        [DataRow(1, @"
        resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
          name: 'name'
          location: resourceGroup().location
          sku: {
            name: 'azuredatalakestore.net'
            capacity: 1
          }
        }
        ")]
        [DataRow(0, @"
        resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
          name: 'name'
          location: resourceGroup().location
          sku: {
            name: 'http://schema.management.azure.com'
            capacity: 1
          }
        }
        ")]
        [DataTestMethod]
        public void Simple(int diagnosticCount, string text)
        {
            AssertLinterRuleDiagnostics(NoHardcodedEnvironmentUrlsRule.Code, text, diagnosticCount, new Options(OnCompileErrors.Ignore));
        }

        [DataRow(1, @"
        param p1 string
        param p2 string
        var a = '${p1}azuredatalakestore.net${p2}'
        ")]
        [DataRow(1, @"
        param p1 string
        param p2 string
        var a = '${p1} azuredatalakestore.net${p2}'
        ")]
        [DataRow(1, @"
        param p1 string
        param p2 string
        var a = '${p1} azuredatalakestore.net$ {p2}'
        ")]
        [DataTestMethod]
        public void InsideStringInterpolation(int diagnosticCount, string text)
        {
            AssertLinterRuleDiagnostics(NoHardcodedEnvironmentUrlsRule.Code, text, diagnosticCount);
        }

        [DataRow(1, @"var a = 'azuredatalakestore.net' + 1")]
        [DataRow(2, @"var a = 'azuredatalakestore.net azuredatalakestore.net' + 1")]
        [DataRow(1, @"
        param p1 string
        param p2 string
        var a = concat('${p1}${'azuredatalakestore.net'}${p2}', 'foo')
        ")]
        [DataRow(0, @"
        param p1 string
        param p2 string
        var a = concat('${p1}${'https://schema.management.azure.com'}${p2}', 'foo')
        ")]
        [DataRow(2, @"
        param p1 string
        param p2 string
        var a = concat('${p1}${'azuredatalakestore.net'}${p2}${'management.azure.com'}-${'schema.management.azure.com'}', 'foo')
        ")]
        [DataTestMethod]
        public void InsideExpressions(int diagnosticCount, string text)
        {
            AssertLinterRuleDiagnostics(NoHardcodedEnvironmentUrlsRule.Code, text, diagnosticCount, new Options(OnCompileErrors.Ignore));
        }

        [DataTestMethod]
        // valid matches (i.e., linter rule fails)
        [DataRow("aschema.management.azure.com", true)]
        [DataRow("azure.aschema.management.azure.com", true)]
        [DataRow("management.azure.com", true)]
        [DataRow("http://management.azure.com", true)]
        [DataRow("https://management.azure.com", true)]
        [DataRow("subdomain1.management.azure.com", true)]
        [DataRow("http://subdomain1.management.azure.com", true)]
        [DataRow("https://subdomain1.management.azure.com", true)]
        // should not match (i.e., linter rule passes)
        [DataRow("azure.schema.management.azure.com", false)]
        [DataRow("othermanagement.azure.com", false)]
        [DataRow("azure.schema.mannnnagement.azure.com", false)]
        [DataRow("management.azzzzure.com", false)]
        [DataRow("http://management.azzzzure.com", false)]
        [DataRow("https://management.azzzzure.com", false)]
        [DataRow("subdomain1.management.azzzure.com", false)]
        [DataRow("http://subdomain1.manageeeement.azure.com", false)]
        [DataRow("https://subdomain1.managemeeeent.azure.com", false)]
        [DataRow("asazure.windows.net/servers}", false)]
        [DataRow("${'location'}.asazure.windows.net/servers/${'aasServerName'}", false)]
        public void DisallowedHostsMatchingTest(string testString, bool isMatch)
        {
            AssertLinterRuleDiagnostics(NoHardcodedEnvironmentUrlsRule.Code, @$"output str string = '{testString}'", diags =>
            {
                if (isMatch)
                {
                    diags.Should().HaveCount(1);
                    diags.Select(d => d.Code).Should().AllBe(NoHardcodedEnvironmentUrlsRule.Code);
                }
                else
                {
                    diags.Should().BeEmpty();
                }
            });
        }

        [DataTestMethod]
        // valid matches (i.e. it will be excluded and there should be no linter failures)
        [DataRow("schema.management.azure.com", true)]
        [DataRow("http://schema.management.azure.com", true)]
        [DataRow("https://schema.management.azure.com", true)]
        [DataRow("subany.schema.management.azure.com", true)]
        [DataRow("http://subany.schema.management.azure.com", true)]
        [DataRow("https://subany.schema.management.azure.com", true)]
        [DataRow("https://subany.SCHEMA.MANAGEMENT.Azure.Com", true)]
        [DataRow("all the world is a stage, but subdomain1.schema.management.azure.com should not be hardcoded", true)]
        [DataRow("rasazure.windows.net/servers}", true)]
        // should not match (i.e. it will not be excluded and there *should* be linter failures)
        [DataRow("aschema.management.azure.com", false)]
        [DataRow("azure.aschema.management.azure.com", false)]
        [DataRow("management.azure.com", false)]
        [DataRow("http://management.azure.com", false)]
        [DataRow("https://management.azure.com", false)]
        [DataRow("subdomain1.management.azure.com", false)]
        [DataRow("http://subdomain1.management.azure.com", false)]
        [DataRow("https://subdomain1.management.azure.com", false)]
        [DataRow("all the world is a stage, but subdomain1.management.azure.com should not be hardcoded", false)]
        [DataRow("all the world is a stage, but subdomain1.1schema.management.azure.com should not be hardcoded", false)]
        public void ExcludedHostsMatchingTest(string testString, bool isMatch)
        {
            AssertLinterRuleDiagnostics(NoHardcodedEnvironmentUrlsRule.Code, @$"output str string = '{testString}'", diags =>
            {
                if (isMatch)
                {
                    diags.Should().BeEmpty();
                }
                else
                {
                    diags.Should().HaveCount(1);
                    diags.Select(d => d.Code).Should().AllBe(NoHardcodedEnvironmentUrlsRule.Code);
                }
            });
        }
    }
}
