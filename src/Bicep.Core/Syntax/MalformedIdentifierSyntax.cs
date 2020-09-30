// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    /// <summary>
    /// Represents a malformed or missing identifier.
    /// </summary>
    public class MalformedIdentifierSyntax : IdentifierSyntaxBase
    {
        public MalformedIdentifierSyntax(IEnumerable<Token> tokens, IEnumerable<Diagnostic> diagnostics)
        {
            this.Tokens = tokens.ToImmutableArray();
            if (this.Tokens.Any() == false)
            {
                // we're relying on list contents to figure out the span
                // an empty list would have 0 length but the position would be unknown
                throw new ArgumentException($"At least one token must be specified. To represent a missing identifier, specify a list containing a single token of type {TokenType.Missing}.");
            }

            this.Diagnostics = diagnostics.ToImmutableArray();
        }

        public ImmutableArray<Token> Tokens { get; }

        public ImmutableArray<Diagnostic> Diagnostics { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitMalformedIdentifierSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.Tokens.First(), this.Tokens.Last());

        public override string IdentifierName => this.Tokens.First().Type == TokenType.Missing ? "<missing>" : "<error>";
    }
}