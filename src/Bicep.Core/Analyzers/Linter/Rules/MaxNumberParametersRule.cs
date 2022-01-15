// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class MaxNumberParametersRule : LinterRuleBase
    {
        public new const string Code = "no-unused-params";
        public MaxNumberParametersRule() : base(
            code: Code,
            description: CoreResources.MaxNumberParametersRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            diagnosticLabel: Diagnostics.DiagnosticLabel.Unnecessary)
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.MaxNumberParametersRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            if (model.Root.ParameterDeclarations.Count() > 256) {
                return model.Root.ParameterDeclarations.Select(param => CreateDiagnosticForSpan(param.NameSyntax.Span, param.Name));
            }
            return Enumerable.Empty<IDiagnostic>();
        }
    }
}
