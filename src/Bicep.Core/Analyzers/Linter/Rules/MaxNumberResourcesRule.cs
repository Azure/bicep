// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class MaxNumberResourcesRule : LinterRuleBase
    {
        public new const string Code = "max-resources";
        public const int MaxNumber = 800;

        public MaxNumberResourcesRule() : base(
            code: Code,
            description: CoreResources.MaxNumberResourcesRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            diagnosticLabel: Diagnostics.DiagnosticLabel.Unnecessary,
            diagnosticLevel: DiagnosticLevel.Error)
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.MaxNumberResourcesRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            if (model.Root.ResourceDeclarations.Count() > MaxNumber)
            {
                return model.Root.ResourceDeclarations.Select(param => CreateDiagnosticForSpan(param.NameSyntax.Span, MaxNumber));
            }
            return Enumerable.Empty<IDiagnostic>();
        }
    }
}
