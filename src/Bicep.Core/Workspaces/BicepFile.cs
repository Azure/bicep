// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.Text;

namespace Bicep.Core.Workspaces
{
    public class BicepFile : ISourceFile
    {
        private readonly Lazy<ImmutableDictionary<int, (int, ImmutableArray<string>)>> disableNextLineDiagnosticDirectivesCacheLazy;

        public BicepFile(Uri fileUri, ImmutableArray<int> lineStarts, ProgramSyntax programSyntax)
        {
            FileUri = fileUri;
            LineStarts = lineStarts;
            ProgramSyntax = programSyntax;
            Hierarchy = new SyntaxHierarchy();
            Hierarchy.AddRoot(ProgramSyntax);

            var visitor = new SyntaxTriviaVisitor(LineStarts);
            visitor.Visit(ProgramSyntax);

            disableNextLineDiagnosticDirectivesCacheLazy = new Lazy<ImmutableDictionary<int, (int, ImmutableArray<string>)>>(() => visitor.GetDisableNextLineDiagnosticDirectivesCache());
        }

        public BicepFile(BicepFile original)
        {
            FileUri = original.FileUri;
            LineStarts = original.LineStarts;
            ProgramSyntax = original.ProgramSyntax;
            Hierarchy = original.Hierarchy;

            var visitor = new SyntaxTriviaVisitor(LineStarts);
            visitor.Visit(ProgramSyntax);

            disableNextLineDiagnosticDirectivesCacheLazy = new Lazy<ImmutableDictionary<int, (int, ImmutableArray<string>)>>(() => visitor.GetDisableNextLineDiagnosticDirectivesCache());
        }

        public Uri FileUri { get; }

        public ImmutableArray<int> LineStarts { get; }

        public ProgramSyntax ProgramSyntax { get; }

        public SyntaxHierarchy Hierarchy { get; }

        public ImmutableDictionary<int, (int, ImmutableArray<string>)> DisableNextLineDiagnosticDirectivesCache => disableNextLineDiagnosticDirectivesCacheLazy.Value;

        private class SyntaxTriviaVisitor : SyntaxVisitor
        {
            private ImmutableArray<int> lineStarts;
            private ImmutableDictionary<int, (int, ImmutableArray<string>)>.Builder disableNextLineDiagnosticDirectivesCacheBuilder = ImmutableDictionary.CreateBuilder<int, (int, ImmutableArray<string>)>();

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

                    disableNextLineDiagnosticDirectivesCacheBuilder.Add(line, (syntaxTrivia.Span.GetEndPosition(), codes));
                }
            }

            public ImmutableDictionary<int, (int, ImmutableArray<string>)> GetDisableNextLineDiagnosticDirectivesCache() => disableNextLineDiagnosticDirectivesCacheBuilder.ToImmutable();
        }
    }
}
