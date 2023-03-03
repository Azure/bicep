// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2.Documents;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.PrettyPrintV2
{
    public delegate IEnumerable<Document> DocumentLayoutSpecifier<TSyntax>(TSyntax syntax)
        where TSyntax : SyntaxBase;

    public partial class SyntaxLayouts : ISyntaxVisitor
    {
        private readonly LineBreakerTracker lineBreakerTracker;

        private IEnumerable<Document> current = Enumerable.Empty<Document>();

        public SyntaxLayouts(HashSet<GroupDocument> lineBreakingGroups)
        {
            this.lineBreakerTracker = new(lineBreakingGroups);
        }

        public void VisitArrayAccessSyntax(ArrayAccessSyntax syntax) => this.Layout(syntax, this.LayoutArrayAccessSyntax);

        public void VisitArrayItemSyntax(ArrayItemSyntax syntax) => this.Layout(syntax.Value);

        public void VisitArraySyntax(ArraySyntax syntax) => this.Layout(syntax, this.LayoutArraySyntax);

        public void VisitArrayTypeMemberSyntax(ArrayTypeMemberSyntax syntax) => this.Layout(syntax.Value);

        public void VisitArrayTypeSyntax(ArrayTypeSyntax syntax) => this.Layout(syntax, this.LayoutArrayTypeSyntax);

        public void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax) => this.Layout(syntax, this.LayoutBinaryOperationSyntax);

        public void VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax) => this.Layout(syntax.Literal);

        public void VisitDecoratorSyntax(DecoratorSyntax syntax) => this.Layout(syntax, this.LayoutDecoratorSyntax);

        public void VisitForSyntax(ForSyntax syntax) => this.Layout(syntax, this.LayoutForSyntax);

        public void VisitFunctionArgumentSyntax(FunctionArgumentSyntax syntax) => this.Layout(syntax.Expression);

        public void VisitFunctionCallSyntax(FunctionCallSyntax syntax) => this.Layout(syntax, this.LayoutFunctionCallSyntax);

        public void VisitIdentifierSyntax(IdentifierSyntax syntax) => this.Layout(syntax.Child);

        public void VisitIfConditionSyntax(IfConditionSyntax syntax) => this.Layout(syntax, this.LayoutIfConditionSyntax);

        public void VisitImportAsClauseSyntax(ImportAsClauseSyntax syntax) => this.Layout(syntax, this.LayoutImportAsClauseSyntax);

        public void VisitImportDeclarationSyntax(ImportDeclarationSyntax syntax) => this.Layout(syntax, this.LayoutImportDeclarationSyntax);

        public void VisitImportWithClauseSyntax(ImportWithClauseSyntax syntax) => this.Layout(syntax, this.LayoutImportWithClauseSyntax);

        public void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax) => this.Layout(syntax, this.LayoutIntanceFunctionCallSyntax);

        public void VisitIntegerLiteralSyntax(IntegerLiteralSyntax syntax) => this.Layout(syntax.Literal);

        public void VisitLambdaSyntax(LambdaSyntax syntax) => this.Layout(syntax, this.LayoutLambdaSyntax);

        public void VisitLocalVariableSyntax(LocalVariableSyntax syntax) => this.Layout(syntax.Name);

        public void VisitMetadataDeclarationSyntax(MetadataDeclarationSyntax syntax) => this.Layout(syntax, this.LayoutMetadataDeclarationSyntax);

        public void VisitMissingDeclarationSyntax(MissingDeclarationSyntax syntax) => this.Layout(syntax, this.LayoutMissingDeclarationSyntax);

        public void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax) => this.Layout(syntax, this.LayoutModuleDeclarationSyntax);

        public void VisitNonNullAssertionSyntax(NonNullAssertionSyntax syntax) => this.Layout(syntax, this.LayoutNonNullAssertionSyntax);

        public void VisitNullableTypeSyntax(NullableTypeSyntax syntax) => this.Layout(syntax, this.LayoutNullableTypeSyntax);

        public void VisitNullLiteralSyntax(NullLiteralSyntax syntax) => this.Layout(syntax.NullKeyword);

        public void VisitObjectPropertySyntax(ObjectPropertySyntax syntax) => this.Layout(syntax, this.LayoutObjectPropertySyntax);

        public void VisitObjectSyntax(ObjectSyntax syntax) => this.Layout(syntax, this.LayoutObjectSyntax);

        public void VisitObjectTypeAdditionalPropertiesSyntax(ObjectTypeAdditionalPropertiesSyntax syntax) => this.Layout(syntax, this.LayoutObjectTypeAdditionalPropertiesSyntax);

        public void VisitObjectTypePropertySyntax(ObjectTypePropertySyntax syntax) => this.Layout(syntax, this.LayoutObjectTypePropertySyntax);

        public void VisitObjectTypeSyntax(ObjectTypeSyntax syntax) => this.Layout(syntax, this.LayoutObjectTypeSyntax);

        public void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax) => this.Layout(syntax, this.LayoutOutputDeclarationSyntax);

        public void VisitParameterAssignmentSyntax(ParameterAssignmentSyntax syntax) => this.Layout(syntax, this.LayoutParameterAssignmentSyntax);

        public void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax) => this.Layout(syntax, this.LayoutParameterDeclarationSyntax);

        public void VisitParameterDefaultValueSyntax(ParameterDefaultValueSyntax syntax) => this.Layout(syntax, this.LayoutParameterDefaultValueSyntax);

        public void VisitParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax) => this.Layout(syntax, this.LayoutParenthesizedExpressionSyntax);

        public void VisitProgramSyntax(ProgramSyntax syntax) => this.Layout(syntax, this.LayoutProgramSyntax);

        public void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax) => this.Layout(syntax, this.LayoutPropertyAccessSyntax);

        public void VisitResourceAccessSyntax(ResourceAccessSyntax syntax) => this.Layout(syntax, this.LayoutResourceAccessSyntax);

        public void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax) => this.Layout(syntax, this.LayoutResourceDeclarationSyntax);

        public void VisitResourceTypeSyntax(ResourceTypeSyntax syntax) => this.Layout(syntax, this.LayoutResourceTypeSyntax);

        public void VisitSeparatedSyntaxList(SeparatedSyntaxList syntax) => throw new NotImplementedException();

        public void VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax) => this.Layout(syntax, this.LayoutSkippedTriviaSyntax);

        public void VisitStringSyntax(StringSyntax syntax) => this.Layout(syntax, this.LayoutStringSyntax);

        public void VisitTargetScopeSyntax(TargetScopeSyntax syntax) => this.Layout(syntax, this.LayoutTargetScopeSyntax);

        public void VisitTernaryOperationSyntax(TernaryOperationSyntax syntax) => this.Layout(syntax, this.LayoutTernaryOperationSyntax);

        public void VisitToken(Token token) => this.Layout(token, this.LayoutToken);

        public void VisitTupleTypeItemSyntax(TupleTypeItemSyntax syntax) => this.Layout(syntax, this.LayoutTupleTypeItemSyntax);

        public void VisitTupleTypeSyntax(TupleTypeSyntax syntax) => this.Layout(syntax, this.LayoutTupleTypeSyntax);

        public void VisitTypeDeclarationSyntax(TypeDeclarationSyntax syntax) => this.Layout(syntax, this.LayoutTypeDeclarationSyntax);

        public void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax) => this.Layout(syntax, this.LayoutUnaryOperationSyntax);

        public void VisitUnionTypeMemberSyntax(UnionTypeMemberSyntax syntax) => this.Layout(syntax.Value);

        public void VisitUnionTypeSyntax(UnionTypeSyntax syntax) => this.Layout(syntax, this.LayoutUnionTypeSyntax);

        public void VisitUsingDeclarationSyntax(UsingDeclarationSyntax syntax) => this.Layout(syntax, this.LayoutUsingDeclarationSyntax);

        public void VisitVariableAccessSyntax(VariableAccessSyntax syntax) => this.Layout(syntax.Name);

        public void VisitVariableBlockSyntax(VariableBlockSyntax syntax) => this.Layout(syntax, this.LayoutVariableBlockSyntax);

        public void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax) => this.Layout(syntax, this.LayoutVariableDeclarationSyntax);

        public IEnumerable<Document> Layout(SyntaxBase syntax)
        {
            syntax.Accept(this);

            return this.current;
        }

        private Document LayoutSingle(SyntaxBase syntax) => this.Layout(syntax).Single();

        private IEnumerable<Document> LayoutMany(IEnumerable<SyntaxBase> syntaxes) => syntaxes.SelectMany(this.Layout);

        private void Layout<TSyntax>(TSyntax syntax, DocumentLayoutSpecifier<TSyntax> layoutSpecifier)
            where TSyntax : SyntaxBase
        {
            this.current = syntax switch
            {
                ITopLevelDeclarationSyntax when syntax.HasParseErrors() => TextDocument.Create(syntax.ToTextPreserveFormatting()),
                _ => this.lineBreakerTracker.Track(layoutSpecifier).Invoke(syntax),
            };
        }

        private class LineBreakerTracker
        {
            private readonly HashSet<GroupDocument> lineBreakingGroups;

            private int lineBreakersCount = 0;

            public LineBreakerTracker(HashSet<GroupDocument> lineBreakingGroups)
            {
                this.lineBreakingGroups = lineBreakingGroups;
            }

            public ImmutableHashSet<GroupDocument> LineBreakingGroups => this.lineBreakingGroups.ToImmutableHashSet();

            public void AddOne()
            {
                this.lineBreakersCount++;
            }

            public void AddGroup(GroupDocument group)
            {
                this.lineBreakingGroups.Add(group);
                this.AddOne();
            }

            public DocumentLayoutSpecifier<TSyntax> Track<TSyntax>(DocumentLayoutSpecifier<TSyntax> layoutSpecifier)
                where TSyntax : SyntaxBase
            {
                return (syntax) =>
                {
                    var countBefore = this.lineBreakersCount;
                    var document = layoutSpecifier(syntax);
                    var countAfter = this.lineBreakersCount;

                    if (countBefore < countAfter && document is GroupDocument group)
                    {
                        this.AddGroup(group);
                    }

                    return document;
                };
            }
        }
    }
}
