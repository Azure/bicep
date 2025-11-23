// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.LanguageServer.Completions.SyntaxPatterns;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Utils;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Completions
{
    public class BicepCompletionContext
    {
        public record IndexedSyntaxContext<T>(T Syntax, int ArgumentIndex) where T : SyntaxBase?;

        private static readonly CompositeSyntaxPattern ExpectingExtensionSpecification = CompositeSyntaxPattern.Create(
            cursor: '|',
            "extension |",
            "extension |kuber",
            "extension kuber|");

        private static readonly CompositeSyntaxPattern ExpectingExtensionWithOrAsKeyword = CompositeSyntaxPattern.Create(
            cursor: '|',
            "extension kubernetes |",
            "extension kubernetes a|",
            "extension kubernetes |b");

        private static readonly CompositeSyntaxPattern ExpectingExtensionConfig = CompositeSyntaxPattern.Create(
            cursor: '|',
            "extension kubernetes with |",
            "extension kubernetes with | as foo");

        private static readonly SyntaxPattern ExpectingExtensionAsKeyword = SyntaxPattern.Create(
            cursor: '|',
            "extension kubernetes with { foo: true } |");

        // completions will replace only these token types
        // all others will result in an insertion upon completion commit
        private static readonly ImmutableHashSet<TokenType> ReplaceableTokens =
        [
            TokenType.Identifier,
            TokenType.Integer,
            TokenType.StringComplete,
            .. LanguageConstants.NonContextualKeywords.Values,
        ];

        private BicepCompletionContext(
            BicepSourceFile sourceFile,
            BicepCompletionContextKind kind,
            Range replacementRange,
            SyntaxBase replacementTarget,
            SyntaxBase? enclosingDeclaration,
            DecorableSyntax? enclosingDecorable,
            ObjectSyntax? @object,
            ObjectPropertySyntax? property,
            IndexedSyntaxContext<ArraySyntax?>? array,
            PropertyAccessSyntax? propertyAccess,
            TypePropertyAccessSyntax? typePropertyAccess,
            ResourceAccessSyntax? resourceAccess,
            ArrayAccessSyntax? arrayAccess,
            TypeArrayAccessSyntax? typeArrayAccess,
            TargetScopeSyntax? targetScope,
            IndexedSyntaxContext<FunctionCallSyntaxBase>? functionArgument,
            IndexedSyntaxContext<ParameterizedTypeInstantiationSyntaxBase>? typeArgument,
            ImmutableArray<ILanguageScope> activeScopes)
        {
            this.SourceFile = sourceFile;
            this.Kind = kind;
            this.ReplacementRange = replacementRange;
            this.ReplacementTarget = replacementTarget;
            this.EnclosingDeclaration = enclosingDeclaration;
            this.EnclosingDecorable = enclosingDecorable;
            this.Object = @object;
            this.Property = property;
            this.Array = array;
            this.PropertyAccess = propertyAccess;
            this.TypePropertyAccess = typePropertyAccess;
            this.ResourceAccess = resourceAccess;
            this.ArrayAccess = arrayAccess;
            this.TypeArrayAccess = typeArrayAccess;
            this.TargetScope = targetScope;
            this.FunctionArgument = functionArgument;
            this.TypeArgument = typeArgument;
            this.ActiveScopes = activeScopes;
        }

        public BicepSourceFile SourceFile { get; }

        public BicepCompletionContextKind Kind { get; }

        public SyntaxBase? EnclosingDeclaration { get; }

        public SyntaxBase? EnclosingDecorable { get; }

        public ObjectSyntax? Object { get; }

        public ObjectPropertySyntax? Property { get; }

        public IndexedSyntaxContext<ArraySyntax?>? Array { get; }

        public PropertyAccessSyntax? PropertyAccess { get; }

        public TypePropertyAccessSyntax? TypePropertyAccess { get; }

        public ResourceAccessSyntax? ResourceAccess { get; }

        public ArrayAccessSyntax? ArrayAccess { get; }

        public TypeArrayAccessSyntax? TypeArrayAccess { get; }

        public TargetScopeSyntax? TargetScope { get; }

        public IndexedSyntaxContext<FunctionCallSyntaxBase>? FunctionArgument { get; }

        public IndexedSyntaxContext<ParameterizedTypeInstantiationSyntaxBase>? TypeArgument { get; }

        public ImmutableArray<ILanguageScope> ActiveScopes { get; }

        public Range ReplacementRange { get; }

        public SyntaxBase ReplacementTarget { get; }

        public RootConfiguration Configuration => this.SourceFile.Configuration;

        public IFeatureProvider Features => this.SourceFile.Features;

        public static BicepCompletionContext Create(Compilation compilation, int offset)
        {
            var bicepFile = compilation.SourceFileGrouping.EntryPoint;
            var matchingNodes = SyntaxMatcher.FindNodesMatchingOffset(bicepFile.ProgramSyntax, offset);
            if (!matchingNodes.Any())
            {
                // this indicates a bug
                throw new ArgumentException($"The specified offset {offset} is outside the span of the specified {nameof(ProgramSyntax)} node.");
            }

            // the check at the beginning guarantees we have at least 1 node
            var replacementTarget = matchingNodes[^1];
            var replacementRange = GetReplacementRange(bicepFile, replacementTarget, offset);

            var triviaMatchingOffset = FindTriviaMatchingOffset(bicepFile.ProgramSyntax, offset);
            switch (triviaMatchingOffset?.Type)
            {
                case SyntaxTriviaType.Whitespace:
                    var position = triviaMatchingOffset.Span.Position;
                    if (position > 0)
                    {
                        var previousTrivia = FindTriviaMatchingOffset(bicepFile.ProgramSyntax, position - 1);

                        if (previousTrivia is DisableNextLineDiagnosticsSyntaxTrivia)
                        {
                            return new BicepCompletionContext(bicepFile, BicepCompletionContextKind.DisableNextLineDiagnosticsCodes, replacementRange, replacementTarget, null, null, null, null, null, null, null, null, null, null, null, null, null, []);
                        }
                    }
                    break;
                case SyntaxTriviaType.DisableNextLineDiagnosticsDirective:
                    // This will handle the following case: #disable-next-line |
                    if (triviaMatchingOffset.Text.EndsWith(' '))
                    {
                        return new BicepCompletionContext(bicepFile, BicepCompletionContextKind.DisableNextLineDiagnosticsCodes, replacementRange, replacementTarget, null, null, null, null, null, null, null, null, null, null, null, null, null, []);
                    }
                    return new BicepCompletionContext(bicepFile, BicepCompletionContextKind.None, replacementRange, replacementTarget, null, null, null, null, null, null, null, null, null, null, null, null, null, []);
                case SyntaxTriviaType.SingleLineComment when offset > triviaMatchingOffset.Span.Position:
                case SyntaxTriviaType.MultiLineComment when offset > triviaMatchingOffset.Span.Position && offset < triviaMatchingOffset.Span.Position + triviaMatchingOffset.Span.Length:
                    // we're in a comment, no hints here
                    return new BicepCompletionContext(bicepFile, BicepCompletionContextKind.None, replacementRange, replacementTarget, null, null, null, null, null, null, null, null, null, null, null, null, null, []);
            }

            if (IsDisableNextLineDiagnosticsDirectiveStartContext(bicepFile, offset, matchingNodes))
            {
                return new BicepCompletionContext(bicepFile, BicepCompletionContextKind.DisableNextLineDiagnosticsDirectiveStart, replacementRange, replacementTarget, null, null, null, null, null, null, null, null, null, null, null, null, null, []);
            }

            var pattern = SyntaxPattern.Create(bicepFile.ProgramSyntax, offset);
            var topLevelDeclarationInfo = SyntaxMatcher.FindLastNodeOfType<ITopLevelDeclarationSyntax, SyntaxBase>(matchingNodes);
            var enclosingDecorable = SyntaxMatcher.FindLastNodeOfType<DecorableSyntax, DecorableSyntax>(matchingNodes);
            var objectInfo = SyntaxMatcher.FindLastNodeOfType<ObjectSyntax, ObjectSyntax>(matchingNodes);
            var propertyInfo = SyntaxMatcher.FindLastNodeOfType<ObjectPropertySyntax, ObjectPropertySyntax>(matchingNodes);
            var propertyKey = propertyInfo.node?.TryGetKeyText();
            var arrayInfo = SyntaxMatcher.FindLastNodeOfType<ArraySyntax, ArraySyntax>(matchingNodes);
            var propertyAccessInfo = SyntaxMatcher.FindLastNodeOfType<PropertyAccessSyntax, PropertyAccessSyntax>(matchingNodes);
            var typePropertyAccessInfo = SyntaxMatcher.FindLastNodeOfType<TypePropertyAccessSyntax, TypePropertyAccessSyntax>(matchingNodes);
            var resourceAccessInfo = SyntaxMatcher.FindLastNodeOfType<ResourceAccessSyntax, ResourceAccessSyntax>(matchingNodes);
            var arrayAccessInfo = SyntaxMatcher.FindLastNodeOfType<ArrayAccessSyntax, ArrayAccessSyntax>(matchingNodes);
            var typeArrayAccessInfo = SyntaxMatcher.FindLastNodeOfType<TypeArrayAccessSyntax, TypeArrayAccessSyntax>(matchingNodes);
            var targetScopeInfo = SyntaxMatcher.FindLastNodeOfType<TargetScopeSyntax, TargetScopeSyntax>(matchingNodes);
            var activeScopes = ActiveScopesVisitor.GetActiveScopes(compilation.GetEntrypointSemanticModel().Root, offset);
            var functionArgumentContext = TryGetFunctionArgumentContext(matchingNodes, offset);
            var typeArgumentContext = TryGetTypeArgumentContext(matchingNodes, offset);

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
                ConvertFlag(IsTestBodyContext(matchingNodes, offset), BicepCompletionContextKind.TestBody) |
                ConvertFlag(IsParameterDefaultValueContext(matchingNodes, offset), BicepCompletionContextKind.ParameterDefaultValue | BicepCompletionContextKind.Expression) |
                ConvertFlag(IsVariableValueContext(matchingNodes, offset), BicepCompletionContextKind.VariableValue | BicepCompletionContextKind.Expression) |
                ConvertFlag(IsOutputValueContext(matchingNodes, offset), BicepCompletionContextKind.OutputValue | BicepCompletionContextKind.Expression) |
                ConvertFlag(IsOutputTypeFollowerContext(matchingNodes, offset), BicepCompletionContextKind.OutputTypeFollower) |
                ConvertFlag(IsOuterExpressionContext(matchingNodes, offset), BicepCompletionContextKind.Expression) |
                ConvertFlag(IsTargetScopeContext(matchingNodes, offset), BicepCompletionContextKind.TargetScope) |
                ConvertFlag(IsDecoratorNameContext(matchingNodes, offset), BicepCompletionContextKind.DecoratorName) |
                ConvertFlag(functionArgumentContext is not null, BicepCompletionContextKind.FunctionArgument | BicepCompletionContextKind.Expression) |
                ConvertFlag(IsUsingPathContext(matchingNodes, offset), BicepCompletionContextKind.UsingFilePath) |
                ConvertFlag(IsExtendsPathContext(matchingNodes, offset), BicepCompletionContextKind.ExtendsFilePath) |
                ConvertFlag(IsTestPathContext(matchingNodes, offset), BicepCompletionContextKind.TestPath) |
                ConvertFlag(IsModulePathContext(matchingNodes, offset), BicepCompletionContextKind.ModulePath) |
                ConvertFlag(IsImportPathContext(matchingNodes, offset), BicepCompletionContextKind.ModulePath) |
                ConvertFlag(IsParameterIdentifierContext(matchingNodes, offset), BicepCompletionContextKind.ParamIdentifier) |
                ConvertFlag(IsParameterValueContext(matchingNodes, offset), BicepCompletionContextKind.ParamValue) |
                ConvertFlag(IsObjectTypePropertyValueContext(matchingNodes, offset), BicepCompletionContextKind.ObjectTypePropertyValue) |
                ConvertFlag(IsUnionTypeMemberContext(matchingNodes, offset), BicepCompletionContextKind.UnionTypeMember) |
                ConvertFlag(IsTypedLocalVariableTypeContext(matchingNodes, offset), BicepCompletionContextKind.TypedLocalVariableType) |
                ConvertFlag(IsTypedLambdaOutputTypeContext(matchingNodes, offset), BicepCompletionContextKind.TypedLambdaOutputType) |
                ConvertFlag(typeArgumentContext is not null, BicepCompletionContextKind.TypeArgument) |
                ConvertFlag(IsTypeMemberAccessContext(matchingNodes, typePropertyAccessInfo, offset), BicepCompletionContextKind.TypeMemberAccess) |
                ConvertFlag(IsImportIdentifierContext(matchingNodes, offset), BicepCompletionContextKind.ImportIdentifier) |
                ConvertFlag(IsImportedSymbolListItemContext(matchingNodes, offset), BicepCompletionContextKind.ImportedSymbolIdentifier) |
                ConvertFlag(ExpectingContextualAsKeyword(matchingNodes, offset), BicepCompletionContextKind.ExpectingExtensionAsKeyword) |
                ConvertFlag(ExpectingContextualFromKeyword(matchingNodes, offset), BicepCompletionContextKind.ExpectingImportFromKeyword) |
                ConvertFlag(IsAfterSpreadTokenContext(matchingNodes, offset), BicepCompletionContextKind.Expression) |
                ConvertFlag(IsVariableNameFollowerContext(matchingNodes, offset), BicepCompletionContextKind.VariableNameFollower) |
                ConvertFlag(ExpectingExtensionSpecification.TailMatch(pattern), BicepCompletionContextKind.ExpectingExtensionSpecification) |
                ConvertFlag(ExpectingExtensionWithOrAsKeyword.TailMatch(pattern), BicepCompletionContextKind.ExpectingExtensionWithOrAsKeyword) |
                ConvertFlag(ExpectingExtensionConfig.TailMatch(pattern), BicepCompletionContextKind.ExpectingExtensionConfig) |
                ConvertFlag(ExpectingExtensionAsKeyword.TailMatch(pattern), BicepCompletionContextKind.ExpectingExtensionAsKeyword) |
                ConvertFlag(IsUsingFollowerContext(matchingNodes, offset), BicepCompletionContextKind.UsingFollower) |
                ConvertFlag(IsUsingWithFollowerContext(matchingNodes, offset), BicepCompletionContextKind.UsingWithFollower);

            if (bicepFile.Features.AssertsEnabled)
            {
                kind |= ConvertFlag(IsAssertValueContext(matchingNodes, offset), BicepCompletionContextKind.AssertValue | BicepCompletionContextKind.Expression);
            }

            if (kind == BicepCompletionContextKind.None)
            {
                // previous processing hasn't identified a completion context kind
                // check if we're inside an expression
                kind |= ConvertFlag(IsInnerExpressionContext(matchingNodes, offset), BicepCompletionContextKind.Expression);

                if (kind.HasFlag(BicepCompletionContextKind.Expression))
                {
                    if (PropertyTypeShouldFlowThrough(matchingNodes, propertyInfo, offset))
                    {
                        kind |= BicepCompletionContextKind.PropertyValue;
                    }

                    var arrayItemInfo = SyntaxMatcher.FindLastNodeOfType<ArrayItemSyntax, ArrayItemSyntax>(matchingNodes);
                    if (ArrayItemTypeShouldFlowThrough(matchingNodes, arrayItemInfo, arrayInfo.node))
                    {
                        kind |= BicepCompletionContextKind.ArrayItem;
                    }
                }
            }

            kind |= ConvertFlag(IsResourceDependsOnArrayItemContext(kind, propertyKey, topLevelDeclarationInfo.node), BicepCompletionContextKind.ExpectsResourceSymbolicReference);

            return new BicepCompletionContext(
                bicepFile,
                kind,
                replacementRange,
                replacementTarget,
                topLevelDeclarationInfo.node,
                enclosingDecorable.node,
                objectInfo.node,
                propertyInfo.node,
                new(arrayInfo.node, arrayInfo.index),
                propertyAccessInfo.node,
                typePropertyAccessInfo.node,
                resourceAccessInfo.node,
                arrayAccessInfo.node,
                typeArrayAccessInfo.node,
                targetScopeInfo.node,
                functionArgumentContext,
                typeArgumentContext,
                activeScopes);
        }

        private static bool IsDisableNextLineDiagnosticsDirectiveStartContext(BicepSourceFile bicepFile, int offset, List<SyntaxBase> matchingNodes)
        {
            return matchingNodes[^1] is Token token &&
                token.Text == "#" &&
                token.Span.GetEndPosition() == offset &&
                ShouldAllowCompletionAfterPoundSign(bicepFile, token);
        }

        private static bool ShouldAllowCompletionAfterPoundSign(BicepSourceFile bicepFile, Token token)
        {
            var lineStarts = bicepFile.LineStarts;
            var position = token.GetPosition();
            (var line, _) = TextCoordinateConverter.GetPosition(lineStarts, position);
            var lineStart = lineStarts[line];

            if (position == lineStart)
            {
                return true;
            }

            var triviaMatchingLineStart = FindTriviaMatchingOffset(bicepFile.ProgramSyntax, lineStart);

            if (triviaMatchingLineStart?.Type == SyntaxTriviaType.Whitespace &&
                triviaMatchingLineStart.GetEndPosition() == position)
            {
                return true;
            }

            return false;
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

            bool CheckParameterResourceTypeIsExpected(ResourceTypeSyntax type) =>
                // This handles the case where we have the resource keyword but no type-string for a parameter.
                // Must be inside the type
                type.Span.Length > 0 &&
                offset >= type.Keyword.GetEndPosition() &&
                (type.Type is null || type.Type is SkippedTriviaSyntax);

            bool CheckOutputResourceTypeIsExpected(OutputDeclarationSyntax output) =>
                // This handles the case where we have the resource keyword but no type-string for an output.
                // Must be after the type (`resource` is valid for type) and
                // Before the `=` if `=` is present
                output.Type is ResourceTypeSyntax type &&
                offset >= type.Keyword.GetEndPosition() &&
                (type.Type is null || type.Type is SkippedTriviaSyntax) &&
                (output.Assignment is SkippedTriviaSyntax || (output.Assignment is Token assignment && offset <= assignment.GetPosition()));

            if (SyntaxMatcher.IsTailMatch<ParameterDeclarationSyntax>(matchingNodes, parameter => CheckTypeIsExpected(parameter.Name, parameter.Type)) ||
                SyntaxMatcher.IsTailMatch<ParameterDeclarationSyntax, TypeVariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (_, _, _, token) => token.Type == TokenType.Identifier))
            {
                // the most specific matching node is a parameter declaration
                // the declaration syntax is "param <identifier> <type> ..."
                // the cursor position is on the type if we have an identifier (non-zero length span) and the offset matches the type position
                // OR
                // we are in a token that is inside a TypeVariableAccessSyntax node, which is inside a parameter node
                return BicepCompletionContextKind.ParameterType;
            }

            if (SyntaxMatcher.IsTailMatch<TypeDeclarationSyntax>(matchingNodes, typeDeclaration => typeDeclaration.Assignment is not SkippedTriviaSyntax && offset > typeDeclaration.Assignment.GetEndPosition() && offset <= typeDeclaration.Value.Span.Position) ||
                SyntaxMatcher.IsTailMatch<TypeDeclarationSyntax, TypeVariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (_, _, _, token) => token.Type == TokenType.Identifier))
            {
                // the most specific matching node is a type declaration
                // the declaration syntax is "type <identifier> = <type>"
                // the cursor position is on the type if we have an identifier (non-zero length span) and the offset matches the type position
                // OR
                // we are in a token that is inside a TypeVariableAccessSyntax node, which is inside a type declaration node
                return BicepCompletionContextKind.TypeDeclarationValue;
            }

            if (SyntaxMatcher.IsTailMatch<ParameterDeclarationSyntax, ResourceTypeSyntax>(matchingNodes, (parameter, type) => CheckParameterResourceTypeIsExpected(type)) ||
                SyntaxMatcher.IsTailMatch<ParameterDeclarationSyntax, ResourceTypeSyntax, StringSyntax, Token>(matchingNodes, (_, _, _, token) => token.Type == TokenType.StringComplete))
            {
                // the most specific matching node is a parameter declaration with the resource keyword
                // the declaration syntax is "param <identifier> resource ..."
                // the cursor position is on the resource type if we have the resource keyword but nothing else
                // OR
                // we are in a token that is inside a ResourceTypeSyntax node, which is inside a parameter node
                return BicepCompletionContextKind.ResourceType;
            }

            if (SyntaxMatcher.IsTailMatch<OutputDeclarationSyntax>(matchingNodes, output => CheckTypeIsExpected(output.Name, output.Type)) ||
                SyntaxMatcher.IsTailMatch<OutputDeclarationSyntax, TypeVariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (_, _, _, token) => token.Type == TokenType.Identifier))
            {
                // the most specific matching node is an output declaration
                // the declaration syntax is "output <identifier> <type> ..."
                // the cursor position is on the type if we have an identifier (non-zero length span) and the offset matches the type position
                // OR
                // we are in a token that is inside a TypeVariableAccessSyntax node, which is inside an output node
                return BicepCompletionContextKind.OutputType;
            }

            // NOTE: this logic is different between parameters and outputs because the resource type is optional for outputs.
            if (SyntaxMatcher.IsTailMatch<OutputDeclarationSyntax>(matchingNodes, output => CheckOutputResourceTypeIsExpected(output)) ||
                SyntaxMatcher.IsTailMatch<OutputDeclarationSyntax, ResourceTypeSyntax, StringSyntax, Token>(matchingNodes, (_, _, _, token) => token.Type == TokenType.StringComplete))
            {
                // the most specific matching node is an output declaration with the resource keyword
                // the declaration syntax is "output <identifier> resource ..."
                // the cursor position is on the resource type if we have the resource keyword but nothing else
                // OR
                // we are in a token that is inside a ResourceTypeSyntax node, which is inside an output node
                return BicepCompletionContextKind.ResourceType;
            }

            if (IsResourceTypeContext(matchingNodes, offset))
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
            SyntaxMatcher.IsTailMatch<ResourceDeclarationSyntax, Token>(matchingNodes, (resource, token) => IsAtTokenStart(resource.Assignment, token, offset) && token.Type == TokenType.Assignment);

        private static bool IsVariableNameFollowerContext(List<SyntaxBase> matchingNodes, int offset) =>
            // var foo |
            SyntaxMatcher.IsTailMatch<VariableDeclarationSyntax>(matchingNodes, variable =>
                offset > variable.Name.GetEndPosition() &&
                variable.Type is null &&
                variable.Assignment is SkippedTriviaSyntax &&
                offset <= variable.Assignment.Span.Position) ||
            // var foo |
            // OR
            // var foo | = ...
            SyntaxMatcher.IsTailMatch<VariableDeclarationSyntax>(matchingNodes, variable => offset > (variable.Type ?? variable.Name).GetEndPosition() && offset <= variable.Assignment.Span.Position) ||
            // var foo e|
            // OR
            // var foo e| = ...
            SyntaxMatcher.IsTailMatch<VariableDeclarationSyntax, TypeVariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (_, _, _, token) => token.Type == TokenType.Identifier) ||
            // var foo |=
            SyntaxMatcher.IsTailMatch<VariableDeclarationSyntax, Token>(matchingNodes, (resource, token) => IsAtTokenStart(resource.Assignment, token, offset) && token.Type == TokenType.Assignment);

        private static bool IsTargetScopeContext(List<SyntaxBase> matchingNodes, int offset) =>
            SyntaxMatcher.IsTailMatch<TargetScopeSyntax>(matchingNodes, targetScope =>
                !targetScope.Assignment.Span.ContainsInclusive(offset) &&
                targetScope.Value is SkippedTriviaSyntax && offset == targetScope.Value.Span.Position) ||
            SyntaxMatcher.IsTailMatch<TargetScopeSyntax, Token>(matchingNodes, (targetScope, token) =>
                token.Type == TokenType.Assignment &&
                ReferenceEquals(targetScope.Assignment, token));

        private static bool IsUsingFollowerContext(List<SyntaxBase> matchingNodes, int offset) =>
            // using 'main.bicep' |
            SyntaxMatcher.IsTailMatch<UsingDeclarationSyntax>(matchingNodes, syntax =>
                offset > syntax.Path.GetEndPosition() &&
                syntax.WithClause is SkippedTriviaSyntax &&
                offset <= syntax.WithClause.Span.Position);

        private static bool IsUsingWithFollowerContext(List<SyntaxBase> matchingNodes, int offset) =>
            // using 'main.bicep' with |
            SyntaxMatcher.IsTailMatch<UsingWithClauseSyntax>(matchingNodes, syntax =>
                offset > syntax.Keyword.GetEndPosition() &&
                syntax.Config is SkippedTriviaSyntax &&
                offset <= syntax.Config.Span.Position);

        private static bool IsTopLevelDeclarationStartContext(List<SyntaxBase> matchingNodes, int offset)
        {
            if (matchingNodes.Count == 1 && matchingNodes[0] is ProgramSyntax)
            {
                // the file is empty and the AST has a ProgramSyntax with 0 children and an EOF
                // because we picked the left node as winner, the only matching node is the ProgramSyntax node
                return true;
            }

            if (matchingNodes.Count >= 2 && matchingNodes[^1] is Token token)
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
                        (propertyAccess, token) => token.Type == TokenType.Question && ReferenceEquals(propertyAccess.SafeAccessMarker, token)) ||
                    SyntaxMatcher.IsTailMatch<PropertyAccessSyntax, Token>(
                        matchingNodes,
                        (propertyAccess, token) => token.Type == TokenType.Dot && ReferenceEquals(propertyAccess.Dot, token)) ||
                    SyntaxMatcher.IsTailMatch<PropertyAccessSyntax>(
                        matchingNodes,
                        propertyAccess => offset > propertyAccess.Dot.Span.Position));
        }

        private static bool IsTypeMemberAccessContext(List<SyntaxBase> matchingNodes, (TypePropertyAccessSyntax? node, int index) propertyAccessInfo, int offset)
        {
            return propertyAccessInfo.node != null &&
                   (SyntaxMatcher.IsTailMatch<TypePropertyAccessSyntax, IdentifierSyntax, Token>(
                        matchingNodes,
                        (propertyAccess, identifier, token) => ReferenceEquals(propertyAccess.PropertyName, identifier) && token.Type == TokenType.Identifier) ||
                    SyntaxMatcher.IsTailMatch<TypePropertyAccessSyntax, Token>(
                        matchingNodes,
                        (propertyAccess, token) => token.Type == TokenType.Dot && ReferenceEquals(propertyAccess.Dot, token)) ||
                    SyntaxMatcher.IsTailMatch<TypePropertyAccessSyntax>(
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
                    (objectSyntax, token) => (token == objectSyntax.OpenBrace || token.Type == TokenType.NewLine || token.Type == TokenType.Comma) && CanInsertChildNodeAtOffset(objectSyntax, offset)) ||

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
                    SyntaxMatcher.IsTailMatch<ObjectPropertySyntax, Token>(matchingNodes, (_, token) => token.Type == TokenType.Colon && offset >= token.GetEndPosition()) ||
                    // the cursor is inside a string value of the property that is not interpolated
                    SyntaxMatcher.IsTailMatch<ObjectPropertySyntax, StringSyntax, Token>(matchingNodes, (property, stringSyntax, token) => ReferenceEquals(property.Value, stringSyntax) && !stringSyntax.Expressions.Any()) ||
                    // the cursor could is a partial or full identifier
                    // which will present as either a keyword or identifier token
                    SyntaxMatcher.IsTailMatch<ObjectPropertySyntax, VariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (property, variableAccess, identifier, token) => ReferenceEquals(property.Value, variableAccess)))
                {
                    return BicepCompletionContextKind.PropertyValue | BicepCompletionContextKind.Expression;
                }
                ;

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

        private static bool PropertyTypeShouldFlowThrough(List<SyntaxBase> matchingNodes, (ObjectPropertySyntax? node, int index) propertyInfo, int offset)
        {
            if (propertyInfo.node is null)
            {
                return false;
            }

            // Property types should flow through parenthesized and ternary expressions. For examples:
            // {
            //   key: (( | ))
            // }
            // {
            //   key: conditionA ? | : false
            // }
            // {
            //   key: conditionA ? (conditionB ? true : |) : false
            // }
            if ((SyntaxMatcher.IsTailMatch<TernaryOperationSyntax>(matchingNodes) ||
                SyntaxMatcher.IsTailMatch<TernaryOperationSyntax, Token>(matchingNodes) ||
                SyntaxMatcher.IsTailMatch<ParenthesizedExpressionSyntax>(matchingNodes) ||
                SyntaxMatcher.IsTailMatch<ParenthesizedExpressionSyntax, Token>(matchingNodes) ||
                SyntaxMatcher.IsTailMatch<ParenthesizedExpressionSyntax, SkippedTriviaSyntax>(matchingNodes, (parenthesizedExpression, _) =>
                    parenthesizedExpression.Expression is SkippedTriviaSyntax)) &&
                matchingNodes.Skip(propertyInfo.index + 1).SkipLast(1).All(node => node is TernaryOperationSyntax or ParenthesizedExpressionSyntax))
            {
                return true;
            }

            return false;
        }

        private static bool ArrayItemTypeShouldFlowThrough(
            List<SyntaxBase> matchingNodes,
            (ArrayItemSyntax? node, int index) arrayItemInfo,
            ArraySyntax? arraySyntax
        )
        {
            if (arraySyntax is null)
            {
                return false;
            }

            // Array item types should flow through parenthesized and ternary expressions. For examples:
            // [(|)]
            // [(...), (conditionA ? |)]
            // [
            //   ( | )
            // ]
            // [
            //   conditionA ? | : false
            // ]
            // [
            //   conditionA ? (conditionB ? true : |) : false
            // ]
            if (arrayItemInfo.node != null
                && matchingNodes.Skip(arrayItemInfo.index + 1).SkipLast(1).All(node => node is TernaryOperationSyntax or ParenthesizedExpressionSyntax)
                && matchingNodes.Last() is TernaryOperationSyntax or ParenthesizedExpressionSyntax or Token)
            {
                return true;
            }

            var arrayHasNewLines = arraySyntax.Children.Any(c => c is Token { Type: TokenType.NewLine });
            // something like [, (|)] ..
            return !arrayHasNewLines && SyntaxMatcher.IsTailMatch<ArraySyntax, SkippedTriviaSyntax, Token>(matchingNodes);
        }

        private static bool IsArrayItemContext(List<SyntaxBase> matchingNodes, (ArraySyntax? node, int index) arrayInfo, int offset)
        {
            if (arrayInfo.node == null)
            {
                // none of the nodes are arrays
                // so we can't possibly be in an array item context
                return false;
            }

            // var arr = [ | ]
            if (SyntaxMatcher.IsTailMatch<ArraySyntax>(matchingNodes)
                // var arr = [|] var arr = [ |]
                || SyntaxMatcher.IsTailMatch<ArraySyntax, Token>(
                    matchingNodes,
                    (_, token) => token is { Type: TokenType.NewLine or TokenType.Comma or TokenType.LeftSquare or TokenType.RightSquare }
                )
                || SyntaxMatcher.IsTailMatch<ArraySyntax, SkippedTriviaSyntax>(matchingNodes) // var arr = [, | ,]
                || SyntaxMatcher.IsTailMatch<ArraySyntax, SkippedTriviaSyntax, Token>( // var arr = [,|]
                    matchingNodes,
                    (_, _, token) => token is { Type: TokenType.Comma }
                ))
            {
                return CanInsertChildNodeAtOffset(arrayInfo.node, offset);
            }

            // var arr = [a|]
            return SyntaxMatcher.IsTailMatch<ArraySyntax, ArrayItemSyntax, VariableAccessSyntax, IdentifierSyntax, Token>(
                matchingNodes,
                (_, _, _, _, token) => token is { Type: TokenType.Identifier }
            );
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
            SyntaxMatcher.IsTailMatch<OutputDeclarationSyntax, VariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (output, _, _, token) => output.Assignment is Token assignmentToken && offset > assignmentToken.GetEndPosition() && token.Type == TokenType.Identifier);

        private static bool IsOutputTypeFollowerContext(List<SyntaxBase> matchingNodes, int offset) =>
            // | below indicates cursor position
            // output foo type |
            SyntaxMatcher.IsTailMatch<OutputDeclarationSyntax>(matchingNodes, output => offset > output.Type.GetEndPosition() && offset <= output.Assignment.Span.Position);

        private static bool IsAfterSpreadTokenContext(List<SyntaxBase> matchingNodes, int offset) =>
            // ... |
            SyntaxMatcher.IsTailMatch<SpreadExpressionSyntax>(matchingNodes, spread => spread.Expression is not SkippedTriviaSyntax && offset >= spread.Expression.GetEndPosition()) ||
            // ...|
            SyntaxMatcher.IsTailMatch<SpreadExpressionSyntax, Token>(matchingNodes, (spread, token) => spread.Ellipsis == token && offset == token.GetEndPosition());


        private static bool IsVariableValueContext(List<SyntaxBase> matchingNodes, int offset) =>
            // | below indicates cursor position
            // var foo = |
            SyntaxMatcher.IsTailMatch<VariableDeclarationSyntax>(matchingNodes, variable => variable.Assignment is not SkippedTriviaSyntax && offset >= variable.Assignment.GetEndPosition()) ||
            // var foo =|
            SyntaxMatcher.IsTailMatch<VariableDeclarationSyntax, Token>(matchingNodes, (variable, token) => variable.Assignment == token && token.Type == TokenType.Assignment && offset == token.GetEndPosition()) ||
            // var foo = a|
            SyntaxMatcher.IsTailMatch<VariableDeclarationSyntax, VariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (_, _, _, token) => token.Type == TokenType.Identifier);

        private static bool IsAssertValueContext(List<SyntaxBase> matchingNodes, int offset) =>
            // | below indicates cursor position
            // assert foo = |
            SyntaxMatcher.IsTailMatch<AssertDeclarationSyntax>(matchingNodes, assert => assert.Assignment is not SkippedTriviaSyntax && offset >= assert.Assignment.GetEndPosition()) ||
            // assert foo =|
            SyntaxMatcher.IsTailMatch<AssertDeclarationSyntax, Token>(matchingNodes, (assert, token) => assert.Assignment == token && token.Type == TokenType.Assignment && offset == token.GetEndPosition()) ||
            // assert foo = a|
            SyntaxMatcher.IsTailMatch<AssertDeclarationSyntax, VariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (assert, _, _, token) => token.Type == TokenType.Identifier && assert.Assignment is Token assignmentToken && offset > assignmentToken.GetEndPosition());

        private static bool IsImportIdentifierContext(List<SyntaxBase> matchingNodes, int offset) =>
            // import |
            SyntaxMatcher.IsTailMatch<CompileTimeImportDeclarationSyntax>(matchingNodes, declaration => declaration.ImportExpression is SkippedTriviaSyntax &&
                declaration.ImportExpression.Span.ContainsInclusive(offset) &&
                declaration.FromClause is SkippedTriviaSyntax &&
                declaration.FromClause.Span.Length == 0);

        private static bool IsImportedSymbolListItemContext(List<SyntaxBase> matchingNodes, int offset) =>
            SyntaxMatcher.IsTailMatch<ImportedSymbolsListItemSyntax, IdentifierSyntax, Token>(matchingNodes, (_, _, token) => token.Type == TokenType.Identifier) ||
            SyntaxMatcher.IsTailMatch<ImportedSymbolsListSyntax, Token>(matchingNodes) ||
            SyntaxMatcher.IsTailMatch<ImportedSymbolsListSyntax>(matchingNodes, list => IsBetweenNodes(offset, list.OpenBrace, list.CloseBrace));

        private static bool ExpectingContextualAsKeyword(List<SyntaxBase> matchingNodes, int offset) =>
            // import {} | or import * |
            SyntaxMatcher.IsTailMatch<CompileTimeImportDeclarationSyntax>(matchingNodes, statement => statement.ImportExpression is SkippedTriviaSyntax importExpressionTrivia &&
                statement.FromClause.Span.Length == 0 &&
                importExpressionTrivia.Elements.Length == 1 &&
                importExpressionTrivia.Elements[0] is Token importToken &&
                importToken.Type == TokenType.Asterisk);

        private static bool ExpectingContextualFromKeyword(List<SyntaxBase> matchingNodes, int offset) =>
            // import {} | or import * as foo |
            SyntaxMatcher.IsTailMatch<CompileTimeImportDeclarationSyntax>(matchingNodes, statement => statement.ImportExpression is not SkippedTriviaSyntax &&
                statement.FromClause is SkippedTriviaSyntax &&
                statement.FromClause.Span.ContainsInclusive(offset));

        private static bool IsBetweenNodes(int offset, IPositionable first, IPositionable second)
            => first.Span.Length > 0 && first.IsBefore(offset) && second.IsOnOrAfter(offset);

        private static bool IsAtTokenStart(SyntaxBase syntax, Token token, int offset) =>
            syntax == token && offset == token.Span.Position;

        private static bool IsAtTokenEnd(SyntaxBase syntax, Token token, int offset) =>
            syntax == token && offset == token.Span.GetEndPosition();

        private static bool IsImportPathContext(List<SyntaxBase> matchingNodes, int offset) =>
            // import {} from |
            SyntaxMatcher.IsTailMatch<CompileTimeImportFromClauseSyntax>(matchingNodes, (fromClause) => IsBetweenNodes(offset, fromClause.Keyword, fromClause.Path)) ||
            // import {} from 'f|oo'
            SyntaxMatcher.IsTailMatch<CompileTimeImportFromClauseSyntax, StringSyntax, Token>(matchingNodes, (fromClause, @string, _) => fromClause.Path == @string) ||
            // import {} from fo|o
            SyntaxMatcher.IsTailMatch<CompileTimeImportFromClauseSyntax, SkippedTriviaSyntax, Token>(matchingNodes, (fromClause, skipped, _) => fromClause.Path == skipped);

        private static bool IsModulePathContext(IList<SyntaxBase> matchingNodes, int offset) =>
            // module foo | =
            SyntaxMatcher.IsTailMatch<ModuleDeclarationSyntax>(matchingNodes, module => IsBetweenNodes(offset, module.Name, module.Path)) ||
            // module foo 'f|oo'
            SyntaxMatcher.IsTailMatch<ModuleDeclarationSyntax, StringSyntax, Token>(matchingNodes, (module, @string, _) => module.Path == @string) ||
            // module foo fo|o
            SyntaxMatcher.IsTailMatch<ModuleDeclarationSyntax, SkippedTriviaSyntax, Token>(matchingNodes, (module, skipped, _) => module.Path == skipped);

        private static bool IsResourceTypeContext(IList<SyntaxBase> matchingNodes, int offset) =>
            // resource foo | =
            SyntaxMatcher.IsTailMatch<ResourceDeclarationSyntax>(matchingNodes, resource => IsBetweenNodes(offset, resource.Name, resource.Type)) ||
            // resource foo 'f|oo'
            SyntaxMatcher.IsTailMatch<ResourceDeclarationSyntax, StringSyntax, Token>(matchingNodes, (resource, @string, _) => resource.Type == @string) ||
            // resource foo fo|o
            SyntaxMatcher.IsTailMatch<ResourceDeclarationSyntax, SkippedTriviaSyntax, Token>(matchingNodes, (resource, skipped, _) => resource.Type == skipped);

        private static bool IsTestPathContext(IList<SyntaxBase> matchingNodes, int offset) =>
            // test foo | =
            SyntaxMatcher.IsTailMatch<TestDeclarationSyntax>(matchingNodes, test => IsBetweenNodes(offset, test.Name, test.Path)) ||
            // test foo 'f|oo'
            SyntaxMatcher.IsTailMatch<TestDeclarationSyntax, StringSyntax, Token>(matchingNodes, (test, @string, _) => test.Path == @string) ||
            // test foo fo|o
            SyntaxMatcher.IsTailMatch<TestDeclarationSyntax, SkippedTriviaSyntax, Token>(matchingNodes, (test, skipped, _) => test.Path == skipped);

        private static bool IsUsingPathContext(IList<SyntaxBase> matchingNodes, int offset) =>
            // using |
            SyntaxMatcher.IsTailMatch<UsingDeclarationSyntax>(matchingNodes, usingClause => usingClause.Keyword.IsBefore(offset)) ||
            // using 'f|oo'
            SyntaxMatcher.IsTailMatch<UsingDeclarationSyntax, StringSyntax, Token>(matchingNodes, (@using, @string, _) => @using.Path == @string) ||
            // using fo|o
            SyntaxMatcher.IsTailMatch<UsingDeclarationSyntax, SkippedTriviaSyntax, Token>(matchingNodes, (@using, skipped, _) => @using.Path == skipped);

        private static bool IsExtendsPathContext(IList<SyntaxBase> matchingNodes, int offset) =>
            // extends |
            SyntaxMatcher.IsTailMatch<ExtendsDeclarationSyntax>(matchingNodes, extendsClause => extendsClause.Keyword.IsBefore(offset)) ||
            // extends 'f|oo'
            SyntaxMatcher.IsTailMatch<ExtendsDeclarationSyntax, StringSyntax, Token>(matchingNodes, (@extends, @string, _) => @extends.Path == @string) ||
            // extends fo|o
            SyntaxMatcher.IsTailMatch<ExtendsDeclarationSyntax, SkippedTriviaSyntax, Token>(matchingNodes, (@extends, skipped, _) => @extends.Path == skipped);

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
            SyntaxMatcher.IsTailMatch<ResourceDeclarationSyntax, ForSyntax, Token>(matchingNodes, (resource, @for, token) => resource.Value == @for && IsAtTokenEnd(@for.Colon, token, offset) && token.Type == TokenType.Colon);

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
            SyntaxMatcher.IsTailMatch<ModuleDeclarationSyntax, ForSyntax, Token>(matchingNodes, (module, @for, token) => module.Value == @for && IsAtTokenEnd(@for.Colon, token, offset) && token.Type == TokenType.Colon);

        private static bool IsTestBodyContext(List<SyntaxBase> matchingNodes, int offset) =>
            // tests only allow {} as the body so we don't need to worry about
            // providing completions for a partially-typed identifier
            SyntaxMatcher.IsTailMatch<TestDeclarationSyntax>(matchingNodes, test =>
                !test.Path.Span.ContainsInclusive(offset) &&
                !test.Assignment.Span.ContainsInclusive(offset) &&
                test.Value is SkippedTriviaSyntax && offset == test.Value.Span.Position) ||
            // cursor is after the = token
            SyntaxMatcher.IsTailMatch<TestDeclarationSyntax, Token>(matchingNodes, (_, token) => token.Type == TokenType.Assignment && offset == token.GetEndPosition());

        private static bool IsDecoratorNameContext(List<SyntaxBase> matchingNodes, int offset) =>
            SyntaxMatcher.IsTailMatch<DecoratorSyntax, VariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (_, _, _, token) => token.Type == TokenType.Identifier) ||
            SyntaxMatcher.IsTailMatch<DecoratorSyntax, Token>(matchingNodes, (_, token) => token.Type == TokenType.At) ||
            SyntaxMatcher.IsTailMatch<DecoratorSyntax>(matchingNodes, decoratorSyntax => offset > decoratorSyntax.At.Span.Position) ||
            SyntaxMatcher.IsTailMatch<DecoratorSyntax, PropertyAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (_, _, _, token) => token.Type == TokenType.Identifier) ||
            SyntaxMatcher.IsTailMatch<DecoratorSyntax, PropertyAccessSyntax, Token>(matchingNodes, (_, _, token) => token.Type == TokenType.Dot) ||
            SyntaxMatcher.IsTailMatch<DecoratorSyntax, PropertyAccessSyntax>(matchingNodes, (_, propertyAccessSyntax) => offset > propertyAccessSyntax.Dot.Span.Position);

        private static bool IsObjectTypePropertyValueContext(List<SyntaxBase> matchingNodes, int offset) =>
            SyntaxMatcher.IsTailMatch<ObjectTypePropertySyntax>(matchingNodes, typePropertySyntax => typePropertySyntax.Colon is not SkippedTriviaSyntax && offset > typePropertySyntax.Colon.Span.Position) ||
            SyntaxMatcher.IsTailMatch<ObjectTypePropertySyntax, TypeVariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (_, _, _, token) => token.Type == TokenType.Identifier);

        private static bool IsUnionTypeMemberContext(List<SyntaxBase> matchingNodes, int offset) =>
            SyntaxMatcher.IsTailMatch<UnionTypeSyntax, Token>(matchingNodes, (_, token) => token.Type == TokenType.Pipe) ||
            SyntaxMatcher.IsTailMatch<UnionTypeSyntax>(matchingNodes, union => union.Children.LastOrDefault() is SkippedTriviaSyntax) ||
            SyntaxMatcher.IsTailMatch<UnionTypeSyntax, UnionTypeMemberSyntax, TypeVariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (_, _, _, _, token) => token.Type == TokenType.Identifier);

        private static IndexedSyntaxContext<FunctionCallSyntaxBase>? TryGetFunctionArgumentContext(List<SyntaxBase> matchingNodes, int offset)
        {
            // someFunc(|)
            // abc.someFunc(|)
            if (SyntaxMatcher.IsTailMatch<FunctionCallSyntaxBase, Token>(matchingNodes, (func, token) => token == func.OpenParen))
            {
                return new(Syntax: (FunctionCallSyntaxBase)matchingNodes[^2], ArgumentIndex: 0);
            }

            // someFunc(x, |)
            // abc.someFunc(x, |)
            if (SyntaxMatcher.IsTailMatch<FunctionCallSyntaxBase, FunctionArgumentSyntax>(matchingNodes, (func, _) => true))
            {
                var function = (FunctionCallSyntaxBase)matchingNodes[^2];

                return new(Syntax: function, ArgumentIndex: function.Arguments.IndexOf((FunctionArgumentSyntax)matchingNodes[^1]));
            }

            // someFunc(x,|)
            // abc.someFunc(x,|)
            if (SyntaxMatcher.IsTailMatch<FunctionCallSyntaxBase, Token>(matchingNodes, (func, _) => true))
            {
                var function = (FunctionCallSyntaxBase)matchingNodes[^2];
                var previousArg = function.Arguments.LastOrDefault(x => x.Span.Position < offset);

                return new(Syntax: function, ArgumentIndex: previousArg is null ? 0 : (function.Arguments.IndexOf(previousArg) + 1));
            }

            // someFunc(x, 'a|bc')
            // abc.someFunc(x, 'de|f')
            if (SyntaxMatcher.IsTailMatch<FunctionCallSyntaxBase, FunctionArgumentSyntax, StringSyntax, Token>(matchingNodes, (_, _, _, token) => token.Type == TokenType.StringComplete))
            {
                var function = (FunctionCallSyntaxBase)matchingNodes[^4];

                return new(Syntax: function, ArgumentIndex: function.Arguments.IndexOf((FunctionArgumentSyntax)matchingNodes[^3]));
            }

            // someFunc(x, ab|c)
            // abc.someFunc(x, de|f)
            if (SyntaxMatcher.IsTailMatch<FunctionCallSyntaxBase, FunctionArgumentSyntax, VariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (_, _, _, _, token) => token.Type == TokenType.Identifier))
            {
                var function = (FunctionCallSyntaxBase)matchingNodes[^5];

                return new(Syntax: function, ArgumentIndex: function.Arguments.IndexOf((FunctionArgumentSyntax)matchingNodes[^4]));
            }

            return null;
        }

        private static IndexedSyntaxContext<ParameterizedTypeInstantiationSyntaxBase>? TryGetTypeArgumentContext(List<SyntaxBase> matchingNodes, int offset)
        {
            // someType<|>
            // abc.someType(|)
            if (SyntaxMatcher.IsTailMatch<ParameterizedTypeInstantiationSyntaxBase, Token>(matchingNodes, (instantiation, token) => token == instantiation.OpenChevron))
            {
                return new(Syntax: (ParameterizedTypeInstantiationSyntaxBase)matchingNodes[^2], ArgumentIndex: 0);
            }

            // someType<x, |>
            // abc.someType<x, |>
            if (SyntaxMatcher.IsTailMatch<ParameterizedTypeInstantiationSyntaxBase, ParameterizedTypeArgumentSyntax>(matchingNodes, (instantiation, _) => true))
            {
                var instantiation = (ParameterizedTypeInstantiationSyntaxBase)matchingNodes[^2];
                var args = instantiation.Arguments;

                return new(Syntax: instantiation, ArgumentIndex: args.IndexOf((ParameterizedTypeArgumentSyntax)matchingNodes[^1]));
            }

            // someType<x,|>
            // abc.someType<x,|>
            if (SyntaxMatcher.IsTailMatch<ParameterizedTypeInstantiationSyntaxBase, Token>(matchingNodes, (func, _) => true))
            {
                var instantiation = (ParameterizedTypeInstantiationSyntaxBase)matchingNodes[^2];
                var args = instantiation.Arguments;
                var previousArg = args.LastOrDefault(x => x.Span.Position < offset);

                return new(Syntax: instantiation, ArgumentIndex: previousArg is null ? 0 : (args.IndexOf(previousArg) + 1));
            }

            // someType<x, 'a|bc'>
            // abc.someType<x, 'de|f'>
            if (SyntaxMatcher.IsTailMatch<ParameterizedTypeInstantiationSyntaxBase, ParameterizedTypeArgumentSyntax, StringTypeLiteralSyntax, Token>(matchingNodes, (_, _, _, token) => token.Type == TokenType.StringComplete))
            {
                var instantiation = (ParameterizedTypeInstantiationSyntaxBase)matchingNodes[^4];
                var args = instantiation.Arguments;

                return new(Syntax: instantiation, ArgumentIndex: args.IndexOf((ParameterizedTypeArgumentSyntax)matchingNodes[^3]));
            }

            // someType<x, ab|c>
            // abc.someType<x, de|f>
            if (SyntaxMatcher.IsTailMatch<ParameterizedTypeInstantiationSyntaxBase, ParameterizedTypeArgumentSyntax, TypeVariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (_, _, _, _, token) => token.Type == TokenType.Identifier))
            {
                var instantiation = (ParameterizedTypeInstantiationSyntaxBase)matchingNodes[^5];
                var args = instantiation.Arguments;

                return new(Syntax: instantiation, ArgumentIndex: args.IndexOf((ParameterizedTypeArgumentSyntax)matchingNodes[^4]));
            }

            return null;
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
                           variable.Type?.Span.ContainsInclusive(offset) != true &&
                           !variable.Assignment.Span.ContainsInclusive(offset) &&
                           variable.Value is SkippedTriviaSyntax && offset == variable.Value.Span.Position;

                case ParameterAssignmentSyntax paramAssignment:
                    // is the cursor after the equals sign in the param assignment?
                    return !paramAssignment.Name.Span.ContainsInclusive(offset) &&
                           !paramAssignment.Assignment.Span.ContainsInclusive(offset) &&
                           paramAssignment.Value is SkippedTriviaSyntax && offset == paramAssignment.Value.Span.Position;

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

                        case ParameterAssignmentSyntax paramAssignment:
                            return ReferenceEquals(paramAssignment.Value, variableAccess);
                    }

                    break;

                case Token token when token.Type == TokenType.Assignment && matchingNodes.Count >= 2 && offset == token.GetEndPosition():
                    // cursor is after the = token
                    // check if parent is of the right type
                    var parent = matchingNodes[^2];
                    return parent is ParameterDefaultValueSyntax ||
                           parent is VariableDeclarationSyntax ||
                           parent is OutputDeclarationSyntax ||
                           parent is ParameterDeclarationSyntax ||
                           parent is ParameterAssignmentSyntax;
            }

            return false;
        }

        /// <summary>
        /// Determines if we are inside an expression. Will not produce a correct result if context kind is set is already set to something.
        /// </summary>
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
                TokenType.StringLeftPiece when IsOffsetImmediatlyAfterNode(offset, token) => false,
                TokenType.StringMiddlePiece when IsOffsetImmediatlyAfterNode(offset, token) => false,
                // In other cases, we are in a string segment.
                TokenType.StringComplete => true,
                TokenType.StringLeftPiece => true,
                TokenType.StringMiddlePiece => true,
                TokenType.StringRightPiece => true,
                _ => false,
            });

            if (isInStringSegment)
            {
                return false;
            }

            // var foo = true ?|:
            // var foo = true ?| :
            // var foo = true ? |:
            // var foo = true ? | :
            var isInEmptyTrueExpression = SyntaxMatcher.IsTailMatch<TernaryOperationSyntax>(
                matchingNodes,
                ternaryOperation =>
                    ternaryOperation.Question.GetPosition() <= offset &&
                    ternaryOperation.Colon.GetPosition() >= offset &&
                    ternaryOperation.TrueExpression is SkippedTriviaSyntax);

            if (isInEmptyTrueExpression)
            {
                return true;
            }

            // var foo = true ? : | <white spaces>
            var isInEmptyFalseExpression = SyntaxMatcher.IsTailMatch<TernaryOperationSyntax>(
                matchingNodes,
                ternaryOperation =>
                    ternaryOperation.Colon.GetPosition() <= offset &&
                    ternaryOperation.FalseExpression.GetPosition() >= offset &&
                    ternaryOperation.FalseExpression is SkippedTriviaSyntax);

            if (isInEmptyFalseExpression)
            {
                return true;
            }

            // It does not make sense to insert expressions at the cursor positions shown in the comments below.
            return !(
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

                // var test = map([], ( | ) => 'asdf')
                SyntaxMatcher.IsTailMatch<VariableBlockSyntax>(matchingNodes) ||
                // var test = map([], (a|) => 'asdf')
                SyntaxMatcher.IsTailMatch<VariableBlockSyntax, LocalVariableSyntax, IdentifierSyntax, Token>(matchingNodes) ||
                // var test = map([], (|) => 'asdf')
                SyntaxMatcher.IsTailMatch<VariableBlockSyntax, Token>(matchingNodes) ||

                // func foo( | ) string => 'asdf'
                SyntaxMatcher.IsTailMatch<TypedVariableBlockSyntax>(matchingNodes) ||
                // func foo(a|) string => 'asdf'
                SyntaxMatcher.IsTailMatch<TypedVariableBlockSyntax, TypedLocalVariableSyntax, IdentifierSyntax, Token>(matchingNodes) ||
                // func foo(|) string => 'asdf'
                SyntaxMatcher.IsTailMatch<TypedVariableBlockSyntax, Token>(matchingNodes) ||

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

        private static bool IsParameterIdentifierContext(List<SyntaxBase> matchingNodes, int offset) =>
            // param |
            SyntaxMatcher.IsTailMatch<ParameterAssignmentSyntax>(matchingNodes, (paramAssignment => paramAssignment.Name.IdentifierName == LanguageConstants.MissingName)) ||
            // param | =
            SyntaxMatcher.IsTailMatch<ParameterAssignmentSyntax>(matchingNodes, (paramAssignment => paramAssignment.Name.IdentifierName == LanguageConstants.ErrorName)) ||
            // param t|
            SyntaxMatcher.IsTailMatch<ParameterAssignmentSyntax, IdentifierSyntax, Token>(matchingNodes, (_, _, token) => token.Type == TokenType.Identifier);

        private static bool IsParameterValueContext(List<SyntaxBase> matchingNodes, int offset) =>
            // | below indicates cursor position
            // param foo = |
            SyntaxMatcher.IsTailMatch<ParameterAssignmentSyntax>(matchingNodes, variable => variable.Assignment is not SkippedTriviaSyntax && offset >= variable.Assignment.GetEndPosition()) ||
            // param foo =|
            SyntaxMatcher.IsTailMatch<ParameterAssignmentSyntax, Token>(matchingNodes, (variable, token) => variable.Assignment == token && token.Type == TokenType.Assignment && offset == token.GetEndPosition()) ||
            // param foo = a|
            SyntaxMatcher.IsTailMatch<ParameterAssignmentSyntax, VariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (_, _, _, token) => token.Type == TokenType.Identifier) ||
            // param foo = 'o|'
            SyntaxMatcher.IsTailMatch<ParameterAssignmentSyntax, StringSyntax, Token>(matchingNodes, (_, _, token) => token.Type == TokenType.StringComplete);

        private static bool IsTypedLambdaOutputTypeContext(List<SyntaxBase> matchingNodes, int offset) =>
            // func foo() | => bar
            SyntaxMatcher.IsTailMatch<TypedLambdaSyntax>(matchingNodes, (lambda) => offset > lambda.VariableSection.GetEndPosition() && (lambda.Arrow.IsSkipped || offset < lambda.Arrow.GetPosition())) ||
            // func foo() a| => bar
            SyntaxMatcher.IsTailMatch<TypedLambdaSyntax, TypeVariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (lambda, variable, _, _) => lambda.ReturnType == variable);

        private static bool IsTypedLocalVariableTypeContext(List<SyntaxBase> matchingNodes, int offset) =>
            // func foo(a |) string => bar
            SyntaxMatcher.IsTailMatch<TypedVariableBlockSyntax, TypedLocalVariableSyntax>(matchingNodes, (_, variable) => offset > variable.Name.GetEndPosition()) ||
            // func foo(a b|) string => bar
            SyntaxMatcher.IsTailMatch<TypedVariableBlockSyntax, TypedLocalVariableSyntax, TypeVariableAccessSyntax, IdentifierSyntax, Token>(matchingNodes, (_, variable, type, _, _) => variable.Type == type);

        private static bool IsResourceDependsOnArrayItemContext(BicepCompletionContextKind kind, string? propertyName, SyntaxBase? topLevelDeclarationInfo)
        {
            return propertyName == LanguageConstants.ResourceDependsOnPropertyName
                   && kind.HasFlag(BicepCompletionContextKind.ArrayItem)
                   && topLevelDeclarationInfo is ResourceDeclarationSyntax or ModuleDeclarationSyntax;
        }

        static bool IsOffsetImmediatlyAfterNode(int offset, SyntaxBase node) => node.Span.Position + node.Span.Length == offset;

        private static Range GetReplacementRange(BicepSourceFile bicepFile, SyntaxBase innermostMatchingNode, int offset)
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
                // \n|\n
                return true;
            }

            SyntaxBase[] nodes = [objectSyntax.OpenBrace, .. objectSyntax.Children, objectSyntax.CloseBrace];
            var nodeBefore = nodes.LastOrDefault(node => node.GetEndPosition() <= offset);
            var nodeAfter = nodes.FirstOrDefault(node => node.GetPosition() >= offset);

            return (nodeBefore, nodeAfter) switch
            {
                (Token { Type: TokenType.NewLine }, { } after) when after == objectSyntax.CloseBrace => true,
                (Token { Type: TokenType.Comma }, { } after) when after == objectSyntax.CloseBrace => true,
                ({ } before, { } after) when before == objectSyntax.OpenBrace && after == objectSyntax.CloseBrace => true,
                ({ } before, Token { Type: TokenType.NewLine }) when before == objectSyntax.OpenBrace => true,
                (Token { Type: TokenType.NewLine }, Token { Type: TokenType.NewLine }) => true,
                _ => false,
            };
        }

        private static bool CanInsertChildNodeAtOffset(ArraySyntax arraySyntax, int offset)
        {
            var enclosingNode = arraySyntax.Children.FirstOrDefault(child => child.IsEnclosing(offset));

            if (enclosingNode is Token { Type: TokenType.NewLine })
            {
                // /r/n|/r/n
                return true;
            }

            var nodes = arraySyntax.OpenBracket.AsEnumerable().Concat(arraySyntax.Children).Concat(arraySyntax.CloseBracket).ToList();
            var hasNewLines = nodes.Any(node => node is Token { Type: TokenType.NewLine });
            var lastNodeBeforeOffset = nodes.LastOrDefault(node => node.GetEndPosition() <= offset);
            var firstNodeAfterOffset = nodes.FirstOrDefault(node => node.GetPosition() >= offset);

            return lastNodeBeforeOffset switch
            {
                Token { Type: TokenType.NewLine } =>
                    // To insert a new child in a multiline array, we must be in between newlines.
                    firstNodeAfterOffset is Token { Type: TokenType.NewLine },
                _ when hasNewLines =>
                    false,
                Token { Type: TokenType.LeftSquare } =>
                    firstNodeAfterOffset is Token { Type: TokenType.RightSquare } or ArrayItemSyntax or SkippedTriviaSyntax,
                Token { Type: TokenType.Comma } or ArrayItemSyntax =>
                    true,
                SkippedTriviaSyntax =>
                    firstNodeAfterOffset is Token { Type: TokenType.RightSquare },
                _ => false
            };
        }
    }
}
