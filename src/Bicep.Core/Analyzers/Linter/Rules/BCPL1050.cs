// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    internal class BCPL1050 : LinterRule
    {
        internal BCPL1050() : base(
            code: "BCPL1050",
            ruleName: "Declared variable not used",
            description: "Declared variable encountered that is not used within scope.",
            docUri: "https://bicep/linter/rules/BCPL1050")
        { }

        override public IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel model)
        {
            // variables must have a reference of type VariableAccessSyntax
            var unreferencedVariables = model.Root.Declarations.OfType<VariableSymbol>()
                                    .Where(sym => !model.FindReferences(sym).OfType<VariableAccessSyntax>().Any());

            foreach(var sym in unreferencedVariables)
            {
                yield return CreateDiagnosticForSpan(sym.NameSyntax.Span);
            }

            // local variables must have a reference of type VariableAccessSyntax
            var unreferencedLocalVariables = model.Root.Declarations.OfType<LocalVariableSymbol>()
                        .Where(sym => !model.FindReferences(sym).OfType<VariableAccessSyntax>().Any());


            foreach (var sym in unreferencedLocalVariables)
            {
                yield return CreateDiagnosticForSpan(sym.NameSyntax.Span);
            }
        }


    }
}
