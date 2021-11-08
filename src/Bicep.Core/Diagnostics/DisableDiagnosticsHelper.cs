// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using Bicep.Core.Text;

namespace Bicep.Core.Diagnostics
{
    public static class DisableDiagnosticsHelper
    {
        public static SyntaxTrivia? GetDisableNextLineDiagnosticStatementFromPreviousLine(ProgramSyntax programSyntax, ImmutableArray<int> lineStarts, int position, out int previousLine)
        {
            (var line, var character) = TextCoordinateConverter.GetPosition(lineStarts, position);
            previousLine = line - 1;

            if (line == 0)
            {
                return null;
            }

            int lineStart = lineStarts[line];
            int previousLineStart = lineStarts[line - 1];

            for (int i = previousLineStart; i < lineStart; i++)
            {
                var syntaxTrivia = programSyntax.TryFindMostSpecificTriviaInclusive(i, current => true);

                if (syntaxTrivia is not null && syntaxTrivia.Type == SyntaxTriviaType.DisableNextLineStatement)
                {
                    return syntaxTrivia;
                }
            }

            return null;
        }
    }
}
