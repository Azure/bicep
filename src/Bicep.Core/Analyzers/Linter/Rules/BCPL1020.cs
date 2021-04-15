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
            ruleName: "Environment URL hardcoded",
            description: "Environment URLs should not be hardcoded",
            level: Diagnostics.DiagnosticLevel.Error,
            docUri: "https://bicep/linter/rules/BCPL1020")// TODO: setup up doc pages
        { }

        public override IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel model)
        {
            // TODO: Implement this
            yield break;
        }
    }
}
