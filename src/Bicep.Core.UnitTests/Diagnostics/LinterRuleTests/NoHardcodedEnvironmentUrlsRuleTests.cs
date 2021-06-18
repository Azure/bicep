// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

// TODO: Test with different configs
namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class NoHardcodedEnvironmentUrlsRuleTests : LinterRuleTestsBase
    {
        private void CompileAndTest(string text, int expectedDiagnosticCount)
        {
            using (new AssertionScope($"linter errors for this code:\n{text}\n"))
            {
                var errors = GetDiagnostics(NoHardcodedEnvironmentUrlsRule.Code, text);
                errors.Should().HaveCount(expectedDiagnosticCount);
            }
        }

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
        [DataRow(1, @"
param param1 string
var location = 'https://management.core.windows.net'
output sub int = sum
")]
        [DataRow(1, @"
param param1 string
var location = 'http://MANAGEMENT.core.windows.net'
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
        [DataTestMethod]
        public void Simple(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }

        [DataRow(1, @"
param p1 string
param p2 string
var a = '${p1}azuredatalakestore.net${p2}'
")]
        [DataRow(1, @"
param p1 string
param p2 string
var a = '${p1}azuredatalakestore.net${p2}'
")]
        [DataTestMethod]
        public void InsideStringInterpolation(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }

        [DataRow(1, @"
var a = 'azuredatalakestore.net' + 1
")]
        [DataRow(1, @"
param p1 string
param p2 string
var a = concat('${p1}${'azuredatalakestore.net'}${p2}', 'foo')
")]
        [DataTestMethod]
        public void InsideExpressions(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }

        [DataTestMethod]
        [DataRow(@"azure.microsoft.com", 0, 0)]
        [DataRow(@"https://azure.microsoft.com", 0, 0)]
        [DataRow(@"https:/schema/azure.microsoft.com", 0, 0)]
        [DataRow(@"There is more string here https:/schema/azure.microsoft.com with following string here", 0, 0)]
        [DataRow(@"gallery.azure.com", 1, 0)]
        [DataRow(@"https://gallery.azure.com", 1, 0)]
        [DataRow(@"https://schema.gallery.azure.com", 1, 1)]
        [DataRow(@"There is more string here gallery.azure.com with following string here", 1, 0)]
        [DataRow(@"There is more string here https://gallery.azure.com with following string here", 1, 0)]
        [DataRow(@"There is more string here https://schema.gallery.azure.com with following string here", 1, 1)]
        public void DisallowedHostsRegexTest(string host, int hostsToFind, int schemasToFind)
        {
            var disallowedHosts = new[] {
                "management.core.windows.net",
                "gallery.azure.com",
                "management.core.windows.net",
                "management.azure.com",
                "database.windows.net",
                "core.windows.net",
                "login.microsoftonline.com",
                "graph.windows.net",
                "graph.windows.net",
                "vault.azure.net",
                "datalake.azure.net",
                "azuredatalakestore.net",
                "azuredatalakeanalytics.net",
                "vault.azure.net",
                "api.loganalytics.io",
                "api.loganalytics.iov1",
                "asazure.windows.net",
                "region.asazure.windows.net",
                "api.loganalytics.iov1",
                "api.loganalytics.io",
                "asazure.windows.net",
                "region.asazure.windows.net",
                "batch.core.windows.net"};

            var regexMatchStr = string.Join('|', disallowedHosts);
            var regexMatch = new Regex(regexMatchStr, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var schemaMatchStr = @"https://schema\.";
            var schemaRegex = new Regex(schemaMatchStr, RegexOptions.Compiled | RegexOptions.RightToLeft | RegexOptions.IgnoreCase);

            int hostCt = 0;
            int schemaCt = 0;

            // Walk thru each host reference found 
            foreach (Match match in regexMatch.Matches(host))
            {
                hostCt++;
                //and see if it's preceeded by a schema.
                var schemaMatch = schemaRegex.Match(host, match.Index);

                // schema is found immediately preceeding this host match
                bool schemaFound = schemaMatch.Success && (schemaMatch.Index + schemaMatch.Length) == match.Index;
                if (schemaFound)
                {
                    schemaCt++;
                }
            }

            Assert.AreEqual(hostsToFind, hostCt, "Host count mismatch");
            Assert.AreEqual(schemasToFind, schemaCt, "Schema count mismatch");

            //Assert.IsTrue(regexWild.IsMatch("https://www.azure.microsoft.com"));
            //Assert.IsTrue(regexWild.IsMatch("www.azure.microsoft.com"));
            //Assert.IsTrue(regexWild.IsMatch("azure.microsoft.com"));

            //var regexExact = new Regex(@"(http(s)?://)?azure\.microsoft\.com");
            //Assert.IsFalse(regexExact.IsMatch("https://www.azure.microsoft.com"));
            //Assert.IsFalse(regexExact.IsMatch("www.azure.microsoft.com"));
            //Assert.IsTrue(regexExact.IsMatch("azure.microsoft.com"));
            //Assert.IsTrue(regexExact.IsMatch("https://azure.microsoft.com"));
            //Assert.IsTrue(regexExact.IsMatch("https://azure.microsoft.com/lions/tigers/bears"));
        }
    }
}

