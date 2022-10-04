// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class MaxNumberVariablesRule : LinterRuleBase
    {
        public new const string Code = "max-variables";
        public const int MaxNumber = 256;

        public MaxNumberVariablesRule() : base(
            code: Code,
            description: CoreResources.MaxNumberVariablesRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            diagnosticLevel: DiagnosticLevel.Error)
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.MaxNumberVariablesRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            if (model.Root.VariableDeclarations.Count() > MaxNumber)
            {
                var firstItem = model.Root.VariableDeclarations.First();
                return new IDiagnostic[] { CreateDiagnosticForSpan(GetDiagnosticLevel(model), firstItem.NameSyntax.Span, MaxNumber) };
            }
            return Enumerable.Empty<IDiagnostic>();
        }
    }
}
