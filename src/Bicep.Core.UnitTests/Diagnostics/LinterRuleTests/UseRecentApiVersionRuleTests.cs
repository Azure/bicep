// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Json;
using Bicep.Core.Parsing;
using Bicep.Core.TypeSystem;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bicep.Core.CodeAction;

#pragma warning disable CA1825 // Avoid zero-length array allocations

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public partial class UseRecentApiVersionRuleTests : LinterRuleTestsBase
    {
        public record DiagnosticAndFixes
        (
            string ExpectedDiagnosticMessage,
            string ExpectedFixTitle,
            string ExpectedSubstringInReplacedBicep
        );

        private static void CompileAndTestWithFakeDateAndTypes(string bicep, ResourceScope scope, string[] resourceTypes, string fakeToday, string[] expectedMessagesForCode, OnCompileErrors onCompileErrors = OnCompileErrors.IncludeErrors)
        {
            // Test with the linter thinking today's date is fakeToday and also fake resource types from FakeResourceTypes
            // Note: The compiler does not know about these fake types, only the linter.
            var apiProvider = new ApiVersionProvider(BicepTestConstants.Features, BicepTestConstants.NamespaceProvider);
            apiProvider.InjectTypeReferences(scope, FakeResourceTypes.GetFakeResourceTypeReferences(resourceTypes));

            AssertLinterRuleDiagnostics(UseRecentApiVersionRule.Code,
                bicep,
                expectedMessagesForCode,
                new Options(
                    OnCompileErrors: onCompileErrors,
                    IncludePosition.LineNumber,
                    Configuration: CreateConfigurationWithFakeToday(fakeToday),
                    ApiVersionProvider: apiProvider));
        }

        private static void CompileAndTestFixWithFakeDateAndTypes(string bicep, ResourceScope scope, string[] resourceTypes, string fakeToday, DiagnosticAndFixes[] expectedDiagnostics)
        {
            // Test with the linter thinking today's date is fakeToday and also fake resource types from FakeResourceTypes
            // Note: The compiler does not know about these fake types, only the linter.
            var apiProvider = new ApiVersionProvider(BicepTestConstants.Features, BicepTestConstants.NamespaceProvider);
            apiProvider.InjectTypeReferences(scope, FakeResourceTypes.GetFakeResourceTypeReferences(resourceTypes));

            AssertLinterRuleDiagnostics(
                UseRecentApiVersionRule.Code,
                bicep,
                (diags) =>
                {
                    // Diagnostic titles
                    diags.Select(d => d.Message).Should().BeEquivalentTo(expectedDiagnostics.Select(d => d.ExpectedDiagnosticMessage));

                    // Fixes
                    for (int i = 0; i < diags.Count(); ++i)
                    {
                        var actual = diags.Skip(i).First();
                        var expected = expectedDiagnostics[i];

                        var fixable = actual.Should().BeAssignableTo<IFixable>().Which;
                        fixable.Fixes.Should().HaveCount(1, "Expecting 1 fix");
                        var fix = fixable.Fixes.First();

                        fix.Kind.Should().Be(CodeFixKind.QuickFix);
                        fix.Description.Should().Be(expected.ExpectedFixTitle);

                        fix.Replacements.Should().HaveCount(1, "Expecting 1 replacement");
                        var replacement = fix.Replacements.First();
                        var replacementText = replacement.Text;
                        var newBicep = string.Concat(bicep.AsSpan(0, replacement.Span.Position), replacementText, bicep.AsSpan(replacement.Span.Position + replacement.Span.Length));

                        newBicep.Should().Contain(expected.ExpectedSubstringInReplacedBicep, "the suggested API version should be replaced in the bicep file");
                    }
                },
                new Options(
                    OnCompileErrors.IncludeErrors,
                    IncludePosition.LineNumber,
                    Configuration: CreateConfigurationWithFakeToday(fakeToday),
                    ApiVersionProvider: apiProvider));
        }

        private static RootConfiguration CreateConfigurationWithFakeToday(string today)
        {
            return new RootConfiguration(
                BicepTestConstants.BuiltInConfiguration.Cloud,
                BicepTestConstants.BuiltInConfiguration.ModuleAliases,
                    new AnalyzersConfiguration(
                         JsonElementFactory.CreateElement(@"
                            {
                              ""core"": {
                                ""enabled"": true,
                                ""rules"": {
                                  ""use-recent-api-versions"": {
                                      ""level"": ""warning"",
                                      ""test-today"": ""<TESTING_TODAY_DATE>"",
                                      ""test-warn-not-found"": true
                                  }
                                }
                              }
                            }".Replace("<TESTING_TODAY_DATE>", today))),
                BicepTestConstants.BuiltInConfiguration.CacheRootDirectory,
                BicepTestConstants.BuiltInConfiguration.ExperimentalFeaturesEnabled,
                null,
                null);
        }

        [TestClass]
        public class GetAcceptableApiVersionsTests
        {
            private static void TestGetAcceptableApiVersions(string fullyQualifiedResourceType, ResourceScope scope, string resourceTypes, string today, string[] expectedApiVersions, int maxAllowedAgeInDays = UseRecentApiVersionRule.MaxAllowedAgeInDays)
            {
                var apiVersionProvider = new ApiVersionProvider(BicepTestConstants.Features, BicepTestConstants.NamespaceProvider);
                apiVersionProvider.InjectTypeReferences(scope, FakeResourceTypes.GetFakeResourceTypeReferences(resourceTypes));
                var (_, allowedVersions) = UseRecentApiVersionRule.GetAcceptableApiVersions(apiVersionProvider, ApiVersionHelper.ParseDateFromApiVersion(today), maxAllowedAgeInDays, scope, fullyQualifiedResourceType);
                var allowedVersionsStrings = allowedVersions.Select(v => v.Formatted).ToArray();
                allowedVersionsStrings.Should().BeEquivalentTo(expectedApiVersions, options => options.WithStrictOrdering());
            }

            [TestMethod]
            public void CaseInsensitiveResourceType()
            {
                TestGetAcceptableApiVersions(
                    "Fake.KUSTO/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2418-09-07-preview
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2418-09-07-preview",
                    });
            }

            [TestMethod]
            public void CaseInsensitiveApiSuffix()
            {
                TestGetAcceptableApiVersions(
                    "Fake.KUSTO/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2418-09-07-PREVIEW
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2418-09-07-preview",
                    });
            }

            [TestMethod]
            public void ResourceTypeNotRecognized_ReturnNone()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kisto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2421-01-01
                    ",
                    "2421-07-07",
                    new string[]
                    {
                    });
            }

            [TestMethod]
            public void NoStable_OldPreview_PickOnlyMostRecentPreview()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2413-01-21-preview
                        Fake.Kusto/clusters@2413-05-15-beta
                        Fake.Kusto/clusters@2413-09-07-alpha
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2413-09-07-alpha",
                    });
            }

            [TestMethod]
            public void NoStable_OldPreview_PickOnlyMostRecentPreview_MultiplePreviewWithSameDate()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2413-01-21-preview
                        Fake.Kusto/clusters@2413-05-15-beta
                        Fake.Kusto/clusters@2413-09-07-alpha
                        Fake.Kusto/clusters@2413-09-07-beta
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2413-09-07-alpha",
                        "2413-09-07-beta",
                    });
            }

            [TestMethod]
            public void NoStable_NewPreview_PickAllNewPreview()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2419-07-21-preview
                        Fake.Kusto/clusters@2419-08-15-beta
                        Fake.Kusto/clusters@2419-09-07-alpha
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2419-09-07-alpha",
                        "2419-08-15-beta",
                        "2419-07-21-preview",
                    });
            }

            [TestMethod]
            public void NoStable_NewPreview_PickNewPreview_MultiplePreviewHaveSameDate()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2419-07-21-preview
                        Fake.Kusto/clusters@2419-08-15-beta
                        Fake.Kusto/clusters@2419-09-07-alpha
                        Fake.Kusto/clusters@2419-07-21-beta
                        Fake.Kusto/clusters@2419-08-15-privatepreview
                        Fake.Kusto/clusters@2419-09-07-beta
                        Fake.Kusto/clusters@2419-09-07-privatepreview
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2419-09-07-alpha",
                        "2419-09-07-beta",
                        "2419-09-07-privatepreview",
                        "2419-08-15-beta",
                        "2419-08-15-privatepreview",
                        "2419-07-21-beta",
                        "2419-07-21-preview",
                    });
            }

            [TestMethod]
            public void NoStable_OldAndNewPreview_PickNewPreview()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2419-07-15-privatepreview
                        Fake.Kusto/clusters@2413-01-21-preview
                        Fake.Kusto/clusters@2414-05-15-beta
                        Fake.Kusto/clusters@2415-09-07-alpha
                        Fake.Kusto/clusters@2419-08-21-beta
                        Fake.Kusto/clusters@2419-09-07-beta
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2419-09-07-beta",
                        "2419-08-21-beta",
                        "2419-07-15-privatepreview",
                    });
            }

            [TestMethod]
            public void OldStable_NoPreview_PickOnlyMostRecentStable()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2419-01-21
                        Fake.Kusto/clusters@2419-05-15
                        Fake.Kusto/clusters@2419-09-07
                        Fake.Kusto/clusters@2419-11-09
                        Fake.Kusto/clusters@2420-02-15
                        Fake.Kusto/clusters@2420-06-14
                        Fake.Kusto/clusters@2420-09-18
                    ",
                    "2500-07-07",
                    new string[]
                    {
                        "2420-09-18",
                    });
            }

            [TestMethod]
            public void OldStable_OldPreview_NewestPreviewIsOlderThanNewestStable_PickOnlyNewestStable()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2413-01-21
                        Fake.Kusto/clusters@2413-05-15
                        Fake.Kusto/clusters@2413-06-15-preview
                        Fake.Kusto/clusters@2413-09-07
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2413-09-07",
                    });
            }

            [TestMethod]
            public void OldStable_OldPreview_NewestPreviewIsSameAgeAsNewestStable_PickOnlyNewestStable()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2413-01-21
                        Fake.Kusto/clusters@2413-05-15
                        Fake.Kusto/clusters@2413-06-15-preview
                        Fake.Kusto/clusters@2413-06-15
                        Fake.Kusto/clusters@2413-06-15-beta
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2413-06-15",
                    });
            }

            [TestMethod]
            public void OldStable_OldPreview_NewestPreviewIsNewThanNewestStable_PickJustNewestStable()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2413-01-21
                        Fake.Kusto/clusters@2413-01-21-preview
                        Fake.Kusto/clusters@2413-05-15
                        Fake.Kusto/clusters@2413-06-15
                        Fake.Kusto/clusters@2413-09-07-preview
                        Fake.Kusto/clusters@2413-09-07-beta
                        Fake.Kusto/clusters@2413-09-08-beta
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2413-06-15",
                    });
            }

            [TestMethod]
            public void OldStable_NewPreview_PickNewestStableAndNewPreview()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2413-01-21
                        Fake.Kusto/clusters@2413-05-15
                        Fake.Kusto/clusters@2413-06-15
                        Fake.Kusto/clusters@2419-09-07-preview
                        Fake.Kusto/clusters@2419-09-07-beta
                        Fake.Kusto/clusters@2419-09-08-beta
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2419-09-08-beta",
                        "2419-09-07-beta",
                        "2419-09-07-preview",
                        "2413-06-15",
                    });
            }

            [TestMethod]
            public void OldStable_OldAndNewPreview_PickNewestStableAndNewPreview()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2413-01-21
                        Fake.Kusto/clusters@2413-05-15
                        Fake.Kusto/clusters@2413-06-15-beta
                        Fake.Kusto/clusters@2419-09-07-preview
                        Fake.Kusto/clusters@2419-09-07-beta
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2419-09-07-beta",
                        "2419-09-07-preview",
                        "2413-05-15",
                    });
            }

            [TestMethod]
            public void NewStable_NoPreview_PickNewStable()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2419-07-21
                        Fake.Kusto/clusters@2419-08-15
                        Fake.Kusto/clusters@2420-09-18
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2420-09-18",
                        "2419-08-15",
                        "2419-07-21",
                    });
            }

            [TestMethod]
            public void OnlyPickPreviewThatAreNewerThanNewestStable_NoPreviewAreNewer()
            {
                TestGetAcceptableApiVersions(
                   "Fake.Kusto/clusters",
                   ResourceScope.ResourceGroup,
                   @"
                        Fake.Kusto/clusters@2413-07-21
                        Fake.Kusto/clusters@2419-07-21
                        Fake.Kusto/clusters@2419-07-15-alpha
                        Fake.Kusto/clusters@2419-07-16-beta
                        Fake.Kusto/clusters@2420-09-18
                    ",
                   "2421-07-07",
                   new string[]
                   {
                        "2420-09-18",
                        "2419-07-21",
                   });
            }

            [TestMethod]
            public void OnlyPickPreviewThatAreNewerThanNewestStable_OnePreviewIsOlder()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2419-07-21
                        Fake.Kusto/clusters@2419-07-15-alpha
                        Fake.Kusto/clusters@2420-09-18
                        Fake.Kusto/clusters@2421-07-16-beta
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2421-07-16-beta",
                        "2420-09-18",
                        "2419-07-21",
                    });
            }

            [TestMethod]
            public void OnlyPickPreviewThatAreNewerThanNewestStable_MultiplePreviewsAreNewer()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2413-07-21
                        Fake.Kusto/clusters@2419-07-21
                        Fake.Kusto/clusters@2419-07-15-alpha
                        Fake.Kusto/clusters@2420-09-18
                        Fake.Kusto/clusters@2421-07-16-beta
                        Fake.Kusto/clusters@2421-07-17-preview
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2421-07-17-preview",
                        "2421-07-16-beta",
                        "2420-09-18",
                        "2419-07-21",
                    });
            }

            [TestMethod]
            public void OnlyPickPreviewThatAreNewerThanNewestStable_MultiplePreviewsAreNewer_AllAreOld()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2415-07-21
                        Fake.Kusto/clusters@2415-07-15-alpha
                        Fake.Kusto/clusters@2416-09-18
                        Fake.Kusto/clusters@2417-07-16-beta
                        Fake.Kusto/clusters@2417-07-17-preview
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2416-09-18",
                    });
            }

            [TestMethod]
            public void OnlyPickPreviewThatAreNewerThanNewestStable_MultiplePreviewsAreNewer_AllStableAreOld()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2415-07-21
                        Fake.Kusto/clusters@2415-07-15-alpha
                        Fake.Kusto/clusters@2416-09-18
                        Fake.Kusto/clusters@2421-07-16-beta
                        Fake.Kusto/clusters@2421-07-17-preview
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2421-07-17-preview",
                        "2421-07-16-beta",
                        "2416-09-18",
                    });
            }

            [TestMethod]
            public void OnlyPickPreviewThatAreNewerThanNewestStable_AllAreNew_NoPreviewAreNewerThanStable()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2413-01-21-preview
                        Fake.Kusto/clusters@2419-07-11
                        Fake.Kusto/clusters@2419-07-15-alpha
                        Fake.Kusto/clusters@2419-07-16-beta
                        Fake.Kusto/clusters@2420-09-18
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2420-09-18",
                        "2419-07-11",
                    });
            }

            [TestMethod]
            public void OldAndNewStable_NoPreview_PickNewStable()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2419-01-21
                        Fake.Kusto/clusters@2419-05-15
                        Fake.Kusto/clusters@2419-09-07
                        Fake.Kusto/clusters@2421-01-01
                        Fake.Kusto/clusters@2425-01-01
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2425-01-01",
                        "2421-01-01",
                        "2419-09-07",
                    });
            }

            [TestMethod]
            public void OldAndNewStable_OldPreview_PickNewStable()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2413-09-07-privatepreview
                        Fake.Kusto/clusters@2413-09-07-preview
                        Fake.Kusto/clusters@2414-01-21
                        Fake.Kusto/clusters@2414-05-15
                        Fake.Kusto/clusters@2414-09-07
                        Fake.Kusto/clusters@2415-11-09
                        Fake.Kusto/clusters@2415-02-15
                        Fake.Kusto/clusters@2420-06-14
                        Fake.Kusto/clusters@2420-09-18
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2420-09-18",
                        "2420-06-14",
                    });
            }

            [TestMethod]
            public void OldAndNewStable_NewPreviewButOlderThanNewestStable_PickNewStableOnly()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2419-09-07-privatepreview
                        Fake.Kusto/clusters@2419-09-07-preview
                        Fake.Kusto/clusters@2414-01-21
                        Fake.Kusto/clusters@2414-05-15
                        Fake.Kusto/clusters@2414-09-07
                        Fake.Kusto/clusters@2415-11-09
                        Fake.Kusto/clusters@2415-02-15
                        Fake.Kusto/clusters@2420-06-14
                        Fake.Kusto/clusters@2420-09-18
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2420-09-18",
                        "2420-06-14",
                    });
            }

            [TestMethod]
            public void OldAndNewStable_OldAndNewPreview_PickNewStableAndPreviewNewestThanNewestStable()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2413-09-07-privatepreview
                        Fake.Kusto/clusters@2421-09-07-privatepreview
                        Fake.Kusto/clusters@2419-09-07-preview
                        Fake.Kusto/clusters@2417-09-07-preview
                        Fake.Kusto/clusters@2414-01-21
                        Fake.Kusto/clusters@2414-05-15
                        Fake.Kusto/clusters@2414-09-07
                        Fake.Kusto/clusters@2415-11-09
                        Fake.Kusto/clusters@2415-02-15
                        Fake.Kusto/clusters@2420-06-14
                        Fake.Kusto/clusters@2420-09-18
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2421-09-07-privatepreview",
                        "2420-09-18",
                        "2420-06-14",
                    });
            }

            [TestMethod]
            public void OnlyPreviewVersionsAvailable_AcceptAllRecentPreviews()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2410-01-01-preview
                        Fake.Kusto/clusters@2410-01-02-preview
                        Fake.Kusto/clusters@2421-01-01-preview
                        Fake.Kusto/clusters@2421-01-02-preview
                        Fake.Kusto/clusters@2421-01-03-preview
                        Fake.Kusto/clusters@2421-03-01-preview
                        Fake.Kusto/clusters@2421-04-01-preview
                        Fake.Kusto/clusters@2421-04-02-preview
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2421-04-02-preview",
                        "2421-04-01-preview",
                        "2421-03-01-preview",
                        "2421-01-03-preview",
                        "2421-01-02-preview",
                        "2421-01-01-preview",
                    });
            }

            [TestMethod]
            public void LotsOfPreviewVersions_AndOneRecentGA_Available_AllowOnlyPreviewsMoreRecentThanGA()
            {
                TestGetAcceptableApiVersions(
                    "Fake.Kusto/clusters",
                    ResourceScope.ResourceGroup,
                    @"
                        Fake.Kusto/clusters@2410-01-01-preview
                        Fake.Kusto/clusters@2410-01-02-preview
                        Fake.Kusto/clusters@2421-01-01-preview
                        Fake.Kusto/clusters@2421-01-02-preview
                        Fake.Kusto/clusters@2421-01-03-preview
                        Fake.Kusto/clusters@2421-02-01
                        Fake.Kusto/clusters@2421-03-01-preview
                        Fake.Kusto/clusters@2421-04-01-beta
                        Fake.Kusto/clusters@2421-04-02-preview
                    ",
                    "2421-07-07",
                    new string[]
                    {
                        "2421-04-02-preview",
                        "2421-04-01-beta",
                        "2421-03-01-preview",
                        "2421-02-01", // No previews older than this allowed, even if < 2 years old
                    });
            }
        }

        [TestClass]
        public class GetAcceptableApiVersionsInvariantsTests
        {
            private static readonly ApiVersionProvider RealApiVersionProvider = new(BicepTestConstants.Features, BicepTestConstants.NamespaceProvider);
            private static readonly bool Exhaustive = false;

            public class TestData
            {
                public TestData(ResourceScope resourceScope, string fullyQualifiedResourceType, DateTime today, int maxAllowedDaysAge)
                {
                    ResourceScope = resourceScope;
                    FullyQualifiedResourceType = fullyQualifiedResourceType;
                    MaxAllowedAgeDays = maxAllowedDaysAge;
                    Today = today;
                }

                public string FullyQualifiedResourceType { get; }
                public ResourceScope ResourceScope { get; }
                public int MaxAllowedAgeDays { get; }
                public DateTime Today { get; }

                public static string GetDisplayName(MethodInfo _, object[] data)
                {
                    var me = ((TestData)data[0]);
                    return $"{me.ResourceScope}:{me.FullyQualifiedResourceType} (max={me.MaxAllowedAgeDays} days)";
                }
            }

            private static IEnumerable<object[]> GetTestData()
            {
                const int maxAllowedAgeDays = 2 * 365;
                const int maxResourcesToTest = 500; // test is slow

                foreach (ResourceScope scope in new ResourceScope[]
                    {
                        ResourceScope.ResourceGroup,
                        ResourceScope.Tenant,
                        ResourceScope.ManagementGroup,
                        ResourceScope.Subscription,
                    })
                {
                    foreach (string typeName in RealApiVersionProvider.GetResourceTypeNames(scope).Take(maxResourcesToTest))
                    {
                        var apiVersionDates = RealApiVersionProvider.GetApiVersions(scope, typeName).Select(v => v.Date);

                        var datesToTest = new List<DateTime>();
                        if (Exhaustive)

                        {
                            foreach (DateTime dt in apiVersionDates)
                            {
                                datesToTest.Add(dt.AddDays(-1));
                                datesToTest.Add(dt);
                                datesToTest.Add(dt.AddDays(1));
                            }
                        }

                        datesToTest.Add(apiVersionDates.Min().AddDays(-(maxAllowedAgeDays - 1)));
                        if (Exhaustive)
                        {
                            datesToTest.Add(apiVersionDates.Min().AddDays(-maxAllowedAgeDays));
                            datesToTest.Add(apiVersionDates.Min().AddDays(-(maxAllowedAgeDays + 1)));
                        }

                        datesToTest.Add(apiVersionDates.Max().AddDays(maxAllowedAgeDays - 1));
                        if (Exhaustive)
                        {
                            datesToTest.Add(apiVersionDates.Max().AddDays(maxAllowedAgeDays));
                            datesToTest.Add(apiVersionDates.Max().AddDays(maxAllowedAgeDays + 1));
                        }

                        datesToTest = datesToTest.Distinct().ToList();
                        foreach (var today in datesToTest)
                        {
                            yield return new object[] { new TestData(scope, typeName, today, maxAllowedAgeDays) };
                        }
                    }
                }
            }

            private static bool DateIsRecent(DateTime dt, DateTime today, int maxAllowedAgeDays)
            {
                return DateIsEqualOrMoreRecentThan(dt.AddDays(maxAllowedAgeDays), today);
            }

            private static bool DateIsOld(DateTime dt, DateTime today, int maxAllowedAgeDays)
            {
                return !DateIsRecent(dt, today, maxAllowedAgeDays);
            }

            private static bool DateIsMoreRecentThan(DateTime dt, DateTime other)
            {
                return DateTime.Compare(dt, other) > 0;
            }

            private static bool DateIsEqualOrMoreRecentThan(DateTime dt, DateTime other)
            {
                return DateTime.Compare(dt, other) >= 0;
            }

            private static bool DateIsOlderThan(DateTime dt, DateTime other)
            {
                return DateTime.Compare(dt, other) < 0;
            }

            private static bool DateIsEqualOrOlderThan(DateTime dt, DateTime other)
            {
                return DateTime.Compare(dt, other) < 0;
            }

            [DataTestMethod]
            [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(TestData), DynamicDataDisplayName = nameof(TestData.GetDisplayName))]
            public void Invariants(TestData data)
            {
                var (allVersions, allowedVersions) = UseRecentApiVersionRule.GetAcceptableApiVersions(RealApiVersionProvider, data.Today, data.MaxAllowedAgeDays, data.ResourceScope, data.FullyQualifiedResourceType);

                allVersions.Should().NotBeNull();
                allowedVersions.Should().NotBeNull();

                allVersions.Should().NotBeEmpty();
                allowedVersions.Should().NotBeEmpty();

                var stableVersions = allVersions.Where(v => v.IsStable).ToArray();
                var recentStableVersions = stableVersions.Where(v => DateIsRecent(v.Date, data.Today, data.MaxAllowedAgeDays)).ToArray();

                var previewVersions = allVersions.Where(v => v.IsPreview).ToArray();
                var recentPreviewVersions = previewVersions.Where(v => DateIsRecent(v.Date, data.Today, data.MaxAllowedAgeDays)).ToArray();

                // if there are any stable versions available...
                if (stableVersions.Any())
                {
                    // The most recent stable version is always allowed
                    allowedVersions.Count(v => v.IsStable).Should().BeGreaterThan(0, "the most recent stable version is always allowed");
                    var mostRecentStable = stableVersions.OrderBy(v => v.Date).Last();
                    allowedVersions.Should().Contain(mostRecentStable, "the most recent stable version is always allowed");

                    // All stable versions < 2 years old are allowed
                    if (recentStableVersions.Any())
                    {
                        allowedVersions.Should().Contain(recentStableVersions, "all stable versions < 2 years old are allowed");
                    }

                    foreach (var preview in previewVersions)
                    {
                        var previewDate = preview.Date;

                        // Any preview version more recent than the most recent stable version and < 2 years old is allowed
                        if (DateIsRecent(preview.Date, data.Today, data.MaxAllowedAgeDays))
                        {
                            if (DateIsMoreRecentThan(previewDate, mostRecentStable.Date))
                            {
                                allowedVersions.Should().Contain(preview, "any preview version more recent than the most recent stable version and < 2 years old is allowed");
                            }
                        }

                        // If a preview version has a more recent or equally recent stable version, it is not allowed
                        if (stableVersions.Any(v => DateIsEqualOrMoreRecentThan(v.Date, previewDate)))
                        {
                            allowedVersions.Should().NotContain(preview, "if a preview version has a more recent or equally recent stable version, it is not allowed");
                        }
                    }
                }
                else
                {
                    // No stable versions

                    // If no stable versions at all, the most recent preview version is allowed, no matter its age
                    var mostRecentPreview = previewVersions.OrderBy(v => v.Date).Last();
                    allowedVersions.Should().Contain(mostRecentPreview, "if no stable versions at all, the most recent preview version is allowed, no matter its age");
                }

                // COROLLARIES TO THE IMPLEMENTATION RULES

                // If the most recent version is a preview version that’s > 2 years old, only that one preview version is allowed
                var mostRecent = allVersions.OrderBy(v => v.Date).Last();
                if (mostRecent.IsPreview && DateIsOld(mostRecent.Date, data.Today, data.MaxAllowedAgeDays))
                {
                    allowedVersions.Where(v => v.IsPreview).Select(v => v.Formatted).Should().BeEquivalentTo(new string[] { mostRecent.Formatted }, "if the most recent version is a preview version that’s > 2 years old, that one preview version is allowed");
                }

                // If there are no stable versions, all recent preview versions are allowed
                if (!stableVersions.Any())
                {
                    allowedVersions.Should().BeEquivalentTo(previewVersions.Where(v => DateIsRecent(v.Date, data.Today, data.MaxAllowedAgeDays)));
                }
            }
        }

        [TestClass]
        public class AnalyzeApiVersionTests
        {
            private static void Test(
                DateTime currentVersionDate,
                string currentVersionSuffix,
                DateTime[] gaVersionDates,
                DateTime[] previewVersionDates,
                (string message, string acceptableVersions, string replacement)? expectedFix)
            {
                string currentVersion = ApiVersionHelper.Format(currentVersionDate) + currentVersionSuffix;
                string[] gaVersions = gaVersionDates.Select(d => "Whoever.whatever/whichever@" + ApiVersionHelper.Format(d)).ToArray();
                string[] previewVersions = previewVersionDates.Select(d => "Whoever.whatever/whichever@" + ApiVersionHelper.Format(d) + "-preview").ToArray();
                var apiVersionProvider = new ApiVersionProvider(BicepTestConstants.Features, BicepTestConstants.NamespaceProvider);
                apiVersionProvider.InjectTypeReferences(ResourceScope.ResourceGroup, FakeResourceTypes.GetFakeResourceTypeReferences(gaVersions.Concat(previewVersions)));
                var result = UseRecentApiVersionRule.AnalyzeApiVersion(
                    apiVersionProvider,
                    DateTime.Today,
                    new TextSpan(17, 47),
                    new TextSpan(17, 47),
                    ResourceScope.ResourceGroup,
                    "Whoever.whatever/whichever",
                    new ApiVersion(currentVersion),
                    returnNotFoundDiagnostics: true);

                if (expectedFix == null)
                {
                    result.Should().BeNull();
                }
                else
                {
                    result.Should().NotBeNull();
                    result!.Span.Should().Be(new TextSpan(17, 47));
                    result.Message.Should().Be(expectedFix.Value.message);
                    (string.Join(", ", result.AcceptableVersions.Select(v => v.Formatted))).Should().Be(expectedFix.Value.acceptableVersions);
                    result.Fixes.Should().HaveCount(1); // Right now we only create one fix
                    result.Fixes[0].Replacements.Should().SatisfyRespectively(r => r.Span.Should().Be(new TextSpan(17, 47)));
                    result.Fixes[0].Replacements.Select(r => r.Text).Should().BeEquivalentTo(new string[] { expectedFix.Value.replacement });
                }
            }

            [TestMethod]
            public void WithCurrentVersionLessThanTwoYearsOld_ShouldNotAddDiagnostics()
            {
                DateTime currentVersionDate = DateTime.Today.AddDays(-1 * 365);
                DateTime recentGAVersionDate = DateTime.Today.AddDays(-5 * 31);

                Test(currentVersionDate, "", new DateTime[] { currentVersionDate, recentGAVersionDate }, new DateTime[] { },
                    null);
            }

            [TestMethod]
            public void WithCurrentVersionMoreThanTwoYearsOldAndRecentApiVersionIsAvailable_ShouldAddDiagnostics()
            {
                DateTime currentVersionDate = DateTime.Today.AddDays(-3 * 365);
                string currentVersion = ApiVersionHelper.Format(currentVersionDate);
                DateTime recentGAVersionDate = DateTime.Today.AddDays(-5 * 30);
                string recentGAVersion = ApiVersionHelper.Format(recentGAVersionDate);

                Test(currentVersionDate, "", new DateTime[] { currentVersionDate, recentGAVersionDate }, new DateTime[] { },
                    (
                        $"Use more recent API version for 'Whoever.whatever/whichever'. '{currentVersion}' is {3 * 365} days old, should be no more than 730 days old, or the most recent.",
                        acceptableVersions: recentGAVersion,
                        replacement: recentGAVersion
                    ));
            }

            [TestMethod]
            public void WithCurrentAndRecentApiVersionsMoreThanTwoYearsOld_ShouldAddDiagnosticsToUseRecentApiVersion()
            {
                DateTime currentVersionDate = DateTime.Today.AddDays(-4 * 365);
                string currentVersion = ApiVersionHelper.Format(currentVersionDate);

                DateTime recentGAVersionDate = DateTime.Today.AddDays(-3 * 365);
                string recentGAVersion = ApiVersionHelper.Format(recentGAVersionDate);

                Test(currentVersionDate, "", new DateTime[] { currentVersionDate, recentGAVersionDate }, new DateTime[] { },
                     (
                        $"Use more recent API version for 'Whoever.whatever/whichever'. '{currentVersion}' is {4 * 365} days old, should be no more than 730 days old, or the most recent.",
                        acceptableVersions: recentGAVersion,
                        replacement: recentGAVersion
                     ));
            }

            [TestMethod]
            public void WhenCurrentAndRecentApiVersionsAreSameAndMoreThanTwoYearsOld_ShouldNotAddDiagnostics()
            {
                DateTime currentVersionDate = DateTime.Today.AddDays(-3 * 365);

                Test(currentVersionDate, "", new DateTime[] { currentVersionDate }, new DateTime[] { },
                    null);
            }

            [TestMethod]
            public void WithPreviewVersion_WhenCurrentPreviewVersionIsLatest_ShouldNotAddDiagnostics()
            {
                DateTime currentVersionDate = DateTime.Today.AddDays(-365);

                DateTime recentGAVersionDate = DateTime.Today.AddDays(-3 * 365);

                DateTime recentPreviewVersionDate = currentVersionDate;


                Test(currentVersionDate, "", new DateTime[] { currentVersionDate, recentGAVersionDate }, new DateTime[] { recentPreviewVersionDate },
                     null);
            }

            [TestMethod]
            public void WithOldPreviewVersion_WhenRecentPreviewVersionIsAvailable_ButIsOlderThanGAVersion_ShouldAddDiagnosticsAboutBeingOld()
            {
                DateTime currentVersionDate = DateTime.Today.AddDays(-5 * 365);
                string currentVersion = ApiVersionHelper.Format(currentVersionDate, "-preview");

                DateTime recentGAVersionDate = DateTime.Today.AddDays(-1 * 365);
                string recentGAVersion = ApiVersionHelper.Format(recentGAVersionDate);

                DateTime recentPreviewVersionDate = DateTime.Today.AddDays(-2 * 365);

                Test(currentVersionDate, "-preview", new DateTime[] { recentGAVersionDate }, new DateTime[] { currentVersionDate, recentPreviewVersionDate },
                    (
                       $"Use more recent API version for 'Whoever.whatever/whichever'. '{currentVersion}' is {5 * 365} days old, should be no more than 730 days old, or the most recent.",
                       acceptableVersions: recentGAVersion,
                       replacement: recentGAVersion
                    ));
            }

            [TestMethod]
            public void WithPreviewVersion_WhenRecentPreviewVersionIsAvailable_AndIfNewerGAVersion_ShouldAddDiagnosticsToUseGA()
            {
                DateTime currentVersionDate = DateTime.Today.AddDays(-5 * 365);
                string currentVersion = ApiVersionHelper.Format(currentVersionDate, "-preview");

                DateTime recentGAVersionDate = DateTime.Today.AddDays(-3 * 365);
                string recentGAVersion = ApiVersionHelper.Format(recentGAVersionDate);

                DateTime recentPreviewVersionDate = DateTime.Today.AddDays(-2 * 365);
                string recentPreviewVersion = ApiVersionHelper.Format(recentPreviewVersionDate, "-preview");

                Test(currentVersionDate, "-preview", new DateTime[] { recentGAVersionDate }, new DateTime[] { currentVersionDate, recentPreviewVersionDate },
                    (

                       $"Use more recent API version for 'Whoever.whatever/whichever'. '{currentVersion}' is {5 * 365} days old, should be no more than 730 days old, or the most recent.",
                       acceptableVersions: $"{recentPreviewVersion}, {recentGAVersion}",
                       replacement: recentPreviewVersion // TODO recommend most recent, or just most recent GA version? Right now we always suggest the most recent
                    ));
            }

            [TestMethod]
            public void WithOldPreviewVersion_WhenRecentGAVersionIsAvailable_ShouldAddDiagnostics()
            {
                DateTime currentVersionDate = DateTime.Today.AddDays(-5 * 365);
                string currentVersion = ApiVersionHelper.Format(currentVersionDate);

                DateTime recentGAVersionDate = DateTime.Today.AddDays(-2 * 365);
                string recentGAVersion = ApiVersionHelper.Format(recentGAVersionDate);

                DateTime recentPreviewVersionDate = DateTime.Today.AddDays(-3 * 365);

                Test(currentVersionDate, "-preview", new DateTime[] { recentGAVersionDate }, new DateTime[] { currentVersionDate, recentPreviewVersionDate },
                  (
                     $"Use more recent API version for 'Whoever.whatever/whichever'. '{currentVersion}-preview' is {5 * 365} days old, should be no more than 730 days old, or the most recent.",
                     acceptableVersions: recentGAVersion,
                     replacement: recentGAVersion
                  ));
            }

            [TestMethod]
            public void WithRecentPreviewVersion_WhenRecentGAVersionIsAvailable_ShouldAddDiagnostics()
            {
                DateTime currentVersionDate = DateTime.Today.AddDays(-2 * 365);
                string currentVersion = ApiVersionHelper.Format(currentVersionDate);

                DateTime recentGAVersionDate = DateTime.Today.AddDays(-1 * 365);
                string recentGAVersion = ApiVersionHelper.Format(recentGAVersionDate);

                Test(currentVersionDate, "-preview", new DateTime[] { recentGAVersionDate }, new DateTime[] { currentVersionDate },
                  (
                     $"Use more recent API version for 'Whoever.whatever/whichever'. '{currentVersion}-preview' is a preview version and there is a more recent non-preview version available.",
                     acceptableVersions: recentGAVersion,
                     replacement: recentGAVersion
                  ));
            }

            [TestMethod]
            public void WithRecentPreviewVersion_WhenRecentGAVersionIsSameAsPreviewVersion_ShouldAddDiagnosticsUsingGAVersion()
            {
                DateTime currentVersionDate = DateTime.Today.AddDays(-2 * 365);
                string currentVersion = ApiVersionHelper.Format(currentVersionDate, "-preview");

                DateTime recentGAVersionDate = currentVersionDate;
                string recentGAVersion = ApiVersionHelper.Format(recentGAVersionDate);

                Test(currentVersionDate, "-preview", new DateTime[] { recentGAVersionDate }, new DateTime[] { currentVersionDate },
                 (
                    $"Use more recent API version for 'Whoever.whatever/whichever'. '{currentVersion}' is a preview version and there is a non-preview version available with the same date.",
                    acceptableVersions: recentGAVersion,
                    replacement: recentGAVersion
                 ));
            }

            [TestMethod]
            public void WithPreviewVersion_WhenGAVersionisNull_AndCurrentVersionIsNotRecent_ShouldAddDiagnosticsUsingRecentPreviewVersion()
            {
                DateTime currentVersionDate = DateTime.Today.AddDays(-3 * 365);
                string currentVersion = ApiVersionHelper.Format(currentVersionDate, "-preview");

                DateTime recentPreviewVersionDate = DateTime.Today.AddDays(-2 * 365);
                string recentPreviewVersion = ApiVersionHelper.Format(recentPreviewVersionDate, "-preview");

                Test(currentVersionDate, "-preview", new DateTime[] { }, new DateTime[] { recentPreviewVersionDate, currentVersionDate },
                    (
                       $"Use more recent API version for 'Whoever.whatever/whichever'. '{currentVersion}' is {3 * 365} days old, should be no more than 730 days old, or the most recent.",
                       acceptableVersions: recentPreviewVersion,
                      replacement: recentPreviewVersion
                    ));
            }

            [TestMethod]
            public void WithPreviewVersion_WhenGAVersionisNull_AndCurrentVersionIsRecent_ShouldNotAddDiagnostics()
            {
                DateTime currentVersionDate = DateTime.Today.AddDays(-2 * 365);

                DateTime recentPreviewVersionDate = currentVersionDate;

                Test(currentVersionDate, "-preview", new DateTime[] { }, new DateTime[] { recentPreviewVersionDate, currentVersionDate },
                    null);
            }
        }

        [TestClass]
        public class CompilationTests
        {
            [TestMethod]
            public void ArmTtk_ApiVersionIsNotAnExpression_Error()
            {
                string bicep = @"
                    resource publicIPAddress1 'fake.Network/publicIPAddresses@[concat(\'2020\', \'01-01\')]' = {
                      name: 'publicIPAddress1'
                      #disable-next-line no-loc-expr-outside-params
                      location: resourceGroup().location
                      tags: {
                        displayName: 'publicIPAddress1'
                      }
                      properties: {
                        publicIPAllocationMethod: 'Dynamic'
                      }
                    }";
                CompileAndTestWithFakeDateAndTypes(bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] { "[2] The resource type is not valid. Specify a valid resource type of format \"<types>@<apiVersion>\"." });
            }

            [TestMethod]
            public void NestedResources1_Fail()
            {
                string bicep = @"
                    param location string

                    resource namespace1 'fake.ServiceBus/namespaces@2418-01-01-preview' = {
                      name: 'namespace1'
                      location: location
                      properties: {
                      }
                    }

                    // Using 'parent'
                    resource namespace1_queue1 'fake.ServiceBus/namespaces/queues@2417-04-01' = {  // this is the latest stable version
                      parent: namespace1
                      name: 'queue1'
                    }

                    // Using 'parent'
                    resource namespace1_queue1_rule1 'fake.ServiceBus/namespaces/queues/authorizationRules@2415-08-01' = {
                      parent: namespace1_queue1
                      name: 'rule1'
                    }

                    // Using nested name
                    resource namespace1_queue2 'fake.ServiceBus/namespaces/queues@2417-04-01' = { // this is the latest stable version
                      name: 'namespace1/queue1'
                    }

                    // Using 'parent'
                    resource namespace1_queue2_rule2 'fake.ServiceBus/namespaces/queues/authorizationRules@2418-01-01-preview' = {
                      parent: namespace1_queue2
                      name: 'rule2'
                    }

                    // Using nested name
                    resource namespace1_queue2_rule3 'fake.ServiceBus/namespaces/queues/authorizationRules@4017-04-01' = {
                      name: 'namespace1/queue2/rule3'
                    }";

                CompileAndTestWithFakeDateAndTypes(
                    bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                        "[4] Use more recent API version for 'fake.ServiceBus/namespaces'. '2418-01-01-preview' is 1645 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2421-06-01-preview, 2421-01-01-preview, 2417-04-01",
                        "[18] Use more recent API version for 'fake.ServiceBus/namespaces/queues/authorizationRules'. '2415-08-01' is 2529 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2421-06-01-preview, 2421-01-01-preview, 2417-04-01",
                        "[29] Use more recent API version for 'fake.ServiceBus/namespaces/queues/authorizationRules'. '2418-01-01-preview' is 1645 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2421-06-01-preview, 2421-01-01-preview, 2417-04-01",
                        "[35] Could not find apiVersion 4017-04-01 for fake.ServiceBus/namespaces/queues/authorizationRules. Acceptable versions: 2421-06-01-preview, 2421-01-01-preview, 2417-04-01",
                    });
            }

            [TestMethod]
            public void NestedResources2_Fail()
            {
                string bicep = @"
                        param location string

                        // Using resource nesting
                        resource namespace2 'fake.ServiceBus/namespaces@2418-01-01-preview' = {
                          name: 'namespace2'
                          location: location

                          resource queue1 'queues@2415-08-01' = {
                            name: 'queue1'
                            location: location

                            resource rule1 'authorizationRules@2418-01-01-preview' = {
                              name: 'rule1'
                            }
                          }
                        }

                        // Using nested name (parent is a nested resource)
                        resource namespace2_queue1_rule2 'fake.ServiceBus/namespaces/queues/authorizationRules@2415-08-01' = {
                          name: 'namespace2/queue1/rule2'
                        }

                        // Using parent (parent is a nested resource)
                        resource namespace2_queue1_rule3 'fake.ServiceBus/namespaces/queues/authorizationRules@2415-08-01' = {
                          parent: namespace2::queue1
                          name: 'rule3'
                        }";

                CompileAndTestWithFakeDateAndTypes(
                    bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new[] {
                        "[5] Use more recent API version for 'fake.ServiceBus/namespaces'. '2418-01-01-preview' is 1645 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2421-06-01-preview, 2421-01-01-preview, 2417-04-01",
                        "[9] Use more recent API version for 'fake.ServiceBus/namespaces/queues'. '2415-08-01' is 2529 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2421-06-01-preview, 2421-01-01-preview, 2417-04-01",
                        "[13] Use more recent API version for 'fake.ServiceBus/namespaces/queues/authorizationRules'. '2418-01-01-preview' is 1645 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2421-06-01-preview, 2421-01-01-preview, 2417-04-01",
                        "[20] Use more recent API version for 'fake.ServiceBus/namespaces/queues/authorizationRules'. '2415-08-01' is 2529 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2421-06-01-preview, 2421-01-01-preview, 2417-04-01",
                        "[25] Use more recent API version for 'fake.ServiceBus/namespaces/queues/authorizationRules'. '2415-08-01' is 2529 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2421-06-01-preview, 2421-01-01-preview, 2417-04-01",
                    });
            }

            [TestMethod]
            public void ArmTtk_NotAString_Error()
            {
                string bicep = @"
                    resource publicIPAddress1 'fake.Network/publicIPAddresses@True' = {
                    name: 'publicIPAddress1'
                    #disable-next-line no-loc-expr-outside-params no-hardcoded-location
                    location: 'westus'
                    tags: {
                        displayName: 'publicIPAddress1'
                    }
                    properties: {
                        publicIPAllocationMethod: 'Dynamic'
                    }
                }
                ";

                CompileAndTestWithFakeDateAndTypes(
                    bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                      "[2] The resource type is not valid. Specify a valid resource type of format \"<types>@<apiVersion>\"."
                    });
            }

            [TestMethod]
            public void ArmTtk_PreviewWhenNonPreviewIsAvailable_WithSameDateAsStable_Fail()
            {
                string bicep = @"
                        resource db 'fake.DBforMySQL/servers@2417-12-01-preview' = {
                          name: 'db]'
                        #disable-next-line no-hardcoded-location
                          location: 'westeurope'
                          properties: {
                            administratorLogin: 'sa'
                            administratorLoginPassword: 'don\'t put passwords in plain text'
                            createMode: 'Default'
                            sslEnforcement: 'Disabled'
                          }
                        }
                    ";

                CompileAndTestWithFakeDateAndTypes(
                    bicep,
                    ResourceScope.ResourceGroup,
                    new string[]
                    {
                           "Fake.DBforMySQL/servers@2417-12-01",
                           "Fake.DBforMySQL/servers@2417-12-01-preview",
                    },
                    fakeToday: "2422-07-04",
                    new[] {
                        "[2] Use more recent API version for 'fake.DBforMySQL/servers'. '2417-12-01-preview' is 1676 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2417-12-01",
                    });
            }

            [TestMethod]
            public void ArmTtk_PreviewWhenNonPreviewIsAvailable_WithLaterDateThanStable_Fail()
            {
                string bicep = @"
                        resource db 'fake.DBforMySQL/servers@2417-12-01-preview' = {
                          name: 'db]'
                        #disable-next-line no-hardcoded-location
                          location: 'westeurope'
                          properties: {
                            administratorLogin: 'sa'
                            administratorLoginPassword: 'don\'t put passwords in plain text'
                            createMode: 'Default'
                            sslEnforcement: 'Disabled'
                          }
                        }
                    ";

                CompileAndTestWithFakeDateAndTypes(
                    bicep,
                    ResourceScope.ResourceGroup,
                    new string[]
                    {
                           "Fake.DBforMySQL/servers@2417-12-02",
                           "Fake.DBforMySQL/servers@2417-12-01-preview",
                    },
                    fakeToday: "2422-07-04",
                    new[] {
                        "[2] Use more recent API version for 'fake.DBforMySQL/servers'. '2417-12-01-preview' is 1676 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2417-12-02",
                    });
            }

            [TestMethod]
            public void ArmTtk_PreviewWhenNonPreviewIsAvailable_WithEarlierDateThanStable_Pass()
            {
                string bicep = @"
                    resource db 'fake.DBforMySQL/servers@2417-12-01-preview' = {
                        name: 'db]'
                    #disable-next-line no-hardcoded-location
                        location: 'westeurope'
                        properties: {
                        administratorLogin: 'sa'
                        administratorLoginPassword: 'don\'t put passwords in plain text'
                        createMode: 'Default'
                        sslEnforcement: 'Disabled'
                        }
                    }
                ";

                CompileAndTestWithFakeDateAndTypes(
                    bicep,
                    ResourceScope.ResourceGroup,
                    new string[]
                    {
                        "Fake.DBforMySQL/servers@2417-11-30",
                        "Fake.DBforMySQL/servers@2417-12-01-preview",
                    },
                    fakeToday: "2422-07-04",
                    new[] {
                        "[2] Use more recent API version for 'fake.DBforMySQL/servers'. '2417-12-01-preview' is 1676 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2417-11-30",
                    });
            }

            [TestMethod]
            public void ArmTtk_OnlyPreviewAvailable_EvenIfOld_Pass()
            {
                string bicep = @"
                       resource namespace 'Fake.DevTestLab/schedules@2417-08-01-preview' = {
                          name: 'namespace'
                          location: 'global'
                          properties: {
                          }
                       }";

                CompileAndTestWithFakeDateAndTypes(
                    bicep,
                    ResourceScope.ResourceGroup,
                    new string[]
                    {
                           "Fake.DevTestLab/schedules@2417-06-01-preview",
                           "Fake.DevTestLab/schedules@2417-08-01-preview",
                    },
                    fakeToday: "2422-07-04",
                    new string[] { });
            }

            [TestMethod]
            public void NewerPreviewAvailable_Fail()
            {
                string bicep = @"
                       resource namespace 'Fake.MachineLearningCompute/operationalizationClusters@2417-06-01-preview' = {
                          name: 'clusters'
                          location: 'global'
                       }";

                CompileAndTestWithFakeDateAndTypes(
                    bicep,
                    ResourceScope.ResourceGroup,
                    new string[]
                    {
                           "Fake.MachineLearningCompute/operationalizationClusters@2417-06-01-preview",
                           "Fake.MachineLearningCompute/operationalizationClusters@2417-08-01-preview",
                    },
                    fakeToday: "2422-07-04",
                    new[] {
                        "[2] Use more recent API version for 'Fake.MachineLearningCompute/operationalizationClusters'. '2417-06-01-preview' is 1859 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2417-08-01-preview",
                    });
            }

            [TestMethod]
            public void ExtensionResources_RoleAssignment_Pass()
            {
                // https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/scope-extension-resources#apply-to-resource
                /*
                    [-] apiVersions Should Be Recent (15 ms)
                    Api versions must be the latest or under 2 years old (730 days) - API version 2020-04-01-preview of Microsoft.Authorization/roleAssignments is 830 days old Line: 40, Column: 8
                    Valid Api Versions:
                    2018-07-01
                    2022-01-01-preview
                    2021-04-01-preview
                    2020-10-01-preview
                    2020-08-01-preview
                */
                CompileAndTestWithFakeDateAndTypes(@"
                        targetScope = 'subscription'

                        @description('The principal to assign the role to')
                        param principalId string

                        @allowed([
                          'Owner'
                          'Contributor'
                          'Reader'
                        ])
                        @description('Built-in role to assign')
                        param builtInRoleType string

                        var role = {
                          Owner: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/8e3af657-a8ff-443c-a75c-2fe8c4bcb635'
                          Contributor: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'
                          Reader: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/acdd72a7-3385-48ef-bd42-f606fba81ae7'
                        }

                        resource roleAssignSub 'fake.Authorization/roleAssignments@2420-04-01-preview' = {
                          name: guid(subscription().id, principalId, role[builtInRoleType])
                          properties: {
                            roleDefinitionId: role[builtInRoleType]
                            principalId: principalId
                          }
                        }",
                        ResourceScope.Subscription,
                        FakeResourceTypes.SubscriptionScopeTypes,
                       fakeToday: "2422-07-04",
                       new String[] {
                           "[21] Use more recent API version for 'fake.Authorization/roleAssignments'. '2420-04-01-preview' is 824 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2420-10-01-preview, 2420-08-01-preview, 2417-09-01"
                       });
            }

            [TestMethod]
            public void ExtensionResources_Lock_Pass()
            {
                // https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/scope-extension-resources#apply-to-resource
                CompileAndTestWithFakeDateAndTypes(@"
                       resource createRgLock 'Fake.Authorization/locks@2420-05-01' = {
                          name: 'rgLock'
                          properties: {
                            level: 'CanNotDelete'
                            notes: 'Resource group should not be deleted.'
                          }
                        }",
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] { });
            }

            [TestMethod]
            public void ExtensionResources_SubscriptionRole_Pass()
            {
                // https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/scope-extension-resources#apply-to-resource
                CompileAndTestWithFakeDateAndTypes(@"
                        targetScope = 'subscription'

                        @description('The principal to assign the role to')
                        param principalId string

                        @allowed([
                          'Owner'
                          'Contributor'
                          'Reader'
                        ])
                        @description('Built-in role to assign')
                        param builtInRoleType string

                        var role = {
                          Owner: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/8e3af657-a8ff-443c-a75c-2fe8c4bcb635'
                          Contributor: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'
                          Reader: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/acdd72a7-3385-48ef-bd42-f606fba81ae7'
                        }

                        resource roleAssignSub 'fake.Authorization/roleAssignments@2420-08-01-preview' = {
                          name: guid(subscription().id, principalId, role[builtInRoleType])
                          properties: {
                            roleDefinitionId: role[builtInRoleType]
                            principalId: principalId
                          }
                        }",
                        ResourceScope.Subscription,
                        FakeResourceTypes.SubscriptionScopeTypes,
                        "2422-07-04",
                        new string[] { }
                    );
            }

            [TestMethod]
            public void ExtensionResources_SubscriptionRole_Fail()
            {
                // https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/scope-extension-resources#apply-to-resource
                CompileAndTestWithFakeDateAndTypes(@"
                        targetScope = 'subscription'

                        @description('The principal to assign the role to')
                        param principalId string

                        @allowed([
                          'Owner'
                          'Contributor'
                          'Reader'
                        ])
                        @description('Built-in role to assign')
                        param builtInRoleType string

                        var role = {
                          Owner: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/8e3af657-a8ff-443c-a75c-2fe8c4bcb635'
                          Contributor: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'
                          Reader: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/acdd72a7-3385-48ef-bd42-f606fba81ae7'
                        }

                        resource roleAssignSub 'fake.Authorization/roleAssignments@2417-10-01-preview' = {
                          name: guid(subscription().id, principalId, role[builtInRoleType])
                          properties: {
                            roleDefinitionId: role[builtInRoleType]
                            principalId: principalId
                          }
                        }",
                        ResourceScope.Subscription,
                        FakeResourceTypes.SubscriptionScopeTypes,
                        "2422-07-04",
                        new[] {
                            "[21] Use more recent API version for 'fake.Authorization/roleAssignments'. '2417-10-01-preview' is 1737 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2420-10-01-preview, 2420-08-01-preview, 2417-09-01",
                        }
                    );
            }

            [TestMethod]
            public void ExtensionResources_ScopeProperty()
            {
                // https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/scope-extension-resources#apply-to-resource
                CompileAndTestWithFakeDateAndTypes(@"
                        @description('The principal to assign the role to')
                        param principalId string

                        @allowed([
                          'Owner'
                          'Contributor'
                          'Reader'
                        ])
                        @description('Built-in role to assign')
                        param builtInRoleType string

                        param location string = resourceGroup().location

                        var role = {
                          Owner: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/8e3af657-a8ff-443c-a75c-2fe8c4bcb635'
                          Contributor: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c'
                          Reader: '/subscriptions/${subscription().subscriptionId}/providers/Microsoft.Authorization/roleDefinitions/acdd72a7-3385-48ef-bd42-f606fba81ae7'
                        }
                        #disable-next-line no-loc-expr-outside-params
                        var uniqueStorageName = 'storage${uniqueString(resourceGroup().id)}'

                        // newer stable available
                        resource demoStorageAcct 'fake.Storage/storageAccounts@2420-08-01-preview' = {
                          name: uniqueStorageName
                          location: location
                          sku: {
                            name: 'Standard_LRS'
                          }
                          kind: 'Storage'
                          properties: {}
                        }

                        // old
                        resource roleAssignStorage 'fake.Authorization/roleAssignments@2420-04-01-preview' = {
                          name: guid(demoStorageAcct.id, principalId, role[builtInRoleType])
                          properties: {
                            roleDefinitionId: role[builtInRoleType]
                            principalId: principalId
                          }
                          scope: demoStorageAcct
                        }",
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new[] {
                        "[24] Use more recent API version for 'fake.Storage/storageAccounts'. '2420-08-01-preview' is a preview version and there is a more recent non-preview version available. Acceptable versions: 2421-06-01, 2421-04-01, 2421-02-01, 2421-01-01",
                        "[35] Use more recent API version for 'fake.Authorization/roleAssignments'. '2420-04-01-preview' is 824 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2420-10-01-preview, 2420-08-01-preview, 2415-07-01",
                    });
            }

            [TestMethod]
            public void ExtensionResources_ScopeProperty_ExistingResource_PartiallyPass()
            {
                // https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/scope-extension-resources#apply-to-resource
                CompileAndTestWithFakeDateAndTypes(@"
                        resource demoStorageAcct 'fake.Storage/storageAccounts@2421-04-01' existing = {
                          name: 'examplestore'
                        }

                        resource createStorageLock 'fake.Authorization/locks@2416-09-01' = {
                          name: 'storeLock'
                          scope: demoStorageAcct
                          properties: {
                            level: 'CanNotDelete'
                            notes: 'Storage account should not be deleted.'
                          }
                        }",
                     ResourceScope.ResourceGroup,
                     FakeResourceTypes.ResourceScopeTypes,
                     "2422-07-04",
                     new[] {
                        "[6] Use more recent API version for 'fake.Authorization/locks'. '2416-09-01' is 2132 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2420-05-01",
                     });
            }

            [TestMethod]
            public void SubscriptionDeployment_OldApiVersion_Fail()
            {
                CompileAndTestWithFakeDateAndTypes(@"
                        targetScope='subscription'

                        param resourceGroupName string
                        param resourceGroupLocation string

                        resource newRG 'fake.Resources/resourceGroups@2419-05-10' = {
                          name: resourceGroupName
                          location: resourceGroupLocation
                        }",
                    ResourceScope.Subscription,
                    FakeResourceTypes.SubscriptionScopeTypes,
                    "2422-07-04",
                    new[] {
                        "[7] Use more recent API version for 'fake.Resources/resourceGroups'. '2419-05-10' is 1151 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2421-05-01, 2421-04-01, 2421-01-01, 2420-10-01, 2420-08-01"
                    });
            }

            [TestMethod]
            public void SubscriptionDeployment_Pass()
            {
                CompileAndTestWithFakeDateAndTypes(@"
                        targetScope='subscription'

                        param resourceGroupName string
                        param resourceGroupLocation string

                        resource newRG 'fake.Resources/resourceGroups@2421-01-01' = {
                          name: resourceGroupName
                          location: resourceGroupLocation
                        }",
                    ResourceScope.Subscription,
                    FakeResourceTypes.SubscriptionScopeTypes,
                    "2422-07-04",
                    new string[] { });
            }

            [TestMethod]
            public void ResourceTypeNotFound_Error()
            {
                string bicep = @"
                       resource namespace 'DontKnowWho.MachineLearningCompute/operationalizationClusters@2417-06-01-preview' = {
                          name: 'clusters'
                          location: 'global'
                       }";

                CompileAndTestWithFakeDateAndTypes(
                    bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                new string[] {
                    "[2] Could not find resource type \"DontKnowWho.MachineLearningCompute/operationalizationClusters\".",
                });
            }

            [TestMethod]
            public void ApiVersionNotFound_Error()
            {
                string bicep = @"
                       resource namespace 'Fake.MachineLearningCompute/operationalizationClusters@2417-06-01-beta' = {
                          name: 'clusters'
                          location: 'global'
                       }";

                CompileAndTestWithFakeDateAndTypes(
                    bicep,
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                        "[2] Could not find apiVersion 2417-06-01-beta for Fake.MachineLearningCompute/operationalizationClusters. Acceptable versions: 2417-08-01-preview"
                    });
            }

            [TestMethod]
            public void ProvidesFixWithMostRecentVersion()
            {
                CompileAndTestFixWithFakeDateAndTypes(@"
                        targetScope='subscription'

                        param resourceGroupName string
                        param resourceGroupLocation string

                        resource newRG 'fake.Resources/resourceGroups@2419-05-10' = {
                          name: resourceGroupName
                          location: resourceGroupLocation
                        }",
                    ResourceScope.Subscription,
                    FakeResourceTypes.SubscriptionScopeTypes,
                    "2422-07-04",
                    new[] {
                        new DiagnosticAndFixes(
                            "Use more recent API version for 'fake.Resources/resourceGroups'. '2419-05-10' is 1151 days old, should be no more than 730 days old, or the most recent. Acceptable versions: 2421-05-01, 2421-04-01, 2421-01-01, 2420-10-01, 2420-08-01",
                            "Replace with 2421-05-01",
                            "resource newRG 'fake.Resources/resourceGroups@2421-05-01'"
                            )
                    });
            }

            [TestMethod]
            public void LinterIgnoresNotAzureResources()
            {
                CompileAndTestWithFakeDateAndTypes(@"
                        import kubernetes as kubernetes {
                          namespace: 'default'
                          kubeConfig: ''
                        }
                        resource service 'core/Service@v1' existing = {
                          metadata: {
                            name: 'existing-service'
                            namespace: 'default'
                            labels: {
                              format: 'k8s-extension'
                            }
                            annotations: {
                              foo: 'bar'
                            }
                          }
                        }
                    ",
                    ResourceScope.ResourceGroup,
                    FakeResourceTypes.ResourceScopeTypes,
                    "2422-07-04",
                    new string[] {
                       // pass
                    },
                    OnCompileErrors.Ignore);
            }
        }
    }
}
