// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Text;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrint;
using Bicep.Core.Syntax;

namespace Bicep.Core.Navigation
{
    public static class SyntaxBaseExtensions
    {
        public static SyntaxBase? TryFindMostSpecificNodeInclusive(this SyntaxBase root, int offset, Func<SyntaxBase, bool> predicate) => 
            TryFindMostSpecificNodeInternal(root, offset, predicate, inclusive: true);

        public static SyntaxBase? TryFindMostSpecificNodeExclusive(this SyntaxBase root, int offset, Func<SyntaxBase, bool> predicate) => 
            TryFindMostSpecificNodeInternal(root, offset, predicate, inclusive: false);

        private static SyntaxBase? TryFindMostSpecificNodeInternal(SyntaxBase root, int offset, Func<SyntaxBase, bool> predicate, bool inclusive)
        {
            var visitor = new NavigationSearchVisitor(offset, predicate, inclusive);
            visitor.Visit(root);

            return visitor.Result;
        }

        private sealed class NavigationSearchVisitor : SyntaxVisitor
        {
            private readonly int offset;
            private readonly Func<SyntaxBase, bool> predicate;
            private readonly bool inclusive;

            public NavigationSearchVisitor(int offset, Func<SyntaxBase, bool> predicate, bool inclusive)
            {
                this.offset = offset;
                this.predicate = predicate;
                this.inclusive = inclusive;
            }

            public SyntaxBase? Result { get; private set; }

            protected override void VisitInternal(SyntaxBase node)
            {
                // check if offset is inside the node's span
                if (CheckNodeContainsOffset(node))
                {
                    // the node span contains the offset
                    // check the predicate
                    if (this.predicate(node))
                    {
                        // store the potential result
                        this.Result = node;
                    }

                    // visiting the children may find a more specific node
                    base.VisitInternal(node);
                }

                // the offset is outside of the node span
                // there's no point to visit the children
            }

            private bool CheckNodeContainsOffset(SyntaxBase node) => this.inclusive
                    ? node.Span.ContainsInclusive(offset)
                    : node.Span.Contains(offset);
        }

        /// <summary>
        /// Generate a string that represents this Syntax element
        /// </summary>
        /// <param name="syntax"></param>
        /// <param name="indent"></param>
        /// <returns></returns>
        public static string ToText(this SyntaxBase syntax, string indent="")
        {
            var sb = new StringBuilder();
            var documentBuildVisitor = new DocumentBuildVisitor();
            var document = documentBuildVisitor.BuildDocument(syntax);
            document.Layout(sb, indent, System.Environment.NewLine);
            return sb.ToString();
        }

        /// <summary>
        /// Generate a string that represents this Syntax element.
        /// </summary>
        public static string ToTextPreserveFormatting(this SyntaxBase syntax)
        {
            var sb = new StringBuilder();
            var printVisitor = new PrintVisitor(sb);
            printVisitor.Visit(syntax);
            return sb.ToString();
        }

        private class PrintVisitor : SyntaxVisitor
        {
            private readonly StringBuilder buffer;

            public PrintVisitor(StringBuilder buffer)
            {
                this.buffer = buffer;
            }

            public override void VisitToken(Token token)
            {
                WriteTrivia(token.LeadingTrivia);
                buffer.Append(token.Text);
                WriteTrivia(token.TrailingTrivia);
            }

            private void WriteTrivia(IEnumerable<SyntaxTrivia> triviaList)
            {
                foreach (var trivia in triviaList)
                {
                    buffer.Append(trivia.Text);
                }
            }
        }
    }
}

