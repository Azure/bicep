// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.Text;

namespace Bicep.Core.Diagnostics
{
    public static class DisableDiagnosticsHelper
    {
        public static DisableNextLineDiagnosticsSyntaxTrivia? GetDisableNextLineDiagnosticStatementFromPreviousLine(ProgramSyntax programSyntax, ImmutableArray<int> lineStarts, int position, out int previousLine)
        {
            (var diagnosticLine, var character) = TextCoordinateConverter.GetPosition(lineStarts, position);
            previousLine = diagnosticLine - 1;

            if (diagnosticLine == 0)
            {
                return null;
            }

            int lineStart = lineStarts[diagnosticLine];
            int previousLineStart = lineStarts[diagnosticLine - 1];

            for (int i = previousLineStart; i < lineStart; i++)
            {
                var syntax = programSyntax.Children.FirstOrDefault(x => x.Span.Contains(position));

                if (syntax is null)
                {
                    return null;
                }

                var syntaxTrivia = syntax.TryFindMostSpecificTriviaInclusive(i, current => true);

                if (syntaxTrivia is not null && syntaxTrivia.Type == SyntaxTriviaType.DisableNextLineDirective)
                {
                    return syntaxTrivia as DisableNextLineDiagnosticsSyntaxTrivia;
                }

                var previousChild = programSyntax.Children.FirstOrDefault(x => x.Span.Contains(syntax.Span.Position - 1));

                if (previousChild is not null && previousChild is Token token)
                {
                    syntaxTrivia = token.LeadingTrivia.FirstOrDefault(x => x.Type == SyntaxTriviaType.DisableNextLineDirective);

                    if (syntaxTrivia is not null &&
                        TextCoordinateConverter.GetPosition(lineStarts, syntaxTrivia.Span.Position).line + 1 == diagnosticLine)
                    {
                        return syntaxTrivia as DisableNextLineDiagnosticsSyntaxTrivia;
                    }
                }
            }

            return null;
        }
    }
}
