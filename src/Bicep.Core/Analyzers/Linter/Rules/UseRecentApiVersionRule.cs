// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Comparers;
using Bicep.Core.Diagnostics;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class UseRecentApiVersionRule : LinterRuleBase
    {
        public new const string Code = "use-recent-api-version";
        public UseRecentApiVersionRule() : base(
            code: Code,
            description: CoreResources.UseRecentApiVersionRuleDescription,
            docUri: new System.Uri("https://aka.ms/bicep/linter/no-unused-params"),
            diagnosticLabel: Diagnostics.DiagnosticLabel.Unnecessary)
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.UseRecentApiVersionRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var resourceTypeReferences = model.Binder.NamespaceResolver.GetAvailableResourceTypes()
                .OrderByDescending(rt => rt.ApiVersion, ApiVersionComparer.Instance)
                .GroupBy(rt => rt.FullyQualifiedType)
                .Select(rt => rt.First())
                .OrderBy(rt => rt.FullyQualifiedType, StringComparer.OrdinalIgnoreCase)
                .ToList();

            List<IDiagnostic> diagnostics = new List<IDiagnostic>();

            foreach (var resourceSymbol in model.Root.ResourceDeclarations)
            {
                if (resourceSymbol.TryGetResourceTypeReference() is ResourceTypeReference resourceTypeReference)
                {
                    var recentApiVersion = resourceTypeReferences.First(x => x.FullyQualifiedType.Equals(resourceTypeReference.FullyQualifiedType)).ApiVersion;

                    if (DateTime.Compare(
                        DateTime.Parse(recentApiVersion),
                        DateTime.Parse(resourceTypeReference.ApiVersion)) > 0)
                    {
                        diagnostics.Add(CreateDiagnosticForSpan(resourceSymbol.DeclaringResource.Type.Span, recentApiVersion));
                    }
                }
            }

            return diagnostics;
        }
    }
}
