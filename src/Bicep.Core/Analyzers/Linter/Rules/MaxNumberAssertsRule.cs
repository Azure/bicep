// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class MaxNumberAssertsRule : LinterRuleBase
    {
        public new const string Code = "max-asserts";
        public const int MaxNumber = 32;

        public MaxNumberAssertsRule() : base(
            code: Code,
            description: CoreResources.MaxNumberAssertsRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            diagnosticLevel: DiagnosticLevel.Error)
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.MaxNumberAssertsRuleMessageFormat, values);
        }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            if (model.Root.AssertDeclarations.Count() > MaxNumber)
            {
                var firstItem = model.Root.AssertDeclarations.First();
                yield return CreateDiagnosticForSpan(diagnosticLevel, firstItem.NameSource.Span, MaxNumber);
            }
        }
    }
}
