// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.CodeAction.Fixes;

public class MultilineStringCodeFixProvider : ICodeFixProvider
{
    public const string Title = "Convert to multi-line string";

    public IEnumerable<CodeFix> GetFixes(SemanticModel semanticModel, IReadOnlyList<SyntaxBase> matchingNodes)
    {
        // we only want to suggest this replacement if the newline sequence matches that of the file
        var newlineSequence = semanticModel.Configuration.Formatting.Data.NewlineKind.ToEscapeSequence();

        foreach (var stringSyntax in matchingNodes.OfType<StringSyntax>())
        {
            if (stringSyntax.IsVerbatimString() || // it's already a multi-line string
                stringSyntax.TryGetLiteralValue() is not { } stringValue || // it's an interpolated string
                !stringValue.Contains(newlineSequence) || // it's non-interpolated, but doesn't have newlines
                stringValue.Contains("'''")) // there's no way to escape this sequence in a multiline string 
            {
                continue;
            }

            var stringToken = stringSyntax.StringTokens.Single();
            var multilineString = SyntaxFactory.CreateMultilineString(stringValue, stringToken.LeadingTrivia, stringToken.TrailingTrivia);

            yield return new CodeFix(
                Title,
                false,
                CodeFixKind.Refactor,
                new(stringSyntax.Span, multilineString.ToString()));
        }
    }
}
