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

        void VisitTestDeclarationSyntax(TestDeclarationSyntax syntax);

        void VisitNullableTypeSyntax(NullableTypeSyntax syntax);

        void VisitNullLiteralSyntax(NullLiteralSyntax syntax);

        void VisitNoneLiteralSyntax(NoneLiteralSyntax syntax);

        void VisitIntegerLiteralSyntax(IntegerLiteralSyntax syntax);

        void VisitObjectPropertySyntax(ObjectPropertySyntax syntax);

        void VisitObjectSyntax(ObjectSyntax syntax);

        void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax);

        void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax);

        void VisitParameterAssignmentSyntax(ParameterAssignmentSyntax syntax);

        void VisitAssignmentClauseSyntax(AssignmentClauseSyntax syntax);

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

        void VisitObjectTypeAdditionalPropertiesSyntax(ObjectTypeAdditionalPropertiesSyntax syntax);

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

        void VisitAssertDeclarationSyntax(AssertDeclarationSyntax syntax);

        void VisitIfConditionSyntax(IfConditionSyntax syntax);

        void VisitForSyntax(ForSyntax syntax);

        void VisitVariableBlockSyntax(VariableBlockSyntax syntax);

        void VisitDecoratorSyntax(DecoratorSyntax syntax);

        void VisitMissingDeclarationSyntax(MissingDeclarationSyntax syntax);

        void VisitExtensionDeclarationSyntax(ExtensionDeclarationSyntax syntax);

        void VisitExtensionConfigAssignmentSyntax(ExtensionConfigAssignmentSyntax syntax);

        void VisitExtensionWithClauseSyntax(ExtensionWithClauseSyntax syntax);

        void VisitAliasAsClauseSyntax(AliasAsClauseSyntax syntax);

        void VisitUsingDeclarationSyntax(UsingDeclarationSyntax syntax);

        void VisitExtendsDeclarationSyntax(ExtendsDeclarationSyntax syntax);

        void VisitLambdaSyntax(LambdaSyntax syntax);

        void VisitNonNullAssertionSyntax(NonNullAssertionSyntax syntax);

        void VisitTypedVariableBlockSyntax(TypedVariableBlockSyntax syntax);

        void VisitTypedLocalVariableSyntax(TypedLocalVariableSyntax syntax);

        void VisitTypedLambdaSyntax(TypedLambdaSyntax syntax);

        void VisitFunctionDeclarationSyntax(FunctionDeclarationSyntax syntax);

        void VisitCompileTimeImportDeclarationSyntax(CompileTimeImportDeclarationSyntax syntax);

        void VisitImportedSymbolsListSyntax(ImportedSymbolsListSyntax syntax);

        void VisitImportedSymbolsListItemSyntax(ImportedSymbolsListItemSyntax syntax);

        void VisitWildcardImportSyntax(WildcardImportSyntax syntax);

        void VisitCompileTimeImportFromClauseSyntax(CompileTimeImportFromClauseSyntax syntax);

        void VisitParameterizedTypeInstantiationSyntax(ParameterizedTypeInstantiationSyntax syntax);

        void VisitInstanceParameterizedTypeInstantiationSyntax(InstanceParameterizedTypeInstantiationSyntax syntax);

        void VisitParameterizedTypeArgumentSyntax(ParameterizedTypeArgumentSyntax syntax);

        void VisitTypePropertyAccessSyntax(TypePropertyAccessSyntax syntax);

        void VisitTypeAdditionalPropertiesAccessSyntax(TypeAdditionalPropertiesAccessSyntax syntax);

        void VisitTypeArrayAccessSyntax(TypeArrayAccessSyntax syntax);

        void VisitTypeItemsAccessSyntax(TypeItemsAccessSyntax syntax);

        void VisitTypeVariableAccessSyntax(TypeVariableAccessSyntax syntax);

        void VisitStringTypeLiteralSyntax(StringTypeLiteralSyntax syntax);

        void VisitIntegerTypeLiteralSyntax(IntegerTypeLiteralSyntax syntax);

        void VisitBooleanTypeLiteralSyntax(BooleanTypeLiteralSyntax syntax);

        void VisitNullTypeLiteralSyntax(NullTypeLiteralSyntax syntax);

        void VisitUnaryTypeOperationSyntax(UnaryTypeOperationSyntax syntax);

        void VisitNonNullableTypeSyntax(NonNullableTypeSyntax syntax);

        void VisitParenthesizedTypeSyntax(ParenthesizedTypeSyntax syntax);

        void VisitSpreadExpressionSyntax(SpreadExpressionSyntax syntax);
    }
}
