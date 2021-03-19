// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using System.Collections.Generic;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    internal class BL1020 : LinterRule
    {
        internal BL1020() : base(
            code: "BL1020",
            ruleName: "Environment URl hardcoded",
            description: "Environment URLs can't be hardcoded",
            level: Diagnostics.DiagnosticLevel.Error,
            docUri: "https://bicep/linter/rules/BL1020")
        { }

        public override IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel model)
        {
            return base.Analyze(model);
        }
    }
}
