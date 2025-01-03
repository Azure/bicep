// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class MaxNumberVariablesRule : LinterRuleBase
    {
        public new const string Code = "max-variables";
        // https://learn.microsoft.com/en-us/azure/azure-resource-manager/templates/best-practices#template-limits
        public const int MaxNumber = 512;

        public MaxNumberVariablesRule() : base(
            code: Code,
            description: CoreResources.MaxNumberVariablesRuleDescription,
            LinterRuleCategory.DeploymentError,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"))
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.MaxNumberVariablesRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            var variablesInCompiledTemplate = model.Root.VariableDeclarations.Count() +
                model.ImportClosureInfo.ImportedVariablesInClosure.Count();
            if (variablesInCompiledTemplate > MaxNumber)
            {
                var firstItem = model.Root.VariableDeclarations.First();
                return new IDiagnostic[] { CreateDiagnosticForSpan(diagnosticLevel, firstItem.NameSource.Span, MaxNumber) };
            }
            return [];
        }
    }
}
