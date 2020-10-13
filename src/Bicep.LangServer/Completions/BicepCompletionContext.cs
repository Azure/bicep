// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Navigation;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.LanguageServer.Completions
{
    public class BicepCompletionContext
    {
        public BicepCompletionContext(BicepCompletionContextKind kind)
        {
            this.Kind = kind;
        }

        public BicepCompletionContextKind Kind { get; }

        public static BicepCompletionContext Create(ProgramSyntax syntax, int offset)
        {
            var matchingNodes = FindNodesMatchingOffset(syntax, offset);
            var kind = ConvertFlag(IsDeclarationStartContext(matchingNodes, offset), BicepCompletionContextKind.DeclarationStart) |
                       GetDeclarationTypeFlags(matchingNodes, offset);

            return new BicepCompletionContext(kind);
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
            }

            return BicepCompletionContextKind.None;
        }

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
                        // 2. the token is a newline and offset is 0 (file has content, but cursor is at the beginning)
                        // 3. the token is a newline and offset is past the beginning position of the new line
                        // (this prevents the end of a declaration from being considered a declaration context)
                        return token.Type == TokenType.EndOfFile || 
                               token.Type == TokenType.NewLine && (offset > token.Span.Position || offset == 0);
                    
                    case SkippedTriviaSyntax _:
                        // we are in a partial declaration
                        // if the token at current position is an identifier, assume declaration context
                        // (completions will be filtered by the text that is present, so we don't have to be 100% right)
                        return token.Type == TokenType.Identifier;

                    case IDeclarationSyntax declaration:
                        // we are in a fully or partially parsed declaration
                        // whether we are in a declaration context depends on whether our offset is within the keyword token
                        // (by using exclusive span containment, the cursor position at the end of a keyword token 
                        // result counts as being outside of the declaration context)
                        return declaration.Keyword.Span.Contains(offset);
                }
            }

            return false;
        }
    }
}
