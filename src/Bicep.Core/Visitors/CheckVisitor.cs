using Bicep.Core.Syntax;

namespace Bicep.Core.Visitors
{
    class CheckVisitor : ErrorVisitor
    {
        public CheckVisitor(Scope globalScope)
            : base(globalScope)
        {
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            Visit(syntax.Parent);
            Visit(syntax.Property);
        }

        public override void VisitArraySyntax(ArraySyntax syntax)
        {
            VisitSeparatedSyntaxList(syntax.Items);
        }

        public override void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax)
        {
            Visit(syntax.LeftExpression);
            Visit(syntax.RightExpression);
        }

        public override void VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax)
        {
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            Visit(syntax.Parent);
        }

        public override void VisitGroupingSyntax(GroupingSyntax syntax)
        {
            Visit(syntax.Expression);
        }

        public override void VisitIdentifierSyntax(IdentifierSyntax syntax)
        {
        }

        public override void VisitInputDeclSyntax(InputDeclSyntax syntax)
        {
        }

        public override void VisitNullLiteralSyntax(NullLiteralSyntax syntax)
        {
        }

        public override void VisitNumericLiteralSyntax(NumericLiteralSyntax syntax)
        {
        }

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            Visit(syntax.Expression);
        }

        public override void VisitObjectSyntax(ObjectSyntax syntax)
        {
            VisitSeparatedSyntaxList(syntax.Properties);
        }

        public override void VisitOutputDeclSyntax(OutputDeclSyntax syntax)
        {
            Visit(syntax.Expression);
        }

        public override void VisitProgramSyntax(ProgramSyntax syntax)
        {
            foreach (var statement in syntax.Statements)
            {
                Visit(statement);
            }
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            Visit(syntax.Parent);
        }

        public override void VisitResourceDeclSyntax(ResourceDeclSyntax syntax)
        {
            Visit(syntax.Expression);
        }

        public override void VisitStringSyntax(StringSyntax syntax)
        {
        }

        public override void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax)
        {
            Visit(syntax.Expression);
        }

       public override void VisitSeparatedSyntaxList(SeparatedSyntaxList syntax)
       {
           foreach (var (element, token) in syntax.GetPairedElements())
           {
               Visit(element);
           }
       }

        public override void VisitVarDeclSyntax(VarDeclSyntax syntax)
        {
            Visit(syntax.Expression);
        }
    }
}