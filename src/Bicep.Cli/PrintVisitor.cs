using System;
using Bicep.Parser;
using Bicep.Syntax;
using Bicep.Visitors;

namespace Bicep.Cli
{
    class PrintVisitor : TokenVisitor
    {
        private readonly Action<string> printFunc;
        private readonly bool printDelimiters;

        public PrintVisitor(Action<string> printFunc, bool printDelimiters)
        {
            this.printFunc = printFunc;
            this.printDelimiters = printDelimiters;
        }

        protected override void VisitToken(Token token)
        {
            if (printDelimiters)
            {
                printFunc("|<");
            }
            printFunc(token.LeadingTrivia);
            if (printDelimiters)
            {
                printFunc("|");
            }
            printFunc(token.Text);
            if (printDelimiters)
            {
                printFunc("|");
            }
            printFunc(token.TrailingTrivia);
            if (printDelimiters)
            {
                printFunc(">|");
            }
        }
    }
}