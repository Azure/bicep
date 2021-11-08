// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer
{
    public class SemanticTokenVisitor : SyntaxVisitor
    {
        private readonly List<(IPositionable positionable, SemanticTokenType tokenType)> tokens;

        private SemanticTokenVisitor()
        {
            this.tokens = new List<(IPositionable, SemanticTokenType)>();
        }

        public static void BuildSemanticTokens(SemanticTokensBuilder builder, BicepFile bicepFile)
        {
            var visitor = new SemanticTokenVisitor();

            visitor.Visit(bicepFile.ProgramSyntax);

            // the builder is fussy about ordering. tokens are visited out of order, we need to call build after visiting everything
            foreach (var (positionable, tokenType) in visitor.tokens.OrderBy(t => t.positionable.Span.Position))
            {
                var tokenRanges = positionable.ToRangeSpanningLines(bicepFile.LineStarts);
                foreach (var tokenRange in tokenRanges)
                {
                    builder.Push(tokenRange.Start.Line, tokenRange.Start.Character, tokenRange.End.Character - tokenRange.Start.Character, tokenType as SemanticTokenType?);
                }
            }
        }

        private void AddTokenType(IPositionable? positionable, SemanticTokenType tokenType)
        {
            if (positionable is not null)
            {
                tokens.Add((positionable, tokenType));
            }
        }

        /// <summary>
        /// Adds the specified positionable element if it represents the specified keyword.
        /// This function only needs to be used if the parser does not prevent SkippedTriviaSyntax in place of the keyword.
        /// </summary>
        /// <param name="positionable">the positionable element</param>
        /// <param name="keyword">the expected keyword text</param>
        private void AddContextualKeyword(IPositionable positionable, string keyword)
        {
            // contextual keywords should only be highlighted as keywords if they are valid
            if (positionable is Token {Type: TokenType.Identifier} token && string.Equals(token.Text, keyword, StringComparison.Ordinal))
            {
                AddTokenType(positionable, SemanticTokenType.Keyword);
            }
        }

        public override void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax)
        {
            AddTokenType(syntax.OperatorToken, SemanticTokenType.Operator);
            base.VisitBinaryOperationSyntax(syntax);
        }

        public override void VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax)
        {
            AddTokenType(syntax.Literal, SemanticTokenType.Keyword);
            base.VisitBooleanLiteralSyntax(syntax);
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            // We need to set token types for OpenParen and CloseParen in case the function call
            // is inside a string interpolation. Our current textmate grammar will tag them as
            // string if they are not overrode by the semantic tokens.
            AddTokenType(syntax.Name, SemanticTokenType.Function);
            AddTokenType(syntax.OpenParen, SemanticTokenType.Operator);
            AddTokenType(syntax.CloseParen, SemanticTokenType.Operator);
            base.VisitFunctionCallSyntax(syntax);
        }

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            AddTokenType(syntax.Dot, SemanticTokenType.Operator);
            AddTokenType(syntax.Name, SemanticTokenType.Function);
            AddTokenType(syntax.OpenParen, SemanticTokenType.Operator);
            AddTokenType(syntax.CloseParen, SemanticTokenType.Operator);
            base.VisitInstanceFunctionCallSyntax(syntax);
        }

        public override void VisitNullLiteralSyntax(NullLiteralSyntax syntax)
        {
            AddTokenType(syntax.NullKeyword, SemanticTokenType.Keyword);
            base.VisitNullLiteralSyntax(syntax);
        }

        public override void VisitIntegerLiteralSyntax(IntegerLiteralSyntax syntax)
        {
            AddTokenType(syntax.Literal, SemanticTokenType.Number);
            base.VisitIntegerLiteralSyntax(syntax);
        }

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            if (syntax.Key is StringSyntax @string)
            {
                Visit(@string);
            }
            else
            {
                AddTokenType(syntax.Key, SemanticTokenType.Method);
            }
            Visit(syntax.Colon);
            Visit(syntax.Value);
        }

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
            AddTokenType(syntax.Name, SemanticTokenType.Variable);
            base.VisitOutputDeclarationSyntax(syntax);
        }

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
            AddTokenType(syntax.Name, SemanticTokenType.Variable);
            base.VisitParameterDeclarationSyntax(syntax);
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            AddTokenType(syntax.Dot, SemanticTokenType.Operator);
            AddTokenType(syntax.PropertyName, SemanticTokenType.Property);
            base.VisitPropertyAccessSyntax(syntax);
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            AddTokenType(syntax.OpenSquare, SemanticTokenType.Operator);
            AddTokenType(syntax.CloseSquare, SemanticTokenType.Operator);
            base.VisitArrayAccessSyntax(syntax);
        }

        public override void VisitResourceAccessSyntax(ResourceAccessSyntax syntax)
        {
            AddTokenType(syntax.ResourceName, SemanticTokenType.Property);
            base.VisitResourceAccessSyntax(syntax);
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
            AddTokenType(syntax.Name, SemanticTokenType.Variable);
            AddTokenType(syntax.ExistingKeyword, SemanticTokenType.Keyword);
            base.VisitResourceDeclarationSyntax(syntax);
        }

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
            AddTokenType(syntax.Name, SemanticTokenType.Variable);
            base.VisitModuleDeclarationSyntax(syntax);
        }

        public override void VisitIfConditionSyntax(IfConditionSyntax syntax)
        {
            AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
            base.VisitIfConditionSyntax(syntax);
        }

        public override void VisitForSyntax(ForSyntax syntax)
        {
            AddTokenType(syntax.ForKeyword, SemanticTokenType.Keyword);
            AddContextualKeyword(syntax.InKeyword, LanguageConstants.InKeyword);
            base.VisitForSyntax(syntax);
        }

        public override void VisitLocalVariableSyntax(LocalVariableSyntax syntax)
        {
            AddTokenType(syntax.Name, SemanticTokenType.Variable);
            base.VisitLocalVariableSyntax(syntax);
        }

        private void AddStringToken(Token token)
        {
            var endInterp = token.Type switch {
                TokenType.StringLeftPiece => LanguageConstants.StringHoleOpen,
                TokenType.StringMiddlePiece => LanguageConstants.StringHoleOpen,
                _ => ""
            };

            var startInterp = token.Type switch {
                TokenType.StringMiddlePiece => LanguageConstants.StringHoleClose,
                TokenType.StringRightPiece => LanguageConstants.StringHoleClose,
                _ => ""
            };

            // need to be extremely cautious here. it may be that lexing has failed to find a string terminating character,
            // but we still have a syntax tree to display tokens for.
            var hasEndOperator = endInterp.Length > 0 && token.Text.EndsWith(endInterp, StringComparison.Ordinal);
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
                case TokenType.MultilineString:
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
                case SyntaxTriviaType.DisableNextLineStatement:
                    AddTokenType(syntaxTrivia, SemanticTokenType.Macro);
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
            AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
            AddTokenType(syntax.Name, SemanticTokenType.Variable);
            base.VisitVariableDeclarationSyntax(syntax);
        }

        public override void VisitTargetScopeSyntax(TargetScopeSyntax syntax)
        {
            AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
            base.VisitTargetScopeSyntax(syntax);
        }

        public override void VisitImportDeclarationSyntax(ImportDeclarationSyntax syntax)
        {
            AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
            AddTokenType(syntax.AliasName, SemanticTokenType.Variable);
            AddContextualKeyword(syntax.FromKeyword, LanguageConstants.FromKeyword);
            AddTokenType(syntax.ProviderName, SemanticTokenType.Variable);
            base.VisitImportDeclarationSyntax(syntax);
        }
    }
}
