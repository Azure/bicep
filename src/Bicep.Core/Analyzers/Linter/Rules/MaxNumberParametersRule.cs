// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class MaxNumberParametersRule : LinterRuleBase
    {
        public new const string Code = "max-params";
        public const int MaxNumber = 256;

        public MaxNumberParametersRule() : base(
            code: Code,
            description: CoreResources.MaxNumberParametersRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            diagnosticLevel: DiagnosticLevel.Error)
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.MaxNumberParametersRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            if (model.Root.ParameterDeclarations.Count() > MaxNumber)
            {
                var firstItem = model.Root.ParameterDeclarations.First();
                return new IDiagnostic[] { CreateDiagnosticForSpan(GetDiagnosticLevel(model), firstItem.NameSyntax.Span, MaxNumber) };
            }
            return Enumerable.Empty<IDiagnostic>();
        }
    }
}
