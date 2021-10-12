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
        private readonly ApiVersionProvider ApiVersionProvider = new ApiVersionProvider();

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
        public void AddCodeFixIfNonPreviewVersionIsNotLatest_WithCurrentApiVersionLessThanTwoYearsOld_ShouldNotAddDiagnostics()
        {
            DateTime currentApiVersionDate = DateTime.Today.AddYears(-1);
            DateTime recentNonPreviewApiVersionDate = DateTime.Today.AddMonths(-5);

            string recentNonPreviewApiVersion = ConvertDateTimeToString(recentNonPreviewApiVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfNonPreviewVersionIsNotLatest(new TextSpan(37, 75),
                                                                 recentNonPreviewApiVersion,
                                                                 currentApiVersionDate);

            spanFixes.Should().BeEmpty();
        }

        [TestMethod]
        public void AddCodeFixIfNonPreviewVersionIsNotLatest_WithCurrentApiVersionMoreThanTwoYearsOldAndRecentApiVersionIsAvailable_ShouldAddDiagnostics()
        {
            DateTime currentApiVersionDate = DateTime.Today.AddYears(-3);
            DateTime recentNonPreviewApiVersionDate = DateTime.Today.AddMonths(-5);

            string recentNonPreviewApiVersion = ConvertDateTimeToString(recentNonPreviewApiVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfNonPreviewVersionIsNotLatest(new TextSpan(37, 75),
                                                             recentNonPreviewApiVersion,
                                                             currentApiVersionDate);

            spanFixes.Should().SatisfyRespectively(
                x =>
                {
                    x.Value.Description.Should().Be("Use recent api version " + recentNonPreviewApiVersion);
                });
        }

        [TestMethod]
        public void AddCodeFixIfNonPreviewVersionIsNotLatest_WithCurrentAndRecentApiVersionsMoreThanTwoYearsOld_ShouldAddDiagnosticsToUseRecentApiVersion()
        {
            DateTime currentApiVersionDate = DateTime.Today.AddYears(-4);
            DateTime recentNonPreviewApiVersionDate = DateTime.Today.AddYears(-3);

            string recentNonPreviewApiVersion = ConvertDateTimeToString(recentNonPreviewApiVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfNonPreviewVersionIsNotLatest(new TextSpan(37, 75),
                                                             recentNonPreviewApiVersion,
                                                             currentApiVersionDate);

            spanFixes.Should().SatisfyRespectively(
                x =>
                {
                    x.Value.Description.Should().Be("Use recent api version " + recentNonPreviewApiVersion);
                });
        }

        [TestMethod]
        public void AddCodeFixIfNonPreviewVersionIsNotLatest_WhenCurrentAndRecentApiVersionsAreSameAndMoreThanTwoYearsOld_ShouldNotAddDiagnostics()
        {
            DateTime currentApiVersionDate = DateTime.Today.AddYears(-3);
            DateTime recentNonPreviewApiVersionDate = currentApiVersionDate;

            string recentNonPreviewApiVersion = ConvertDateTimeToString(recentNonPreviewApiVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfNonPreviewVersionIsNotLatest(new TextSpan(37, 75),
                                                             recentNonPreviewApiVersion,
                                                             currentApiVersionDate);

            spanFixes.Should().BeEmpty();
        }

        [TestMethod]
        public void AddCodeFixIfPreviewVersionIsNotLatest_WhenCurrentApiVersionIsLatest_ShouldNotAddDiagnostics()
        {
            DateTime currentApiVersionDate = DateTime.Today.AddYears(-1);
            DateTime recentNonPreviewApiVersionDate = DateTime.Today.AddYears(-3);
            DateTime recentPreviewApiVersionDate = currentApiVersionDate;

            string recentNonPreviewApiVersion = ConvertDateTimeToString(recentNonPreviewApiVersionDate);
            string recentPreviewApiVersion = ConvertDateTimeToString(recentPreviewApiVersionDate) + "-preview";

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfPreviewVersionIsNotLatest(new TextSpan(37, 75),
                                                          recentPreviewApiVersion,
                                                          recentNonPreviewApiVersion,
                                                          currentApiVersionDate);

            spanFixes.Should().BeEmpty();
        }

        [TestMethod]
        public void AddCodeFixIfPreviewVersionIsNotLatest_WhenRecentPreviewVersionIsAvailable_ShouldAddDiagnostics()
        {
            DateTime currentApiVersionDate = DateTime.Today.AddYears(-5);
            DateTime recentNonPreviewApiVersionDate = DateTime.Today.AddYears(-3);
            DateTime recentPreviewApiVersionDate = DateTime.Today.AddYears(-2);

            string recentNonPreviewApiVersion = ConvertDateTimeToString(recentNonPreviewApiVersionDate);
            string recentPreviewApiVersionWithoutPreviewPrefix = ConvertDateTimeToString(recentPreviewApiVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfPreviewVersionIsNotLatest(new TextSpan(37, 75),
                                                          recentPreviewApiVersionWithoutPreviewPrefix,
                                                          recentNonPreviewApiVersion,
                                                          currentApiVersionDate);

            spanFixes.Should().SatisfyRespectively(
                x =>
                {
                    x.Value.Description.Should().Be("Use recent api version " + recentPreviewApiVersionWithoutPreviewPrefix + "-preview");
                });
        }

        [TestMethod]
        public void AddCodeFixIfPreviewVersionIsNotLatest_WhenRecentNonPreviewVersionIsAvailable_ShouldAddDiagnostics()
        {
            DateTime currentApiVersionDate = DateTime.Today.AddYears(-5);
            DateTime recentNonPreviewApiVersionDate = DateTime.Today.AddYears(-2);
            DateTime recentPreviewApiVersionDate = DateTime.Today.AddYears(-3);

            string recentNonPreviewApiVersion = ConvertDateTimeToString(recentNonPreviewApiVersionDate);
            string recentPreviewApiVersionWithoutPreviewPrefix = ConvertDateTimeToString(recentPreviewApiVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfPreviewVersionIsNotLatest(new TextSpan(37, 75),
                                                          recentPreviewApiVersionWithoutPreviewPrefix,
                                                          recentNonPreviewApiVersion,
                                                          currentApiVersionDate);

            spanFixes.Should().SatisfyRespectively(
                x =>
                {
                    x.Value.Description.Should().Be("Use recent api version " + recentNonPreviewApiVersion);
                });
        }

        [TestMethod]
        public void AddCodeFixIfPreviewVersionIsNotLatest_WhenRecentNonPreviewVersionisSameAsPreviewVersion_ShouldAddDiagnosticsUsingNonPreviewVersion()
        {
            DateTime currentApiVersionDate = DateTime.Today.AddYears(-3);
            DateTime recentNonPreviewApiVersionDate = DateTime.Today.AddYears(-2);
            DateTime recentPreviewApiVersionDate = DateTime.Today.AddYears(-2);

            string recentNonPreviewApiVersion = ConvertDateTimeToString(recentNonPreviewApiVersionDate);
            string recentPreviewApiVersionWithoutPreviewPrefix = ConvertDateTimeToString(recentPreviewApiVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfPreviewVersionIsNotLatest(new TextSpan(37, 75),
                                                          recentPreviewApiVersionWithoutPreviewPrefix,
                                                          recentNonPreviewApiVersion,
                                                          currentApiVersionDate);

            spanFixes.Should().SatisfyRespectively(
                x =>
                {
                    x.Value.Description.Should().Be("Use recent api version " + recentNonPreviewApiVersion);
                });
        }

        [TestMethod]
        public void AddCodeFixIfPreviewVersionIsNotLatest_WhenRecentNonPreviewVersionisNull_AndCurrentApiVersionIsNotRecent_ShouldAddDiagnosticsUsingPreviewVersion()
        {
            DateTime currentApiVersionDate = DateTime.Today.AddYears(-3);
            DateTime recentPreviewApiVersionDate = DateTime.Today.AddYears(-2);

            string recentPreviewApiVersionWithoutPreviewPrefix = ConvertDateTimeToString(recentPreviewApiVersionDate);

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfPreviewVersionIsNotLatest(new TextSpan(37, 75),
                                                          recentPreviewApiVersionWithoutPreviewPrefix,
                                                          null,
                                                          currentApiVersionDate);

            spanFixes.Should().SatisfyRespectively(
                x =>
                {
                    x.Value.Description.Should().Be("Use recent api version " + recentPreviewApiVersionWithoutPreviewPrefix + "-preview");
                });
        }

        [TestMethod]
        public void AddCodeFixIfPreviewVersionIsNotLatest_WhenRecentNonPreviewVersionisNull_AndCurrentApiVersionIsRecent_ShouldNotAddDiagnostics()
        {
            DateTime currentApiVersionDate = DateTime.Today.AddYears(-2);
            DateTime recentPreviewApiVersionDate = currentApiVersionDate;

            string recentPreviewApiVersion = ConvertDateTimeToString(recentPreviewApiVersionDate) + "-preview";

            Dictionary<TextSpan, CodeFix> spanFixes = new();

            Visitor visitor = new Visitor(spanFixes, SemanticModel);

            visitor.AddCodeFixIfPreviewVersionIsNotLatest(new TextSpan(37, 75),
                                                          recentPreviewApiVersion,
                                                          null,
                                                          currentApiVersionDate);

            spanFixes.Should().BeEmpty();
        }

        private string ConvertDateTimeToString(DateTime dateTime)
        {
            return dateTime.Year + "-" + dateTime.Month + "-" + dateTime.Day;
        }
    }
}
