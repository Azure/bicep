// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class SimplifyJsonNullRule : LinterRuleBase
    {
        public new const string Code = "simplify-json-null";

        public SimplifyJsonNullRule() : base(
            code: Code,
            description: CoreResources.SimplifyJsonNullRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"))
        { }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            var spanFixes = new Dictionary<TextSpan, CodeFix>();
            var visitor = new Visitor(spanFixes, model);
            visitor.Visit(model.SourceFile.ProgramSyntax);

            return spanFixes.Select(kvp => CreateFixableDiagnosticForSpan(diagnosticLevel, kvp.Key, kvp.Value));
        }

        private sealed class Visitor : AstVisitor
        {
            private readonly Dictionary<TextSpan, CodeFix> spanFixes;
            private readonly SemanticModel model;

            public Visitor(Dictionary<TextSpan, CodeFix> spanFixes, SemanticModel model)
            {
                this.spanFixes = spanFixes;
                this.model = model;
            }
            public override void VisitFunctionCallSyntax(FunctionCallSyntax functionCallSyntax)
            {
                if (functionCallSyntax.NameEquals("json") &&
                    functionCallSyntax.Arguments.Length == 1 &&
                    functionCallSyntax.Arguments[0].Expression is StringSyntax argSyntax)
                {
                    var argValue = argSyntax.TryGetLiteralValue();
                    if (argValue?.Trim() == "null")
                    {
                        AddCodeFix(functionCallSyntax.Span, "null");
                    }
                }
            }

            private void AddCodeFix(TextSpan span, string name)
            {
                var codeReplacement = new CodeReplacement(span, name);
                var fix = new CodeFix(CoreResources.SimplifyJsonNullFixTitle, true, CodeFixKind.QuickFix, codeReplacement);
                spanFixes[span] = fix;
            }
        }
    }
}