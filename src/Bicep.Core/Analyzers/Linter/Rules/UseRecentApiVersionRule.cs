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
            docUri: new Uri("https://aka.ms/bicep/linter/no-unused-params"),
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
                    DateTime currentApiVersionDate = ConvertApiVersionToDateTime(apiVersion);
                    string? recentNonPreviewApiVersion = apiVersionProvider.GetRecentApiVersion(fullyQualifiedType);

                    if (apiVersion.EndsWith("-preview"))
                    {
                        string? recentPreviewVersionWithoutPreviewPrefix = apiVersionProvider.GetRecentApiVersion(fullyQualifiedType, useNonApiVersionCache: false);

                        if (recentPreviewVersionWithoutPreviewPrefix is not null)
                        {
                            AddCodeFixIfPreviewVersionIsNotLatest(replacementSpan,
                                                                      recentPreviewVersionWithoutPreviewPrefix,
                                                                      recentNonPreviewApiVersion,
                                                                      currentApiVersionDate);
                        }
                    }
                    else
                    {
                        AddCodeFixIfNonPreviewVersionIsNotLatest(replacementSpan,
                                                                     recentNonPreviewApiVersion,
                                                                     currentApiVersionDate);
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
            public void AddCodeFixIfPreviewVersionIsNotLatest(TextSpan span,
                                                                  string recentPreviewVersionWithoutPreviewPrefix,
                                                                  string? recentNonPreviewApiVersion,
                                                                  DateTime currentApiVersionDate)
            {
                DateTime recentPreviewVersionDate = ConvertApiVersionToDateTime(recentPreviewVersionWithoutPreviewPrefix);

                if (recentNonPreviewApiVersion is not null &&
                    DateTime.Parse(recentNonPreviewApiVersion) is DateTime recentNonPreviewApiVersionDate &&
                    DateTime.Compare(recentNonPreviewApiVersionDate, recentPreviewVersionDate) >= 0 &&
                    DateTime.Compare(recentNonPreviewApiVersionDate, currentApiVersionDate) >= 0)
                {
                    AddCodeFix(span, recentNonPreviewApiVersion);
                    return;
                }

                if (DateTime.Compare(recentPreviewVersionDate, currentApiVersionDate) > 0)
                {
                    AddCodeFix(span, recentPreviewVersionWithoutPreviewPrefix + "-preview");
                }
            }

            // 1. Any non-preview version is allowed as long as it's not > 2 years old, even if there is a more recent non-preview version
            // 2. If there is no non preview apiVersion less than 2 years old, then take the latest one available from the cache of non preview versions
            public void AddCodeFixIfNonPreviewVersionIsNotLatest(TextSpan span,
                                                                 string? recentNonPreviewApiVersion,
                                                                 DateTime currentApiVersionDate)
            {
                if (recentNonPreviewApiVersion is null || DateTime.Now.Year - currentApiVersionDate.Year <= 2)
                {
                    return;
                }

                DateTime recentNonPreviewApiVersionDate = DateTime.Parse(recentNonPreviewApiVersion);

                if (DateTime.Compare(recentNonPreviewApiVersionDate, currentApiVersionDate) > 0)
                {
                    AddCodeFix(span, recentNonPreviewApiVersion);
                }
            }

            private static DateTime ConvertApiVersionToDateTime(string apiVersion)
            {
                return DateTime.Parse(apiVersion.Split("-preview")[0]);
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
