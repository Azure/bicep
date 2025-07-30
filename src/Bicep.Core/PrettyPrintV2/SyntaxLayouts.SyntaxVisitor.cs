// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2.Documents;
using Bicep.Core.Syntax;

namespace Bicep.Core.PrettyPrintV2
{
    public partial class SyntaxLayouts : ISyntaxVisitor
    {
        private delegate IEnumerable<Document> SyntaxLayoutSpecifier<TSyntax>(TSyntax syntax)
            where TSyntax : SyntaxBase;

        private readonly PrettyPrinterV2Context context;

        private IEnumerable<Document> current = [];

        public SyntaxLayouts(PrettyPrinterV2Context context)
        {
            this.context = context;
        }

        public void VisitArrayAccessSyntax(ArrayAccessSyntax syntax) => this.Apply(syntax, this.LayoutArrayAccessSyntax);

        public void VisitArrayItemSyntax(ArrayItemSyntax syntax) => this.Layout(syntax.Value);

        public void VisitArraySyntax(ArraySyntax syntax) => this.Apply(syntax, this.LayoutArraySyntax);

        public void VisitArrayTypeMemberSyntax(ArrayTypeMemberSyntax syntax) => this.Layout(syntax.Value);

        public void VisitArrayTypeSyntax(ArrayTypeSyntax syntax) => this.Apply(syntax, this.LayoutArrayTypeSyntax);

        public void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax) => this.Apply(syntax, this.LayoutBinaryOperationSyntax);

