// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Bicep.Core.ApiVersion;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class UseRecentApiVersionRule : LinterRuleBase
    {
        public new const string Code = "use-recent-api-version";

        private readonly ApiVersionProvider ApiVersionProvider;

        public UseRecentApiVersionRule() : base(
            code: Code,
            description: CoreResources.UseRecentApiVersionRuleDescription,
            docUri: new Uri("https://aka.ms/bicep/linter/no-unused-params"),
            diagnosticLabel: Diagnostics.DiagnosticLabel.Unnecessary)
        {
            ApiVersionProvider = new ApiVersionProvider();
        }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.UseRecentApiVersionRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            List<IDiagnostic> diagnostics = new();

            foreach (var resourceSymbol in model.Root.ResourceDeclarations)
            {
                if (resourceSymbol.TryGetResourceTypeReference() is ResourceTypeReference resourceTypeReference &&
                    resourceTypeReference.ApiVersion is string apiVersion &&
                    GetReplacementSpan(resourceSymbol, apiVersion) is TextSpan replacementSpan)
                {
                    DateTime currentApiVersionDate = ConvertApiVersionToDateTime(apiVersion);
                    string? recentNonPreviewApiVersion = ApiVersionProvider.GetRecentApiVersionDate(resourceTypeReference.FullyQualifiedType);

                    if (apiVersion.EndsWith("-preview"))
                    {
                        string? recentPreviewVersionWithoutPreviewPrefix = ApiVersionProvider.GetRecentApiVersionDate(resourceTypeReference.FullyQualifiedType, useNonApiVersionCache: false);

                        if (recentPreviewVersionWithoutPreviewPrefix is not null)
                        {
                            AddDiagnosticsIfPreviewVersionIsNotLatest(replacementSpan,
                                                                      recentPreviewVersionWithoutPreviewPrefix,
                                                                      recentNonPreviewApiVersion,
                                                                      currentApiVersionDate,
                                                                      diagnostics);
                        }
                    }
                    else
                    {
                        AddDiagnosticsIfNonPreviewVersionIsNotLatest(replacementSpan,
                                                                     recentNonPreviewApiVersion,
                                                                     currentApiVersionDate,
                                                                     diagnostics);
                    }
                }
            }

            return diagnostics;
        }

        private TextSpan? GetReplacementSpan(ResourceSymbol resourceSymbol, string apiVersion)
        {
            if (resourceSymbol.DeclaringResource.TypeString is StringSyntax typeString &&
                typeString.TryGetLiteralValue() is string value &&
                value is not null)
            {
                TextSpan typeSpan = typeString.Span;
                int replacementSpanStart = typeSpan.Position + value.IndexOf(apiVersion) + 1;

                return new TextSpan(replacementSpanStart, typeSpan.Position + typeSpan.Length - replacementSpanStart);
            }

            return null;
        }


        // A preview version is valid only if it is latest and there is no later non-preview version
        public void AddDiagnosticsIfPreviewVersionIsNotLatest(TextSpan span,
                                                              string recentPreviewVersionWithoutPreviewPrefix,
                                                              string? recentNonPreviewApiVersion,
                                                              DateTime currentApiVersionDate,
                                                              List<IDiagnostic> diagnostics)
        {
            DateTime recentPreviewVersionDate = ConvertApiVersionToDateTime(recentPreviewVersionWithoutPreviewPrefix);

            if (recentNonPreviewApiVersion is not null &&
                DateTime.Parse(recentNonPreviewApiVersion) is DateTime recentNonPreviewApiVersionDate &&
                DateTime.Compare(recentNonPreviewApiVersionDate, recentPreviewVersionDate) >= 0 &&
                DateTime.Compare(recentNonPreviewApiVersionDate, currentApiVersionDate) >= 0)
            {
                diagnostics.Add(CreateDiagnosticForSpan(span, recentNonPreviewApiVersion));
                return;
            }

            if (DateTime.Compare(recentPreviewVersionDate, currentApiVersionDate) > 0)
            {
                diagnostics.Add(CreateDiagnosticForSpan(span, recentPreviewVersionWithoutPreviewPrefix + "-preview"));
            }
        }

        // 1. Any non-preview version is allowed as long as it's not > 2 years old, even if there is a more recent non-preview version
        // 2. If there is no non preview apiVersion less than 2 years old, then take the latest one available from the cache of non preview versions
        public void AddDiagnosticsIfNonPreviewVersionIsNotLatest(TextSpan span,
                                                                 string? recentNonPreviewApiVersion,
                                                                 DateTime currentApiVersionDate,
                                                                 List<IDiagnostic> diagnostics)
        {
            if (recentNonPreviewApiVersion is null || DateTime.Now.Year - currentApiVersionDate.Year <= 2)
            {
                return;
            }

            DateTime recentNonPreviewApiVersionDate = DateTime.Parse(recentNonPreviewApiVersion);

            if (DateTime.Compare(recentNonPreviewApiVersionDate, currentApiVersionDate) > 0)
            {
                diagnostics.Add(CreateDiagnosticForSpan(span, recentNonPreviewApiVersion));
            }
        }

        private static DateTime ConvertApiVersionToDateTime(string apiVersion)
        {
            return DateTime.Parse(apiVersion.Split("-preview")[0]);
        }
    }
}
