namespace Bicep.Core.Syntax
{
    public abstract class SyntaxVisitor
    {
        public void Visit(SyntaxBase node)
        {
            node.Accept(this);
        }

        //public virtual void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax) { }

        public virtual void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax) { }

        public virtual void VisitIdentifierSyntax(IdentifierSyntax syntax) { }

        //public virtual void VisitGroupingSyntax(GroupingSyntax syntax) { }

        //public virtual void VisitFunctionCallSyntax(FunctionCallSyntax syntax) { }

        public virtual void VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax) { }

        //public virtual void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax) { }

        //public virtual void VisitArraySyntax(ArraySyntax syntax) { }

        //public virtual void VisitArrayAccessSyntax(ArrayAccessSyntax syntax) { }

        //public virtual void VisitVarDeclSyntax(VarDeclSyntax syntax) { }

        public virtual void VisitStringSyntax(StringSyntax syntax) { }

        //public virtual void VisitResourceDeclSyntax(ResourceDeclSyntax syntax) { }

        //public virtual void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax) { }

        public virtual void VisitProgramSyntax(ProgramSyntax syntax) { }

        //public virtual void VisitOutputDeclSyntax(OutputDeclSyntax syntax) { }

        //public virtual void VisitObjectSyntax(ObjectSyntax syntax) { }

        //public virtual void VisitObjectPropertySyntax(ObjectPropertySyntax syntax) { }

        public virtual void VisitNumericLiteralSyntax(NumericLiteralSyntax syntax) { }

        //public virtual void VisitSeparatedSyntaxList(SeparatedSyntaxList syntax) { }

        public virtual void VisitNullLiteralSyntax(NullLiteralSyntax syntax) { }

        public virtual void VisitSkippedTokensTriviaSyntax(SkippedTokensTriviaSyntax syntax) { }
    }
}