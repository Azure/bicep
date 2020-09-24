// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.Wasm.LanguageHelpers
{
    public class SemanticTokenVisitor : SyntaxVisitor
    {
        private readonly List<(IPositionable positionable, SemanticTokenType tokenType)> tokens;

        private SemanticTokenVisitor()
        {
            this.tokens = new List<(IPositionable, SemanticTokenType)>();
        }

        public static IEnumerable<SemanticToken> BuildSemanticTokens(ProgramSyntax programSyntax, ImmutableArray<int> lineStarts)
        {
            var visitor = new SemanticTokenVisitor();

            visitor.Visit(programSyntax);

            // the builder is fussy about ordering. tokens are visited out of order, we need to call build after visiting everything
            foreach (var (positionable, tokenType) in visitor.tokens.OrderBy(t => t.positionable.Span.Position))
            {
                var tokenRanges = positionable.ToRangeSpanningLines(lineStarts);
                foreach (var tokenRange in tokenRanges)
                {
                    yield return new SemanticToken(tokenRange.Start.Line, tokenRange.Start.Character, tokenRange.End.Character - tokenRange.Start.Character, tokenType);
                }
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
            AddTokenType(syntax.Name, SemanticTokenType.Function);
            base.VisitFunctionCallSyntax(syntax);
        }

        public override void VisitIdentifierSyntax(IdentifierSyntax syntax)
        {
            base.VisitIdentifierSyntax(syntax);
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
            if (syntax.Key is StringSyntax @string)
            {
                Visit(@string);
            }
            else
            {
                AddTokenType(syntax.Key, SemanticTokenType.Member);
            }
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
            base.VisitParameterDeclarationSyntax(syntax);
        }

        public override void VisitParameterDefaultValueSyntax(ParameterDefaultValueSyntax syntax)
        {
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
            AddTokenType(syntax.PropertyName, SemanticTokenType.Property);
            base.VisitPropertyAccessSyntax(syntax);
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            AddTokenType(syntax.ResourceKeyword, SemanticTokenType.Keyword);
            AddTokenType(syntax.Name, SemanticTokenType.Variable);
            base.VisitResourceDeclarationSyntax(syntax);
        }

        public override void VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax)
        {
            base.VisitSkippedTriviaSyntax(syntax);
        }

        private void AddStringToken(Token token)
        {
            var endInterp = token.Type switch {
                TokenType.StringLeftPiece => LanguageConstants.StringHoleOpen,
                TokenType.StringMiddlePiece => LanguageConstants.StringHoleOpen,
                _ => "",
            };

            var startInterp = token.Type switch {
                TokenType.StringMiddlePiece => LanguageConstants.StringHoleClose,
                TokenType.StringRightPiece => LanguageConstants.StringHoleClose,
                _ => "",
            };

            // need to be extremely cautious here. it may be that lexing has failed to find a string terminating character,
            // but we still have a syntax tree to display tokens for.
            var hasEndOperator = endInterp.Length > 0 && token.Text.EndsWith(endInterp);
            var endOperatorLength = hasEndOperator ? endInterp.Length : 0;

            var hasStartOperator = startInterp.Length > 0 && token.Text.Length >= startInterp.Length;
            var startOperatorLength = hasStartOperator ? startInterp.Length : 0;

            if (hasStartOperator)
            {
                AddTokenType(token.GetSpanSlice(0, startOperatorLength), SemanticTokenType.Operator);
            }

            AddTokenType(token.GetSpanSlice(startOperatorLength, token.Span.Length - startOperatorLength - endOperatorLength), SemanticTokenType.String);

            if (hasEndOperator)
            {
                AddTokenType(token.GetSpanSlice(token.Span.Length - endOperatorLength, endOperatorLength), SemanticTokenType.Operator);
            }
        }

        public override void VisitStringSyntax(StringSyntax syntax)
        {
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
            switch (token.Type)
            {
                case TokenType.StringComplete:
                case TokenType.StringLeftPiece:
                case TokenType.StringMiddlePiece:
                case TokenType.StringRightPiece:
                    AddStringToken(token);
                    break;
                default:
                    break;
            }

            base.VisitToken(token);
        }

        public override void VisitSyntaxTrivia(SyntaxTrivia syntaxTrivia)
        {
            switch (syntaxTrivia.Type)
            {
                case SyntaxTriviaType.SingleLineComment:
                case SyntaxTriviaType.MultiLineComment:
                    AddTokenType(syntaxTrivia, SemanticTokenType.Comment);
                    break;
            }
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
