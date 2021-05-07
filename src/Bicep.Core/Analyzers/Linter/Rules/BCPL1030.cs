// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.CodeAction;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class BCPL1030 : LinterRuleBase
    {
        public BCPL1030() : base(
            code: "BCPL1030",
            ruleName: "Secure parameter default not allowed",
            description: "Secure parameters can't have hardcoded default. This prevents storage of sensitive data in the Bicep declaration.",
            docUri: "https://bicep/linter/rules/BCPL1030") // TODO: setup up doc pages
        { }

        public override IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel model)
        {
            var defaultValueSyntaxes = model.Root.ParameterDeclarations.Where(p => p.IsSecure())
                .Select(p => p.Modifier as ParameterDefaultValueSyntax)
                .OfType<ParameterDefaultValueSyntax>(); // this eliminates nulls

            foreach (var defaultValueSyntax in defaultValueSyntaxes)
            {
                yield return CreateFixableDiagnosticForSpan(defaultValueSyntax.Span,
                    new CodeFix("Remove default value of secure parameter", true,
                            new CodeReplacement(defaultValueSyntax.Span, string.Empty)));
            }
        }
    }
}
