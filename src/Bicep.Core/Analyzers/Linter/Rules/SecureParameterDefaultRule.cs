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
    public sealed class SecureParameterDefaultRule : LinterRuleBase
    {
        public SecureParameterDefaultRule() : base(
            code: "Secure paramenter default",
            ruleName: "Secure parameter default not allowed",
            description: CoreResources.SecureParameterDefaultRuleDescription,
            docUri: "https://bicep/linter/rules/BCPL1030") // TODO: setup up doc pages
        { }

        override internal IEnumerable<IBicepAnalyzerDiagnostic> AnalyzeInternal(SemanticModel model)
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
