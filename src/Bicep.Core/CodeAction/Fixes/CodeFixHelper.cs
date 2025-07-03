// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.CodeAction.Fixes;

public static class CodeFixHelper
{
    public static CodeFix GetCodeFixForMissingBicepParams(ProgramSyntax program, ImmutableArray<string> missingRequiredParams)
    {
        var terminatingNewlines = program.Children.LastOrDefault() is Token { Type: TokenType.NewLine } newLineToken ?
            StringUtils.CountNewlines(newLineToken.Text) :
            0;

        var newSyntaxList = new List<SyntaxBase>();

        var precedingNewlineSyntax = terminatingNewlines switch
        {
            0 => SyntaxFactory.DoubleNewlineToken,
            1 => SyntaxFactory.NewlineToken,
            _ => null,
        };

        if (precedingNewlineSyntax is { })
        {
            newSyntaxList.Add(precedingNewlineSyntax);
        }

        for (var i = 0; i < missingRequiredParams.Length; i++)
        {
            var paramSyntax = SyntaxFactory.CreateParameterAssignmentSyntax(missingRequiredParams[i], SyntaxFactory.CreateEmptySyntaxWithComment("TODO"));
            newSyntaxList.Add(paramSyntax);

            var isLastEntry = i == missingRequiredParams.Length - 1;
            var trailingNewlineSyntax = isLastEntry switch
            {
                false => SyntaxFactory.DoubleNewlineToken,
                true => SyntaxFactory.NewlineToken,
            };

            newSyntaxList.Add(trailingNewlineSyntax);
        }

        var additionalProgram = new ProgramSyntax(newSyntaxList, SyntaxFactory.EndOfFileToken);

        return new(
            "Insert missing required parameters",
            true,
            CodeFixKind.QuickFix,
            new(program.EndOfFile.Span, additionalProgram.ToString()));
    }

    public static CodeFix GetCodeFixForMissingBicepExtensionConfigAssignments(ProgramSyntax program, BicepSourceFile file, IReadOnlyList<(string Alias, ObjectLikeType ExpectedConfigType)> missingExtAliasesWithExpectedType)
    {
        // Create the new syntax. Start with a double new line because this is a top-level insertion.
        var leadingNewLine = SyntaxFactory.DoubleNewlineToken;
        var newSyntaxList = new List<SyntaxBase>(missingExtAliasesWithExpectedType.Count * 2); // * 2 for new lines per new syntax

        for (var i = 0; i < missingExtAliasesWithExpectedType.Count; i++)
        {
            var (alias, expectedConfigType) = missingExtAliasesWithExpectedType[i];

            var assignmentObjSyntaxWithRequiredProperties = expectedConfigType switch
            {
                ObjectType assignObjType
                    => SyntaxFactory.CreateObject(
                        assignObjType.Properties
                            .Where(p => p.Value.Flags.HasFlag(TypePropertyFlags.Required))
                            .OrderBy(p => p.Key)
                            .Select(p => SyntaxFactory.CreateObjectProperty(p.Key, SyntaxFactory.EmptySkippedTrivia))),
                DiscriminatedObjectType discrimObjType
                    => SyntaxFactory.CreateObject([SyntaxFactory.CreateObjectProperty(discrimObjType.DiscriminatorKey, SyntaxFactory.EmptySkippedTrivia)]),
                _ => SyntaxFactory.CreateObject([])
            };

            var extConfigAssignmentSyntax = SyntaxFactory.CreateExtensionConfigAssignmentSyntax(
                alias,
                assignmentObjSyntaxWithRequiredProperties);

            newSyntaxList.Add(extConfigAssignmentSyntax);

            if (i != missingExtAliasesWithExpectedType.Count - 1)
            {
                newSyntaxList.Add(SyntaxFactory.DoubleNewlineToken);
            }
        }

        // Determine where the new syntax should go. Find the syntax we'll insert after and the proceeding syntax of it.
        SyntaxBase insertAfterSyntax = program.EndOfFile;
        SyntaxBase? existingSyntaxProceedingInsert = null;

        for (var i = program.Children.Length - 1; i >= 0; --i)
        {
            var currentSyntax = program.Children[i];

            if (currentSyntax is ExtensionConfigAssignmentSyntax) // Use last existing config assignment, if any.
            {
                insertAfterSyntax = currentSyntax;
                existingSyntaxProceedingInsert = program.Children.ElementAtOrDefault(i + 1);

                break;
            }
            else if (currentSyntax is UsingDeclarationSyntax) // otherwise, insert near the top of file by default if no existing site. Extension configs are more foundational for the deployment.
            {
                insertAfterSyntax = currentSyntax;
                existingSyntaxProceedingInsert = program.Children.ElementAtOrDefault(i + 1);
            }
        }

        var prettyPrintContext = PrettyPrinterV2Context.Create(file.Configuration.Formatting.Data, file.LexingErrorLookup, file.ParsingErrorLookup);
        var codeReplacement = CreatePrettyPrintedInsertionCodeReplacement(program, prettyPrintContext, insertAfterSyntax, leadingNewLine, newSyntaxList, existingSyntaxProceedingInsert);

        return new(
            "Insert missing required extension configuration assignments",
            true,
            CodeFixKind.QuickFix,
            codeReplacement);
    }

