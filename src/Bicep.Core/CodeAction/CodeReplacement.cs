// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

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
            => new CodeReplacement(span, syntax.ToText());
    }
}
