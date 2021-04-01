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
    internal class BCPL1030 : LinterRule
    {
        internal BCPL1030() : base(
            code: "BCPL1030",
            ruleName: "Secure parameter default not allowed",
            description: "Secure parameters can't have hardcoded default. This prevents storage of sensitive data in the Bicep declaration.",
            docUri: "https://bicep/linter/rules/BCPL1030")
        { }

        public override IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel model)
        {
            var parameters = model.Root.Declarations.OfType<ParameterSymbol>();

            foreach (var param in parameters.Where( p => IsSecure(p) && HasDefault(p)))
            {
                yield return new AnalyzerDiagnostic(
                                    this.AnalyzerName,
                                    param.DeclaringParameter.Span,
                                    this.DiagnosticLevel,
                                    this.Code,
                                    this.GetMessage());
            }
        }

        private bool HasDefault(ParameterSymbol p)
        {
            //TODO: how to figure out?
            return false;
        }

        private bool IsSecure(ParameterSymbol p)
        {
            // TODO: find value or no
            return false;
        }
    }
}
