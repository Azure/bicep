// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class MaxNumberParametersRule : LinterRuleBase
    {
        public new const string Code = "max-params";
        // https://learn.microsoft.com/en-us/azure/azure-resource-manager/templates/best-practices#template-limits
        public const int MaxNumber = 256;

        public MaxNumberParametersRule() : base(
            code: Code,
            description: CoreResources.MaxNumberParametersRuleDescription,
            LinterRuleCategory.DeploymentError)
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.MaxNumberParametersRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            if (model.Root.ParameterDeclarations.Count() > MaxNumber)
            {
                var firstItem = model.Root.ParameterDeclarations.First();
                return new IDiagnostic[] { CreateDiagnosticForSpan(diagnosticLevel, firstItem.NameSource.Span, MaxNumber) };
            }
            return [];
        }
    }
}
