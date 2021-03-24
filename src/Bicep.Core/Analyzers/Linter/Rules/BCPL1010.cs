// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    internal class BCPL1010 : LinterRule
    {
        internal BCPL1010() : base(
            code: "BCPL1010",
            ruleName: "Parameters must be used",
            description: "Declared parameter must be referenced within the document scope.",
            docUri: "https://bicep/linter/rules/BCPL1010")
        { }

        override public IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel model)
        {
            // parameters must have at least two references
            //  1) One reference will be the the paramater syntax declaration
            //  2) VariableAccessSyntax indicates a reference to the parameter
            var unreferencedParams = model.Root.Declarations.OfType<ParameterSymbol>()
                                    .Where(sym => !model.FindReferences(sym).OfType<VariableAccessSyntax>().Any());

            return unreferencedParams.Select(param => CreateDiagnosticForSpan(param.NameSyntax.Span));
        }


    }
}
