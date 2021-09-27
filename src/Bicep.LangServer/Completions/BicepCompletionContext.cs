// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
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
            ResourceAccessSyntax? resourceAccess,
            ArrayAccessSyntax? arrayAccess,
            TargetScopeSyntax? targetScope,
            ImmutableArray<ILanguageScope> activeScopes)
        {
            this.Kind = kind;
            this.ReplacementRange = replacementRange;
            this.EnclosingDeclaration = enclosingDeclaration;
            this.Object = @object;
            this.Property = property;
            this.Array = array;
            this.PropertyAccess = propertyAccess;
            this.ResourceAccess = resourceAccess;
            this.ArrayAccess = arrayAccess;
            this.TargetScope = targetScope;
            this.ActiveScopes = activeScopes;
        }

        public BicepCompletionContextKind Kind { get; }

        public SyntaxBase? EnclosingDeclaration { get; }

        public ObjectSyntax? Object { get; }

        public ObjectPropertySyntax? Property { get; }

        public ArraySyntax? Array { get; }

        public PropertyAccessSyntax? PropertyAccess { get; }

        public ResourceAccessSyntax? ResourceAccess { get; }

        public ArrayAccessSyntax? ArrayAccess { get; }

        public TargetScopeSyntax? TargetScope { get; }

        public ImmutableArray<ILanguageScope> ActiveScopes { get; }

        public Range ReplacementRange { get; }


        public static BicepCompletionContext Create(IFeatureProvider featureProvider, Compilation compilation, int offset)
        {
            var bicepFile = compilation.SourceFileGrouping.EntryPoint;
            var matchingNodes = SyntaxMatcher.FindNodesMatchingOffset(bicepFile.ProgramSyntax, offset);
            if (!matchingNodes.Any())
            {
                // this indicates a bug
                throw new ArgumentException($"The specified offset {offset} is outside the span of the specified {nameof(ProgramSyntax)} node.");
            }

            // the check at the beginning guarantees we have at least 1 node
            var replacementRange = GetReplacementRange(bicepFile, matchingNodes[^1], offset);

            var triviaMatchingOffset = FindTriviaMatchingOffset(bicepFile.ProgramSyntax, offset);
            switch (triviaMatchingOffset?.Type)
            {
                case SyntaxTriviaType.SingleLineComment when offset > triviaMatchingOffset.Span.Position:
                case SyntaxTriviaType.MultiLineComment when offset > triviaMatchingOffset.Span.Position && offset < triviaMatchingOffset.Span.Position + triviaMatchingOffset.Span.Length:
                    //we're in a comment, no hints here
                    return new BicepCompletionContext(BicepCompletionContextKind.None, replacementRange, null, null, null, null, null, null, null, null, ImmutableArray<ILanguageScope>.Empty);
            }

            var topLevelDeclarationInfo = SyntaxMatcher.FindLastNodeOfType<ITopLevelNamedDeclarationSyntax, SyntaxBase>(matchingNodes);
            var objectInfo = SyntaxMatcher.FindLastNodeOfType<ObjectSyntax, ObjectSyntax>(matchingNodes);
            var propertyInfo = SyntaxMatcher.FindLastNodeOfType<ObjectPropertySyntax, ObjectPropertySyntax>(matchingNodes);
            var arrayInfo = SyntaxMatcher.FindLastNodeOfType<ArraySyntax, ArraySyntax>(matchingNodes);
            var propertyAccessInfo = SyntaxMatcher.FindLastNodeOfType<PropertyAccessSyntax, PropertyAccessSyntax>(matchingNodes);
            var resourceAccessInfo = SyntaxMatcher.FindLastNodeOfType<ResourceAccessSyntax, ResourceAccessSyntax>(matchingNodes);
            var arrayAccessInfo = SyntaxMatcher.FindLastNodeOfType<ArrayAccessSyntax, ArrayAccessSyntax>(matchingNodes);
            var targetScopeInfo = SyntaxMatcher.FindLastNodeOfType<TargetScopeSyntax, TargetScopeSyntax>(matchingNodes);
            var activeScopes = ActiveScopesVisitor.GetActiveScopes(compilation.GetEntrypointSemanticModel().Root, offset);

            var kind = ConvertFlag(IsTopLevelDeclarationStartContext(matchingNodes, offset), BicepCompletionContextKind.TopLevelDeclarationStart) |
                       ConvertFlag(IsNestedResourceStartContext(matchingNodes, topLevelDeclarationInfo, objectInfo, offset), BicepCompletionContextKind.NestedResourceDeclarationStart) |
                       GetDeclarationTypeFlags(matchingNodes, offset) |
                       ConvertFlag(IsResourceTypeFollowerContext(matchingNodes, offset), BicepCompletionContextKind.ResourceTypeFollower) |
                       GetObjectPropertyNameFlags(matchingNodes, objectInfo, offset) |
                       ConvertFlag(IsMemberAccessContext(matchingNodes, propertyAccessInfo, offset), BicepCompletionContextKind.MemberAccess) |
                       ConvertFlag(IsResourceAccessContext(matchingNodes, resourceAccessInfo, offset), BicepCompletionContextKind.ResourceAccess) |
                       ConvertFlag(IsArrayIndexContext(matchingNodes, arrayAccessInfo), BicepCompletionContextKind.ArrayIndex | BicepCompletionContextKind.Expression) |
                       GetPropertyValueFlags(matchingNodes, propertyInfo, offset) |
                       ConvertFlag(IsArrayItemContext(matchingNodes, arrayInfo, offset), BicepCompletionContextKind.ArrayItem | BicepCompletionContextKind.Expression) |
                       ConvertFlag(IsResourceBodyContext(matchingNodes, offset), BicepCompletionContextKind.ResourceBody) |
                       ConvertFlag(IsModuleBodyContext(matchingNodes, offset), BicepCompletionContextKind.ModuleBody) |
                       ConvertFlag(IsParameterDefaultValueContext(matchingNodes, offset), BicepCompletionContextKind.ParameterDefaultValue | BicepCompletionContextKind.Expression) |
                       ConvertFlag(IsVariableValueContext(matchingNodes, offset), BicepCompletionContextKind.VariableValue | BicepCompletionContextKind.Expression) |
                       ConvertFlag(IsOutputValueContext(matchingNodes, offset), BicepCompletionContextKind.OutputValue | BicepCompletionContextKind.Expression) |
                       ConvertFlag(IsOuterExpressionContext(matchingNodes, offset), BicepCompletionContextKind.Expression) |
                       ConvertFlag(IsTargetScopeContext(matchingNodes, offset), BicepCompletionContextKind.TargetScope) |
                       ConvertFlag(IsDecoratorNameContext(matchingNodes, offset), BicepCompletionContextKind.DecoratorName);
                       
            if (featureProvider.ImportsEnabled)
            {
                kind |= ConvertFlag(IsImportSymbolFollower(matchingNodes, offset), BicepCompletionContextKind.ImportSymbolFollower) |
                    ConvertFlag(IsImportFromFollower(matchingNodes, offset), BicepCompletionContextKind.ImportFromFollower);
            }

            if (kind == BicepCompletionContextKind.None)
            {
                // previous processing hasn't identified a completion context kind
                // check if we're inside an expression
                kind |= ConvertFlag(IsInnerExpressionContext(matchingNodes, offset), BicepCompletionContextKind.Expression);
            }

            return new BicepCompletionContext(
                kind,
                replacementRange,
                topLevelDeclarationInfo.node,
                objectInfo.node,
                propertyInfo.node,
                arrayInfo.node,
                propertyAccessInfo.node,
                resourceAccessInfo.node,
                arrayAccessInfo.node,
                targetScopeInfo.node,
                activeScopes);
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
                SyntaxMatcher.IsTailMatch<ResourceDeclarationSyntax, SkippedTriviaSyntax, Token>(matchingNodes, (resource, skipped, token) => resource.Type == skipped && token.Type == TokenType.Identifier))
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

        private static bool IsResourceTypeFollowerContext(List<SyntaxBase> matchingNodes, int offset) =>
            // resource foo '...' |
            // OR
            // resource foo '...' | = {
            SyntaxMatcher.IsTailMatch<ResourceDeclarationSyntax>(matchingNodes, resource => offset > resource.Type.GetEndPosition() && offset <= resource.Assignment.Span.Position) ||
            // resource foo '...' e|
            // OR
            // resource foo '...' e| = {
            SyntaxMatcher.IsTailMatch<ResourceDeclarationSyntax, SkippedTriviaSyntax, Token>(matchingNodes, (resource, skipped, token) => resource.Assignment == skipped && token.Type == TokenType.Identifier) ||
            // resource foo '...' |=
            SyntaxMatcher.IsTailMatch<ResourceDeclarationSyntax, Token>(matchingNodes, (resource, token) => resource.Assignment == token && token.Type == TokenType.Assignment && offset == token.Span.Position);

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
                    case ProgramSyntax programSyntax:
                        // the token at current position is inside a program node
                        // we're in a declaration if one of the following conditions is met:
                        // 1. the token is EOF
                        // 2. the token is a newline and we can insert at the offset
                        return token.Type == TokenType.EndOfFile ||
                               (token.Type == TokenType.NewLine && CanInsertChildNodeAtOffset(programSyntax, offset));

                    case SkippedTriviaSyntax _ when matchingNodes.Count >= 3:
                        // we are in a line that has a partial declaration keyword (for example "resour" or "modu")
                        // if the token at current position is an identifier, assume declaration context
                        // (completions will be filtered by the text that is present, so we don't have to be 100% right)
                        return token.Type == TokenType.Identifier && matchingNodes[^3] is ProgramSyntax;

                    case ITopLevelNamedDeclarationSyntax declaration:
                        // we are in a partially parsed declaration which only contains a keyword
                        // whether we are in a declaration context depends on whether our offset is within the keyword token
                        // (by using exclusive span containment, the cursor position at the end of a keyword token
                        // result counts as being outside of the declaration context)
                        return declaration.Name.IdentifierName.Equals(LanguageConstants.MissingName, LanguageConstants.IdentifierComparison) &&
                            declaration.Keyword.Span.Contains(offset);
                }
            }

            return false;
        }

        private static bool IsNestedResourceStartContext(List<SyntaxBase> matchingNodes, (SyntaxBase? node, int index) topLevelDeclarationInfo, (ObjectSyntax? node, int index) objectInfo, int offset)
        {
            // A nested resource must be at the top level of a resource declaration (not nested within another level of object).
            if (objectInfo.node == null)
            {
                // none of the matching nodes are ObjectSyntax,
                // so we cannot possibly be in a position to begin an object property
                return false;
            }

            if (topLevelDeclarationInfo.node == null || topLevelDeclarationInfo.node is not ResourceDeclarationSyntax resourceDeclarationSyntax)
            {
                // not inside of a resource, top level declarations are handled separately.
                return false;
            }

            if (!ReferenceEquals(resourceDeclarationSyntax.TryGetBody(), objectInfo.node))
            {
                // we're inside of an object, but it's not the body of a resource.
                return false;
            }

            return
                SyntaxMatcher.IsTailMatch<ObjectSyntax>(
                    matchingNodes,
                    objectSyntax => CanInsertChildNodeAtOffset(objectSyntax, offset)) ||
                SyntaxMatcher.IsTailMatch<ObjectSyntax, Token>(
                    matchingNodes,
                    (objectSyntax, token) => token.Type == TokenType.NewLine && CanInsertChildNodeAtOffset(objectSyntax, offset)) ||
                SyntaxMatcher.IsTailMatch<ObjectPropertySyntax, IdentifierSyntax, Token>(
                    matchingNodes,
                    (objectPropertySyntax, _, _) => objectPropertySyntax.Colon is SkippedTriviaSyntax && objectPropertySyntax.Value is SkippedTriviaSyntax);
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

        private static bool IsResourceAccessContext(List<SyntaxBase> matchingNodes, (ResourceAccessSyntax? node, int index) resourceAccessInfo, int offset)
        {
            return resourceAccessInfo.node != null &&
                   (SyntaxMatcher.IsTailMatch<ResourceAccessSyntax, IdentifierSyntax, Token>(
                        matchingNodes,
                        (propertyAccess, identifier, token) => ReferenceEquals(propertyAccess.ResourceName, identifier) && token.Type == TokenType.Identifier) ||
                    SyntaxMatcher.IsTailMatch<ResourceAccessSyntax, Token>(
                        matchingNodes,
                        (propertyAccess, token) => token.Type == TokenType.DoubleColon && ReferenceEquals(propertyAccess.DoubleColon, token)) ||
                    SyntaxMatcher.IsTailMatch<ResourceAccessSyntax>(
                        matchingNodes,
                        propertyAccess => offset > propertyAccess.DoubleColon.Span.Position));
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

        private static BicepCompletionContextKind GetObjectPropertyNameFlags(List<SyntaxBase> matchingNodes, (ObjectSyntax? node, int index) objectInfo, int offset)
        {
            if (objectInfo.node == null)
            {
                // none of the matching nodes are ObjectSyntax,
                // so we cannot possibly be in a position to begin an object property
                return BicepCompletionContextKind.None;
            }

            // assume colon doesn't exist
            var colonExists = false;
            bool CheckColonExists(ObjectPropertySyntax property) => colonExists = property.Colon is Token { Type: TokenType.Colon };

            var isObjectProperty =
                // we are somewhere in the trivia portion of the object node (trivia span is not included in the token span)
                // which is why the last node in the list of matching nodes is not a Token.
                SyntaxMatcher.IsTailMatch<ObjectSyntax>(matchingNodes, objectSyntax => CanInsertChildNodeAtOffset(objectSyntax, offset)) ||

                SyntaxMatcher.IsTailMatch<ObjectSyntax, Token>(
                    matchingNodes,
                    (objectSyntax, token) => token.Type == TokenType.NewLine && CanInsertChildNodeAtOffset(objectSyntax, offset)) ||

                // we are in a partial or full property name
                SyntaxMatcher.IsTailMatch<ObjectSyntax, ObjectPropertySyntax, IdentifierSyntax, Token>(
                    matchingNodes,
                    (_, property, identifier, _) => ReferenceEquals(property.Key, identifier),
                    (_, property, _, _) => CheckColonExists(property)) ||

                // we are in a missing or malformed property name
                SyntaxMatcher.IsTailMatch<ObjectSyntax, ObjectPropertySyntax, SkippedTriviaSyntax, Token>(
                    matchingNodes,
                    (_, property, skipped, _) => ReferenceEquals(property.Key, skipped),
                    (_, property, _, _) => CheckColonExists(property));

            return ConvertFlag(isObjectProperty, BicepCompletionContextKind.ObjectPropertyName) | ConvertFlag(colonExists, BicepCompletionContextKind.ObjectPropertyColonExists);
        }

        private static BicepCompletionContextKind GetPropertyValueFlags(List<SyntaxBase> matchingNodes, (ObjectPropertySyntax? node, int index) propertyInfo, int offset)
        {
            if (propertyInfo.node is not null)
            {
                if (
                    // the cursor position may be in the trivia following the colon that follows the property name
                    // if that's the case, the offset should match the end of the property span exactly
                    SyntaxMatcher.IsTailMatch<ObjectPropertySyntax>(matchingNodes, property => property.Colon is not SkippedTriviaSyntax && offset >= property.Colon.GetEndPosition() && property.Value is SkippedTriviaSyntax) ||
                    // the cursor position is after the colon that follows the property name
                    SyntaxMatcher.IsTailMatch<ObjectPropertySyntax, Token>(matchingNodes, (_, token) => token.Type == TokenType.Colon && offset>= token.GetEndPosition()) ||
                    // the cursor is inside a string value of the property that is not interpolated
                    SyntaxMatcher.IsTailMatch<ObjectPropertySyntax, StringSyntax, Token>(matchingNodes, (property, stringSyntax, token) => ReferenceEquals(property.Value, stringSyntax) && !stringSyntax.Expressions.Any()) ||
                    // the cursor could is a partial or full identifier
                    // which will present as either a keyword or identifier token
                    SyntaxMatcher.IsTailMatch<ObjectPropertySyntax, VariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (property, variableAccess, identifier, token) => ReferenceEquals(property.Value, variableAccess)))
                {
                    return BicepCompletionContextKind.PropertyValue | BicepCompletionContextKind.Expression;
                };

                // | indicates cursor position
                if (
                    // {
                    //   key |
                    // }
                    SyntaxMatcher.IsTailMatch<ObjectPropertySyntax>(matchingNodes, property => true) ||
                    // {
                    //   key |:
                    // }
                    SyntaxMatcher.IsTailMatch<ObjectPropertySyntax, Token>(matchingNodes, (_, token) => token.Type == TokenType.Colon))
                {
                    // we don't want to offer completions in this case and also want to prevent fallback to expression
                    return BicepCompletionContextKind.NotValid;
                }
            }

            return BicepCompletionContextKind.None;
        }

        private static bool IsArrayItemContext(List<SyntaxBase> matchingNodes, (ArraySyntax? node, int index) arrayInfo, int offset)
        {
            if (arrayInfo.node == null)
            {
                // none of the nodes are arrays
                // so we can't possibly be in an array item context
                return false;
            }

            switch (matchingNodes[^1])
            {
                case ArraySyntax arraySyntax:
                    return CanInsertChildNodeAtOffset(arraySyntax, offset);

                case Token token:
                    int nodeCount = matchingNodes.Count - arrayInfo.index;

                    switch (nodeCount)
                    {
                        case 2:
                            return token.Type == TokenType.NewLine && CanInsertChildNodeAtOffset((ArraySyntax)matchingNodes[^2], offset);

                        case 5:
                            return token.Type == TokenType.Identifier;
                    }

                    break;
            }

            return false;
        }

        private static bool IsParameterDefaultValueContext(List<SyntaxBase> matchingNodes, int offset) =>
            // | below indicates cursor position
            // param foo type = |
            SyntaxMatcher.IsTailMatch<ParameterDeclarationSyntax, ParameterDefaultValueSyntax>(matchingNodes, (_, @default) => offset >= @default.AssignmentToken.GetEndPosition()) ||
            // param foo type =|
            SyntaxMatcher.IsTailMatch<ParameterDeclarationSyntax, ParameterDefaultValueSyntax, Token>(matchingNodes, (_, @default, token) => @default.AssignmentToken == token && offset == token.GetEndPosition()) ||
            // param foo type = a|
            SyntaxMatcher.IsTailMatch<ParameterDeclarationSyntax, ParameterDefaultValueSyntax, VariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (_, _, _, _, token) => token.Type == TokenType.Identifier);

        private static bool IsOutputValueContext(List<SyntaxBase> matchingNodes, int offset) =>
            // | below indicates cursor position
            // output foo type = |
            SyntaxMatcher.IsTailMatch<OutputDeclarationSyntax>(matchingNodes, output => output.Assignment is not SkippedTriviaSyntax && offset >= output.Assignment.GetEndPosition()) ||
            // output foo type =|
            SyntaxMatcher.IsTailMatch<OutputDeclarationSyntax, Token>(matchingNodes, (output, token) => output.Assignment == token && token.Type == TokenType.Assignment && offset == token.GetEndPosition()) ||
            // output foo type = a|
            SyntaxMatcher.IsTailMatch<OutputDeclarationSyntax, VariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (_, _, _, token) => token.Type == TokenType.Identifier);

        private static bool IsVariableValueContext(List<SyntaxBase> matchingNodes, int offset) =>
            // | below indicates cursor position
            // var foo = |
            SyntaxMatcher.IsTailMatch<VariableDeclarationSyntax>(matchingNodes, variable => variable.Assignment is not SkippedTriviaSyntax && offset >= variable.Assignment.GetEndPosition()) ||
            // var foo =|
            SyntaxMatcher.IsTailMatch<VariableDeclarationSyntax, Token>(matchingNodes, (variable, token) => variable.Assignment == token && token.Type == TokenType.Assignment && offset == token.GetEndPosition()) ||
            // var foo = a|
            SyntaxMatcher.IsTailMatch<VariableDeclarationSyntax, VariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (_, _, _, token) => token.Type == TokenType.Identifier);

        private static bool IsResourceBodyContext(List<SyntaxBase> matchingNodes, int offset) =>
            // resources only allow {} as the body so we don't need to worry about
            // providing completions for a partially-typed identifier
            SyntaxMatcher.IsTailMatch<ResourceDeclarationSyntax>(matchingNodes, resource =>
                    !resource.Name.Span.ContainsInclusive(offset) &&
                    !resource.Type.Span.ContainsInclusive(offset) &&
                    !resource.Assignment.Span.ContainsInclusive(offset) &&
                    resource.Value is SkippedTriviaSyntax && offset == resource.Value.Span.Position) ||
            // cursor is after the = token
            SyntaxMatcher.IsTailMatch<ResourceDeclarationSyntax, Token>(matchingNodes, (_, token) => token.Type == TokenType.Assignment && offset == token.GetEndPosition()) ||
            // [for x in y: |]
            SyntaxMatcher.IsTailMatch<ResourceDeclarationSyntax, ForSyntax, SkippedTriviaSyntax>(matchingNodes, (resource, @for, skipped) => resource.Value == @for && @for.Body == skipped) ||
            // [for x in y: | ];
            SyntaxMatcher.IsTailMatch<ResourceDeclarationSyntax, ForSyntax>(matchingNodes, (resource, @for) => resource.Value == @for) ||
            // [for x in y:|]
            SyntaxMatcher.IsTailMatch<ResourceDeclarationSyntax, ForSyntax, Token>(matchingNodes, (resource, @for, token) => resource.Value == @for && @for.Colon == token && token.Type == TokenType.Colon && offset == token.Span.GetEndPosition());

        private static bool IsModuleBodyContext(List<SyntaxBase> matchingNodes, int offset) =>
            // modules only allow {} as the body so we don't need to worry about
            // providing completions for a partially-typed identifier
            SyntaxMatcher.IsTailMatch<ModuleDeclarationSyntax>(matchingNodes, module =>
                !module.Name.Span.ContainsInclusive(offset) &&
                !module.Path.Span.ContainsInclusive(offset) &&
                !module.Assignment.Span.ContainsInclusive(offset) &&
                module.Value is SkippedTriviaSyntax && offset == module.Value.Span.Position) ||
            // cursor is after the = token
            SyntaxMatcher.IsTailMatch<ModuleDeclarationSyntax, Token>(matchingNodes, (_, token) => token.Type == TokenType.Assignment && offset == token.GetEndPosition()) ||
            // [for x in y: |]
            SyntaxMatcher.IsTailMatch<ModuleDeclarationSyntax, ForSyntax, SkippedTriviaSyntax>(matchingNodes, (module, @for, skipped) => module.Value == @for && @for.Body == skipped) ||
            // [for x in y: | ];
            SyntaxMatcher.IsTailMatch<ModuleDeclarationSyntax, ForSyntax>(matchingNodes, (module, @for) => module.Value == @for) ||
            // [for x in y:|]
            SyntaxMatcher.IsTailMatch<ModuleDeclarationSyntax, ForSyntax, Token>(matchingNodes, (module, @for, token) => module.Value == @for && @for.Colon == token && token.Type == TokenType.Colon && offset == token.Span.GetEndPosition());

        private static bool IsDecoratorNameContext(List<SyntaxBase> matchingNodes, int offset) =>
            SyntaxMatcher.IsTailMatch<DecoratorSyntax, VariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (_, _, _, token) => token.Type == TokenType.Identifier) ||
            SyntaxMatcher.IsTailMatch<DecoratorSyntax, Token>(matchingNodes, (_, token) => token.Type == TokenType.At) ||
            SyntaxMatcher.IsTailMatch<DecoratorSyntax>(matchingNodes, decoratorSyntax => offset > decoratorSyntax.At.Span.Position) ||
            SyntaxMatcher.IsTailMatch<DecoratorSyntax, PropertyAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (_, _, _, token) => token.Type == TokenType.Identifier) ||
            SyntaxMatcher.IsTailMatch<DecoratorSyntax, PropertyAccessSyntax, Token>(matchingNodes, (_, _, token) => token.Type == TokenType.Dot) ||
            SyntaxMatcher.IsTailMatch<DecoratorSyntax, PropertyAccessSyntax>(matchingNodes, (_, propertyAccessSyntax) => offset > propertyAccessSyntax.Dot.Span.Position);

        private static bool IsImportSymbolFollower(List<SyntaxBase> matchingNodes, int offset) =>
            // import foo |
            SyntaxMatcher.IsTailMatch<ImportDeclarationSyntax>(matchingNodes, import => import.AliasName.IsValid && offset > import.AliasName.GetEndPosition() && offset <= import.FromKeyword.GetEndPosition()) ||
            // import foo f|
            SyntaxMatcher.IsTailMatch<ImportDeclarationSyntax, SkippedTriviaSyntax, Token>(matchingNodes, (import, skipped, _) => import.FromKeyword == skipped);

        private static bool IsImportFromFollower(List<SyntaxBase> matchingNodes, int offset) =>
            // import foo from |
            SyntaxMatcher.IsTailMatch<ImportDeclarationSyntax>(matchingNodes, import => offset > import.FromKeyword.GetEndPosition() && import.ProviderName.Child is SkippedTriviaSyntax) ||
            // import foo from f|
            SyntaxMatcher.IsTailMatch<ImportDeclarationSyntax, IdentifierSyntax, Token>(matchingNodes, (import, ident, _) => import.ProviderName == ident);

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
        private static bool IsInnerExpressionContext(List<SyntaxBase> matchingNodes, int offset)
        {
            if (!matchingNodes.OfType<ExpressionSyntax>().Any())
            {
                // Fail fast.
                return false;
            }

            var isBooleanOrNumberOrNull = SyntaxMatcher.IsTailMatch<Token>(matchingNodes, token => token.Type switch
            {
                TokenType.TrueKeyword => true,
                TokenType.FalseKeyword => true,
                TokenType.Integer => true,
                TokenType.NullKeyword => true,
                _ => false,
            });

            if (isBooleanOrNumberOrNull)
            {
                // Don't provide completions for a boolean, number, or null literal because it may be confusing.
                return false;
            }

            var isInStringSegment = SyntaxMatcher.IsTailMatch<StringSyntax, Token>(matchingNodes, (_, token) => token.Type switch
            {
                // The cursor is immediately after the { character: '...${|...}...'.
                TokenType.StringLeftPiece when IsOffsetImmediatlyAfterNode(offset, token)=> false,
                TokenType.StringMiddlePiece when IsOffsetImmediatlyAfterNode(offset, token) => false,
                // In other cases, we are in a string segment.
                TokenType.StringComplete => true,
                TokenType.StringLeftPiece => true,
                TokenType.StringMiddlePiece => true,
                TokenType.StringRightPiece => true,
                TokenType.MultilineString => true,
                _ => false,
            });

            if (isInStringSegment)
            {
                return false;
            }

            // It does not make sense to insert expressions at the cursor positions shown in the comments below.
            return  !(
                //║{              ║{             ║|{|           ║{            ║{
                //║  foo: true |  ║ | foo: true  ║  foo: true   ║  foo: true  ║  |
                //║}              ║}             ║}             ║|}|          ║}
                SyntaxMatcher.IsTailMatch<ObjectSyntax>(matchingNodes) ||
                SyntaxMatcher.IsTailMatch<ObjectSyntax, Token>(matchingNodes) ||

                //║[         ║[        ║|[|      ║[
                //║  true |  ║ | true  ║  true   ║  true
                //║]         ║]        ║]        ║|]|
                SyntaxMatcher.IsTailMatch<ArraySyntax>(matchingNodes) ||
                SyntaxMatcher.IsTailMatch<ArraySyntax, Token>(
                    matchingNodes,
                    (arraySyntax, token) => token.Type != TokenType.NewLine || !CanInsertChildNodeAtOffset(arraySyntax, offset)) ||

                // var foo = ! | bar
                SyntaxMatcher.IsTailMatch<UnaryOperationSyntax>(
                    matchingNodes,
                    unaryOperation => !unaryOperation.Expression.IsOverlapping(offset)) ||

                // var foo = |!bar
                // var foo = !| bar
                SyntaxMatcher.IsTailMatch<UnaryOperationSyntax, Token>(
                    matchingNodes,
                    (unaryOperation, operatorToken) =>
                        operatorToken.GetPosition() == offset ||
                        (operatorToken.GetEndPosition() == offset && unaryOperation.Expression is not SkippedTriviaSyntax)) ||

                // var foo = 1 | + ...
                // var foo = 1 + | 2
                SyntaxMatcher.IsTailMatch<BinaryOperationSyntax>(
                    matchingNodes,
                    binaryOperation =>
                        !binaryOperation.LeftExpression.IsOverlapping(offset) &&
                        !binaryOperation.RightExpression.IsOverlapping(offset)) ||

                // var foo = 1 |+ ...
                // var foo = 1 +| 2
                SyntaxMatcher.IsTailMatch<BinaryOperationSyntax, Token>(
                    matchingNodes,
                    (binaryOperation, operatorToken) =>
                        (operatorToken.GetPosition() == offset && binaryOperation.LeftExpression is not SkippedTriviaSyntax) ||
                        (operatorToken.GetEndPosition() == offset && binaryOperation.RightExpression is not SkippedTriviaSyntax)) ||

                // var foo = true | ? ...
                // var foo = true ? | 'yes'
                // var foo = true ? 'yes' | : ...
                // var foo = true ? 'yes' : | 'no'
                SyntaxMatcher.IsTailMatch<TernaryOperationSyntax>(
                    matchingNodes,
                    ternaryOperation =>
                        !ternaryOperation.ConditionExpression.IsOverlapping(offset) &&
                        !ternaryOperation.TrueExpression.IsOverlapping(offset) &&
                        !ternaryOperation.FalseExpression.IsOverlapping(offset)) ||

                // var foo = true |? ...
                // var foo = true ?| 'yes'
                // var foo = true ? 'yes' |: ...
                // var foo = true ? 'yes' :| 'no'
                SyntaxMatcher.IsTailMatch<TernaryOperationSyntax, Token>(
                    matchingNodes,
                    (ternaryOperation, operatorToken) =>
                        (operatorToken.Type == TokenType.Question && operatorToken.GetPosition() == offset && ternaryOperation.ConditionExpression is not SkippedTriviaSyntax) ||
                        (operatorToken.Type == TokenType.Question && operatorToken.GetEndPosition() == offset && ternaryOperation.TrueExpression is not SkippedTriviaSyntax) ||
                        (operatorToken.Type == TokenType.Colon && operatorToken.GetPosition() == offset && ternaryOperation.TrueExpression is not SkippedTriviaSyntax) ||
                        (operatorToken.Type == TokenType.Colon && operatorToken.GetEndPosition() == offset && ternaryOperation.FalseExpression is not SkippedTriviaSyntax)));
        }
        
        static bool IsOffsetImmediatlyAfterNode(int offset, SyntaxBase node) => node.Span.Position + node.Span.Length == offset;

        private static Range GetReplacementRange(BicepFile bicepFile, SyntaxBase innermostMatchingNode, int offset)
        {
            if (innermostMatchingNode is Token token && ReplaceableTokens.Contains(token.Type))
            {
                // the token is replaceable - replace it
                return token.Span.ToRange(bicepFile.LineStarts);
            }

            // the innermost matching node is either a non-token or it's not replaceable
            // (non-replaceable tokens include colons, newlines, parens, etc.)
            // produce an insertion edit
            return new TextSpan(offset, 0).ToRange(bicepFile.LineStarts);
        }

        private static bool CanInsertChildNodeAtOffset(ProgramSyntax programSyntax, int offset)
        {
            var enclosingNode = programSyntax.Children.FirstOrDefault(child => child.IsEnclosing(offset));

            if (enclosingNode is Token { Type: TokenType.NewLine })
            {
                // /r/n|/r/n
                return true;
            }

            var lastNodeBeforeOffset = programSyntax.Children.LastOrDefault(node => node.GetEndPosition() <= offset);
            var firstNodeAfterOffset = programSyntax.Children.FirstOrDefault(node => node.GetPosition() >= offset);

            // Ensure we are in between newlines.
            return lastNodeBeforeOffset is null or Token { Type: TokenType.NewLine } &&
                firstNodeAfterOffset is null or Token { Type: TokenType.NewLine };
        }

        private static bool CanInsertChildNodeAtOffset(ObjectSyntax objectSyntax, int offset)
        {
            var enclosingNode = objectSyntax.Children.FirstOrDefault(child => child.IsEnclosing(offset));

            if (enclosingNode is Token { Type: TokenType.NewLine })
            {
                // /r/n|/r/n
                return true;
            }

            var nodes = objectSyntax.OpenBrace.AsEnumerable().Concat(objectSyntax.Children).Concat(objectSyntax.CloseBrace);
            var lastNodeBeforeOffset = nodes.LastOrDefault(node => node.GetEndPosition() <= offset);
            var firstNodeAfterOffset = nodes.FirstOrDefault(node => node.GetPosition() >= offset);

            // To insert a new child in an object, we must be in between newlines.
            // This will not be the case once https://github.com/Azure/bicep/issues/146 is implemented.
            return lastNodeBeforeOffset is Token { Type: TokenType.NewLine } &&
                firstNodeAfterOffset is Token { Type: TokenType.NewLine };
        }

        private static bool CanInsertChildNodeAtOffset(ArraySyntax arraySyntax, int offset)
        {
            var enclosingNode = arraySyntax.Children.FirstOrDefault(child => child.IsEnclosing(offset));

            if (enclosingNode is Token { Type: TokenType.NewLine })
            {
                // /r/n|/r/n
                return true;
            }

            var nodes = arraySyntax.OpenBracket.AsEnumerable().Concat(arraySyntax.Children).Concat(arraySyntax.CloseBracket);
            var lastNodeBeforeOffset = nodes.LastOrDefault(node => node.GetEndPosition() <= offset);
            var firstNodeAfterOffset = nodes.FirstOrDefault(node => node.GetPosition() >= offset);

            // To insert a new child in an array, we must be in between newlines.
            // This will not be the case once https://github.com/Azure/bicep/issues/146 is implemented.
            return lastNodeBeforeOffset is Token { Type: TokenType.NewLine } &&
                firstNodeAfterOffset is Token { Type: TokenType.NewLine };
        }

        private class ActiveScopesVisitor : SymbolVisitor
        {
            private readonly int offset;

            private ActiveScopesVisitor(int offset)
            {
                this.offset = offset;
            }

            public override void VisitFileSymbol(FileSymbol symbol)
            {
                // global scope is always active
                this.ActiveScopes.Add(symbol);

                base.VisitFileSymbol(symbol);
            }

            public override void VisitLocalScope(LocalScope symbol)
            {
                // use binding syntax because this is used to find accessible symbols
                // in a child scope
                if (symbol.BindingSyntax.Span.Contains(this.offset))
                {
                    // the offset is inside the binding scope
                    // this scope is active
                    this.ActiveScopes.Add(symbol);

                    // visit children to find more active scopes within
                    base.VisitLocalScope(symbol);
                }
            }

            private IList<ILanguageScope> ActiveScopes { get; } = new List<ILanguageScope>();

            public static ImmutableArray<ILanguageScope> GetActiveScopes(FileSymbol file, int offset)
            {
                var visitor = new ActiveScopesVisitor(offset);
                visitor.Visit(file);

                return visitor.ActiveScopes.ToImmutableArray();
            }
        }
    }
}
