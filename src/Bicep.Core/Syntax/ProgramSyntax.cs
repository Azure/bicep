// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class ProgramSyntax(IEnumerable<SyntaxBase> children, Token endOfFile) : SyntaxBase
    {
        public ImmutableArray<SyntaxBase> Children { get; } = children.ToImmutableArray();

        public Token EndOfFile { get; } = endOfFile;

        public override void Accept(ISyntaxVisitor visitor)
            => visitor.VisitProgramSyntax(this);

        public override TextSpan Span => TextSpan.Between(TextSpan.TextDocumentStart, this.EndOfFile);

        // TODO: Should we have a DeclarationSyntax abstract class?
        public IEnumerable<SyntaxBase> Declarations => this.Children.Where(c => c is ITopLevelDeclarationSyntax);
    }
}
