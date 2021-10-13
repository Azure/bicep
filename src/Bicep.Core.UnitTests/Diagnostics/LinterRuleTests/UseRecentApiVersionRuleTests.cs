// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
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
            AssertRuleCodeDiagnostics(UseRecentApiVersionRule.Code, text, diags =>
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
                                                               SourceFileGroupingFactory.CreateFromText("", BicepTestConstants.FileResolver),
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

            visitor.AddCodeFixIfGAVersionIsNotLatest(new TextSpan(37, 75),
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

            visitor.AddCodeFixIfGAVersionIsNotLatest(new TextSpan(37, 75),
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

            visitor.AddCodeFixIfGAVersionIsNotLatest(new TextSpan(37, 75),
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

            visitor.AddCodeFixIfGAVersionIsNotLatest(new TextSpan(37, 75),
                                                     recentGAVersion,
                                                     currentVersion);

            spanFixes.Should().BeEmpty();
        }

        [TestMethod]
        public void AddCodeFixIfNonGAVersionIsNotLatest_WhenCurrentPreviewApiVersionIsLatest_ShouldNotAddDiagnostics()
        {
            DateTime currentVersionDate = DateTime.Today.AddYears(-1);
            string currentVersion = ConvertDateTimeToString(currentVersionDate);

            DateTime recentGAVersionDate = DateTime.Today.AddYears(-3);
            string recentGAVersion = ConvertDateTimeToString(recentGAVersionDate);

            DateTime recentPreviewVersionDate = currentVersionDate;
            string recentPreviewVersion = ConvertDateTimeToString(recentPreviewVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfNonGAVersionIsNotLatest(new TextSpan(37, 75),
                                                        recentGAVersion,
                                                        recentPreviewVersion,
                                                        null,
                                                        ApiVersionPrefixConstants.Preview,
                                                        currentVersion);

            spanFixes.Should().BeEmpty();
        }

        [TestMethod]
        public void AddCodeFixIfNonGAVersionIsNotLatest_WhenRecentPreviewVersionIsAvailable_ShouldAddDiagnostics()
        {
            DateTime currentVersionDate = DateTime.Today.AddYears(-5);
            string currentVersion = ConvertDateTimeToString(currentVersionDate);

            DateTime recentGAVersionDate = DateTime.Today.AddYears(-3);
            string recentGAVersion = ConvertDateTimeToString(recentGAVersionDate);

            DateTime recentPreviewVersionDate = DateTime.Today.AddYears(-2);
            string recentPreviewVersion = ConvertDateTimeToString(recentPreviewVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfNonGAVersionIsNotLatest(new TextSpan(37, 75),
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
        public void AddCodeFixIfNonGAVersionIsNotLatest_WhenRecentGAVersionIsAvailable_ShouldAddDiagnostics()
        {
            DateTime currentVersionDate = DateTime.Today.AddYears(-5);
            string currentVersion = ConvertDateTimeToString(currentVersionDate);

            DateTime recentGAVersionDate = DateTime.Today.AddYears(-2);
            string recentGAVersion = ConvertDateTimeToString(recentGAVersionDate);

            DateTime recentPreviewVersionDate = DateTime.Today.AddYears(-3);
            string recentPreviewVersion = ConvertDateTimeToString(recentPreviewVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfNonGAVersionIsNotLatest(new TextSpan(37, 75),
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
        public void AddCodeFixIfNonGAVersionIsNotLatest_WhenRecentGAVersionIsSameAsPreviewVersion_ShouldAddDiagnosticsUsingGAPreviewVersion()
        {
            DateTime currentVersionDate = DateTime.Today.AddYears(-3);
            string currentVersion = ConvertDateTimeToString(currentVersionDate);

            DateTime recentGAVersionDate = DateTime.Today.AddYears(-2);
            string recentGAVersion = ConvertDateTimeToString(recentGAVersionDate);

            DateTime recentPreviewVersionDate = DateTime.Today.AddYears(-2);
            string recentPreviewVersion = ConvertDateTimeToString(recentPreviewVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfNonGAVersionIsNotLatest(new TextSpan(37, 75),
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
        public void AddCodeFixIfNonGAVersionIsNotLatest_WhenGAVersionisNull_AndCurrentVersionIsNotRecent_ShouldAddDiagnosticsUsingPreviewVersion()
        {
            DateTime currentVersionDate = DateTime.Today.AddYears(-3);
            string currentVersion = ConvertDateTimeToString(currentVersionDate);

            DateTime recentPreviewVersionDate = DateTime.Today.AddYears(-2);
            string recentPreviewVersion = ConvertDateTimeToString(recentPreviewVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfNonGAVersionIsNotLatest(new TextSpan(37, 75),
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
        public void AddCodeFixIfNonGAVersionIsNotLatest_WhenGAVersionisNull_AndCurrentVersionIsRecent_ShouldNotAddDiagnostics()
        {
            DateTime currentVersionDate = DateTime.Today.AddYears(-2);
            string currentVersion = ConvertDateTimeToString(currentVersionDate);

            DateTime recentPreviewVersionDate = currentVersionDate;
            string recentPreviewApiVersion = ConvertDateTimeToString(recentPreviewVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfNonGAVersionIsNotLatest(new TextSpan(37, 75),
                                                        null,
                                                        recentPreviewApiVersion,
                                                        null,
                                                        ApiVersionPrefixConstants.Preview,
                                                        currentVersion);

            spanFixes.Should().BeEmpty();
        }

        private string ConvertDateTimeToString(DateTime dateTime)
        {
            return dateTime.Year + "-" + dateTime.Month + "-" + dateTime.Day;
        }
    }
}
