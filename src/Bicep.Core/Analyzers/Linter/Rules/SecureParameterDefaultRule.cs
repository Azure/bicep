// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.CodeAction;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class SecureParameterDefaultRule : LinterRuleBase
    {
        public new const string Code = "Secure paramenter default";

        public SecureParameterDefaultRule() : base(
            code: Code,
            description: CoreResources.SecureParameterDefaultRuleDescription,
            docUri: "https://github.com/Azure/bicep/tree/main/docs/rules/SecureParameterDefaultRule.md")
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
