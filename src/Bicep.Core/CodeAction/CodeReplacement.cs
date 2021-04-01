// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrint;
using Bicep.Core.Syntax;
using System.Text;

namespace Bicep.Core.CodeAction
{
    public class CodeReplacement : IPositionable
    {
        public CodeReplacement(TextSpan span, string text)
        {
            this.Span = span;
            this.Text = text;
        }

        public TextSpan Span { get; }

        public string Text { get; }

        public static CodeReplacement FromSyntax(TextSpan span, SyntaxBase syntax)
        {
            var sb = new StringBuilder();
            var documentBuildVisitor = new DocumentBuildVisitor();
            var document = documentBuildVisitor.BuildDocument(syntax);
            document.Layout(sb, "", System.Environment.NewLine);
            return new CodeReplacement(span, sb.ToString());
        }
    }
}
