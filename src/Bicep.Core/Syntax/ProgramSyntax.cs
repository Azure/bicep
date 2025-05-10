// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class ProgramSyntax : SyntaxBase
    {
        public ProgramSyntax(IEnumerable<SyntaxBase> children, Token endOfFile)
        {
            this.Children = [.. children];
            this.EndOfFile = endOfFile;
        }

        public ImmutableArray<SyntaxBase> Children { get; }

        public Token EndOfFile { get; }

        public override void Accept(ISyntaxVisitor visitor)
            => visitor.VisitProgramSyntax(this);

        public override TextSpan Span => TextSpan.Between(TextSpan.TextDocumentStart, this.EndOfFile);

        // TODO: Should we have a DeclarationSyntax abstract class?
        public IEnumerable<SyntaxBase> Declarations => this.Children.Where(c => c is ITopLevelDeclarationSyntax);
    }
}
