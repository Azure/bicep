// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class MaxNumberOutputsRule : LinterRuleBase
    {
        public new const string Code = "max-outputs";
        // https://learn.microsoft.com/en-us/azure/azure-resource-manager/templates/best-practices#template-limits
        public const int MaxNumber = 64;

        public MaxNumberOutputsRule() : base(
            code: Code,
            description: CoreResources.MaxNumberOutputsRuleDescription,
            LinterRuleCategory.DeploymentError)
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.MaxNumberOutputsRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            if (model.Root.OutputDeclarations.Count() > MaxNumber)
            {
                var firstItem = model.Root.OutputDeclarations.First();
                return new IDiagnostic[] { CreateDiagnosticForSpan(diagnosticLevel, firstItem.NameSource.Span, MaxNumber) };
            }
            return [];
        }
    }
}
