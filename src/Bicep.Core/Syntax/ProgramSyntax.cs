// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Parsing;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;

namespace Bicep.Core.Syntax
{
    public class ProgramSyntax : SyntaxBase
    {
        public ProgramSyntax(IEnumerable<SyntaxBase> children, Token endOfFile)
            : this(children, endOfFile, EmptyDiagnosticLookup.Instance, EmptyDiagnosticLookup.Instance)
        {
        }

        public ProgramSyntax(IEnumerable<SyntaxBase> children, Token endOfFile, IDiagnosticLookup lexingErrorLookup, IDiagnosticLookup parsingErrorLookup)
        {
            this.Children = children.ToImmutableArray();
            this.EndOfFile = endOfFile;
            this.LexingErrorLookup = lexingErrorLookup;
            this.ParsingErrorLookup = parsingErrorLookup;
        }

        public ImmutableArray<SyntaxBase> Children { get; }

        public Token EndOfFile { get; }

        public IDiagnosticLookup LexingErrorLookup { get; }

        public IDiagnosticLookup ParsingErrorLookup { get; }

        public override void Accept(ISyntaxVisitor visitor)
            => visitor.VisitProgramSyntax(this);

        public override TextSpan Span => TextSpan.Between(TextSpan.TextDocumentStart, this.EndOfFile);

        // TODO: Should we have a DeclarationSyntax abstract class?
        public IEnumerable<SyntaxBase> Declarations => this.Children.Where(c => c is ITopLevelNamedDeclarationSyntax);
    }
}
