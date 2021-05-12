// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using System;
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
            diagnosticLevel: Diagnostics.DiagnosticLevel.Warning,
            docUri: "https://bicep/linter/rules/BCPL1000") //TODO: set up online documentation location
        {
        }

        override internal IEnumerable<IBicepAnalyzerDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            bool parametersExist = model.Root.ParameterDeclarations.Any();

            if (!parametersExist)
            {
                //Document level diagnostic set to position 0 for length of 0
                var span = new TextSpan(0, 0);
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
