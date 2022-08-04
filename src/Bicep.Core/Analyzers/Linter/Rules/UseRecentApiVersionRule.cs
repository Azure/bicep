// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.CodeAction;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class UseRecentApiVersionRule : LinterRuleBase
    {
        public new const string Code = "use-recent-api-versions";
        public const int MaxAllowedAgeInDays = 365 * 2;

        // Debug/test switch: Pretend today is a different date
        private DateTime today = DateTime.Today;

        // Debug/test switch: Warn if the resource type or API version are not found (normally we don't
        // give errors for these because Bicep always provides a warning about types not being available)
        private bool warnNotFound = false;

        public UseRecentApiVersionRule() : base(
            code: Code,
            description: CoreResources.UseRecentApiVersionRule_Description,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            diagnosticStyling: DiagnosticStyling.Default,
            diagnosticLevel: DiagnosticLevel.Off
        )
        {}

        public override void Configure(AnalyzersConfiguration config)
        {
            base.Configure(config);

            // Today's date can be changed to enable testing/debug scenarios
            string? testToday = this.GetConfigurationValue<string?>("test-today", null);
            if (testToday is not null)
            {
                this.today = ApiVersionHelper.ParseDateFromApiVersion(testToday);
            }

            // Testing/debug: Warn if the resource type and/or API version are not found
            bool debugWarnNotFound = this.GetConfigurationValue<bool>("test-warn-not-found", false);
            this.warnNotFound = debugWarnNotFound == true;

        }

        public override string FormatMessage(params object[] values)
        {
            var resourceType = (string)values[0];
            var reason = (string)values[1];
            var acceptableVersions = (ApiVersion[])values[2];

            var acceptableVersionsString = string.Join(", ", acceptableVersions.Select(v => v.Formatted));
            return string.Format(CoreResources.UseRecentApiVersionRule_ErrorMessageFormat, resourceType)
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
            internal readonly List<(TextSpan span, string resourceType, string reason, ApiVersion[] acceptableVersions, CodeFix[] fixes)> Failures = new();

            private readonly IApiVersionProvider apiVersionProvider;
            private readonly SemanticModel model;
            private readonly DateTime today;
            private readonly int maxAllowedAgeInDays;
            private readonly bool warnNotFound;

            public Visitor(SemanticModel model, DateTime today, int maxAllowedAgeInDays, bool warnNotFound)
            {
                this.apiVersionProvider = model.Compilation.ApiVersionProvider ?? new ApiVersionProvider();
                this.model = model;
                this.today = today;
                this.maxAllowedAgeInDays = maxAllowedAgeInDays;
                this.warnNotFound = warnNotFound;
            }

            public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax resourceDeclarationSyntax)
            {
                if ( model.GetSymbolInfo(resourceDeclarationSyntax) is not ResourceSymbol resourceSymbol)
                {
                    return;
                }

                if (model.DeclaredResources.FirstOrDefault(r => r.Symbol == resourceSymbol) is not DeclaredResourceMetadata declaredResourceMetadata
                    || !declaredResourceMetadata.IsAzResource)
                {
                    // Skip if it's not an Az resource or is invalid
                    return;
                }

                if (resourceSymbol.TryGetResourceTypeReference() is ResourceTypeReference resourceTypeReference &&
                    resourceTypeReference.ApiVersion is string apiVersionString &&
                    GetReplacementSpan(resourceSymbol, apiVersionString) is TextSpan replacementSpan)
                {
                    string fullyQualifiedResourceType = resourceTypeReference.FormatType();
                    var (date, suffix) = ApiVersionHelper.TryParse(apiVersionString);
                    if (date != null)
                    {
                        var failure = AnalyzeApiVersion(replacementSpan, model.TargetScope, fullyQualifiedResourceType, new ApiVersion(date, suffix));
                        if (failure is not null)
                        {
                            Failures.Add(failure.Value);
                        }
                    }
                }

                base.VisitResourceDeclarationSyntax(resourceDeclarationSyntax);
            }

            public (TextSpan span, string resourceType, string reason, ApiVersion[] acceptableVersions, CodeFix[] fixes)?
                AnalyzeApiVersion(TextSpan replacementSpan, ResourceScope scope, string fullyQualifiedResourceType, ApiVersion actualApiVersion)
            {
                var (allApiVersions, acceptableApiVersions) = GetAcceptableApiVersions(apiVersionProvider, today, maxAllowedAgeInDays, scope, fullyQualifiedResourceType);
                if (!allApiVersions.Any())
                {
                    // Resource type not recognized
                    if (warnNotFound)
                    {
                        return (replacementSpan, fullyQualifiedResourceType, $"Could not find resource type {fullyQualifiedResourceType}", Array.Empty<ApiVersion>(), Array.Empty<CodeFix>());
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
                    // apiVersion for resource type not recognized.
                    if (warnNotFound)
                    {
                        return CreateFailure(replacementSpan, fullyQualifiedResourceType, actualApiVersion, $"Could not find apiVersion {actualApiVersion.Formatted} for {fullyQualifiedResourceType}", acceptableApiVersions);
                    }

                    return null;
                }

                // At this point, the rule has failed. Just need to determine reason for failure, for the message.
                string? failureReason = null;

                // Is it because the version is recent but in preview, and there's a newer stable version available?
                if (actualApiVersion.IsPreview && IsRecent(actualApiVersion, today, maxAllowedAgeInDays))
                {
                    var mostRecentStableVersion = GetNewestDateOrNull(FilterStable(allApiVersions));
                    if (mostRecentStableVersion is not null)
                    {
                        var comparison = DateTime.Compare(actualApiVersion.Date, mostRecentStableVersion.Value);
                        var stableIsMoreRecent = comparison < 0;
                        var stableIsSameDate = comparison == 0;
                        if (stableIsMoreRecent)
                        {
                            failureReason = $"'{actualApiVersion.Formatted}' is a preview version and there is a more recent non-preview version available.";
                        }
                        else if (stableIsSameDate)
                        {
                            failureReason = $"'{actualApiVersion.Formatted}' is a preview version and there is a non-preview version available with the same date.";
                        }
                    }
                }
                if (failureReason is null)
                {
                    int ageInDays = today.Subtract(actualApiVersion.Date).Days;
                    failureReason = $"'{actualApiVersion.Formatted}' is {ageInDays} days old, should be no more than {maxAllowedAgeInDays} days old.";
                }

                Debug.Assert(failureReason is not null);
                return CreateFailure(
                    replacementSpan,
                    fullyQualifiedResourceType,
                    actualApiVersion,
                    failureReason,
                    acceptableApiVersions);
            }

            public static (ApiVersion[] allApiVersions, ApiVersion[] acceptableVersions) GetAcceptableApiVersions(IApiVersionProvider apiVersionProvider, DateTime today, int maxAllowedAgeInDays, ResourceScope scope, string fullyQualifiedResourceType)
            {
                var allVersions = apiVersionProvider.GetApiVersions(scope, fullyQualifiedResourceType).ToArray();
                if (!allVersions.Any())
                {
                    // The resource type is not recognized.
                    return (allVersions, Array.Empty<ApiVersion>());
                }

                var stableVersionsSorted = FilterStable(allVersions).OrderBy(v => v.Date).ToArray();
                var previewVersionsSorted = FilterPreview(allVersions).OrderBy(v => v.Date).ToArray();

                var recentStableVersionsSorted = FilterRecent(stableVersionsSorted, today, maxAllowedAgeInDays).ToArray();
                var recentPreviewVersionsSorted = FilterRecent(previewVersionsSorted, today, maxAllowedAgeInDays).ToArray();

                // Start with all recent stable versions
                List<ApiVersion> acceptableVersions = recentStableVersionsSorted.ToList();

                // If no recent stable versions, add the most recent stable version, if any
                if (!acceptableVersions.Any())
                {
                    acceptableVersions.AddRange(FilterMostRecentApiVersion(stableVersionsSorted));
                }

                // Add any recent (not old) preview versions that are newer than the newest stable version
                var mostRecentStableDate = GetNewestDateOrNull(stableVersionsSorted);
                if (mostRecentStableDate != null)
                {
                    Debug.Assert(stableVersionsSorted.Any(), "There should have been at least one stable version since mostRecentStableDate != null");
                    var previewsNewerThanMostRecentStable = recentPreviewVersionsSorted.Where(v => IsMoreRecentThan(v.Date, mostRecentStableDate.Value));
                    acceptableVersions.AddRange(previewsNewerThanMostRecentStable);
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

                // Sort
                var acceptableVersionsSorted = acceptableVersions
                    .OrderByDescending(v => v.Date) // first by date
                    .ThenBy(v => v.IsStable ? 0 : 1) // then stable/preview (stable first)
                    .ThenBy(v => v.Suffix, StringComparer.OrdinalIgnoreCase) // then alphabetically by suffix
                    .ToArray();

                Debug.Assert(acceptableVersions.Any(), $"Didn't find any acceptable API versions for {fullyQualifiedResourceType}");
                return (allVersions, acceptableVersionsSorted);
            }

            // Find the portion of the resource.type@api-version string that corresponds to the api version
            private static TextSpan? GetReplacementSpan(ResourceSymbol resourceSymbol, string apiVersion)
            {
                if (resourceSymbol.DeclaringResource.TypeString is StringSyntax typeString &&
                    typeString.StringTokens.First() is Token token)
                {
                    int replacementSpanStart = token.Span.Position + token.Text.IndexOf(apiVersion);

                    return new TextSpan(replacementSpanStart, apiVersion.Length);
                }

                return null;
            }

            private static (TextSpan span, string resourceType, string reason, ApiVersion[] acceptableVersionsSorted, CodeFix[] fixes)
            CreateFailure(TextSpan span, string fullyQualifiedResourceType, ApiVersion actualApiVersion, string reason, ApiVersion[] acceptableVersionsSorted)
            {
                // For now, always choose the most recent for the suggested auto-fix
                var preferredVersion = acceptableVersionsSorted[0];
                var codeReplacement = new CodeReplacement(span, preferredVersion.Formatted);

                var fix = new CodeFix(
                    string.Format(CoreResources.UseRecentApiVersionRule_Fix_ReplaceApiVersion, preferredVersion.Formatted),
                    isPreferred: true,
                    CodeFixKind.QuickFix,
                    codeReplacement);

                return (span, fullyQualifiedResourceType, reason, acceptableVersionsSorted, new CodeFix[] { fix });
            }

            private static DateTime? GetNewestDateOrNull(IEnumerable<ApiVersion> apiVersions)
            {
                return apiVersions.Any() ? apiVersions.Max(v => v.Date) : null;
            }

            // Retrieves the most recent API version (this could be more than one if there are multiple apiVersions
            //   with the same, most recent date, but different suffixes)
            private static IEnumerable<ApiVersion> FilterMostRecentApiVersion(IEnumerable<ApiVersion> apiVersions)
            {
                var mostRecentDate = GetNewestDateOrNull(apiVersions);
                if (mostRecentDate is not null)
                {
                    return FilterByDateEquals(apiVersions, mostRecentDate.Value);
                }

                return Array.Empty<ApiVersion>();
            }

            private static IEnumerable<ApiVersion> FilterByDateEquals(IEnumerable<ApiVersion> apiVersions, DateTime date)
            {
                Debug.Assert(date == date.Date);
                return apiVersions.Where(v => v.Date == date);
            }

            // Recent meaning < maxAllowedAgeInDays old
            private static bool IsRecent(ApiVersion apiVersion, DateTime today, int maxAllowedAgeInDays)
            {
                return apiVersion.Date >= today.AddDays(-maxAllowedAgeInDays);
            }

            // Recent meaning < maxAllowedAgeInDays old
            private static IEnumerable<ApiVersion> FilterRecent(IEnumerable<ApiVersion> apiVersions, DateTime today, int maxAllowedAgeInDays)
            {
                return apiVersions.Where(v => IsRecent(v, today, maxAllowedAgeInDays));
            }

            private static IEnumerable<ApiVersion> FilterPreview(IEnumerable<ApiVersion> apiVersions)
            {
                return apiVersions.Where(v => v.IsPreview);
            }

            private static IEnumerable<ApiVersion> FilterStable(IEnumerable<ApiVersion> apiVersions)
            {
                return apiVersions.Where(v => v.IsStable);
            }

            private static bool IsMoreRecentThan(DateTime date, DateTime other)
            {
                return DateTime.Compare(date, other) > 0;
            }
        }
    }
}
