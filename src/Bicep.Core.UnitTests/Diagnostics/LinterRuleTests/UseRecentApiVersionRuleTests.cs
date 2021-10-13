// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.ApiVersion;
using Bicep.Core.CodeAction;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Bicep.Core.Analyzers.Linter.Rules.UseRecentApiVersionRule;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class UseRecentApiVersionRuleTests : LinterRuleTestsBase
    {
        private void CompileAndTest(string text, params string[] useRecentApiVersions)
        {
            AssertLinterRuleDiagnostics(UseRecentApiVersionRule.Code, text, diags =>
            {
                if (useRecentApiVersions.Any())
                {
                    var rule = new UseRecentApiVersionRule();
                    string[] expectedMessages = useRecentApiVersions.Select(p => rule.GetMessage(p)).ToArray();
                    diags.Select(e => e.Message).Should().ContainInOrder(expectedMessages);
                }
                else
                {
                    diags.Should().BeEmpty();
                }
            });
        }

        private SemanticModel SemanticModel => new Compilation(TestTypeHelper.CreateEmptyProvider(),
                                                               SourceFileGroupingFactory.CreateFromText(string.Empty, BicepTestConstants.FileResolver),
                                                               BicepTestConstants.BuiltInConfiguration,
                                                               BicepTestConstants.ApiVersionProvider).GetEntrypointSemanticModel();

        [DataRow(@"
            resource dnsZone 'Microsoft.Network/dnsZones@2015-10-01-preview' = {
              name: 'name'
              location: resourceGroup().location
            }",
            "2018-05-01")]
        [DataRow(@"
            resource dnsZone 'Microsoft.Network/dnsZones@2017-10-01' = {
              name: 'name'
              location: resourceGroup().location
            }",
            "2018-05-01")]
        [DataRow(@"
            resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
              name: 'name'
              location: resourceGroup().location
            }")]
        [DataRow(@"
            resource containerRegistry 'Microsoft.ContainerRegistry/registries@2020-11-01-preview' = {
              name: 'name'
              location: resourceGroup().location
              sku: {
                name: 'Basic'
              }
            }",
            "2021-06-01-preview")]
        [DataRow(@"
            resource containerRegistry 'Microsoft.ContainerRegistry/registries@2019-05-01' = {
              name: 'name'
              location: resourceGroup().location
              sku: {
                name: 'Basic'
              }
            }")]
        [DataRow(@"
            resource containerRegistry 'Microsoft.ContainerRegistry/registries@2021-06-01-preview' = {
              name: 'name'
              location: resourceGroup().location
              sku: {
                name: 'Basic'
              }
            }")]
        [DataRow(@"
            resource appServicePlan 'Microsoft.Web/serverfarms@2021-01-01' = {
              name: 'name'
              location: resourceGroup().location
            }")]
        [DataTestMethod]
        public void TestRule(string text, params string[] useRecentApiVersions)
        {
            CompileAndTest(text, useRecentApiVersions);
        }

        [TestMethod]
        public void AddCodeFixIfGAVersionIsNotLatest_WithCurrentVersionLessThanTwoYearsOld_ShouldNotAddDiagnostics()
        {
            DateTime currentVersionDate = DateTime.Today.AddYears(-1);
            string currentVersion = ConvertDateTimeToString(currentVersionDate);

            DateTime recentGAVersionDate = DateTime.Today.AddMonths(-5);
            string recentGAVersion = ConvertDateTimeToString(recentGAVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfGAVersionIsNotLatest(new TextSpan(17, 47),
                                                     recentGAVersion,
                                                     currentVersion);

            spanFixes.Should().BeEmpty();
        }

        [TestMethod]
        public void AddCodeFixIfGAVersionIsNotLatest_WithCurrentVersionMoreThanTwoYearsOldAndRecentApiVersionIsAvailable_ShouldAddDiagnostics()
        {
            DateTime currentVersionDate = DateTime.Today.AddYears(-3);
            string currentVersion = ConvertDateTimeToString(currentVersionDate);

            DateTime recentGAVersionDate = DateTime.Today.AddMonths(-5);
            string recentGAVersion = ConvertDateTimeToString(recentGAVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfGAVersionIsNotLatest(new TextSpan(17, 47),
                                                     recentGAVersion,
                                                     currentVersion);

            spanFixes.Should().SatisfyRespectively(
                x =>
                {
                    x.Value.Description.Should().Be("Use recent api version " + recentGAVersion);
                });
        }

        [TestMethod]
        public void AddCodeFixIfGAVersionIsNotLatest_WithCurrentAndRecentApiVersionsMoreThanTwoYearsOld_ShouldAddDiagnosticsToUseRecentApiVersion()
        {
            DateTime currentVersionDate = DateTime.Today.AddYears(-4);
            string currentVersion = ConvertDateTimeToString(currentVersionDate);

            DateTime recentGAVersionDate = DateTime.Today.AddYears(-3);
            string recentGAVersion = ConvertDateTimeToString(recentGAVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfGAVersionIsNotLatest(new TextSpan(17, 47),
                                                     recentGAVersion,
                                                     currentVersion);

            spanFixes.Should().SatisfyRespectively(
                x =>
                {
                    x.Value.Description.Should().Be("Use recent api version " + recentGAVersion);
                });
        }

        [TestMethod]
        public void AddCodeFixIfGAVersionIsNotLatest_WhenCurrentAndRecentApiVersionsAreSameAndMoreThanTwoYearsOld_ShouldNotAddDiagnostics()
        {
            DateTime currentVersionDate = DateTime.Today.AddYears(-3);
            string currentVersion = ConvertDateTimeToString(currentVersionDate);

            DateTime recentGAVersionDate = currentVersionDate;
            string recentGAVersion = ConvertDateTimeToString(recentGAVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfGAVersionIsNotLatest(new TextSpan(17, 47),
                                                     recentGAVersion,
                                                     currentVersion);

            spanFixes.Should().BeEmpty();
        }

        [TestMethod]
        public void AddCodeFixIfNonGAVersionIsNotLatest_WithPreviewVersion_WhenCurrentPreviewVersionIsLatest_ShouldNotAddDiagnostics()
        {
            DateTime currentVersionDate = DateTime.Today.AddYears(-1);
            string currentVersion = ConvertDateTimeToString(currentVersionDate);

            DateTime recentGAVersionDate = DateTime.Today.AddYears(-3);
            string recentGAVersion = ConvertDateTimeToString(recentGAVersionDate);

            DateTime recentPreviewVersionDate = currentVersionDate;
            string recentPreviewVersion = ConvertDateTimeToString(recentPreviewVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfNonGAVersionIsNotLatest(new TextSpan(17, 47),
                                                        recentGAVersion,
                                                        recentPreviewVersion,
                                                        null,
                                                        ApiVersionPrefixConstants.Preview,
                                                        currentVersion);

            spanFixes.Should().BeEmpty();
        }

        [TestMethod]
        public void AddCodeFixIfNonGAVersionIsNotLatest_WithPreviewVersion_WhenRecentPreviewVersionIsAvailable_ShouldAddDiagnostics()
        {
            DateTime currentVersionDate = DateTime.Today.AddYears(-5);
            string currentVersion = ConvertDateTimeToString(currentVersionDate);

            DateTime recentGAVersionDate = DateTime.Today.AddYears(-3);
            string recentGAVersion = ConvertDateTimeToString(recentGAVersionDate);

            DateTime recentPreviewVersionDate = DateTime.Today.AddYears(-2);
            string recentPreviewVersion = ConvertDateTimeToString(recentPreviewVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfNonGAVersionIsNotLatest(new TextSpan(17, 47),
                                                        recentGAVersion,
                                                        recentPreviewVersion,
                                                        null,
                                                        ApiVersionPrefixConstants.Preview,
                                                        currentVersion);

            spanFixes.Should().SatisfyRespectively(
                x =>
                {
                    x.Value.Description.Should().Be("Use recent api version " + recentPreviewVersion + ApiVersionPrefixConstants.Preview);
                });
        }

        [TestMethod]
        public void AddCodeFixIfNonGAVersionIsNotLatest_WithPreviewVersion_WhenRecentGAVersionIsAvailable_ShouldAddDiagnostics()
        {
            DateTime currentVersionDate = DateTime.Today.AddYears(-5);
            string currentVersion = ConvertDateTimeToString(currentVersionDate);

            DateTime recentGAVersionDate = DateTime.Today.AddYears(-2);
            string recentGAVersion = ConvertDateTimeToString(recentGAVersionDate);

            DateTime recentPreviewVersionDate = DateTime.Today.AddYears(-3);
            string recentPreviewVersion = ConvertDateTimeToString(recentPreviewVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfNonGAVersionIsNotLatest(new TextSpan(17, 47),
                                                        recentGAVersion,
                                                        recentPreviewVersion,
                                                        null,
                                                        ApiVersionPrefixConstants.Preview,
                                                        currentVersion);

            spanFixes.Should().SatisfyRespectively(
                x =>
                {
                    x.Value.Description.Should().Be("Use recent api version " + recentGAVersion);
                });
        }

        [TestMethod]
        public void AddCodeFixIfNonGAVersionIsNotLatest_WithPreviewVersion_WhenRecentGAVersionIsSameAsPreviewVersion_ShouldAddDiagnosticsUsingGAVersion()
        {
            DateTime currentVersionDate = DateTime.Today.AddYears(-3);
            string currentVersion = ConvertDateTimeToString(currentVersionDate);

            DateTime recentGAVersionDate = DateTime.Today.AddYears(-2);
            string recentGAVersion = ConvertDateTimeToString(recentGAVersionDate);

            string recentPreviewVersion = recentGAVersion;

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfNonGAVersionIsNotLatest(new TextSpan(17, 47),
                                                        recentGAVersion,
                                                        recentPreviewVersion,
                                                        null,
                                                        ApiVersionPrefixConstants.Preview,
                                                        currentVersion);

            spanFixes.Should().SatisfyRespectively(
                x =>
                {
                    x.Value.Description.Should().Be("Use recent api version " + recentGAVersion);
                });
        }

        [TestMethod]
        public void AddCodeFixIfNonGAVersionIsNotLatest_WithPreviewVersion_WhenGAVersionisNull_AndCurrentVersionIsNotRecent_ShouldAddDiagnosticsUsingRecentPreviewVersion()
        {
            DateTime currentVersionDate = DateTime.Today.AddYears(-3);
            string currentVersion = ConvertDateTimeToString(currentVersionDate);

            DateTime recentPreviewVersionDate = DateTime.Today.AddYears(-2);
            string recentPreviewVersion = ConvertDateTimeToString(recentPreviewVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfNonGAVersionIsNotLatest(new TextSpan(17, 47),
                                                        null,
                                                        recentPreviewVersion,
                                                        null,
                                                        ApiVersionPrefixConstants.Preview,
                                                        currentVersion);

            spanFixes.Should().SatisfyRespectively(
                x =>
                {
                    x.Value.Description.Should().Be("Use recent api version " + recentPreviewVersion + ApiVersionPrefixConstants.Preview);
                });
        }

        [TestMethod]
        public void AddCodeFixIfNonGAVersionIsNotLatest_WithPreviewVersion_WhenGAVersionisNull_AndCurrentVersionIsRecent_ShouldNotAddDiagnostics()
        {
            DateTime currentVersionDate = DateTime.Today.AddYears(-2);
            string currentVersion = ConvertDateTimeToString(currentVersionDate);

            DateTime recentPreviewVersionDate = currentVersionDate;
            string recentPreviewVersion = ConvertDateTimeToString(recentPreviewVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfNonGAVersionIsNotLatest(new TextSpan(17, 47),
                                                        null,
                                                        recentPreviewVersion,
                                                        null,
                                                        ApiVersionPrefixConstants.Preview,
                                                        currentVersion);

            spanFixes.Should().BeEmpty();
        }

        [TestMethod]
        public void AddCodeFixIfNonGAVersionIsNotLatest_WithNonPreviewVersion_WhenGAVersionisNull_AndPreviewVersionIsRecent_ShouldAddDiagnostics()
        {
            DateTime currentVersionDate = DateTime.Today.AddYears(-3);
            string currentVersion = ConvertDateTimeToString(currentVersionDate);

            DateTime recentPreviewVersionDate = DateTime.Today.AddYears(-1);
            string recentPreviewVersion = ConvertDateTimeToString(recentPreviewVersionDate);

            DateTime recentNonPreviewVersionDate = DateTime.Today.AddYears(-2);
            string recentNonPreviewApiVersion = ConvertDateTimeToString(recentNonPreviewVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfNonGAVersionIsNotLatest(new TextSpan(17, 47),
                                                        null,
                                                        recentPreviewVersion,
                                                        null,
                                                        ApiVersionPrefixConstants.RC,
                                                        currentVersion);

            spanFixes.Should().SatisfyRespectively(
                x =>
                {
                    x.Value.Description.Should().Be("Use recent api version " + recentPreviewVersion + ApiVersionPrefixConstants.Preview);
                });
        }

        [TestMethod]
        public void AddCodeFixIfNonGAVersionIsNotLatest_WithNonPreviewVersion_WhenGAVersionisRecent_ShouldAddDiagnostics()
        {
            DateTime currentVersionDate = DateTime.Today.AddYears(-3);
            string currentVersion = ConvertDateTimeToString(currentVersionDate);

            DateTime recentGAVersionDate = DateTime.Today.AddYears(-1);
            string recentGAVersion = ConvertDateTimeToString(recentGAVersionDate);

            DateTime recentPreviewVersionDate = DateTime.Today.AddYears(-2);
            string recentPreviewVersion = ConvertDateTimeToString(recentPreviewVersionDate);

            DateTime recentNonPreviewVersionDate = DateTime.Today.AddYears(-3);
            string recentNonPreviewVersion = ConvertDateTimeToString(recentNonPreviewVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfNonGAVersionIsNotLatest(new TextSpan(17, 47),
                                                        recentGAVersion,
                                                        recentPreviewVersion,
                                                        recentNonPreviewVersion,
                                                        ApiVersionPrefixConstants.Alpha,
                                                        currentVersion);

            spanFixes.Should().SatisfyRespectively(
                x =>
                {
                    x.Value.Description.Should().Be("Use recent api version " + recentGAVersion);
                });
        }

        [TestMethod]
        public void AddCodeFixIfNonGAVersionIsNotLatest_WithNonPreviewVersion_WhenPreviewAndGAVersionsAreNull_AndNonPreviewVersionIsNotRecent_ShouldAddDiagnostics()
        {
            DateTime currentVersionDate = DateTime.Today.AddYears(-3);
            string currentVersion = ConvertDateTimeToString(currentVersionDate);

            DateTime recentNonPreviewVersionDate = DateTime.Today.AddYears(-2);
            string recentNonPreviewVersion = ConvertDateTimeToString(recentNonPreviewVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfNonGAVersionIsNotLatest(new TextSpan(17, 47),
                                                        null,
                                                        null,
                                                        recentNonPreviewVersion,
                                                        ApiVersionPrefixConstants.Alpha,
                                                        currentVersion);

            spanFixes.Should().SatisfyRespectively(
                x =>
                {
                    x.Value.Description.Should().Be("Use recent api version " + recentNonPreviewVersion + ApiVersionPrefixConstants.Alpha);
                });
        }

        [TestMethod]
        public void AddCodeFixIfNonGAVersionIsNotLatest_WithRecentNonPreviewVersion_ShouldNotAddDiagnostics()
        {
            DateTime currentVersionDate = DateTime.Today.AddMonths(-3);
            string currentVersion = ConvertDateTimeToString(currentVersionDate);

            DateTime recentGAVersionDate = DateTime.Today.AddYears(-1);
            string recentGAVersion = ConvertDateTimeToString(recentGAVersionDate);

            DateTime recentPreviewVersionDate = DateTime.Today.AddYears(-2);
            string recentPreviewVersion = ConvertDateTimeToString(recentPreviewVersionDate);

            DateTime recentNonPreviewVersionDate = DateTime.Today.AddYears(-3);
            string recentNonPreviewVersion = ConvertDateTimeToString(recentNonPreviewVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfNonGAVersionIsNotLatest(new TextSpan(17, 47),
                                                        recentGAVersion,
                                                        recentPreviewVersion,
                                                        recentNonPreviewVersion,
                                                        ApiVersionPrefixConstants.Alpha,
                                                        currentVersion);

            spanFixes.Should().BeEmpty();
        }

        [DataRow("invalid-text")]
        [DataRow("")]
        [DataRow("   ")]
        [TestMethod]
        public void GetApiVersionDate_WithInvalidVersion(string apiVersion)
        {
            Dictionary<TextSpan, CodeFix> spanFixes = new();
            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            DateTime? actual = visitor.GetApiVersionDate(apiVersion);

            actual.Should().BeNull();
        }

        [DataRow("2015-04-01-rc", "2015-04-01")]
        [DataRow("2016-04-01", "2016-04-01")]
        [DataRow("2016-04-01-privatepreview", "2016-04-01")]
        [TestMethod]
        public void GetApiVersionDate_WithValidVersion(string apiVersion, string expectedVersion)
        {
            Dictionary<TextSpan, CodeFix> spanFixes = new();
            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            DateTime? actual = visitor.GetApiVersionDate(apiVersion);

            actual.Should().Be(DateTime.Parse(expectedVersion, CultureInfo.InvariantCulture));
        }

        private string ConvertDateTimeToString(DateTime dateTime)
        {
            return dateTime.Year + "-" + dateTime.Month + "-" + dateTime.Day;
        }
    }
}
