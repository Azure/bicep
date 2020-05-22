using System;
using System.Linq;
using Bicep.Parser;
using Bicep.Syntax;

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

        private void VisitToken(Token token)
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

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            Visit(syntax.Parent);
            VisitToken(syntax.OpenSquare);
            Visit(syntax.Property);
            VisitToken(syntax.CloseSquare);
        }

        public override void VisitArraySyntax(ArraySyntax syntax)
        {
            VisitToken(syntax.OpenSquare);
            VisitSeparatedSyntaxList(syntax.Items);
            VisitToken(syntax.CloseSquare);
        }

        public override void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax)
        {
            Visit(syntax.LeftExpression);
            VisitToken(syntax.OperatorToken);
            Visit(syntax.RightExpression);
        }

        public override void VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax)
        {
            VisitToken(syntax.Literal);
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            Visit(syntax.Parent);
            VisitToken(syntax.OpenParen);
            VisitSeparatedSyntaxList(syntax.Arguments);
            VisitToken(syntax.CloseParen);
        }

        public override void VisitGroupingSyntax(GroupingSyntax syntax)
        {
            VisitToken(syntax.OpenParen);
            Visit(syntax.Expression);
            VisitToken(syntax.CloseParen);
        }

        public override void VisitIdentifierSyntax(IdentifierSyntax syntax)
        {
            VisitToken(syntax.Identifier);
        }

        public override void VisitInputDeclSyntax(InputDeclSyntax syntax)
        {
            VisitToken(syntax.InputKeyword);
            Visit(syntax.Type);
            Visit(syntax.Identifier);
            VisitToken(syntax.Semicolon);
        }

        public override void VisitNullLiteralSyntax(NullLiteralSyntax syntax)
        {
            VisitToken(syntax.Literal);
        }

        public override void VisitNumericLiteralSyntax(NumericLiteralSyntax syntax)
        {
            VisitToken(syntax.Literal);
        }

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            Visit(syntax.Identifier);
            VisitToken(syntax.Colon);
            Visit(syntax.Expression);
        }

        public override void VisitObjectSyntax(ObjectSyntax syntax)
        {
            VisitToken(syntax.OpenBrace);
            VisitSeparatedSyntaxList(syntax.Properties);
            VisitToken(syntax.CloseBrace);
        }

        public override void VisitOutputDeclSyntax(OutputDeclSyntax syntax)
        {
            VisitToken(syntax.OutputKeyword);
            Visit(syntax.Identifier);
            VisitToken(syntax.Colon);
            Visit(syntax.Expression);
            VisitToken(syntax.Semicolon);
        }

        public override void VisitProgramSyntax(ProgramSyntax syntax)
        {
            foreach (var statement in syntax.Statements)
            {
                Visit(statement);
            }
            VisitToken(syntax.EndOfFile);
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            Visit(syntax.Parent);
            VisitToken(syntax.Dot);
            Visit(syntax.Property);
        }

        public override void VisitResourceDeclSyntax(ResourceDeclSyntax syntax)
        {
            VisitToken(syntax.ResourceKeyword);
            Visit(syntax.Provider);
            Visit(syntax.Type);
            Visit(syntax.Identifier);
            VisitToken(syntax.Colon);
            Visit(syntax.Expression);
            VisitToken(syntax.Semicolon);
        }

        public override void VisitStringSyntax(StringSyntax syntax)
        {
            VisitToken(syntax.StringToken);
        }

        public override void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax)
        {
            VisitToken(syntax.Operator);
            Visit(syntax.Expression);
        }

       public override void VisitSeparatedSyntaxList(SeparatedSyntaxList syntax)
       {
           foreach (var (element, token) in syntax.GetPairedElements())
           {
               Visit(element);
               if (token != null)
               {
                   VisitToken(token);
               }
           }
       }

        public override void VisitVarDeclSyntax(VarDeclSyntax syntax)
        {
            VisitToken(syntax.VariableKeyword);
            Visit(syntax.Identifier);
            VisitToken(syntax.Colon);
            Visit(syntax.Expression);
            VisitToken(syntax.Semicolon);
        }

        /*
                public override void VisitErrorSyntax(ErrorSyntax syntax) {
            prevIndent := visitor.indent
            visitor.indent = 0
            fmt.Print("[ERR] ");
            for _, token := range syntax.Tokens {
                      VisitToken(token);
            }
            fmt.Print("[/ERR]");
            visitor.newLine();
            visitor.indent = prevIndent
        }
        */
    }
}