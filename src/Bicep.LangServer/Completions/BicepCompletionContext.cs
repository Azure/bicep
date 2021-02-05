// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Extensions;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Completions
{
    public class BicepCompletionContext
    {
        // completions will replace only these token types
        // all others will result in an insertion upon completion commit
        private static readonly ImmutableHashSet<TokenType> ReplaceableTokens = new[]
        {
            TokenType.Identifier,
            TokenType.Integer,
            TokenType.StringComplete
        }.Concat(LanguageConstants.Keywords.Values).ToImmutableHashSet();

        public BicepCompletionContext(
            BicepCompletionContextKind kind,
            Range replacementRange,
            SyntaxBase? enclosingDeclaration,
            ObjectSyntax? @object,
            ObjectPropertySyntax? property,
            ArraySyntax? array,
            PropertyAccessSyntax? propertyAccess,
            ArrayAccessSyntax? arrayAccess,
            TargetScopeSyntax? targetScope)
        {
            this.Kind = kind;
            this.ReplacementRange = replacementRange;
            this.EnclosingDeclaration = enclosingDeclaration;
            this.Object = @object;
            this.Property = property;
            this.Array = array;
            this.PropertyAccess = propertyAccess;
            this.ArrayAccess = arrayAccess;
            this.TargetScope = targetScope;
        }

        public BicepCompletionContextKind Kind { get; }

        public SyntaxBase? EnclosingDeclaration { get; }

        public ObjectSyntax? Object { get; }

        public ObjectPropertySyntax? Property { get; }

        public ArraySyntax? Array { get; }

        public PropertyAccessSyntax? PropertyAccess { get; }

        public ArrayAccessSyntax? ArrayAccess { get; }

        public TargetScopeSyntax? TargetScope { get; }

        public Range ReplacementRange { get; }


        public static BicepCompletionContext Create(SyntaxTree syntaxTree, int offset)
        {
            var matchingNodes = SyntaxMatcher.FindNodesMatchingOffset(syntaxTree.ProgramSyntax, offset);
            if (!matchingNodes.Any())
            {
                // this indicates a bug
                throw new ArgumentException($"The specified offset {offset} is outside the span of the specified {nameof(ProgramSyntax)} node.");
            }
            
            // the check at the beginning guarantees we have at least 1 node
            var replacementRange = GetReplacementRange(syntaxTree, matchingNodes[^1], offset);

            var matchingTriviaType = FindTriviaMatchingOffset(syntaxTree.ProgramSyntax, offset)?.Type;
            if (matchingTriviaType is not null && (matchingTriviaType == SyntaxTriviaType.MultiLineComment || matchingTriviaType == SyntaxTriviaType.SingleLineComment)) {
                //we're in a comment, no hints here
                return new BicepCompletionContext(BicepCompletionContextKind.None, replacementRange, null, null, null, null, null, null, null);
            }

            var topLeveldeclarationInfo = SyntaxMatcher.FindLastNodeOfType<ITopLevelNamedDeclarationSyntax, SyntaxBase>(matchingNodes);
            var objectInfo = SyntaxMatcher.FindLastNodeOfType<ObjectSyntax, ObjectSyntax>(matchingNodes);
            var propertyInfo = SyntaxMatcher.FindLastNodeOfType<ObjectPropertySyntax, ObjectPropertySyntax>(matchingNodes);
            var arrayInfo = SyntaxMatcher.FindLastNodeOfType<ArraySyntax, ArraySyntax>(matchingNodes);
            var propertyAccessInfo = SyntaxMatcher.FindLastNodeOfType<PropertyAccessSyntax, PropertyAccessSyntax>(matchingNodes);
            var arrayAccessInfo = SyntaxMatcher.FindLastNodeOfType<ArrayAccessSyntax, ArrayAccessSyntax>(matchingNodes);
            var targetScopeInfo = SyntaxMatcher.FindLastNodeOfType<TargetScopeSyntax, TargetScopeSyntax>(matchingNodes);

            var kind = ConvertFlag(IsTopLevelDeclarationStartContext(matchingNodes, offset), BicepCompletionContextKind.TopLevelDeclarationStart) |
                       GetDeclarationTypeFlags(matchingNodes, offset) |
                       ConvertFlag(IsObjectPropertyNameContext(matchingNodes, objectInfo), BicepCompletionContextKind.ObjectPropertyName) |
                       ConvertFlag(IsMemberAccessContext(matchingNodes, propertyAccessInfo, offset), BicepCompletionContextKind.MemberAccess) |
                       ConvertFlag(IsArrayIndexContext(matchingNodes,arrayAccessInfo), BicepCompletionContextKind.ArrayIndex | BicepCompletionContextKind.Expression) |
                       ConvertFlag(IsPropertyValueContext(matchingNodes, propertyInfo), BicepCompletionContextKind.PropertyValue | BicepCompletionContextKind.Expression) |
                       ConvertFlag(IsArrayItemContext(matchingNodes, arrayInfo), BicepCompletionContextKind.ArrayItem | BicepCompletionContextKind.Expression) |
                       ConvertFlag(IsResourceBodyContext(matchingNodes, offset), BicepCompletionContextKind.ResourceBody) |
                       ConvertFlag(IsModuleBodyContext(matchingNodes, offset), BicepCompletionContextKind.ModuleBody) |
                       ConvertFlag(IsOuterExpressionContext(matchingNodes, offset), BicepCompletionContextKind.Expression) |
                       ConvertFlag(IsTargetScopeContext(matchingNodes, offset), BicepCompletionContextKind.TargetScope);

            if (kind == BicepCompletionContextKind.None)
            {
                // previous processing hasn't identified a completion context kind
                // check if we're inside an expression
                kind |= ConvertFlag(IsInnerExpressionContext(matchingNodes), BicepCompletionContextKind.Expression);
            }

            return new BicepCompletionContext(kind, replacementRange, topLeveldeclarationInfo.node, objectInfo.node, propertyInfo.node, arrayInfo.node, propertyAccessInfo.node, arrayAccessInfo.node, targetScopeInfo.node);
        }

        /// <summary>
        /// Returnes trivia which span contains the specified offset.
        /// </summary>
        /// <param name="syntax">The program node</param>
        /// <param name="offset">The offset</param>
        private static SyntaxTrivia? FindTriviaMatchingOffset(ProgramSyntax syntax, int offset)
        {
            return syntax.TryFindMostSpecificTriviaInclusive(offset, current => true);
        }
        
        private static BicepCompletionContextKind ConvertFlag(bool value, BicepCompletionContextKind flag) => value ? flag : BicepCompletionContextKind.None;

        private static BicepCompletionContextKind GetDeclarationTypeFlags(IList<SyntaxBase> matchingNodes, int offset)
        {
            // local function
            bool CheckTypeIsExpected(SyntaxBase name, SyntaxBase type) => name.Span.Length > 0 && offset > name.GetEndPosition() && offset <= type.Span.Position;

            if (SyntaxMatcher.IsTailMatch<ParameterDeclarationSyntax>(matchingNodes, parameter => CheckTypeIsExpected(parameter.Name, parameter.Type)) ||
                SyntaxMatcher.IsTailMatch<ParameterDeclarationSyntax, TypeSyntax, Token>(matchingNodes, (_, _, token) => token.Type == TokenType.Identifier))
            {
                // the most specific matching node is a parameter declaration
                // the declaration syntax is "param <identifier> <type> ..."
                // the cursor position is on the type if we have an identifier (non-zero length span) and the offset matches the type position
                // OR
                // we are in a token that is inside a TypeSyntax node, which is inside a parameter node
                return BicepCompletionContextKind.ParameterType;
            }

            if (SyntaxMatcher.IsTailMatch<OutputDeclarationSyntax>(matchingNodes, output => CheckTypeIsExpected(output.Name, output.Type)) ||
                SyntaxMatcher.IsTailMatch<OutputDeclarationSyntax, TypeSyntax, Token>(matchingNodes, (_, _, token) => token.Type == TokenType.Identifier))
            {
                // the most specific matching node is an output declaration
                // the declaration syntax is "output <identifier> <type> ..."
                // the cursor position is on the type if we have an identifier (non-zero length span) and the offset matches the type position
                // OR
                // we are in a token that is inside a TypeSyntax node, which is inside an output node
                return BicepCompletionContextKind.OutputType;
            }

            if (SyntaxMatcher.IsTailMatch<ResourceDeclarationSyntax>(matchingNodes, resource => CheckTypeIsExpected(resource.Name, resource.Type)) ||
                SyntaxMatcher.IsTailMatch<ResourceDeclarationSyntax, StringSyntax, Token>(matchingNodes, (_, _, token) => token.Type == TokenType.StringComplete) ||
                SyntaxMatcher.IsTailMatch<ResourceDeclarationSyntax, SkippedTriviaSyntax, Token>(matchingNodes, (_, _, token) => token.Type == TokenType.Identifier))
            {
                // the most specific matching node is a resource declaration
                // the declaration syntax is "resource <identifier> '<type>' ..."
                // the cursor position is on the type if we have an identifier (non-zero length span) and the offset matches the type position
                // OR
                // we are in a token that is inside a StringSyntax node, which is inside a resource declaration
                // OR
                // we have an identifier in the place of a type in a resoure (this allows us to show completions when user just types virtualMachines instead of 'virtualMachines')
                return BicepCompletionContextKind.ResourceType;
            }

            if (SyntaxMatcher.IsTailMatch<ModuleDeclarationSyntax>(matchingNodes, module => CheckTypeIsExpected(module.Name, module.Path)) ||
                SyntaxMatcher.IsTailMatch<ModuleDeclarationSyntax, StringSyntax, Token>(matchingNodes, (_, _, token) => token.Type == TokenType.StringComplete) ||
                SyntaxMatcher.IsTailMatch<ModuleDeclarationSyntax, SkippedTriviaSyntax, Token>(matchingNodes, (_, _, token) => token.Type == TokenType.Identifier))
            {
                // the most specific matching node is a module declaration
                // the declaration syntax is "module <identifier> '<path>' ..."
                // the cursor position is on the type if we have an identifier (non-zero length span) and the offset matches the path position
                // OR
                // we are in a token that is inside a StringSyntax node, which is inside a module declaration
                return BicepCompletionContextKind.ModulePath;
            }

            return BicepCompletionContextKind.None;
        }

        private static bool IsTargetScopeContext(List<SyntaxBase> matchingNodes, int offset) =>
            SyntaxMatcher.IsTailMatch<TargetScopeSyntax>(matchingNodes, targetScope =>
                !targetScope.Assignment.Span.ContainsInclusive(offset) &&
                targetScope.Value is SkippedTriviaSyntax && offset == targetScope.Value.Span.Position) ||
            SyntaxMatcher.IsTailMatch<TargetScopeSyntax, Token>(matchingNodes, (targetScope, token) =>
                token.Type == TokenType.Assignment &&
                ReferenceEquals(targetScope.Assignment, token));

        private static bool IsTopLevelDeclarationStartContext(List<SyntaxBase> matchingNodes, int offset)
        {
            if (matchingNodes.Count == 1 && matchingNodes[0] is ProgramSyntax)
            {
                // the file is empty and the AST has a ProgramSyntax with 0 children and an EOF
                // because we picked the left node as winner, the only matching node is the ProgramSyntax node
                return true;
            }

            if (matchingNodes.Count >=2 && matchingNodes[^1] is Token token)
            {
                // we have at least 2 matching nodes in the "stack" and the last one is a token
                var node = matchingNodes[^2];

                switch (node)
                {
                    case ProgramSyntax _:
                        // the token at current position is inside a program node
                        // we're in a declaration if one of the following conditions is met:
                        // 1. the token is EOF
                        // 2. the token is a newline
                        return token.Type == TokenType.EndOfFile || 
                               token.Type == TokenType.NewLine;

                    case SkippedTriviaSyntax _ when matchingNodes.Count >= 3:
                        // we are in a line that has a partial declaration keyword (for example "resour" or "modu")
                        // if the token at current position is an identifier, assume declaration context
                        // (completions will be filtered by the text that is present, so we don't have to be 100% right)
                        return token.Type == TokenType.Identifier && matchingNodes[^3] is ProgramSyntax;

                    case ITopLevelNamedDeclarationSyntax declaration:
                        // we are in a fully or partially parsed declaration
                        // whether we are in a declaration context depends on whether our offset is within the keyword token
                        // (by using exclusive span containment, the cursor position at the end of a keyword token 
                        // result counts as being outside of the declaration context)
                        return declaration.Keyword.Span.Contains(offset);
                }
            }

            return false;
        }

        private static bool IsMemberAccessContext(List<SyntaxBase> matchingNodes, (PropertyAccessSyntax? node, int index) propertyAccessInfo, int offset)
        {
            return propertyAccessInfo.node != null &&
                   (SyntaxMatcher.IsTailMatch<PropertyAccessSyntax, IdentifierSyntax, Token>(
                        matchingNodes,
                        (propertyAccess, identifier, token) => ReferenceEquals(propertyAccess.PropertyName, identifier) && token.Type == TokenType.Identifier) ||
                    SyntaxMatcher.IsTailMatch<PropertyAccessSyntax, Token>(
                        matchingNodes,
                        (propertyAccess, token) => token.Type == TokenType.Dot && ReferenceEquals(propertyAccess.Dot, token)) ||
                    SyntaxMatcher.IsTailMatch<PropertyAccessSyntax>(
                        matchingNodes,
                        propertyAccess => offset > propertyAccess.Dot.Span.Position));
        }

        private static bool IsArrayIndexContext(List<SyntaxBase> matchingNodes, (ArrayAccessSyntax? node, int index) arrayAccessInfo)
        {
            return arrayAccessInfo.node != null &&
                   (SyntaxMatcher.IsTailMatch<ArrayAccessSyntax, Token>(
                        matchingNodes,
                        (arrayAccess, token) => token.Type == TokenType.LeftSquare && ReferenceEquals(arrayAccess.OpenSquare, token)) ||
                    SyntaxMatcher.IsTailMatch<ArrayAccessSyntax, VariableAccessSyntax, IdentifierSyntax, Token>(
                        matchingNodes,
                        (arrayAccess, variableAccess, _, token) => token.Type == TokenType.Identifier && ReferenceEquals(arrayAccess.IndexExpression, variableAccess)) ||
                    SyntaxMatcher.IsTailMatch<ArrayAccessSyntax, StringSyntax, Token>(
                        matchingNodes,
                        (arrayAccess, @string, token) => token.Type == TokenType.StringComplete && ReferenceEquals(arrayAccess.IndexExpression, @string)));
        }

        private static bool IsObjectPropertyNameContext(List<SyntaxBase> matchingNodes, (ObjectSyntax? node, int index) objectInfo)
        {
            if (objectInfo.node == null)
            {
                // none of the matching nodes are ObjectSyntax,
                // so we cannot possibly be in a position to begin an object property
                return false;
            }
            
            switch (matchingNodes[^1])
            {
                case ObjectSyntax _:
                    // we are somewhere in the trivia portion of the object node (trivia span is not included in the token span)
                    // which is why the last node in the list of matching nodes is not a Token.
                    return true;

                case Token token:
                    int nodeCount = matchingNodes.Count - objectInfo.index;

                    switch (nodeCount)
                    {
                        case 2 when token.Type == TokenType.NewLine:
                            return true;

                        case 4 when matchingNodes[^2] is IdentifierSyntax identifier && matchingNodes[^3] is ObjectPropertySyntax property && ReferenceEquals(property.Key, identifier):
                            // we are in a partial or full property name 
                            return true;

                        case 4 when matchingNodes[^2] is SkippedTriviaSyntax skipped && matchingNodes[^3] is ObjectPropertySyntax property && ReferenceEquals(property.Key, skipped):
                            return true;
                    }

                    break;
            }

            return false;
        }

        private static bool IsPropertyValueContext(List<SyntaxBase> matchingNodes, (ObjectPropertySyntax? node, int index) propertyInfo)
        {
            // find the innermost property
            if (propertyInfo.node == null)
            {
                // none of the nodes are object properties,
                // so we can't possibly be in a property value context
                return false;
            }

            switch (matchingNodes[^1])
            {
                case ObjectPropertySyntax _:
                    // the cursor position may be in the trivia following the colon that follows the property name
                    // if that's the case, the offset should match the end of the property span exactly
                    return true;

                case Token token:
                    // how many matching nodes remain including the object node itself
                    int nodeCount = matchingNodes.Count - propertyInfo.index;

                    switch (nodeCount)
                    {
                        case 2 when token.Type == TokenType.Colon:
                        {
                            // the cursor position is after the colon that follows the property name
                            return true;
                        }

                        case 3 when matchingNodes[^2] is StringSyntax stringSyntax && ReferenceEquals(propertyInfo.node.Value, stringSyntax):
                        {
                            // the cursor is inside a string value of the property
                            return true;
                        }

                        case 4 when matchingNodes[^2] is IdentifierSyntax identifier && ReferenceEquals(propertyInfo.node.Value, identifier):
                        {
                            // the cursor could is a partial or full identifier
                            // which will present as either a keyword or identifier token
                            return true;
                        }
                    }

                    break;
            }

            return false;
        }

        private static bool IsArrayItemContext(List<SyntaxBase> matchingNodes, (ArraySyntax? node, int index) arrayInfo)
        {
            if (arrayInfo.node == null)
            {
                // none of the nodes are arrays
                // so we can't possibly be in an array item context
                return false;
            }

            switch (matchingNodes[^1])
            {
                case ArraySyntax _:
                    return true;

                case Token token:
                    int nodeCount = matchingNodes.Count - arrayInfo.index;

                    switch (nodeCount)
                    {
                        case 2:
                            return token.Type == TokenType.NewLine;

                        case 5:
                            return token.Type == TokenType.Identifier;
                    }

                    break;
            }

            return false;
        }

        private static bool IsResourceBodyContext(List<SyntaxBase> matchingNodes, int offset)
        {
            // resources only allow {} as the body so we don't need to worry about
            // providing completions for a partially-typed identifier
            switch (matchingNodes[^1])
            {
                case ResourceDeclarationSyntax resource:
                    return !resource.Name.Span.ContainsInclusive(offset) &&
                           !resource.Type.Span.ContainsInclusive(offset) &&
                           !resource.Assignment.Span.ContainsInclusive(offset) &&
                           resource.Value is SkippedTriviaSyntax && offset == resource.Value.Span.Position;

                case Token token when token.Type == TokenType.Assignment && matchingNodes.Count >= 2 && offset == token.GetEndPosition():
                    // cursor is after the = token
                    // check the type
                    return matchingNodes[^2] is ResourceDeclarationSyntax;
        }

            return false;
        }

        private static bool IsModuleBodyContext(List<SyntaxBase> matchingNodes, int offset)
        {
            // modules only allow {} as the body so we don't need to worry about
            // providing completions for a partially-typed identifier
            switch (matchingNodes[^1])
            {
                case ModuleDeclarationSyntax module:
                    return !module.Name.Span.ContainsInclusive(offset) &&
                           !module.Path.Span.ContainsInclusive(offset) &&
                           !module.Assignment.Span.ContainsInclusive(offset) &&
                           module.Value is SkippedTriviaSyntax && offset == module.Value.Span.Position;

                case Token token when token.Type == TokenType.Assignment && matchingNodes.Count >= 2 && offset == token.GetEndPosition():
                    // cursor is after the = token
                    // check the type
                    return matchingNodes[^2] is ModuleDeclarationSyntax;
            }

            return false;
        }

        private static bool IsOuterExpressionContext(List<SyntaxBase> matchingNodes, int offset)
        {
            switch (matchingNodes[^1])
            {
                case ParameterDefaultValueSyntax paramDefault:
                    return !paramDefault.AssignmentToken.Span.ContainsInclusive(offset) &&
                           paramDefault.DefaultValue is SkippedTriviaSyntax && offset == paramDefault.DefaultValue.Span.Position;

                case VariableDeclarationSyntax variable:
                    // is the cursor after the equals sign in the variable?
                    return !variable.Name.Span.ContainsInclusive(offset) &&
                           !variable.Assignment.Span.ContainsInclusive(offset) &&
                           variable.Value is SkippedTriviaSyntax && offset == variable.Value.Span.Position;

                case OutputDeclarationSyntax output:
                    // is the cursor after the equals sign in the output?
                    return !output.Name.Span.ContainsInclusive(offset) &&
                           !output.Type.Span.ContainsInclusive(offset) &&
                           !output.Assignment.Span.ContainsInclusive(offset) &&
                           output.Value is SkippedTriviaSyntax && offset == output.Value.Span.Position;

                case Token token when token.Type == TokenType.Identifier && matchingNodes.Count >= 4 && matchingNodes[^3] is VariableAccessSyntax variableAccess:
                    switch (matchingNodes[^4])
                    {
                        case VariableDeclarationSyntax variableDeclaration:
                            return ReferenceEquals(variableDeclaration.Value, variableAccess);

                        case OutputDeclarationSyntax outputDeclaration:
                            return ReferenceEquals(outputDeclaration.Value, variableAccess);
                    }
                    
                    break;

                case Token token when token.Type == TokenType.Assignment && matchingNodes.Count >=2 && offset == token.GetEndPosition():
                    // cursor is after the = token
                    // check if parent is of the right type
                    var parent = matchingNodes[^2];
                    return parent is ParameterDefaultValueSyntax ||
                           parent is VariableDeclarationSyntax ||
                           parent is OutputDeclarationSyntax ||
                           parent is ParameterDeclarationSyntax;
            }

            return false;
        }

        /// <summary>
        /// Determines if we are inside an expression. Will not produce a correct result if context kind is set is already set to something.
        /// </summary>
        /// <param name="matchingNodes">The matching nodes</param>
        private static bool IsInnerExpressionContext(List<SyntaxBase> matchingNodes) => matchingNodes.OfType<ExpressionSyntax>().Any();

        private static Range GetReplacementRange(SyntaxTree syntaxTree, SyntaxBase innermostMatchingNode, int offset)
        {
            if (innermostMatchingNode is Token token && ReplaceableTokens.Contains(token.Type))
            {
                // the token is replaceable - replace it
                return token.Span.ToRange(syntaxTree.LineStarts);
            }

            // the innermost matching node is either a non-token or it's not replaceable
            // (non-replaceable tokens include colons, newlines, parens, etc.)
            // produce an insertion edit
            return new TextSpan(offset, 0).ToRange(syntaxTree.LineStarts);
        }
    }
}