    private static CodeReplacement CreatePrettyPrintedInsertionCodeReplacement(ProgramSyntax existingProgram, PrettyPrinterV2Context prettyPrintContext, SyntaxBase insertAfterSyntax, Token? leadingNewLine, IEnumerable<SyntaxBase> syntaxToInsert, SyntaxBase? existingSyntaxProceedingInsert)
    {
        if (existingSyntaxProceedingInsert is null || existingSyntaxProceedingInsert is Token { Type: TokenType.EndOfFile } || object.ReferenceEquals(insertAfterSyntax, existingProgram.EndOfFile))
        {
            // Replace the EOF with new syntax + EOF.
            return new(
                existingProgram.EndOfFile.Span,
                GetPrettyPrintedSyntaxWithSurroundingNewLines(new ProgramSyntax(syntaxToInsert, SyntaxFactory.EndOfFileToken), prettyPrintContext, leadingNewLine, null));
        }

        var newProceedingToken = SyntaxFactory.NewlineToken;

        if (existingSyntaxProceedingInsert is not Token existingTokenProceedingInsert)
        {
            // We have a non-token after we insert. Insert a new line after our new syntax but do not replace the existing syntax span.
            return new(
                existingSyntaxProceedingInsert.Span.ToZeroLengthSpan(), // use a zero length span to insert before the existing proceeding syntax
                GetPrettyPrintedSyntaxWithSurroundingNewLines(
                    new ProgramSyntax(syntaxToInsert, newProceedingToken), prettyPrintContext, leadingNewLine, newProceedingToken));
        }

        // We have an existing proceeding token. If it's not a new line, add two, one for the new syntax and 1 for a blank line before the existing token.
        if (existingTokenProceedingInsert is not { Type: TokenType.NewLine })
        {
            syntaxToInsert = syntaxToInsert.Append(SyntaxFactory.DoubleNewlineToken);
        }
        else
        {
            newProceedingToken = existingTokenProceedingInsert;
        }

        var additionalProgram = new ProgramSyntax(syntaxToInsert, newProceedingToken);

        return new(
            existingTokenProceedingInsert.Span, // replace our existing proceeding token with pretty printed new syntax + existing token
            GetPrettyPrintedSyntaxWithSurroundingNewLines(additionalProgram, prettyPrintContext, leadingNewLine, newProceedingToken));
    }

    private static string GetPrettyPrintedSyntaxWithSurroundingNewLines(ProgramSyntax program, PrettyPrinterV2Context context, Token? leadingToken, Token? trailingToken)
    {
        var prettyPrintedSyntax = PrettyPrinterV2.Print(program, context).Trim(); // trim any trailing whitespace because we're handling it here

        return $"{leadingToken?.ToString() ?? string.Empty}{prettyPrintedSyntax}{trailingToken?.ToString() ?? string.Empty}";
    }
}
