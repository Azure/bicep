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

        void VisitMetadataDeclarationSyntax(MetadataDeclarationSyntax syntax);

        void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax);

        void VisitNullLiteralSyntax(NullLiteralSyntax syntax);

        void VisitIntegerLiteralSyntax(IntegerLiteralSyntax syntax);

        void VisitObjectPropertySyntax(ObjectPropertySyntax syntax);

        void VisitObjectSyntax(ObjectSyntax syntax);

        void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax);

        void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax);

        void VisitParameterAssignmentSyntax(ParameterAssignmentSyntax syntax);

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

        void VisitResourceTypeSyntax(ResourceTypeSyntax syntax);

        void VisitObjectTypeSyntax(ObjectTypeSyntax syntax);

        void VisitObjectTypePropertySyntax(ObjectTypePropertySyntax syntax);

        void VisitTupleTypeSyntax(TupleTypeSyntax syntax);

        void VisitTupleTypeItemSyntax(TupleTypeItemSyntax syntax);

        void VisitArrayTypeSyntax(ArrayTypeSyntax syntax);

        void VisitArrayTypeMemberSyntax(ArrayTypeMemberSyntax syntax);

        void VisitUnionTypeSyntax(UnionTypeSyntax syntax);

        void VisitUnionTypeMemberSyntax(UnionTypeMemberSyntax syntax);

        void VisitTypeDeclarationSyntax(TypeDeclarationSyntax syntax);

        void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax);

        void VisitVariableAccessSyntax(VariableAccessSyntax syntax);

        void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax);

        void VisitLocalVariableSyntax(LocalVariableSyntax syntax);

        void VisitIfConditionSyntax(IfConditionSyntax syntax);

        void VisitForSyntax(ForSyntax syntax);

        void VisitVariableBlockSyntax(VariableBlockSyntax syntax);

        void VisitDecoratorSyntax(DecoratorSyntax syntax);

        void VisitMissingDeclarationSyntax(MissingDeclarationSyntax syntax);

        void VisitImportDeclarationSyntax(ImportDeclarationSyntax syntax);

        void VisitImportWithClauseSyntax(ImportWithClauseSyntax syntax);

        void VisitImportAsClauseSyntax(ImportAsClauseSyntax syntax);

        void VisitUsingDeclarationSyntax(UsingDeclarationSyntax syntax);

        void VisitLambdaSyntax(LambdaSyntax syntax);
    }
}
