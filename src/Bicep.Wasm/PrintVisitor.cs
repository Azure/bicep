using System;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.Wasm
{
    class PrintVisitor : SyntaxVisitor
    {
        private readonly Action<string> printFunc;

        public PrintVisitor(Action<string> printFunc)
        {
            this.printFunc = printFunc;
        }

        public override void VisitToken(Token token)
        {
            printFunc(token.LeadingTrivia);
            printFunc(token.Text);
            printFunc(token.TrailingTrivia);
        }
    }
}