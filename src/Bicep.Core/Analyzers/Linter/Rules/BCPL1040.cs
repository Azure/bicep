// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using System.Collections.Generic;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    internal class BCPL1040 : LinterRule
    {
        internal BCPL1040() : base(
            code: "BCPL1040",
            ruleName: "Location set by parameter",
            description: "Best practice dictates that Location be set via parameter.",
            docUri: "https://bicep/linter/rules/BCPL1040")
        { }

        public override IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel model)
        {
            // TODO: Implement this
            yield break;

        }
    }
}
