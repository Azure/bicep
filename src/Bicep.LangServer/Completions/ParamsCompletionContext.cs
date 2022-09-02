// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Extensions;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Completions
{
    public class ParamsCompletionContext
    {
        // completions will replace only these token types
        // all others will result in an insertion upon completion commit
        private static readonly ImmutableHashSet<TokenType> ReplaceableTokens = new[]
        {
            TokenType.Identifier,
            TokenType.Integer,
            TokenType.StringComplete
        }.Concat(LanguageConstants.Keywords.Values).ToImmutableHashSet();

        public ParamsCompletionContextKind Kind { get; }

        public Range ReplacementRange { get; }

        public SyntaxBase? UsingDeclaration { get; }

        public string? ParamAssignment { get; }

        public ParamsCompletionContext(
            ParamsCompletionContextKind kind,
            Range replacementRange,
            SyntaxBase? usingDeclaration = null,
            string? paramAssignment = null)
        {
            Kind = kind;
            ReplacementRange = replacementRange;
            UsingDeclaration = usingDeclaration;
            ParamAssignment = paramAssignment;
        }

        public static ParamsCompletionContext Create(ParamsCompilationContext paramsCompilationContext, int offset)
        {
            var paramsFile = paramsCompilationContext.ParamsSemanticModel.BicepParamFile;
            var matchingNodes = SyntaxMatcher.FindNodesMatchingOffset(paramsFile.ProgramSyntax, offset);
            if (!matchingNodes.Any())
            {
                // this indicates a bug
                throw new ArgumentException($"The specified offset {offset} is outside the span of the specified {nameof(ProgramSyntax)} node.");
            }
            var replacementRange = GetReplacementRange(paramsFile, matchingNodes[^1], offset);


            var kind = ConvertFlag(IsUsingDeclarationContext(matchingNodes, offset), ParamsCompletionContextKind.UsingFilePath) |
                       ConvertFlag(IsParamIdentifierContext(matchingNodes, offset), ParamsCompletionContextKind.ParamIdentifier) |
                       ConvertFlag(IsParamValueContext(matchingNodes, offset), ParamsCompletionContextKind.ParamValue);

            if (kind == ParamsCompletionContextKind.UsingFilePath)
            {
                var usingDeclarationInfo = SyntaxMatcher.FindLastNodeOfType<UsingDeclarationSyntax, UsingDeclarationSyntax>(matchingNodes);
                return new(kind, replacementRange, usingDeclarationInfo.node);
            }

            if (kind == ParamsCompletionContextKind.ParamValue)
            {
                var paramAssignmentInfo = SyntaxMatcher.FindLastNodeOfType<ParameterAssignmentSyntax, ParameterAssignmentSyntax>(matchingNodes);
                return new(kind, replacementRange, null, paramAssignmentInfo.node?.Name.IdentifierName);
            }

            return new(kind, replacementRange);
        }

        private static bool IsUsingDeclarationContext(List<SyntaxBase> matchingNodes, int offset) =>
            // using |
            SyntaxMatcher.IsTailMatch<UsingDeclarationSyntax>(matchingNodes) ||
            // using '|'        
            // using 'f|oo'
            SyntaxMatcher.IsTailMatch<UsingDeclarationSyntax, StringSyntax, Token>(matchingNodes, (_, _, token) => token.Type == TokenType.StringComplete) ||
            // using fo|o
            SyntaxMatcher.IsTailMatch<UsingDeclarationSyntax, SkippedTriviaSyntax, Token>(matchingNodes, (_, _, token) => token.Type == TokenType.Identifier);

        private static bool IsParamIdentifierContext(List<SyntaxBase> matchingNodes, int offset) =>
            // param |
            SyntaxMatcher.IsTailMatch<ParameterAssignmentSyntax>(matchingNodes, (paramAssignment => paramAssignment.Name.IdentifierName == LanguageConstants.MissingName)) ||
            // param | = 
            SyntaxMatcher.IsTailMatch<ParameterAssignmentSyntax>(matchingNodes, (paramAssignment => paramAssignment.Name.IdentifierName == LanguageConstants.ErrorName)) ||
            // param t|
            SyntaxMatcher.IsTailMatch<ParameterAssignmentSyntax, IdentifierSyntax, Token>(matchingNodes, (_, _, token) => token.Type == TokenType.Identifier);

        private static bool IsParamValueContext(List<SyntaxBase> matchingNodes, int offset) =>
            // param myParam = | 
            SyntaxMatcher.IsTailMatch<ParameterAssignmentSyntax>(matchingNodes, (paramAssignment => paramAssignment.Name.IdentifierName != LanguageConstants.MissingName || paramAssignment.Name.IdentifierName != LanguageConstants.ErrorName)) ||
            // param myParam = 't|'
            SyntaxMatcher.IsTailMatch<ParameterAssignmentSyntax, StringSyntax, Token>(matchingNodes, (paramAssignment, _, _) => paramAssignment.Name.IdentifierName != LanguageConstants.MissingName || paramAssignment.Name.IdentifierName != LanguageConstants.ErrorName);

        private static ParamsCompletionContextKind ConvertFlag(bool value, ParamsCompletionContextKind flag) => value ? flag : ParamsCompletionContextKind.None;

        private static Range GetReplacementRange(BicepParamFile paramsFile, SyntaxBase innermostMatchingNode, int offset)
        {
            if (innermostMatchingNode is Token token && ReplaceableTokens.Contains(token.Type))
            {
                // the token is replaceable - replace it
                return token.Span.ToRange(paramsFile.LineStarts);
            }

            // the innermost matching node is either a non-token or it's not replaceable
            // (non-replaceable tokens include colons, newlines, parens, etc.)
            // produce an insertion edit
            return new TextSpan(offset, 0).ToRange(paramsFile.LineStarts);
        }
    }
}
