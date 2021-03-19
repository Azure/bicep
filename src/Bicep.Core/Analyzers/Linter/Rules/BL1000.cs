// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    internal class BL1000 : LinterRule
    {
        internal BL1000() : base(
            code: "BL1000",
            ruleName: "Parameters Required",
            description: "A valid template must be parameterized.",
            enableForCLI: true,
            enableForEdit: true,
            level: Diagnostics.DiagnosticLevel.Error,
            docUri: "https://bicep/linter/rules/BL1000")
        {
        }

        override public IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel model)
        {
            bool parametersExist = model.Root.Declarations.OfType<ParameterSymbol>().Any();

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
