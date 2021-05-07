// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class BCPL1000 : LinterRuleBase
    {
        public BCPL1000() : base(
            code: "BCPL1000",
            ruleName: "Parameters Required",
            description: "A valid template must be parameterized.",
            enableForCLI: true,
            enableForEdit: true,
            diagnosticLevel: Diagnostics.DiagnosticLevel.Error,
            docUri: "https://bicep/linter/rules/BCPL1000") //TODO: set up online documentation location
        {
        }

        override public IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel model)
        {
            bool parametersExist = model.Root.ParameterDeclarations.Any();

            if (!parametersExist)
            {
                //TODO: what span is appropriate for a document level diagnostic
                var span = new TextSpan(0, 1);
                yield return new AnalyzerDiagnostic(
                                    this.AnalyzerName,
                                    span,
                                    this.DiagnosticLevel,
                                    this.Code,
                                    this.GetMessage());
            }
        }
    }
}
