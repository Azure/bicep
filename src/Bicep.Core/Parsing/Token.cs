// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Bicep.Core.Syntax;

namespace Bicep.Core.Parsing
{
    [DebuggerDisplay("{Type} = {Text}")]
    public class Token : SyntaxBase
    {
        public Token(TokenType type, TextSpan span, IEnumerable<SyntaxTrivia> leadingTrivia, IEnumerable<SyntaxTrivia> trailingTrivia)
        {
            Type = type;
            Span = span;
            LeadingTrivia = leadingTrivia.ToImmutableArray();
            TrailingTrivia = trailingTrivia.ToImmutableArray();
        }

        public TokenType Type { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitToken(this);

        public override TextSpan Span { get; }

        public virtual string Text => SyntaxFacts.GetText(this.Type) ?? throw new InvalidOperationException($"Unable to get text of token type '{this.Type}'.");

        public ImmutableArray<SyntaxTrivia> LeadingTrivia { get; }

        public ImmutableArray<SyntaxTrivia> TrailingTrivia { get; }

        public IEnumerable<SyntaxTrivia> LeadingComments => this.LeadingTrivia.Where(SyntaxExtensions.IsComment);

        public IEnumerable<SyntaxTrivia> TrailingComments => this.TrailingTrivia.Where(SyntaxExtensions.IsComment);
    }
}
