// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Parsing;
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

    public static CodeFix GetCodeFixForMissingBicepExtensionConfigAssignments(ProgramSyntax program, IReadOnlyList<(string Alias, ObjectLikeType ExpectedConfigType)> missingExtAliasesWithExpectedType)
    {
        // Create the new syntax
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

        // Determine spacing above.
        newSyntaxList.Insert(0, SyntaxFactory.DoubleNewlineToken);

        // Determine spacing below.
        var numExistingProceedingNewLines = existingSyntaxProceedingInsert is Token { Type: TokenType.NewLine } proceedingNewLineToken
            ? StringUtils.CountNewlines(proceedingNewLineToken.Text)
            : object.ReferenceEquals(insertAfterSyntax, program.EndOfFile)
                ? -1
                : 0;

        var newProceedingSyntax = numExistingProceedingNewLines switch
        {
            -1 => SyntaxFactory.EndOfFileToken,
            0 => SyntaxFactory.DoubleNewlineToken,
            _ => SyntaxFactory.NewlineToken,
        };

        // Make the code replacement.
        var additionalProgram = new ProgramSyntax(newSyntaxList, newProceedingSyntax);

        return new(
            "Insert missing required extension configuration assignments",
            true,
            CodeFixKind.QuickFix,
            new(insertAfterSyntax.Span, additionalProgram.ToString()));
    }
}
