using System;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.Cli
{
    class PrintVisitor : SyntaxVisitor
    {
        private readonly Action<string> printFunc;
        private readonly bool printDelimiters;

        public PrintVisitor(Action<string> printFunc, bool printDelimiters)
        {
            this.printFunc = printFunc;
            this.printDelimiters = printDelimiters;
        }

        public override void VisitToken(Token token)
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