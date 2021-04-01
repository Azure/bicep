// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using System.Collections.Generic;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    internal class BCPL1020 : LinterRule
    {
        internal BCPL1020() : base(
            code: "BCPL1020",
            ruleName: "Environment URl hardcoded",
            description: "Environment URLs can't be hardcoded",
            level: Diagnostics.DiagnosticLevel.Error,
            docUri: "https://bicep/linter/rules/BCPL1020")
        { }

        public override IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel model)
        {
            // TODO: Implement this
            yield break;

        }
    }
}
