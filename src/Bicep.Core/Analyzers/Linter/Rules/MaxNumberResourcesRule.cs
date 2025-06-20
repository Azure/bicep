// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class MaxNumberResourcesRule : LinterRuleBase
    {
        public new const string Code = "max-resources";
        public const int MaxNumber = 800;

        public MaxNumberResourcesRule() : base(
            code: Code,
            description: CoreResources.MaxNumberResourcesRuleDescription,
            LinterRuleCategory.DeploymentError)
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.MaxNumberResourcesRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            if (model.DeclaredResources.Length > MaxNumber)
            {
                var firstItem = model.DeclaredResources.Where(r => r.Parent is null).First();
                return new IDiagnostic[] { CreateDiagnosticForSpan(diagnosticLevel, firstItem.Symbol.NameSource.Span, MaxNumber) };
            }
            return [];
        }
    }
}
