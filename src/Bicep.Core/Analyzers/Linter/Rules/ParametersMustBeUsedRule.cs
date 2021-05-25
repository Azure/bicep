// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class ParametersMustBeUsedRule : LinterRuleBase
    {
        public new const string Code = "no-unused-params";
        public ParametersMustBeUsedRule() : base(
            code: Code,
            description: CoreResources.ParameterMustBeUsedRuleDescription,
            docUri: "https://aka.ms/bicep/linter/no-unused-params",
            diagnosticLabel: Diagnostics.DiagnosticLabel.Unnecessary)
        { }

        override public IEnumerable<IBicepAnalyzerDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            // parameters must have at least two references
            //  1) One reference will be the the paramater syntax declaration
            //  2) VariableAccessSyntax indicates a reference to the parameter
            var unreferencedParams = model.Root.ParameterDeclarations
                                    .Where(sym => !model.FindReferences(sym).OfType<VariableAccessSyntax>().Any());

            return unreferencedParams.Select(param => CreateDiagnosticForSpan(param.NameSyntax.Span));
        }
    }
}
