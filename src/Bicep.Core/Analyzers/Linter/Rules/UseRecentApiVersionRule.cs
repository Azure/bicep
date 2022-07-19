// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bicep.Core.ApiVersion;
using Bicep.Core.CodeAction;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    //asdfg update
    // Adds linter rule to flag an issue when api version used in resource is not recent
    // 1. Any GA version is allowed as long as it's less than years old, even if there is a more recent GA version
    // 2. If there is no GA apiVersion less than 2 years old, then accept only the latest one GA version available
    // 3. A non-stable version (api version with any -* suffix, such as -preview) is accepted only if it is latest and there is no later GA version
    // 4. For non preview versions(e.g. alpha, beta, privatepreview and rc), order of preference is latest GA -> Preview -> Non Preview   asdf????
    public sealed class UseRecentApiVersionRule : LinterRuleBase
    {
        public new const string Code = "use-recent-api-version";
        public const int MaxAllowedAgeInDays = 365 * 2;

        private DateTime today = DateTime.Today; // Debug/test switch: Pretend today is a different date
        private bool warnNotFound = false; // Debug/test switch: Warn if the resource type or API version are not found

        public UseRecentApiVersionRule() : base(
            code: Code,
            description: CoreResources.UseRecentApiVersionRule_Description,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            diagnosticStyling: DiagnosticStyling.Default)
        {
        }

        public override void Configure(AnalyzersConfiguration config)
        {
            base.Configure(config);

            // Today's date can be changed to enable testing/debug scenarios
            string? testToday = this.GetConfigurationValue<string?>("test-today", null);
            if (testToday is not null)
            {
                this.today = ApiVersionHelper.ParseDate(testToday);
            }

            // Testing/debug: Warn if the resource type and/or API version are not found
            bool debugWarnNotFound = this.GetConfigurationValue<bool>("test-warn-not-found", false);
            this.warnNotFound = debugWarnNotFound == true;

        }

        public override string FormatMessage(params object[] values)
        {
            var resourceType = (string)values[0];
            var reason = (string)values[1];
            var acceptableVersions = (string[])values[2];
            var acceptableVersionsString = string.Join(", ", acceptableVersions);
            return
                string.Format(CoreResources.UseRecentApiVersionRule_MessageFormat, resourceType)
                + (" " + reason)
                + (acceptableVersionsString.Any() ? " " + string.Format(CoreResources.UseRecentApiVersionRule_AcceptableVersions, acceptableVersionsString) : "");
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var visitor = new Visitor(model, today, UseRecentApiVersionRule.MaxAllowedAgeInDays, warnNotFound);
            visitor.Visit(model.SourceFile.ProgramSyntax);

            return visitor.Failures.Select(fix => CreateFixableDiagnosticForSpan(fix.span, fix.fixes, fix.resourceType, fix.reason, fix.acceptableVersions));
        }

        public sealed class Visitor : SyntaxVisitor
        {
            internal readonly List<(TextSpan span, string resourceType, string reason, string[] acceptableVersions, CodeFix[] fixes)> Failures = new();

            private readonly IApiVersionProvider apiVersionProvider;
            private readonly SemanticModel model;
            private readonly DateTime today;
            private readonly int maxAllowedAgeInDays;
            private readonly bool warnNotFound;

            public Visitor(SemanticModel model, DateTime today, int maxAllowedAgeInDays, bool warnNotFound)
            {
                this.apiVersionProvider = model.ApiVersionProvider ?? new ApiVersionProvider();
                this.model = model;
                this.today = today;
                this.maxAllowedAgeInDays = maxAllowedAgeInDays;
                this.warnNotFound = warnNotFound;
            }

            public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax resourceDeclarationSyntax)
            {
                ResourceSymbol resourceSymbol = new ResourceSymbol(model.SymbolContext, resourceDeclarationSyntax.Name.IdentifierName, resourceDeclarationSyntax);

                if (resourceSymbol.TryGetResourceTypeReference() is ResourceTypeReference resourceTypeReference &&
                    resourceTypeReference.ApiVersion is string apiVersion &&
                    GetReplacementSpan(resourceSymbol, apiVersion) is TextSpan replacementSpan)
                {
                    string fullyQualifiedResourceType = resourceTypeReference.FormatType();
                    var failure = AnalyzeApiVersion(replacementSpan, model.TargetScope, fullyQualifiedResourceType, apiVersion);

                    if (failure is not null)
                    {
                        Failures.Add(failure.Value);
                    }
                }

                base.VisitResourceDeclarationSyntax(resourceDeclarationSyntax);
            }

            public (TextSpan span, string resourceType, string reason, string[] acceptableVersions, CodeFix[] fixes)? AnalyzeApiVersion(TextSpan replacementSpan, ResourceScope scope, string fullyQualifiedResourceType, string actualApiVersion)
            {
                (string? currentApiDate, _) = ApiVersionHelper.TryParse(actualApiVersion);
                if (currentApiDate is null)
                {
                    // The API version is not valid. Bicep will show an error, so we don't normally want to show anything else
                    return null;
                }

                var (allApiVersions, acceptableApiVersions) = GetAcceptableApiVersions(apiVersionProvider, today, maxAllowedAgeInDays, scope, fullyQualifiedResourceType);
                if (!allApiVersions.Any())
                {//asdfg testpoint
                    // Resource type not recognized. Bicep will show a warning, so we don't normally want to show anything else
                    if (warnNotFound)
                    {
                        return (replacementSpan, fullyQualifiedResourceType, $"Could not find resource type {fullyQualifiedResourceType}", Array.Empty<string>(), Array.Empty<CodeFix>());
                    }
                    return null;
                }

                Debug.Assert(acceptableApiVersions.Any(), $"There should always be at least one acceptable version for a valid resource type: {fullyQualifiedResourceType} (scope {scope})");
                if (acceptableApiVersions.Contains(actualApiVersion))
                {
                    // Passed - version is acceptable
                    return null;
                }

                if (!allApiVersions.Contains(actualApiVersion))
                {
                    // apiVersion for resource type not recognized. Bicep will show a warning, so we don't want to show anything else
                    if (warnNotFound)
                    {
                        return CreateFailure(replacementSpan, fullyQualifiedResourceType, actualApiVersion, $"Could not find apiVersion {actualApiVersion} for {fullyQualifiedResourceType}", acceptableApiVersions);
                    }

                    return null;
                }

                // At this point, the rule has failed. Question is, why...
                string? failureReason = null;

                // Is it because the version is recent but is preview, and there's a newer stable version available?
                if (ApiVersionHelper.IsPreviewVersion(actualApiVersion) && IsRecent(actualApiVersion, today, maxAllowedAgeInDays)/*asdfg ask brian*/)
                {
                    var mostRecentStableVersion = GetNewestDate(ApiVersionHelper.FilterNonPreview(allApiVersions));
                    if (mostRecentStableVersion is not null)
                    {
                        var comparison = ApiVersionHelper.CompareApiVersionDates(actualApiVersion, mostRecentStableVersion);
                        var stableIsMoreRecent = comparison < 0;
                        var stableIsSameDate = comparison == 0;
                        if (stableIsMoreRecent)
                        {
                            failureReason = $"'{actualApiVersion}' is a preview version and there is a more recent non-preview version available.";//asdfg what version recommend?
                        }
                        else if (stableIsSameDate)
                        {
                            failureReason = $"'{actualApiVersion}' is a preview version and there is a non-preview version available with the same date.";//asdfg what version recommend?
                        }
                    }
                }

                if (failureReason is null)
                {
                    int ageInDays = today.Subtract(ApiVersionHelper.ParseDate(actualApiVersion)).Days;
                    failureReason = $"'{actualApiVersion}' is {ageInDays} days old, should be no more than {maxAllowedAgeInDays} days old.";
                }

                Debug.Assert(failureReason is not null);
                return CreateFailure(
                    replacementSpan,
                    fullyQualifiedResourceType,
                    actualApiVersion,
                    failureReason,
                    acceptableApiVersions);
            }

            public static (string[] allApiVersions, string[] acceptableVersions) GetAcceptableApiVersions(IApiVersionProvider apiVersionProvider, DateTime today, int maxAllowedAgeInDays, ResourceScope scope, string fullyQualifiedResourceType)
            {
                var allVersionsSorted = apiVersionProvider.GetSortedValidApiVersions(scope, fullyQualifiedResourceType).ToArray();
                if (!allVersionsSorted.Any())
                {
                    // The resource type is not recognized.
                    return (allVersionsSorted, Array.Empty<string>());
                }

                var oldestAcceptableDate = GetOldestAcceptableDate(today, maxAllowedAgeInDays);

                var stableVersionsSorted = ApiVersionHelper.FilterNonPreview(allVersionsSorted).ToArray();
                var previewVersionsSorted = ApiVersionHelper.FilterPreview(allVersionsSorted).ToArray();

                var recentStableVersionsSorted = FilterRecentVersions(stableVersionsSorted, oldestAcceptableDate).ToArray();
                var recentPreviewVersionsSorted = FilterRecentVersions(previewVersionsSorted, oldestAcceptableDate).ToArray();

                // Start with all recent stable versions
                List<string> acceptableVersions = recentStableVersionsSorted.ToList();

                // If no recent stable versions, add the most recent stable version, if any
                if (!acceptableVersions.Any())
                {
                    acceptableVersions.AddRange(FilterMostRecentApiVersion(stableVersionsSorted));
                }

                // Add all recent (not old) preview versions that are newer than the newest stable version, if any
                var mostRecentStableDate = GetNewestDate(stableVersionsSorted);
                if (mostRecentStableDate is not null)
                {
                    Debug.Assert(stableVersionsSorted.Any(), "There should have been at least one stable version since mostRecentStableDate != null");
                    acceptableVersions.AddRange(FilterApiVersionsNewerThanDate(recentPreviewVersionsSorted, mostRecentStableDate));
                }
                else
                {
                    // There are no stable versions available at all - add all preview versions that are recent enough
                    acceptableVersions.AddRange(recentPreviewVersionsSorted);

                    // If there are no recent preview versions, add the newest preview only
                    if (!acceptableVersions.Any())
                    {
                        acceptableVersions.AddRange(FilterMostRecentApiVersion(previewVersionsSorted));
                        Debug.Assert(acceptableVersions.Any(), "There should have been at least one preview version available to add");
                    }
                }

                /*asdfg 
                 * // use a deterministic order
            var diagnostics = model.GetAllDiagnostics()
                .OrderBy(x => x.Span.Position)
                .ThenBy(x => x.Span.Length)
                .ThenBy(x => x.Message, StringComparer.Ordinal);*/
                acceptableVersions.Sort((v1, v2) =>
                {
                    // Sort by date descending, then stable first, then others alphabetically ascending
                    var dateCompare = ApiVersionHelper.CompareApiVersionDates(v1, v2);
                    if (dateCompare != 0)
                    {
                        return -dateCompare;
                    }

                    var v1IsStable = !ApiVersionHelper.IsPreviewVersion(v1);
                    var v2IsStable = !ApiVersionHelper.IsPreviewVersion(v2);
                    if (v1IsStable && !v2IsStable)
                    {
                        return -1;
                    }
                    else if (v2IsStable && !v2IsStable)
                    {
                        return 1;
                    }

                    return string.CompareOrdinal(v1, v2);
                });

                Debug.Assert(acceptableVersions.Any(), $"Didn't find any acceptable API versions for {fullyQualifiedResourceType}");
                return (allVersionsSorted, acceptableVersions.ToArray());
            }

            private TextSpan? GetReplacementSpan(ResourceSymbol resourceSymbol, string apiVersion)
            {
                if (resourceSymbol.DeclaringResource.TypeString is StringSyntax typeString &&
                    typeString.StringTokens.First() is Token token)
                {
                    int replacementSpanStart = token.Span.Position + token.Text.IndexOf(apiVersion);

                    return new TextSpan(replacementSpanStart, apiVersion.Length);
                }

                return null;
            }

            private (TextSpan span, string resourceType, string reason, string[] acceptableVersions, CodeFix[] fixes) CreateFailure(TextSpan span, string fullyQualifiedResourceType, string actualApiVersion, string reason, string[] acceptableApiVersions)
            {
                var preferredVersion = acceptableApiVersions[0];
                var codeReplacement = new CodeReplacement(span, preferredVersion);

                var fix = new CodeFix($"Replace apiVersion with {preferredVersion}", true, CodeFixKind.QuickFix, codeReplacement);

                return (span, fullyQualifiedResourceType, reason, acceptableApiVersions, new CodeFix[] { fix });
            }

            // Returns just the date string, not an entire apiVersion
            private static string? GetNewestDate(IEnumerable<string> apiVersions)
            {
                // We're safe to use Max on the apiVersion date strings since they're in the form yyyy-MM-dd, will give most recently since they're sorted ascending
                return apiVersions.Max(v => ApiVersionHelper.TryParse(v).date);
            }

            // Retrieves the most recent API version (this could be more than one if there are multiple apiVersions
            //   with the same, most recent date)
            private static IEnumerable<string> FilterMostRecentApiVersion(IEnumerable<string> apiVersions)
            {
                var mostRecentDate = GetNewestDate(apiVersions);
                if (mostRecentDate is not null)
                {
                    return FilterApiVersionsWithDate(apiVersions, mostRecentDate);
                }

                return Array.Empty<string>();
            }

            // Returns just the date string, not an entire apiVersion
            private static IEnumerable<string> FilterApiVersionsNewerThanDate(IEnumerable<string> apiVersions, string date)
            {
                return apiVersions.Where(v => ApiVersionHelper.CompareApiVersionDates(v, date) > 0);
            }

            private static IEnumerable<string> FilterApiVersionsNewerOrEqualToDate(IEnumerable<string> apiVersions, string date)
            {
                return apiVersions.Where(v => ApiVersionHelper.CompareApiVersionDates(v, date) >= 0);
            }

            // Returns just the date string, not an entire apiVersion
            private static IEnumerable<string> FilterApiVersionsWithDate(IEnumerable<string> apiVersions, string date)
            {
                return apiVersions.Where(v => ApiVersionHelper.CompareApiVersionDates(v, date) == 0);
            }

            private static bool IsRecent(string apiVersion, DateTime today, int maxAllowedAgeInDays)
            {
                //asdfg more efficient, just use dates...?
                var oldestAcceptableDate = GetOldestAcceptableDate(today, maxAllowedAgeInDays);
                return ApiVersionHelper.CompareApiVersionDates(apiVersion, oldestAcceptableDate) >= 0;
            }

            private static IEnumerable<string> FilterRecentVersions(IEnumerable<string> apiVersions, string lastAcceptableRecentDate)
            {
                return FilterApiVersionsNewerOrEqualToDate(apiVersions, lastAcceptableRecentDate);
            }

            private static string GetOldestAcceptableDate(DateTime today, int maxAllowedAgeInDays)
            {
                return ApiVersionHelper.Format(today.AddDays(-maxAllowedAgeInDays), null);
            }
        }
    }
}
