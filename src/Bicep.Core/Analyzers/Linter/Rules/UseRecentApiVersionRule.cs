// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.ApiVersion;
using Bicep.Core.CodeAction;
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

        public UseRecentApiVersionRule() : base(
            code: Code,
            description: CoreResources.UseRecentApiVersionRuleDescription,
            docUri: new Uri("https://aka.ms/bicep/linter/use-recent-api-version"),
            diagnosticLabel: Diagnostics.DiagnosticLabel.Unnecessary)
        {
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var spanFixes = new Dictionary<TextSpan, CodeFix>();
            var visitor = new Visitor(spanFixes, model);
            visitor.Visit(model.SourceFile.ProgramSyntax);

            return spanFixes.Select(kvp => CreateFixableDiagnosticForSpan(kvp.Key, kvp.Value));
        }

        public sealed class Visitor : SyntaxVisitor
        {
            private readonly IApiVersionProvider apiVersionProvider;
            private readonly SemanticModel model;
            private readonly Dictionary<TextSpan, CodeFix> spanFixes;

            public Visitor(Dictionary<TextSpan, CodeFix> spanFixes, SemanticModel model)
            {
                this.apiVersionProvider = model.ApiVersionProvider ?? new ApiVersionProvider();
                this.spanFixes = spanFixes;
                this.model = model;
            }

            public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax resourceDeclarationSyntax)
            {
                ResourceSymbol resourceSymbol = new ResourceSymbol(model.SymbolContext, resourceDeclarationSyntax.Name.IdentifierName, resourceDeclarationSyntax);

                if (resourceSymbol.TryGetResourceTypeReference() is ResourceTypeReference resourceTypeReference &&
                    resourceTypeReference.ApiVersion is string apiVersion &&
                    GetReplacementSpan(resourceSymbol, apiVersion) is TextSpan replacementSpan)
                {
                    string fullyQualifiedType = resourceTypeReference.FullyQualifiedType;
                    (string? currentApiVersion, string? prefix) = apiVersionProvider.GetApiVersionAndPrefix(apiVersion);

                    string? recentGAVersion = apiVersionProvider.GetRecentApiVersion(fullyQualifiedType, ApiVersionPrefixConstants.GA);

                    if (string.IsNullOrEmpty(prefix))
                    {
                        AddCodeFixIfGAVersionIsNotLatest(replacementSpan,
                                                         recentGAVersion,
                                                         currentApiVersion);
                    }
                    else
                    {
                        string? recentNonGAVersion = apiVersionProvider.GetRecentApiVersion(fullyQualifiedType, prefix);
                        string? recentPreviewVersion = apiVersionProvider.GetRecentApiVersion(fullyQualifiedType, ApiVersionPrefixConstants.Preview);

                        AddCodeFixIfNonGAVersionIsNotLatest(replacementSpan,
                                                            recentGAVersion,
                                                            recentPreviewVersion,
                                                            recentNonGAVersion,
                                                            prefix,
                                                            currentApiVersion);
                    }
                }

                base.VisitResourceDeclarationSyntax(resourceDeclarationSyntax);
            }

            private TextSpan? GetReplacementSpan(ResourceSymbol resourceSymbol, string apiVersion)
            {
                if (resourceSymbol.DeclaringResource.TypeString is StringSyntax typeString &&
                    typeString.TryGetLiteralValue() is string value &&
                    value is not null)
                {
                    TextSpan typeSpan = typeString.Span;
                    int replacementSpanStart = typeSpan.Position + value.IndexOf(apiVersion) + 1;

                    return new TextSpan(replacementSpanStart, typeSpan.Position + typeSpan.Length - replacementSpanStart - 1);
                }

                return null;
            }

            // A preview version is valid only if it is latest and there is no later non-preview version
            public void AddCodeFixIfNonGAVersionIsNotLatest(TextSpan span,
                                                            string? recentGAVersion,
                                                            string? recentPreviewVersion,
                                                            string? recentNonGAVersion,
                                                            string prefix,
                                                            string? currentApiVersionWithoutPrefix)
            {
                if (currentApiVersionWithoutPrefix is null)
                {
                    return;
                }

                DateTime currentApiVersionDate = DateTime.Parse(currentApiVersionWithoutPrefix);

                Dictionary<string, DateTime> prefixToRecentApiVersionDateMap = new Dictionary<string, DateTime>();

                if (prefix.Equals(ApiVersionPrefixConstants.Preview))
                {
                    if (recentGAVersion is not null)
                    {
                        prefixToRecentApiVersionDateMap.Add(recentGAVersion, DateTime.Parse(recentGAVersion));
                    }

                    if (recentPreviewVersion is not null)
                    {
                        prefixToRecentApiVersionDateMap.Add(recentPreviewVersion + prefix, DateTime.Parse(recentPreviewVersion));
                    }
                }
                else
                {
                    if (recentGAVersion is not null)
                    {
                        prefixToRecentApiVersionDateMap.Add(recentGAVersion, DateTime.Parse(recentGAVersion));
                    }

                    if (recentNonGAVersion is not null)
                    {
                        prefixToRecentApiVersionDateMap.Add(recentNonGAVersion + prefix, DateTime.Parse(recentNonGAVersion));
                    }

                    if (recentPreviewVersion is not null)
                    {
                        prefixToRecentApiVersionDateMap.Add(recentPreviewVersion + ApiVersionPrefixConstants.Preview, DateTime.Parse(recentPreviewVersion));
                    }
                }

                if (prefixToRecentApiVersionDateMap.Any())
                {
                    var sortedPrefixToRecentApiVersionDateMap = prefixToRecentApiVersionDateMap.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

                    KeyValuePair<string, DateTime> kvp = sortedPrefixToRecentApiVersionDateMap.First();

                    if (DateTime.Compare(kvp.Value, currentApiVersionDate) > 0)
                    {
                        AddCodeFix(span, kvp.Key);
                    }
                }
            }

            // 1. Any non-preview version is allowed as long as it's not > 2 years old, even if there is a more recent non-preview version
            // 2. If there is no non preview apiVersion less than 2 years old, then take the latest one available from the cache of non preview versions
            public void AddCodeFixIfGAVersionIsNotLatest(TextSpan span,
                                                         string? recentNonPreviewApiVersion,
                                                         string? currentApiVersion)
            {
                if (currentApiVersion is null || recentNonPreviewApiVersion is null)
                {
                    return;
                }

                DateTime currentApiVersionDate = DateTime.Parse(currentApiVersion);

                if (DateTime.Now.Year - currentApiVersionDate.Year <= 2)
                {
                    return;
                }

                DateTime recentNonPreviewApiVersionDate = DateTime.Parse(recentNonPreviewApiVersion);

                if (DateTime.Compare(recentNonPreviewApiVersionDate, currentApiVersionDate) > 0)
                {
                    AddCodeFix(span, recentNonPreviewApiVersion);
                }
            }

            private void AddCodeFix(TextSpan span, string apiVersion)
            {
                var codeReplacement = new CodeReplacement(span, apiVersion);
                string description = string.Format(CoreResources.UseRecentApiVersionRuleMessageFormat, apiVersion);
                var fix = new CodeFix(description, true, codeReplacement);
                spanFixes[span] = fix;
            }
        }
    }
}
