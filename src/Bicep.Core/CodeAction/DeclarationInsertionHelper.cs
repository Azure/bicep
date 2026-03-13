// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Text;

namespace Bicep.Core.CodeAction;

/// <summary>
/// Shared helpers for finding insertion points for new top-level declarations
/// and determining whether to surround them with blank lines.
///
/// This encapsulates the logic used by refactorings and code fixes to place
/// new declarations near existing declarations of the same type while
/// respecting existing blank-line patterns.
/// </summary>
public static class DeclarationInsertionHelper
{
    /// <summary>
    /// Finds a suitable location to create a new declaration, putting it near
    /// existing declarations of the same type above the anchor offset when possible.
    /// </summary>
    /// <param name="sourceFile">The source file.</param>
    /// <param name="extractionStatement">The statement that contains the expression being extracted or fixed.</param>
    /// <param name="extractionOffset">The offset of the expression being extracted or fixed.</param>
    /// <param name="declarationSyntaxType">The declaration type to insert (e.g., <see cref="ParameterDeclarationSyntax"/>).</param>
    /// <returns>
    /// A tuple of:
    /// <list type="bullet">
    /// <item><description><c>offset</c>: The text offset where the new declaration should be inserted.</description></item>
    /// <item><description><c>insertNewlineBefore</c>: Whether to insert a blank line before the new declaration.</description></item>
    /// <item><description><c>insertNewlineAfter</c>: Whether to insert a blank line after the new declaration.</description></item>
    /// </list>
    /// </returns>
    public static (int offset, bool insertNewlineBefore, bool insertNewlineAfter) FindOffsetToInsertNewDeclaration(
        BicepSourceFile sourceFile,
        StatementSyntax extractionStatement,
        int extractionOffset,
        Type declarationSyntaxType)
    {
        var lineStarts = sourceFile.LineStarts;

        var extractionLine = TextCoordinateConverter.GetPosition(lineStarts, extractionOffset).line;

        var existingDeclarationStatement = sourceFile.ProgramSyntax.Children.OfType<StatementSyntax>()
            .Where(s => s.GetType() == declarationSyntaxType)
            .Where(s => s.Span.Position < extractionOffset)
            .OrderByDescending(s => s.Span.Position)
            .FirstOrDefault();

        if (existingDeclarationStatement is { })
        {
            // Insert after the existing declaration of the same type
            int existingDeclarationLine = TextCoordinateConverter.GetPosition(lineStarts, existingDeclarationStatement.GetEndPosition()).line;
            var insertionLine = existingDeclarationLine + 1;

            // Is there a blank line around this existing statement (excluding its leading nodes/comments)?
            // If so, mirror that pattern for the new declaration.
            var (addBlankBefore, addBlankAfter) = ShouldAddBlankLines(sourceFile, existingDeclarationStatement);
            return (TextCoordinateConverter.GetOffset(lineStarts, insertionLine, 0), addBlankBefore, addBlankAfter);
        }

        // If no existing declarations of the desired type, insert right before the statement containing the extraction expression
        var extractionStatementFirstLine = GetFirstLineOfStatementIncludingComments(sourceFile, extractionStatement);
        var extractionLineHasNewlineBefore = IsFirstLine(lineStarts, extractionStatementFirstLine) ||
            CheckLineContent(lineStarts, sourceFile.ProgramSyntax, extractionStatementFirstLine - 1).IsEmpty;

        return (TextCoordinateConverter.GetOffset(lineStarts, extractionLine, 0), false, extractionLineHasNewlineBefore);
    }

    private readonly record struct LineContentType(bool HasContent, bool HasComments)
    {
        public bool IsEmpty => !HasContent && !HasComments;
        public bool IsNotEmpty => !IsEmpty;
    }

    private static bool IsFirstLine(IReadOnlyList<int> lineStarts, int line) => line == 0;

    private static bool IsLastLine(IReadOnlyList<int> lineStarts, int line) => line >= lineStarts.Count - 1;

    private static LineContentType CheckLineContent(IReadOnlyList<int> lineStarts, SyntaxBase programSyntax, int line)
    {
        var (hasContent, hasComments) = StatementLineHelper.CheckLineContent(lineStarts, programSyntax, line);
        return new LineContentType(hasContent, hasComments);
    }

    private static int GetFirstLineOfStatementIncludingComments(BicepSourceFile sourceFile, StatementSyntax statementSyntax) =>
        StatementLineHelper.GetFirstLineOfStatementIncludingComments(sourceFile.LineStarts, sourceFile.ProgramSyntax, statementSyntax);

    private static int GetLastLineOfStatement(BicepSourceFile sourceFile, StatementSyntax statementSyntax) =>
        TextCoordinateConverter.GetPosition(sourceFile.LineStarts, statementSyntax.GetEndPosition()).line;

    private static (bool addBefore, bool addAfter) ShouldAddBlankLines(BicepSourceFile sourceFile, StatementSyntax statementSyntax)
    {
        bool addBefore = false;
        bool addAfter = false;

        var lineStarts = sourceFile.LineStarts;
        var startingLine = GetFirstLineOfStatementIncludingComments(sourceFile, statementSyntax);
        var endingLine = GetLastLineOfStatement(sourceFile, statementSyntax);

        bool? hasBlankLineBefore = IsFirstLine(lineStarts, startingLine) ? null : CheckLineContent(lineStarts, sourceFile.ProgramSyntax, startingLine - 1).IsEmpty;
        bool? hasBlankLineAfter = IsLastLine(lineStarts, endingLine) ? null : CheckLineContent(lineStarts, sourceFile.ProgramSyntax, endingLine + 1).IsEmpty;

        bool existingDeclarationUsesBlankLines = hasBlankLineBefore ?? hasBlankLineAfter ?? true;
        addBefore = existingDeclarationUsesBlankLines;
        addAfter = hasBlankLineAfter ?? true;

        // Don't add another after if there's already one
        addAfter = addAfter && (IsLastLine(lineStarts, endingLine) || CheckLineContent(lineStarts, sourceFile.ProgramSyntax, endingLine + 1).IsNotEmpty);

        return (addBefore, addAfter);
    }
}
