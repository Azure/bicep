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
                GetPrettyPrintedSyntaxWithSurroundingNewLines(prettyPrintContext, leadingNewLine, new ProgramSyntax(syntaxToInsert, SyntaxFactory.EndOfFileToken), null));
        }

        var newProceedingToken = SyntaxFactory.NewlineToken;

        if (existingSyntaxProceedingInsert is not Token existingTokenProceedingInsert)
        {
            if (existingSyntaxProceedingInsert is not SkippedTriviaSyntax)
            {
                return new(
                    existingSyntaxProceedingInsert.Span.ToZeroLengthSpan(), // use a zero length span to insert before the existing proceeding syntax
                    GetPrettyPrintedSyntaxWithSurroundingNewLines(
                        prettyPrintContext, leadingNewLine, new ProgramSyntax(syntaxToInsert, newProceedingToken), newProceedingToken));
            }

            syntaxToInsert = new[] { existingSyntaxProceedingInsert, SyntaxFactory.DoubleNewlineToken }.Concat(syntaxToInsert); // keep it with insertAfterSyntax.
            leadingNewLine = null; // we don't need leading now

            return new(
                existingSyntaxProceedingInsert.Span,
                GetPrettyPrintedSyntaxWithSurroundingNewLines(
                    prettyPrintContext, leadingNewLine, new ProgramSyntax(syntaxToInsert, newProceedingToken), newProceedingToken));
        }

        if (existingTokenProceedingInsert.Type == TokenType.NewLine)
        {
            newProceedingToken = existingTokenProceedingInsert; // keep existing new lines
        }
        else
        {
            syntaxToInsert = new SyntaxBase[] { existingTokenProceedingInsert, SyntaxFactory.DoubleNewlineToken }.Concat(syntaxToInsert); // keep it with insertAfterSyntax.
            leadingNewLine = null; // we don't need leading now
        }

        var additionalProgram = new ProgramSyntax(syntaxToInsert, newProceedingToken);

        return new(
            existingTokenProceedingInsert.Span, // replace our existing proceeding token with pretty printed new syntax + existing token
            GetPrettyPrintedSyntaxWithSurroundingNewLines(prettyPrintContext, leadingNewLine, additionalProgram, newProceedingToken));
    }

    private static string GetPrettyPrintedSyntaxWithSurroundingNewLines(PrettyPrinterV2Context context, Token? leadingToken, ProgramSyntax program, Token? trailingToken)
    {
        var prettyPrintedSyntax = PrettyPrinterV2.Print(program, context).Trim(); // trim any trailing whitespace because we're handling it here

        return $"{leadingToken?.ToString() ?? string.Empty}{prettyPrintedSyntax}{trailingToken?.ToString() ?? string.Empty}";
    }
}
