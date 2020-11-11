// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    /// <summary>
    /// Represents a well-formed identifier.
    /// </summary>
    public class IdentifierSyntax : SyntaxBase
    {
        public IdentifierSyntax(Token identifier)
        {
            AssertTokenType(identifier, nameof(identifier), TokenType.Identifier);
            Assert(string.IsNullOrEmpty(identifier.Text) == false, "Identifier must not be null or empty.");

            this.Child = identifier;
        }

        public IdentifierSyntax(SkippedTriviaSyntax skipped)
        {
            this.Child = skipped;
        }

        public SyntaxBase Child { get; }

        public string IdentifierName
        {
            get
            {
                switch (this.Child)
                {
                    case Token identifier:
                        return identifier.Text;

                    case SkippedTriviaSyntax skipped:
                        return skipped.Elements.Any() ? LanguageConstants.ErrorName : LanguageConstants.MissingName;

                    default:
                        throw new NotImplementedException($"Unexpected child node type '{this.Child.GetType().Name}'.");
                }
            }
        }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitIdentifierSyntax(this);

        public override TextSpan Span => this.Child.Span;

        public bool IsValid => this.Child is Token;
    }
}
