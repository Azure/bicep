using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;

namespace Bicep.LanguageServer
{
    [Obsolete] // proposed LSP feature must be marked 'obsolete' to access
    class SemanticTokenVisitor : SyntaxVisitor
    {
        private readonly List<(IPositionable positionable, SemanticTokenType tokenType)> tokens;

        private SemanticTokenVisitor()
        {
            this.tokens = new List<(IPositionable, SemanticTokenType)>();
        }

        public static void BuildSemanticTokens(SemanticTokensBuilder builder, CompilationContext compilationContext)
        {
            var visitor = new SemanticTokenVisitor();

            visitor.Visit(compilationContext.Compilation.ProgramSyntax);

            // the builder is fussy about ordering. tokens are visited out of order, we need to call build after visiting everything
            foreach (var (positionable, tokenType) in visitor.tokens.OrderBy(t => t.positionable.Span.Position))
            {
                var tokenRange = positionable.ToRange(compilationContext.LineStarts);
                builder.Push(tokenRange.Start.Line, tokenRange.Start.Character, tokenRange.End.Character - tokenRange.Start.Character, tokenType as SemanticTokenType?);
            }
        }

        private void AddTokenType(IPositionable positionable, SemanticTokenType tokenType)
        {
            tokens.Add((positionable, tokenType));
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            base.VisitArrayAccessSyntax(syntax);
        }

        public override void VisitArrayItemSyntax(ArrayItemSyntax syntax)
        {
            base.VisitArrayItemSyntax(syntax);
        }

        public override void VisitArraySyntax(ArraySyntax syntax)
        {
            base.VisitArraySyntax(syntax);
        }

        public override void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax)
        {
            AddTokenType(syntax.OperatorToken, SemanticTokenType.Operator);
            base.VisitBinaryOperationSyntax(syntax);
        }

        public override void VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax)
        {
            AddTokenType(syntax.Literal, SemanticTokenType.Number);
            base.VisitBooleanLiteralSyntax(syntax);
        }

        public override void VisitFunctionArgumentSyntax(FunctionArgumentSyntax syntax)
        {
            base.VisitFunctionArgumentSyntax(syntax);
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            AddTokenType(syntax.FunctionName, SemanticTokenType.Function);
            base.VisitFunctionCallSyntax(syntax);
        }

        public override void VisitIdentifierSyntax(IdentifierSyntax syntax)
        {
            base.VisitIdentifierSyntax(syntax);
        }

        public override void VisitNoOpDeclarationSyntax(NoOpDeclarationSyntax syntax)
        {
            base.VisitNoOpDeclarationSyntax(syntax);
        }

        public override void VisitNullLiteralSyntax(NullLiteralSyntax syntax)
        {
            AddTokenType(syntax.NullKeyword, SemanticTokenType.Number);
            base.VisitNullLiteralSyntax(syntax);
        }

        public override void VisitNumericLiteralSyntax(NumericLiteralSyntax syntax)
        {
            AddTokenType(syntax.Literal, SemanticTokenType.Number);
            base.VisitNumericLiteralSyntax(syntax);
        }

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            AddTokenType(syntax.Identifier, SemanticTokenType.Member);
            base.VisitObjectPropertySyntax(syntax);
        }

        public override void VisitObjectSyntax(ObjectSyntax syntax)
        {
            base.VisitObjectSyntax(syntax);
        }

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            AddTokenType(syntax.OutputKeyword, SemanticTokenType.Keyword);
            AddTokenType(syntax.Name, SemanticTokenType.Variable);
            base.VisitOutputDeclarationSyntax(syntax);
        }

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            AddTokenType(syntax.ParameterKeyword, SemanticTokenType.Keyword);
            AddTokenType(syntax.Name, SemanticTokenType.Variable);
            AddTokenType(syntax.Type, SemanticTokenType.Type);
            base.VisitParameterDeclarationSyntax(syntax);
        }

        public override void VisitParameterDefaultValueSyntax(ParameterDefaultValueSyntax syntax)
        {
            AddTokenType(syntax.DefaultKeyword, SemanticTokenType.Keyword);
            base.VisitParameterDefaultValueSyntax(syntax);
        }

        public override void VisitParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax)
        {
            base.VisitParenthesizedExpressionSyntax(syntax);
        }

        public override void VisitProgramSyntax(ProgramSyntax syntax)
        {
            base.VisitProgramSyntax(syntax);
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            AddTokenType(syntax.PropertyName, SemanticTokenType.Member);
            base.VisitPropertyAccessSyntax(syntax);
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            AddTokenType(syntax.ResourceKeyword, SemanticTokenType.Keyword);
            AddTokenType(syntax.Name, SemanticTokenType.Variable);
            base.VisitResourceDeclarationSyntax(syntax);
        }

        public override void VisitSkippedTokensTriviaSyntax(SkippedTokensTriviaSyntax syntax)
        {
            base.VisitSkippedTokensTriviaSyntax(syntax);
        }

        public override void VisitStringSyntax(StringSyntax syntax)
        {
            AddTokenType(syntax.StringToken, SemanticTokenType.String);
            base.VisitStringSyntax(syntax);
        }

        public override void VisitTernaryOperationSyntax(TernaryOperationSyntax syntax)
        {
            AddTokenType(syntax.Colon, SemanticTokenType.Operator);
            AddTokenType(syntax.Question, SemanticTokenType.Operator);
            base.VisitTernaryOperationSyntax(syntax);
        }

        public override void VisitToken(Token token)
        {
            base.VisitToken(token);
        }

        public override void VisitTypeSyntax(TypeSyntax syntax)
        {
            AddTokenType(syntax.Identifier, SemanticTokenType.Type);
            base.VisitTypeSyntax(syntax);
        }

        public override void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax)
        {
            AddTokenType(syntax.OperatorToken, SemanticTokenType.Operator);
            base.VisitUnaryOperationSyntax(syntax);
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            AddTokenType(syntax.Name, SemanticTokenType.Variable);
            base.VisitVariableAccessSyntax(syntax);
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            AddTokenType(syntax.VariableKeyword, SemanticTokenType.Keyword);
            AddTokenType(syntax.Name, SemanticTokenType.Variable);
            base.VisitVariableDeclarationSyntax(syntax);
        }
    }
}