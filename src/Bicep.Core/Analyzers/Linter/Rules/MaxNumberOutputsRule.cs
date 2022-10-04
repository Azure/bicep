// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class MaxNumberOutputsRule : LinterRuleBase
    {
        public new const string Code = "max-outputs";
        public const int MaxNumber = 64;

        public MaxNumberOutputsRule() : base(
            code: Code,
            description: CoreResources.MaxNumberOutputsRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            diagnosticLevel: DiagnosticLevel.Error)
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.MaxNumberOutputsRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            if (model.Root.OutputDeclarations.Count() > MaxNumber)
            {
                var firstItem = model.Root.OutputDeclarations.First();
                return new IDiagnostic[] { CreateDiagnosticForSpan(GetDiagnosticLevel(model), firstItem.NameSyntax.Span, MaxNumber) };
            }
            return Enumerable.Empty<IDiagnostic>();
        }
    }
}
