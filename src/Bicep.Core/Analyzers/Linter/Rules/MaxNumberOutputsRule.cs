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
    public sealed class MaxNumberOutputsRule : LinterRuleBase
    {
        public new const string Code = "max-outputs";
        public MaxNumberOutputsRule() : base(
            code: Code,
            description: CoreResources.MaxNumberOutputsRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            diagnosticLabel: Diagnostics.DiagnosticLabel.Unnecessary,
            diagnosticLevel: DiagnosticLevel.Error)
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.MaxNumberOutputsRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            if (model.Root.OutputDeclarations.Count() > 256) {
                return model.Root.OutputDeclarations.Select(param => CreateDiagnosticForSpan(param.NameSyntax.Span, param.Name));
            }
            return Enumerable.Empty<IDiagnostic>();
        }
    }
}
