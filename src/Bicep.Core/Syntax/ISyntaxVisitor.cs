// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public interface ISyntaxVisitor
    {
        void VisitArrayAccessSyntax(ArrayAccessSyntax syntax);

        void VisitArrayItemSyntax(ArrayItemSyntax syntax);

        void VisitArraySyntax(ArraySyntax syntax);

        void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax);

        void VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax);

        void VisitFunctionArgumentSyntax(FunctionArgumentSyntax syntax);

        void VisitFunctionCallSyntax(FunctionCallSyntax syntax);

        void VisitIdentifierSyntax(IdentifierSyntax syntax);

        void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax);

        void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax);

        void VisitNullLiteralSyntax(NullLiteralSyntax syntax);

        void VisitIntegerLiteralSyntax(IntegerLiteralSyntax syntax);

        void VisitObjectPropertySyntax(ObjectPropertySyntax syntax);

        void VisitObjectSyntax(ObjectSyntax syntax);

        void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax);

        void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax);

        void VisitParameterDefaultValueSyntax(ParameterDefaultValueSyntax syntax);

        void VisitParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax);

        void VisitProgramSyntax(ProgramSyntax syntax);

        void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax);

        void VisitResourceAccessSyntax(ResourceAccessSyntax syntax);

        void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax);

        void VisitSeparatedSyntaxList(SeparatedSyntaxList syntax);

        void VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax);

        void VisitStringSyntax(StringSyntax syntax);

        void VisitTargetScopeSyntax(TargetScopeSyntax syntax);

        void VisitTernaryOperationSyntax(TernaryOperationSyntax syntax);

        void VisitToken(Token token);

        void VisitTypeSyntax(TypeSyntax syntax);

        void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax);

        void VisitVariableAccessSyntax(VariableAccessSyntax syntax);

        void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax);

        void VisitLocalVariableSyntax(LocalVariableSyntax syntax);

        void VisitIfConditionSyntax(IfConditionSyntax syntax);

        void VisitForSyntax(ForSyntax syntax);

        void VisitForVariableBlockSyntax(ForVariableBlockSyntax syntax);

        void VisitDecoratorSyntax(DecoratorSyntax syntax);

        void VisitMissingDeclarationSyntax(MissingDeclarationSyntax syntax);

        void VisitImportDeclarationSyntax(ImportDeclarationSyntax syntax);
    }
}
