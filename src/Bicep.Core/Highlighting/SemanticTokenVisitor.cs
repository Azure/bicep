// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Highlighting;

public class SemanticTokenVisitor : CstVisitor
{
    private readonly List<SemanticToken> tokens = new();
    private readonly SemanticModel model;

    private SemanticTokenVisitor(SemanticModel model)
    {
        this.model = model;
    }

    public static ImmutableArray<SemanticToken> Build(SemanticModel model)
    {
        var visitor = new SemanticTokenVisitor(model);

        visitor.Visit(model.SourceFile.ProgramSyntax);

        return [.. visitor.tokens];
    }

    private void AddTokenType(IPositionable? positionable, SemanticTokenType tokenType)
    {
        if (positionable is not null)
        {
            tokens.Add(new(positionable, tokenType));
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

    public override void VisitBooleanTypeLiteralSyntax(BooleanTypeLiteralSyntax syntax)
    {
        AddTokenType(syntax.Literal, SemanticTokenType.Keyword);
        base.VisitBooleanTypeLiteralSyntax(syntax);
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

    public override void VisitNullTypeLiteralSyntax(NullTypeLiteralSyntax syntax)
    {
        AddTokenType(syntax.NullKeyword, SemanticTokenType.Keyword);
        base.VisitNullTypeLiteralSyntax(syntax);
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

    public override void VisitTypePropertyAccessSyntax(TypePropertyAccessSyntax syntax)
    {
        AddTokenType(syntax.Dot, SemanticTokenType.Operator);
        AddTokenType(syntax.PropertyName, GetSemanticTokenForPotentialTypeSymbol(model.GetSymbolInfo(syntax)));
        base.VisitTypePropertyAccessSyntax(syntax);
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

    public override void VisitFunctionDeclarationSyntax(FunctionDeclarationSyntax syntax)
    {
        AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
        AddTokenType(syntax.Name, SemanticTokenType.Function);
        base.VisitFunctionDeclarationSyntax(syntax);
    }

    private void AddStringToken(Token token, string? start, string? end)
    {
        var endInterp = (token.Type, end) switch
        {
            (TokenType.StringLeftPiece, { }) => end,
            (TokenType.StringMiddlePiece, { }) => end,
            _ => "",
        };

        var startInterp = (token.Type, start) switch
        {
            (TokenType.StringMiddlePiece, { }) => start,
            (TokenType.StringRightPiece, { }) => start,
            _ => "",
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

    public override void VisitStringTypeLiteralSyntax(StringTypeLiteralSyntax syntax)
    {
        var startAndEndTokens = Lexer.TryGetStartAndEndTokens(syntax.StringTokens).ToImmutableArray();
        for (var i = 0; i < syntax.StringTokens.Length; i++)
        {
            var result = startAndEndTokens[i];
            AddStringToken(syntax.StringTokens[i], result?.start, result?.end);
        }
        foreach (var expression in syntax.Expressions)
        {
            Visit(expression);
        }
    }

    public override void VisitStringSyntax(StringSyntax syntax)
    {
        var startAndEndTokens = Lexer.TryGetStartAndEndTokens(syntax.StringTokens).ToImmutableArray();
        for (var i = 0; i < syntax.StringTokens.Length; i++)
        {
            var result = startAndEndTokens[i];
            AddStringToken(syntax.StringTokens[i], result?.start, result?.end);
        }
        foreach (var expression in syntax.Expressions)
        {
            Visit(expression);
        }
    }

    public override void VisitToken(Token token)
    {
        switch (token.Type)
        {
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
        AddTokenType(syntax.Name, GetSemanticTokenForPotentialTypeSymbol(model.GetSymbolInfo(syntax)));
        base.VisitVariableAccessSyntax(syntax);
    }

    public override void VisitTypeVariableAccessSyntax(TypeVariableAccessSyntax syntax)
    {
        AddTokenType(syntax.Name, GetSemanticTokenForPotentialTypeSymbol(model.GetSymbolInfo(syntax)));
        base.VisitTypeVariableAccessSyntax(syntax);
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

    public override void VisitExtensionDeclarationSyntax(ExtensionDeclarationSyntax syntax)
    {
        AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
        this.Visit(syntax.SpecificationString);
        this.Visit(syntax.WithClause);
        this.Visit(syntax.AsClause);
    }

    public override void VisitExtensionWithClauseSyntax(ExtensionWithClauseSyntax syntax)
    {
        AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
        this.Visit(syntax.Config);
    }

    public override void VisitAliasAsClauseSyntax(AliasAsClauseSyntax syntax)
    {
        AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
        AddTokenType(syntax.Alias, SemanticTokenType.Namespace);
    }

    public override void VisitExtensionConfigAssignmentSyntax(ExtensionConfigAssignmentSyntax syntax)
    {
        AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
        this.Visit(syntax.Alias);
        this.Visit(syntax.WithClause);
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
                SymbolKind.TypeAlias => SemanticTokenType.Type,
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

    public override void VisitUsingWithClauseSyntax(UsingWithClauseSyntax syntax)
    {
        AddTokenType(syntax.Keyword, SemanticTokenType.Keyword);
        this.Visit(syntax.Config);
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

    private static SemanticTokenType GetSemanticTokenForPotentialTypeSymbol(Symbol? symbol) =>
        symbol switch
        {
            PropertySymbol property => GetSemanticTokenForPotentialTypeSymbol(property.Type),
            TypeSymbol or
            TypeAliasSymbol or
            AmbientTypeSymbol or
            ImportedTypeSymbol => SemanticTokenType.Type,
            INamespaceSymbol => SemanticTokenType.Namespace,
            _ => SemanticTokenType.Variable,
        };
}
