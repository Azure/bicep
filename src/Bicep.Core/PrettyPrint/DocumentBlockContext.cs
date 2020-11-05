// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;

namespace Bicep.Core.PrettyPrint
{
    public class DocumentBlockContext
    {
        public DocumentBlockContext(SyntaxBase? openSyntax, SyntaxBase? closeSyntax, SyntaxBase? firstNewLine, SyntaxBase? lastNewLine)
        {
            this.OpenSyntax = openSyntax;
            this.CloseSyntax = closeSyntax;
            this.FirstNewLine = firstNewLine;
            this.LastNewLine = lastNewLine;
        }

        public SyntaxBase? OpenSyntax { get; }

        public SyntaxBase? CloseSyntax { get; }

        public SyntaxBase? FirstNewLine { get; }

        public SyntaxBase? LastNewLine { get; }
    }
}
