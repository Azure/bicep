// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Data;
using System.Diagnostics;
using System.Web.Services.Description;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.PublicRegistry;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
using Json.Patch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Semver;
using Semver.Comparers;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    //asdfg search for "RecentApi"
    //asdfg test: using hashes instead of versions

    public sealed class UseRecentModuleVersionsRule : LinterRuleBase
    {
        public new const string Code = "use-recent-module-versions";

        public record Failure(
            TextSpan Span,
            string Message,
            string[] AcceptableVersions, //asdfg?
            CodeFix[] Fixes
        );

        public UseRecentModuleVersionsRule() : base(
            code: Code,
            description: CoreResources.UseRecentModuleVersionsRule_Description,
            LinterRuleCategory.BestPractice,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            overrideCategoryDefaultDiagnosticLevel: DiagnosticLevel.Off // many users prefer this to be off by default due to the noise
            )
        {
            Trace.WriteLine("Constructor");
        }

        public override string FormatMessage(params object[] values)
        {
            var message = (string)values[0];
            var acceptableVersions = (AzureResourceApiVersion[])values[1];

            var acceptableVersionsString = string.Join(", ", acceptableVersions);
            return message
                + (acceptableVersionsString.Any() ? " " + string.Format(CoreResources.UseRecentModuleVersionsRule_Description, acceptableVersionsString) : "");
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            return Enumerable.Empty<IDiagnostic>();
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, IServiceProvider? serviceProvider, DiagnosticLevel diagnosticLevel)
        {
            if (serviceProvider == null) //asdfg
            {
                yield break;
            }

            foreach (var (syntax, artifactResolutionInfo) in model.Compilation.SourceFileGrouping.ArtifactLookup
                .Where(entry => entry.Value.Origin == model.SourceFile))
            {
                //asdfg? artifactResolutionInfo.RequiresRestore
                //asdfg? if (artifactResolutionInfo.Result.IsSuccess(out Uri? success))

                var publicMcrPrefix = $"br:{LanguageConstants.BicepPublicMcrRegistry}/";
                if (artifactResolutionInfo.Reference is IOciArtifactReference ociReference
                    && ociReference.Registry.Equals(LanguageConstants.BicepPublicMcrRegistry, StringComparison.Ordinal)
                    && ociReference.Tag is string tag) {
                    //asdfg
                    var x = ociReference.Repository;
                    var y = serviceProvider.GetRequiredService<IPublicRegistryModuleMetadataProvider>();
                    var z = y.GetCachedModuleVersions(x);
                }
            }
            yield break;

            //int maxAgeInDays = GetConfigurationValue(model.Configuration.Analyzers, MaxAgeInDaysKey, DefaultMaxAgeInDays);

            ////asdfg?
            ////if (maxAgeInDays < MinimumValidMaxAgeInDays)
            ////{
            ////    yield return CreateDiagnosticForSpan(
            ////        diagnosticLevel,
            ////        new TextSpan(),
            ////        $"{Code}: Configuration value for {MaxAgeInDaysKey} is not valid: {maxAgeInDays}",
            ////        Array.Empty<AzureResourceApiVersion>());

            ////    maxAgeInDays = DefaultMaxAgeInDays;
            ////}

            //// Testing/debug: Warn if the API version is not found  asdfg?
            //var warnIfNotFound = GetConfigurationValue(model.Configuration.Analyzers, "test-warn-not-found", false);


            //foreach (var resource in model.mo.Where(r => r.IsAzResource))
            //{
            //    if (AnalyzeModuleReference(model, today, maxAgeInDays, resource.Symbol, warnIfNotFound: warnIfNotFound) is Failure failure)
            //    {
            //        yield return CreateFixableDiagnosticForSpan(
            //            diagnosticLevel,
            //            failure.Span,
            //            failure.Fixes,
            //            failure.Message,
            //            failure.AcceptableVersions);
            //    }
            //}

            //foreach (var callInfo in GetFunctionCallInfos(model))
            //{
            //    if (AnalyzeFunctionCall(model, today, maxAgeInDays, callInfo) is Failure failure)
            //    {
            //        yield return CreateFixableDiagnosticForSpan(
            //            diagnosticLevel,
            //            failure.Span,
            //            failure.Fixes,
            //            failure.Message,
            //            failure.AcceptableVersions);
            //    }
            //}
        }

        //private static Failure? AnalyzeModuleReference(SemanticModel model, ResourceSymbol resourceSymbol, bool warnIfNotFound)
        //{
        //    if (resourceSymbol.TryGetResourceTypeReference() is ResourceTypeReference resourceTypeReference &&
        //        resourceTypeReference.ApiVersion is string apiVersionString &&
        //        GetReplacementSpan(resourceSymbol, apiVersionString) is TextSpan replacementSpan)
        //    {
        //        if (AzureResourceApiVersion.TryParse(apiVersionString, out var apiVersion))
        //        {
        //            string fullyQualifiedResourceType = resourceTypeReference.FormatType();
        //            return AnalyzeVersion(
        //                model.ApiVersionProvider,
        //                today,
        //                maxAgeInDays,
        //                replacementSpan,
        //                replacementSpan,
        //                model.TargetScope,
        //                fullyQualifiedResourceType,
        //                apiVersion,
        //                returnNotFoundDiagnostics: warnIfNotFound);
        //        }
        //    }

        //    return null;
        //}

        //public static Failure? AnalyzeVersion(IPublicRegistryModuleMetadataProvider publicRegistryModuleMetadataProvider/*asdfg?*/, /*asdfg? int maxAgeInDays, */TextSpan errorSpan, TextSpan replacementSpan, string fullyQualifiedModulePath, string referencedVersion, bool returnNotFoundDiagnostics)
        //{
        //    var (allApiVersions, acceptableApiVersions) = GetAcceptableVersions(publicRegistryModuleMetadataProvider, fullyQualifiedModulePath, referencedVersion);
        //    if (!allApiVersions.Any())
        //    {
        //        // Resource type not recognized
        //        if (returnNotFoundDiagnostics)
        //        {
        //            IEnumerable<string> typeNames = apiVersionProvider.GetResourceTypeNames(scope);
        //            string? suggestion = SpellChecker.GetSpellingSuggestion(fullyQualifiedResourceType, typeNames);

        //            var message = string.Format(CoreResources.UseRecentApiVersionRule_UnknownType, fullyQualifiedResourceType);
        //            if (suggestion is not null)
        //            {
        //                message += " " + string.Format(CoreResources.UseRecentApiVersionRule_UnknownTypeSuggestion, suggestion); ;
        //            }
        //            return CreateFailureFromMessage(errorSpan, message);
        //        }

        //        return null;
        //    }

        //    Debug.Assert(acceptableApiVersions.Any(), $"There should always be at least one acceptable version for a valid resource type: {fullyQualifiedResourceType} (scope {scope})");
        //    if (acceptableApiVersions.Contains(actualApiVersion))
        //    {
        //        // Passed - version is acceptable
        //        return null;
        //    }

        //    if (!allApiVersions.Contains(actualApiVersion))
        //    {
        //        // apiVersion for resource type not recognized.
        //        if (returnNotFoundDiagnostics)
        //        {
        //            return CreateFailureFromApiVersion(
        //                errorSpan,
        //                replacementSpan,
        //                string.Format(CoreResources.UseRecentApiVersionRule_UnknownVersion, actualApiVersion, fullyQualifiedResourceType),
        //                acceptableApiVersions);
        //        }

        //        return null;
        //    }

        //    // At this point, the rule has failed. Just need to determine reason for failure, for the message.
        //    string? failureReason = null;

        //    // Is it because the version is recent but in preview, and there's a newer stable version available?
        //    if (actualApiVersion.IsPreview && IsRecent(actualApiVersion, today, maxAgeInDays))
        //    {
        //        var mostRecentStableVersion = GetNewestDateOrNull(FilterStable(allApiVersions));
        //        if (mostRecentStableVersion is not null)
        //        {
        //            var comparison = actualApiVersion.Date.CompareTo(mostRecentStableVersion.Value);
        //            var stableIsMoreRecent = comparison < 0;
        //            var stableIsSameDate = comparison == 0;
        //            if (stableIsMoreRecent)
        //            {
        //                failureReason = string.Format(CoreResources.UseRecentApiVersionRule_MoreRecentStable, actualApiVersion);
        //            }
        //            else if (stableIsSameDate)
        //            {
        //                failureReason = string.Format(CoreResources.UseRecentApiVersionRule_StableWithSameDate, actualApiVersion);
        //            }
        //        }
        //    }
        //    if (failureReason is null)
        //    {
        //        int ageInDays = today.DayNumber - actualApiVersion.Date.DayNumber;
        //        failureReason = string.Format(CoreResources.UseRecentApiVersionRule_TooOld, actualApiVersion, ageInDays, maxAgeInDays);
        //    }

        //    Debug.Assert(failureReason is not null);
        //    var failureMessage = string.Format(CoreResources.UseRecentApiVersionRule_ErrorMessageFormat, fullyQualifiedResourceType, failureReason);
        //    return CreateFailureFromApiVersion(
        //        errorSpan,
        //        replacementSpan,
        //        failureMessage,
        //        acceptableApiVersions);
        //}

        //asdfg test: preview versions
        public static (string[] allVersions, string[] acceptableVersions) GetAcceptableVersions(IPublicRegistryModuleMetadataProvider publicRegistryModuleMetadataProvider, string modulePath, string referencedVersion)
        {
            SemVersion requestedSemver = SemVersion.Parse(referencedVersion, SemVersionStyles.Strict); //what if fail?

            (string version, SemVersion semver)[] allVersions = publicRegistryModuleMetadataProvider.GetCachedModuleVersions(modulePath)
                .Select(v => (v.Version, SemVersion.Parse(v.Version, SemVersionStyles.Strict))) //asdfg what if fail?
                .ToArray();
            if (!allVersions.Any())
            {
                // The resource type is not recognized. //asdfg
                return (Array.Empty<string>(), Array.Empty<string>());
            }

            var sortedVersions = allVersions.OrderBy(v => v.semver, SemVersion.PrecedenceComparer).ToArray();
            var acceptableVersions = sortedVersions.Where(v => v.semver.ComparePrecedenceTo(requestedSemver) >= 0/*asdfg?*/).ToArray();

            return (allVersions.Select(v => v.version).ToArray(), acceptableVersions.Select(v => v.version).ToArray());
        }

        //// Find the portion of the resource.type@api-version string that corresponds to the api version asdfg
        //private static TextSpan? GetReplacementSpan(ResourceSymbol resourceSymbol, string apiVersion)
        //{
        //    if (resourceSymbol.DeclaringResource.TypeString is StringSyntax typeString &&
        //        typeString.StringTokens.First() is Token token)
        //    {
        //        int replacementSpanStart = token.Span.Position + token.Text.IndexOf(apiVersion);

        //        return new TextSpan(replacementSpanStart, apiVersion.Length);
        //    }

        //    return null;
        //}

        //private static Failure CreateFailureFromMessage(TextSpan span, string message)
        //{
        //    return new Failure(span, message, Array.Empty<string>(), Array.Empty<CodeFix>());
        //}

        //private static Failure CreateFailureFromModuleVersion(TextSpan errorSpan, TextSpan replacementSpan, string message, string[] acceptableVersionsSorted)
        //{
        //    CodeFix? fix = null;

        //    if (!replacementSpan.IsNil)
        //    {
        //        // For now, always choose the most recent for the suggested auto-fix
        //        var preferredVersion = acceptableVersionsSorted[0];
        //        var codeReplacement = new CodeReplacement(replacementSpan, preferredVersion);

        //        fix = new CodeFix(
        //            string.Format(CoreResources.UseRecentApiVersionRule_Fix_ReplaceApiVersion, preferredVersion),
        //            isPreferred: true,
        //            CodeFixKind.QuickFix,
        //            codeReplacement);
        //    }

        //    return new Failure(errorSpan, message, acceptableVersionsSorted, fix is null ? [] : [fix]);
        //}
    }
}
