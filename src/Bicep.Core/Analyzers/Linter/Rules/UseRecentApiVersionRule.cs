// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class UseRecentApiVersionRule : LinterRuleBase
    {
        public new const string Code = "use-recent-api-versions";
        public const int MaxAllowedAgeInDays = 365 * 2;

        private static readonly Regex resourceTypeRegex = new(
            "^ [a-z]+\\.[a-z]+ (\\/ [a-z]+)+ $",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public record Failure(
            TextSpan Span,
            string Message,
            ApiVersion[] AcceptableVersions,
            CodeFix[] Fixes
        );

        public record FunctionCallInfo(
            FunctionCallSyntaxBase FunctionCallSyntax,
            string FunctionName,
            string? ResourceType,
            ApiVersion? ApiVersion);

        public UseRecentApiVersionRule() : base(
            code: Code,
            description: CoreResources.UseRecentApiVersionRule_Description,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            diagnosticStyling: DiagnosticStyling.Default,
            diagnosticLevel: DiagnosticLevel.Off
        )
        { }

        public override string FormatMessage(params object[] values)
        {
            var message = (string)values[0];
            var acceptableVersions = (ApiVersion[])values[1];

            var acceptableVersionsString = string.Join(", ", acceptableVersions.Select(v => v.Formatted));
            return message
                + (acceptableVersionsString.Any() ? " " + string.Format(CoreResources.UseRecentApiVersionRule_AcceptableVersions, acceptableVersionsString) : "");
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var diagnosticLevel = GetDiagnosticLevel(model);
            var today = DateTime.Today;
            // Today's date can be changed to enable testing/debug scenarios
            if (GetConfigurationValue<string?>(model.Configuration.Analyzers, "test-today", null) is string testToday)
            {
                today = ApiVersionHelper.ParseDateFromApiVersion(testToday);
            }
            // Testing/debug: Warn if the resource type and/or API version are not found
            var warnIfNotFound = GetConfigurationValue(model.Configuration.Analyzers, "test-warn-not-found", false);

            foreach (var resource in model.DeclaredResources.Where(r => r.IsAzResource))
            {
                if (AnalyzeResource(model, today, resource.Symbol, warnIfNotFound: warnIfNotFound) is Failure failure)
                {
                    yield return CreateFixableDiagnosticForSpan(
                        diagnosticLevel,
                        failure.Span,
                        failure.Fixes,
                        failure.Message,
                        failure.AcceptableVersions);
                }
            }

            foreach (var callInfo in GetFunctionCallInfos(model))
            {
                if (AnalyzeFunctionCall(model, today, callInfo) is Failure failure)
                {
                    yield return CreateFixableDiagnosticForSpan(
                        diagnosticLevel,
                        failure.Span,
                        failure.Fixes,
                        failure.Message,
                        failure.AcceptableVersions);
                }
            }
        }

        private static Failure? AnalyzeFunctionCall(SemanticModel model, DateTime today, FunctionCallInfo functionCallInfo)
        {
            if (functionCallInfo.ApiVersion.HasValue && functionCallInfo.ResourceType is not null)
            {
                return AnalyzeApiVersion(
                    model.ApiVersionProvider,
                    today,
                    errorSpan: functionCallInfo.FunctionCallSyntax.Span,
                    replacementSpan: null,
                    model.TargetScope,
                    functionCallInfo.ResourceType,
                    functionCallInfo.ApiVersion.Value,
                    // Since Bicep doesn't show a warning for API versions in function calls, we want to do it
                    returnNotFoundDiagnostics: true);
            }

            return null;
        }

        public static IEnumerable<FunctionCallInfo> GetFunctionCallInfos(SemanticModel model)
        {
            var referenceAndListFunctionCalls = LinterExpressionHelper.FindFunctionCallsByName(
                model,
                model.SourceFile.ProgramSyntax,
                AzNamespaceType.BuiltInName,
                "reference|(list.*)");

            return referenceAndListFunctionCalls.Select(fc => GetFunctionCallInfo(model, fc));
        }

        private static FunctionCallInfo GetFunctionCallInfo(SemanticModel model, FunctionCallSyntaxBase functionCallSyntax)
        {
            // Assumes we're working with resource or anything starting with list*, both of which have the format:
            //
            //   func(resourceType, apiVersion, ...)
            //     or
            //   func(resourceId(resourceType, ...), apiVersion, ...)
            //

            ApiVersion? apiVersion = null;
            string? resourceType = null;

            // resource type in first argument
            if (functionCallSyntax.Arguments.Length >= 1
                && functionCallSyntax.Arguments[0].Expression is SyntaxBase resourceTypeArgumentSyntax)
            {
                // Handle `reference(resourceId(<resourcetype>, ...), ...)`
                if (resourceType is null && resourceTypeArgumentSyntax is FunctionCallSyntaxBase functionCall)
                {
                    resourceType = TryGetResourceTypeIfResourceIdCall(model, functionCall);
                }

                // Handle `reference(<resourcetype>, ...)`
                resourceType ??= TryGetResourceTypeIfEvaluatesToStringLiteral(model, resourceTypeArgumentSyntax);

                // Handle `reference(<resource>.id, ...)`
                resourceType ??= TryGetResourceTypeIfSymbolicResourceId(model, resourceTypeArgumentSyntax);

                // Handle `reference('resourceName', ...)`
                resourceType ??= TryGetResourceTypeIfResourceName(model, resourceTypeArgumentSyntax);

                // Simplify resource type if it contains additional part
                if (resourceType is not null)
                {
                    resourceType = GetResourceTypeFromResourceId(model, resourceType);
                }
            }

            // apiVersion is in the optional 2nd argument of reference
            if (functionCallSyntax.Arguments.Length >= 2)
            {
                var apiVersionExpression = functionCallSyntax.Arguments[1].Expression;

                if (LinterExpressionHelper.TryGetEvaluatedStringLiteral(model, apiVersionExpression) is (string apiVersionString, StringSyntax apiVersionSyntax, _)
                    && ApiVersion.TryParse(apiVersionString) is ApiVersion apiVersion2)
                {
                    apiVersion = apiVersion2;
                }
            }

            return new FunctionCallInfo(functionCallSyntax, functionCallSyntax.Name.IdentifierName, resourceType, apiVersion);
        }

        private static string? TryGetResourceTypeIfResourceIdCall(SemanticModel model, FunctionCallSyntaxBase functionCallSyntax)
        {
            // Handle resourceId(<resourcetype>, ...)
            if (!functionCallSyntax.Name.IdentifierName.EqualsOrdinally("resourceId"))
            {
                return null;
            }

            // resourceId() has optional arguments at the beginning for subscription and resource group IDs that can't always be determined
            //   at build time, so look for the first argument that looks like a resource ID
            var argsAsStringLiterals = functionCallSyntax.Arguments.Select(x => LinterExpressionHelper.TryGetEvaluatedStringLiteral(model, x)).ToArray();
            for (int i = 0; i < functionCallSyntax.Arguments.Length; ++i)
            {
                if (LinterExpressionHelper.TryGetEvaluatedStringLiteral(model, functionCallSyntax.Arguments[i].Expression) is (string argLiteral, _, _))
                {
                    argLiteral = argLiteral.TrimEnd('/');

                    if (resourceTypeRegex.IsMatch(argLiteral))
                    {
                        // Now folder any following arguments that are also literals into this string separated by "/", e.g.:
                        //   (..., 'Microsoft.Compute/virtualMachineScaleSets', 'virtualMachines/runCommands', 'name', <non-string-literal-arg>, ...)
                        //     =>
                        //   'Microsoft.Compute/virtualMachineScaleSets/virtualMachines/runCommands/name'
                        string folderLiterals = argLiteral;
                        for (int j = i + 1; j < functionCallSyntax.Arguments.Length; ++j)
                        {
                            if (LinterExpressionHelper.TryGetEvaluatedStringLiteral(model, functionCallSyntax.Arguments[j].Expression) is (string argLiteral2, _, _))
                            {
                                folderLiterals += $"/{argLiteral2}";
                            }
                            else
                            {
                                break;
                            }
                        }

                        return folderLiterals;
                    }
                }
            }

            return null;
        }

        private static string? TryGetResourceTypeIfEvaluatesToStringLiteral(SemanticModel model, SyntaxBase expression)
        {
            if (LinterExpressionHelper.TryGetEvaluatedStringLiteral(model, expression)
                is (string resourceIdResTypeString, _, _))
            {
                if (resourceTypeRegex.IsMatch(resourceIdResTypeString))
                {
                    return resourceIdResTypeString;
                }
            }

            return null;
        }

        private static string? TryGetResourceTypeIfSymbolicResourceId(SemanticModel model, SyntaxBase expression)
        {
            if (expression is PropertyAccessSyntax propertyAccessSyntax
                && propertyAccessSyntax.BaseExpression is VariableAccessSyntax variableAccessSyntax
                && model.GetSymbolInfo(variableAccessSyntax) is ResourceSymbol resourceSymbol)
            {
                if (resourceSymbol.TryGetResourceTypeReference() is ResourceTypeReference resourceTypeReference)
                {
                    return resourceTypeReference.FormatType();
                }
            }

            return null;
        }

        private static string? TryGetResourceTypeFromResource(ResourceMetadata resourceMetadata)
        {
            if (resourceMetadata.IsAzResource)
            {
                return resourceMetadata.TypeReference.FormatType();
            }

            return null;
        }

        private static string? TryGetResourceTypeIfResourceName(SemanticModel model, SyntaxBase resourceNameExpression)
        {
            var foundResources = LinterExpressionHelper.TryFindResourceByNameExpression(model, resourceNameExpression);
            if (foundResources.Any())
            {
                return TryGetResourceTypeFromResource(foundResources.First());
            }

            return null;
        }

        private static string GetResourceTypeFromResourceId(SemanticModel model, string resourceId)
        {
            var resourceType = resourceId;
            var mostRecentValid = resourceId;
            while (resourceTypeRegex.IsMatch(resourceType))
            {
                if (model.ApiVersionProvider.GetApiVersions(model.TargetScope, resourceType).Any())
                {
                    // The resource type exists
                    return resourceType;
                }

                // Strip off last slash
                mostRecentValid = resourceType;
                resourceType = resourceType[0..resourceType.LastIndexOf('/')];
            }

            // Assume the most minimal valid resource type is correct
            return mostRecentValid;
        }

        private static Failure? AnalyzeResource(SemanticModel model, DateTime today, ResourceSymbol resourceSymbol, bool warnIfNotFound)
        {
            if (resourceSymbol.TryGetResourceTypeReference() is ResourceTypeReference resourceTypeReference &&
                resourceTypeReference.ApiVersion is string apiVersionString &&
                GetReplacementSpan(resourceSymbol, apiVersionString) is TextSpan replacementSpan)
            {
                var (date, suffix) = ApiVersionHelper.TryParse(apiVersionString);
                if (date is not null)
                {
                    string fullyQualifiedResourceType = resourceTypeReference.FormatType();
                    return AnalyzeApiVersion(
                        model.ApiVersionProvider,
                        today,
                        replacementSpan,
                        replacementSpan,
                        model.TargetScope,
                        fullyQualifiedResourceType,
                        new ApiVersion(date, suffix),
                        returnNotFoundDiagnostics: warnIfNotFound);
                }
            }

            return null;
        }

        public static Failure? AnalyzeApiVersion(IApiVersionProvider apiVersionProvider, DateTime today, TextSpan errorSpan, TextSpan? replacementSpan, ResourceScope scope, string fullyQualifiedResourceType, ApiVersion actualApiVersion, bool returnNotFoundDiagnostics)
        {
            var (allApiVersions, acceptableApiVersions) = GetAcceptableApiVersions(apiVersionProvider, today, MaxAllowedAgeInDays, scope, fullyQualifiedResourceType);
            if (!allApiVersions.Any())
            {
                // Resource type not recognized
                if (returnNotFoundDiagnostics)
                {
                    IEnumerable<string> typeNames = apiVersionProvider.GetResourceTypeNames(scope);
                    string? suggestion = SpellChecker.GetSpellingSuggestion(fullyQualifiedResourceType, typeNames);

                    var message = string.Format(CoreResources.UseRecentApiVersionRule_UnknownType, fullyQualifiedResourceType);
                    if (suggestion is not null)
                    {
                        message += " " + string.Format(CoreResources.UseRecentApiVersionRule_UnknownTypeSuggestion, suggestion); ;
                    }
                    return CreateFailureFromMessage(errorSpan, message);
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
                if (returnNotFoundDiagnostics)
                {
                    return CreateFailureFromApiVersion(
                        errorSpan,
                        replacementSpan,
                        string.Format(CoreResources.UseRecentApiVersionRule_UnknownVersion, actualApiVersion.Formatted, fullyQualifiedResourceType),
                        acceptableApiVersions);
                }

                return null;
            }

            // At this point, the rule has failed. Just need to determine reason for failure, for the message.
            string? failureReason = null;

            // Is it because the version is recent but in preview, and there's a newer stable version available?
            if (actualApiVersion.IsPreview && IsRecent(actualApiVersion, today, MaxAllowedAgeInDays))
            {
                var mostRecentStableVersion = GetNewestDateOrNull(FilterStable(allApiVersions));
                if (mostRecentStableVersion is not null)
                {
                    var comparison = DateTime.Compare(actualApiVersion.Date, mostRecentStableVersion.Value);
                    var stableIsMoreRecent = comparison < 0;
                    var stableIsSameDate = comparison == 0;
                    if (stableIsMoreRecent)
                    {
                        failureReason = string.Format(CoreResources.UseRecentApiVersionRule_MoreRecentStable, actualApiVersion.Formatted);
                    }
                    else if (stableIsSameDate)
                    {
                        failureReason = string.Format(CoreResources.UseRecentApiVersionRule_StableWithSameDate, actualApiVersion.Formatted);
                    }
                }
            }
            if (failureReason is null)
            {
                int ageInDays = today.Subtract(actualApiVersion.Date).Days;
                failureReason = string.Format(CoreResources.UseRecentApiVersionRule_TooOld, actualApiVersion.Formatted, ageInDays, MaxAllowedAgeInDays);
            }

            Debug.Assert(failureReason is not null);
            var failureMessage = string.Format(CoreResources.UseRecentApiVersionRule_ErrorMessageFormat, fullyQualifiedResourceType, failureReason);
            return CreateFailureFromApiVersion(
                errorSpan,
                replacementSpan,
                failureMessage,
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

        private static Failure CreateFailureFromMessage(TextSpan span, string message)
        {
            return new Failure(span, message, Array.Empty<ApiVersion>(), Array.Empty<CodeFix>());
        }

        private static Failure CreateFailureFromApiVersion(TextSpan errorSpan, TextSpan? replacementSpan, string message, ApiVersion[] acceptableVersionsSorted)
        {
            CodeFix? fix = null;
            if (replacementSpan is not null)
            {
                // For now, always choose the most recent for the suggested auto-fix
                var preferredVersion = acceptableVersionsSorted[0];
                var codeReplacement = new CodeReplacement(replacementSpan, preferredVersion.Formatted);

                fix = new CodeFix(
                    string.Format(CoreResources.UseRecentApiVersionRule_Fix_ReplaceApiVersion, preferredVersion.Formatted),
                    isPreferred: true,
                    CodeFixKind.QuickFix,
                    codeReplacement);
            }

            return new Failure(errorSpan, message, acceptableVersionsSorted, fix is null ? Array.Empty<CodeFix>() : new CodeFix[] { fix });
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
