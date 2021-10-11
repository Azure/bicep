// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Bicep.Core.ApiVersion;
using Bicep.Core.Diagnostics;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;

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
            List<IDiagnostic> diagnostics = new List<IDiagnostic>();

            foreach (var resourceSymbol in model.Root.ResourceDeclarations)
            {
                if (resourceSymbol.TryGetResourceTypeReference() is ResourceTypeReference resourceTypeReference)
                {
                    string apiVersion = resourceTypeReference.ApiVersion;
                    if (ApiVersionProvider.GetRecentApiVersionDate(resourceTypeReference.FullyQualifiedType) is DateTime recentNonPreviewApiVersion)
                    {
                        if (apiVersion.EndsWith("-preview"))
                        {
                            if (ApiVersionProvider.ConvertApiVersionToDateTime(apiVersion) is DateTime currentApiVersion &&
                                DateTime.Compare(recentNonPreviewApiVersion, currentApiVersion) > 0)
                            {
                                diagnostics.Add(CreateDiagnosticForSpan(resourceSymbol.DeclaringResource.Type.Span, recentNonPreviewApiVersion.ToString()));
                            }

                        }
                        else
                        {

                        }
                    }
                }
            }

            return diagnostics;
        }
    }
}
