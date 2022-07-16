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


            //TODO: Add top level declaration completions
            var kind = ConvertFlag(IsParamAssignmentContext(matchingNodes, offset), ParamsCompletionContextKind.ParamAssignment);

            return new(kind, replacementRange);
        }

        private static bool IsParamAssignmentContext(List<SyntaxBase> matchingNodes, int offset)
        {
            if(matchingNodes.Count >=1)
            {
                SyntaxBase lastNode = matchingNodes[^1];
                if (lastNode is ParameterAssignmentSyntax assignmentSyntax )
                {
                    //TODO: do completions with partially written identifier names
                    return assignmentSyntax.Name.IdentifierName == LanguageConstants.MissingName;
                }
            }

            return false;
        }

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
