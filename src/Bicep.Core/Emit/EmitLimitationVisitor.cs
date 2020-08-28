// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Emit
{
    /// <summary>
    /// Visitor responsible for detecting issue caused by IL limitations
    /// </summary>
    public class EmitLimitationVisitor : SyntaxVisitor
    {
        private readonly IList<Diagnostic> diagnostics;

        private readonly Stack<VisitorState> stack = new Stack<VisitorState>();

        public EmitLimitationVisitor(IList<Diagnostic> diagnostics)
        {
            this.diagnostics = diagnostics;
        }

        protected override void VisitInternal(SyntaxBase node)
        {
            var kind = (node as IExpressionSyntax)?.ExpressionKind ?? ExpressionKind.None;
            var newState = GetNewState(kind);

            if (newState == VisitorState.SecondOperatorChainInsideObjectLiteralInsideFirstOperatorChain)
            {
                this.diagnostics.Add(DiagnosticBuilder.ForPosition(node.Span).ExpressionContainsObjectLiteralContainingOtherExpressions());
                
                // we won't suddenly regain the ability to compile the expression by visiting children,
                // so let's not continue deeper into the tree
                return;
            }

            this.stack.Push(newState);

            base.VisitInternal(node);

            this.stack.Pop();
        }

        private VisitorState GetNewState(ExpressionKind kind)
        {
            var current = this.stack.Count <= 0 ? VisitorState.None : this.stack.Peek();

            switch (current)
            {
                case VisitorState.None:
                    switch (kind)
                    {
                        case ExpressionKind.None:
                        case ExpressionKind.SimpleLiteral:
                        case ExpressionKind.ArrayLiteral:
                        case ExpressionKind.ObjectLiteral:
                            // simple literals are not interesting for this
                            // outer complex literals are also fine
                            return VisitorState.None;

                        case ExpressionKind.Operator:
                            return VisitorState.FirstOperatorChain;
                    }

                    break;

                case VisitorState.FirstOperatorChain:
                    switch (kind)
                    {
                        case ExpressionKind.None:
                        case ExpressionKind.Operator:
                        case ExpressionKind.SimpleLiteral:
                        case ExpressionKind.ArrayLiteral:
                            // operator doesn't change the state since we're already in the operator chain
                            // simple literal also doesn't change anything
                            // arrays also don't change anything
                            return VisitorState.FirstOperatorChain;

                        case ExpressionKind.ObjectLiteral:
                            // update state
                            return VisitorState.ObjectLiteralInsideFirstOperatorChain;
                    }

                    break;

                case VisitorState.ObjectLiteralInsideFirstOperatorChain:
                    switch (kind)
                    {
                        case ExpressionKind.None:
                        case ExpressionKind.ArrayLiteral:
                        case ExpressionKind.ObjectLiteral:
                        case ExpressionKind.SimpleLiteral:
                            // literals don't change the state
                            return VisitorState.ObjectLiteralInsideFirstOperatorChain;

                        case ExpressionKind.Operator:
                            // update state
                            return VisitorState.SecondOperatorChainInsideObjectLiteralInsideFirstOperatorChain;
                    }

                    break;

                case VisitorState.SecondOperatorChainInsideObjectLiteralInsideFirstOperatorChain:
                    // once we enter this state, we cannot leave it
                    return VisitorState.SecondOperatorChainInsideObjectLiteralInsideFirstOperatorChain;
            }

            throw new NotImplementedException($"Unexpected current state '{current}' and/or expression kind '{kind}'.");
        }

        private enum VisitorState
        {
            /// <summary>
            /// Initial state
            /// </summary>
            None,

            /// <summary>
            /// The first operator chain has begun.
            /// </summary>
            FirstOperatorChain,

            /// <summary>
            /// An object literal was detected inside the first operator chain.
            /// </summary>
            ObjectLiteralInsideFirstOperatorChain,

            /// <summary>
            /// A second operator chain was detected inside an object literal that is also inside the first operator chain.
            /// </summary>
            SecondOperatorChainInsideObjectLiteralInsideFirstOperatorChain
        }
    }
}

