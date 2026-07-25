// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

/// <summary>
/// Helpers for reasoning about the lines occupied by statements and
/// their leading comments.
/// </summary>
public static class StatementLineHelper
{
    /// <summary>
    /// Checks if a line has content (non-whitespace tokens) and/or comments.
    /// </summary>
    public static (bool hasContent, bool hasComments) CheckLineContent(
        IReadOnlyList<int> lineStarts,
        SyntaxBase programSyntax,
        int line)
    {
        var lineSpan = TextCoordinateConverter.GetLineSpan(lineStarts, programSyntax.GetEndPosition(), line);
        var visitor = new CheckContentVisitor(lineSpan);
        programSyntax.Accept(visitor);
        return (visitor.HasContent, visitor.HasComments);
    }

    /// <summary>
    /// Gets the first line of a statement, accounting for any preceding
    /// comment lines that belong to the statement.
    /// </summary>
    public static int GetFirstLineOfStatementIncludingComments(
        IReadOnlyList<int> lineStarts,
        ProgramSyntax programSyntax,
        StatementSyntax statementSyntax)
    {
        // Includes trivia but not comments
        var statementStartLine = TextCoordinateConverter.GetPosition(lineStarts, statementSyntax.Span.Position).line;

        for (int line = statementStartLine; line >= 1; --line)
        {
            var (hasContent, hasComments) = CheckLineContent(lineStarts, programSyntax, line - 1);
            if (hasComments && !hasContent)
            {
                continue;
            }

            return line;
        }

        return 0;
    }

    /// <summary>
    /// Visitor to check if a line has content and/or comments.
    /// </summary>
    private sealed class CheckContentVisitor : CstVisitor
    {
        private readonly TextSpan span;

        public CheckContentVisitor(TextSpan span)
        {
            this.span = span;
        }

        public bool HasContent { get; private set; }
        public bool HasComments { get; private set; }

        public override void VisitSyntaxTrivia(SyntaxTrivia syntaxTrivia)
        {
            if (!HasComments && TextSpan.AreOverlapping(span, syntaxTrivia.Span))
            {
                if (syntaxTrivia.Type == SyntaxTriviaType.SingleLineComment || syntaxTrivia.Type == SyntaxTriviaType.MultiLineComment)
                {
                    HasComments = true;
                }
            }
        }

        protected override void VisitInternal(SyntaxBase node)
        {
            if ((HasComments && HasContent) || !TextSpan.AreOverlapping(span, node.GetSpanIncludingTrivia()))
            {
                return;
            }

            if (!HasContent && node is Token token && token.Text.Trim().Length > 0)
            {
                HasContent = true;
            }

            base.VisitInternal(node);
        }
    }
}
