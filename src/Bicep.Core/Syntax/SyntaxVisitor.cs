// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.CompilerServices;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public abstract class SyntaxVisitor : ISyntaxVisitor
    {
        public abstract void VisitSyntaxTrivia(SyntaxTrivia syntaxTrivia);

        public abstract void VisitArrayAccessSyntax(ArrayAccessSyntax syntax);

        public abstract void VisitArrayItemSyntax(ArrayItemSyntax syntax);

        public abstract void VisitArraySyntax(ArraySyntax syntax);

        public abstract void VisitArrayTypeMemberSyntax(ArrayTypeMemberSyntax syntax);

        public abstract void VisitArrayTypeSyntax(ArrayTypeSyntax syntax);

        public abstract void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax);

        public abstract void VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax);

        public abstract void VisitDecoratorSyntax(DecoratorSyntax syntax);

        public abstract void VisitForSyntax(ForSyntax syntax);

        public abstract void VisitFunctionArgumentSyntax(FunctionArgumentSyntax syntax);

        public abstract void VisitFunctionCallSyntax(FunctionCallSyntax syntax);

        public abstract void VisitIdentifierSyntax(IdentifierSyntax syntax);

        public abstract void VisitIfConditionSyntax(IfConditionSyntax syntax);

        public abstract void VisitAliasAsClauseSyntax(AliasAsClauseSyntax syntax);

        public abstract void VisitExtensionDeclarationSyntax(ExtensionDeclarationSyntax syntax);

        public abstract void VisitExtensionConfigAssignmentSyntax(ExtensionConfigAssignmentSyntax syntax);

        public abstract void VisitExtensionWithClauseSyntax(ExtensionWithClauseSyntax syntax);

        public abstract void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax);

        public abstract void VisitIntegerLiteralSyntax(IntegerLiteralSyntax syntax);

        public abstract void VisitLambdaSyntax(LambdaSyntax syntax);

        public abstract void VisitLocalVariableSyntax(LocalVariableSyntax syntax);

        public abstract void VisitMetadataDeclarationSyntax(MetadataDeclarationSyntax syntax);

        public abstract void VisitMissingDeclarationSyntax(MissingDeclarationSyntax syntax);

        public abstract void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax);

        public abstract void VisitStackDeclarationSyntax(StackDeclarationSyntax syntax);

        public abstract void VisitRuleDeclarationSyntax(RuleDeclarationSyntax syntax);

        public abstract void VisitTestDeclarationSyntax(TestDeclarationSyntax syntax);

        public abstract void VisitNonNullAssertionSyntax(NonNullAssertionSyntax syntax);

        public abstract void VisitNullableTypeSyntax(NullableTypeSyntax syntax);

        public abstract void VisitNullLiteralSyntax(NullLiteralSyntax syntax);

        public abstract void VisitNoneLiteralSyntax(NoneLiteralSyntax syntax);

        public abstract void VisitObjectPropertySyntax(ObjectPropertySyntax syntax);

        public abstract void VisitObjectSyntax(ObjectSyntax syntax);

        public abstract void VisitObjectTypePropertySyntax(ObjectTypePropertySyntax syntax);

        public abstract void VisitObjectTypeAdditionalPropertiesSyntax(ObjectTypeAdditionalPropertiesSyntax syntax);

        public abstract void VisitObjectTypeSyntax(ObjectTypeSyntax syntax);

        public abstract void VisitTupleTypeSyntax(TupleTypeSyntax syntax);

        public abstract void VisitTupleTypeItemSyntax(TupleTypeItemSyntax syntax);

        public abstract void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax);

        public abstract void VisitParameterAssignmentSyntax(ParameterAssignmentSyntax syntax);

        public abstract void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax);

        public abstract void VisitParameterDefaultValueSyntax(ParameterDefaultValueSyntax syntax);

        public abstract void VisitParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax);

        public abstract void VisitProgramSyntax(ProgramSyntax syntax);

        public abstract void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax);

        public abstract void VisitResourceAccessSyntax(ResourceAccessSyntax syntax);

        public abstract void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax);

        public abstract void VisitResourceTypeSyntax(ResourceTypeSyntax syntax);

        public abstract void VisitAssertDeclarationSyntax(AssertDeclarationSyntax syntax);

        public abstract void VisitSeparatedSyntaxList(SeparatedSyntaxList syntax);

        public abstract void VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax);

        public abstract void VisitStringSyntax(StringSyntax syntax);

        public abstract void VisitTargetScopeSyntax(TargetScopeSyntax syntax);

        public abstract void VisitTernaryOperationSyntax(TernaryOperationSyntax syntax);

        public abstract void VisitToken(Token token);

        public abstract void VisitTypeDeclarationSyntax(TypeDeclarationSyntax syntax);

        public abstract void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax);

        public abstract void VisitUnionTypeMemberSyntax(UnionTypeMemberSyntax syntax);

        public abstract void VisitUnionTypeSyntax(UnionTypeSyntax syntax);

        public abstract void VisitUsingDeclarationSyntax(UsingDeclarationSyntax syntax);

        public abstract void VisitExtendsDeclarationSyntax(ExtendsDeclarationSyntax syntax);

        public abstract void VisitVariableAccessSyntax(VariableAccessSyntax syntax);

        public abstract void VisitVariableBlockSyntax(VariableBlockSyntax syntax);

        public abstract void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax);

        public abstract void VisitTypedVariableBlockSyntax(TypedVariableBlockSyntax syntax);

        public abstract void VisitTypedLocalVariableSyntax(TypedLocalVariableSyntax syntax);

        public abstract void VisitTypedLambdaSyntax(TypedLambdaSyntax syntax);

        public abstract void VisitFunctionDeclarationSyntax(FunctionDeclarationSyntax syntax);

        public abstract void VisitCompileTimeImportDeclarationSyntax(CompileTimeImportDeclarationSyntax syntax);

        public abstract void VisitImportedSymbolsListSyntax(ImportedSymbolsListSyntax syntax);

        public abstract void VisitImportedSymbolsListItemSyntax(ImportedSymbolsListItemSyntax syntax);

        public abstract void VisitWildcardImportSyntax(WildcardImportSyntax syntax);

        public abstract void VisitCompileTimeImportFromClauseSyntax(CompileTimeImportFromClauseSyntax syntax);

        public abstract void VisitParameterizedTypeInstantiationSyntax(ParameterizedTypeInstantiationSyntax syntax);

        public abstract void VisitInstanceParameterizedTypeInstantiationSyntax(InstanceParameterizedTypeInstantiationSyntax syntax);

        public abstract void VisitParameterizedTypeArgumentSyntax(ParameterizedTypeArgumentSyntax syntax);

        public abstract void VisitTypePropertyAccessSyntax(TypePropertyAccessSyntax syntax);

        public abstract void VisitTypeAdditionalPropertiesAccessSyntax(TypeAdditionalPropertiesAccessSyntax syntax);

        public abstract void VisitTypeArrayAccessSyntax(TypeArrayAccessSyntax syntax);

        public abstract void VisitTypeItemsAccessSyntax(TypeItemsAccessSyntax syntax);

        public abstract void VisitTypeVariableAccessSyntax(TypeVariableAccessSyntax syntax);

        public abstract void VisitStringTypeLiteralSyntax(StringTypeLiteralSyntax syntax);

        public abstract void VisitIntegerTypeLiteralSyntax(IntegerTypeLiteralSyntax syntax);

        public abstract void VisitBooleanTypeLiteralSyntax(BooleanTypeLiteralSyntax syntax);

        public abstract void VisitNullTypeLiteralSyntax(NullTypeLiteralSyntax syntax);

        public abstract void VisitUnaryTypeOperationSyntax(UnaryTypeOperationSyntax syntax);

        public abstract void VisitNonNullableTypeSyntax(NonNullableTypeSyntax syntax);

        public abstract void VisitParenthesizedTypeSyntax(ParenthesizedTypeSyntax syntax);

        public abstract void VisitSpreadExpressionSyntax(SpreadExpressionSyntax syntax);

        public void Visit(SyntaxBase? node)
        {
            if (node == null)
            {
                return;
            }

            RuntimeHelpers.EnsureSufficientExecutionStack();

            VisitInternal(node);
        }

        protected virtual void VisitInternal(SyntaxBase node)
        {
            node.Accept(this);
        }

        protected void VisitNodes(IEnumerable<SyntaxBase> nodes)
        {
            foreach (SyntaxBase node in nodes)
            {
                this.Visit(node);
            }
        }
    }
}
