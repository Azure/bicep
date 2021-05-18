// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class ParametersRequiredRule : LinterRuleBase
    {
        public new const string Code = "parameters-required";
        public ParametersRequiredRule() : base(
            code: Code,
            description: CoreResources.ParameterRequiredRuleDescription,
            diagnosticLevel: Diagnostics.DiagnosticLevel.Warning,
            docUri: "https://aka.ms/linter-rules")
        {
        }

        override internal IEnumerable<IBicepAnalyzerDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            bool resourcesExist = model.Root.ResourceDeclarations.Any();
            bool parametersExist = model.Root.ParameterDeclarations.Any();

            if (resourcesExist && !parametersExist)
            {
                //Document level diagnostic set to position 0 for length of 0
                var span = new TextSpan(0, 0);
                yield return new AnalyzerDiagnostic(
                                    this.AnalyzerName,
                                    span,
                                    this.DiagnosticLevel,
                                    Code,
                                    this.GetMessage());
            }
        }
    }
}
