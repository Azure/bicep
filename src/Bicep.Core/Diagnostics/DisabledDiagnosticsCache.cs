// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.Text;

namespace Bicep.Core.Diagnostics
{
    public class DisabledDiagnosticsCache
    {
        private readonly ImmutableArray<int> lineStarts;
        private readonly ProgramSyntax programSyntax;
        private readonly Lazy<ImmutableDictionary<int, DisableNextLineDirectiveEndPositionAndCodes>> disableNextLineDiagnosticDirectivesCacheLazy;

        public DisabledDiagnosticsCache(ProgramSyntax programSyntax, ImmutableArray<int> lineStarts)
        {
            this.programSyntax = programSyntax;
            this.lineStarts = lineStarts;

            disableNextLineDiagnosticDirectivesCacheLazy = new Lazy<ImmutableDictionary<int, DisableNextLineDirectiveEndPositionAndCodes>>(() => GetDisableNextLineDiagnosticDirectivesCache());
        }

        public DisableNextLineDirectiveEndPositionAndCodes? TryGetDisabledNextLineDirective(int lineNumber)
            => disableNextLineDiagnosticDirectivesCacheLazy.Value.TryGetValue(lineNumber);

        public record DisableNextLineDirectiveEndPositionAndCodes(int endPosition, ImmutableArray<string> diagnosticCodes);

        public ImmutableDictionary<int, DisableNextLineDirectiveEndPositionAndCodes> GetDisableNextLineDiagnosticDirectivesCache()
        {
            var visitor = new SyntaxTriviaVisitor(lineStarts);
            visitor.Visit(programSyntax);

            return visitor.GetDisableNextLineDiagnosticDirectivesCache();
        }

        private class SyntaxTriviaVisitor : AstVisitor
        {
            private ImmutableArray<int> lineStarts;
            private ImmutableDictionary<int, DisableNextLineDirectiveEndPositionAndCodes>.Builder disableNextLineDiagnosticDirectivesCacheBuilder = ImmutableDictionary.CreateBuilder<int, DisableNextLineDirectiveEndPositionAndCodes>();

            public SyntaxTriviaVisitor(ImmutableArray<int> lineStarts)
            {
                this.lineStarts = lineStarts;
            }

            public override void VisitSyntaxTrivia(SyntaxTrivia syntaxTrivia)
            {
                if (syntaxTrivia is DisableNextLineDiagnosticsSyntaxTrivia disableNextLineDiagnosticsSyntaxTrivia)
                {
                    var codes = disableNextLineDiagnosticsSyntaxTrivia.DiagnosticCodes.Select(x => x.Text).ToImmutableArray();
                    (int line, _) = TextCoordinateConverter.GetPosition(lineStarts, syntaxTrivia.Span.Position);
                    DisableNextLineDirectiveEndPositionAndCodes disableNextLineDirectiveEndPosAndCodes = new(syntaxTrivia.Span.GetEndPosition(), codes);
                    disableNextLineDiagnosticDirectivesCacheBuilder.Add(line, disableNextLineDirectiveEndPosAndCodes);
                }
            }

            public ImmutableDictionary<int, DisableNextLineDirectiveEndPositionAndCodes> GetDisableNextLineDiagnosticDirectivesCache() => disableNextLineDiagnosticDirectivesCacheBuilder.ToImmutable();
        }
    }
}
