// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core;
using Bicep.Core.Navigation;
using Bicep.Core.Parser;
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
            TokenType.Number,
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
            var matchingNodes = FindNodesMatchingOffset(syntaxTree.ProgramSyntax, offset);
            if (!matchingNodes.Any())
            {
                // this indicates a bug
                throw new ArgumentException($"The specified offset {offset} is outside the span of the specified {nameof(ProgramSyntax)} node.");
            }

            var declarationInfo = FindLastNodeOfType<INamedDeclarationSyntax, SyntaxBase>(matchingNodes);
            var objectInfo = FindLastNodeOfType<ObjectSyntax, ObjectSyntax>(matchingNodes);
            var propertyInfo = FindLastNodeOfType<ObjectPropertySyntax, ObjectPropertySyntax>(matchingNodes);
            var arrayInfo = FindLastNodeOfType<ArraySyntax, ArraySyntax>(matchingNodes);
            var propertyAccessInfo = FindLastNodeOfType<PropertyAccessSyntax, PropertyAccessSyntax>(matchingNodes);
            var arrayAccessInfo = FindLastNodeOfType<ArrayAccessSyntax, ArrayAccessSyntax>(matchingNodes);
            var targetScopeInfo = FindLastNodeOfType<TargetScopeSyntax, TargetScopeSyntax>(matchingNodes);

            var kind = ConvertFlag(IsDeclarationStartContext(matchingNodes, offset), BicepCompletionContextKind.DeclarationStart) |
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

            // the check at the beginning guarantees we have at least 1 node
            var replacementRange = GetReplacementRange(syntaxTree, matchingNodes[^1], offset);

            return new BicepCompletionContext(kind, replacementRange, declarationInfo.node, objectInfo.node, propertyInfo.node, arrayInfo.node, propertyAccessInfo.node, arrayAccessInfo.node, targetScopeInfo.node);
        }

        /// <summary>
        /// Returnes nodes whose span contains the specified offset from least specific to the most specific.
        /// </summary>
        /// <param name="syntax">The program node</param>
        /// <param name="offset">The offset</param>
        private static List<SyntaxBase> FindNodesMatchingOffset(ProgramSyntax syntax, int offset)
        {
            var nodes = new List<SyntaxBase>();
            syntax.TryFindMostSpecificNodeInclusive(offset, current =>
            {
                // callback is invoked only if node span contains the offset
                // in inclusive mode, 2 nodes can be returned if cursor is between end of one node and beginning of another
                // we will pick the node to the left as the winner
                if (nodes.Any() == false || TextSpan.AreNeighbors(nodes.Last(), current) == false)
                {
                    nodes.Add(current);
                }

                // don't filter out the nodes
                return true;
            });

            return nodes;
        }

        private static BicepCompletionContextKind ConvertFlag(bool value, BicepCompletionContextKind flag) => value ? flag : BicepCompletionContextKind.None;

        private static BicepCompletionContextKind GetDeclarationTypeFlags(IList<SyntaxBase> matchingNodes, int offset)
        {
            if (matchingNodes.Count < 2)
            {
                return BicepCompletionContextKind.None;
            }

            switch (matchingNodes[^1])
            {
                case ParameterDeclarationSyntax parameter:
                    // the most specific matching node is a parameter declaration
                    // the declaration syntax is "param <identifier> <type> ..."
                    // the cursor position is on the type if we have an identifier (non-zero length span) and the offset matches the type position
                    return ConvertFlag(parameter.Name.Span.Length > 0 && parameter.Type.Span.Position == offset, BicepCompletionContextKind.ParameterType);

                case OutputDeclarationSyntax output:
                    // the most specific matching node is an output declaration
                    // the declaration syntax is "output <identifier> <type> ..."
                    // the cursor position is on the type if we have an identifier (non-zero length span) and the offset matches the type position
                    return ConvertFlag(output.Name.Span.Length > 0 && output.Type.Span.Position == offset, BicepCompletionContextKind.OutputType);

                case ResourceDeclarationSyntax resource:
                    // the most specific matching node is a resource declaration
                    // the declaration syntax is "resource <identifier> '<type>' ..."
                    // the cursor position is on the type if we have an identifier (non-zero length span) and the offset matches the type position
                    return ConvertFlag(resource.Name.Span.Length > 0 && resource.Type.Span.Position == offset, BicepCompletionContextKind.ResourceType);

                case ModuleDeclarationSyntax module:
                    // the most specific matching node is a module declaration
                    // the declaration syntax is "module <identifier> '<path>' ..."
                    // the cursor position is on the type if we have an identifier (non-zero length span) and the offset matches the path position
                    return ConvertFlag(module.Name.Span.Length > 0 && module.Path.Span.Position == offset, BicepCompletionContextKind.ModulePath);

                case Token token when token.Type == TokenType.Identifier && matchingNodes[^2] is TypeSyntax && matchingNodes.Count >= 3:
                    // we are in a token that is inside a TypeSyntax node, which is inside some other node
                    switch (matchingNodes[^3])
                    {
                        case ParameterDeclarationSyntax _:
                            // type syntax is inside a param declaration
                            return BicepCompletionContextKind.ParameterType;

                        case OutputDeclarationSyntax _:
                            // type syntax is inside an output declaration
                            return BicepCompletionContextKind.OutputType;
                    }

                    break;

                case Token token when token.Type == TokenType.StringComplete && matchingNodes[^2] is StringSyntax && matchingNodes.Count >= 3:
                    // we are in a token that is inside a StringSyntax node, which is inside some other node
                    switch (matchingNodes[^3])
                    {
                        case ResourceDeclarationSyntax _:
                            // the string syntax is inside a param declaration
                            return BicepCompletionContextKind.ResourceType;

                        case ModuleDeclarationSyntax _:
                            // the string syntax is inside a module declaration
                            return BicepCompletionContextKind.ModulePath;
                    }

                    break;

                case Token token when token.Type == TokenType.Identifier && matchingNodes[^2] is SkippedTriviaSyntax && matchingNodes.Count >= 3 && matchingNodes[^3] is ResourceDeclarationSyntax:
                    // we have an identifier in the place of a type in a resour
                    return BicepCompletionContextKind.ResourceType;
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

        private static bool IsDeclarationStartContext(List<SyntaxBase> matchingNodes, int offset)
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

                    case INamedDeclarationSyntax declaration:
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
                           resource.Body is SkippedTriviaSyntax && offset == resource.Body.Span.Position;

                case Token token when token.Type == TokenType.Assignment && matchingNodes.Count >= 2 && offset == token.Span.Position + token.Span.Length:
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
                           module.Body is SkippedTriviaSyntax && offset == module.Body.Span.Position;

                case Token token when token.Type == TokenType.Assignment && matchingNodes.Count >= 2 && offset == token.Span.Position + token.Span.Length:
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

                case Token token when token.Type == TokenType.Assignment && matchingNodes.Count >=2 && offset == token.Span.Position + token.Span.Length:
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

        private static (TResult? node, int index) FindLastNodeOfType<TPredicate, TResult>(List<SyntaxBase> matchingNodes) where TResult : SyntaxBase
        {
            var index = matchingNodes.FindLastIndex(matchingNodes.Count - 1, n => n is TPredicate);
            var node = index < 0 ? null : matchingNodes[index] as TResult;

            return (node, index);
        }

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
