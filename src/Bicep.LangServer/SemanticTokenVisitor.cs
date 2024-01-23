// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer
{
    public class SemanticTokenVisitor : CstVisitor
    {
        private readonly SemanticModel model;
        private readonly List<(IPositionable positionable, SemanticTokenType tokenType)> tokens = new();

        private SemanticTokenVisitor(SemanticModel model)
        {
            this.model = model;
        }

        public static void BuildSemanticTokens(SemanticTokensBuilder builder, SemanticModel model)
        {
            var visitor = new SemanticTokenVisitor(model);

            visitor.Visit(model.SourceFile.ProgramSyntax);

            // the builder is fussy about ordering. tokens are visited out of order, we need to call build after visiting everything
            foreach (var (positionable, tokenType) in visitor.tokens.OrderBy(t => t.positionable.Span.Position))
            {
                var tokenRanges = positionable.ToRangeSpanningLines(model.SourceFile.LineStarts);
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
            if (positionable is Token { Type: TokenType.Identifier } token && string.Equals(token.Text, keyword, StringComparison.Ordinal))
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
                AddTokenType(syntax.Key, SemanticTokenType.TypeParameter);
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

        public override void VisitTypeDeclarationSyntax(TypeDeclarationSyntax syntax)
        {
            AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
            AddTokenType(syntax.Name, SemanticTokenType.Type);
            base.VisitTypeDeclarationSyntax(syntax);
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

        public override void VisitTestDeclarationSyntax(TestDeclarationSyntax syntax)
        {
            AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
            AddTokenType(syntax.Name, SemanticTokenType.Variable);
            base.VisitTestDeclarationSyntax(syntax);
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
            var endInterp = token.Type switch
            {
                TokenType.StringLeftPiece => LanguageConstants.StringHoleOpen,
                TokenType.StringMiddlePiece => LanguageConstants.StringHoleOpen,
                _ => ""
            };

            var startInterp = token.Type switch
            {
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
                case TokenType.Comma:
                    AddTokenType(token, SemanticTokenType.Operator);
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
                case SyntaxTriviaType.DisableNextLineDiagnosticsDirective:
                    AddTokenType(syntaxTrivia, SemanticTokenType.Macro);
                    break;
            }
        }

        public override void VisitResourceTypeSyntax(ResourceTypeSyntax syntax)
        {
            // This is intentional, we want 'resource' to look like 'object' or 'array'.
            AddTokenType(syntax.Keyword, SemanticTokenType.Type);
            base.VisitResourceTypeSyntax(syntax);
        }

        public override void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax)
        {
            AddTokenType(syntax.OperatorToken, SemanticTokenType.Operator);
            base.VisitUnaryOperationSyntax(syntax);
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            AddTokenType(syntax.Name, model.GetSymbolInfo(syntax) switch
            {
                TypeAliasSymbol or
                AmbientTypeSymbol or
                ImportedTypeSymbol => SemanticTokenType.Type,
                _ => SemanticTokenType.Variable,
            });
            base.VisitVariableAccessSyntax(syntax);
        }

        public override void VisitMetadataDeclarationSyntax(MetadataDeclarationSyntax syntax)
        {
            AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
            AddTokenType(syntax.Name, SemanticTokenType.Variable);
            base.VisitMetadataDeclarationSyntax(syntax);
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

        public override void VisitProviderDeclarationSyntax(ProviderDeclarationSyntax syntax)
        {
            AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
            this.Visit(syntax.SpecificationString);
            this.Visit(syntax.WithClause);
            this.Visit(syntax.AsClause);
        }

        public override void VisitProviderWithClauseSyntax(ProviderWithClauseSyntax syntax)
        {
            AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
            this.Visit(syntax.Config);
        }

        public override void VisitAliasAsClauseSyntax(AliasAsClauseSyntax syntax)
        {
            AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
            AddTokenType(syntax.Alias, SemanticTokenType.Variable);
        }

        public override void VisitCompileTimeImportDeclarationSyntax(CompileTimeImportDeclarationSyntax syntax)
        {
            AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
            base.VisitCompileTimeImportDeclarationSyntax(syntax);
        }

        public override void VisitImportedSymbolsListItemSyntax(ImportedSymbolsListItemSyntax syntax)
        {
            if (syntax.OriginalSymbolName is IdentifierSyntax)
            {
                AddTokenType(syntax.OriginalSymbolName, model.GetSymbolInfo(syntax)?.Kind switch
                {
                    Core.Semantics.SymbolKind.TypeAlias => SemanticTokenType.Type,
                    _ => SemanticTokenType.Variable,
                });
            }

            base.VisitImportedSymbolsListItemSyntax(syntax);
        }

        public override void VisitWildcardImportSyntax(WildcardImportSyntax syntax)
        {
            AddTokenType(syntax.Wildcard, SemanticTokenType.Variable);
            base.VisitWildcardImportSyntax(syntax);
        }

        public override void VisitCompileTimeImportFromClauseSyntax(CompileTimeImportFromClauseSyntax syntax)
        {
            AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
            base.VisitCompileTimeImportFromClauseSyntax(syntax);
        }

        public override void VisitUsingDeclarationSyntax(UsingDeclarationSyntax syntax)
        {
            AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
            base.VisitUsingDeclarationSyntax(syntax);
        }

        public override void VisitParameterAssignmentSyntax(ParameterAssignmentSyntax syntax)
        {
            AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
            AddTokenType(syntax.Name, SemanticTokenType.Variable);
            base.VisitParameterAssignmentSyntax(syntax);
        }

        public override void VisitAssertDeclarationSyntax(AssertDeclarationSyntax syntax)
        {
            AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
            AddTokenType(syntax.Name, SemanticTokenType.Variable);
            base.VisitAssertDeclarationSyntax(syntax);
        }

        public override void VisitObjectTypePropertySyntax(ObjectTypePropertySyntax syntax)
        {
            if (syntax.Key is StringSyntax @string)
            {
                Visit(@string);
            }
            else
            {
                AddTokenType(syntax.Key, SemanticTokenType.TypeParameter);
            }
            Visit(syntax.Colon);
            Visit(syntax.Value);
        }
    }
}
