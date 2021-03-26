// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.CodeAction;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    internal class BCPL1060 : LinterRule
    {
        internal BCPL1060() : base(
            code: "BCPL1060",
            ruleName: "Dynamic variable used concat",
            description: "Dynamic variable should not use concat - string interpolation should be used.",
            docUri: "https://bicep/linter/rules/BCPL1060")
        { }


        private CodeReplacement GetCodeReplacement(TextSpan span)
            => new CodeReplacement(span, "BCPL1060 - this is the new code");

        override public IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel model)
        {
            var funcSymbols = model.Root.Declarations.OfType<VariableSymbol>()
                                    .Select(sym => sym.Value)
                                    .OfType<FunctionCallSyntax>();

            foreach (var func in funcSymbols.Where(f => f.Name.IdentifierName.Equals("concat", System.StringComparison.OrdinalIgnoreCase)))
            {
                var fix = CreateFix(func);
                yield return CreateFixableDiagnosticForSpan(func.Span, fix);
            }
        }

        private CodeFix CreateFix(FunctionCallSyntax func) =>
            new CodeFix("Use string interpolation", true, GetCodeReplacement(func));

        private CodeReplacement GetCodeReplacement(FunctionCallSyntax func)
        {
            IEnumerable<Token> stringTokens = func.Arguments.Cast<Token>();
            IEnumerable<string> literals = new List<string>();
            var stringSyntax = new StringSyntax(stringTokens, func.Arguments, literals);

            return new CodeReplacement(func.Span, "${arg1}{arg2}");
        }


        class ConcatInterpolationRewriterVisitor : SyntaxRewriteVisitor
        {
            protected override SyntaxBase RewriteInternal(SyntaxBase syntax)
            {
                return base.RewriteInternal(syntax);
            }
        }
    }
}
