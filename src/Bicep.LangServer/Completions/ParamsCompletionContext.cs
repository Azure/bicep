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

        public ParamsCompletionContext(ParamsCompletionContextKind kind, Range replacementRange)
        {
            Kind = kind;
            ReplacementRange = replacementRange;
        }

        public static ParamsCompletionContext Create(ParamsCompilationContext paramsCompilationContext, int offset)
        {
            var paramsFile = paramsCompilationContext.ParamsSemanticModel.bicepParamFile;
            var matchingNodes = SyntaxMatcher.FindNodesMatchingOffset(paramsFile.ProgramSyntax, offset);
            if (!matchingNodes.Any())
            {
                // this indicates a bug
                throw new ArgumentException($"The specified offset {offset} is outside the span of the specified {nameof(ProgramSyntax)} node.");
            }
            var replacementRange = GetReplacementRange(paramsFile, matchingNodes[^1], offset);
            
            var topLevelDeclarationInfo = SyntaxMatcher.FindLastNodeOfType<ITopLevelNamedDeclarationSyntax, SyntaxBase>(matchingNodes);


            var kind = ConvertFlag(IsParamAssignmentContext(matchingNodes, offset), ParamsCompletionContextKind.ParamAssignment);

                    //    ConvertFlag(IsTopLevelDeclarationStartContext(matchingNodes, offset), BicepCompletionContextKind.TopLevelDeclarationStart) |

            return new(kind, replacementRange);
        }

        private static bool IsParamAssignmentContext(List<SyntaxBase> matchingNodes, int offset)
        {
            return matchingNodes.Count >=1 && matchingNodes[^1] is ParameterAssignmentSyntax;
        }

        private static ParamsCompletionContextKind ConvertFlag(bool value, ParamsCompletionContextKind flag) => value ? flag : ParamsCompletionContextKind.None;

        // private static bool IsTopLevelDeclarationStartContext(List<SyntaxBase> matchingNodes, int offset)
        // {
        //     if (matchingNodes.Count == 1 && matchingNodes[0] is ProgramSyntax)
        //     {
        //         // the file is empty and the AST has a ProgramSyntax with 0 children and an EOF
        //         // because we picked the left node as winner, the only matching node is the ProgramSyntax node
        //         return true;
        //     }

        //     if (matchingNodes.Count >= 2 && matchingNodes[^1] is Token token)
        //     {
        //         // we have at least 2 matching nodes in the "stack" and the last one is a token
        //         var node = matchingNodes[^2];

        //         switch (node)
        //         {
        //             case ProgramSyntax programSyntax:
        //                 // the token at current position is inside a program node
        //                 // we're in a declaration if one of the following conditions is met:
        //                 // 1. the token is EOF
        //                 // 2. the token is a newline and we can insert at the offset
        //                 return token.Type == TokenType.EndOfFile ||
        //                        (token.Type == TokenType.NewLine && CanInsertChildNodeAtOffset(programSyntax, offset));

        //             case SkippedTriviaSyntax _ when matchingNodes.Count >= 3:
        //                 // we are in a line that has a partial declaration keyword (for example "resour" or "modu")
        //                 // if the token at current position is an identifier, assume declaration context
        //                 // (completions will be filtered by the text that is present, so we don't have to be 100% right)
        //                 return token.Type == TokenType.Identifier && matchingNodes[^3] is ProgramSyntax;

        //             case ITopLevelNamedDeclarationSyntax declaration:
        //                 // we are in a partially parsed declaration which only contains a keyword
        //                 // whether we are in a declaration context depends on whether our offset is within the keyword token
        //                 // (by using exclusive span containment, the cursor position at the end of a keyword token
        //                 // result counts as being outside of the declaration context)
        //                 return declaration.Name.IdentifierName.Equals(LanguageConstants.MissingName, LanguageConstants.IdentifierComparison) &&
        //                     declaration.Keyword.Span.Contains(offset);
        //         }
        //     }

        //     return false;
        // }

        // private static bool CanInsertChildNodeAtOffset(ProgramSyntax programSyntax, int offset)
        // {
        //     var enclosingNode = programSyntax.Children.FirstOrDefault(child => child.IsEnclosing(offset));

        //     if (enclosingNode is Token { Type: TokenType.NewLine })
        //     {
        //         // /r/n|/r/n
        //         return true;
        //     }

        //     var lastNodeBeforeOffset = programSyntax.Children.LastOrDefault(node => node.GetEndPosition() <= offset);
        //     var firstNodeAfterOffset = programSyntax.Children.FirstOrDefault(node => node.GetPosition() >= offset);

        //     // Ensure we are in between newlines.
        //     return lastNodeBeforeOffset is null or Token { Type: TokenType.NewLine } &&
        //         firstNodeAfterOffset is null or Token { Type: TokenType.NewLine };
        // }


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