        public void VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax) => this.Layout(syntax.Literal);

        public void VisitDecoratorSyntax(DecoratorSyntax syntax) => this.Apply(syntax, this.LayoutDecoratorSyntax);

        public void VisitForSyntax(ForSyntax syntax) => this.Apply(syntax, this.LayoutForSyntax);

        public void VisitFunctionArgumentSyntax(FunctionArgumentSyntax syntax) => this.Layout(syntax.Expression);

        public void VisitFunctionCallSyntax(FunctionCallSyntax syntax) => this.Apply(syntax, this.LayoutFunctionCallSyntax);

        public void VisitIdentifierSyntax(IdentifierSyntax syntax) => this.Layout(syntax.Child);

        public void VisitIfConditionSyntax(IfConditionSyntax syntax) => this.Apply(syntax, this.LayoutIfConditionSyntax);

        public void VisitAliasAsClauseSyntax(AliasAsClauseSyntax syntax) => this.Apply(syntax, this.LayoutAliasAsClauseSyntax);

        public void VisitExtensionDeclarationSyntax(ExtensionDeclarationSyntax syntax) => this.Apply(syntax, this.LayoutExtensionDeclarationSyntax);

        public void VisitExtensionConfigAssignmentSyntax(ExtensionConfigAssignmentSyntax syntax) => this.Apply(syntax, this.LayoutExtensionConfigAssignmentSyntax);

        public void VisitExtensionWithClauseSyntax(ExtensionWithClauseSyntax syntax) => this.Apply(syntax, this.LayoutExtensionWithClauseSyntax);

        public void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax) => this.Apply(syntax, this.LayoutInstanceFunctionCallSyntax);

        public void VisitIntegerLiteralSyntax(IntegerLiteralSyntax syntax) => this.Layout(syntax.Literal);

        public void VisitLambdaSyntax(LambdaSyntax syntax) => this.Apply(syntax, this.LayoutLambdaSyntax);

        public void VisitLocalVariableSyntax(LocalVariableSyntax syntax) => this.Layout(syntax.Name);

        public void VisitMetadataDeclarationSyntax(MetadataDeclarationSyntax syntax) => this.Apply(syntax, this.LayoutMetadataDeclarationSyntax);

        public void VisitMissingDeclarationSyntax(MissingDeclarationSyntax syntax) => this.Apply(syntax, this.LayoutMissingDeclarationSyntax);

        public void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax) => this.Apply(syntax, this.LayoutModuleDeclarationSyntax);

        public void VisitTestDeclarationSyntax(TestDeclarationSyntax syntax) => this.Apply(syntax, this.LayoutTestDeclarationSyntax);

        public void VisitNonNullAssertionSyntax(NonNullAssertionSyntax syntax) => this.Apply(syntax, this.LayoutNonNullAssertionSyntax);

        public void VisitNullableTypeSyntax(NullableTypeSyntax syntax) => this.Apply(syntax, this.LayoutNullableTypeSyntax);

        public void VisitNullLiteralSyntax(NullLiteralSyntax syntax) => this.Layout(syntax.NullKeyword);

        public void VisitNoneLiteralSyntax(NoneLiteralSyntax syntax) => this.Layout(syntax.NoneKeyword);

        public void VisitObjectPropertySyntax(ObjectPropertySyntax syntax) => this.Apply(syntax, this.LayoutObjectPropertySyntax);

        public void VisitObjectSyntax(ObjectSyntax syntax) => this.Apply(syntax, this.LayoutObjectSyntax);

        public void VisitObjectTypeAdditionalPropertiesSyntax(ObjectTypeAdditionalPropertiesSyntax syntax) => this.Apply(syntax, this.LayoutObjectTypeAdditionalPropertiesSyntax);

        public void VisitObjectTypePropertySyntax(ObjectTypePropertySyntax syntax) => this.Apply(syntax, this.LayoutObjectTypePropertySyntax);

        public void VisitObjectTypeSyntax(ObjectTypeSyntax syntax) => this.Apply(syntax, this.LayoutObjectTypeSyntax);

        public void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax) => this.Apply(syntax, this.LayoutOutputDeclarationSyntax);

        public void VisitParameterAssignmentSyntax(ParameterAssignmentSyntax syntax) => this.Apply(syntax, this.LayoutParameterAssignmentSyntax);

        public void VisitAssignmentClauseSyntax(AssignmentClauseSyntax syntax) => this.Apply(syntax, this.LayoutAssignmentClauseSyntax);

        public void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax) => this.Apply(syntax, this.LayoutParameterDeclarationSyntax);

        public void VisitParameterDefaultValueSyntax(ParameterDefaultValueSyntax syntax) => this.Apply(syntax, this.LayoutParameterDefaultValueSyntax);

        public void VisitParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax) => this.Apply(syntax, this.LayoutParenthesizedExpressionSyntax);

        public void VisitProgramSyntax(ProgramSyntax syntax) => this.Apply(syntax, this.LayoutProgramSyntax);

        public void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax) => this.Apply(syntax, this.LayoutPropertyAccessSyntax);

        public void VisitResourceAccessSyntax(ResourceAccessSyntax syntax) => this.Apply(syntax, this.LayoutResourceAccessSyntax);

        public void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax) => this.Apply(syntax, this.LayoutResourceDeclarationSyntax);

        public void VisitResourceTypeSyntax(ResourceTypeSyntax syntax) => this.Apply(syntax, this.LayoutResourceTypeSyntax);

        public void VisitSeparatedSyntaxList(SeparatedSyntaxList syntax) => throw new NotImplementedException();

        public void VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax) => this.Apply(syntax, this.LayoutSkippedTriviaSyntax);

        public void VisitStringSyntax(StringSyntax syntax) => this.Apply(syntax, this.LayoutStringSyntax);

        public void VisitTargetScopeSyntax(TargetScopeSyntax syntax) => this.Apply(syntax, this.LayoutTargetScopeSyntax);

        public void VisitTernaryOperationSyntax(TernaryOperationSyntax syntax) => this.Apply(syntax, this.LayoutTernaryOperationSyntax);

        public void VisitToken(Token token) => this.Apply(token, this.LayoutToken);

        public void VisitTupleTypeItemSyntax(TupleTypeItemSyntax syntax) => this.Apply(syntax, this.LayoutTupleTypeItemSyntax);

        public void VisitTupleTypeSyntax(TupleTypeSyntax syntax) => this.Apply(syntax, this.LayoutTupleTypeSyntax);

        public void VisitTypeDeclarationSyntax(TypeDeclarationSyntax syntax) => this.Apply(syntax, this.LayoutTypeDeclarationSyntax);

        public void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax) => this.Apply(syntax, this.LayoutUnaryOperationSyntax);

        public void VisitUnionTypeMemberSyntax(UnionTypeMemberSyntax syntax) => this.Layout(syntax.Value);

        public void VisitUnionTypeSyntax(UnionTypeSyntax syntax) => this.Apply(syntax, this.LayoutUnionTypeSyntax);

        public void VisitUsingDeclarationSyntax(UsingDeclarationSyntax syntax) => this.Apply(syntax, this.LayoutUsingDeclarationSyntax);

        public void VisitExtendsDeclarationSyntax(ExtendsDeclarationSyntax syntax) => this.Apply(syntax, this.LayoutExtendsDeclarationSyntax);

        public void VisitVariableAccessSyntax(VariableAccessSyntax syntax) => this.Layout(syntax.Name);

        public void VisitVariableBlockSyntax(VariableBlockSyntax syntax) => this.Apply(syntax, this.LayoutVariableBlockSyntax);

        public void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax) => this.Apply(syntax, this.LayoutVariableDeclarationSyntax);

        public void VisitTypedVariableBlockSyntax(TypedVariableBlockSyntax syntax) => this.Apply(syntax, this.LayoutTypedVariableBlockSyntax);

        public void VisitTypedLocalVariableSyntax(TypedLocalVariableSyntax syntax) => this.Apply(syntax, this.LayoutTypedLocalVariableSyntax);

        public void VisitTypedLambdaSyntax(TypedLambdaSyntax syntax) => this.Apply(syntax, this.LayoutTypedLambdaSyntax);

        public void VisitFunctionDeclarationSyntax(FunctionDeclarationSyntax syntax) => this.Apply(syntax, this.LayoutFunctionDeclarationSyntax);

        public void VisitAssertDeclarationSyntax(AssertDeclarationSyntax syntax) => this.Apply(syntax, this.LayoutAssertDeclarationSyntax);

        public void VisitCompileTimeImportDeclarationSyntax(CompileTimeImportDeclarationSyntax syntax) => this.Apply(syntax, this.LayoutCompileTimeImportDeclarationSyntax);

        public void VisitImportedSymbolsListSyntax(ImportedSymbolsListSyntax syntax) => this.Apply(syntax, LayoutImportedSymbolsListSyntax);

        public void VisitImportedSymbolsListItemSyntax(ImportedSymbolsListItemSyntax syntax) => this.Apply(syntax, LayoutImportedSymbolsListItemSyntax);

        public void VisitWildcardImportSyntax(WildcardImportSyntax syntax) => this.Apply(syntax, LayoutWildcardImportSyntax);

        public void VisitCompileTimeImportFromClauseSyntax(CompileTimeImportFromClauseSyntax syntax) => this.Apply(syntax, LayoutCompileTimeImportFromClauseSyntax);

        public void VisitParameterizedTypeInstantiationSyntax(ParameterizedTypeInstantiationSyntax syntax) => this.Apply(syntax, LayoutParameterizedTypeInstantiationSyntax);

        public void VisitInstanceParameterizedTypeInstantiationSyntax(InstanceParameterizedTypeInstantiationSyntax syntax)
            => this.Apply(syntax, LayoutInstanceParameterizedTypeInstantiationSyntax);

        public void VisitParameterizedTypeArgumentSyntax(ParameterizedTypeArgumentSyntax syntax) => this.Layout(syntax.Expression);

        public void VisitTypePropertyAccessSyntax(TypePropertyAccessSyntax syntax) => this.Apply(syntax, LayoutTypePropertyAccessSyntax);

        public void VisitTypeAdditionalPropertiesAccessSyntax(TypeAdditionalPropertiesAccessSyntax syntax)
            => this.Apply(syntax, LayoutTypeAdditionalPropertiesAccessSyntax);

        public void VisitTypeArrayAccessSyntax(TypeArrayAccessSyntax syntax) => this.Apply(syntax, LayoutTypeArrayAccessSyntax);

        public void VisitTypeItemsAccessSyntax(TypeItemsAccessSyntax syntax) => this.Apply(syntax, LayoutTypeItemsAccessSyntax);

        public void VisitTypeVariableAccessSyntax(TypeVariableAccessSyntax syntax) => this.Layout(syntax.Name);

        public void VisitStringTypeLiteralSyntax(StringTypeLiteralSyntax syntax) => this.Apply(syntax, LayoutStringTypeLiteralSyntax);

        public void VisitIntegerTypeLiteralSyntax(IntegerTypeLiteralSyntax syntax) => this.Layout(syntax.Literal);

        public void VisitBooleanTypeLiteralSyntax(BooleanTypeLiteralSyntax syntax) => this.Layout(syntax.Literal);

        public void VisitNullTypeLiteralSyntax(NullTypeLiteralSyntax syntax) => this.Layout(syntax.NullKeyword);

        public void VisitUnaryTypeOperationSyntax(UnaryTypeOperationSyntax syntax) => this.Apply(syntax, LayoutUnaryTypeOperationSyntax);

        public void VisitNonNullableTypeSyntax(NonNullableTypeSyntax syntax) => this.Apply(syntax, LayoutNonNullableTypeSyntax);

        public void VisitParenthesizedTypeSyntax(ParenthesizedTypeSyntax syntax) => this.Apply(syntax, LayoutParenthesizedTypeSyntax);

        public void VisitSpreadExpressionSyntax(SpreadExpressionSyntax syntax) => this.Apply(syntax, LayoutSpreadExpressionSyntax);

        public IEnumerable<Document> Layout(SyntaxBase syntax)
        {
            syntax.Accept(this);

            return this.current;
        }

        private Document LayoutSingle(SyntaxBase syntax) => this.Layout(syntax).Single();

        private IEnumerable<Document> LayoutMany(IEnumerable<SyntaxBase> syntaxes) => syntaxes.SelectMany(this.Layout);

        private void Apply<TSyntax>(TSyntax syntax, SyntaxLayoutSpecifier<TSyntax> layoutSpecifier)
            where TSyntax : SyntaxBase
        {
            this.current = syntax is ITopLevelDeclarationSyntax && this.context.HasSyntaxError(syntax)
                ? TextDocument.From(SyntaxStringifier.Stringify(syntax, this.context.Newline).Trim())
                : layoutSpecifier(syntax);
        }
    }
}
