// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
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
    public sealed class UseRecentModuleVersionsRule : LinterRuleBase
    {
        public new const string Code = "use-recent-module-versions";

        public record Failure(
            TextSpan Span,
            string Message,
            string[] AcceptableVersions,
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
        }

        public override string FormatMessage(params object[] values)
        {
            var message = (string)values[0];
            var acceptableVersions = (string[])values[1];

            var acceptableVersionsString = string.Join(", ", acceptableVersions);
            return message
                + (acceptableVersionsString.Length > 0
                    // Currently we only support using the most recent version
                    ? " " + string.Format(CoreResources.UseRecentModulesVersionRule_MostRecentVersion, acceptableVersionsString)
                    : "");
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, IServiceProvider serviceProvider, DiagnosticLevel diagnosticLevel)
        {
            return GetFailures(model, serviceProvider, diagnosticLevel)
                        .Select(f => CreateFixableDiagnosticForSpan(diagnosticLevel, f.Span, f.Fixes, f.Message, f.AcceptableVersions));
        }

        private static IEnumerable<Failure> GetFailures(SemanticModel model, IServiceProvider serviceProvider, DiagnosticLevel diagnosticLevel)
        {
            var publicModuleMetadataProvider = serviceProvider.GetRequiredService<IPublicModuleMetadataProvider>();
            var hasShownDownloadWarning = false;

            foreach (var (syntax, artifactResolutionInfo) in model.SourceFileGrouping.ArtifactLookup
                .Where(entry => entry.Value.Origin == model.SourceFile
                    && entry.Value.Syntax is ModuleDeclarationSyntax moduleSyntax))
            {
                if (syntax is ModuleDeclarationSyntax moduleSyntax)
                {
                    var errorSpan = syntax.SourceSyntax.Span;

                    if (artifactResolutionInfo.Reference is IOciArtifactReference ociReference
                        && ociReference.Registry.Equals(LanguageConstants.BicepPublicMcrRegistry, StringComparison.Ordinal)
                        && ociReference.Tag is string tag)
                    {
                        if (TryRemoveBicepModuleNamePrefix(ociReference) is not string publicModulePath)
                        {
                            continue;
                        }

                        if (publicModuleMetadataProvider.DownloadError is string downloadError)
                        {
                            if (!hasShownDownloadWarning)
                            {
                                hasShownDownloadWarning = true;
                                yield return new Failure(errorSpan, string.Format(CoreResources.UseRecentModulesVersionRule_CouldNotDownload, downloadError), [], []);
                            }
                            continue;
                        }
                        else if (!publicModuleMetadataProvider.IsCached)
                        {
                            if (!hasShownDownloadWarning)
                            {
                                hasShownDownloadWarning = true;
                                yield return new Failure(errorSpan, CoreResources.UseRecentModulesVersionRule_NotCached, [], []);
                            }
                            continue;
                        }

                        foreach (var failure in AnalyzeBicepModule(publicModuleMetadataProvider, moduleSyntax, errorSpan, tag, publicModulePath))
                        {
                            yield return failure;
                        }
                    }
                }
            }
            yield break;
        }

        private static IEnumerable<Failure> AnalyzeBicepModule(IPublicModuleMetadataProvider publicModuleMetadataProvider, ModuleDeclarationSyntax moduleSyntax, TextSpan errorSpan, string tag, string publicModulePath)
        {
            var availableVersions = publicModuleMetadataProvider.GetModuleVersionsMetadata(publicModulePath)
                .Select(v => v.Version)
                .ToArray();
            if (availableVersions.Length == 0)
            {
                // If the module doesn't exist, we assume the compiler will flag as an error, no need for us to show anything in the linter.  Or else
                //   the cache is not up to date, again we ignore until the cache is updated.
                yield break;
            }

            var moreRecentVersions = GetMoreRecentModuleVersions(availableVersions, publicModulePath, tag);
            if (moreRecentVersions.Length > 0)
            {
                var mostRecentVersion = moreRecentVersions[0];
                var replacementSpan = TryGetReplacementSpan(moduleSyntax, tag);
                if (replacementSpan.HasValue) // if not found, should still fail, just not have a fix
                {
                    yield return CreateFailureWithFix(
                        errorSpan,
                        replacementSpan.Value,
                        string.Format(CoreResources.UseRecentModuleVersionRule_ErrorMessageFormat, publicModulePath),
                        [mostRecentVersion]);
                }
            }
        }

        private static string? TryRemoveBicepModuleNamePrefix(IOciArtifactReference ociReference)
        {
            var repoPath = ociReference.Repository;
            if (!repoPath.StartsWith(LanguageConstants.BicepPublicMcrPathPrefix, StringComparison.Ordinal))
            {
                return null;
            }

            return repoPath.Substring(LanguageConstants.BicepPublicMcrPathPrefix.Length);
        }

        public static string[] GetMoreRecentModuleVersions(string[] availableVersions, string modulePath, string referencedVersion)
        {
            if (!SemVersion.TryParse(referencedVersion, SemVersionStyles.Strict, out SemVersion? requestedSemver))
            {
                // Invalid semantic version
                return [];
            }

            if (!availableVersions.Any())
            {
                // The module name is not recognized.
                return [];
            }

            var availableParsedVersions = availableVersions
                .Select(v => (version: v, semVersion: SemVersion.Parse(v, SemVersionStyles.Strict)));

            return [.. availableParsedVersions.Where(v => v.semVersion.ComparePrecedenceTo(requestedSemver) > 0)
                .OrderByDescending(v => v.semVersion, SemVersion.PrecedenceComparer)
                .Select(v => v.version)];
        }

        // Find the portion of the module/path:version string that corresponds to the module version,
        //   i.e., the span of the text that should be replaced for a fix
        private static TextSpan? TryGetReplacementSpan(ModuleDeclarationSyntax syntax, string apiVersion)
        {
            if (syntax.Path is StringSyntax pathString &&
                pathString.StringTokens.FirstOrDefault() is Token token)
            {
                int replacementSpanStart = token.Span.Position + token.Text.IndexOf(apiVersion);

                return new TextSpan(replacementSpanStart, apiVersion.Length);
            }

            return null;
        }

        private static Failure CreateFailureWithFix(TextSpan errorSpan, TextSpan replacementSpan, string message, string[] acceptableVersionsSorted)
        {
            CodeFix? fix = null;

            if (!replacementSpan.IsNil)
            {
                // Choose the most recent for the suggested auto-fix
                var preferredVersion = acceptableVersionsSorted[0];
                var codeReplacement = new CodeReplacement(replacementSpan, preferredVersion);

                fix = new CodeFix( //asfdg run resx generator
                    string.Format(CoreResources.UseRecentModuleVersionRule_Fix_ReplaceWithMostRecent, preferredVersion),
                    isPreferred: true,
                    CodeFixKind.QuickFix,
                    codeReplacement);
            }

            return new Failure(errorSpan, message, acceptableVersionsSorted, fix is null ? [] : [fix]);
        }
    }
}
