// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
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
}
